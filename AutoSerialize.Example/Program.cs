using AutoSerialize.TypeAccessors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace AutoSerialize.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                .AddSingleton<ITypeAccessor<Byte>, ByteAccessor>()
                .AddSingleton<ITypeAccessor<Int16>, Int16Accessor>()
                .AddSingleton<ITypeAccessor<Int32>, Int32Accessor>()
                .AddSingleton<ITypeAccessor<Int64>, Int64Accessor>()
                .AddSingleton<ITypeAccessor<SByte>, SByteAccessor>()
                .AddSingleton<ITypeAccessor<UInt16>, UInt16Accessor>()
                .AddSingleton<ITypeAccessor<UInt32>, UInt32Accessor>()
                .AddSingleton<ITypeAccessor<UInt64>, UInt64Accessor>()
                .AddSingleton<ITypeAccessor<VarInt>, VarIntAccessor>()
                .AddSingleton<ITypeAccessor<String>, StringAccessor>()
                .AddSingleton<ITypeAccessor<Boolean>, BooleanAccessor>()
                .AddSingleton<ITypeAccessor<Double>, DoubleAccessor>()
                .AddSingleton<ITypeAccessor<Single>, FloatAccessor>()
                .BuildServiceProvider();

            var read = ExpressionBuilder.BuildRead(typeof(ExampleType), provider);
            var write = ExpressionBuilder.BuildWrite(typeof(ExampleType), provider);

            using (var stream = new MemoryStream())
            {
                write(stream, new ExampleType()
                {
                    UsualInt = 1,
                    WellAString = "Hey there!",
                    AnEnum = SampleEnum.FirstValue,
                    SpecialEnum = SpecialEnum.Third,
                    AnArray = new Byte[]
                    {
                        123,
                        255,
                        0,
                        5
                    }
                });
                var v = new ExampleType();
                stream.Position = 0; // so we can re-read
                read(stream, v);
            }
        }
    }
}
