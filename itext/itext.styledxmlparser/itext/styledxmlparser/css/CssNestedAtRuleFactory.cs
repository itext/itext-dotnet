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
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Page;

namespace iText.StyledXmlParser.Css {
    /// <summary>
    /// A factory for creating
    /// <see cref="CssNestedAtRule"/>
    /// objects.
    /// </summary>
    public sealed class CssNestedAtRuleFactory {
        /// <summary>
        /// Creates a new
        /// <see cref="CssNestedAtRuleFactory"/>
        /// instance.
        /// </summary>
        private CssNestedAtRuleFactory() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssNestedAtRule"/>
        /// object.
        /// </summary>
        /// <param name="ruleDeclaration">the rule declaration</param>
        /// <returns>
        /// a
        /// <see cref="CssNestedAtRule"/>
        /// instance
        /// </returns>
        public static CssNestedAtRule CreateNestedRule(String ruleDeclaration) {
            ruleDeclaration = ruleDeclaration.Trim();
            String ruleName = ExtractRuleNameFromDeclaration(ruleDeclaration);
            String ruleParameters = ruleDeclaration.Substring(ruleName.Length).Trim();
            //TODO: DEVSIX-2263 consider media rules in SVG
            switch (ruleName) {
                case CssRuleName.MEDIA: {
                    return new CssMediaRule(ruleParameters);
                }

                case CssRuleName.PAGE: {
                    return new CssPageRule(ruleParameters);
                }

                case CssRuleName.TOP_LEFT_CORNER:
                case CssRuleName.TOP_LEFT:
                case CssRuleName.TOP_CENTER:
                case CssRuleName.TOP_RIGHT:
                case CssRuleName.TOP_RIGHT_CORNER:
                case CssRuleName.LEFT_TOP:
                case CssRuleName.LEFT_MIDDLE:
                case CssRuleName.LEFT_BOTTOM:
                case CssRuleName.RIGHT_TOP:
                case CssRuleName.RIGHT_MIDDLE:
                case CssRuleName.RIGHT_BOTTOM:
                case CssRuleName.BOTTOM_LEFT_CORNER:
                case CssRuleName.BOTTOM_LEFT:
                case CssRuleName.BOTTOM_CENTER:
                case CssRuleName.BOTTOM_RIGHT:
                case CssRuleName.BOTTOM_RIGHT_CORNER: {
                    return new CssMarginRule(ruleName);
                }

                case CssRuleName.FONT_FACE: {
                    return new CssFontFaceRule();
                }

                default: {
                    return new CssNestedAtRule(ruleName, ruleParameters);
                }
            }
        }

        /// <summary>Extracts the rule name from the CSS rule declaration.</summary>
        /// <param name="ruleDeclaration">the rule declaration</param>
        /// <returns>the rule name</returns>
        internal static String ExtractRuleNameFromDeclaration(String ruleDeclaration) {
            int spaceIndex = ruleDeclaration.IndexOf(' ');
            int colonIndex = ruleDeclaration.IndexOf(':');
            int separatorIndex;
            if (spaceIndex == -1) {
                separatorIndex = colonIndex;
            }
            else {
                if (colonIndex == -1) {
                    separatorIndex = spaceIndex;
                }
                else {
                    separatorIndex = Math.Min(spaceIndex, colonIndex);
                }
            }
            return separatorIndex == -1 ? ruleDeclaration : ruleDeclaration.JSubstring(0, separatorIndex);
        }
    }
}
