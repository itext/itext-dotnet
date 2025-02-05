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
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector {
    /// <summary>
    /// <see cref="ICssSelector"/>
    /// implementation for CSS page margin box selectors.
    /// </summary>
    public class CssPageMarginBoxSelector : ICssSelector {
        /// <summary>The page margin box name.</summary>
        private String pageMarginBoxName;

        /// <summary>The page selector.</summary>
        private ICssSelector pageSelector;

        /// <summary>
        /// Creates a new
        /// <see cref="CssPageMarginBoxSelector"/>
        /// instance.
        /// </summary>
        /// <param name="pageMarginBoxName">the page margin box name</param>
        /// <param name="pageSelector">the page selector</param>
        public CssPageMarginBoxSelector(String pageMarginBoxName, ICssSelector pageSelector) {
            this.pageMarginBoxName = pageMarginBoxName;
            this.pageSelector = pageSelector;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.ICssSelector#calculateSpecificity()
        */
        public virtual int CalculateSpecificity() {
            return pageSelector.CalculateSpecificity();
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.ICssSelector#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual bool Matches(INode node) {
            if (!(node is PageMarginBoxContextNode)) {
                return false;
            }
            PageMarginBoxContextNode marginBoxNode = (PageMarginBoxContextNode)node;
            if (pageMarginBoxName.Equals(marginBoxNode.GetMarginBoxName())) {
                INode parent = node.ParentNode();
                return pageSelector.Matches(parent);
            }
            return false;
        }
    }
}
