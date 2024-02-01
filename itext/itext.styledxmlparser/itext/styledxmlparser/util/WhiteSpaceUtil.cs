/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Util {
    /// <summary>Utility class for white-space handling methods that are used both in pdfHTML and the iText-core SVG module
    ///     </summary>
    public class WhiteSpaceUtil {
        private static readonly ICollection<char> EM_SPACES;

        static WhiteSpaceUtil() {
            // HashSet is required in order to autoport correctly in .Net
            HashSet<char> tempSet = new HashSet<char>();
            tempSet.Add((char)0x2002);
            tempSet.Add((char)0x2003);
            tempSet.Add((char)0x2009);
            EM_SPACES = JavaCollectionsUtil.UnmodifiableSet(tempSet);
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
