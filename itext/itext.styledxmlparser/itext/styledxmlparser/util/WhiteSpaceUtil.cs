using System;
using System.Collections.Generic;
using System.Text;

namespace iText.StyledXmlParser.Util {
    /// <summary>Utility class for white-space handling methods that are used both in pdfHTML and the iText-core SVG module
    ///     </summary>
    public class WhiteSpaceUtil {
        private static readonly ICollection<char> EM_SPACES = new HashSet<char>();

        static WhiteSpaceUtil() {
            EM_SPACES.Add((char)0x2002);
            EM_SPACES.Add((char)0x2003);
            EM_SPACES.Add((char)0x2009);
        }

        /// <summary>Collapse all consecutive spaces of the passed String into single spaces</summary>
        /// <param name="s">String to collapse</param>
        /// <returns>a String containing the contents of the input, with consecutive spaces collapsed</returns>
        public static String CollapseConsecutiveSpaces(String s) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++) {
                if (IsNonEmSpace(s[i])) {
                    if (sb.Length == 0 || !IsNonEmSpace(sb[sb.Length - 1])) {
                        sb.Append(" ");
                    }
                }
                else {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>Checks if a character is white space value that is not em, en or similar special whitespace character.
        ///     </summary>
        /// <param name="ch">the character</param>
        /// <returns>true, if the character is a white space character, but no em, en or similar</returns>
        public static bool IsNonEmSpace(char ch) {
            return iText.IO.Util.TextUtil.IsWhiteSpace(ch) && !EM_SPACES.Contains(ch);
        }
    }
}
