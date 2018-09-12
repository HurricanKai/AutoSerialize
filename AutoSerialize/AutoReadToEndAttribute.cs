using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize
{
    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class AutoReadToEndAttribute : Attribute
    {
    }
}
