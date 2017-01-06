using System;
using System.Reflection;

namespace iText.Test {
    internal static class PdfTestExtensions {
        public static Attribute GetCustomAttribute(this Type classType, Type attributeType) {
            return Attribute.GetCustomAttribute(classType, attributeType);
        }

        public static Assembly GetAssembly(this Type type) {
            return type.Assembly;
        }
    }
}