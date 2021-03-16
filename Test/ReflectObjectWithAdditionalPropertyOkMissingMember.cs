using ReflectObject;

namespace Test
{
    [DoNotThrowMissingMembers]
    internal class ReflectObjectWithAdditionalPropertyOkMissingMember : ReflectObjectProperties
    {
        public ReflectObjectWithAdditionalPropertyOkMissingMember(object toReflect) : base(toReflect)
        {

        }

        public bool AditionalProperty { get; protected set; }
    }

}
