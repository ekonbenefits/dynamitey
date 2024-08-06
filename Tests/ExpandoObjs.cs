using System.Dynamic;

namespace Dynamitey.Tests
{
	[TestFixture]
	public class ExpandoObjs
	{
		[Test]
		public void TestExpando()
		{
			var New = Builder.New<ExpandoObject>();

			var tExpando = New.Object(
				Test: "test1",
				Test2: "Test 2nd"
			);

			var tExpandoNew = Expando.New(
				Test: "test1",
				Test2: "Test 2nd"
			);

			Assert.That(tExpandoNew.Test, Is.EqualTo("test1"));
			Assert.That(tExpandoNew.Test2, Is.EqualTo("Test 2nd"));

			Assert.That(tExpando.Test, Is.EqualTo(tExpandoNew.Test));
			Assert.That(tExpando.Test2, Is.EqualTo(tExpandoNew.Test2));
			Assert.That(tExpando.GetType(), Is.EqualTo(tExpandoNew.GetType()));
		}

		[Test]
		public void TestExpando2()
		{
			dynamic NewD = new DynamicObjects.Builder<ExpandoObject>();

			var tExpandoNamedTest = NewD.Robot(
				LeftArm: "Rise",
				RightArm: "Clamp"
			);

			dynamic NewE = new Expando();

			var tExpandoNamedTestShortcut = NewE.Robot(
				LeftArm: "Rise",
				RightArm: "Clamp"
			);

			Assert.That(tExpandoNamedTestShortcut.LeftArm, Is.EqualTo("Rise"));
			Assert.That(tExpandoNamedTestShortcut.RightArm, Is.EqualTo("Clamp"));

			Assert.That(tExpandoNamedTest.LeftArm, Is.EqualTo(tExpandoNamedTestShortcut.LeftArm));
			Assert.That(tExpandoNamedTest.RightArm, Is.EqualTo(tExpandoNamedTestShortcut.RightArm));
			Assert.That(tExpandoNamedTest.GetType(), Is.EqualTo(tExpandoNamedTestShortcut.GetType()));
		}
	}
}
