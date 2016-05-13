using System;
using System.Text;

namespace iTextSharp.Barcodes
{
    internal static class BarcodesExtensions {
        public static byte[] GetBytes(this String str, String encoding) {
            return Encoding.GetEncoding(encoding).GetBytes(str);
        }

        public static String JSubstring(this String str, int beginIndex, int endIndex)
        {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }
    }
}