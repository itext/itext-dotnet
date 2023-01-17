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
