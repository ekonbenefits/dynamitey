using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Dynamitey.Internal.Optimization;


namespace Dynamitey.DynamicObjects
{
    /// <summary>
    /// Late bind types from libraries not not at compile type
    /// </summary>
    public class LateType:BaseForwarder
    {


        /// <summary>
        /// Exception When The Late Type can not be found to bind.
        /// </summary>
        public class MissingTypeException:Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MissingTypeException" /> class.
            /// </summary>
            /// <param name="typename">The typename.</param>
             public MissingTypeException(string typename)
                 : base(String.Format("Could Not Find Type. {0}", typename))
             {
                 
             }

             /// <summary>
             /// Initializes a new instance of the <see cref="MissingTypeException" /> class.
             /// </summary>
             /// <param name="message">The message.</param>
             /// <param name="innerException">The inner exception.</param>
            public MissingTypeException(string message, Exception innerException) : base(message, innerException)
            {
                
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LateType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public LateType(Type type)
            : base(type)
        {

        }

        private readonly string TypeName;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LateType"/> class.
        /// </summary>
        /// <param name="typeName">Qualified Name of the type.</param>
        public LateType(string typeName)
            : base(Type.GetType(typeName, false))
        {
            TypeName = typeName;
          
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LateType" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        public LateType(Assembly assembly, string typeName)
            : base(assembly.GetType(typeName, false))
        {
            TypeName = typeName;

        }

        /// <summary>
        /// Returns a late bound constructor
        /// </summary>
        /// <value>The late bound constructor</value>
        public dynamic @new
        {
            get { return new ConstructorForward((Type)Target); }
        }

        /// <summary>
        /// Forward argument to constructor including named arguments
        /// </summary>
        public class ConstructorForward:DynamicObject
        {
            private readonly Type _type;
            internal ConstructorForward(Type type)
            {
                _type = type;
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
                result = Dynamic.InvokeConstructor(_type, Util.NameArgsIfNecessary(binder.CallInfo, args));
                return true;
            }

        }

        /// <summary>
        /// Gets a value indicating whether this Type is available at runtime.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
        /// </value>
        public bool IsAvailable
        {
            get { return Target != null; }
        }


        /// <summary>
        /// Gets the call target.
        /// </summary>
        /// <value>
        /// The call target.
        /// </value>
        /// <exception cref="Dynamitey.DynamicObjects.LateType.MissingTypeException"></exception>
        protected override object CallTarget
        {
            get
            {
                if(Target ==null)
                    throw new MissingTypeException(TypeName);

                return InvokeContext.CreateStatic((Type)Target);
            }
        }
    


    }
}
