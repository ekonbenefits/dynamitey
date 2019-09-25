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


namespace Dynamitey.DynamicObjects
{
    /// <summary>
    /// Similar to Expando Objects but handles null values when the property is defined with an impromptu interface
    /// </summary>
    public class Dictionary:BaseDictionary,IDictionary<string,object?>
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
        public Dictionary(IEnumerable<KeyValuePair<string, object?>> dict) : base(dict)
        {
        }


        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _dictionary.Count;


        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
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
        public object? this[string key]
        {
            get => _dictionary[key];
            set => SetProperty(key, value);
        }
    }


    /// <summary>
    /// Adds extra syntax to initialize properties to match up with clay
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
        public ChainableDictionary(IEnumerable<KeyValuePair<string, object?>> dict) : base(dict)
        {
        }


        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args" /> is equal to 100.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
		public override bool TryInvokeMember (InvokeMemberBinder binder, object?[] args, out object? result)
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
