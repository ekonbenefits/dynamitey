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
using System.Linq;
using System.Text;

namespace Dynamitey.Internal.Optimization
{
    internal class BinderHash
    {
   

        protected BinderHash(Type delegateType, String name, Type context, string[] argNames, Type binderType, bool staticContext, bool isEvent, bool knownBinder)
        {
            KnownBinder = knownBinder;
            BinderType = binderType;
            StaticContext = staticContext;
            DelegateType = delegateType;
            Name = name;
            IsSpecialName = false;
            GenericArgs = null;
            Context = context;
            ArgNames = argNames;
            IsEvent = isEvent;
            

        }

        protected BinderHash(Type delegateType, InvokeMemberName name, Type context, string[] argNames, Type binderType, bool staticContext, bool isEvent, bool knownBinder)
        {
            KnownBinder = knownBinder;
            BinderType = binderType;
            StaticContext = staticContext;
            DelegateType = delegateType;
            Name = name.Name;
            IsSpecialName = name.IsSpecialName;
            GenericArgs = name.GenericArgs;
            Context = context;
            ArgNames = argNames;
            IsEvent = isEvent;


        }


 

        public bool KnownBinder { get; }
        public Type BinderType { get;  }
        public bool StaticContext { get; }
        public bool IsEvent { get;  }
        public Type DelegateType { get; }
        public string Name { get;  }
        public bool IsSpecialName { get; }
        public Type[] GenericArgs { get;  }
        public Type Context { get; }
        public string[] ArgNames { get; }

        public virtual bool Equals(BinderHash other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            var tArgNames = ArgNames;
            var tOtherArgNames = other.ArgNames;
            var tGenArgs = GenericArgs;
            var tOtherGenArgs = other.GenericArgs;

            return
                !(tOtherArgNames == null && tArgNames != null)
                && !(tArgNames == null && tOtherArgNames != null)
                && other.IsEvent == IsEvent
                && other.StaticContext == StaticContext
                && other.Context == Context
                && (KnownBinder || other.BinderType == BinderType)
                && other.DelegateType == DelegateType
                && Equals(other.Name, Name)
                && !(other.IsSpecialName ^ IsSpecialName)
                && !(tOtherGenArgs == null && tGenArgs != null)
                && !(tGenArgs == null && tOtherGenArgs != null)
                && (tOtherGenArgs == null || tOtherGenArgs.SequenceEqual(tGenArgs))
                && (tOtherArgNames == null || tOtherArgNames.SequenceEqual(tArgNames));
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is BinderHash)) return false;
            return Equals((BinderHash) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var tArgNames = ArgNames;

                int result = (tArgNames == null ? 0 : tArgNames.Length * 397);
                result = (result  ^ StaticContext.GetHashCode());
                //result = (result * 397) ^ DelegateType.GetHashCode();
                //result = (result * 397) ^ Context.GetHashCode();
                result = (result * 397) ^ Name.GetHashCode();
                return result;
            }
        }
    }



    internal class BinderHash<T> : BinderHash where T : class
    {
        public static BinderHash<T> Create(string name, Type context, string[] argNames, Type binderType, bool staticContext, bool isEvent, bool knownBinder)
        {
            return new BinderHash<T>(name, context, argNames, binderType, staticContext, isEvent, knownBinder);
        }

        public static BinderHash<T> Create(InvokeMemberName name, Type context, string[] argNames, Type binderType, bool staticContext, bool isEvent, bool knownBinder)
        {
            return new BinderHash<T>(name, context, argNames, binderType, staticContext, isEvent, knownBinder);
        }

        protected BinderHash(InvokeMemberName name, Type context, string[] argNames, Type binderType, bool staticContext, bool isEvent,bool knownBinder)
            : base(typeof(T), name, context, argNames, binderType, staticContext, isEvent,knownBinder)
        {
        }

        protected BinderHash(string name, Type context, string[] argNames, Type binderType, bool staticContext, bool isEvent, bool knownBinder)
            : base(typeof(T), name, context, argNames, binderType, staticContext, isEvent, knownBinder)
        {
        }

        public override bool Equals(BinderHash other)
        {
           
                if (other is BinderHash<T>)
                {
                    var tGenArgs = GenericArgs;
                    var tOtherGenArgs = other.GenericArgs;

                    return 
                           !(other.ArgNames == null ^ ArgNames == null)
                           && other.IsEvent == IsEvent
                           && other.StaticContext == StaticContext
                           && (KnownBinder || Equals(other.BinderType, BinderType))
                           && Equals(other.Context, Context)
                           && Equals(other.Name, Name)
                            && !(other.IsSpecialName ^ IsSpecialName)
                            && !(tOtherGenArgs == null ^ tGenArgs == null)
                            && (tGenArgs == null ||
                                    //Exclusive Or makes sure this doesn't happen
                                    // ReSharper disable AssignNullToNotNullAttribute
                            tGenArgs.SequenceEqual(tOtherGenArgs))
                        // ReSharper restore AssignNullToNotNullAttribute
                           && (ArgNames == null
                            // ReSharper disable AssignNullToNotNullAttribute
                                 //Exclusive Or Makes Sure this doesn't happen
                                 || other.ArgNames.SequenceEqual(ArgNames));
                            // ReSharper restore AssignNullToNotNullAttribute
                }
                return false;
            
           
        }
    }
}
