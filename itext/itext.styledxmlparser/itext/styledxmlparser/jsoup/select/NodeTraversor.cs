/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Depth-first node traversor.</summary>
    /// <remarks>
    /// Depth-first node traversor. Use to iterate through all nodes under and including the specified root node.
    /// <para />
    /// This implementation does not use recursion, so a deep DOM does not risk blowing the stack.
    /// </remarks>
    public class NodeTraversor {
        /// <summary>Start a depth-first traverse of the root and all of its descendants.</summary>
        /// <param name="visitor">Node visitor.</param>
        /// <param name="root">the root node point to traverse.</param>
        public static void Traverse(NodeVisitor visitor, iText.StyledXmlParser.Jsoup.Nodes.Node root) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = root;
            iText.StyledXmlParser.Jsoup.Nodes.Node parent;
            // remember parent to find nodes that get replaced in .head
            int depth = 0;
            while (node != null) {
                parent = node.ParentNode();
                visitor.Head(node, depth);
                // visit current node
                if (parent != null && !node.HasParent()) {
                    // must have been replaced; find replacement
                    node = parent.ChildNode(node.SiblingIndex());
                }
                // replace ditches parent but keeps sibling index
                if (node.ChildNodeSize() > 0) {
                    // descend
                    node = node.ChildNode(0);
                    depth++;
                }
                else {
                    while (true) {
                        System.Diagnostics.Debug.Assert(node != null);
                        // as depth > 0, will have parent
                        if (!(node.NextSibling() == null && depth > 0)) {
                            break;
                        }
                        visitor.Tail(node, depth);
                        // when no more siblings, ascend
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

        /// <summary>Start a depth-first traverse of all elements.</summary>
        /// <param name="visitor">Node visitor.</param>
        /// <param name="elements">Elements to filter.</param>
        public static void Traverse(NodeVisitor visitor, Elements elements) {
            Validate.NotNull(visitor);
            Validate.NotNull(elements);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in elements) {
                Traverse(visitor, el);
            }
        }

        /// <summary>Start a depth-first filtering of the root and all of its descendants.</summary>
        /// <param name="filter">Node visitor.</param>
        /// <param name="root">the root node point to traverse.</param>
        /// <returns>
        /// The filter result of the root node, or
        /// <see cref="FilterResult.STOP"/>.
        /// </returns>
        public static NodeFilter.FilterResult Filter(NodeFilter filter, iText.StyledXmlParser.Jsoup.Nodes.Node root
            ) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = root;
            int depth = 0;
            while (node != null) {
                NodeFilter.FilterResult result = filter.Head(node, depth);
                if (result == NodeFilter.FilterResult.STOP) {
                    return result;
                }
                // Descend into child nodes:
                if (result == NodeFilter.FilterResult.CONTINUE && node.ChildNodeSize() > 0) {
                    node = node.ChildNode(0);
                    ++depth;
                    continue;
                }
                // No siblings, move upwards:
                while (true) {
                    System.Diagnostics.Debug.Assert(node != null);
                    // depth > 0, so has parent
                    if (!(node.NextSibling() == null && depth > 0)) {
                        break;
                    }
                    // 'tail' current node:
                    if (result == NodeFilter.FilterResult.CONTINUE || result == NodeFilter.FilterResult.SKIP_CHILDREN) {
                        result = filter.Tail(node, depth);
                        if (result == NodeFilter.FilterResult.STOP) {
                            return result;
                        }
                    }
                    iText.StyledXmlParser.Jsoup.Nodes.Node prev = node;
                    // In case we need to remove it below.
                    node = node.ParentNode();
                    depth--;
                    if (result == NodeFilter.FilterResult.REMOVE) {
                        prev.Remove();
                    }
                    // Remove AFTER finding parent.
                    result = NodeFilter.FilterResult.CONTINUE;
                }
                // Parent was not pruned.
                // 'tail' current node, then proceed with siblings:
                if (result == NodeFilter.FilterResult.CONTINUE || result == NodeFilter.FilterResult.SKIP_CHILDREN) {
                    result = filter.Tail(node, depth);
                    if (result == NodeFilter.FilterResult.STOP) {
                        return result;
                    }
                }
                if (node == root) {
                    return result;
                }
                iText.StyledXmlParser.Jsoup.Nodes.Node prev_1 = node;
                // In case we need to remove it below.
                node = node.NextSibling();
                if (result == NodeFilter.FilterResult.REMOVE) {
                    prev_1.Remove();
                }
            }
            // Remove AFTER finding sibling.
            // root == null?
            return NodeFilter.FilterResult.CONTINUE;
        }

        /// <summary>Start a depth-first filtering of all elements.</summary>
        /// <param name="filter">Node filter.</param>
        /// <param name="elements">Elements to filter.</param>
        public static void Filter(NodeFilter filter, Elements elements) {
            Validate.NotNull(filter);
            Validate.NotNull(elements);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in elements) {
                if (Filter(filter, el) == NodeFilter.FilterResult.STOP) {
                    break;
                }
            }
        }
    }
}
