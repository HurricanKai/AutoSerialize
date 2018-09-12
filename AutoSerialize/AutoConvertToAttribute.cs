using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public sealed class AutoConvertToAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly Type _type;

        // This is a positional argument
        public AutoConvertToAttribute(Type type)
        {
            this._type = type;
        }

        public Type Type => _type;
    }
}
