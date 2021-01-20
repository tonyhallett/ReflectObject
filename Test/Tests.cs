using NUnit.Framework;
using ReflectObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Set_Public_Instance_Property_Without_ReflectFlagsAttribute()
        {
            var toReflect = new TestObject
            {
                BoolProp = true
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.IsTrue(testReflectObject.BoolProp);
        }

        [Test]
        public void Should_Set_Property_Using_BindingFlags_From_ReflectFlagsAttribute()
        {
            var toReflect = new TestObject
            {
                NonPublicStringProp = "Hello from root"
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreEqual("Hello from root", testReflectObject.NonPublicStringProp);
        }

        [Test]
        public void Should_Wrap_Permitted_Properties()
        {
            var toReflect = new TestObject
            {
                Child = new ChildObject
                {
                    ChildStringProp = "Hello from child"
                }
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreEqual("Hello from child", testReflectObject.Child.ChildStringProp);
        }

        [Test]
        public void Should_Not_Wrap_Null_Values()
        {
            var toReflect = new TestObject
            {
                
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.Null(testReflectObject.Child);
        }

        [Test]
        public void Should_Wrap_Permitted_List_Properties_Against_IEnumerable()
        {
            var toReflect = new TestObject
            {
                Children = new List<ChildObject> { 
                    new ChildObject { ChildStringProp = "One"},
                    new ChildObject { ChildStringProp = "Two"}

                }
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreEqual(2, testReflectObject.Children.Count);
            Assert.AreEqual("One", testReflectObject.Children[0].ChildStringProp);
            Assert.AreEqual("Two", testReflectObject.Children[1].ChildStringProp);
        }

        

        [Test]
        public void Should_Yield_IEnumerable_Properties()
        {
            var yielded = false;
            IEnumerable<ChildObject> GetYielded()
            {
                yielded = true;
                yield return new ChildObject { ChildStringProp = "One" };
            }
            var toReflect = new TestObject
            {
                YieldedChildren = GetYielded()
            };

            var testReflectObject = new TestReflectObject(toReflect);
            Assert.False(yielded);
            var toYield = testReflectObject.YieldedChildren;
            Assert.False(yielded);
            Assert.AreEqual("One", toYield.ToList()[0].ChildStringProp);
            Assert.True(yielded);
        }

        [Test]
        public void Should_Not_Wrap_Nulls_In_List()
        {
            var toReflect = new TestObject
            {
                Children = new List<ChildObject> {
                    new ChildObject { ChildStringProp = "One"},
                    null

                }
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.Null(testReflectObject.Children[1]);
        }

        [Test]
        public void Should_Not_Wrap_Nulls_In_IEnumerable()
        {
            IEnumerable<ChildObject> GetYielded()
            {
                yield return new ChildObject { ChildStringProp = "One" };
                yield return null;
            }
            var toReflect = new TestObject
            {
                YieldedChildren = GetYielded()
            };

            var testReflectObject = new TestReflectObject(toReflect);
            var toYield = testReflectObject.YieldedChildren;
            Assert.Null(toYield.ToList()[1]);
        }

        [Test]
        public void Should_Expose_The_Reflected_Object()
        {
            var toReflect = new TestObject
            {
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreSame(toReflect, testReflectObject.ReflectedObject);
        }

        [Test]
        public void Should_Expose_The_Reflected_Object_Type()
        {
            var toReflect = new TestObject
            {
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreSame(toReflect.GetType(), testReflectObject.ReflectedType);
        }

        [Test]
        public void Should_Expose_Methods_As_Properties()
        {
            var toReflect = new TestObject
            {
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreEqual("It worked ? : True", testReflectObject.MethodWithArgs("It worked ?", true));

        }

        [Test]
        public void Should_Treat_Delegate_As_Property_If_Marked_With_DelegatePropertyAttribute()
        {
            var toReflect = new TestObject
            {
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.AreEqual("This is a property", testReflectObject.ThisIsAProperty());
        }

        [Test]
        public void Should_Work_With_Up_To_8_Action_Parameters()
        {
            var toReflect = new TestObject
            {
            };
            var testReflectObject = new TestReflectObject(toReflect);
            testReflectObject.Action();
            Assert.IsEmpty(toReflect.DelegateArguments);
            List<Delegate> remainingActionDelegates = new List<Delegate>
            {
                testReflectObject.Action1,
                testReflectObject.Action2,
                testReflectObject.Action3,
                testReflectObject.Action4,
                testReflectObject.Action5,
                testReflectObject.Action6,
                testReflectObject.Action7,
                testReflectObject.Action8
            };
            var possibleArguments = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            for(var i = 0; i < remainingActionDelegates.Count; i++)
            {
                var arguments = possibleArguments.Take(i + 1).Select(i=>(object)i).ToArray();
                remainingActionDelegates[i].DynamicInvoke(arguments);
                Assert.AreEqual(arguments, toReflect.DelegateArguments);

            }
        }

        [Test]
        public void Should_Work_With_Up_To_8_Func_Parameters()
        {
            var toReflect = new TestObject
            {
            };
            var testReflectObject = new TestReflectObject(toReflect);
            testReflectObject.Action();
            var funcReturn = testReflectObject.Func();
            Assert.AreEqual(0, funcReturn);
            List<Delegate> remainingActionDelegates = new List<Delegate>
            {
                testReflectObject.Func1,
                testReflectObject.Func2,
                testReflectObject.Func3,
                testReflectObject.Func4,
                testReflectObject.Func5,
                testReflectObject.Func6,
                testReflectObject.Func7,
                testReflectObject.Func8

            };
            var possibleArguments = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            for (var i = 0; i < remainingActionDelegates.Count; i++)
            {
                var arguments = possibleArguments.Take(i + 1).Select(i => (object)i).ToArray();
                var result = remainingActionDelegates[i].DynamicInvoke(arguments);
                Assert.AreEqual(arguments, toReflect.DelegateArguments);
                Assert.AreEqual(result, i+1);

            }
        }

        [Test]
        public void Property_Getters_Always_Return_The_Reflected_Value_At_The_Time_Of_Construction()
        {
            var toReflect = new TestObject
            {
                BoolProp = true
            };
            var testReflectObject = new TestReflectObject(toReflect);
            Assert.IsTrue(testReflectObject.BoolProp);
            toReflect.BoolProp = false;
            Assert.IsTrue(testReflectObject.BoolProp);
        }

        [Test]
        public void Should_Work_With_Multiple_Instances()
        {
            var toReflect1 = new TestObject
            {
                BoolProp = true
            };
            var testReflectObject1 = new TestReflectObject(toReflect1);
            Assert.IsTrue(testReflectObject1.BoolProp);

            var toReflect2 = new TestObject
            {
                BoolProp = false
            };
            var testReflectObject2 = new TestReflectObject(toReflect2);
            Assert.IsFalse(testReflectObject2.BoolProp);
        }

        [Test]
        public void Should_Throw_For_Additional_Property_Not_Present_On_Reflected()
        {
            //todo test the message
            Assert.Throws<PropertyDoesNotExistException>(() =>
            {
                var reflectObject = new ReflectObjectWithAdditionalProperty(new object());
            });
        }

        [Test]
        public void Should_Throw_For_Additional_Func_Not_Present_On_Reflected()
        {
            Assert.Throws<PropertyDoesNotExistException>(() =>
            {
                var reflectObject = new ReflectObjectWithAdditionalFunc(new object());
            });
        }

        [Test]
        public void Should_Not_Throw_For_Additional_Properties_If_Ignore_With_Attribute()
        {
            Assert.DoesNotThrow(() =>
            {
                var reflectObject = new ReflectObjectWithAdditionalSafeProperties(new object());
            });
        }

        [Test]
        public void PerformanceTest()
        {
            var mean = Clock.BenchmarkMeanTime(() =>
            {
                var testObject = new TestObject
                {
                    Child = new ChildObject
                    {
                        ChildStringProp = "Child"
                    },

                };
                var testReflect = new TestReflectObject(testObject);
            });

            
            Assert.That(mean, Is.LessThanOrEqualTo(1200));
            
        }

        //[Test]// - uncomment and run this and update time above if change is more performant
        //public void PerformanceBaitTest() // https://stackoverflow.com/questions/15181358/how-can-i-unit-test-performance-optimisations-in-c
        //{
        //    var mean = Clock.BenchmarkMeanTime(() =>
        //    {
        //        var testObject = new TestObject
        //        {
        //            Child = new ChildObject
        //            {
        //                ChildStringProp = "Child"
        //            },

        //        };
        //        var testReflect = new TestReflectObject(testObject);
        //    });
        //    Assert.That(mean, Is.LessThanOrEqualTo(0));

        //}
    }

    
}