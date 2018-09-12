using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize.TypeAccessors
{
    /// <summary>
    /// This is simply a Type Around Int32
    /// It's used to Indicate a special Int32 Serialization
    /// This Serialization works by the Least Significant Bit indicating if there is another byte, and the leftover 7 Bits are used for the number. So for little Numbers this is worth, but with Big Numbers it gets less efficent
    /// </summary>
    [AutoConvertTo(typeof(int))]
    public class VarInt
    {
        private int _val;
        private VarInt(int val) { _val = val; }

        public static implicit operator Int32(VarInt v)
            => v._val;
        public static implicit operator VarInt(Int32 v)
            => new VarInt(v);
    }
}
