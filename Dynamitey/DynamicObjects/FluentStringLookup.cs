using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using System.Text;

using System.Reflection;
namespace Dynamitey.DynamicObjects
{


    /// <summary>
    /// Building block to use Method calls as dynamic lookups
    /// </summary>
    public class FluentStringLookup:DynamicObject
    {
       
        private readonly Func<string, dynamic> _lookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentStringLookup"/> class.
        /// </summary>
        /// <param name="lookup">The lookup.</param>
        public FluentStringLookup(Func<string,dynamic> lookup)
        {
            _lookup = lookup;
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
            result = _lookup(binder.Name);
            return true;
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
            result = null;
            if (args.Length == 1 && args.First() is String)
            {
                result = _lookup(args[0] as String);
                return true;
            }
            return false;
        }


    }
}
