
// 
//  Copyright 2011 Ekon Benefits
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Dynamitey.Internal.Compat;

namespace Dynamitey.Internal.Optimization {


    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class BinderCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>>? _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ??= new Dictionary<BinderHash<T>, CallSite<T>>();
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class BinderGetCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>>? _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ??= new Dictionary<BinderHash<T>, CallSite<T>>();
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class BinderSetCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>>? _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ??= new Dictionary<BinderHash<T>, CallSite<T>>();
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class BinderConstructorCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>>? _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ??= new Dictionary<BinderHash<T>, CallSite<T>>();
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class BinderMemberCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>>? _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ??= new Dictionary<BinderHash<T>, CallSite<T>>();
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class BinderDirectCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>>? _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ??= new Dictionary<BinderHash<T>, CallSite<T>>();
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }

    internal static partial class InvokeHelper
    {


        internal static readonly Type[] FuncKinds;
        internal static readonly Type[] ActionKinds;
		internal static readonly Type[] TupleKinds;

		internal static readonly IDictionary<Type,int> FuncArgs;
		internal static readonly IDictionary<Type,int> ActionArgs;
		internal static readonly IDictionary<Type,int> TupleArgs;

        static InvokeHelper()
        {
            FuncKinds = new []
                            {
								typeof(Func<>), //0
								typeof(Func<,>), //1
								typeof(Func<,,>), //2
								typeof(Func<,,,>), //3
								typeof(Func<,,,,>), //4
								typeof(Func<,,,,,>), //5
								typeof(Func<,,,,,,>), //6
								typeof(Func<,,,,,,,>), //7
								typeof(Func<,,,,,,,,>), //8
								typeof(Func<,,,,,,,,,>), //9
								typeof(Func<,,,,,,,,,,>), //10
								typeof(Func<,,,,,,,,,,,>), //11
								typeof(Func<,,,,,,,,,,,,>), //12
								typeof(Func<,,,,,,,,,,,,,>), //13
								typeof(Func<,,,,,,,,,,,,,,>), //14
								typeof(Func<,,,,,,,,,,,,,,,>), //15
								typeof(Func<,,,,,,,,,,,,,,,,>), //16
                            };

            ActionKinds = new []
                            {
                                typeof(Action), //0
								typeof(Action<>), //1
								typeof(Action<,>), //2
								typeof(Action<,,>), //3
								typeof(Action<,,,>), //4
								typeof(Action<,,,,>), //5
								typeof(Action<,,,,,>), //6
								typeof(Action<,,,,,,>), //7
								typeof(Action<,,,,,,,>), //8
								typeof(Action<,,,,,,,,>), //9
								typeof(Action<,,,,,,,,,>), //10
								typeof(Action<,,,,,,,,,,>), //11
								typeof(Action<,,,,,,,,,,,>), //12
								typeof(Action<,,,,,,,,,,,,>), //13
								typeof(Action<,,,,,,,,,,,,,>), //14
								typeof(Action<,,,,,,,,,,,,,,>), //15
								typeof(Action<,,,,,,,,,,,,,,,>), //16
                            };

			TupleKinds = new []
                            {
								typeof(Tuple<>), //1
								typeof(Tuple<,>), //2
								typeof(Tuple<,,>), //3
								typeof(Tuple<,,,>), //4
								typeof(Tuple<,,,,>), //5
								typeof(Tuple<,,,,,>), //6
								typeof(Tuple<,,,,,,>), //7
								typeof(Tuple<,,,,,,,>), //8
                            };


			FuncArgs = FuncKinds.Zip(Enumerable.Range(0, FuncKinds.Length), (key, value) => new { key, value }).ToDictionary(k => k.key, v => v.value);
            ActionArgs = ActionKinds.Zip(Enumerable.Range(0, ActionKinds.Length), (key, value) => new { key, value }).ToDictionary(k => k.key, v => v.value);
			TupleArgs = TupleKinds.Zip(Enumerable.Range(1, ActionKinds.Length), (key, value) => new { key, value }).ToDictionary(k => k.key, v => v.value);

		
        }

		internal static dynamic TupleItem(dynamic tuple, int index){
			switch(index){
				case 1:
					return tuple.Item1;
				case 2:
					return tuple.Item2;
				case 3:
					return tuple.Item3;
				case 4:
					return tuple.Item4;
				case 5:
					return tuple.Item5;
				case 6:
					return tuple.Item6;
				case 7:
					return tuple.Item7;
				default:
					return tuple.Rest;
			}
		}


        internal static void InvokeMemberAction(ref CallSite? callsite,
		                                            Type? binderType,
													int knownType,
													LazyBinder? binder,
                                                    InvokeMemberName name,
													bool staticContext,
                                                    Type context, 
                                                    string[]? argNames,
                                                    [DisallowNull] object target,
                                                    params object?[] args)
        {

            var tSwitch = args.Length;
            switch (tSwitch)
            {
#region Optimizations
                case 0:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target);
                        break;
                    }
                case 1:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0]);
                        break;
                    }
                case 2:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1]);
                        break;
                    }
                case 3:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2]);
                        break;
                    }
                case 4:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3]);
                        break;
                    }
                case 5:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4]);
                        break;
                    }
                case 6:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5]);
                        break;
                    }
                case 7:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                        break;
                    }
                case 8:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                        break;
                    }
                case 9:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                        break;
                    }
                case 10:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                        break;
                    }
                case 11:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                        break;
                    }
                case 12:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                        break;
                    }
                case 13:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
                        break;
                    }
                case 14:
                    {
					    if(!(callsite is CallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>> tCallSite)){
							tCallSite = CreateCallSite<Action<CallSite,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?>>(binderType!,knownType, binder!, name, context, argNames, staticContext);
						    callsite=tCallSite;
						}
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
                        break;
                    }
