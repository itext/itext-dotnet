using System;
using System.Linq;
using System.Reflection;

namespace iText.Test {
    internal static class PdfTestExtensions {
        public static Attribute GetCustomAttribute(this Type classType, Type attributeType) {
#if !NETSTANDARD1_6
            return Attribute.GetCustomAttribute(classType, attributeType);
#else
            return classType.GetTypeInfo().GetCustomAttribute(attributeType);
#endif
        }

        public static Assembly GetAssembly(this Type type) {
#if !NETSTANDARD1_6
            return type.Assembly;
#else
            return type.GetTypeInfo().Assembly;
#endif
        }

#if NETSTANDARD1_6
        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit) {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }
#endif
    }
}