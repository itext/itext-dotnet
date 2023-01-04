/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Util;
using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Utils {
    /// <summary>Class containing utility methods for text operations in the context of SVG processing</summary>
    public sealed class SvgTextUtil {
        private SvgTextUtil() {
        }

        /// <summary>Trim all the leading whitespace characters from the passed string</summary>
        /// <param name="toTrim">string to trim</param>
        /// <returns>string with all leading whitespace characters removed</returns>
        public static String TrimLeadingWhitespace(String toTrim) {
            if (toTrim == null) {
                return "";
            }
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
        /// <param name="toTrim">string to trim</param>
        /// <returns>string with al trailing whitespace characters removed</returns>
        public static String TrimTrailingWhitespace(String toTrim) {
            if (toTrim == null) {
                return "";
            }
            int end = toTrim.Length;
            if (end > 0) {
                int current = end - 1;
                while (current >= 0) {
                    char currentChar = toTrim[current];
                    if (iText.IO.Util.TextUtil.IsWhiteSpace(currentChar) && !(currentChar == '\n' || currentChar == '\r')) {
                        //if the character is whitespace and not a newline, increase current
                        current--;
                    }
                    else {
                        break;
                    }
                }
                if (current < 0) {
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

        /// <summary>Process the whitespace inside the Text Tree.</summary>
        /// <remarks>
        /// Process the whitespace inside the Text Tree.
        /// Whitespace is collapsed and new lines are handled
        /// A leading element in each subtree is handled different: the preceding whitespace is trimmed instead of kept
        /// </remarks>
        /// <param name="root">root of the text-renderer subtree</param>
        /// <param name="isLeadingElement">true if this element is a leading element(either the first child or the first element after an absolute position change)
        ///     </param>
        public static void ProcessWhiteSpace(TextSvgBranchRenderer root, bool isLeadingElement) {
            // when svg is parsed by jsoup it leaves all whitespace in text element as is. Meaning that
            // tab/space indented xml files will retain their tabs and spaces.
            // The following regex replaces all whitespace with a single space.
            bool performLeadingTrim = isLeadingElement;
            foreach (ISvgTextNodeRenderer child in root.GetChildren()) {
                //If leaf, process contents, if branch, call function again
                if (child is TextSvgBranchRenderer) {
                    //Branch processing
                    ProcessWhiteSpace((TextSvgBranchRenderer)child, child.ContainsAbsolutePositionChange());
                    ((TextSvgBranchRenderer)child).MarkWhiteSpaceProcessed();
                }
                if (child is TextLeafSvgNodeRenderer) {
                    //Leaf processing
                    TextLeafSvgNodeRenderer leafRend = (TextLeafSvgNodeRenderer)child;
                    //Process text
                    String toProcess = leafRend.GetAttribute(SvgConstants.Attributes.TEXT_CONTENT);
                    toProcess = iText.Commons.Utils.StringUtil.ReplaceAll(toProcess, "\\s+", " ");
                    toProcess = WhiteSpaceUtil.CollapseConsecutiveSpaces(toProcess);
                    if (performLeadingTrim) {
                        //Trim leading white spaces
                        toProcess = TrimLeadingWhitespace(toProcess);
                        toProcess = TrimTrailingWhitespace(toProcess);
                        performLeadingTrim = false;
                    }
                    else {
                        //only collapse whitespace
                        toProcess = TrimTrailingWhitespace(toProcess);
                    }
                    leafRend.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, toProcess);
                }
            }
        }

        /// <summary>Check if the String is only composed of whitespace characters</summary>
        /// <param name="s">string to check</param>
        /// <returns>true if the string only contains whitespace characters, false otherwise</returns>
        public static bool IsOnlyWhiteSpace(String s) {
            String trimmedText = iText.Commons.Utils.StringUtil.ReplaceAll(s, "\\s+", " ");
            //Trim leading whitespace
            trimmedText = iText.Svg.Utils.SvgTextUtil.TrimLeadingWhitespace(trimmedText);
            //Trim trailing whitespace
            trimmedText = iText.Svg.Utils.SvgTextUtil.TrimTrailingWhitespace(trimmedText);
            return "".Equals(trimmedText);
        }

        /// <summary>Resolve the font size stored inside the passed renderer</summary>
        /// <param name="renderer">renderer containing the font size declaration</param>
        /// <param name="parentFontSize">parent font size to fall back on if the renderer does not contain a font size declarations or if the stored declaration is invalid
        ///     </param>
        /// <returns>float containing the font-size, or the parent font size if the renderer's declaration cannot be resolved
        ///     </returns>
        public static float ResolveFontSize(ISvgTextNodeRenderer renderer, float parentFontSize) {
            //Use own font-size declaration if it is present, parent's otherwise
            float fontSize = float.NaN;
            String elementFontSize = renderer.GetAttribute(SvgConstants.Attributes.FONT_SIZE);
            if (null != elementFontSize && !String.IsNullOrEmpty(elementFontSize)) {
                if (CssTypesValidationUtils.IsRelativeValue(elementFontSize) || CommonCssConstants.LARGER.Equals(elementFontSize
                    ) || CommonCssConstants.SMALLER.Equals(elementFontSize)) {
                    fontSize = CssDimensionParsingUtils.ParseRelativeFontSize(elementFontSize, parentFontSize);
                }
                else {
                    fontSize = CssDimensionParsingUtils.ParseAbsoluteFontSize(elementFontSize, CommonCssConstants.PX);
                }
            }
            if ((float.IsNaN(fontSize)) || fontSize < 0f) {
                fontSize = parentFontSize;
            }
            return fontSize;
        }

        /// <summary>The reference value may contain a hashtag character or 'url' designation and this method will filter them.
        ///     </summary>
        /// <param name="name">value to be filtered</param>
        /// <returns>filtered value</returns>
        public static String FilterReferenceValue(String name) {
            return name.Replace("#", "").Replace("url(", "").Replace(")", "").Trim();
        }
    }
}
