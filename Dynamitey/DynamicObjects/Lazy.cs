using System;
using System.Collections.Generic;
using System.Linq;



namespace Dynamitey.DynamicObjects
{

    /// <summary>
    /// Abstract base for the Generic class <see cref="Lazy{T}"/> with <see cref="Create{T}(System.Func{T})"/> fatory methods
    /// </summary>
   
    public abstract class Lazy:BaseForwarder
    {
        /// <summary>
        /// Creates Lazy based on the specified valuefactory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns></returns>
        public static dynamic Create<T>(Func<T> valueFactory)
        {
            return new Lazy<T>(valueFactory);
        }
        /// <summary>
        /// Creates Lazy based on the specified target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static dynamic Create<T>(System.Lazy<T> target)
        {
            return new Lazy<T>(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        protected Lazy(object target) : base(target)
        {
        }

    }

    /// <summary>
    /// Wraps a Lazy Type evalutaes on first method call
    /// </summary>
    /// <typeparam name="T"></typeparam>
   
    public class Lazy<T> : Lazy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public Lazy(System.Lazy<T> target) : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory.</param>
        public Lazy(Func<T> valueFactory ):base(new System.Lazy<T>(valueFactory))
        {
            
        }
     

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return ((System.Lazy<T>)Target).IsValueCreated 
                ? base.GetDynamicMemberNames() 
                : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets the call target.
        /// </summary>
        /// <value>
        /// The call target.
        /// </value>
        protected override object CallTarget => ((System.Lazy<T>)Target).Value!;
    }
}
