﻿// 
//  Copyright 2010  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DynamiteyCore.Internal;
using DynamiteyCore.Internal.Optimization;
using Microsoft.CSharp.RuntimeBinder;

namespace DynamiteyCore
{
    /// <summary>
    /// Main API
    /// </summary>
    public static class Dynamic
    {


        private static readonly Type ComObjectType;

        private static readonly dynamic ComBinder;

        static Dynamic()
        {
            try
            {
                ComObjectType = typeof(object).GetTypeInfo().Assembly.GetType("System.__ComObject");
                ComBinder = new DynamicObjects.LateType(
                "System.Dynamic.ComBinder, System.Dynamic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            }
            catch
            {
                
            }
        }
        /// <summary>
        /// Creates a cached call site at runtime.
        /// </summary>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="binder">The CallSite binder.</param>
        /// <param name="name">Member Name</param>
        /// <param name="context">Permissions Context type</param>
        /// <param name="argNames">The arg names.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <param name="isEvent">if set to <c>true</c> [is event].</param>
        /// <returns>The CallSite</returns>
        /// <remarks>
        /// Advanced usage only for serious custom dynamic invocation.
        /// </remarks>
        /// <seealso cref="CreateCallSite{T}"/>
        public static CallSite CreateCallSite(Type delegateType, CallSiteBinder binder, String_OR_InvokeMemberName name,
                                              Type context, string[] argNames = null, bool staticContext = false,
                                              bool isEvent = false)
        {

            return InvokeHelper.CreateCallSite(delegateType, binder.GetType(), InvokeHelper.Unknown, () => binder, (InvokeMemberName)name, context, argNames,
                                               staticContext,
                                               isEvent);
        }

        /// <summary>
        /// Creates the call site.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binder">The binder.</param>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        /// <param name="argNames">The arg names.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <param name="isEvent">if set to <c>true</c> [is event].</param>
        /// <returns></returns>
        /// /// 
        /// <example>
        /// Unit test that exhibits usage
        /// <code><![CDATA[
        /// string tResult = String.Empty;
        /// var tPoco = new MethOutPoco();
        /// var tBinder =
        /// Binder.InvokeMember(BinderFlags.None, "Func", null, GetType(),
        /// new[]
        /// {
        /// Info.Create(
        /// InfoFlags.None, null),
        /// Info.Create(
        /// InfoFlags.IsOut |
        /// InfoFlags.UseCompileTimeType, null)
        /// });
        /// var tSite = Impromptu.CreateCallSite<DynamicTryString>(tBinder);
        /// tSite.Target.Invoke(tSite, tPoco, out tResult);
        /// Assert.AreEqual("success", tResult);
        /// ]]></code>
        /// </example>
        /// <seealso cref="CreateCallSite"/>
        public static CallSite<T> CreateCallSite<T>(CallSiteBinder binder, String_OR_InvokeMemberName name, Type context,
                                                    string[] argNames = null, bool staticContext = false,
                                                    bool isEvent = false) where T : class
        {
            return InvokeHelper.CreateCallSite<T>(binder.GetType(), InvokeHelper.Unknown, () => binder, (InvokeMemberName) name, context, argNames, staticContext,
                                                  isEvent);
        }


        /// <summary>
        /// Puts a dynamic linq proxy around the specified enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        public static dynamic Linq(object enumerable)
        {
            if(enumerable.GetType().GetTypeInfo().GetInterfaces().Where(it=>it.GetTypeInfo().IsGenericType)
                .All(it => it.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
            {
                var tEnum = enumerable as System.Collections.IEnumerable;
                if (tEnum !=null)
                {
                    enumerable = tEnum.Cast<object>();
                }
            }

            return new DynamicObjects.LinqInstanceProxy(enumerable);
        }

    

        /// <summary>
        /// Dynamically Invokes a member method using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name. Can be a string it will be implicitly converted</param>
        /// <param name="args">The args.</param>
        /// <returns> The result</returns>
        /// <example>   
        /// Unit test that exhibits usage:
        /// <code>
        /// <![CDATA[
        ///    dynamic tExpando = new ExpandoObject();
        ///    tExpando.Func = new Func<int, string>(it => it.ToString());
        ///
        ///    var tValue = 1;
        ///    var tOut = Impromptu.InvokeMember(tExpando, "Func", tValue);
        ///
        ///    Assert.AreEqual(tValue.ToString(), tOut);
        /// ]]>
        /// </code>
        /// </example>
        public static dynamic InvokeMember(object target, String_OR_InvokeMemberName name, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeMemberCallSite(target, (InvokeMemberName)name, args, tArgNames, tContext, tStaticContext,
                                                     ref tCallSite);
        }


        /// <summary>
        /// Invokes the binary operator.
        /// </summary>
        /// <param name="leftArg">The left arg.</param>
        /// <param name="op">The op.</param>
        /// <param name="rightArg">The right Arg.</param>
        /// <returns></returns>
        public static dynamic InvokeBinaryOperator(dynamic leftArg, ExpressionType op, dynamic rightArg)
        {
            switch (op)
            {
                case ExpressionType.Add:
                    return leftArg + rightArg;
                case ExpressionType.AddAssign:
                    leftArg += rightArg;
                    return leftArg;
                case ExpressionType.AndAssign:
                    leftArg &= rightArg;
                    return leftArg;
                case ExpressionType.Divide:
                    return leftArg/rightArg;
                case ExpressionType.DivideAssign:
                    leftArg /= rightArg;
                    return leftArg;
                case ExpressionType.Equal:
                    return leftArg == rightArg;
                case ExpressionType.ExclusiveOr:
                    return leftArg ^ rightArg;
                case ExpressionType.ExclusiveOrAssign:
                    leftArg ^= rightArg;
                    return leftArg;
                case ExpressionType.GreaterThan:
                    return leftArg > rightArg;
                case ExpressionType.GreaterThanOrEqual:
                    return leftArg >= rightArg;
                case ExpressionType.LeftShift:
                    return leftArg << rightArg;
                case ExpressionType.LeftShiftAssign:
                    leftArg <<= rightArg;
                    return leftArg;
                case ExpressionType.LessThan:
                    return leftArg < rightArg;
                case ExpressionType.LessThanOrEqual:
                    return leftArg <= rightArg;
                case ExpressionType.Modulo:
                    return leftArg%rightArg;
                case ExpressionType.ModuloAssign:
                    leftArg %= rightArg;
                    return leftArg;
                case ExpressionType.Multiply:
                    return leftArg*rightArg;
                case ExpressionType.MultiplyAssign:
                    leftArg *= rightArg;
                    return leftArg;
                case ExpressionType.NotEqual:
                    return leftArg != rightArg;
                case ExpressionType.OrAssign:
                    leftArg |= rightArg;
                    return leftArg;
                case ExpressionType.RightShift:
                    return leftArg >> rightArg;
                case ExpressionType.RightShiftAssign:
                    leftArg >>= rightArg;
                    return leftArg;
                case ExpressionType.Subtract:
                    return leftArg - rightArg;
                case ExpressionType.SubtractAssign:
                    leftArg -= rightArg;
                    return leftArg;
                case ExpressionType.Or:
                    return leftArg | rightArg;
                case ExpressionType.And:
                    return leftArg & rightArg;
                default:
                    throw new ArgumentException("Unsupported Operator", "op");
            }
        }

        /// <summary>
        /// Invokes the unary opartor.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <param name="op">The op.</param>
        /// <returns></returns>
        public static dynamic InvokeUnaryOpartor(ExpressionType op, dynamic arg)
        {
            switch (op)
            {
                case ExpressionType.Not:
                    return !arg;
                case ExpressionType.Negate:
                    return -arg;
                case ExpressionType.Decrement:
                    return --arg;
                case ExpressionType.Increment:
                    return ++arg;
                default:
                    throw new ArgumentException("Unsupported Operator", "op");
            }
        }

        /// <summary>
        /// Invokes the specified target using the DLR;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static dynamic Invoke(object target, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeDirectCallSite(target, args, tArgNames, tContext, tStaticContext, ref tCallSite);
        }


        /// <summary>
        /// Dynamically Invokes indexer using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="indexes">The indexes.</param>
        /// <returns></returns>
        public static dynamic InvokeGetIndex(object target, params object[] indexes)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            indexes = Util.GetArgsAndNames(indexes, out tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeGetIndexCallSite(target, indexes, tArgNames, tContext, tStaticContext,
                                                       ref tCallSite);
        }


        /// <summary>
        /// Convenience version of InvokeSetIndex that separates value and indexes.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value</param>
        /// <param name="indexes">The indexes </param>
        /// <returns></returns>
        public static object InvokeSetValueOnIndexes(object target, object value, params object[] indexes)
        {
            var tList = new List<object>(indexes) {value};
            return InvokeSetIndex(target, indexesThenValue: tList.ToArray());
        }

        /// <summary>
        /// Invokes setindex.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="indexesThenValue">The indexes then value.</param>
        public static object InvokeSetIndex(object target, params object[] indexesThenValue)
        {
            if (indexesThenValue.Length < 2)
            {
                throw new ArgumentException("Requires atleast one index and one value", "indexesThenValue");
            }

            string[] tArgNames;
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            indexesThenValue = Util.GetArgsAndNames(indexesThenValue, out tArgNames);

            CallSite tCallSite = null;
            return InvokeHelper.InvokeSetIndexCallSite(target, indexesThenValue, tArgNames, tContext, tStaticContext,
                                                ref tCallSite);
        }

        /// <summary>
        /// Dynamically Invokes a member method which returns void using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <example>
        /// Unit test that exhibits usage:
        /// <code>
        /// <![CDATA[
        ///    var tTest = "Wrong";
        ///    var tValue = "Correct";
        ///
        ///    dynamic tExpando = new ExpandoObject();
        ///    tExpando.Action = new Action<string>(it => tTest = it);
        ///
        ///    Impromptu.InvokeMemberAction(tExpando, "Action", tValue);
        ///
        ///    Assert.AreEqual(tValue, tTest);
        /// ]]>
        /// </code>
        /// </example>
        public static void InvokeMemberAction(object target, String_OR_InvokeMemberName name, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;

            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeMemberActionCallSite(target, (InvokeMemberName)name, args, tArgNames, tContext, tStaticContext,
                                                    ref tCallSite);
        }

        /// <summary>
        /// Invokes the action using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        public static void InvokeAction(object target, params object[] args)
        {
            string[] tArgNames;
            Type tContext;
            bool tStaticContext;

            target = target.GetTargetContext(out tContext, out tStaticContext);
            args = Util.GetArgsAndNames(args, out tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeDirectActionCallSite(target, args, tArgNames, tContext, tStaticContext, ref tCallSite);
        }


        /// <summary>
        /// Dynamically Invokes a set member using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <example>
        /// Unit test that exhibits usage:
        /// <code>
        /// <![CDATA[
        ///    dynamic tExpando = new ExpandoObject();
        ///
        ///    var tSetValue = "1";
        ///
        ///    Impromptu.InvokeSet(tExpando, "Test", tSetValue);
        ///
        ///    Assert.AreEqual(tSetValue, tExpando.Test);
        /// ]]>
        /// </code>
        /// </example>
        /// <remarks>
        /// if you call a static property off a type with a static context the csharp dlr binder won't do it, so this method reverts to reflection
        /// </remarks>
        public static object InvokeSet(object target, string name, object value)
        {
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            tContext = tContext.FixContext();


            CallSite tCallSite = null;
            return InvokeHelper.InvokeSetCallSite(target, name, value, tContext, tStaticContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes the set on the end of a property chain.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyChain">The property chain.</param>
        /// <param name="value">The value.</param>
        public static object InvokeSetChain(object target, string propertyChain, object value)
        {
            var tProperties = _chainRegex.FluentMatches(propertyChain).ToList();
            var tGetProperties = tProperties.Take(tProperties.Count - 1);

       
            var tTarget = target;
            foreach (var tProperty in tGetProperties)
            {
                var tGetter = tProperty.Getter;
                var tIntIndexer = tProperty.IntIndexer;
                var tStringIndexer = tProperty.StringIndexer;

                if (tGetter != null)
                    tTarget = InvokeGet(tTarget, tGetter);
                else if (tIntIndexer != null)
                    tTarget = InvokeGetIndex(tTarget, Dynamic.CoerceConvert(tIntIndexer, typeof(int)));
                else if (tStringIndexer != null)
                    tTarget = InvokeGetIndex(tTarget, tStringIndexer);
                else
                {
                    throw new Exception(string.Format("Could Not Parse :'{0}'", propertyChain));
                }
            }

            var tSetProperty = tProperties.Last();

            var tSetGetter = tSetProperty.Getter;
            var tSetIntIndexer = tSetProperty.IntIndexer;
            var tSetStringIndexer = tSetProperty.StringIndexer;

            if (tSetGetter != null)
                return InvokeSet(tTarget, tSetGetter, value);
            if (tSetIntIndexer != null)
                return InvokeSetIndex(tTarget, Dynamic.CoerceConvert(tSetIntIndexer, typeof(int)), value);
            if (tSetStringIndexer != null)
                return InvokeSetIndex(tTarget, tSetStringIndexer, value);
            
            throw new Exception(string.Format("Could Not Parse :'{0}'", propertyChain));
        }

           




        private static readonly dynamic _invokeSetAll = new InvokeSetters();

        /// <summary>
        /// Call Like method invokes set on target and a list of property/value. Invoke with dictionary, anonymous type or named arguments.
        /// </summary>
        /// <value>The invoke set all.</value>
        public static dynamic InvokeSetAll
        {
            get { return _invokeSetAll; }
        }

        /// <summary>
        /// Wraps a target to partial apply a method (or target if you can invoke target directly eg delegate).
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="totalArgCount">The total arg count.</param>
        /// <returns></returns>
        public static dynamic Curry(object target, int? totalArgCount = null)
        {
            if (target is Delegate && !totalArgCount.HasValue)
                return Curry((Delegate) target);
            return new Curry(target, totalArgCount);
        }

        /// <summary>
        /// Wraps a delegate to partially apply it.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static dynamic Curry(Delegate target)
        {
            return new Curry(target, target.GetMethodInfo().GetParameters().Length);
        }



        /// <summary>
        /// Dynamically Invokes a get member using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>The result.</returns>
        /// <example>
        /// Unit Test that describes usage
        /// <code>
        /// <![CDATA[
        ///    var tSetValue = "1";
        ///    var tAnon = new { Test = tSetValue };
        ///
        ///    var tOut =Impromptu.InvokeGet(tAnon, "Test");
        ///
        ///    Assert.AreEqual(tSetValue, tOut);
        /// ]]>
        /// </code>
        /// </example>
        public static dynamic InvokeGet(object target, string name)
        {
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);            CallSite tSite = null;
            return InvokeHelper.InvokeGetCallSite(target, name, tContext, tStaticContext, ref tSite);
        }


  private static readonly Regex _chainRegex
           = new Regex(@"((\.?(?<Getter>\w+))|(\[(?<IntIndexer>\d+)\])|(\['(?<StringIndexer>\w+)'\]))");

        /// <summary>
        /// Invokes the getter property chain.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyChain">The property chain.</param>
        /// <returns></returns>
        public static dynamic InvokeGetChain(object target, string propertyChain)
        {
            var tProperties = _chainRegex.FluentMatches(propertyChain);
            var tTarget = target;
            foreach (var tProperty in tProperties)
            {
                var tGetter = tProperty.Getter;
                var tIntIndexer = tProperty.IntIndexer;
                var tStringIndexer = tProperty.StringIndexer;

                if (tGetter != null)
                    tTarget = InvokeGet(tTarget, tGetter);
                else if (tIntIndexer != null)
                    tTarget = InvokeGetIndex(tTarget, Dynamic.CoerceConvert(tIntIndexer,typeof(int)));
                else if (tStringIndexer != null)
                    tTarget = InvokeGetIndex(tTarget, tStringIndexer);
                else
                {
                    throw new Exception(string.Format("Could Not Parse :'{0}'", propertyChain));
                }
            }
            return tTarget;
        }

        /// <summary>
        /// Determines whether the specified name on target is event. This allows you to know whether to InvokeMemberAction
        ///  add_{name} or a combo of {invokeget, +=, invokeset} and the corresponding remove_{name} 
        /// or a combon of {invokeget, -=, invokeset}
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target is event; otherwise, <c>false</c>.
        /// </returns>
        public static bool InvokeIsEvent(object target, string name)
        {
            Type tContext;
            bool tStaticContext;
            target = target.GetTargetContext(out tContext, out tStaticContext);
            tContext = tContext.FixContext();
            CallSite tCallSite = null;
            return InvokeHelper.InvokeIsEventCallSite(target, name, tContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes add assign with correct behavior for events.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void InvokeAddAssignMember(object target, string name, object value)
        {
            CallSite callSiteAdd =null;
            CallSite callSiteGet =null;
            CallSite callSiteSet =null;
            CallSite callSiteIsEvent = null;
            Type context;
            bool staticContext;
            target = target.GetTargetContext(out context, out staticContext);

            var args = new[] { value };
            string[] argNames;
            args = Util.GetArgsAndNames(args, out argNames);

            InvokeHelper.InvokeAddAssignCallSite(target, name, args, argNames, context, staticContext, ref callSiteIsEvent, ref callSiteAdd, ref callSiteGet, ref callSiteSet);
        }

        /// <summary>
        /// Invokes subtract assign with correct behavior for events.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void InvokeSubtractAssignMember(object target, string name, object value)
        {
            Type context;
            bool staticContext;
            target = target.GetTargetContext(out context, out staticContext);

            var args = new[] { value };
            string[] argNames;

            args = Util.GetArgsAndNames(args, out argNames);


            CallSite callSiteIsEvent = null;
            CallSite callSiteRemove = null;
            CallSite callSiteGet = null;
            CallSite callSiteSet = null;


            InvokeHelper.InvokeSubtractAssignCallSite(target, name, args, argNames, context, staticContext, ref callSiteIsEvent, ref callSiteRemove, ref callSiteGet,ref  callSiteSet);
        }

        /// <summary>
        /// Invokes  convert using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="type">The type.</param>
        /// <param name="explicit">if set to <c>true</c> [explicit].</param>
        /// <returns></returns>
        public static dynamic InvokeConvert(object target, Type type, bool @explicit =false)
        {
            Type tContext;
            bool tDummy;
            target = target.GetTargetContext(out tContext, out tDummy);

            CallSite tCallSite =null;
            return InvokeHelper.InvokeConvertCallSite(target, @explicit, type, tContext, ref tCallSite);

        }

        internal static readonly IDictionary<Type, Delegate> CompiledExpressions = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Coerces any invokable to specificied delegate type.
        /// </summary>
        /// <param name="invokeableObject">The invokeable object.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <returns></returns>
        public static dynamic CoerceToDelegate(object invokeableObject, Type delegateType)
            {
                if (!typeof(Delegate).GetTypeInfo().IsAssignableFrom(delegateType.GetTypeInfo().BaseType))
                {
                    return null;
                }
                var tDelMethodInfo = delegateType.GetTypeInfo().GetMethod("Invoke");
                var tReturnType = tDelMethodInfo.ReturnType;
                var tAction = tReturnType == typeof(void);
                var tParams = tDelMethodInfo.GetParameters();
                var tLength = tDelMethodInfo.GetParameters().Length;
                Delegate tBaseDelegate = tAction
                                             ? InvokeHelper.WrapAction(invokeableObject, tLength)
                                             : InvokeHelper.WrapFunc(tReturnType, invokeableObject, tLength);


                if (!InvokeHelper.IsActionOrFunc(delegateType) || tParams.Any(it => it.ParameterType.GetTypeInfo().IsValueType))
                //Conditions that aren't contravariant;
                {
                    Delegate tGetResult;

                    if (!CompiledExpressions.TryGetValue(delegateType, out tGetResult))
                    {
                        var tParamTypes = tParams.Select(it => it.ParameterType).ToArray();
                        var tDelParam = Expression.Parameter(tBaseDelegate.GetType());
                        var tInnerParams = tParamTypes.Select(Expression.Parameter).ToArray();

                        var tI = Expression.Invoke(tDelParam,
                                                   tInnerParams.Select(it => (Expression)Expression.Convert(it, typeof(object))));
                        var tL = Expression.Lambda(delegateType, tI, tInnerParams);

                        tGetResult =
                            Expression.Lambda(Expression.GetFuncType(tBaseDelegate.GetType(), delegateType), tL,
                                              tDelParam).Compile();
                        CompiledExpressions[delegateType] = tGetResult;
                    }

                    return tGetResult.DynamicInvoke(tBaseDelegate);
                }
                return tBaseDelegate;

            }

        private static readonly dynamic LateConvert = new DynamicObjects.LateType(typeof(Convert));


        /// <summary>
        /// Determines whether value is DBNull dynamically (Useful for PCL)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is DBNull]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDBNull(object value)
        {

            try
            {
                return LateConvert.IsDBNull(value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Applies the equivalent type hint to dynamic object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="types">The types.</param>
        public static void ApplyEquivalentType(DynamicObjects.IEquivalentType target, params Type[] types)
        {
            if(types.Length == 1)
                target.EquivalentType = types.First();
            else
                target.EquivalentType = new DynamicObjects.AggreType(types.ConvertAll<DynamicObjects.FauxType>().ToArray());
          
        }

        /// <summary>
        /// Implicit or Explicit Converts the items of the specified enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="explict">if set to <c>true</c> [explict].</param>
        /// <returns></returns>
        public static IEnumerable<T> ConvertAll<T>(this System.Collections.IEnumerable enumerable, bool explict =false)
        {
            return enumerable.Cast<Object>().Select(it => InvokeConvert(it, typeof (T), explict)).Cast<T>();
        } 

        internal static readonly dynamic Impromptu
            = new DynamicObjects.LateType("ImpromptuInterface.Impromptu, ImpromptuInterface, PublicKeyToken=0b1781c923b2975b");

        internal static readonly dynamic TypeDescriptor
             = new DynamicObjects.LateType("System.ComponentModel.TypeDescriptor, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        internal static readonly Type TypeConverterAttributeSL
            = Type.GetType("System.ComponentModel.TypeConverter, System, Version=5.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", false);

        /// <summary>
        /// Goes the extra mile to convert target to type.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static dynamic CoerceConvert(object target, Type type)
        {
            if (target != null && !type.GetTypeInfo().IsInstanceOfType(target) && !IsDBNull(target))
            {

                var delegateConversion = CoerceToDelegate(target, type);

                if (delegateConversion != null)
                    return delegateConversion;


                if (type.GetTypeInfo().IsInterface && Impromptu.IsAvailable)
                {


                
                    if (target is IDictionary<string, object> && !(target is DynamicObjects.BaseObject))
                    {
                        target = new DynamicObjects.Dictionary((IDictionary<string, object>)target);
                    }
                    else if(!(target is DynamicObjects.BaseObject))
                    {
                        target = new DynamicObjects.Get(target);
                    }


                    target = Impromptu.DynamicActLike(target, type);
                }
                else
                {
                

                    try
                    {
                        object tResult;

                        tResult = Dynamic.InvokeConvert(target, type, @explicit: true);

                        target = tResult;
                    }
                    catch (RuntimeBinderException)
                    {
                        Type tReducedType = type;
                        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            tReducedType = type.GetTypeInfo().GetGenericArguments().First();
                        }

                        if (typeof (Enum).GetTypeInfo().IsAssignableFrom(tReducedType) && target is string)
                        {
                            target = Enum.Parse(tReducedType, target as String, true);

                        }
                        else if (target is IConvertible && typeof (IConvertible).GetTypeInfo().IsAssignableFrom(tReducedType))
                        {

                            target = Convert.ChangeType(target, tReducedType,System.Globalization.CultureInfo.CurrentCulture);

                        }
                        else
                        {
                            try
                            {
                                dynamic converter = null;
                                if (TypeDescriptor.IsAvailable)
                                {
                                    converter = TypeDescriptor.GetConverter(tReducedType);
                                }
                                else if (TypeConverterAttributeSL != null) 
                                {
                                        var tAttributes =
                                            tReducedType.GetTypeInfo().GetCustomAttributes(TypeConverterAttributeSL, false);
                                        dynamic attribute = tAttributes.FirstOrDefault();
                                        if (attribute != null)
                                        {
                                            converter =
                                                Impromptu.InvokeConstructor(Type.GetType(attribute.ConverterTypeName));
                                        }
                                }
                                

                                if (converter != null && converter.CanConvertFrom(target.GetType()))
                                {
                                    target = converter.ConvertFrom(target);
                                }
                            }
                            catch (RuntimeBinderException)
                            {
                                
                            }
                        }

                    }
                }
            }
            else if (((target == null) || IsDBNull(target )) && type.GetTypeInfo().IsValueType)
            {
                target = Dynamic.InvokeConstructor(type);
            }
            else if (!type.GetTypeInfo().IsInstanceOfType(target) && IsDBNull(target))
            {
                return null;
            }
            return target;
        }

        /// <summary>
        /// Invokes the constuctor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static dynamic InvokeConstructor(Type type, params object[] args)
        {
            string[] tArgNames;
            bool tValue = type.GetTypeInfo().IsValueType;
            if (tValue && args.Length == 0)  //dynamic invocation doesn't see constructors of value types
            {
                return Activator.CreateInstance(type);
            }

           args = Util.GetArgsAndNames( args, out tArgNames);
           CallSite tCallSite = null;


            return InvokeHelper.InvokeConstructorCallSite(type, tValue, args, tArgNames, ref tCallSite);
        }


        /// <summary>
        /// FastDynamicInvoke extension method. Runs up to runs up to 20x faster than <see cref="System.Delegate.DynamicInvoke"/> .
        /// </summary>
        /// <param name="del">The del.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
		public static object FastDynamicInvoke(this Delegate del, params object[] args)
		{
		    if(del.GetMethodInfo().ReturnType == typeof(void)){
				
				InvokeHelper.FastDynamicInvokeAction(del, args);
				return null;
			}
		    return InvokeHelper.FastDynamicInvokeReturn(del, args);
		}

        /// <summary>
        /// Given a generic parameter count and whether it returns void or not gives type of Action or Func
        /// </summary>
        /// <param name="paramCount">The param count.</param>
        /// <param name="returnVoid">if set to <c>true</c> [return void].</param>
        /// <returns>Type of Action or Func</returns>
        public static Type GenericDelegateType(int paramCount, bool returnVoid = false)
        {
            var tParamCount = returnVoid ? paramCount : paramCount - 1;
            if (tParamCount > 16)
                throw new ArgumentException(String.Format("{0} only handle at  most {1} parameters", returnVoid ? "Action" : "Func", returnVoid ? 16 : 17), "paramCount");
            if(tParamCount < 0)
                throw new ArgumentException(String.Format("{0} must have at least {1} parameter(s)", returnVoid ? "Action" : "Func", returnVoid ? 0 : 1), "paramCount");


            return returnVoid
                ? InvokeHelper.ActionKinds[tParamCount]
                : InvokeHelper.FuncKinds[tParamCount];
        }

        /// <summary>
        /// Gets the member names of properties. Not all IDynamicMetaObjectProvider have support for this.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="dynamicOnly">if set to <c>true</c> [dynamic only]. Won't add reflected properties</param>
        /// <returns></returns>
        public static IEnumerable<string> GetMemberNames(object target, bool dynamicOnly = false)
        {
            var tList = new List<string>();
            if (!dynamicOnly)
            {
               tList.AddRange(target.GetType().GetTypeInfo().GetProperties().Select(it => it.Name));
            }

            var tTarget = target as IDynamicMetaObjectProvider;
            if (tTarget !=null)
            {
                tList.AddRange(tTarget.GetMetaObject(Expression.Constant(tTarget)).GetDynamicMemberNames());
            }else
            {

                if (ComObjectType != null && ComObjectType.GetTypeInfo().IsInstanceOfType(target) && ComBinder.IsAvailable)
                {
                    tList.AddRange(ComBinder.GetDynamicDataMemberNames(target));
                }
            }
            return tList;
        } 

        /// <summary>
        /// Dynamically invokes a method determined by the CallSite binder and be given an appropriate delegate type
        /// </summary>
        /// <param name="callSite">The Callsite</param>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        /// <remarks>
        /// Advanced use only. Use this method for serious custom invocation, otherwise there are other convenience methods such as
        /// <see cref="InvokeMember"></see>, <see cref="InvokeGet"></see>, <see cref="InvokeSet"></see> and <see cref="InvokeMemberAction"></see>
        /// </remarks>
        public static dynamic InvokeCallSite(CallSite callSite, object target, params object[] args)
        {
         
            
            var tParameters = new List<object> {callSite, target};
            tParameters.AddRange(args);

            MulticastDelegate tDelegate = ((dynamic)callSite).Target;

            return tDelegate.FastDynamicInvoke(tParameters.ToArray());
        }


    }

}
