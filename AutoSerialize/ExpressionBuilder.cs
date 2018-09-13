using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace AutoSerialize
{
    public static class ExpressionBuilder
    {
        public static Action<Stream, Object> BuildWrite(Type t, IServiceProvider provider)
        {
            Log($"Building Write of {t.Name}");
            ParameterExpression inputStream = Expression.Parameter(typeof(Stream), "inputStream");
            ParameterExpression inputObject = Expression.Parameter(typeof(Object), "inputObject");
            List<Expression> expressions = new List<Expression>();
            var Fields = from field in t.GetFields()
                where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(AutoSerializeAttribute))
                orderby field.GetCustomAttribute<AutoSerializeAttribute>().Order
                select field;
            foreach (var field in Fields)
            {
                var isArray = field.FieldType.IsArray;
                var serializer = GetSerializer(provider, isArray ? field.FieldType.GetElementType() : field.FieldType,
                    out Type UsedSerializerType, out List<Type> ConverterTypes);
#if DEBUG
                Log(
                    $"Write of {t.Name}/{field.Name} ({field.FieldType.Name})\nSerializer: {serializer.GetType().Name}");
#endif

                var preSerialize = field.GetCustomAttribute<PreSerializeAttribute>();
                if (preSerialize?.Writing != null)
                {
                    Log($"PreSerialize {preSerialize.Writing}");
                    expressions.Add(Expression.Call(Expression.Convert(inputObject, t), t.GetMethod(preSerialize.Writing)));
                }

                Expression writeExpression =
                    // The Convert above because GetValue returns object
                    // Second Parameter: the value of this field of the inputObject
                    // this field
                    Expression.Call(Expression.Constant(field),
                        // get its value
                        typeof(FieldInfo).GetMethod("GetValue"),
                        // the input is the instance
                        inputObject);
                writeExpression = !isArray
                    ? ConverterTypes.Aggregate(writeExpression,
                        Expression.ConvertChecked)
                    : Expression.ConvertChecked(writeExpression,
                        field.FieldType);

                if (isArray)
                {
                    var leftOverDataAttribute = field.GetCustomAttribute<LeftOverDataAttribute>();
                    if (leftOverDataAttribute == null)
                    {
                        // Write Length as Int32
                        expressions.Add(Expression.Call(Expression.Constant(provider.GetService<ITypeAccessor<Int32>>()),
                            typeof(ITypeAccessor<Int32>).GetMethod("Write"), inputStream,
                            Expression.Property(
                                Expression.ConvertChecked(
                                    Expression.Call(Expression.Constant(field), typeof(FieldInfo).GetMethod("GetValue"),
                                        inputObject), typeof(Array)), typeof(Array).GetProperty("Length"))));
                    }
                }
                // call the write methode on him
                expressions.Add(Expression.Call(Expression.Constant(serializer),
                    // First of all, we want to have the Write Methode on the Serializer
                    UsedSerializerType.GetMethod(isArray ? "WriteArray" : "Write"),
                    // First Parameter: the inputStream
                    inputStream, writeExpression));
            }

            var build = Expression.Lambda<Action<Stream, Object>>(
                Expression.Block(expressions), inputStream, inputObject).Compile();
            Log("Done Building Write");
            return build;
        }

        private static void Log(string s)
        {
#if DEBUG
            Console.WriteLine(s);
#endif
        }

        // Object is by Ref anyways
        public static Action<Stream, Object> BuildRead(Type t, IServiceProvider provider)
        {
            Log($"Building Read of {t.Name}");
            var inputStream = Expression.Parameter(typeof(Stream), "inputStream");
            var inputObject = Expression.Parameter(typeof(Object), "inputObject");
            var expressions = new List<Expression>();
            var fields = (from field in t.GetFields()
                where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(AutoSerializeAttribute))
                orderby field.GetCustomAttribute<AutoSerializeAttribute>().Order
                select field).ToList();

            for (var indexer = 0; indexer < fields.Count; indexer++)
            {
                var field = fields[indexer];
                var isArray = field.FieldType.IsArray;
                var isLast = indexer == fields.Count - 1;

                var serializer = GetSerializer(provider, isArray ? field.FieldType.GetElementType() : field.FieldType,
                    out Type usedSerializerType, out List<Type> converterTypes);

                if (isLast)
                {
                    var LeftOverDataAttribute = field.GetCustomAttribute<LeftOverDataAttribute>();
                    if (LeftOverDataAttribute != null)
                    {
                        if (field.FieldType == typeof(Byte[]))
                        {
                            Log("Left over data");
                            //serializer.ReadArray(inputStream, inputStream.Length - inputStream.Position);
                            expressions.Add(Expression.Call(Expression.Constant(field),
                                typeof(FieldInfo).GetMethod("SetValue", new Type[] { typeof(Object), typeof(Object) }), inputObject,
                                // Convert To Object
                                Expression.ConvertChecked(
                                    Expression.Call(
                                        Expression.Constant(serializer),
                                        usedSerializerType.GetMethod("ReadArray"),
                                        inputStream,
                                        Expression.Subtract(
                                            Expression.ConvertChecked(
                                                Expression.Property(inputStream,
                                                typeof(Stream).GetProperty("Length")),
                                                typeof(Int32)),
                                            Expression.ConvertChecked(
                                                Expression.Property(inputStream,
                                                typeof(Stream).GetProperty("Position")),
                                                typeof(Int32))
                                        )),
                                typeof(Object))));
                            continue;
                        }
                    }
                }

                var PreSerializeAtt = field.GetCustomAttribute<PreSerializeAttribute>();
                if (PreSerializeAtt != null)
                {
                    if (PreSerializeAtt.Reading != null)
                    {
                        Log($"Pre-Serialize {PreSerializeAtt.Reading}");
                        expressions.Add(Expression.Call(inputObject, t.GetMethod(PreSerializeAtt.Reading)));
                    }
                }

                Log(
                    $"Read of {t.Name}/{field.Name} ({field.FieldType.Name})\nSerializer: {serializer?.GetType().Name ?? "null"}");
                Expression readExpression;
                if (!isArray)
                    readExpression = Expression.Call(Expression.Constant(serializer),
                        // First of all, we want to have the Read Methode on the Serializer
                        usedSerializerType.GetMethod("Read"),
                        // First Parameter: the inputStream
                        inputStream);
                else
                    readExpression = Expression.Call(Expression.Constant(serializer),
                        // First of all, we want to have the Read Methode on the Serializer
                        usedSerializerType.GetMethod("ReadArray"),
                        // First Parameter: the inputStream
                        inputStream,
                        Expression.Call(Expression.Constant(provider.GetService<ITypeAccessor<Int32>>()),
                            typeof(ITypeAccessor<Int32>).GetMethod("Read"), inputStream));
                if (!isArray) // TODO: Array Multihop
                    for (var i = converterTypes.Count - 1; i >= 0; i--)
                        readExpression = Expression.ConvertChecked(readExpression, converterTypes[i]);
                else readExpression = Expression.ConvertChecked(readExpression, field.FieldType);

                // field.SetValue(inputObject, serializer.Read(inputStream));
                expressions.Add(Expression.Call(Expression.Constant(field),
                    typeof(FieldInfo).GetMethod("SetValue", new Type[] {typeof(Object), typeof(Object)}), inputObject,
                    // Convert To Object
                    Expression.ConvertChecked(readExpression
                        , typeof(Object))));
            }

            var build = Expression.Lambda<Action<Stream, Object>>(
                Expression.Block(expressions), inputStream, inputObject).Compile();
            Log("Done Building Write");
            return build;
        }

        private static Object GetSerializer(IServiceProvider provider, Type fieldType, out Type UsedSerializerType,
            out List<Type> ConvertFields)
        {
            if (fieldType.IsArray) fieldType = fieldType.GetElementType();
            ConvertFields = new List<Type>();
            Type firstType = fieldType;
            var sas = fieldType.GetCustomAttribute<AutoSerializeAsAttribute>();
            if (sas != null) firstType = sas.Type;
            else // PacketSerializeAs > EnumSubType
            if (fieldType.IsEnum) firstType = Enum.GetUnderlyingType(fieldType);
            if (fieldType != firstType)
            {
                ConvertFields.Add(fieldType);
                var workType = firstType;
                while (true)
                {
                    var v = workType.GetCustomAttribute<AutoConvertToAttribute>();
                    if (v != null)
                    {
                        var v2 = v.Type;
                        if (v2.IsArray) v2 = v2.GetElementType();
                        workType = v2;
                        ConvertFields.Add(v2);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ConvertFields.Add(firstType);
            UsedSerializerType = typeof(ITypeAccessor<>).MakeGenericType(firstType);
            return provider.GetService(UsedSerializerType);
        }
    }
}
