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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Selector.Item;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Utilities class to parse CSS page selectors.</summary>
    public sealed class CssPageSelectorParser {
        /// <summary>The pattern string for page selectors.</summary>
        private const String PAGE_SELECTOR_PATTERN_STR = "(^-?[_a-zA-Z][\\w-]*)|(:(?i)(left|right|first|blank))";

        /// <summary>The pattern for page selectors.</summary>
        private static readonly Regex selectorPattern = iText.Commons.Utils.StringUtil.RegexCompile(PAGE_SELECTOR_PATTERN_STR
            );

        /// <summary>
        /// Parses the selector items into a list of
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
        /// instances.
        /// </summary>
        /// <param name="selectorItemsStr">
        /// the selector items in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the resulting list of
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
        /// instances
        /// </returns>
        public static IList<ICssSelectorItem> ParseSelectorItems(String selectorItemsStr) {
            IList<ICssSelectorItem> selectorItems = new List<ICssSelectorItem>();
            Matcher itemMatcher = iText.Commons.Utils.Matcher.Match(selectorPattern, selectorItemsStr);
            while (itemMatcher.Find()) {
                String selectorItem = itemMatcher.Group(0);
                if (selectorItem[0] == ':') {
                    selectorItems.Add(new CssPagePseudoClassSelectorItem(selectorItem.Substring(1).ToLowerInvariant()));
                }
                else {
                    selectorItems.Add(new CssPageTypeSelectorItem(selectorItem));
                }
            }
            return selectorItems;
        }
    }
}
