using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using Dynamitey.SupportLibrary;
using Microsoft.CSharp.RuntimeBinder;
using Moq;
using NUnit.Framework;
using System.Globalization;

namespace Dynamitey.Tests
{
    public class Invoke:AssertionHelper
    {
        [TestFixtureTearDown]
        public void DestroyCaches()
        {
            Dynamic.ClearCaches();
        }


        [Test]
        public void TestDynamicSet()
        {
            dynamic tExpando = new ExpandoObject();

            var tSetValue = "1";

            Dynamic.InvokeSet(tExpando, "Test", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test);

        }



        [Test]
        public void TestPocoSet()
        {
            var tPoco = new PropPoco();

            var tSetValue = "1";

            Dynamic.InvokeSet(tPoco, "Prop1", tSetValue);

            Assert.AreEqual(tSetValue, tPoco.Prop1);

        }


        [Test]
        public void TestStructSet()
        {
            object tPoco = new PropStruct();

            var tSetValue = "1";

            Dynamic.InvokeSet(tPoco, "Prop1", tSetValue);

            Assert.AreEqual(tSetValue, ((PropStruct)tPoco).Prop1);

        }

        [Test]
        public void TestCacheableDyanmicSetAndPocoSetAndSetNull()
        {
            dynamic tExpando = new ExpandoObject();
            var tSetValueD = "4";


            var tCachedInvoke = new CacheableInvocation(InvocationKind.Set, "Prop1");

            tCachedInvoke.Invoke((object)tExpando, tSetValueD);


            Assert.AreEqual(tSetValueD, tExpando.Prop1);

            var tPoco = new PropPoco();
            var tSetValue = "1";

            tCachedInvoke.Invoke(tPoco, tSetValue);

            Assert.AreEqual(tSetValue, tPoco.Prop1);

            String tSetValue2 = null;

            tCachedInvoke.Invoke(tPoco, tSetValue2);

            Assert.AreEqual(tSetValue2, tPoco.Prop1);
        }



        [Test]
        public void TestConvert()
        {
            var tEl = new XElement("Test", "45");

            var tCast = Dynamic.InvokeConvert(tEl, typeof(int), @explicit: true);

            Assert.AreEqual(typeof(int), tCast.GetType());
            Assert.AreEqual(45, tCast);
        }

        [Test]
        public void TestConvertCacheable()
        {
            var tEl = new XElement("Test", "45");

            var tCacheInvoke = new CacheableInvocation(InvocationKind.Convert, convertType: typeof(int),
                                                       convertExplicit: true);
            var tCast = tCacheInvoke.Invoke(tEl);

            Assert.AreEqual(typeof(int), tCast.GetType());
            Assert.AreEqual(45, tCast);
        }

        [Test]
        public void TestConstruct()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(List<object>), new object[]
                                                                              {
                                                                                  new string[] {"one", "two", "three"}
                                                                              });

