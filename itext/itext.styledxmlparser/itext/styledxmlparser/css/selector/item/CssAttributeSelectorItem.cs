/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Text;
using System.Text.RegularExpressions;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    /// <summary>
    /// <see cref="ICssSelectorItem"/>
    /// implementation for attribute selectors.
    /// </summary>
    public class CssAttributeSelectorItem : ICssSelectorItem {
//\cond DO_NOT_DOCUMENT
        /// <summary>Special characters that needs to be escaped when used as literal value in a pattern.</summary>
        /// <remarks>
        /// Special characters that needs to be escaped when used as literal value in a pattern.
        /// White space and '#' characters are included as they are dotnet special characters.
        /// Java allows to escape such (but not all) non-special characters and treats them as literal value.
        /// </remarks>
        internal const String SPECIAL_CHARACTERS = "\\[]/^$.|?*+(){}# \t\n\r" + "\u000B\u000C\u001C\u001D\u001E\u001F\u00A0\u1680\u2000\u2001\u2002\u2003"
             + "\u2004\u2005\u2006\u2007\u2008\u2009\u200A\u2028\u2029\u202F\u205F\u3000";
//\endcond

        /// <summary>The property.</summary>
        private String property;

        /// <summary>The match symbol.</summary>
        private char matchSymbol = (char)0;

        /// <summary>The value.</summary>
        private String value = null;

        /// <summary>
        /// Creates a new
        /// <see cref="CssAttributeSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="attrSelector">the attribute</param>
        public CssAttributeSelectorItem(String attrSelector) {
            int indexOfEqual = attrSelector.IndexOf('=');
            if (indexOfEqual == -1) {
                property = attrSelector.JSubstring(1, attrSelector.Length - 1);
            }
            else {
                if (attrSelector[indexOfEqual + 1] == '"' || attrSelector[indexOfEqual + 1] == '\'') {
                    value = attrSelector.JSubstring(indexOfEqual + 2, attrSelector.Length - 2);
                }
                else {
                    value = attrSelector.JSubstring(indexOfEqual + 1, attrSelector.Length - 1);
                }
                matchSymbol = attrSelector[indexOfEqual - 1];
                if ("~^$*|".IndexOf(matchSymbol) == -1) {
                    matchSymbol = (char)0;
                    property = attrSelector.JSubstring(1, indexOfEqual);
                }
                else {
                    property = attrSelector.JSubstring(1, indexOfEqual - 1);
                }
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#getSpecificity()
        */
        public virtual int GetSpecificity() {
            return CssSpecificityConstants.CLASS_SPECIFICITY;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            IElementNode element = (IElementNode)node;
            String attributeValue = element.GetAttribute(property);
            if (attributeValue == null) {
                return false;
            }
            if (value == null) {
                return true;
            }
            else {
                switch (matchSymbol) {
                    case (char)0: {
                        return value.Equals(attributeValue);
                    }

                    case '|': {
                        return value.Length > 0 && attributeValue.StartsWith(value) && (attributeValue.Length == value.Length || attributeValue
                            [value.Length] == '-');
                    }

                    case '^': {
                        return value.Length > 0 && attributeValue.StartsWith(value);
                    }

                    case '$': {
                        return value.Length > 0 && attributeValue.EndsWith(value);
                    }

                    case '~': {
                        String literalValue = EscapeSpecialCharacters(value);
                        String pattern = MessageFormatUtil.Format("(^{0}\\s+)|(\\s+{1}\\s+)|(\\s+{2}$)", literalValue, literalValue
                            , literalValue);
                        return iText.Commons.Utils.Matcher.Match(iText.Commons.Utils.StringUtil.RegexCompile(pattern), attributeValue
                            ).Matches();
                    }

                    case '*': {
                        return value.Length > 0 && attributeValue.Contains(value);
                    }

                    default: {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Escapes special characters determined by
        /// <see cref="SPECIAL_CHARACTERS"/>
        /// in provided
        /// <see cref="System.String"/>
        /// in order to be used as literal value in a pattern.
        /// </summary>
        /// <remarks>
        /// Escapes special characters determined by
        /// <see cref="SPECIAL_CHARACTERS"/>
        /// in provided
        /// <see cref="System.String"/>
        /// in order to be used as literal value in a pattern.
        /// Note that special characters contain white space and '#' characters to match dotnet special characters.
        /// Java allows to escape such (but not all) non-special characters and treats them as literal characters.
        /// </remarks>
        /// <param name="input">String to escape special characters in</param>
        /// <returns>string with special characters escaped</returns>
        public static String EscapeSpecialCharacters(String input) {
            int firstEscapeIndex = -1;
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (IsSpecialCharacter(c)) {
                    firstEscapeIndex = i;
                    break;
                }
            }
            if (firstEscapeIndex == -1) {
                return input;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.JAppend(input, 0, firstEscapeIndex);
            for (int i = firstEscapeIndex; i < input.Length; ++i) {
                char c = input[i];
                if (IsSpecialCharacter(c)) {
                    stringBuilder.Append('\\');
                }
                stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        private static bool IsSpecialCharacter(char c) {
            return SPECIAL_CHARACTERS.IndexOf(c) != -1;
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            if (value == null) {
                return MessageFormatUtil.Format("[{0}]", property);
            }
            else {
                return MessageFormatUtil.Format("[{0}{1}=\"{2}\"]", property, matchSymbol == 0 ? "" : matchSymbol.ToString
                    (), value);
            }
        }
    }
}
