using AutoSerialize.TypeAccessors;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSerialize.Benchmark
{
    [RPlotExporter, RankColumn]
    public class AutoSerializeVsDotNetSerialize
    {
        private IServiceProvider provider;
        private TestType obj;
        private Action<Stream, Object> read;
        private Action<Stream, Object> write;
        private IFormatter formatter;

        [Params(1, 10, 100, 1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            provider = new ServiceCollection()
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

            formatter = new BinaryFormatter();

            read = ExpressionBuilder.BuildRead(typeof(TestType), provider);
            write = ExpressionBuilder.BuildWrite(typeof(TestType), provider);

            obj = new TestType()
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
            };
        }

        [Benchmark]
        public void AutoSerialize()
        {
            for (int i = 0; i < N; i++)
            using (Stream s = new MemoryStream())
            {
                write(s, obj);
            }
        }

        [Benchmark]
        public void DotNet()
        {
            for (int i = 0; i < N; i++)
            using (Stream s = new MemoryStream())
            {
                formatter.Serialize(s, obj);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<AutoSerializeVsDotNetSerialize>();
            Console.WriteLine(summary);
            Console.ReadLine();
        }
    }
}
