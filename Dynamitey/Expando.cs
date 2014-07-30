using System.Dynamic;
using Dynamitey.DynamicObjects;

namespace Dynamitey
{
    public class Expando : Builder<ExpandoObject>
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly dynamic _expandoBuilder = new Builder<ExpandoObject>().Object;
        // ReSharper restore StaticFieldInGenericType        

        /// <summary>
        /// Initializes a new instance of the <see cref="Expando"/> class.
        /// This constructor is shorthand for new Builder&lt;ExpandoObject>();
        /// </summary>
        public Expando()
        {
        }

        /// <summary>
        /// Gets the new expandoObject builder. This method is short hand for Build&gt;ExpandoObject>.NewObject()
        /// </summary>
        /// <value>The new expandoObject.</value>
        public static dynamic New
        {
            get
            {
                return _expandoBuilder;
            }
        }        
    }
}
