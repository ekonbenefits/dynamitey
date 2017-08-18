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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dynamitey.Internal.Optimization;
using Dynamitey.Internal.Compat;

namespace Dynamitey.DynamicObjects
{

    /// <summary>
    /// Interface for simplistic builder options
    /// </summary>
    public interface IBuilder
    {  
        
        /// <summary>
        /// Creates a prototype list
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        dynamic List(params dynamic[] contents);

        /// <summary>
        /// Setup List or Array, takes either one <see cref="Activate"/> or a list of constructor args that will use objects Type
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        dynamic ListSetup(params dynamic[] constructorArgs);

        /// <summary>
        /// Setup List or Array if list has a default constrcutor
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <returns></returns>
        dynamic ListSetup<TList>();

        /// <summary>
        /// Setup List or Array, takes either one <see cref="Activate"/> or a list of constructor args that will use objects Type
        /// </summary>
        /// <param name="constructorArgsFactory">The constructor args factory.</param>
        /// <returns></returns>
        dynamic ListSetup(Func<object[]> constructorArgsFactory);

        /// <summary>
        /// Setup List or Array if list has a default constrcutor
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <returns></returns>
        dynamic ArraySetup<TList>();

        /// <summary>
        /// Alternative name for <see cref="ListSetup(object[])"/>
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        dynamic ArraySetup(params dynamic[] constructorArgs);


        /// <summary>
        /// Alternative name for <see cref="ListSetup{TList}"/>
        /// </summary>
        /// <param name="constructorArgsFactory">The constructor args factory.</param>
        /// <returns></returns>
        dynamic ArraySetup(Func<object[]> constructorArgsFactory);


        /// <summary>
        /// Alternative name for <see cref="List"/>
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        dynamic Array(params dynamic[] contents);

        /// <summary>
        /// Generates Object, use by calling with named arguments <code>builder.Object(Prop1:"test",Prop2:"test")</code>
        /// returns new object;
        /// </summary>
        dynamic Object { get; }

        /// <summary>
        /// Sets up object builder
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        dynamic ObjectSetup(params dynamic[] constructorArgs);

        /// <summary>
        /// Sets up object builder
        /// </summary>
        /// <param name="constructorArgsFactory"></param>
        /// <returns></returns>
        dynamic ObjectSetup(Func<object[]> constructorArgsFactory);

        /// <summary>
        /// Setups up named builders 
        /// </summary>
        /// <value>The setup.</value>
        dynamic Setup { get; }
    }

    /// <summary>
    /// Builds Expando-Like Objects with an inline Syntax
    /// </summary>
    /// <typeparam name="TObjectProtoType">The type of the object proto type.</typeparam>
   
    public class Builder<TObjectProtoType>: BaseObject, IBuilder
    {
        /// <summary>
        /// Build factory storage
        /// </summary>
       
		protected IDictionary<string,Activate> _buildType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Builder{TObjectProtoType}"/> class.
        /// </summary>
		public Builder(){
            _buildType = new Dictionary<string, Activate>();
			Setup = new SetupTrampoline(this);
			Object = new BuilderTrampoline(this);
		}
		
        /// <summary>
        /// Creates a prototype list
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public dynamic List(params dynamic[] contents)
        {
            Activate tBuildType;
            if (!_buildType.TryGetValue("List", out tBuildType))
                tBuildType = null;

            if (tBuildType != null)
            {
                dynamic tList = tBuildType.Create();

                if (contents != null)
                {
                    foreach (var item in contents)
                    {
                        tList.Add(item);
                    }
                }
                return tList;
            }

            return new List(contents);
        }



        /// <summary>
        /// Setup List or Array, takes either one <see cref="Activate" /> or a list of constructor args that will use objects Type
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        public dynamic ListSetup(params dynamic[] constructorArgs)
        {
            var tActivate =constructorArgs.OfType<Activate>().SingleOrDefault();

            
            if (tActivate == null)
            {

                if (!_buildType.TryGetValue("Object", out tActivate))
                    tActivate = null;
                if (tActivate != null)
                {
                    tActivate = new Activate(tActivate.Type,constructorArgs);
                }
                if(tActivate == null)
                    tActivate = new Activate<List>(constructorArgs);
            }

            _buildType["List"] = tActivate;
            _buildType["Array"] = tActivate;
            return this;
        }

        /// <summary>
        /// Setup List or Array if list has a default constrcutor
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <returns></returns>
        public dynamic ListSetup<TList>()
        {
            return ListSetup(new Activate<TList>());
        }

        /// <summary>
        /// Setup List or Array, takes either one <see cref="Activate" /> or a list of constructor args that will use objects Type
        /// </summary>
        /// <param name="constructorArgsFactory">The constructor args factory.</param>
        /// <returns></returns>
        public dynamic ListSetup(Func<object[]> constructorArgsFactory)
        {
            return ListSetup((object)constructorArgsFactory);
        }

        /// <summary>
        /// Setup List or Array if list has a default constrcutor
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <returns></returns>
        public dynamic ArraySetup<TList>()
        {
            return ListSetup(new Activate<TList>());
        }

        /// <summary>
        /// Alternative name for <see cref="ListSetup(object[])" />
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        public dynamic ArraySetup(params dynamic[] constructorArgs)
        {
            return ListSetup(constructorArgs);
        }

        /// <summary>
        /// Alternative name for <see cref="ListSetup{TList}" />
        /// </summary>
        /// <param name="constructorArgsFactory">The constructor args factory.</param>
        /// <returns></returns>
        public dynamic ArraySetup(Func<object[]> constructorArgsFactory)
        {
            return ListSetup((object)constructorArgsFactory);
        }

