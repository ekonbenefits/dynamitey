using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace Dynamitey.DynamicObjects
{


    /// <summary>
    /// Proxy that can turn extension methods into instance methods 
    /// </summary>
    public class ExtensionToInstanceProxy: BaseForwarder
    {
       
        private readonly Type _extendedType;
       
        private readonly Type[] _staticTypes;
       
        private readonly Type[] _instanceHints;

        /// <summary>
        /// Gets the instance hints.
        /// </summary>
        /// <value>
        /// The instance hints.
        /// </value>
        public IEnumerable<Type> InstanceHints
        {
            get { return _instanceHints/* ?? KnownInterfaces*/; }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionToInstanceProxy" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="staticTypes">The static types.</param>
        /// <param name="instanceHints">The instance hints.</param>
        /// <exception cref="System.ArgumentException">Don't Nest ExtensionToInstance Objects</exception>
        public ExtensionToInstanceProxy(dynamic target,  Type extendedType, Type[] staticTypes, Type[] instanceHints = null):base((object)target)
        {
            _staticTypes = staticTypes;
            _extendedType = extendedType;
            _instanceHints = instanceHints;

            if(target is ExtensionToInstanceProxy)
                throw new ArgumentException("Don't Nest ExtensionToInstance Objects");

            if (IsExtendedType(target))
            {
                return;
            }

            throw new ArgumentException(String.Format("Non a valid {0} to be wrapped.",_extendedType));
            
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {

            if (!base.TryGetMember(binder, out result))
            {

                var tInterface = CallTarget.GetType().GetTypeInfo().GetInterfaces().Single(it => it.Name == _extendedType.Name);
                var typeInfo = tInterface.GetTypeInfo();
                result = new Invoker(binder.Name,
                                     typeInfo.IsGenericType ? typeInfo.GetGenericArguments() : new Type[] {},null, this);
            }
            return true;
        }

        /// <summary>
        /// Basic Invoker syntax for dynamic generics
        /// </summary>
        public class Invoker:BaseObject
        {
            /// <summary>
            /// The name
            /// </summary>
            protected string Name;
            /// <summary>
            /// The parent
            /// </summary>
            protected ExtensionToInstanceProxy Parent;
            /// <summary>
            /// The overload types
            /// </summary>
            protected IDictionary<int, Type[]> OverloadTypes;
            /// <summary>
            /// The generic params
            /// </summary>
            protected Type[] GenericParams;
            /// <summary>
            /// The generic method parameters
            /// </summary>
            protected Type[] GenericMethodParameters;

            internal Invoker(string name, Type[] genericParameters, Type[] genericMethodParameters, ExtensionToInstanceProxy parent, Type[] overloadTypes = null)
            {
                Name = name;
                Parent = parent;
                GenericParams = genericParameters;
                GenericMethodParameters = genericMethodParameters;
                OverloadTypes = new Dictionary<int,Type[]>();

                if (overloadTypes == null)
                {

                    foreach (var tGenInterface in parent.InstanceHints)
                    {
                        var tNewType = tGenInterface;

                        if (tNewType.GetTypeInfo().IsGenericType)
                        {
                            tNewType = tNewType.MakeGenericType(GenericParams);
                        }

                        var members = tNewType.GetTypeInfo().GetMethods(BindingFlags.Instance |
                                                                                   BindingFlags.Public).Where(
                                                                                       it => it.Name == Name).ToList();
                        foreach (var tMethodInfo in members)
                        {
                            var tParams = tMethodInfo.GetParameters().Select(it => it.ParameterType).ToArray();

                            if (OverloadTypes.ContainsKey(tParams.Length))
                            {
                                OverloadTypes[tParams.Length] = new Type[] {};
                            }
                            else
                            {
                                OverloadTypes[tParams.Length] = tParams.Select(ReplaceGenericTypes).ToArray();
                            }
                        }

                        foreach (var tOverloadType in OverloadTypes.ToList())
                        {
                            if (tOverloadType.Value.Length == 0)
                            {
                                OverloadTypes.Remove(tOverloadType);
                            }
                        }

                    }
                }
                else
                    {
                        OverloadTypes[overloadTypes.Length] = overloadTypes;
                    }
            }

            private Type ReplaceGenericTypes(Type type)
            {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsGenericType && typeInfo.ContainsGenericParameters)
                {
                    var tArgs = typeInfo.GetGenericArguments();

                    tArgs = tArgs.Select(ReplaceGenericTypes).ToArray();

                    return type.GetGenericTypeDefinition().MakeGenericType(tArgs);
                }

                if (typeInfo.ContainsGenericParameters)
                {
                    return typeof (object);
                }
               
                return type;
            }

            /// <summary>
            /// Tries the get member.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (binder.Name == "Overloads")
                {
                    result = new OverloadInvoker(Name, GenericParams,GenericMethodParameters, Parent);
                    return true;
                }
                return base.TryGetMember(binder, out result);
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
                object[] tArgs = args;
                if (OverloadTypes.ContainsKey(args.Length))
                {
                    tArgs = OverloadTypes[args.Length].Zip(args, Tuple.Create)
                        .Select(it => it.Item2 != null ? Dynamic.InvokeConvert(it.Item2, it.Item1, @explicit: true) : null).ToArray();
                    
                }

                var name = InvokeMemberName.Create(Name, GenericMethodParameters);

                result = Parent.InvokeStaticMethod(name, tArgs);
                return true;
            }

            /// <summary>
            /// Tries the index of the get.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="indexes">The indexes.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                result = new Invoker(Name, GenericParams, indexes.Select(it => Dynamic.InvokeConvert(it, typeof(Type), @explicit: true)).Cast<Type>().ToArray(), Parent);
                return true;
            }
        }

        /// <summary>
        /// Overload Invoker
        /// </summary>
        public class OverloadInvoker:Invoker
        {
            internal OverloadInvoker(string name, Type[] genericParameters, Type[] genericMethodParameters, ExtensionToInstanceProxy parent)
                : base(name, genericParameters,genericMethodParameters, parent)
            {
            }


            /// <summary>
            /// Tries the index of the get.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="indexes">The indexes.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                result = new Invoker(Name, GenericParams, GenericMethodParameters, Parent, indexes.Select(it => Dynamic.InvokeConvert(it, typeof(Type), @explicit: true)).Cast<Type>().ToArray());
                return true;
            }
        }


        /// <summary>
        /// Tries the invoke member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="args">The args.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            if (!base.TryInvokeMember(binder, args, out result))
            {

                Type[] types = null;
                try
                {
                    IList<Type> typeList =Dynamic.InvokeGet(binder,
                                           "Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder.TypeArguments");
                    if(typeList != null)
                    {

                        types = typeList.ToArray();
                        
                    }

                }catch(RuntimeBinderException)
                {
                    types = null;
                }

                var name=InvokeMemberName.Create;
                result = InvokeStaticMethod(name(binder.Name, types), args);
            }
            return true;
        }

        /// <summary>
        /// Invokes the static method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected object InvokeStaticMethod(String_OR_InvokeMemberName name, object[] args)
        {
            var staticType = InvokeContext.CreateStatic;
            var nameArgs = InvokeMemberName.Create;

            var tList = new List<object> { CallTarget };
            tList.AddRange(args);

            object result =null;
            var sucess = false;
            var exceptionList = new List<Exception>();

            var tGenericPossibles = new List<Type[]>();
            if (name.GenericArgs != null && name.GenericArgs.Length > 0)
            {
                var tInterface = CallTarget.GetType().GetTypeInfo().GetInterfaces().Single(it => it.Name == _extendedType.Name);
                var tTypeGenerics = (tInterface.GetTypeInfo().IsGenericType ? tInterface.GetTypeInfo().GetGenericArguments()
                                            : new Type[] { }).Concat(name.GenericArgs).ToArray();

                tGenericPossibles.Add(tTypeGenerics);
                tGenericPossibles.Add(name.GenericArgs);
            }
            else
            {
                tGenericPossibles.Add(null);
            }
                      


            foreach (var sType in _staticTypes)
            {
                foreach (var tGenericPossible in tGenericPossibles)
                {
                    try
                    {
                        result = Dynamic.InvokeMember(staticType(sType), nameArgs(name.Name, tGenericPossible), tList.ToArray());
                        sucess = true;
                        break;
                    }
                    catch (RuntimeBinderException ex)
                    {
                        exceptionList.Add(ex);
                    }
                }
            }

            if (!sucess)
            {
                throw exceptionList.First();
            }


            Type tOutType;
            if (TryTypeForName(name.Name, out tOutType))
            {
                var outTypeInfo = tOutType.GetTypeInfo();
                if (outTypeInfo.IsInterface)
                {
                    var tIsGeneric = outTypeInfo.IsGenericType;
                    if (outTypeInfo.IsGenericType)
                    {
                        tOutType = tOutType.GetGenericTypeDefinition();
                    }

                    if (InstanceHints.Select(it => tIsGeneric && it.GetTypeInfo().IsGenericType ? it.GetGenericTypeDefinition() : it)
                            .Contains(tOutType))
                    {
                        result = CreateSelf(result, _extendedType, _staticTypes, _instanceHints);
                    }
                }
            }
            else
            {
                if (IsExtendedType(result))
                {
                    result = CreateSelf(result, _extendedType, _staticTypes, _instanceHints);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates the self.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="staticTypes">The static types.</param>
        /// <param name="instanceHints">The instance hints.</param>
        /// <returns></returns>
        protected virtual ExtensionToInstanceProxy CreateSelf(object target, Type extendedType, Type[] staticTypes, Type[] instanceHints)
        {
            return  new ExtensionToInstanceProxy(target,extendedType,staticTypes, instanceHints);
        }

        private bool IsExtendedType(object target)
        {

            if (target is ExtensionToInstanceProxy)
            {
                return false;
            }

            bool genericDef = _extendedType.GetTypeInfo().IsGenericTypeDefinition;

            return target.GetType().GetTypeInfo().GetInterfaces().Any(
                it => ((genericDef && it.GetTypeInfo().IsGenericType) ? it.GetGenericTypeDefinition() : it) == _extendedType);

        }

        
    }
}
