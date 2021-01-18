using ReflectObject;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test
{
    internal class TestReflectObject : ReflectObjectProperties
    {
        public TestReflectObject(object toReflect) : base(toReflect)
        {
        }

        public bool BoolProp { get; protected set; }

        [ReflectFlags(BindingFlags.NonPublic | BindingFlags.Instance)]
        public string NonPublicStringProp { get; protected set; }
        public TestChildReflectObject Child { get; protected set; }

        public List<TestChildReflectObject> Children { get; protected set; }

        public IEnumerable<TestChildReflectObject> YieldedChildren { get; protected set; }

        public Func<string, bool, string> MethodWithArgs { get; protected set; }

        public Action Action { get; protected set; }
        public Action<int> Action1 { get; protected set; }

        public Action<int,int> Action2 { get; protected set; }

        public Action<int, int,int> Action3 { get; protected set; }
        public Action<int, int, int, int> Action4 { get; protected set; }
        public Action<int, int, int, int, int> Action5 { get; protected set; }
        public Action<int, int, int, int, int, int> Action6 { get; protected set; }
        public Action<int, int, int, int, int, int, int> Action7 { get; protected set; }
        public Action<int, int, int, int, int, int, int, int> Action8 { get; protected set; }

        public Func<int> Func { get; protected set; }

        public Func<int, int> Func1 { get; protected set; }

        public Func<int, int, int> Func2 { get; protected set; }
        public Func<int, int, int, int> Func3 { get; protected set; }
        public Func<int, int, int, int, int> Func4 { get; protected set; }
        public Func<int, int, int, int, int, int> Func5 { get; protected set; }
        public Func<int, int, int, int, int, int, int> Func6 { get; protected set; }
        public Func<int, int, int, int, int, int, int, int> Func7 { get; protected set; }
        public Func<int, int, int, int, int, int, int, int, int> Func8 { get; protected set; }

        [DelegateProperty]
        public Func<string> ThisIsAProperty { get; protected set; }
    }

    
}