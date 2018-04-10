using System;

namespace iText.Svg.Utils {
    /// <summary>Class containing utility methods for text operations in the context of SVG processing</summary>
    public class SvgTextUtil {
        /// <summary>Trim all the leading whitespace characters from the passed string</summary>
        /// <param name="toTrim">string to trim</param>
        /// <returns>string with all leading whitespace characters removed</returns>
        public static String TrimLeadingWhitespace(String toTrim) {
            int current = 0;
            int end = toTrim.Length;
            while (current < end) {
                char currentChar = toTrim[current];
                if (iText.IO.Util.TextUtil.IsWhiteSpace(currentChar) && !(currentChar == '\n' || currentChar == '\r')) {
                    //if the character is whitespace and not a newline, increase current
                    current++;
                }
                else {
                    break;
                }
            }
            return toTrim.Substring(current);
        }

        /// <summary>Trim all the trailing whitespace characters from the passed string</summary>
        /// <param name="toTrim">string to trom</param>
        /// <returns>string with al trailing whitespace characters removed</returns>
        public static String TrimTrailingWhitespace(String toTrim) {
            int end = toTrim.Length;
            if (end > 0) {
                int current = end - 1;
                while (current > 0) {
                    char currentChar = toTrim[current];
                    if (iText.IO.Util.TextUtil.IsWhiteSpace(currentChar) && !(currentChar == '\n' || currentChar == '\r')) {
                        //if the character is whitespace and not a newline, increase current
                        current--;
                    }
                    else {
                        break;
                    }
                }
                if (current == 0) {
                    return "";
                }
                else {
                    return toTrim.JSubstring(0, current + 1);
                }
            }
            else {
                return toTrim;
            }
        }
    }
}