#endregion
                default:
                    var argTypes = Enumerable.Repeat(typeof(object), tSwitch);
                    var delegateType = EmitCallSiteFuncType(argTypes, typeof(void));
                    Dynamic.InvokeCallSite(CreateCallSite(delegateType, binderType!,knownType, binder!, name, context, argNames), target, args);
                    break;

            }
        }

        

       

	



	    [return:MaybeNull]
        internal static TReturn InvokeMemberTargetType<TTarget,TReturn>(
										ref CallSite? callsite,
										Type? binderType,
										int knownType,
										LazyBinder? binder,
                                       InvokeMemberName name,
									 bool staticContext,
                                     Type context,
                                     string[]? argNames,
                                     [DisallowNull] TTarget target, params object?[] args)
        {

        

            var tSwitch = args.Length;

            switch (tSwitch)
            {
#region Optimizations
                case 0:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget, TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget, TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target);
                    }
                case 1:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0]);
                    }
                case 2:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1]);
                    }
                case 3:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2]);
                    }
                case 4:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3]);
                    }
                case 5:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4]);
                    }
                case 6:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5]);
                    }
                case 7:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    }
                case 8:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                    }
                case 9:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                    }
                case 10:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                    }
                case 11:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                    }
                case 12:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                    }
                case 13:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
                    }
                case 14:
                    {
					    if(!(callsite is CallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>> tCallSite)){
							 tCallSite = CreateCallSite<Func<CallSite, TTarget,  object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?, object?,TReturn>>(binderType!,knownType,binder!, name, context, argNames, staticContext);
							 callsite =tCallSite;
						}
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
                    }
#endregion
                default:
                    var argTypes = Enumerable.Repeat(typeof(object), tSwitch);
                    var delegateType = EmitCallSiteFuncType(argTypes, typeof(TTarget));
                    return Dynamic.InvokeCallSite(CreateCallSite(delegateType, binderType!,knownType, binder!, name, context, argNames), target, args)!;

            }
        }

     




		internal static Delegate WrapFuncHelper<TReturn>(dynamic invokable, int length)
        {
			  return length switch {
#region Optimizations
  					0 => new Func< TReturn>(()=> invokable()),
  					1 => new Func< object, TReturn>((a1)=> invokable(a1)),
  					2 => new Func< object, object, TReturn>((a1,a2)=> invokable(a1,a2)),
  					3 => new Func< object, object, object, TReturn>((a1,a2,a3)=> invokable(a1,a2,a3)),
  					4 => new Func< object, object, object, object, TReturn>((a1,a2,a3,a4)=> invokable(a1,a2,a3,a4)),
  					5 => new Func< object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5)=> invokable(a1,a2,a3,a4,a5)),
  					6 => new Func< object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6)=> invokable(a1,a2,a3,a4,a5,a6)),
  					7 => new Func< object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7)=> invokable(a1,a2,a3,a4,a5,a6,a7)),
  					8 => new Func< object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8)),
  					9 => new Func< object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9)),
  					10 => new Func< object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10)),
  					11 => new Func< object, object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11)),
  					12 => new Func< object, object, object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12)),
  					13 => new Func< object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13)),
  					14 => new Func< object, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14)),
  					15 => new Func< object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15)),
  					16 => new Func< object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15,a16)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15,a16)),
