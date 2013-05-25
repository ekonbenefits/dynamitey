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

     
        public class MissingTypeException:Exception
        {
             public MissingTypeException(string typename)
                 : base(String.Format("Could Not Find Type. {0}", typename))
             {
                 
             }

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
