using ReflectObject;
using System;

namespace Test
{
    internal class ReflectObjectWithAdditionalSafeProperties : ReflectObjectProperties
    {
        public ReflectObjectWithAdditionalSafeProperties(object toReflect) : base(toReflect)
        {

        }
        [Ignore]
        public bool AditionalProperty { get; protected set; }
        [Ignore]
        public Func<string> AditionalFunc { get; protected set; }
    }

}