#endregion	
				_ => (Delegate) new DynamicFunc<TReturn>(args=>(TReturn)Dynamic.Invoke((object)invokable,args))
			};
        }

        internal static Delegate WrapAction(dynamic invokable, int length)
        {
           	 return length switch {
#region Optimizations
				0 => (Delegate) new Action(()=>invokable()),
  				1 => new Action< object>((a1)=> invokable(a1)),
  				2 => new Action< object, object>((a1,a2)=> invokable(a1,a2)),
  				3 => new Action< object, object, object>((a1,a2,a3)=> invokable(a1,a2,a3)),
  				4 => new Action< object, object, object, object>((a1,a2,a3,a4)=> invokable(a1,a2,a3,a4)),
  				5 => new Action< object, object, object, object, object>((a1,a2,a3,a4,a5)=> invokable(a1,a2,a3,a4,a5)),
  				6 => new Action< object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6)=> invokable(a1,a2,a3,a4,a5,a6)),
  				7 => new Action< object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7)=> invokable(a1,a2,a3,a4,a5,a6,a7)),
  				8 => new Action< object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8)),
  				9 => new Action< object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9)),
  				10 => new Action< object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10)),
  				11 => new Action< object, object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11)),
  				12 => new Action< object, object, object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12)),
  				13 => new Action< object, object, object, object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13)),
  				14 => new Action< object, object, object, object, object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14)),
  				15 => new Action< object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15)),
  				16 => new Action< object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>((a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15,a16)=> invokable(a1,a2,a3,a4,a5,a6,a7,a8,a9,a10,a11,a12,a13,a14,a15,a16)),
#endregion		
				_ => new DynamicAction(args=>Dynamic.InvokeAction((object)invokable,args))
			};
        }


        internal static object? FastDynamicInvokeReturn(Delegate del, dynamic?[] args)
        {
            dynamic tDel =del;
            switch(args.Length){
                default:
                    try
                    {
                        return del.DynamicInvoke(args);
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException is { } iex)
                        {
                            ExceptionDispatchInfo.Capture(iex).Throw();
                        }
                        throw;
                    }
#region Optimization
				case 1:
                    return tDel(args[0]);
				case 2:
                    return tDel(args[0],args[1]);
				case 3:
                    return tDel(args[0],args[1],args[2]);
				case 4:
                    return tDel(args[0],args[1],args[2],args[3]);
				case 5:
                    return tDel(args[0],args[1],args[2],args[3],args[4]);
				case 6:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5]);
				case 7:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6]);
				case 8:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7]);
				case 9:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8]);
				case 10:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9]);
				case 11:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10]);
				case 12:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11]);
				case 13:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12]);
				case 14:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12],args[13]);
				case 15:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12],args[13],args[14]);
				case 16:
                    return tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12],args[13],args[14],args[15]);
#endregion
            }
        }

        internal static void FastDynamicInvokeAction(Delegate del, params dynamic?[] args)
        {
            dynamic tDel =del;
            switch(args.Length){
                default: 
					try
                    {
						del.DynamicInvoke(args);
					}
					catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException is { } iex)
                        {
                            ExceptionDispatchInfo.Capture(iex).Throw();
                        }
                        throw;
                    }
                    return;
#region Optimization
				case 1:
                    tDel(args[0]);
                    return;
				case 2:
                    tDel(args[0],args[1]);
                    return;
				case 3:
                    tDel(args[0],args[1],args[2]);
                    return;
				case 4:
                    tDel(args[0],args[1],args[2],args[3]);
                    return;
				case 5:
                    tDel(args[0],args[1],args[2],args[3],args[4]);
                    return;
				case 6:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5]);
                    return;
				case 7:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6]);
                    return;
				case 8:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7]);
                    return;
				case 9:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8]);
                    return;
				case 10:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9]);
                    return;
				case 11:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10]);
                    return;
				case 12:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11]);
                    return;
				case 13:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12]);
                    return;
				case 14:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12],args[13]);
                    return;
				case 15:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12],args[13],args[14]);
                    return;
				case 16:
                    tDel(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8],args[9],args[10],args[11],args[12],args[13],args[14],args[15]);
                    return;
#endregion
            }
        }
    }
}