        /// <summary>
        /// Alternative name for <see cref="List"/>
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public dynamic Array(params dynamic[] contents)
        {
            return List(contents);
        }

        /// <summary>
        /// Creates a Prototype object.
        /// </summary>
        /// <value>The object.</value>
        public dynamic Object { get; }

        /// <summary>
        /// Sets up object builder
        /// </summary>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns></returns>
        public dynamic ObjectSetup(params dynamic[] constructorArgs)
        {
            _buildType["Object"] = new Activate<TObjectProtoType>(constructorArgs);
            return this;
        }

        /// <summary>
        /// Sets up object builder
        /// </summary>
        /// <param name="constructorArgsFactory"></param>
        /// <returns></returns>
        public dynamic ObjectSetup(Func<object[]> constructorArgsFactory)
        {
            return ObjectSetup((object) constructorArgsFactory);
        }

        /// <summary>
        /// Trapoline for setting up Builders
        /// </summary>
        public dynamic Setup { get; private set; }


        ///<summary>
        /// Trampoline for builder
        ///</summary>
        public class BuilderTrampoline:DynamicObject
        {
            Builder<TObjectProtoType> _buider;

            /// <summary>
            /// Initializes a new instance of the <see cref="Builder{TObjectProtoType}.BuilderTrampoline"/> class.
            /// </summary>
            /// <param name="builder">The builder.</param>
            public BuilderTrampoline(Builder<TObjectProtoType> builder)
            {
				_buider = builder;
			}

            /// <summary>
            /// Tries the invoke.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="args">The args.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                Activate tBuildType;
                if (!_buider._buildType.TryGetValue("Object", out tBuildType))
                    tBuildType = null;

                result = InvokeHelper(binder.CallInfo, args, tBuildType);
                return true;
            }
        }

        /// <summary>
        /// Trampoline for setup builder
        /// </summary>
        public class SetupTrampoline : DynamicObject
        {
			Builder<TObjectProtoType> _buider;

            /// <summary>
            /// Initializes a new instance of the <see cref="Builder{TObjectProtoType}.SetupTrampoline"/> class.
            /// </summary>
            /// <param name="builder">The builder.</param>
			public SetupTrampoline(Builder<TObjectProtoType> builder){
				_buider = builder;
			}

            /// <summary>
            /// Tries the invoke.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="args">The args.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentException">Requires argument names for every argument</exception>
            public override bool TryInvoke(InvokeBinder binder, dynamic[] args, out object result)
            {
				if (binder.CallInfo.ArgumentNames.Count != binder.CallInfo.ArgumentCount)
               		 throw new ArgumentException("Requires argument names for every argument");
                var tArgs = args.Select(it => it is Type ? new Activate(it) : (Activate) it);
                foreach (var tKeyPair in binder.CallInfo.ArgumentNames.Zip(tArgs, (n, a) => new KeyValuePair<string, Activate>(n, a)))
                {
					_buider._buildType[tKeyPair.Key]=tKeyPair.Value;
				}
				result = _buider;
				return true;
            }

        }

        /// <summary>
        /// Tries the set member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		public override bool TrySetMember(SetMemberBinder binder, dynamic value){
            if (value != null)
            {
                if (value is Type)
                {
                    _buildType[binder.Name] = new Activate(value);
                    return true;
                }

                if (value is Activate)
                {
                    _buildType[binder.Name] = value;
                    return true;
                }
            }
            else
            {
                _buildType[binder.Name] = null;
                return true;
            }
			return false;
		}

        /// <summary>
        /// Tries the invoke member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="args">The args.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Type tType;

			Activate tBuildType;
			if(!_buildType.TryGetValue(binder.Name, out tBuildType))
				tBuildType = null;

            if (tBuildType == null && !_buildType.TryGetValue("Object", out tBuildType))
                tBuildType = null;

            result = InvokeHelper(binder.CallInfo, args,tBuildType);
            if (TryTypeForName(binder.Name, out tType))
            {
                var typeInfo = tType.GetTypeInfo();
                if (typeInfo.IsInterface && result != null && !typeInfo.IsAssignableFrom(result.GetType()))
                {
                   // result = Impromptu.DynamicActLike(result, tType);
                }
            }
            return true;

        }

        private static object InvokeHelper(CallInfo callinfo, IList<object> args, Activate buildType =null)
        {
           
            bool tSetWithName = true;
            object tArg = null;
            if (callinfo.ArgumentNames.Count == 0 && callinfo.ArgumentCount == 1)
            {
                tArg =args[0];
                
                if (Util.IsAnonymousType(tArg) || tArg is IEnumerable<KeyValuePair<string, object>>)
                {
                    tSetWithName = false;
                }
            }

            if (tSetWithName && callinfo.ArgumentNames.Count != callinfo.ArgumentCount)
                throw new ArgumentException("Requires argument names for every argument");
            object result;
            if (buildType != null)
            {
                result = buildType.Create();
            }
            else{
                try
                {
                    result = Activator.CreateInstance<TObjectProtoType>();//Try first because faster but doens't work with optional parameters
                }
                catch (Exception)
                {
                    result = Dynamic.InvokeConstructor(typeof (TObjectProtoType));
                }

            }
            if(tSetWithName)
            {
                tArg = callinfo.ArgumentNames.Zip(args, (n, a) => new KeyValuePair<string, object>(n, a));
            }

            return Dynamic.InvokeSetAll(result, tArg);
        }
    }
}
