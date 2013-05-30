using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamitey.SupportLibrary1;
using NUnit.Framework;
using System.Diagnostics;

namespace Dynamitey.Tests
{
    [TestFixture]
    [Category("Performance")]
    public class SpeedTest:AssertionHelper
    {

    

        public class TimeIt
        {

            private Stopwatch _watch1;
            private Stopwatch _watch2;
            private bool _skipInitializationCosts;
            public TimeIt(bool skipInitializationCosts = false)
            {
                _watch1 = new Stopwatch();
                _watch2 = new Stopwatch();
                _skipInitializationCosts = skipInitializationCosts;
            }

            public Tuple<TimeSpan, TimeSpan> Go(int iteration = 1000000)
            {
                if (_skipInitializationCosts)
                {
                    iteration++;
                }

                for (int i = 0; i < iteration; i++)
                {
                    _watch1.Start();
                    Action1();
                    _watch1.Stop();
                    _watch2.Start();
                    Action2();
                    _watch2.Stop();
                    if (i == 0 && _skipInitializationCosts)
                    {
                        _watch1.Reset();
                        _watch2.Reset();
                    }
                }

                return Tuple.Create(_watch1.Elapsed, _watch2.Elapsed);
            }
            
            public Action Action1 { get; set; }
            public Action Action2 { get; set; }
        }


        [TestFixtureSetUp]
        public void WarmUpDlr()
        {
            dynamic i = 1;
            i.ToString();
        }


        public TimeIt Timer;
        [SetUp]
        public void Setup()
        {
            Timer = new TimeIt();
        }


        [Test]
        public void TestPropPocoGetValueTimed()
        {

           



            var tSetValue = "1";
            var tAnon = new { Test = tSetValue };



            Timer.Action1= () => { var tOut = Dynamic.InvokeGet(tAnon, "Test"); };

            var tPropertyInfo = tAnon.GetType().GetProperty("Test");
            Timer.Action2 = () =>
            {
                var tOut = tPropertyInfo.GetValue(tAnon, null);
            };

            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)elapsed.Item2.Ticks / elapsed.Item1.Ticks);
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }



        [Test]
        public void TestSetTimed()
        {

           
            
            var tPoco1 = new PropPoco();
            var tPoco2 = new PropPoco();
            var tSetValue = "1";

            Timer.Action1 = () => Dynamic.InvokeSet(tPoco1, "Prop1", tSetValue);
            var tPropertyInfo = tPoco2.GetType().GetProperty("Prop1");
            Timer.Action2 = () => tPropertyInfo.SetValue(tPoco2, tSetValue, new object[] { });

            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)elapsed.Item2.Ticks / elapsed.Item1.Ticks);
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void TestCacheableSetTimed()
        {

            var tPoco1 = new PropPoco();
            var tPoco2 = new PropPoco();

            var tSetValue = "1";

            var tCacheable = new CacheableInvocation(InvocationKind.Set, "Prop1");
            Timer.Action1 = () => tCacheable.Invoke(tPoco1, tSetValue);

            var tPropertyInfo = tPoco2.GetType().GetProperty("Prop1");
            Timer.Action2 = () => tPropertyInfo.SetValue(tPoco2, tSetValue, new object[] { });

            var elapsed = Timer.Go();


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)elapsed.Item2.Ticks / elapsed.Item1.Ticks);
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void TestSetNullTimed()
        {

            var tPoco1 = new PropPoco();
            var tPoco2 = new PropPoco();

            String tSetValue = null;

            Timer.Action1 = () => Dynamic.InvokeSet(tPoco1, "Prop1", tSetValue);
            var tPropertyInfo = tPoco2.GetType().GetProperty("Prop1");
            Timer.Action2 = () => tPropertyInfo.SetValue(tPoco2, tSetValue, new object[] { });

            var elapsed = Timer.Go();


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0:0.0} x faster", (double)elapsed.Item2.Ticks / elapsed.Item1.Ticks);
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

    }
}
