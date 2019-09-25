



using System;
using System.Globalization;

namespace Dynamitey.Internal.Compat
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Globalization;
    using System.Threading;

#if NETSTANDARD2_0
    internal sealed class MaybeNullAttribute : Attribute { }

    internal sealed class DisallowNullAttribute : Attribute { }

    internal sealed class NotNullAttribute : Attribute { }

    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter may be null.
        /// </param>
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }

    }

    internal sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }
    }
#endif



    public static class Net40
    {
#if NET40 || PROFILE158

        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            return del.Method;
        }

       public static CultureInfo GetDefaultThreadCurrentCulture() {
            return Thread.CurrentThread.CurrentCulture;

       }
#else
        public static CultureInfo GetDefaultThreadCurrentCulture() {

            return CultureInfo.DefaultThreadCurrentCulture;

        }

#endif


    }

}

