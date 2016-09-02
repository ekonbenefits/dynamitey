﻿// 
//  Copyright 2011  Ekon Benefits
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
using System.Reflection;
using System.Runtime.CompilerServices;
using DynamiteyCore.DynamicObjects;

namespace DynamiteyCore.Internal.Optimization
{
    /// <summary>
    /// Utility Class
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Is Current Runtime Mono?
        /// </summary>
        public static readonly bool IsMono;

        static Util()
        {
            IsMono = Type.GetType("Mono.Runtime") != null;


        }
   


        /// <summary>
        /// Determines whether [is anonymous type] [the specified target].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// 	<c>true</c> if [is anonymous type] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnonymousType(object target)
        {
            if (target == null)
                return false;

            var type = target as Type ?? target.GetType();

            return type.GetTypeInfo().IsNotPublic
                   && type.GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute), false) != null;
        }


        /// <summary>
        /// Names the args if necessary.
        /// </summary>
        /// <param name="callInfo">The call info.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static object[] NameArgsIfNecessary(CallInfo callInfo, object[] args)
        {
            object[] tArgs;
            if (callInfo.ArgumentNames.Count == 0)
                tArgs = args;
            else
            {
                var tStop = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;
                tArgs = Enumerable.Repeat(default(string), tStop).Concat(callInfo.ArgumentNames).Zip(args, (n, v) => n == null ? v : new InvokeArg(n, v)).ToArray();
            }
            return tArgs;
        }

        /// <summary>
        /// Gets the target context.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <returns></returns>
        public static object GetTargetContext(this object target, out Type context, out bool staticContext)
        {
            var tInvokeContext = target as InvokeContext;
            staticContext = false;
            if (tInvokeContext != null)
            {
                staticContext = tInvokeContext.StaticContext;
                context = tInvokeContext.Context;
                context = context.FixContext();
                return tInvokeContext.Target;
            }

            context = target as Type ?? target.GetType();
            context = context.FixContext();
            return target;
        }


        /// <summary>
        /// Fixes the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static Type FixContext(this Type context)
        {
            if (context.IsArray)
            {
                return typeof (object);
            }
            return context;
        }

   
        internal static bool MassageResultBasedOnInterface(this BaseObject target, string binderName, bool resultFound, ref object result)
        {
            if (result is BaseForwarder.AddRemoveMarker) //Don't massage AddRemove Proxies
                return true;

            Type tType;
            var tTryType = target.TryTypeForName(binderName, out tType);
            if (tTryType && tType == typeof(void))
            {
                return true;
            }

            if(resultFound){
              if (result is IDictionary<string, object> && !(result is BaseDictionary)
                    && (!tTryType || tType == typeof(object)))
                {
                    result = new Dictionary((IDictionary<string, object>)result);
                }
              else if (tTryType)
              {
                  result = Dynamic.CoerceConvert(result, tType);
              }
            }
            else
            {
                result = null;
                if (!tTryType)
                {

                    return false;
                }
                if (typeof (Delegate).GetTypeInfo().IsAssignableFrom(tType))
                {
                    result = new BaseForwarder.AddRemoveMarker();
                }

                if (tType.GetTypeInfo().IsValueType)
                {
                    result = Dynamic.InvokeConstructor(tType);
                }
            }
            return true;
        }




        internal static object[] GetArgsAndNames(object[]args,out string[]argNames)
        {
            if (args == null)
                args = new object[] { null };

            //Optimization: linq statement creates a slight overhead in this case
            // ReSharper disable LoopCanBeConvertedToQuery
            // ReSharper disable ForCanBeConvertedToForeach
            argNames = new string[args.Length];

            var tArgSet = false;
            var tNewArgs = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var tArg = args[i];
                string tName = null;

                if (tArg is InvokeArg)
                {
                    tName = ((InvokeArg)tArg).Name;

                    tNewArgs[i] = ((InvokeArg)tArg).Value;
                    tArgSet = true;
                }else
                {
                    tNewArgs[i] = tArg;
                }
                argNames[i] = tName;
            }

            // ReSharper restore ForCanBeConvertedToForeach
            // ReSharper restore LoopCanBeConvertedToQuery
            if (!tArgSet)
                argNames = null;
            return tNewArgs;
        }
    }
}
