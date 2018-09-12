using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize
{
    [System.AttributeUsage(
          AttributeTargets.Class 
        | AttributeTargets.Enum 
        | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public sealed class AutoSerializeAsAttribute : Attribute
    {
        readonly Type _type;
        
        public AutoSerializeAsAttribute(Type type)
        {
            this._type = type;
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
