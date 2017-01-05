using System;

namespace iText.Test {
    internal static class PdfTestExtensions {
        public static Attribute GetCustomAttribute(this Type classType, Type attributeType) {
            return Attribute.GetCustomAttribute(classType, attributeType);
        }
    }
}