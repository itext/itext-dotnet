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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    /// <summary>
    /// <see cref="ICssSelectorItem"/>
    /// implementation for attribute selectors.
    /// </summary>
    public class CssAttributeSelectorItem : ICssSelectorItem {
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
                        String pattern = MessageFormatUtil.Format("(^{0}\\s+)|(\\s+{1}\\s+)|(\\s+{2}$)", value, value, value);
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
