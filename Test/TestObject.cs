using System;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    public class TestObject
    {
        public bool BoolProp { get; set; }
        internal string NonPublicStringProp { get; set; }
        public ChildObject Child { get; set; }

        public IEnumerable Children { get; set; }

        public IEnumerable<ChildObject> YieldedChildren { get; set; }

        public void Action() {
            DelegateArguments = new int[] { };
        }
        public int[] DelegateArguments { get; private set; }

        public void Action1(int arg1) {
            DelegateArguments = new int[] { arg1 };
        }
        public void Action2(int arg1,int arg2) {
            DelegateArguments = new int[] { arg1, arg2 };
        }
        public void Action3(int arg1, int arg2, int arg3) {
            DelegateArguments = new int[] { arg1,arg2, arg3 };
        }
        public void Action4(int arg1, int arg2, int arg3,int arg4) {
            DelegateArguments = new int[] { arg1, arg2, arg3, arg4 };
        }
        public void Action5(int arg1, int arg2, int arg3, int arg4,int arg5) {
            DelegateArguments = new int[] { arg1, arg2, arg3,arg4, arg5 };
        }
        public void Action6(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6) {
            DelegateArguments = new int[] { arg1, arg2, arg3,arg4, arg5, arg6 };
        }
        public void Action7(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7) {
            DelegateArguments = new int[] { arg1, arg2, arg3,arg4, arg5, arg6, arg7 };
        }
        public void Action8(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8) {
            DelegateArguments = new int[] { arg1, arg2, arg3,arg4, arg5, arg6, arg7, arg8 };
        }

        public int Func()
        {
            DelegateArguments = new int[] { };
            return 0;
        }
        public int Func1(int arg1)
        {
            DelegateArguments = new int[] { arg1 };
            return 1;
        }
        public int Func2(int arg1, int arg2)
        {
            DelegateArguments = new int[] { arg1, arg2 };
            return 2;
        }
        public int Func3(int arg1, int arg2, int arg3)
        {
            DelegateArguments = new int[] { arg1, arg2, arg3 };
            return 3;
        }
        public int Func4(int arg1, int arg2, int arg3, int arg4)
        {
            DelegateArguments = new int[] { arg1, arg2, arg3, arg4 };
            return 4;
        }
        public int Func5(int arg1, int arg2, int arg3, int arg4, int arg5)
        {
            DelegateArguments = new int[] { arg1, arg2, arg3, arg4, arg5 };
            return 5;
        }
        public int Func6(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6)
        {
            DelegateArguments = new int[] { arg1, arg2, arg3, arg4, arg5, arg6 };
            return 6;
        }
        public int Func7(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7)
        {
            DelegateArguments = new int[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
            return 7;
        }
        public int Func8(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8)
        {
            DelegateArguments = new int[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
            return 8;
        }


        public string MethodWithArgs(string arg1,bool arg2)
        {
            return arg1 + " : " + arg2.ToString();
        }

        public Func<string> ThisIsAProperty { get; protected set; } = () => "This is a property";
    }

    
}