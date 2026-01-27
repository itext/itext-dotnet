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
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Utility class providing methods for traversing and filtering nodes
    /// in a tree structure, particularly focusing on element nodes based
    /// on various criteria.
    /// </summary>
    /// <remarks>
    /// Utility class providing methods for traversing and filtering nodes
    /// in a tree structure, particularly focusing on element nodes based
    /// on various criteria. This class serves as a helper for operations
    /// related to CSS selector processing.
    /// </remarks>
    internal sealed class CssSelectorItemTraversalUtil {
        private CssSelectorItemTraversalUtil() {
        }

//\cond DO_NOT_DOCUMENT
        // utility class
        internal static bool IsValidElementNode(INode node) {
            return node is IElementNode && !(node is ICustomElementNode) && !(node is IDocumentNode);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static IList<INode> GetElementSiblings(INode node) {
            INode parent = node != null ? node.ParentNode() : null;
            return parent != null ? GetElementChildren(parent) : JavaCollectionsUtil.EmptyList<INode>();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static IList<INode> GetElementSiblingsOfSameType(INode node) {
            if (!(node is IElementNode)) {
                return JavaCollectionsUtil.EmptyList<INode>();
            }
            INode parent = node.ParentNode();
            if (parent == null) {
                return JavaCollectionsUtil.EmptyList<INode>();
            }
            IList<INode> children = parent.ChildNodes();
            if (children == null || children.IsEmpty()) {
                return JavaCollectionsUtil.EmptyList<INode>();
            }
            IList<INode> result = new List<INode>(children.Count);
            String name = ((IElementNode)node).Name();
            foreach (INode child in children) {
                if (child is IElementNode && name.Equals(((IElementNode)child).Name())) {
                    result.Add(child);
                }
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static INode GetNextElementSibling(INode node) {
            INode parent = node != null ? node.ParentNode() : null;
            if (parent == null) {
                return null;
            }
            bool afterNode = false;
            foreach (INode sibling in parent.ChildNodes()) {
                if (!afterNode) {
                    afterNode = (sibling == node);
                    continue;
                }
                if (sibling is IElementNode) {
                    return sibling;
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void ForEachFollowingElementSibling(INode node, Action<INode> action) {
            INode parent = node != null ? node.ParentNode() : null;
            if (parent == null) {
                return;
            }
            bool afterNode = false;
            foreach (INode sibling in parent.ChildNodes()) {
                if (!afterNode) {
                    afterNode = (sibling == node);
                    continue;
                }
                if (sibling is IElementNode) {
                    action(sibling);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static bool AnyDescendantElementMatches(INode scope, Predicate<INode> predicate) {
            if (scope == null) {
                return false;
            }
            Stack<INode> stack = new Stack<INode>();
            foreach (INode child in scope.ChildNodes()) {
                if (child is IElementNode) {
                    stack.Push(child);
                }
            }
            while (!stack.IsEmpty()) {
                INode candidate = stack.Pop();
                if (predicate(candidate)) {
                    return true;
                }
                foreach (INode child in candidate.ChildNodes()) {
                    if (child is IElementNode) {
                        stack.Push(child);
                    }
                }
            }
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void ForEachDescendantElement(INode scope, Action<INode> action) {
            if (scope == null) {
                return;
            }
            Stack<INode> stack = new Stack<INode>();
            foreach (INode child in scope.ChildNodes()) {
                if (child is IElementNode) {
                    stack.Push(child);
                }
            }
            while (!stack.IsEmpty()) {
                INode candidate = stack.Pop();
                action(candidate);
                foreach (INode child in candidate.ChildNodes()) {
                    if (child is IElementNode) {
                        stack.Push(child);
                    }
                }
            }
        }
//\endcond

        private static IList<INode> GetElementChildren(INode parent) {
            if (parent == null) {
                return JavaCollectionsUtil.EmptyList<INode>();
            }
            IList<INode> children = parent.ChildNodes();
            if (children == null || children.IsEmpty()) {
                return JavaCollectionsUtil.EmptyList<INode>();
            }
            IList<INode> result = new List<INode>(children.Count);
            foreach (INode child in children) {
                if (child is IElementNode) {
                    result.Add(child);
                }
            }
            return result;
        }
    }
//\endcond
}
