// 
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
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dynamitey.Internal.Compat;

namespace Dynamitey.DynamicObjects
{

    /// <summary>
    /// Can Represent an equivalent static type to help dynamically convert member output
    /// </summary>
    public interface IEquivalentType
    {
        /// <summary>
        /// Gets or sets the type of the equivalent.
        /// </summary>
        /// <value>
        /// The type of the equivalent.
        /// </value>
        FauxType EquivalentType { get; set; }
    }
    
    


    /// <summary>
    /// Dynamic Object that knows about the Impromtu Interface return types;
    /// Override Typical Dynamic Object methods, and use TypeForName to get the return type of an interface member.
    /// </summary>
    public abstract class BaseObject : DynamicObject, IEquivalentType

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class.
        /// </summary>
        protected BaseObject()
        {
            
        }



        /// <summary>
        /// Tries the name of the member to see if it has a type.
        /// </summary>
        /// <param name="binderName">Name of the binder.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public bool TryTypeForName(string binderName, out Type type)
        {
           var eqType = (IEquivalentType) this;
           type = null;
           if (eqType.EquivalentType == null)
                return false;

           var types = eqType.EquivalentType.GetMember(binderName)
               .Select(it =>
                           {

                               switch (it)
                               {
                                  case PropertyInfo p:
                                      return p.PropertyType;
                                  case MethodInfo m:
                                      return m.ReturnType;
                                  case EventInfo e:
                                      return e.EventHandlerType;
#if NETFRAMEWORK || PROFILE158
                                  case Type t:
                                      return t;
#else
                                  case TypeInfo t:
                                       return t.UnderlyingSystemType;
#endif
                                  default:
                                      return typeof (object);
                               }
                               
                           }).ToList();

;
            if (!types.Any())
                return false;
            foreach (var currenttype in types)
            {
                if (type == null || type.Name == currenttype.Name)
                    type = currenttype;
                else
                    type = typeof (object);
            }
            return true;
        }


        FauxType IEquivalentType.EquivalentType { get; set; }
    }
}
