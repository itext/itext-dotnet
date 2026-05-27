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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    /// <summary>Base class for pseudo-classes that accept a forgiving selector list (e.g. :is(), :where()).</summary>
    internal abstract class CssPseudoClassForgivingSelectorListSelectorItem : CssPseudoClassSelectorItem {
        protected internal readonly IList<ICssSelector> selectorList;

        protected internal CssPseudoClassForgivingSelectorListSelectorItem(String pseudoClass, IList<ICssSelector>
             selectorList, String argumentsString)
            : base(pseudoClass, argumentsString) {
            this.selectorList = selectorList;
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            foreach (ICssSelector sel in selectorList) {
                if (sel != null && sel.Matches(node)) {
                    return true;
                }
            }
            return false;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Parses a forgiving selector list for :is() / :where().</summary>
        /// <remarks>
        /// Parses a forgiving selector list for :is() / :where().
        /// <para />
        /// Per Selectors Level 4, :is() and :where() accept a forgiving selector list:
        /// invalid selectors are ignored rather than invalidating the whole pseudo-class.
        /// </remarks>
        /// <param name="arguments">selector list as written inside parentheses</param>
        /// <returns>list of valid selectors (possibly empty), or null if arguments are syntactically missing</returns>
        internal static IList<ICssSelector> ParseForgivingSelectorListWithoutPseudoElements(String arguments) {
            if (arguments == null || String.IsNullOrEmpty(arguments.Trim())) {
                // :is() / :where() with empty arguments is invalid.
                return null;
            }
            IList<String> parts = CssSelectorParser.SplitByTopLevelComma(arguments);
            if (parts.IsEmpty()) {
                return null;
            }
            IList<ICssSelector> selectors = new List<ICssSelector>();
            foreach (String rawPart in parts) {
                String part = rawPart == null ? "" : rawPart.Trim();
                if (String.IsNullOrEmpty(part)) {
                    // Empty entries like :is(.a,,.b) are invalid selectors in the list; ignore (forgiving).
                    continue;
                }
                try {
                    CssSelector sel = new CssSelector(CssSelectorParser.ParseSelectorItems(part, false));
                    if (!ContainsPseudoElement(JavaCollectionsUtil.SingletonList(sel))) {
                        selectors.Add(sel);
                    }
                }
                catch (ArgumentException) {
                }
            }
            // Invalid/unsupported selector in the list; ignore (forgiving).
            return selectors;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool ContainsPseudoElement(IList<ICssSelector> selectors) {
            foreach (ICssSelector sel in selectors) {
                if (sel is CssSelector) {
                    foreach (ICssSelectorItem item in ((CssSelector)sel).GetSelectorItems()) {
                        if (item is CssPseudoElementSelectorItem) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
//\endcond
    }
//\endcond
}
