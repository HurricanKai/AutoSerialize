﻿using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerialize.TypeAccessors
{
    public class SByteAccessor : TypeAccessor<SByte>
    {
        private ITypeAccessor<Byte> _accessorByte;

        public SByteAccessor(ITypeAccessor<Byte> accessorByte)
        {
            _accessorByte = accessorByte;
        }

        public override SByte Read(Stream stream)
            => (SByte) _accessorByte.Read(stream);

        public override void Write(Stream stream, SByte val)
            =>  _accessorByte.Write(stream, (Byte)val);
    }
}
