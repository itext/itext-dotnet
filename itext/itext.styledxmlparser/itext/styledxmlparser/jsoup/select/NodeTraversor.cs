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
namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Depth-first node traversor.</summary>
    /// <remarks>
    /// Depth-first node traversor. Use to iterate through all nodes under and including the specified root node.
    /// <para />
    /// This implementation does not use recursion, so a deep DOM does not risk blowing the stack.
    /// </remarks>
    public class NodeTraversor {
        private NodeVisitor visitor;

        /// <summary>Create a new traversor.</summary>
        /// <param name="visitor">
        /// a class implementing the
        /// <see cref="NodeVisitor"/>
        /// interface, to be called when visiting each node.
        /// </param>
        public NodeTraversor(NodeVisitor visitor) {
            this.visitor = visitor;
        }

        /// <summary>Start a depth-first traverse of the root and all of its descendants.</summary>
        /// <param name="root">the root node point to traverse.</param>
        public virtual void Traverse(iText.StyledXmlParser.Jsoup.Nodes.Node root) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = root;
            int depth = 0;
            while (node != null) {
                visitor.Head(node, depth);
                if (node.ChildNodeSize() > 0) {
                    node = node.ChildNode(0);
                    depth++;
                }
                else {
                    while (node.NextSibling() == null && depth > 0) {
                        visitor.Tail(node, depth);
                        node = node.ParentNode();
                        depth--;
                    }
                    visitor.Tail(node, depth);
                    if (node == root) {
                        break;
                    }
                    node = node.NextSibling();
                }
            }
        }
    }
}
