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
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    internal class CssPseudoClassChildSelectorItem : CssPseudoClassSelectorItem {
//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="CssPseudoClassSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="pseudoClass">the pseudo class name</param>
        internal CssPseudoClassChildSelectorItem(String pseudoClass)
            : base(pseudoClass) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassChildSelectorItem(String pseudoClass, String arguments)
            : base(pseudoClass, arguments) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the all the siblings of a child node.</summary>
        /// <param name="node">the child node</param>
        /// <returns>the sibling nodes</returns>
        internal virtual IList<INode> GetAllSiblings(INode node) {
            INode parentElement = node.ParentNode();
            if (parentElement != null) {
                IList<INode> childrenUnmodifiable = parentElement.ChildNodes();
                IList<INode> children = new List<INode>(childrenUnmodifiable.Count);
                foreach (INode iNode in childrenUnmodifiable) {
                    if (iNode is IElementNode) {
                        children.Add(iNode);
                    }
                }
                return children;
            }
            return JavaCollectionsUtil.EmptyList<INode>();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets all siblings of a child node with the type of a child node.</summary>
        /// <param name="node">the child node</param>
        /// <returns>the sibling nodes with the type of a child node</returns>
        internal virtual IList<INode> GetAllSiblingsOfNodeType(INode node) {
            INode parentElement = node.ParentNode();
            if (parentElement != null) {
                IList<INode> childrenUnmodifiable = parentElement.ChildNodes();
                IList<INode> children = new List<INode>(childrenUnmodifiable.Count);
                foreach (INode iNode in childrenUnmodifiable) {
                    if (iNode is IElementNode && ((IElementNode)iNode).Name().Equals(((IElementNode)node).Name())) {
                        children.Add(iNode);
                    }
                }
                return children;
            }
            return JavaCollectionsUtil.EmptyList<INode>();
        }
//\endcond
    }
//\endcond
}
