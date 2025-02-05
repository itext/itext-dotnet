/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Utilities class to parse CSS rule sets.</summary>
    public sealed class CssRuleSetParser {
        /// <summary>The logger.</summary>
        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Parse.CssRuleSetParser
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="CssRuleSetParser"/>
        /// instance.
        /// </summary>
        private CssRuleSetParser() {
        }

        /// <summary>Parses property declarations.</summary>
        /// <param name="propertiesStr">
        /// the property declarations in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the list of
        /// <see cref="iText.StyledXmlParser.Css.CssDeclaration"/>
        /// instances
        /// </returns>
        public static IList<CssDeclaration> ParsePropertyDeclarations(String propertiesStr) {
            IList<CssDeclaration> declarations = new List<CssDeclaration>();
            int openedCommentPos = propertiesStr.IndexOf("/*", 0);
            if (openedCommentPos != -1) {
                declarations.AddAll(ParsePropertyDeclarations(propertiesStr.JSubstring(0, openedCommentPos)));
                int closedCommentPos = propertiesStr.IndexOf("*/", openedCommentPos);
                if (closedCommentPos != -1) {
                    declarations.AddAll(ParsePropertyDeclarations(propertiesStr.JSubstring(closedCommentPos + 2, propertiesStr
                        .Length)));
                }
            }
            else {
                int pos = GetSemicolonPosition(propertiesStr, 0);
                while (pos != -1) {
                    String[] propertySplit = SplitCssProperty(propertiesStr.JSubstring(0, pos));
                    if (propertySplit != null) {
                        declarations.Add(new CssDeclaration(propertySplit[0], propertySplit[1]));
                    }
                    propertiesStr = propertiesStr.Substring(pos + 1);
                    pos = GetSemicolonPosition(propertiesStr, 0);
                }
                if (!String.IsNullOrEmpty(iText.Commons.Utils.StringUtil.ReplaceAll(propertiesStr, "[\\n\\r\\t ]", ""))) {
                    String[] propertySplit = SplitCssProperty(propertiesStr);
                    if (propertySplit != null) {
                        declarations.Add(new CssDeclaration(propertySplit[0], propertySplit[1]));
                    }
                    return declarations;
                }
            }
            return declarations;
        }

        /// <summary>
        /// Parses a rule set into a list of
        /// <see cref="iText.StyledXmlParser.Css.CssRuleSet"/>
        /// instances.
        /// </summary>
        /// <remarks>
        /// Parses a rule set into a list of
        /// <see cref="iText.StyledXmlParser.Css.CssRuleSet"/>
        /// instances.
        /// This method returns a
        /// <see cref="System.Collections.IList{E}"/>
        /// because a selector can
        /// be compound, like "p, div, #navbar".
        /// </remarks>
        /// <param name="selectorStr">the selector</param>
        /// <param name="propertiesStr">the properties</param>
        /// <returns>
        /// the resulting list of
        /// <see cref="iText.StyledXmlParser.Css.CssRuleSet"/>
        /// instances
        /// </returns>
        public static IList<CssRuleSet> ParseRuleSet(String selectorStr, String propertiesStr) {
            IList<CssDeclaration> declarations = ParsePropertyDeclarations(propertiesStr);
            IList<CssRuleSet> ruleSets = new List<CssRuleSet>();
            //check for rules like p, {…}
            String[] selectors = iText.Commons.Utils.StringUtil.Split(selectorStr, ",");
            for (int i = 0; i < selectors.Length; i++) {
                selectors[i] = CssUtils.RemoveDoubleSpacesAndTrim(selectors[i]);
                if (selectors[i].Length == 0) {
                    return ruleSets;
                }
            }
            foreach (String currentSelectorStr in selectors) {
                try {
                    ruleSets.Add(new CssRuleSet(new CssSelector(currentSelectorStr), declarations));
                }
                catch (Exception exc) {
                    logger.LogError(exc, MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                        .ERROR_PARSING_CSS_SELECTOR, currentSelectorStr));
                    //if any separated selector has errors, all others become invalid.
                    //in this case we just clear map, it is the easies way to support this.
                    declarations.Clear();
                    return ruleSets;
                }
            }
            return ruleSets;
        }

        /// <summary>
        /// Splits CSS properties into an array of
        /// <see cref="System.String"/>
        /// values.
        /// </summary>
        /// <param name="property">the properties</param>
        /// <returns>the array of property values</returns>
        private static String[] SplitCssProperty(String property) {
            if (String.IsNullOrEmpty(property.Trim())) {
                return null;
            }
            String[] result = new String[2];
            int position = property.IndexOf(":", StringComparison.Ordinal);
            if (position < 0) {
                logger.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , property.Trim()));
                return null;
            }
            result[0] = property.JSubstring(0, position);
            result[1] = property.Substring(position + 1);
            return result;
        }

        /// <summary>Gets the semicolon position.</summary>
        /// <param name="propertiesStr">the properties</param>
        /// <param name="fromIndex">the from index</param>
        /// <returns>the semicolon position</returns>
        private static int GetSemicolonPosition(String propertiesStr, int fromIndex) {
            int semiColonPos = propertiesStr.IndexOf(";", fromIndex);
            int closedBracketPos = propertiesStr.IndexOf(")", semiColonPos + 1);
            int openedBracketPos = propertiesStr.IndexOf("(", fromIndex);
            if (semiColonPos != -1 && openedBracketPos < semiColonPos && closedBracketPos > 0) {
                int nextOpenedBracketPos = openedBracketPos;
                do {
                    openedBracketPos = nextOpenedBracketPos;
                    nextOpenedBracketPos = propertiesStr.IndexOf("(", openedBracketPos + 1);
                }
                while (nextOpenedBracketPos < closedBracketPos && nextOpenedBracketPos > 0);
            }
            if (semiColonPos != -1 && semiColonPos > openedBracketPos && semiColonPos < closedBracketPos) {
                return GetSemicolonPosition(propertiesStr, closedBracketPos + 1);
            }
            return semiColonPos;
        }
    }
}
