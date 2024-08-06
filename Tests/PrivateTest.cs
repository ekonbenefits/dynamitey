using Dynamitey.SupportLibrary;
using Microsoft.CSharp.RuntimeBinder;

namespace Dynamitey.Tests
{
    [TestFixture]
    public class PrivateTest 
    {
  
        [Test]
        public void TestInvokePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            Assert.That((object)Dynamic.InvokeMember(tTest, "Test"), Is.EqualTo(3));
        }

        [Test]
        public void TestInvokePrivateMethodAcrossAssemblyBoundries()
        {
            var tTest = new PublicType();
            Assert.That((object)Dynamic.InvokeMember(tTest, "PrivateMethod", 3), Is.True);
        }

        [Test]
        public void TestInvokeInternalTypeMethodAcrossAssemblyBoundries()
        {
            var tTest = PublicType.InternalInstance;
            Assert.That((object)Dynamic.InvokeMember(tTest, "InternalMethod", 3), Is.True);
        }

        [Test]
        public void TestInvokeDoNotExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var context = InvokeContext.CreateContext;
            Assert.That(() => Dynamic.InvokeMember(context(tTest,this), "Test"), Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void TestCacheableDoNotExposePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Test");
            Assert.That(() => tCachedInvoke.Invoke(tTest), Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void TestCacheableExposePrivateMethodViaInstance()
        {
            var tTest = new TestWithPrivateMethod();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Test", context: tTest);
            Assert.That(tCachedInvoke.Invoke(tTest), Is.EqualTo(3));
        }

        [Test]
        public void TestCacheableExposePrivateMethodViaType()
        {
            var tTest = new TestWithPrivateMethod();
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Test", context: typeof(TestWithPrivateMethod));
            Assert.That( tCachedInvoke.Invoke(tTest), Is.EqualTo(3));
        }
    }

    public class TestWithPrivateMethod
    {
        private int Test()
        {
            return 3;
        }
    }
}
