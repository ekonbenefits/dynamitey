using System.Dynamic;
using NUnit.Framework;

namespace Dynamitey.Tests
{
    [TestFixture]
    public class ExpandoObjs : AssertionHelper
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


            Assert.AreEqual("test1", tExpandoNew.Test);
            Assert.AreEqual("Test 2nd", tExpandoNew.Test2);

            Assert.AreEqual(tExpando.Test, tExpandoNew.Test);
            Assert.AreEqual(tExpando.Test2, tExpandoNew.Test2);
            Assert.AreEqual(tExpando.GetType(), tExpandoNew.GetType());
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

            Assert.AreEqual("Rise", tExpandoNamedTestShortcut.LeftArm);
            Assert.AreEqual("Clamp", tExpandoNamedTestShortcut.RightArm);

            Assert.AreEqual(tExpandoNamedTest.LeftArm, tExpandoNamedTestShortcut.LeftArm);
            Assert.AreEqual(tExpandoNamedTest.RightArm, tExpandoNamedTestShortcut.RightArm);
            Assert.AreEqual(tExpandoNamedTest.GetType(), tExpandoNamedTestShortcut.GetType());
        }
    }
}
