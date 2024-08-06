using Dynamitey.DynamicObjects;
using Dynamitey.SupportLibrary;

namespace Dynamitey.Tests
{
	[TestFixture(Category = "Impromptu")]
	public class Impromptu
	{
		public static readonly dynamic Interfacing = Dynamic.Curry(new Func<object, dynamic>(it => ImpromptuInterface.Impromptu.ActLike(it)));

		[Test]
		public void DictionaryInterfaceNullMethodsTest()
		{
			dynamic tNew = new DynamicObjects.Dictionary();
			ISimpleStringMethod tActsLike = ImpromptuInterface.Impromptu.ActLike(tNew);

			Assert.AreEqual(false, tActsLike.StartsWith("Te"));
		}

		[Test]
		public void FauxTypeTest()
		{
			var testProp = new Dictionary<string, Type>
			{
				{"test", typeof(bool)}
			};

			var propType = new PropretySpecType(testProp);
			var propMembers = propType.GetMemberNames();

			Assert.That(propMembers, Has.Member("test"));

			var realType = new RealType(typeof(ISimpeleClassProps));
			var realMembers = realType.GetMemberNames();

			Assert.That(realMembers, Has.Member("Prop2"));

			var aggrType = new AggreType(propType, realType);
			var aggrMembers = aggrType.GetMemberNames();

			Assert.That(aggrMembers, Has.Member("Prop2"));
			Assert.That(aggrMembers, Has.Member("test"));
		}

		[Test]
		public void PropertySpecTest()
		{
			var testProp = new Dictionary<string, Type>
			{
				{"test", typeof(bool)}
			};

			var baseObj = new DynamicObjects.Dictionary();
			var output = ImpromptuInterface.Impromptu.ActLikeProperties(baseObj, testProp);

			if (baseObj.TryTypeForName("test", out Type ot))
			{
				Assert.AreEqual(typeof(bool), ot);
			}
			else
			{
				Assert.Fail("Could not find property spec for member");
			}
		}

		[Test]
		public void InterfaceSpecTest()
		{
			var baseObj = new DynamicObjects.Dictionary();
			var output = ImpromptuInterface.Impromptu.ActLike<ISimpeleClassProps>(baseObj);

			if (baseObj.TryTypeForName("Prop2", out Type ot))
			{
				Assert.AreEqual(typeof(long), ot);
			}
			else
			{
				Assert.Fail("Could not find property spec for member");
			}
		}

		[Test]
		public void DictionaryCurriedAcctlikeNullMethodsTest()
		{
			ISimpleStringMethod tActsLike = Interfacing << new DynamicObjects.Dictionary();
			Assert.AreEqual(false, tActsLike.StartsWith("Te"));
		}

		public interface IBuilder
		{
			INest Nester(object props);
			INested Nester2(object props);

			[ImpromptuInterface.UseNamedArgument]
			INest Nester(string NameLevel1, INested Nested);

			INested Nester2([ImpromptuInterface.UseNamedArgument] string NameLevel2);
		}

		public interface INest
		{
			string NameLevel1 { get; set; }
			INested Nested { get; set; }
		}

		public interface INested
		{
			string NameLevel2 { get; }
		}

		[Test]
		public void TestBuilderActLikeAnon()
		{
			IBuilder New = Interfacing << Builder.New();

			var tNest = New.Nester(new
			{
				NameLevel1 = "Lvl1",
				Nested = New.Nester2(new
				{
					NameLevel2 = "Lvl2"
				})
			});

			Assert.AreEqual("Lvl1", tNest.NameLevel1);
			Assert.AreEqual("Lvl2", tNest.Nested.NameLevel2);
		}

		[Test]
		public void TestBuilderActLikeNamed()
		{
			IBuilder New = ImpromptuInterface.Impromptu.ActLike(Builder.New());

			var tNest = New.Nester(
				NameLevel1: "Lvl1",
				Nested: New.Nester2(
					NameLevel2: "Lvl2"
					)
				);

			Assert.AreEqual("Lvl1", tNest.NameLevel1);
			Assert.AreEqual("Lvl2", tNest.Nested.NameLevel2);
		}
	}
}
