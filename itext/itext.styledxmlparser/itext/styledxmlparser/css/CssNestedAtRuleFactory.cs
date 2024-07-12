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

//\cond DO_NOT_DOCUMENT
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
//\endcond
    }
}
