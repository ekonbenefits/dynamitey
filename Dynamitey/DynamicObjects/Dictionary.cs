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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;


namespace Dynamitey.DynamicObjects
{
    /// <summary>
    /// Similar to Expando Objects but handles null values when the property is defined with an impromptu interface
    /// </summary>
      
    
     public class Dictionary:BaseDictionary,IDictionary<string,object>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        public Dictionary() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        public Dictionary(IEnumerable<KeyValuePair<string, object>> dict) : base(dict)
        {
        }


//#if !SILVERLIGHT
//        /// <summary>
//        /// Initializes a new instance of the <see cref="Dictionary"/> class.
//        /// </summary>
//        /// <param name="info">The info.</param>
//        /// <param name="context">The context.</param>
//        protected Dictionary(SerializationInfo info, 
//           StreamingContext context):base(info,context)
//        {

//        }
//#endif

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _dictionary.Count; }
        }


        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
           return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            var tKeys = Keys;
           

            _dictionary.Clear();

            foreach (var tKey in tKeys)
            {
                OnPropertyChanged(tKey);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return _dictionary[key]; }
            set
            {
                SetProperty(key, value);
            }
        }
    }


    /// <summary>
    /// Adds extra synatx to intialize properties to match up with clay
    /// </summary>
     
		public class ChainableDictionary:Dictionary{


            /// <summary>
            /// Initializes a new instance of the <see cref="ChainableDictionary"/> class.
            /// </summary>
        public ChainableDictionary() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        /// <param name="dict">The dict.</param>
        public ChainableDictionary(IEnumerable<KeyValuePair<string, object>> dict) : base(dict)
        {
        }


			public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result)
			{
				if(base.TryInvokeMember (binder, args, out result)){
					return true;
				}
				if(binder.CallInfo.ArgumentCount ==1){
					 SetProperty(binder.Name, args.FirstOrDefault());
					result = this;
					return true;
				}
                if (binder.CallInfo.ArgumentCount > 1)
                {
                    SetProperty(binder.Name,new List(args));
                    result = this;
                    return true;
                }
				
				return false;
			}
		}
}
