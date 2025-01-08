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
                    // If the character is whitespace and not a newline, increase current.
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
                        // If the character is whitespace and not a newline, increase current.
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
            // When svg is parsed by jsoup it leaves all whitespace in text element as is. Meaning that
            // tab/space indented xml files will retain their tabs and spaces.
            // The following regex replaces all whitespace with a single space.
            String whiteSpace = root.GetAttributeMapCopy().IsEmpty() ? CommonCssConstants.NORMAL : root.GetAttribute(CommonCssConstants
                .WHITE_SPACE);
            if (whiteSpace == null) {
                // XML_SPACE is 'default' or 'preserve'.
                whiteSpace = root.GetAttribute(SvgConstants.Attributes.XML_SPACE);
                if ("preserve".Equals(whiteSpace)) {
                    whiteSpace = CommonCssConstants.PRE;
                }
                else {
                    whiteSpace = CommonCssConstants.NORMAL;
                }
            }
            bool keepLineBreaks = CommonCssConstants.PRE.Equals(whiteSpace) || CommonCssConstants.PRE_WRAP.Equals(whiteSpace
                ) || CommonCssConstants.PRE_LINE.Equals(whiteSpace) || CommonCssConstants.BREAK_SPACES.Equals(whiteSpace
                );
            bool collapseSpaces = !(CommonCssConstants.PRE.Equals(whiteSpace) || CommonCssConstants.PRE_WRAP.Equals(whiteSpace
                ) || CommonCssConstants.BREAK_SPACES.Equals(whiteSpace));
            foreach (ISvgTextNodeRenderer child in root.GetChildren()) {
                // If child is leaf, process contents, if it is branch, call function again.
                if (child is TextSvgBranchRenderer) {
                    // Branch processing.
                    ProcessWhiteSpace((TextSvgBranchRenderer)child, child.ContainsAbsolutePositionChange() || isLeadingElement
                        );
                    ((TextSvgBranchRenderer)child).MarkWhiteSpaceProcessed();
                    isLeadingElement = false;
                }
                if (child is TextLeafSvgNodeRenderer) {
                    // Leaf processing.
                    TextLeafSvgNodeRenderer leafRend = (TextLeafSvgNodeRenderer)child;
                    // Process text.
                    String toProcess = leafRend.GetAttribute(SvgConstants.Attributes.TEXT_CONTENT);
                    // For now, text element contains single line and no-wrapping by default.
                    toProcess = toProcess.Replace("\n", "");
                    toProcess = WhiteSpaceUtil.ProcessWhitespaces(toProcess, keepLineBreaks, collapseSpaces);
                    if (!keepLineBreaks) {
                        if (isLeadingElement) {
                            // Trim leading and trailing whitespaces.
                            toProcess = TrimLeadingWhitespace(toProcess);
                            toProcess = TrimTrailingWhitespace(toProcess);
                            isLeadingElement = false;
                        }
                        else {
                            // Only trim trailing whitespaces.
                            toProcess = TrimTrailingWhitespace(toProcess);
                        }
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
            // Trim leading whitespace.
            trimmedText = iText.Svg.Utils.SvgTextUtil.TrimLeadingWhitespace(trimmedText);
            // Trim trailing whitespace.
            trimmedText = iText.Svg.Utils.SvgTextUtil.TrimTrailingWhitespace(trimmedText);
            return "".Equals(trimmedText);
        }

        /// <summary>Resolve the font size stored inside the passed renderer</summary>
        /// <param name="renderer">renderer containing the font size declaration</param>
        /// <param name="parentFontSize">parent font size to fall back on if the renderer does not contain a font size declarations or if the stored declaration is invalid
        ///     </param>
        /// <returns>float containing the font-size, or the parent font size if the renderer's declaration cannot be resolved
        ///     </returns>
        [System.ObsoleteAttribute(@"will be removed together with iText.Svg.Renderers.Impl.TextLeafSvgNodeRenderer.GetTextContentLength(float, iText.Kernel.Font.PdfFont)"
            )]
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
