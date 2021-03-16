using System;

namespace ReflectObject
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public class DoNotThrowMissingMembersAttribute : Attribute
	{

	}

}
