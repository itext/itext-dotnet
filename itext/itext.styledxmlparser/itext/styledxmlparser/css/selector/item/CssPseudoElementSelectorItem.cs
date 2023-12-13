/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Css.Pseudo;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    /// <summary>
    /// <see cref="ICssSelectorItem"/>
    /// implementation for pseudo element selectors.
    /// </summary>
    public class CssPseudoElementSelectorItem : ICssSelectorItem {
        /// <summary>The pseudo element name.</summary>
        private String pseudoElementName;

        /// <summary>
        /// Creates a new
        /// <see cref="CssPseudoElementSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="pseudoElementName">the pseudo element name</param>
        public CssPseudoElementSelectorItem(String pseudoElementName) {
            this.pseudoElementName = pseudoElementName;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#getSpecificity()
        */
        public virtual int GetSpecificity() {
            return CssSpecificityConstants.ELEMENT_SPECIFICITY;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.selector.item.ICssSelectorItem#matches(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual bool Matches(INode node) {
            return node is CssPseudoElementNode && ((CssPseudoElementNode)node).GetPseudoElementName().Equals(pseudoElementName
                );
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            return "::" + pseudoElementName;
        }
    }
}
