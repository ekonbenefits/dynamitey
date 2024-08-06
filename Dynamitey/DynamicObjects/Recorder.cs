// 
//  Copyright 2011  Ekon Benefits
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


using Dynamitey.Internal.Optimization;


namespace Dynamitey.DynamicObjects
{
    /// <summary>
    /// Proxy that Records Dynamic Invocations on an object
    /// </summary>
    public class Recorder:BaseForwarder
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Recorder"/> class.
        /// </summary>
        public Recorder():base(new Dummy())
        {
            Recording = new List<Invocation>();
        }

        /// <summary>
        /// Gets or sets the recording.
        /// </summary>
        /// <value>The recording.</value>
       
        public IList<Invocation> Recording { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Recorder"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public Recorder(object target) : base(target)
        {
            Recording = new List<Invocation>();
        }

        /// <summary>
        /// Replays the recording on target.
        /// </summary>
        /// <param name="target">The target.</param>
        public T ReplayOn<T>(T target)
        {
            foreach (var tInvocation in Recording)
            {
                tInvocation.InvokeWithStoredArgs(target);
            }

            return target;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result))
            {
                Recording.Add(new Invocation(InvocationKind.Get,binder.Name));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the set member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            if (base.TrySetMember(binder, value))
            {
                Recording.Add(new Invocation(InvocationKind.Set,binder.Name,value));
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
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            if (base.TryInvokeMember(binder, args, out result))
            {
                Recording.Add(new Invocation(InvocationKind.InvokeMemberUnknown, binder.Name, Util.NameArgsIfNecessary(binder.CallInfo, args)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the index of the get.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool TryGetIndex(System.Dynamic.GetIndexBinder binder, object[] indexes, out object result)
        {
            if (base.TryGetIndex(binder, indexes, out result))
            {
                Recording.Add(new Invocation(InvocationKind.GetIndex, Invocation.IndexBinderName, Util.NameArgsIfNecessary(binder.CallInfo, indexes)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries the index of the set.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="indexes">The indexes.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override bool TrySetIndex(System.Dynamic.SetIndexBinder binder, object[] indexes, object value)
        {
            if (base.TrySetIndex(binder, indexes, value))
            {
                var tCombinedArgs = indexes.Concat(new[] { value }).ToArray();
                Recording.Add(new Invocation(InvocationKind.GetIndex, Invocation.IndexBinderName, Util.NameArgsIfNecessary(binder.CallInfo, tCombinedArgs)));
                return true;
            }
            return false;
        }

    }
}
