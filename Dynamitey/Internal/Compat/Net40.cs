



namespace Dynamitey.Internal.Compat
{
	using System.Globalization;

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

