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
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;

using System.Reflection;
namespace Dynamitey.DynamicObjects
{
    /// <summary>
    /// Dynamic Object that knows about the Impromtu Interface return types;
    /// Override Typical Dynamic Object methods, and use TypeForName to get the return type of an interface member.
    /// </summary>
     
    public interface IEquivalentType
    {
        Type EquivalentType { get; set; }
    }

    [DataContract]
    public abstract class BaseObject : DynamicObject, IEquivalentType

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class.
        /// </summary>
        protected BaseObject()
        {
            
        }



        public bool TryTypeForName(string binderName, out Type type)
        {
           var eqType = (IEquivalentType) this;
           var types = eqType.EquivalentType.GetMember(binderName)
               .Where(it => it is PropertyInfo || it is MethodInfo || it is EventInfo)
               .Select(it =>
                           {
                               var prop =it as PropertyInfo;
                               if (prop != null)
                                   return prop.PropertyType;
                               var mem = it as MethodInfo;
                               if (mem != null)
                                   return mem.ReturnType;
                               var eve = it as EventInfo;
                               if (eve != null)
                                   return eve.EventHandlerType;
                               return typeof (object);
                           }).ToList();

            type = null;
            if (!types.Any())
                return false;
            foreach (var currenttype in types)
            {
                if (type == null || type == currenttype)
                    type = currenttype;
                else
                    type = typeof (object);
            }
            return true;
        }

        [DataMember]
        Type IEquivalentType.EquivalentType { get; set; }
    }
}
