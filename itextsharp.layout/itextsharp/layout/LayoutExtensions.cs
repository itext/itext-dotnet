using System;

namespace iTextSharp.Layout {
    internal static class LayoutExtensions {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void JGetChars(this String str, int srcBegin, int srcEnd, char[] dst, int dstBegin) {
            str.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }
    }
}
