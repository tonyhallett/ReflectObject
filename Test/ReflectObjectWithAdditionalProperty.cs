using ReflectObject;

namespace Test
{
    internal class ReflectObjectWithAdditionalProperty:ReflectObjectProperties
    {
        public ReflectObjectWithAdditionalProperty(object toReflect) : base(toReflect)
        {

        }

        public bool AditionalProperty { get; protected set; }
    }

}
