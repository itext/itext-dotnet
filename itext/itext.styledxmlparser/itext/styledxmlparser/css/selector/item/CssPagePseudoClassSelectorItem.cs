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
using iText.StyledXmlParser.Css.Page;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    /// <summary>
    /// <see cref="ICssSelectorItem"/>
    /// implementation for page pseudo classes selectors.
    /// </summary>
    public class CssPagePseudoClassSelectorItem : ICssSelectorItem {
        /// <summary>Indicates if the page pseudo class is a spread pseudo class (left or right).</summary>
        private bool isSpreadPseudoClass;

        /// <summary>The page pseudo class.</summary>
        private String pagePseudoClass;

        /// <summary>
        /// Creates a new
        /// <see cref="CssPagePseudoClassSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="pagePseudoClass">the page pseudo class name</param>
        public CssPagePseudoClassSelectorItem(String pagePseudoClass) {
            this.isSpreadPseudoClass = pagePseudoClass.Equals(PageContextConstants.LEFT) || pagePseudoClass.Equals(PageContextConstants
                .RIGHT);
            this.pagePseudoClass = pagePseudoClass;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#getSpecificity()
        */
        public virtual int GetSpecificity() {
            return isSpreadPseudoClass ? CssSpecificityConstants.ELEMENT_SPECIFICITY : CssSpecificityConstants.CLASS_SPECIFICITY;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual bool Matches(INode node) {
            if (!(node is PageContextNode)) {
                return false;
            }
            return ((PageContextNode)node).GetPageClasses().Contains(pagePseudoClass);
        }
    }
}
