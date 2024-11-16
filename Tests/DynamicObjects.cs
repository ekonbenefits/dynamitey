using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Dynamitey.SupportLibrary;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Dynamitey.Tests
{
    [TestFixture]
    public class DynamicObjs : Helper
    {




        [Test]
        public void GetterAnonTest()
        {
            var tAnon = new {Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid()};

            dynamic tTest = new DynamicObjects.Get(tAnon);

            Assert.AreEqual(tAnon.Prop1, tTest.Prop1);
            Assert.AreEqual(tAnon.Prop2, tTest.Prop2);
            Assert.AreEqual(tAnon.Prop3, tTest.Prop3);
        }

        [Test]
        public void GetterVoidTest()
        {
            var tPoco = new VoidMethodPoco();

            dynamic tTest = new DynamicObjects.Get(tPoco);

            tTest.Action();
        }

        [Test]
        public void GetterArrayTest()
        {


            var tArray = new int[] {1, 2, 3};

            dynamic tTest = new DynamicObjects.Get(tArray);
            Dynamic.ApplyEquivalentType(tTest, typeof (IStringIntIndexer));

            Assert.AreEqual(tArray[2].ToString(), tTest[2]);
        }

        [Test]
        public void GetterEventTest()
        {
            dynamic dynEvent = new DynamicObjects.Get(new PocoEvent());
            Dynamic.ApplyEquivalentType(dynEvent, typeof (IEvent));
            var tSet = false;
            EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
            dynEvent.Event += tActsLikeOnEvent;

            dynEvent.OnEvent(null, null);
            Assert.AreEqual(true, tSet);

        }


        [Test]
        public void GetterEventTest2()
        {
            dynamic dynEvent = new DynamicObjects.Get(new PocoEvent());
            Dynamic.ApplyEquivalentType(dynEvent, typeof (IEvent));
            var tSet = false;
            EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
            dynEvent.Event += tActsLikeOnEvent;
            dynEvent.Event -= tActsLikeOnEvent;
            dynEvent.OnEvent(null, null);
            Assert.AreEqual(false, tSet);

        }


        [Test]
        public void GetterDynamicTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            dynamic tTest = new DynamicObjects.Get(tNew);


            Assert.AreEqual(tNew.Prop1, tTest.Prop1);
            Assert.AreEqual(tNew.Prop2, tTest.Prop2);
            Assert.AreEqual(tNew.Prop3, tTest.Prop3);
        }

        public class TestForwarder : Dynamitey.DynamicObjects.BaseForwarder
        {
            public TestForwarder(object target)
                : base(target)
            {
            }
        }

        [Test]
        public void ForwardAnonTest()
        {
            var tAnon = new {Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid()};

            dynamic tTest = new TestForwarder(tAnon);

            Assert.AreEqual(tAnon.Prop1, tTest.Prop1);
            Assert.AreEqual(tAnon.Prop2, tTest.Prop2);
            Assert.AreEqual(tAnon.Prop3, tTest.Prop3);
        }

        [Test]
        public void ForwardVoidTest()
        {
            var tPoco = new VoidMethodPoco();

            dynamic tTest = new TestForwarder(tPoco);

            tTest.Action();
        }


        [Test]
        public void ForwardGenericMethodsTest()
        {
            dynamic tNew = new ForwardGenericMethodsTestClass();

            dynamic tFwd = new TestForwarder(tNew);

            Assert.AreEqual("test99", tFwd.Create<ForwardGenericMethodsTestClass>(99).Value);
        }


        [Test]
        public void ForwardDynamicTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            dynamic tTest = new TestForwarder(tNew);


            Assert.AreEqual(tNew.Prop1, tTest.Prop1);
            Assert.AreEqual(tNew.Prop2, tTest.Prop2);
            Assert.AreEqual(tNew.Prop3, tTest.Prop3);
        }

        [Test]
        public void DictionaryMethodsTest()
        {

            dynamic tNew = new DynamicObjects.Dictionary();
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new Func<string>(() => "test");
            tNew.Action4 = new Func<int, string>(arg => "test" + arg);





            Assert.That(() => tNew.Action1(), Throws.InstanceOf<AssertionException>());
            Assert.That(() => tNew.Action2(true), Throws.InstanceOf<AssertionException>());

            Assert.That((object)tNew.Action3(), Is.EqualTo("test"));

            Assert.That((object)tNew.Action4(4), Is.EqualTo("test4"));
        }

        [Test]
        public void ForwardMethodsTest()
        {

            dynamic tNew = new DynamicObjects.Dictionary();
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new Func<string>(() => "test");
            tNew.Action4 = new Func<int, string>(arg => "test" + arg);


            dynamic tFwd = new TestForwarder(tNew);



            Assert.That(() => tFwd.Action1(), Throws.InstanceOf<AssertionException>());
            Assert.That(() => tFwd.Action2(true), Throws.InstanceOf<AssertionException>());

            Assert.That((object)tFwd.Action3(), Is.EqualTo("test"));

            Assert.That((object)tFwd.Action4(4), Is.EqualTo("test4"));
        }

        [Test]
        public void DictionaryMethodsOutTest()
        {

            dynamic tNew = new DynamicObjects.Dictionary();
            tNew.Func = new DynamicTryString(TestOut);

            Assert.AreEqual(true, tNew.Func(null, "Test", out string tOut));
            Assert.AreEqual("Test", tOut);

            Assert.AreEqual(false, tNew.Func(null, 1, out string tOut2));
            Assert.AreEqual(null, tOut2);
        }

        private static object TestOut(CallSite dummy, object @in, out string @out)
        {
            @out = @in as string;

            return @out != null;
        }


        [Test]
        public void DictionaryMethodsTestWithPropertyAccess()
        {

            dynamic tNew = new DynamicObjects.Dictionary();
            tNew.PropCat = "Cat-";
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new ThisFunc<string>(@this => @this.PropCat + "test");



            Assert.That(() => tNew.Action1(), Throws.InstanceOf<AssertionException>());
            Assert.That(() => tNew.Action2(true), Throws.InstanceOf<AssertionException>());

            Assert.AreEqual("Cat-test", tNew.Action3());


        }

        [Test]
        public void DictionaryNullMethodsTest()
        {

            dynamic tNew = new DynamicObjects.Dictionary();
            Dynamic.ApplyEquivalentType(tNew, typeof (ISimpleStringMethod));

            Assert.That((object)tNew.StartsWith("Te"), Is.False);



        }


        [Test]
        public void DynamicDictionaryWrappedTest()
        {

            var tDictionary = new Dictionary<string, object>
                                  {
                                      {"Test1", 1},
                                      {"Test2", 2},
                                      {
                                          "TestD", new Dictionary<string, object>()
                                                       {
                                                           {"TestA", "A"},
                                                           {"TestB", "B"}
                                                       }
                                      }
                                  };

            dynamic tNew = new DynamicObjects.Dictionary(tDictionary);

            Assert.AreEqual(1, tNew.Test1);
            Assert.AreEqual(2, tNew.Test2);
            Assert.AreEqual("A", tNew.TestD.TestA);
            Assert.AreEqual("B", tNew.TestD.TestB);
        }

        [Test]
        public void InterfaceDictionaryWrappedTest()
        {

            var tDictionary = new Dictionary<string, object>
                                  {
                                      {"Test1", 1},
                                      {"Test2", 2L},
                                      {"Test3", 1},
                                      {"Test4", "Two"},
                                      {
                                          "TestD", new Dictionary<string, object>()
                                                       {
                                                           {"TestA", "A"},
                                                           {"TestB", "B"}
                                                       }
                                      }
                                  };

            dynamic tDynamic = new DynamicObjects.Dictionary(tDictionary);
            dynamic tNotDynamic = new DynamicObjects.Dictionary(tDictionary);


            Dynamic.ApplyEquivalentType(tDynamic, typeof (IDynamicDict));
            Dynamic.ApplyEquivalentType(tNotDynamic, typeof (INonDynamicDict));


            Assert.AreEqual(tDynamic, tNotDynamic);

            Assert.AreEqual(1, tDynamic.Test1);
            Assert.AreEqual(2L, tDynamic.Test2);
            Assert.AreEqual(TestEnum.One, tDynamic.Test3);
            Assert.AreEqual(TestEnum.Two, tDynamic.Test4);

            Assert.AreEqual("A", tDynamic.TestD.TestA);
            Assert.AreEqual("B", tDynamic.TestD.TestB);

            Assert.AreEqual(1, tNotDynamic.Test1);
            Assert.AreEqual(2L, tNotDynamic.Test2);
            Assert.AreEqual(TestEnum.One, tNotDynamic.Test3);
            Assert.AreEqual(TestEnum.Two, tNotDynamic.Test4);

            Assert.AreEqual(typeof (Dictionary<string, object>), tNotDynamic.TestD.GetType());
            Assert.AreEqual(typeof (DynamicObjects.Dictionary), tDynamic.TestD.GetType());
        }

        [Test]
        public void DynamicObjectEqualsTest()
        {
            var tDictionary = new Dictionary<string, object>
                                  {
                                      {"Test1", 1},
                                      {"Test2", 2},
                                      {
                                          "TestD", new Dictionary<string, object>()
                                                       {
                                                           {"TestA", "A"},
                                                           {"TestB", "B"}
                                                       }
                                      }
                                  };

            dynamic tDynamic = new DynamicObjects.Dictionary(tDictionary);
            dynamic tNotDynamic = new DynamicObjects.Dictionary(tDictionary);


            Dynamic.ApplyEquivalentType(tDynamic, typeof (IDynamicDict));
            Dynamic.ApplyEquivalentType(tNotDynamic, typeof (INonDynamicDict));

            Assert.AreEqual(tDynamic, tNotDynamic);

            Assert.AreEqual(tDynamic, tDictionary);

            Assert.AreEqual(tNotDynamic, tDictionary);
        }

        [Test]
        public void DynamicAnnonymousWrapper()
        {
            var tData = new Dictionary<int, string> {{1, "test"}};
            var tDyn = DynamicObjects.Get.Create(new
                                                     {
                                                         Test1 = 1,
                                                         Test2 = "2",
                                                         IsGreaterThan5 = Return<bool>.Arguments<int>(it => it > 5),
                                                         ClearData = ReturnVoid.Arguments(() => tData.Clear())
                                                     });

            Assert.AreEqual(1, tDyn.Test1);
            Assert.AreEqual("2", tDyn.Test2);
            Assert.AreEqual(true, tDyn.IsGreaterThan5(6));
            Assert.AreEqual(false, tDyn.IsGreaterThan5(4));

            Assert.AreEqual(1, tData.Count);
            tDyn.ClearData();
            Assert.AreEqual(0, tData.Count);

        }

        [Test]
        public void TestAnonInterface()
        {
            dynamic tInterface = new DynamicObjects.Get(new
                                                            {
                                                                CopyArray =
                                                            ReturnVoid.Arguments<Array, int>(
                                                                (ar, i) => Enumerable.Range(1, 10)),
                                                                Count = 10,
                                                                IsSynchronized = false,
                                                                SyncRoot = this,
                                                                GetEnumerator =
                                                            Return<IEnumerator>.Arguments(
                                                                () => Enumerable.Range(1, 10).GetEnumerator())
                                                            });

            Dynamic.ApplyEquivalentType(tInterface, typeof (ICollection), typeof (IEnumerable));

            Assert.AreEqual(10, tInterface.Count);
            Assert.AreEqual(false, tInterface.IsSynchronized);
            Assert.AreEqual(this, tInterface.SyncRoot);
            Assert.That((object)tInterface.GetEnumerator(), Is.InstanceOf<IEnumerator>());
        }

        [Test]
        public void TestBuilder()
        {
            var New = Builder.New<ExpandoObject>();

            var tExpando = New.Object(
                Test: "test1",
                Test2: "Test 2nd"
                );
            Assert.AreEqual("test1", tExpando.Test);
            Assert.AreEqual("Test 2nd", tExpando.Test2);

            dynamic NewD = new DynamicObjects.Builder<ExpandoObject>();


            var tExpandoNamedTest = NewD.Robot(
                LeftArm: "Rise",
                RightArm: "Clamp"
                );

            Assert.AreEqual("Rise", tExpandoNamedTest.LeftArm);
            Assert.AreEqual("Clamp", tExpandoNamedTest.RightArm);
        }

        [Test]
        public void TestSetupOtherTypes()
        {
            var New = Builder.New().Setup(
                Expando: typeof (ExpandoObject),
                Dict: typeof (DynamicObjects.Dictionary)
                );

            var tExpando = New.Expando(
                LeftArm: "Rise",
                RightArm: "Clamp"
                );

            var tDict = New.Dict(
                LeftArm: "RiseD",
                RightArm: "ClampD"
                );

            Assert.AreEqual("Rise", tExpando.LeftArm);
            Assert.AreEqual("Clamp", tExpando.RightArm);
            Assert.AreEqual(typeof (ExpandoObject), tExpando.GetType());

            Assert.AreEqual("RiseD", tDict.LeftArm);
            Assert.AreEqual("ClampD", tDict.RightArm);
            Assert.AreEqual(typeof (DynamicObjects.Dictionary), tDict.GetType());

        }

        [Test]

        //This test data is modified from MS-PL Clay project http://clay.codeplex.com
        public void TestClayFactorySyntax()
        {
            dynamic New = Builder.New();

            {
                var person = New.Person();
                person.FirstName = "Louis";
                person.LastName = "Dejardin";
                Assert.AreEqual("Louis", person.FirstName);
                Assert.AreEqual("Dejardin", person.LastName);
            }
            {
                var person = New.Person();
                person["FirstName"] = "Louis";
                person["LastName"] = "Dejardin";
                Assert.AreEqual("Louis", person.FirstName);
                Assert.AreEqual("Dejardin", person.LastName);
            }
            {
                var person = New.Person(
                    FirstName: "Bertrand",
                    LastName: "Le Roy"
                    ).Aliases("bleroy", "boudin");

                Assert.AreEqual("Bertrand", person.FirstName);
                Assert.AreEqual("Le Roy", person.LastName);
                Assert.AreEqual("boudin", person.Aliases[1]);
            }

            {
                var person = New.Person()
                                .FirstName("Louis")
                                .LastName("Dejardin")
                                .Aliases(new[] {"Lou"});

                Assert.AreEqual(person.FirstName, "Louis");
                Assert.AreEqual(person.Aliases[0], "Lou");
            }

            {
                var person = New.Person(new
                                            {
                                                FirstName = "Louis",
                                                LastName = "Dejardin"
                                            });
                Assert.AreEqual(person.FirstName, "Louis");
                Assert.AreEqual(person.LastName, "Dejardin");
            }

        }





        [Test]
        //This test data is modified from MS-PL Clay project http://clay.codeplex.com
        public void TestFactoryListSyntax()
        {
            dynamic New = Builder.New();

            //Test using Clay Syntax
            var people = New.Array(
                New.Person().FirstName("Louis").LastName("Dejardin"),
                New.Person().FirstName("Bertrand").LastName("Le Roy")
                );

            Assert.AreEqual("Dejardin", people[0].LastName);
            Assert.AreEqual("Le Roy", people[1].LastName);

            var people2 = new DynamicObjects.List()
                              {
                                  New.Robot(Name: "Bender"),
                                  New.Robot(Name: "RobotDevil")
                              };


            Assert.AreEqual("Bender", people2[0].Name);
            Assert.AreEqual("RobotDevil", people2[1].Name);

        }

        [Test]
        public void TestQuicListSyntax()
        {
            var tList = Build.NewList("test", "one", "two");
            Assert.AreEqual("one", tList[1]);

            var tList2 = Build.NewList("test", "one", "two", "three");
            Assert.AreEqual("three", tList2[3]);
        }


        [Test]
        public void TestRecorder()
        {
            dynamic New = Builder.New<DynamicObjects.Recorder>();

            DynamicObjects.Recorder tRecording = New.Watson(Test: "One", Test2: 2, NameLast: "Watson");


            dynamic tVar = tRecording.ReplayOn(new ExpandoObject());

            Assert.AreEqual("One", tVar.Test);
            Assert.AreEqual(2, tVar.Test2);
            Assert.AreEqual("Watson", tVar.NameLast);
        }


