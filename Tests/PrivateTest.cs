using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamitey.SupportLibrary1;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

namespace Dynamitey.Tests
{
    [TestFixture]
    public class PrivateTest : AssertionHelper
    {
  
        [Test]
        public void TestInvokePrivateMethod()
        {
            var tTest = new TestWithPrivateMethod();
            Assert.That(Dynamic.InvokeMember(tTest, "Test"), Is.EqualTo(3));
        }

        [Test]
        public void TestInvokePrivateMethodAcrossAssemblyBoundries()
        {
            var tTest = new PublicType();
            Assert.That(Dynamic.InvokeMember(tTest, "PrivateMethod", 3), Is.True);
        }

        [Test]
        public void TestInvokeInternalTypeMethodAcrossAssemblyBoundries()
        {
            var tTest = PublicType.InternalInstance;
            Assert.That(Dynamic.InvokeMember(tTest, "InternalMethod", 3), Is.True);
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
