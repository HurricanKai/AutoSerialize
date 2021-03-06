﻿using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerialize.TypeAccessors
{
    public class Int32Accessor : TypeAccessor<Int32>
    {
        private ITypeAccessor<UInt32> _accessorU32;

        public Int32Accessor(ITypeAccessor<UInt32> accessorU32)
        {
            _accessorU32 = accessorU32;
        }

        public override Int32 Read(Stream stream)
            => (Int32) _accessorU32.Read(stream);
        public override void Write(Stream stream, Int32 val)
            =>  _accessorU32.Write(stream, (UInt32)val);
    }
}
