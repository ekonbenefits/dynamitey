using Dynamitey.SupportLibrary;
using System.Collections;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Dynamitey.Tests
{
	[TestFixture]
	public class DynamicObjs
	{
		[Test]
		public void GetterAnonTest()
		{
			var tAnon = new { Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid() };

			dynamic tTest = new DynamicObjects.Get(tAnon);

			Assert.That(tAnon.Prop1, Is.EqualTo(tTest.Prop1));
			Assert.That(tAnon.Prop2, Is.EqualTo(tTest.Prop2));
			Assert.That(tAnon.Prop3, Is.EqualTo(tTest.Prop3));
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
			var tArray = new int[] { 1, 2, 3 };

			dynamic tTest = new DynamicObjects.Get(tArray);
			Dynamic.ApplyEquivalentType(tTest, typeof(IStringIntIndexer));

			Assert.That(tArray[2].ToString(), Is.EqualTo(tTest[2]));
		}

		[Test]
		public void GetterEventTest()
		{
			dynamic dynEvent = new DynamicObjects.Get(new PocoEvent());
			Dynamic.ApplyEquivalentType(dynEvent, typeof(IEvent));
			var tSet = false;
			EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
			dynEvent.Event += tActsLikeOnEvent;

			dynEvent.OnEvent(null, null);
			Assert.That(tSet, Is.True);
		}

		[Test]
		public void GetterEventTest2()
		{
			dynamic dynEvent = new DynamicObjects.Get(new PocoEvent());
			Dynamic.ApplyEquivalentType(dynEvent, typeof(IEvent));
			var tSet = false;
			EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
			dynEvent.Event += tActsLikeOnEvent;
			dynEvent.Event -= tActsLikeOnEvent;
			dynEvent.OnEvent(null, null);
			Assert.That(tSet, Is.False);
		}

		[Test]
		public void GetterDynamicTest()
		{
			dynamic tNew = new ExpandoObject();
			tNew.Prop1 = "Test";
			tNew.Prop2 = 42L;
			tNew.Prop3 = Guid.NewGuid();

			dynamic tTest = new DynamicObjects.Get(tNew);

			Assert.That(tNew.Prop1, Is.EqualTo(tTest.Prop1));
			Assert.That(tNew.Prop2, Is.EqualTo(tTest.Prop2));
			Assert.That(tNew.Prop3, Is.EqualTo(tTest.Prop3));
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
			var tAnon = new { Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid() };

			dynamic tTest = new TestForwarder(tAnon);

			Assert.That(tAnon.Prop1, Is.EqualTo(tTest.Prop1));
			Assert.That(tAnon.Prop2, Is.EqualTo(tTest.Prop2));
			Assert.That(tAnon.Prop3, Is.EqualTo(tTest.Prop3));
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

			Assert.That(tFwd.Create<ForwardGenericMethodsTestClass>(99).Value, Is.EqualTo("test99"));
		}

		[Test]
		public void ForwardDynamicTest()
		{
			dynamic tNew = new ExpandoObject();
			tNew.Prop1 = "Test";
			tNew.Prop2 = 42L;
			tNew.Prop3 = Guid.NewGuid();

			dynamic tTest = new TestForwarder(tNew);

			Assert.That(tNew.Prop1, Is.EqualTo(tTest.Prop1));
			Assert.That(tNew.Prop2, Is.EqualTo(tTest.Prop2));
			Assert.That(tNew.Prop3, Is.EqualTo(tTest.Prop3));
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
			Assert.That(tNew.Action3(), Is.EqualTo("test"));
			Assert.That(tNew.Action4(4), Is.EqualTo("test4"));
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
			Assert.That(tFwd.Action3(), Is.EqualTo("test"));
			Assert.That(tFwd.Action4(4), Is.EqualTo("test4"));
		}

		[Test]
		public void DictionaryMethodsOutTest()
		{
			dynamic tNew = new DynamicObjects.Dictionary();
			tNew.Func = new DynamicTryString(TestOut);

			Assert.That(tNew.Func(null, "Test", out string tOut), Is.True);
			Assert.That(tOut, Is.EqualTo("Test"));

			Assert.That(tNew.Func(null, 1, out string tOut2), Is.False);
			Assert.That(tOut2, Is.Null);
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
			Assert.That(tNew.Action3(), Is.EqualTo("Cat-test"));
		}

		[Test]
		public void DictionaryNullMethodsTest()
		{
			dynamic tNew = new DynamicObjects.Dictionary();
			Dynamic.ApplyEquivalentType(tNew, typeof(ISimpleStringMethod));

			Assert.That(tNew.StartsWith("Te"), Is.False);
		}

		[Test]
		public void DynamicDictionaryWrappedTest()
		{
			var tDictionary = new Dictionary<string, object>
			{
				{"Test1", 1},
				{"Test2", 2},
				{
					"TestD", new Dictionary<string, object>
					{
						{"TestA", "A"},
						{"TestB", "B"}
					}
				}
			};

			dynamic tNew = new DynamicObjects.Dictionary(tDictionary);

			Assert.That(tNew.Test1, Is.EqualTo(1));
			Assert.That(tNew.Test2, Is.EqualTo(2));
			Assert.That(tNew.TestD.TestA, Is.EqualTo("A"));
			Assert.That(tNew.TestD.TestB, Is.EqualTo("B"));
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
					"TestD", new Dictionary<string, object>
					{
						{"TestA", "A"},
						{"TestB", "B"}
					}
				}
			};

			dynamic tDynamic = new DynamicObjects.Dictionary(tDictionary);
			dynamic tNotDynamic = new DynamicObjects.Dictionary(tDictionary);

			Dynamic.ApplyEquivalentType(tDynamic, typeof(IDynamicDict));
			Dynamic.ApplyEquivalentType(tNotDynamic, typeof(INonDynamicDict));

			Assert.That(tDynamic, Is.EqualTo(tNotDynamic));

			Assert.That(tDynamic.Test1, Is.EqualTo(1));
			Assert.That(tDynamic.Test2, Is.EqualTo(2L));
			Assert.That(tDynamic.Test3, Is.EqualTo(TestEnum.One));
			Assert.That(tDynamic.Test4, Is.EqualTo(TestEnum.Two));

			Assert.That(tDynamic.TestD.TestA, Is.EqualTo("A"));
			Assert.That(tDynamic.TestD.TestB, Is.EqualTo("B"));

			Assert.That(tNotDynamic.Test1, Is.EqualTo(1));
			Assert.That(tNotDynamic.Test2, Is.EqualTo(2L));
			Assert.That(tNotDynamic.Test3, Is.EqualTo(TestEnum.One));
			Assert.That(tNotDynamic.Test4, Is.EqualTo(TestEnum.Two));

			Assert.That(tNotDynamic.TestD.GetType(), Is.EqualTo(typeof(Dictionary<string, object>)));
			Assert.That(tDynamic.TestD.GetType(), Is.EqualTo(typeof(DynamicObjects.Dictionary)));
		}

		[Test]
		public void DynamicObjectEqualsTest()
		{
			var tDictionary = new Dictionary<string, object>
			{
				{"Test1", 1},
				{"Test2", 2},
				{
					"TestD", new Dictionary<string, object>
					{
						{"TestA", "A"},
						{"TestB", "B"}
					}
				}
			};

			dynamic tDynamic = new DynamicObjects.Dictionary(tDictionary);
			dynamic tNotDynamic = new DynamicObjects.Dictionary(tDictionary);

			Dynamic.ApplyEquivalentType(tDynamic, typeof(IDynamicDict));
			Dynamic.ApplyEquivalentType(tNotDynamic, typeof(INonDynamicDict));

			Assert.That(tDynamic, Is.EqualTo(tNotDynamic));
			Assert.That(tDynamic, Is.EqualTo(tDictionary));
			Assert.That(tNotDynamic, Is.EqualTo(tDictionary));
		}

		[Test]
		public void DynamicAnnonymousWrapper()
		{
			var tData = new Dictionary<int, string> { { 1, "test" } };
			var tDyn = DynamicObjects.Get.Create(new
			{
				Test1 = 1,
				Test2 = "2",
				IsGreaterThan5 = Return<bool>.Arguments<int>(it => it > 5),
				ClearData = ReturnVoid.Arguments(() => tData.Clear())
			});

			Assert.That(tDyn.Test1, Is.EqualTo(1));
			Assert.That(tDyn.Test2, Is.EqualTo("2"));
			Assert.That(tDyn.IsGreaterThan5(6), Is.True);
			Assert.That(tDyn.IsGreaterThan5(4), Is.False);

			Assert.That(tData.Count, Is.EqualTo(1));
			tDyn.ClearData();
			Assert.That(tData.Count, Is.EqualTo(0));
		}

		[Test]
		public void TestAnonInterface()
		{
			dynamic tInterface = new DynamicObjects.Get(new
			{
				CopyArray = ReturnVoid.Arguments<Array, int>((ar, i) => Enumerable.Range(1, 10)),
				Count = 10,
				IsSynchronized = false,
				SyncRoot = this,
				GetEnumerator = Return<IEnumerator>.Arguments(() => Enumerable.Range(1, 10).GetEnumerator())
			});

			Dynamic.ApplyEquivalentType(tInterface, typeof(ICollection), typeof(IEnumerable));

			Assert.That(tInterface.Count, Is.EqualTo(10));
			Assert.That(tInterface.IsSynchronized, Is.False);
			Assert.That(tInterface.SyncRoot, Is.EqualTo(this));
			Assert.That(tInterface.GetEnumerator(), Is.InstanceOf<IEnumerator>());
		}

		[Test]
		public void TestBuilder()
		{
			var New = Builder.New<ExpandoObject>();

			var tExpando = New.Object(
				Test: "test1",
				Test2: "Test 2nd"
			);
			Assert.That(tExpando.Test, Is.EqualTo("test1"));
			Assert.That(tExpando.Test2, Is.EqualTo("Test 2nd"));

			dynamic NewD = new DynamicObjects.Builder<ExpandoObject>();

			var tExpandoNamedTest = NewD.Robot(
				LeftArm: "Rise",
				RightArm: "Clamp"
			);

			Assert.That(tExpandoNamedTest.LeftArm, Is.EqualTo("Rise"));
			Assert.That(tExpandoNamedTest.RightArm, Is.EqualTo("Clamp"));
		}

		[Test]
		public void TestSetupOtherTypes()
		{
			var New = Builder.New().Setup(
				Expando: typeof(ExpandoObject),
				Dict: typeof(DynamicObjects.Dictionary)
			);

			var tExpando = New.Expando(
				LeftArm: "Rise",
				RightArm: "Clamp"
			);

			var tDict = New.Dict(
				LeftArm: "RiseD",
				RightArm: "ClampD"
			);

			Assert.That(tExpando.LeftArm, Is.EqualTo("Rise"));
			Assert.That(tExpando.RightArm, Is.EqualTo("Clamp"));
			Assert.That(tExpando.GetType(), Is.EqualTo(typeof(ExpandoObject)));

			Assert.That(tDict.LeftArm, Is.EqualTo("RiseD"));
			Assert.That(tDict.RightArm, Is.EqualTo("ClampD"));
			Assert.That(tDict.GetType(), Is.EqualTo(typeof(DynamicObjects.Dictionary)));
		}

		[Test]
		public void TestClayFactorySyntax()
		{
			dynamic New = Builder.New();

			{
				var person = New.Person();
				person.FirstName = "Louis";
				person.LastName = "Dejardin";
				Assert.That(person.FirstName, Is.EqualTo("Louis"));
				Assert.That(person.LastName, Is.EqualTo("Dejardin"));
			}
			{
				var person = New.Person();
				person["FirstName"] = "Louis";
				person["LastName"] = "Dejardin";
				Assert.That(person.FirstName, Is.EqualTo("Louis"));
				Assert.That(person.LastName, Is.EqualTo("Dejardin"));
			}
			{
				var person = New.Person(
					FirstName: "Bertrand",
					LastName: "Le Roy"
				).Aliases("bleroy", "boudin");

				Assert.That(person.FirstName, Is.EqualTo("Bertrand"));
				Assert.That(person.LastName, Is.EqualTo("Le Roy"));
				Assert.That(person.Aliases[1], Is.EqualTo("boudin"));
			}
			{
				var person = New.Person()
								.FirstName("Louis")
								.LastName("Dejardin")
								.Aliases(new[] { "Lou" });

				Assert.That(person.FirstName, Is.EqualTo("Louis"));
				Assert.That(person.Aliases[0], Is.EqualTo("Lou"));
			}
			{
				var person = New.Person(new
				{
					FirstName = "Louis",
					LastName = "Dejardin"
				});
				Assert.That(person.FirstName, Is.EqualTo("Louis"));
				Assert.That(person.LastName, Is.EqualTo("Dejardin"));
			}
		}

		[Test]
		public void TestFactoryListSyntax()
		{
			dynamic New = Builder.New();

			// Test using Clay Syntax
			var people = New.Array(
				New.Person().FirstName("Louis").LastName("Dejardin"),
				New.Person().FirstName("Bertrand").LastName("Le Roy")
			);

			Assert.That(people[0].LastName, Is.EqualTo("Dejardin"));
			Assert.That(people[1].LastName, Is.EqualTo("Le Roy"));

			var people2 = new DynamicObjects.List
			{
				New.Robot(Name: "Bender"),
				New.Robot(Name: "RobotDevil")
			};

			Assert.That(people2[0].Name, Is.EqualTo("Bender"));
			Assert.That(people2[1].Name, Is.EqualTo("RobotDevil"));
		}

		[Test]
		public void TestQuicListSyntax()
		{
			var tList = Build.NewList("test", "one", "two");
			Assert.That(tList[1], Is.EqualTo("one"));

			var tList2 = Build.NewList("test", "one", "two", "three");
			Assert.That(tList2[3], Is.EqualTo("three"));
		}

		[Test]
		public void TestRecorder()
		{
			dynamic New = Builder.New<DynamicObjects.Recorder>();

			DynamicObjects.Recorder tRecording = New.Watson(Test: "One", Test2: 2, NameLast: "Watson");

			dynamic tVar = tRecording.ReplayOn(new ExpandoObject());

			Assert.That(tVar.Test, Is.EqualTo("One"));
			Assert.That(tVar.Test2, Is.EqualTo(2));
			Assert.That(tVar.NameLast, Is.EqualTo("Watson"));
		}

#if !NET6_0_OR_GREATER

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

            var parameters = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };

            CompilerResults cr = codeProvider.CompileAssemblyFromSource(parameters, code);

            dynamic DynConcatenateString = new DynamicObjects.LateType(cr.CompiledAssembly, "CodeInjection.DynConcatenateString");

            Assert.That(DynConcatenateString.Concatenate("1", "2"), Is.EqualTo("1 ! 2"));
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
				Assert.That(two.IsEven, Is.True);

				var tParsed = tBigIntType.Parse("4");

				Assert.That(tParsed.IsEven, Is.True);
			}
			else
			{
				Assert.Fail("Big Int Didn't Load");
			}
		}
	}
}
