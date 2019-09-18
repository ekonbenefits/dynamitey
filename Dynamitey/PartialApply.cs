using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

using System.Text;
using System.Reflection;
using Dynamitey.Internal.Optimization;

namespace Dynamitey
{



    /// <summary>
    /// Internal method for subsequent invocations of <see cref="Dynamic.Curry(object,System.Nullable{int})"/>
    /// </summary>
   
    public class PartialApply : DynamicObject, IPartialApply
    {

        /// <summary>
        /// Pipes the argument into the function
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static dynamic operator |(dynamic argument, PartialApply function)
        {
           return ((dynamic)function)(argument);
        }

        /// <summary>
        /// Provides implementation for binary operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as addition and multiplication.
        /// </summary>
        /// <param name="binder">Provides information about the binary operation. The binder.Operation property returns an <see cref="T:System.Linq.Expressions.ExpressionType" /> object. For example, for the sum = first + second statement, where first and second are derived from the DynamicObject class, binder.Operation returns ExpressionType.Add.</param>
        /// <param name="arg">The right operand for the binary operation. For example, for the sum = first + second statement, where first and second are derived from the DynamicObject class, <paramref name="arg" /> is equal to second.</param>
        /// <param name="result">The result of the binary operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            result = null;
            if (binder.Operation == ExpressionType.LeftShift)
            {
                result = ((dynamic)(this))(arg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
        /// </summary>
        /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
        /// <param name="result">The result of the type conversion operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Dynamic.CoerceToDelegate(this, binder.Type);

            return result != null;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PartialApply" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="invocationKind">Kind of the invocation.</param>
        public PartialApply(object target, object[] args, string memberName = null, int? totalCount = null, InvocationKind? invocationKind = null)
        {
            _target = target;
            _memberName = memberName;
            _invocationKind = invocationKind ?? (String.IsNullOrWhiteSpace(_memberName)
                                     ? InvocationKind.InvokeUnknown
                                     : InvocationKind.InvokeMemberUnknown);
            _totalArgCount = totalCount;
            _args = args;
        }

       
        private readonly int? _totalArgCount;
       
        private readonly object _target;
       
        private readonly string _memberName;
       
        private readonly object[] _args;
       
        private readonly InvocationKind _invocationKind;

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target => _target;

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName => _memberName;

        /// <summary>
        /// Gets the args.
        /// </summary>
        /// <value>The args.</value>
        public object[] Args => _args;

        /// <summary>
        /// Gets the total arg count.
        /// </summary>
        /// <value>The total arg count.</value>
        public int? TotalArgCount => _totalArgCount;

        /// <summary>
        /// Gets the kind of the invocation.
        /// </summary>
        /// <value>The kind of the invocation.</value>
        public InvocationKind InvocationKind => _invocationKind;

        private IDictionary<int, CacheableInvocation> _cacheableInvocation = new Dictionary<int, CacheableInvocation>();
#pragma warning disable 1734
        /// <summary>
        /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
        /// </summary>
        /// <param name="binder">Provides information about the invoke operation.</param>
        /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args[0]"/> is equal to 100.</param>
        /// <param name="result">The result of the object invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
#pragma warning restore 1734
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            var tNamedArgs = Util.NameArgsIfNecessary(binder.CallInfo, args);
            var tNewArgs = _args.Concat(tNamedArgs).ToArray();

            if (_totalArgCount.HasValue && (_totalArgCount - Args.Length - args.Length > 0))
            //Not Done currying
            {
                result = new PartialApply(Target, tNewArgs, MemberName,
                                   TotalArgCount, InvocationKind);

                return true;
            }
            var tInvokeDirect = String.IsNullOrWhiteSpace(_memberName);

            if (tInvokeDirect && binder.CallInfo.ArgumentNames.Count == 0 && _target is Delegate tDel)
            //Optimization for direct delegate calls
            {
                result = tDel.FastDynamicInvoke(tNewArgs);
                return true;
            }


            Invocation tInvocation;
            if (binder.CallInfo.ArgumentNames.Count == 0) //If no argument names we can cache the callsite
            {
                if (!_cacheableInvocation.TryGetValue(tNewArgs.Length, out var tCacheableInvocation))
                {
                    tCacheableInvocation = new CacheableInvocation(InvocationKind, _memberName, argCount: tNewArgs.Length, context: _target);
                    _cacheableInvocation[tNewArgs.Length] = tCacheableInvocation;

                }
                tInvocation = tCacheableInvocation;
            }
            else
            {
                tInvocation = new Invocation(InvocationKind, _memberName);
            }

            result = tInvocation.Invoke(_target, tNewArgs);


            return true;
        }
    }

    /// <summary>
    /// Partial Application Proxy
    /// </summary>
    public interface IPartialApply
    {
    }
}
