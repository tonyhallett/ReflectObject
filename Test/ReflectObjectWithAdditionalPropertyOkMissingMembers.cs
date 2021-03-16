using ReflectObject;

namespace Test
{
    [DoNotThrowMissingMembers]
    internal class ReflectObjectWithAdditionalPropertyOkMissingMembers : ReflectObjectProperties
    {
        public ReflectObjectWithAdditionalPropertyOkMissingMembers(object toReflect) : base(toReflect)
        {

        }

        public bool AditionalProperty { get; protected set; }
    }

}
