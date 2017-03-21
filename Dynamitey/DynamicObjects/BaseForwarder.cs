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
using System.Reflection;
using System.Text;
using Dynamitey.Internal.Optimization;
using Microsoft.CSharp;
using Microsoft.CSharp.RuntimeBinder;

namespace Dynamitey.DynamicObjects
{

    /// <summary>
    /// An proxy object
    /// </summary>
    public interface IForwarder
    {
        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        object Target { get; }
    }

    /// <summary>
    /// Proxies Calls allows subclasser to override do extra actions before or after base invocation
    /// </summary>
    /// <remarks>
    /// This may not be as efficient as other proxies that can work on just static objects or just dynamic objects...
    /// Consider this when using.
    /// </remarks>

    public abstract class BaseForwarder : BaseObject, IForwarder
    {

        /// <summary>
        /// Marks whether we are adding or removing the delegate
        /// </summary>
        public class AddRemoveMarker
        {
            /// <summary>
            /// Implements the operator +.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static AddRemoveMarker operator +(AddRemoveMarker left, object right)
            {
                left.Delegate = right;
                left.IsAdding = true;

                return left;
            }

            /// <summary>
            /// Implements the operator -.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static AddRemoveMarker operator -(AddRemoveMarker left, object right)
            {
                left.Delegate = right;
                left.IsAdding = false;

                return left;
            }

            /// <summary>
            /// Gets or sets the delegate.
            /// </summary>
            /// <value>The delegate.</value>
           
            public object Delegate { get; protected set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is adding.
            /// </summary>
            /// <value><c>true</c> if this instance is adding; otherwise, <c>false</c>.</value>
           
            public bool IsAdding { get; protected set; }

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseForwarder"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        protected BaseForwarder(object target)
        {
            Target = target;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
           
                var tDyanmic = Dynamic.GetMemberNames(CallTarget, dynamicOnly: true);
                if (!tDyanmic.Any())
                {
                    return Dynamic.GetMemberNames(CallTarget);
                }
            
            return base.GetDynamicMemberNames();
        }


        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
         
       
        protected object Target {  get;  set; }

        object IForwarder.Target{get { return Target; }}

        /// <summary>
        /// Gets the call target.
        /// </summary>
        /// <value>The call target.</value>
        protected virtual object CallTarget
        {
            get { return Target; }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (CallTarget == null)
            {
                result = null;
                return false;
            }

            if (Dynamic.InvokeIsEvent(CallTarget, binder.Name))
            {
                result = new AddRemoveMarker();
                return true;
            }

            try
            {
                result = Dynamic.InvokeGet(CallTarget, binder.Name);
            }
            catch (RuntimeBinderException)
            {
                result = null;
                return false;
            }

            return true;

        }
#pragma warning disable 1734
        /// <summary>
        /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
        /// </summary>
        /// <param name="binder">Provides information about the invoke operation.</param>
        /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args[0]" /> is equal to 100.</param>
        /// <param name="result">The result of the object invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
#pragma warning restore 1734
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            if (CallTarget == null)
            {
                result = null;
                return false;
            }

            var tArgs = Util.NameArgsIfNecessary(binder.CallInfo, args);

            try
            {
                result = Dynamic.Invoke(CallTarget, tArgs);

            }
            catch (RuntimeBinderException)
            {
                result = null;
                try
                {
                    Dynamic.InvokeAction(CallTarget, tArgs);
                }
                catch (RuntimeBinderException)
                {

                    return false;
                }
            }
            return true;
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
            if (CallTarget == null)
            {
                result = null;
                return false;
            }

            object[] tArgs = Util.NameArgsIfNecessary(binder.CallInfo, args);

            try
            {
                result = Dynamic.InvokeMember(CallTarget, binder.Name, tArgs);
               
            }
            catch (RuntimeBinderException)
            {
                result = null;
                try
                {
                    Dynamic.InvokeMemberAction(CallTarget, binder.Name, tArgs);
                }
                catch (RuntimeBinderException)
                {

                    return false;
                }
            }
            return true;
        }



        /// <summary>
        /// Tries the set member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (CallTarget == null)
            {
                return false;
            }

            if (Dynamic.InvokeIsEvent(CallTarget, binder.Name) && value is AddRemoveMarker)
            {
                var tValue = value as AddRemoveMarker;

                if (tValue.IsAdding)
                {
                    Dynamic.InvokeAddAssignMember(CallTarget, binder.Name, tValue.Delegate);
                }
                else
                {
                    Dynamic.InvokeSubtractAssignMember(CallTarget, binder.Name, tValue.Delegate);
                }

                return true;
            }

            try
            {
                Dynamic.InvokeSet(CallTarget, binder.Name, value);

                return true;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
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
            if (CallTarget == null)
            {
                result = null;
                return false;
            }

            object[] tArgs = Util.NameArgsIfNecessary(binder.CallInfo, indexes);

            try
            {
                result = Dynamic.InvokeGetIndex(CallTarget, tArgs);
                return true;
            }
            catch (RuntimeBinderException)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Tries the index of the set.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (CallTarget == null)
            {
                return false;
            }

            var tCombinedArgs = indexes.Concat(new[] { value }).ToArray();
            object[] tArgs = Util.NameArgsIfNecessary(binder.CallInfo, tCombinedArgs);
            try
            {


                Dynamic.InvokeSetIndex(CallTarget, tArgs);
                return true;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
        }


        /// <summary>
        /// Equals the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(BaseForwarder other)
        {
            if (ReferenceEquals(null, other)) return ReferenceEquals(null, CallTarget);
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.CallTarget, CallTarget);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return ReferenceEquals(null, CallTarget); 
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BaseForwarder)) return false;
            return Equals((BaseForwarder) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (CallTarget != null ? CallTarget.GetHashCode() : 0);
        }

    
    }
}
