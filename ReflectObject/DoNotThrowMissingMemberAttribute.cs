using System;

namespace ReflectObject
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class DoNotThrowMissingMemberAttribute : Attribute
    {

    }

}
