using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSerialize
{
    /// <summary>
    /// Can only be applied to `Byte[]` else it will have no effect.
    /// For Reading this Attribute indicates the Serialized Type wants to get any non-Read Data (infered from the Position - the Length)
    /// For Writing this indicates that the Serializer shoud not head this with an int32 to indicate Length
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LeftOverDataAttribute : Attribute
    {
    }
}
