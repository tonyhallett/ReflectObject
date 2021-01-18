using ReflectObject;

namespace Test
{
    internal class TestChildReflectObject : ReflectObjectProperties
    {
        public TestChildReflectObject(object toReflect) : base(toReflect)
        {
        }

        public string ChildStringProp { get; protected set; }
    }

    
}