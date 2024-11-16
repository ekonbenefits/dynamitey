



namespace Dynamitey.Internal.Compat
{

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Globalization;
    using System.Threading;

    public static class Net40
    {
#if NETFRAMEWORK || PROFILE158

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

