using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectObject
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreAttribute:Attribute
    {
    }
}
