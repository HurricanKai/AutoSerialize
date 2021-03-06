﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoSerialize.TypeAccessors
{
    public class DoubleAccessor : TypeAccessor<Double>
    {
        private ITypeAccessor<UInt64> _accessorUInt64;

        public DoubleAccessor(ITypeAccessor<UInt64> accessorUInt64)
        {
            _accessorUInt64 = accessorUInt64;
        }
        public override Double Read(Stream stream)
        {
            var v = _accessorUInt64.Read(stream);
            unsafe
            {
                double value = *((double*)&v);
                return value;
            }
        }

        public override void Write(Stream stream, Double val)
        {
            unsafe
            {
                ulong value = *((ulong*)&val);
                _accessorUInt64.Write(stream, value);
            }
        }
    }
}
