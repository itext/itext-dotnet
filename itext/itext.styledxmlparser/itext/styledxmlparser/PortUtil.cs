/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf.Colorspace;

namespace iText.StyledXmlParser {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public class PortUtil {
        private PortUtil() {
        }

        public static TextReader WrapInBufferedReader(TextReader inputStreamReader) {
            return inputStreamReader;
        }
        
        /// <summary>
        /// By default "." symbol in regular expressions does not match line terminators.
        /// The issue is more complicated by the fact that "." does not match only "\n" in C#, while it does not
        /// match several other characters as well in Java.
        /// This utility method creates a pattern in which dots match any character, including line terminators
        /// </summary>
        /// <param name="regex">regular expression string</param>
        /// <returns>pattern in which dot characters match any Unicode char, including line terminators</returns>
        public static Regex CreateRegexPatternWithDotMatchingNewlines(String regex) {
            return new Regex(regex, RegexOptions.Singleline);
        }
    }
}
