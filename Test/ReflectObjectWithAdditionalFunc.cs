using ReflectObject;
using System;

namespace Test
{
    internal class ReflectObjectWithAdditionalFunc : ReflectObjectProperties
    {
        public ReflectObjectWithAdditionalFunc(object toReflect) : base(toReflect)
        {

        }

        public Func<string> AditionalFunc{ get; protected set; }
    }

}
