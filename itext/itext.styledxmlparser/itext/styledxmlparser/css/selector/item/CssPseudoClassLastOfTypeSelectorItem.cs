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
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassLastOfTypeSelectorItem : CssPseudoClassChildSelectorItem {
        private static readonly iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassLastOfTypeSelectorItem instance
             = new iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassLastOfTypeSelectorItem();

        private CssPseudoClassLastOfTypeSelectorItem()
            : base(CommonCssConstants.LAST_OF_TYPE) {
        }

        public static iText.StyledXmlParser.Css.Selector.Item.CssPseudoClassLastOfTypeSelectorItem GetInstance() {
            return instance;
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            IList<INode> children = GetAllSiblingsOfNodeType(node);
            return !children.IsEmpty() && node.Equals(children[children.Count - 1]);
        }
    }
}