            Assert.AreEqual("two", tCast[1]);
        }


        [Test]
        public void TestCacheableConstruct()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 1);

            dynamic tCast = tCachedInvoke.Invoke(typeof(List<object>), new object[]
                                                                              {
                                                                                  new string[] {"one", "two", "three"}
                                                                              });

            Assert.AreEqual("two", tCast[1]);
        }

        

        [Test]
        public void TestConstructOptional()
        {
            var argname = InvokeArg.Create;


            PocoOptConstructor tCast = Dynamic.InvokeConstructor(typeof(PocoOptConstructor), argname("three", "3"));

            Assert.AreEqual("-1", tCast.One);
            Assert.AreEqual("-2", tCast.Two);
            Assert.AreEqual("3", tCast.Three);
        }

        [Test]
        public void TestCacheableConstructOptional()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 1, argNames: new[] { "three" });

            var tCast = (PocoOptConstructor)tCachedInvoke.Invoke(typeof(PocoOptConstructor), "3");

            Assert.AreEqual("-1", tCast.One);
            Assert.AreEqual("-2", tCast.Two);
            Assert.AreEqual("3", tCast.Three);
        }

        [Test]
        public void TestOptionalArgumentActivationNoneAndCacheable()
        {

            Assert.Throws<MissingMethodException>(() => Activator.CreateInstance<DynamicObjects.List>());

            var tList = Dynamic.InvokeConstructor(typeof(DynamicObjects.List));


            Assert.AreEqual(typeof(DynamicObjects.List), tList.GetType());

            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor);

            var tList1 = tCachedInvoke.Invoke(typeof(DynamicObjects.List));


            Assert.AreEqual(typeof(DynamicObjects.List), tList1.GetType());
        }



        [Test]
        public void TestConstructValueType()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(DateTime), 2009, 1, 20);

            Assert.AreEqual(20, tCast.Day);

        }

        [Test]
        public void TestCacheableConstructValueType()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor, argCount: 3);
            dynamic tCast = tCachedInvoke.Invoke(typeof(DateTime), 2009, 1, 20);

            Assert.AreEqual(20, tCast.Day);

        }

        [Test]
        public void TestConstructValueTypeJustDynamic()
        {
            dynamic day = 20;
            dynamic year = 2009;
            dynamic month = 1;
            var tCast = new DateTime(year, month, day);
            DateTime tDate = tCast;
            Assert.AreEqual(20, tDate.Day);
        }

        [Test]
        public void TestConstructprimativetype()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(Int32));

            Assert.AreEqual(default(Int32), tCast);
        }


        [Test]
        public void TestConstructDateTimeNoParams()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(DateTime));

            Assert.AreEqual(default(DateTime), tCast);
        }

        [Test]
        public void TestConstructOBjectNoParams()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(object));

            Assert.AreEqual(typeof(object), tCast.GetType());
        }

        [Test]
        public void TestConstructNullableprimativetype()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(Nullable<Int32>));

            Assert.AreEqual(null, tCast);
        }

        [Test]
        public void TestConstructGuid()
        {
            var tCast = Dynamic.InvokeConstructor(typeof(Guid));

            Assert.AreEqual(default(Guid), tCast);
        }

        [Test]
        public void TestCacheablePrimativeDateTimeObjectNullableAndGuidNoParams()
        {
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Constructor);

            dynamic tCast = tCachedInvoke.Invoke(typeof(Int32));

            Assert.AreEqual(default(Int32), tCast);

            tCast = tCachedInvoke.Invoke(typeof(DateTime));

            Assert.AreEqual(default(DateTime), tCast);

            tCast = tCachedInvoke.Invoke(typeof(List<string>));

            Assert.AreEqual(typeof(List<string>), tCast.GetType());

            tCast = tCachedInvoke.Invoke(typeof(object));

            Assert.AreEqual(typeof(object), tCast.GetType());

            tCast = tCachedInvoke.Invoke(typeof(Nullable<Int32>));

            Assert.AreEqual(null, tCast);

            tCast = tCachedInvoke.Invoke(typeof(Guid));

            Assert.AreEqual(default(Guid), tCast);
        }


        [Test]
        public void TestStaticCall()
        {
            var @static = InvokeContext.CreateStatic;
            var generic = InvokeMemberName.Create;

            var tOut = Dynamic.InvokeMember(@static(typeof(StaticType)),
                                              generic("Create",new[]{typeof(bool)}), 1);
            Assert.AreEqual(false, tOut);
        }

        [Test]
        public void TestCacheableStaticCall()
        {
            var @static = InvokeContext.CreateStatic;
            var generic = InvokeMemberName.Create;

            var tCached = new CacheableInvocation(InvocationKind.InvokeMember, generic("Create",new[]{typeof(bool)}) , argCount: 1,
                                    context: @static(typeof(StaticType)));

            var tOut = tCached.Invoke(typeof(StaticType), 1);
            Assert.AreEqual(false, tOut);
        }
        
        private class TestClass
        {
            public static int StaticProperty { get; set; }
        }

        [Test]
        public void TestStaticPropertySetFollowedByGetTest()
        {
            var staticContext = InvokeContext.CreateStatic;
            Dynamic.InvokeSet(staticContext(typeof(TestClass)), "StaticProperty", 42);
            var tOut = Dynamic.InvokeGet(staticContext(typeof(TestClass)), "StaticProperty");
            Assert.AreEqual(42, tOut);
        }
        
     
        [Test]
        public void TestImplicitConvert()
        {
            var tEl = 45;

            var tCast = Dynamic.InvokeConvert(tEl, typeof(long));

            Assert.AreEqual(typeof(long), tCast.GetType());
        }

        [Test]
        public void TestCoerceConverterColor()
        {
            var colorString = "PaleVioletRed";

            var color =Dynamic.CoerceConvert(colorString, typeof (Color));

            Assert.That((object)color,Is.TypeOf<Color>());
            Assert.That((object)color, Is.EqualTo(Color.PaleVioletRed));

        }

        [Test]
        public void TestCoerceConverterDBNULL()
        {
            var tEl = DBNull.Value;

            var tCast = Dynamic.CoerceConvert(tEl, typeof(long));

            Assert.AreEqual(typeof(long), tCast.GetType());

            var tCast2 = Dynamic.CoerceConvert(tEl, typeof(string));
            Assert.AreEqual(null, tCast2);

            Assert.AreNotEqual(null, tEl);
        }



        [Test]
        public void TestCacheableImplicitConvert()
        {
            var tEl = 45;

            var tCachedInvoke = CacheableInvocation.CreateConvert(typeof(long));

            var tCast = tCachedInvoke.Invoke(tEl);

            Assert.AreEqual(typeof(long), tCast.GetType());
        }


        [Test]
        public void TestCacheableGet()
        {
            var tCached = new CacheableInvocation(InvocationKind.Get, "Prop1");

            var tSetValue = "1";
            var tAnon = new PropPoco { Prop1 = tSetValue };

            var tOut = tCached.Invoke(tAnon);
            Assert.AreEqual(tSetValue, tOut);

            var tSetValue2 = "2";
            tAnon = new PropPoco { Prop1 = tSetValue2 };


            var tOut2 = tCached.Invoke(tAnon);


            Assert.AreEqual(tSetValue2, tOut2);

        }

        [Test]
        public void TestGetIndexer()
        {

            dynamic tSetValue = "1";
            var tAnon = new[] { tSetValue, "2" };


            string tOut = Dynamic.InvokeGetIndex(tAnon, 0);

            Assert.AreEqual(tSetValue, tOut);

        }


        [Test]
        public void TestGetIndexerValue()
        {


            var tAnon = new int[] { 1, 2 };


            int tOut = Dynamic.InvokeGetIndex(tAnon, 1);

            Assert.AreEqual(tAnon[1], tOut);

        }


        [Test]
        public void TestGetLengthArray()
        {
            var tAnon = new[] { "1", "2" };


            int tOut = Dynamic.InvokeGet(tAnon, "Length");

            Assert.AreEqual(2, tOut);

        }

        [Test]
        public void TestGetIndexerArray()
        {
            dynamic tSetValue = "1";
            var tAnon = new List<string> { tSetValue, "2" };


            string tOut = Dynamic.InvokeGetIndex(tAnon, 0);

            Assert.AreEqual(tSetValue, tOut);

        }


        [Test]
        public void TestCacheableIndexer()
        {

            var tStrings = new[] { "1", "2" };

            var tCachedInvoke = new CacheableInvocation(InvocationKind.GetIndex, argCount: 1);

            var tOut = (string)tCachedInvoke.Invoke(tStrings, 0);

            Assert.AreEqual(tStrings[0], tOut);

            var tOut2 = (string)tCachedInvoke.Invoke(tStrings, 1);

            Assert.AreEqual(tStrings[1], tOut2);

            var tInts = new int[] { 3, 4 };

            var tOut3 = (int)tCachedInvoke.Invoke(tInts, 0);

            Assert.AreEqual(tInts[0], tOut3);

            var tOut4 = (int)tCachedInvoke.Invoke(tInts, 1);

            Assert.AreEqual(tInts[1], tOut4);

            var tList = new List<string> { "5", "6" };

            var tOut5 = (string)tCachedInvoke.Invoke(tList, 0);

            Assert.AreEqual(tList[0], tOut5);

            var tOut6 = (string)tCachedInvoke.Invoke(tList, 0);

            Assert.AreEqual(tList[0], tOut6);
        }

        [Test]
        public void TestSetIndexer()
        {

            dynamic tSetValue = "3";
            var tAnon = new List<string> { "1", "2" };

            Dynamic.InvokeSetIndex(tAnon, 0, tSetValue);

            Assert.AreEqual(tSetValue, tAnon[0]);

        }

        [Test]
        public void TestCacheableSetIndexer()
        {

            dynamic tSetValue = "3";
            var tList = new List<string> { "1", "2" };


            var tCachedInvoke = new CacheableInvocation(InvocationKind.SetIndex, argCount: 2);

            tCachedInvoke.Invoke(tList, 0, tSetValue);

            Assert.AreEqual(tSetValue, tList[0]);

        }



        [Test]
        public void TestMethodDynamicPassAndGetValue()
        {
            dynamic tExpando = new ExpandoObject();
            tExpando.Func = new Func<int, string>(it => it.ToString());

            var tValue = 1;

            var tOut = Dynamic.InvokeMember(tExpando, "Func", tValue);

            Assert.AreEqual(tValue.ToString(), tOut);
        }


        [Test]
        public void TestCacheableMethodDynamicPassAndGetValue()
        {
            dynamic tExpando = new ExpandoObject();
            tExpando.Func = new Func<int, string>(it => it.ToString());

            var tValue = 1;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", 1);

            var tOut = tCachedInvoke.Invoke((object)tExpando, tValue);

            Assert.AreEqual(tValue.ToString(), tOut);
        }


        [Test]
        public void TestMethodStaticOverloadingPassAndGetValue()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tOut = Dynamic.InvokeMember(tPoco, "Func", tValue);

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = Dynamic.InvokeMember(tPoco, "Func", 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = Dynamic.InvokeMember(tPoco, "Func", new { Anon = 1 });

            Assert.AreEqual("object", tOut3);
        }

        [Test]
        public void TestCachedMethodStaticOverloadingPassAndGetValue()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 1);


            var tOut = tCachedInvoke.Invoke(tPoco, tValue);

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = tCachedInvoke.Invoke(tPoco, 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = tCachedInvoke.Invoke(tPoco, new { Anon = 1 });

            Assert.AreEqual("object", tOut3);
        }

        [Test]
        public void TestMethodPocoOverloadingPassAndGetValueArg()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tOut = Dynamic.InvokeMember(tPoco, "Func", new InvokeArg("arg", tValue));

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = Dynamic.InvokeMember(tPoco, "Func", 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = Dynamic.InvokeMember(tPoco, "Func", new { Anon = 1 });

            Assert.AreEqual("object", tOut3);
        }

        [Test]
        public void TestMethodPocoOverloadingPassAndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var arg = InvokeArg.Create;

            var tOut = Dynamic.InvokeMember(tPoco, "Func", arg("two", tValue));

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }

        [Test]
        public void TestCacheableMethodPocoOverloadingPassAndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tCachedIvnocation = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 1,
                                                            argNames: new[] { "two" });

            var tOut = tCachedIvnocation.Invoke(tPoco, tValue);

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }

        [Test]
        public void TestCacheableMethodPocoOverloadingPassAndGetValueArgPostiionalOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue1 = 1;
            var tValue2 = 2;

            var tCachedIvnocation = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 2,
                                                            argNames: new[] { "two" });

            var tOut = tCachedIvnocation.Invoke(tPoco, tValue1, tValue2);

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }

        [Test]
        public void TestMethodPocoOverloadingPass2AndGetValueArgOptional()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var arg = InvokeArg.Create;

            var tOut = Dynamic.InvokeMember(tPoco, "Func", arg("two", tValue), arg("one", tValue));

            Assert.AreEqual("object named", tOut);

            Assert.AreEqual("object named", (object)tOut);
        }

        [Test]
        public void TestMethodPocoOverloadingPassAndGetValueNull()
        {
            var tPoco = new OverloadingMethPoco();

            var tValue = 1;

            var tOut = Dynamic.InvokeMember(tPoco, "Func", tValue);

            Assert.AreEqual("int", tOut);

            Assert.AreEqual("int", (object)tOut); //should still be int because this uses runtime type


            var tOut2 = Dynamic.InvokeMember(tPoco, "Func", 1m);

            Assert.AreEqual("object", tOut2);

            var tOut3 = Dynamic.InvokeMember(tPoco, "Func", null);

            Assert.AreEqual("object", tOut3);

            var tOut4 = Dynamic.InvokeMember(tPoco, "Func", null, null, "test", null, null, null);

            Assert.AreEqual("object 6", tOut4);

            var tOut5 = Dynamic.InvokeMember(tPoco, "Func", null, null, null, null, null, null);

            Assert.AreEqual("object 6", tOut5);
        }

        /// <summary>
        /// To dynamically invoke a method with out or ref parameters you need to know the signature
        /// </summary>
        [Test]
        public void TestOutMethod()
        {



            string tResult = String.Empty;

            var tPoco = new MethOutPoco();


            var tName = "Func";
            var tContext = GetType();
            var tBinder =
                Binder.InvokeMember(CSharpBinderFlags.None, tName, null, tContext,
                                            new[]
                                                {
                                                    CSharpArgumentInfo.Create(
                                                        CSharpArgumentInfoFlags.None, null),
                                                    CSharpArgumentInfo.Create(
                                                        CSharpArgumentInfoFlags.IsOut |
                                                        CSharpArgumentInfoFlags.UseCompileTimeType, null)
                                                });


            var tSite = Dynamic.CreateCallSite<DynamicTryString>(tBinder, tName, tContext);


            tSite.Target.Invoke(tSite, tPoco, out tResult);

            Assert.AreEqual("success", tResult);

        }


        [Test]
        public void TestMethodDynamicPassVoid()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);



            Dynamic.InvokeMemberAction(tExpando, "Action", tValue);

            Assert.AreEqual(tValue, tTest);
        }

        [Test]
        public void TestCacheableMethodDynamicPassVoid()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMemberAction, "Action", argCount: 1);

            tCachedInvoke.Invoke((object)tExpando, tValue);

            Assert.AreEqual(tValue, tTest);
        }

        [Test]
        public void TestCacheableMethodDynamicUnknowns()
        {
            var tTest = "Wrong";

            var tValue = "Correct";

            dynamic tExpando = new ExpandoObject();
            tExpando.Action = new Action<string>(it => tTest = it);
            tExpando.Func = new Func<string, string>(it => it);

            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMemberUnknown, "Action", argCount: 1);

            tCachedInvoke.Invoke((object)tExpando, tValue);

            Assert.AreEqual(tValue, tTest);

            var tCachedInvoke2 = new CacheableInvocation(InvocationKind.InvokeMemberUnknown, "Func", argCount: 1);

            var Test2 = tCachedInvoke2.Invoke((object)tExpando, tValue);

            Assert.AreEqual(tValue, Test2);
        }



        [Test]
        public void TestMethodPocoGetValue()
        {


            var tValue = 1;

            var tOut = Dynamic.InvokeMember(tValue, "ToString");

            Assert.AreEqual(tValue.ToString(), tOut);
        }



        [Test]
        public void TestMethodPocoPassAndGetValue()
        {


            HelpTestPocoPassAndGetValue("Test", "Te");


            HelpTestPocoPassAndGetValue("Test", "st");
        }

        private void HelpTestPocoPassAndGetValue(string tValue, string tParam)
        {
            var tExpected = tValue.StartsWith(tParam);

            var tOut = Dynamic.InvokeMember(tValue, "StartsWith", tParam);

            Assert.AreEqual(tExpected, tOut);
        }


        [Test]
        public void TestGetDynamic()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;



            var tOut = Dynamic.InvokeGet(tExpando, "Test");

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestGetDynamicChained()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = new ExpandoObject();
            tExpando.Test.Test2 = new ExpandoObject();
            tExpando.Test.Test2.Test3 = tSetValue;


            var tOut = Dynamic.InvokeGetChain(tExpando, "Test.Test2.Test3");

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestGetDynamicChainedWithIndexes()
        {

            var tSetValue = "1";
            dynamic tExpando = Build.NewObject(
                Test: Build.NewObject(
                        Test2: Build.NewList(
                        Build.NewObject(Test3: Build.NewObject(Test4: tSetValue))
                        )
                    )
                );



            var tOut = Dynamic.InvokeGetChain(tExpando, "Test.Test2[0].Test3['Test4']");

            Assert.AreEqual(tSetValue, tOut);
        }


        [Test]
        public void TestSetDynamicChained()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = new ExpandoObject();
            tExpando.Test.Test2 = new ExpandoObject();


            Dynamic.InvokeSetChain(tExpando, "Test.Test2.Test3", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test.Test2.Test3);
        }

        [Test]
        public void TestSetDynamicChainedWithInexes()
        {
            var tSetValue = "1";
            dynamic tExpando = Build.NewObject(
                Test: Build.NewObject(
                        Test2: Build.NewList(
                        Build.NewObject(Test3: Build.NewObject())
                        )
                    )
                );


            var tOut = Dynamic.InvokeSetChain(tExpando, "Test.Test2[0].Test3['Test4']", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test.Test2[0].Test3["Test4"]);

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestSetDynamicAllDict()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = new ExpandoObject();
            tExpando.Test.Test2 = new ExpandoObject();


            Dynamic.InvokeSetAll(tExpando, new Dictionary<string, object> { { "Test.Test2.Test3", tSetValue }, { "One", 1 }, { "Two", 2 } });

            Dynamic.InvokeSetChain(tExpando, "Test.Test2.Test3", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test.Test2.Test3);
            Assert.AreEqual(1, tExpando.One);
            Assert.AreEqual(2, tExpando.Two);
        }

        [Test]
        public void TestSetDynamicAllAnonymous()
        {
            dynamic tExpando = new ExpandoObject();

            Dynamic.InvokeSetAll(tExpando, new { One = 1, Two = 2, Three = 3 });


            Assert.AreEqual(1, tExpando.One);
            Assert.AreEqual(2, tExpando.Two);
            Assert.AreEqual(3, tExpando.Three);
        }

        [Test]
        public void TestSetDynamicAllNamed()
        {
            dynamic tExpando = new ExpandoObject();

            Dynamic.InvokeSetAll(tExpando, One: 1, Two: 2, Three: 3);


            Assert.AreEqual(1, tExpando.One);
            Assert.AreEqual(2, tExpando.Two);
            Assert.AreEqual(3, tExpando.Three);
        }

        [Test]
        public void TestSetDynamicChainedOne()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();


            Dynamic.InvokeSetChain(tExpando, "Test", tSetValue);

            Assert.AreEqual(tSetValue, tExpando.Test);
        }

        [Test]
        public void TestGetDynamicChainedOne()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;



            var tOut = Dynamic.InvokeGetChain(tExpando, "Test");

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestCacheableGetDynamic()
        {

            var tSetValue = "1";
            dynamic tExpando = new ExpandoObject();
            tExpando.Test = tSetValue;

            var tCached = new CacheableInvocation(InvocationKind.Get, "Test");

            var tOut = tCached.Invoke((object)tExpando);

            Assert.AreEqual(tSetValue, tOut);
        }

        [Test]
        public void TestStaticGet()
        {
            var @static = InvokeContext.CreateStatic;
            var tDate = Dynamic.InvokeGet(@static(typeof(DateTime)), "Today");
            Assert.AreEqual(DateTime.Today, tDate);
        }

        [Test]
        public void TestCacheableStaticGet()
        {
            var @static = InvokeContext.CreateStatic;
            var tCached = new CacheableInvocation(InvocationKind.Get, "Today", context: @static(typeof(DateTime)));

            var tDate = tCached.Invoke(typeof(DateTime));
            Assert.AreEqual(DateTime.Today, tDate);
        }


        [Test]
        public void TestStaticGet2()
        {
            var @static = InvokeContext.CreateStatic;
            var tVal = Dynamic.InvokeGet(@static(typeof(StaticType)), "Test");
            Assert.AreEqual(true, tVal);
        }

        [Test]
        public void TestStaticGet3()
        {
            var tVal = Dynamic.InvokeGet((StaticContext)typeof(StaticType), "Test");
            Assert.AreEqual(true, tVal);
        }
        [Test]
        public void TestStaticSet()
        {
            var @static = InvokeContext.CreateStatic;
            int tValue = 12;
            Dynamic.InvokeSet(@static(typeof(StaticType)), "TestSet", tValue);
            Assert.AreEqual(tValue, StaticType.TestSet);
        }

        [Test]
        public void TestCacheableStaticSet()
        {
            int tValue = 12;
            var @static = InvokeContext.CreateStatic;
            var tCachedInvoke = new CacheableInvocation(InvocationKind.Set, "TestSet",
                                                        context: @static(typeof(StaticType)));
            tCachedInvoke.Invoke(typeof(StaticType), tValue);
            Assert.AreEqual(tValue, StaticType.TestSet);
        }

        [Test]
        public void TestStaticDateTimeMethod()
        {
            var @static = InvokeContext.CreateStatic;
            object tDateDyn = "01/20/2009";
            var tDate = Dynamic.InvokeMember(@static(typeof(DateTime)), "Parse", tDateDyn, CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(new DateTime(2009, 1, 20), tDate);
        }

        [Test]
        public void TestCacheableStaticDateTimeMethod()
        {
            var @static = InvokeContext.CreateStatic;
            object tDateDyn = "01/20/2009";
            var tCachedInvoke = new CacheableInvocation(InvocationKind.InvokeMember, "Parse", 2,
                                                        context: @static(typeof(DateTime)));
            var tDate = tCachedInvoke.Invoke(typeof(DateTime), tDateDyn,CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(new DateTime(2009, 1, 20), tDate);
        }



        [Test]
        public void TestIsEvent()
        {
            dynamic tPoco = new PocoEvent();

            var tResult = Dynamic.InvokeIsEvent(tPoco, "Event");

            Assert.AreEqual(true, tResult);
        }

        [Test]
        public void TestCacheableIsEventAndIsNotEvent()
        {
            object tPoco = new PocoEvent();

            var tCachedInvoke = new CacheableInvocation(InvocationKind.IsEvent, "Event");

            var tResult = tCachedInvoke.Invoke(tPoco);

            Assert.AreEqual(true, tResult);

            dynamic tDynamic = new DynamicObjects.Dictionary();

            tDynamic.Event = null;

            var tResult2 = tCachedInvoke.Invoke((object)tDynamic);

            Assert.AreEqual(false, tResult2);
        }

        [Test]
        public void TestIsNotEvent()
        {
            dynamic tDynamic = new DynamicObjects.Dictionary();

            tDynamic.Event = null;

            var tResult = Dynamic.InvokeIsEvent(tDynamic, "Event");

            Assert.AreEqual(false, tResult);

            bool tTest = false;
            bool tTest2 = false;


            tDynamic.Event += new EventHandler<EventArgs>((@object, args) => { tTest = true; });

            tDynamic.Event += new EventHandler<EventArgs>((@object, args) => { tTest2 = true; });

            Assert.AreEqual(false, tTest);

            Assert.AreEqual(false, tTest2);

            tDynamic.Event(null, null);

            Assert.AreEqual(true, tTest);

            Assert.AreEqual(true, tTest2);

        }

        [Test]
        public void TestPocoAddAssign()
        {
            var tPoco = new PocoEvent();
            bool tTest = false;

            Dynamic.InvokeAddAssignMember(tPoco, "Event", new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

            tPoco.OnEvent(null, null);

            Assert.AreEqual(true, tTest);

            var tPoco2 = new PropPoco() { Prop2 = 3 };

            Dynamic.InvokeAddAssignMember(tPoco2, "Prop2", 4);

            Assert.AreEqual(7L, tPoco2.Prop2);
        }

        [Test]
        public void TestCacheablePocoAddAssign()
        {
            var tPoco = new PocoEvent();
            bool tTest = false;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.AddAssign, "Event");

            tCachedInvoke.Invoke(tPoco, new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

            tPoco.OnEvent(null, null);

            Assert.AreEqual(true, tTest);

            var tPoco2 = new PropPoco() { Event = 3 };

            tCachedInvoke.Invoke(tPoco2, 4);

            Assert.AreEqual(7L, tPoco2.Event);
        }

        [Test]
        public void TestPocoSubtractAssign()
        {
            var tPoco = new PocoEvent();
            bool tTest = false;
            var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

            tPoco.Event += tEvent;

            Dynamic.InvokeSubtractAssignMember(tPoco, "Event", tEvent);

            tPoco.OnEvent(null, null);

            Assert.AreEqual(false, tTest);

            Dynamic.InvokeSubtractAssignMember(tPoco, "Event", tEvent);//Test Second Time

            var tPoco2 = new PropPoco() { Prop2 = 3 };

            Dynamic.InvokeSubtractAssignMember(tPoco2, "Prop2", 4);

            Assert.AreEqual(-1L, tPoco2.Prop2);
        }

        [Test]
        public void TestCacheablePocoSubtractAssign()
        {
            var tPoco = new PocoEvent();
            bool tTest = false;
            var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

            var tCachedInvoke = new CacheableInvocation(InvocationKind.SubtractAssign, "Event");

            tPoco.Event += tEvent;

            tCachedInvoke.Invoke(tPoco, tEvent);

            tPoco.OnEvent(null, null);

            Assert.AreEqual(false, tTest);

            tCachedInvoke.Invoke(tPoco, tEvent);//Test Second Time

            var tPoco2 = new PropPoco() { Event = 3 };

            tCachedInvoke.Invoke(tPoco2, 4);

            Assert.AreEqual(-1, tPoco2.Event);
        }

        [Test]
        public void TestDynamicAddAssign()
        {
            var tDyanmic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
            bool tTest = false;

            Dynamic.InvokeAddAssignMember(tDyanmic, "Event", new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

            tDyanmic.OnEvent(null, null);

            Assert.AreEqual(true, tTest);

            Dynamic.InvokeAddAssignMember(tDyanmic, "Prop2", 4);

            Assert.AreEqual(7L, tDyanmic.Prop2);
        }

        [Test]
        public void TestCacheableDynamicAddAssign()
        {
            var tDyanmic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
            var tDynamic2 = Build.NewObject(Event: 3);
            bool tTest = false;

            var tCachedInvoke = new CacheableInvocation(InvocationKind.AddAssign, "Event");

            tCachedInvoke.Invoke((object)tDyanmic, new EventHandler<EventArgs>((@object, args) => { tTest = true; }));

            tDyanmic.OnEvent(null, null);

            Assert.AreEqual(true, tTest);

            tCachedInvoke.Invoke((object)tDynamic2, 4);

            Assert.AreEqual(7, tDynamic2.Event);
        }

        [Test]
        public void TestDynamicSubtractAssign()
        {
            var tDyanmic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
            bool tTest = false;
            var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

            tDyanmic.Event += tEvent;

            Dynamic.InvokeSubtractAssignMember(tDyanmic, "Event", tEvent);

            tDyanmic.OnEvent(null, null);

            Assert.AreEqual(false, tTest);


            Dynamic.InvokeSubtractAssignMember(tDyanmic, "Prop2", 4);

            Assert.AreEqual(-1L, tDyanmic.Prop2);
        }


        [Test]
        public void TestCacheableDynamicSubtractAssign()
        {
            var tDyanmic = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
            var tDynamic2 = Build.NewObject(Event: 3);

            bool tTest = false;
            var tEvent = new EventHandler<EventArgs>((@object, args) => { tTest = true; });

            var tCachedInvoke = new CacheableInvocation(InvocationKind.SubtractAssign, "Event");

            tDyanmic.Event += tEvent;

            tCachedInvoke.Invoke((object)tDyanmic, tEvent);

            tDyanmic.OnEvent(null, null);

            Assert.AreEqual(false, tTest);


            tCachedInvoke.Invoke((object)tDynamic2, 4);

            Assert.AreEqual(-1, tDynamic2.Event);
        }

        [Test]
        public void TestDynamicMemberNamesExpando()
        {
            ExpandoObject tExpando = Build<ExpandoObject>.NewObject(One: 1);

            Assert.AreEqual("One", Dynamic.GetMemberNames(tExpando, dynamicOnly: true).Single());
        }

        [Test]
        public void TestDynamicMemberNamesImpromput()
        {
            DynamicObjects.Dictionary tDict = Build.NewObject(Two: 2);

            Assert.AreEqual("Two", Dynamic.GetMemberNames(tDict, dynamicOnly: true).Single());
        }

        [Test]
        public void TestCachedInvocationEquality()
        {
            var tCachedIvnocation1 = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 2,
                                                            argNames: new[] { "two" });

            var tCachedIvnocation2 = new CacheableInvocation(InvocationKind.InvokeMember, "Func", argCount: 2,
                                                            argNames: new[] { "two" });

            Assert.AreEqual(tCachedIvnocation1.GetHashCode(), tCachedIvnocation2.GetHashCode());
            Assert.AreEqual(tCachedIvnocation1, tCachedIvnocation2);
        }


        private DynamicObject CreateMock(ExpressionType op)
        {
            var tMock = new Mock<DynamicObject>() { CallBase = true };
            object result = It.IsAny<object>();
            tMock.Setup(
                s => s.TryBinaryOperation(It.Is<BinaryOperationBinder>(b => b.Operation == op), It.IsAny<object>(), out result)
                ).Returns(true);
            return tMock.Object;
        }

        public class OperatorTestDynObject:DynamicObject{
            ExpressionType _type;
            public OperatorTestDynObject(ExpressionType type){
                _type = type;
            }

            public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result){
                Assert.AreEqual(_type, binder.Operation);
                result = _type;
                return true;
            }

            public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result){
                Assert.AreEqual(_type, binder.Operation);
                result = _type;
                return true;
            }

        }
         private void RunBinaryMockTests(ExpressionType type){
            var mock = new OperatorTestDynObject(type);
            var dummy = new Object();
            Dynamic.InvokeBinaryOperator(mock, type, dummy);
        }

        private void RunUnaryMockTests(ExpressionType type){
            var mock = new OperatorTestDynObject(type);
            Dynamic.InvokeUnaryOpartor(type,mock);
        }

        [Test]
        public void TestInvokeAdd()
        {
            Assert.AreEqual(Dynamic.InvokeBinaryOperator(1, ExpressionType.Add, 2), 3);
        }

        [Test]
        public void TestInvokeBasicUnaryOperatorsDynamic()
        {
            RunUnaryMockTests(ExpressionType.Not);
            RunUnaryMockTests(ExpressionType.Negate);
            RunUnaryMockTests(ExpressionType.Increment);
            RunUnaryMockTests(ExpressionType.Decrement);
        


        }

        [Test]
        public void TestInvokeBasicBinaryOperatorsDynamic()
        {
            RunBinaryMockTests(ExpressionType.Add);
            RunBinaryMockTests(ExpressionType.Subtract);
            RunBinaryMockTests(ExpressionType.Divide);
            RunBinaryMockTests(ExpressionType.Multiply);
            RunBinaryMockTests(ExpressionType.Modulo);

            RunBinaryMockTests(ExpressionType.And);
            RunBinaryMockTests(ExpressionType.Or);
            RunBinaryMockTests(ExpressionType.ExclusiveOr);
            RunBinaryMockTests(ExpressionType.LeftShift);
            RunBinaryMockTests(ExpressionType.RightShift);

            RunBinaryMockTests(ExpressionType.AddAssign);
            RunBinaryMockTests(ExpressionType.SubtractAssign);
            RunBinaryMockTests(ExpressionType.DivideAssign);
            RunBinaryMockTests(ExpressionType.MultiplyAssign);
            RunBinaryMockTests(ExpressionType.ModuloAssign);

            RunBinaryMockTests(ExpressionType.AndAssign);
            RunBinaryMockTests(ExpressionType.OrAssign);
            RunBinaryMockTests(ExpressionType.ExclusiveOrAssign);
            RunBinaryMockTests(ExpressionType.LeftShiftAssign);
            RunBinaryMockTests(ExpressionType.RightShiftAssign);
        }


        [Test]
        public void TestInvokeSubtract()
        {
            Assert.AreEqual(Dynamic.InvokeBinaryOperator(1, ExpressionType.Subtract, 2), -1);
        }

    }
}
