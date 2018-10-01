using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple =false, Inherited = true)]
    public class NoLengthAttribute : Attribute
    {
    }
}