#if NETFRAMEWORK

        [Test]
        public void TestCodeDomLateTypeBind()
        {  
            // http://stackoverflow.com/questions/16918612/dynamically-use-runtime-compiled-assemlby/16920438#16920438
            string code = @"
                namespace CodeInjection
                {
                    public static class DynConcatenateString
                    {
                        public static string Concatenate(string s1, string s2){
                            return s1 + "" ! "" + s2;
                        }
                    }
                }";
   
            var codeProvider = new CSharpCodeProvider();
 
            var parameters = new CompilerParameters {GenerateExecutable = false, GenerateInMemory = true};

            CompilerResults cr = codeProvider.CompileAssemblyFromSource(parameters,code);


            dynamic DynConcatenateString = new DynamicObjects.LateType(cr.CompiledAssembly, "CodeInjection.DynConcatenateString");


            Assert.That("1 ! 2", Is.EqualTo(DynConcatenateString.Concatenate("1","2")));

        }
    
#endif


    [Test]
        public void TestLateLibrarybind()
        {

            dynamic tBigIntType =
                new DynamicObjects.LateType(
                    "System.Numerics.BigInteger, System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

            if (tBigIntType.IsAvailable)
            {

                var one = tBigIntType.@new(1);
                var two = tBigIntType.@new(2);

                Assert.IsFalse(one.IsEven);
                Assert.AreEqual(true, two.IsEven);

                var tParsed = tBigIntType.Parse("4");

                Assert.AreEqual(true, tParsed.IsEven);



            }
            else
            {

                Assert.Fail("Big Int Didn't Load");


            }
        }
    }
}
