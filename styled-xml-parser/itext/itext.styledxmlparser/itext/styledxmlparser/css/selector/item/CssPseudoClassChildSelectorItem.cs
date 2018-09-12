using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassChildSelectorItem : CssPseudoClassSelectorItem {
        /// <summary>
        /// Creates a new
        /// <see cref="CssPseudoClassSelectorItem"/>
        /// instance.
        /// </summary>
        /// <param name="pseudoClass">the pseudo class name</param>
        internal CssPseudoClassChildSelectorItem(String pseudoClass)
            : base(pseudoClass) {
        }

        internal CssPseudoClassChildSelectorItem(String pseudoClass, String arguments)
            : base(pseudoClass, arguments) {
        }

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
    }
}
