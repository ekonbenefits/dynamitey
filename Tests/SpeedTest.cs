using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamitey.SupportLibrary;
using NUnit.Framework;
using Microsoft.FSharp.Reflection;

namespace Dynamitey.Tests
{
    [TestFixture]
    [Category("Performance")]
    public class SpeedTest:AssertionHelper
    {
        [OneTimeSetUp]
        public void WarmUpDlr()
        {
            Dynamic.InvokeMember(1, "ToString");
        }


        public TimeIt Timer;
        [SetUp]
        public void Setup()
        {
            Timer = new TimeIt();
        }


        [Test]
        public void PropPocoGetValueTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif

            var tSetValue = "1";
            var tAnon = new { TestGet = tSetValue };



            Timer.Action1 = () => { var tOut = Dynamic.InvokeGet(tAnon, "TestGet"); };

            var tPropertyInfo = tAnon.GetType().GetProperty("TestGet");
            Timer.Action2 = () =>
            {
                var tOut = tPropertyInfo.GetValue(tAnon, null);
            };

            var elapsed = Timer.Go(5 * TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


        [Test]
        public void CacheableGetValueTimed()
        {
            var tSetValue = "1";
            var tAnon = new PropPoco() { Prop1 = tSetValue };


            var tInvoke = new CacheableInvocation(InvocationKind.Get, "Prop1");
            Timer.Action1 = () => { var tOut = tInvoke.Invoke(tAnon); };

            var tPropertyInfo = tAnon.GetType().GetProperty("Prop1");
            Timer.Action2 = () =>
                                {
                                    var tOut = tPropertyInfo.GetValue(tAnon, null);
                                };

            var elapsed = Timer.Go(2*TimeIt.Million);


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }



        [Test]
        public void ConstructorTimed()
        {

            Timer.Action1 = (() => { var tOut = Dynamic.InvokeConstructor(typeof(Tuple<string>), "Test"); });
            Timer.Action2 = (() =>
            {
                var tOut = Activator.CreateInstance(typeof(Tuple<string>), "Test");
            });

            var elapsed = Timer.Go();


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }
        [Test]
        public void CacheableConstructorTimed()
        {

            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 1);
            Timer.Action1 = (() => { var tOut = tCachedInvoke.Invoke(typeof(Tuple<string>), "Test"); });
            Timer.Action2 = (() =>
            {
                var tOut = Activator.CreateInstance(typeof(Tuple<string>), "Test");
            });

             var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void ConstructorNoARgTimed()
        {
            Timer.Action1=(() => { var tOut = Dynamic.InvokeConstructor(typeof(List<string>)); });
            Timer.Action2=(() =>
            {
                var tOut = Activator.CreateInstance(typeof(List<string>));
            });
            Timer.Action3=(() =>
            {
                var tOut = Activator.CreateInstance<List<string>>();
            });
               
            var elapsed = Timer.GoThree();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Refelection Generic: " + elapsed.Item3);
            Console.WriteLine("Impromptu VS Reflection: {0:0.0} x slower", (double)elapsed.Item1.Ticks / elapsed.Item2.Ticks);

            Assert.Ignore("I don't think this is beatable at the moment");
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CachableConstructorNoARgTimed()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor);
            Timer.Action1=(() => { var tOut = tCachedInvoke.Invoke(typeof(List<string>)); });
            Timer.Action2=(() =>
            {
                var tOut = Activator.CreateInstance(typeof(List<string>));
            });
            Timer.Action3=(() =>
            {
                var tOut = Activator.CreateInstance<List<string>>();
            });
                    
            var elapsed = Timer.GoThree();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Refelection Generic: " + elapsed.Item3);
            Console.WriteLine("Impromptu VS Reflection: {0:0.0} x slower", (double)elapsed.Item1.Ticks / elapsed.Item2.Ticks);

            Assert.Ignore("I don't think this is beatable at the moment");
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


        [Test]
        public void ConstructorValueTypeTimed()
        {



            Timer.Action1=(() => { var tOut = Dynamic.InvokeConstructor(typeof(DateTime), 2010, 1, 20); });
            Timer.Action2=(() =>
            {
                var tOut = Activator.CreateInstance(typeof(DateTime), 2010, 1, 20);
            });

               
            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CachedConstructorValueTypeTimed()
        {


            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 3);
            Timer.Action1=(() => { var tOut = tCachedInvoke.Invoke(typeof(DateTime), 2010, 1, 20); });
            Timer.Action2=(() =>
            {
                var tOut = Activator.CreateInstance(typeof(DateTime), 2010, 1, 20);
            });

              var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


        [Test]
        public void MethodPocoGetValueTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif

            var tValue = 1;


            Timer.Action1=(() => { var tOut = Dynamic.InvokeMember(tValue, "ToString"); });
            var tMethodInfo = tValue.GetType().GetMethod("ToString", new Type[] { });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { });
            });

