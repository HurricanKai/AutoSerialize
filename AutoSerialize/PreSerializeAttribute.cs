using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PreSerializeAttribute : Attribute
    {
        public string Writing { get; set; }
        public string Reading { get; set; }
    }
}
