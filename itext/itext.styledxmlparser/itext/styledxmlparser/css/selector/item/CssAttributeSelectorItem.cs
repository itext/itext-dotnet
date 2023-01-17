/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