            var elapsed = Timer.Go(2* TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableMethodPocoGetValueTimed()
        {


            var tValue = 1;


            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "ToString");
            Timer.Action1=(() => { var tOut = tCachedInvoke.Invoke(tValue); });
            var tMethodInfo = tValue.GetType().GetMethod("ToString", new Type[] { });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { });
            });

                  var elapsed = Timer.Go(3* TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void GetStaticTimed()
        {

#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif


            var tStaticType = typeof(DateTime);
            var tTarget = InvokeContext.CreateStatic(tStaticType);
            Timer.Action1=(() => { var tOut = Dynamic.InvokeGet(tTarget, "Today"); });
            var tMethodInfo = typeof(DateTime).GetProperty("Today").GetGetMethod();
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { });
            });

            var elapsed = Timer.Go(3 * TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableGetStaticTimed()
        {

            var tStaticType = typeof(DateTime);
            var tContext = InvokeContext.CreateStatic(tStaticType);
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Get, "Today", context: tContext);

            Timer.Action1=(() =>
            {
                var tOut = tCachedInvoke.Invoke(tStaticType);
            });
            var tMethodInfo = typeof(DateTime).GetProperty("Today").GetGetMethod();

            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { });
            });


            var elapsed = Timer.Go(3 * TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void MethodStaticMethodValueTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            var tStaticType = typeof(DateTime);
            var tTarget =  InvokeContext.CreateStatic(tStaticType);
            string tDate = "01/20/2009";
            Timer.Action1=(() => { var tOut = Dynamic.InvokeMember(tTarget, "Parse", tDate); });
            var tMethodInfo = typeof(DateTime).GetMethod("Parse", new[] { typeof(string) });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { tDate });
            });

               var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableMethodStaticMethodValueTimed()
        {

            var tStaticType = typeof(DateTime);
            var tContext =  InvokeContext.CreateStatic(tStaticType);
            string tDate = "01/20/2009";

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Parse", argCount: 1,
                                                        context: tContext);
            Timer.Action1=(() => { var tOut = tCachedInvoke.Invoke(tStaticType, tDate); });
            var tMethodInfo = typeof(DateTime).GetMethod("Parse", new[] { typeof(string) });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tStaticType, new object[] { tDate });
            });

             var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

          [Test]
        public void MethodPocoGetValuePassNullTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif

            var tValue = new OverloadingMethPoco();


            Timer.Action1=(() => { var tOut = Dynamic.InvokeMember(tValue, "Func", null); });
            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object)});
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null});
            });

          
             var elapsed = Timer.Go(3 * TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableMethodPocoGetValuePassNullTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif

            var tValue = new OverloadingMethPoco();



            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount:1);

            Timer.Action1=(() => { var tOut = tCachedInvoke.Invoke(tValue, null); });
            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object) });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null });
            });
                
            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


        [Test]
        public void MethodPocoGetValuePassNullDoubleCallTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            var tValue = new OverloadingMethPoco();


            Timer.Action1=(() => { 
                var tOut = Dynamic.InvokeMember(tValue, "Func", null); 
                var tOut2 = Dynamic.InvokeMember(tValue, "Func", 2); });

            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object) });
            var tMethodInfo2 = tValue.GetType().GetMethod("Func", new Type[] { typeof(int) });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null });
                var tOut2 = tMethodInfo2.Invoke(tValue, new object[] { 2 });
            });

            var elapsed = Timer.Go(3 * TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableMethodPocoGetValuePassNullDoubleCallTimed()
        {
            var tValue = new OverloadingMethPoco();

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", 1);
            Timer.Action1=(() =>
            {
                var tOut = tCachedInvoke.Invoke(tValue, null);
                var tOut2 = tCachedInvoke.Invoke(tValue, 2);
            });

            var tMethodInfo = tValue.GetType().GetMethod("Func", new Type[] { typeof(object) });
            var tMethodInfo2 = tValue.GetType().GetMethod("Func", new Type[] { typeof(int) });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { null });
                var tOut2 = tMethodInfo2.Invoke(tValue, new object[] { 2 });
            });
                   
            var elapsed = Timer.Go();

             Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void MethodPocoGetValue4argsTimed()
        {

#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            var tValue = "test 123 45 string";



            Timer.Action1=(() => { var tOut = Dynamic.InvokeMember(tValue, "IndexOf", "45", 0, 14, StringComparison.InvariantCulture); });
            var tMethodInfo = tValue.GetType().GetMethod("IndexOf", new Type[] { typeof(string), typeof(int), typeof(int), typeof(StringComparison) });
            Timer.Action2=(() =>
                                        {
                                            var tOut = tMethodInfo.Invoke(tValue, new object[] { "45", 0, 14, StringComparison.InvariantCulture });
                                        });

               var elapsed = Timer.Go();

             Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableMethodPocoGetValue4argsTimed()
        {


            var tValue = "test 123 45 string";


            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "IndexOf", 4);
            Timer.Action1=(() =>
                                       {
                                           var tOut = tCachedInvoke.Invoke(tValue,"45", 0, 14, StringComparison.InvariantCulture);
                                       });
            var tMethodInfo = tValue.GetType().GetMethod("IndexOf", new Type[] { typeof(string), typeof(int), typeof(int), typeof(StringComparison) });
            Timer.Action2=(() =>
            {
                var tOut = tMethodInfo.Invoke(tValue, new object[] { "45", 0, 14, StringComparison.InvariantCulture });
            });

             var elapsed = Timer.Go();

             Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }
 

        [Test]
        public void MethodPocoVoidTimed()
        {

#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            var tValue = new Dictionary<object,object>();



            Timer.Action1=(() => Dynamic.InvokeMemberAction(tValue, "Clear"));
            var tMethodInfo = tValue.GetType().GetMethod("Clear", new Type[] { });
            Timer.Action2=(() => tMethodInfo.Invoke(tValue, new object[] { }));

             var elapsed = Timer.Go(5 * TimeIt.Million);

             Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableMethodPocoVoidTimed()
        {


            var tValue = new Dictionary<object, object>();

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMemberAction, "Clear");

            Timer.Action1=(() => tCachedInvoke.Invoke(tValue));
            var tMethodInfo = tValue.GetType().GetMethod("Clear", new Type[] { });
            Timer.Action2=(() => tMethodInfo.Invoke(tValue, new object[] { }));
               
            var elapsed = Timer.Go();
                  
            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

    


        [Test]
        public void SetTimed()
        {

#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            
            var tPoco1 = new PropPoco();
            var tPoco2 = new PropPoco();
            var tSetValue = "1";

            Timer.Action1 = () => Dynamic.InvokeSet(tPoco1, "Prop1", tSetValue);
            var tPropertyInfo = tPoco2.GetType().GetProperty("Prop1");
            Timer.Action2 = () => tPropertyInfo.SetValue(tPoco2, tSetValue, new object[] { });

            var elapsed = Timer.Go(5* TimeIt.Million);

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableSetTimed()
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
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void CacheableSetNullTimed()
        {
            var tPoco = new PropPoco();

            String tSetValue = null;
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Set, "Prop1");
            Timer.Action1 = (() => tCachedInvoke.Invoke(tPoco, tSetValue));
            var tPropertyInfo = tPoco.GetType().GetProperty("Prop1");
            Timer.Action2 = (() => tPropertyInfo.SetValue(tPoco, tSetValue, new object[] { }));

            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void SetNullTimed()
        {
#if DEBUG
            Assert.Ignore("Visual Studio slows down dynamic too much in debug mode");
#endif
            var tPoco1 = new PropPoco();
            var tPoco2 = new PropPoco();

            String tSetValue = null;

            Timer.Action1 = () => Dynamic.InvokeSet(tPoco1, "Prop1", tSetValue);
            var tPropertyInfo = tPoco2.GetType().GetProperty("Prop1");
            Timer.Action2 = () => tPropertyInfo.SetValue(tPoco2, tSetValue, new object[] { });

            var elapsed = Timer.Go(5 * TimeIt.Million);


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


            [Test]
        public void FastDynamicInvoke()
        {
            Func<int, bool> tFunc = it => it > 10;
             Timer.Action1 =(() => tFunc.FastDynamicInvoke(5));

              Timer.Action2 = (() => tFunc.DynamicInvoke(5));

  
            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void FastDynamicInvokeAction()
        {
            Action<int> tFunc = it => it.ToString();
              Timer.Action1 = (() => tFunc.FastDynamicInvoke(5));

             Timer.Action2 = (() => tFunc.DynamicInvoke(5));
            
            var elapsed = Timer.Go();

            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


        [Test]
        public void IsTupleTimed()
        {

            object tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);

            Timer.Action1 = () => Tupler.IsTuple(tup);

            Timer.Action2 = () => FSharpType.IsTuple(tup.GetType());

            var elapsed = Timer.Go();


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("FSharp Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS FSharp Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }


        [Test]
        public void TupleIndexTimed()
        {

            object tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);

            Timer.Action1 = () => Tupler.Index(tup,14);

            Timer.Action2 = () => FSharpValue.GetTupleField(tup,14);

            var elapsed = Timer.Go(50000);


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("FSharp Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS FSharp Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }
      

        [Test]
        public void TupleToListTimed()
        {

            object tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);

            Timer.Action1 = () => Tupler.ToList(tup);
        
            Timer.Action2 = () => FSharpValue.GetTupleFields(tup).ToList();

            var elapsed = Timer.Go(50000);


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("FSharp Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS FSharp Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);
        }

        [Test]
        public void ListToTupleTimed()
        {
            var list = new object[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20};

            Timer.Action1 = () => Tupler.ToTuple(list);

            Timer.Action2 = () =>
                                {
                                    var types = list.Select(it => it.GetType()).ToArray();
                                    var tupType = FSharpType.MakeTupleType(types);
                                    FSharpValue.MakeTuple(list, tupType);
                                };

            var elapsed = Timer.Go(50000);


            Console.WriteLine("Impromptu: " + elapsed.Item1);
            Console.WriteLine("FSharp Refelection: " + elapsed.Item2);
            Console.WriteLine("Impromptu VS FSharp Reflection: {0}", TimeIt.RelativeSpeed(elapsed));
            Assert.Less(elapsed.Item1, elapsed.Item2);

        }
    }
}
