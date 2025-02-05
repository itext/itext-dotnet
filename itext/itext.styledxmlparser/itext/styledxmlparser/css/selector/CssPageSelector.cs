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
using iText.StyledXmlParser.Css.Page;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector {
    /// <summary>
    /// <see cref="ICssSelector"/>
    /// implementation for CSS page selectors.
    /// </summary>
    public class CssPageSelector : AbstractCssSelector {
        /// <summary>
        /// Creates a new
        /// <see cref="CssPageSelector"/>
        /// instance.
        /// </summary>
        /// <param name="pageSelectorStr">the page selector</param>
        public CssPageSelector(String pageSelectorStr)
            : base(CssPageSelectorParser.ParseSelectorItems(pageSelectorStr)) {
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.ICssSelector#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public override bool Matches(INode node) {
            if (!(node is PageContextNode)) {
                return false;
            }
            foreach (ICssSelectorItem selectorItem in selectorItems) {
                if (!selectorItem.Matches(node)) {
                    return false;
                }
            }
            return true;
        }
    }
}
