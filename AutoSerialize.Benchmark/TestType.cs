using AutoSerialize.TypeAccessors;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize.Benchmark
{
    [Serializable]
    public class TestType
    {
        [AutoSerialize(0)]
        public int UsualInt;
        [AutoSerialize(1)]
        public string WellAString;
        [AutoSerialize(2)]
        public SampleEnum AnEnum;
        [AutoSerialize(3)]
        public SpecialEnum SpecialEnum;
        [AutoSerialize(4)]
        public byte[] AnArray;
    }

    // Written as int       THIS    is required
    public enum SampleEnum : int
    {
        FirstValue = 1,
        SecondValue = 2,
        ThirdValue = 3,
    }

    // Automatically Wrote using VarInt
    // AutoSerializeAs Maybe stacked to an inifite
    [AutoSerializeAs(typeof(VarInt))]
    public enum SpecialEnum : int
    {
        FirstVarInt = 1,
        Second = 2,
        Third = 3,
    }
}
