using System;
using System.Text;

namespace iText.StyledXmlParser
{
    internal static class StyledXmlParserExtensions
    {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static String JSubstring(this StringBuilder sb, int beginIndex, int endIndex) {
            return sb.ToString(beginIndex, endIndex - beginIndex);
        }
    }
}