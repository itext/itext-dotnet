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
namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Node filter interface.</summary>
    /// <remarks>
    /// Node filter interface. Provide an implementing class to
    /// <see cref="NodeTraversor"/>
    /// to iterate through nodes.
    /// <para />
    /// This interface provides two methods,
    /// <c>head</c>
    /// and
    /// <c>tail</c>
    /// . The head method is called when the node is first
    /// seen, and the tail method when all of the node's children have been visited. As an example, head can be used to
    /// create a start tag for a node, and tail to create the end tag.
    /// <para />
    /// For every node, the filter has to decide whether to
    /// <list type="bullet">
    /// <item><description>continue (
    /// <see cref="FilterResult.CONTINUE"/>
    /// ),
    /// </description></item>
    /// <item><description>skip all children (
    /// <see cref="FilterResult.SKIP_CHILDREN"/>
    /// ),
    /// </description></item>
    /// <item><description>skip node entirely (
    /// <see cref="FilterResult.SKIP_ENTIRELY"/>
    /// ),
    /// </description></item>
    /// <item><description>remove the subtree (
    /// <see cref="FilterResult.REMOVE"/>
    /// ),
    /// </description></item>
    /// <item><description>interrupt the iteration and return (
    /// <see cref="FilterResult.STOP"/>
    /// ).
    /// </description></item>
    /// </list>
    /// The difference between
    /// <see cref="FilterResult.SKIP_CHILDREN"/>
    /// and
    /// <see cref="FilterResult.SKIP_ENTIRELY"/>
    /// is that the first
    /// will invoke
    /// <see cref="Tail(iText.StyledXmlParser.Jsoup.Nodes.Node, int)"/>
    /// on the node, while the latter will not.
    /// Within
    /// <see cref="Tail(iText.StyledXmlParser.Jsoup.Nodes.Node, int)"/>
    /// , both are equivalent to
    /// <see cref="FilterResult.CONTINUE"/>.
    /// </remarks>
    public abstract class NodeFilter {
        /// <summary>Filter decision.</summary>
        public enum FilterResult {
            /// <summary>Continue processing the tree</summary>
            CONTINUE,
            /// <summary>
            /// Skip the child nodes, but do call
            /// <see cref="NodeFilter.Tail(iText.StyledXmlParser.Jsoup.Nodes.Node, int)"/>
            /// next.
            /// </summary>
            SKIP_CHILDREN,
            /// <summary>
            /// Skip the subtree, and do not call
            /// <see cref="NodeFilter.Tail(iText.StyledXmlParser.Jsoup.Nodes.Node, int)"/>.
            /// </summary>
            SKIP_ENTIRELY,
            /// <summary>Remove the node and its children</summary>
            REMOVE,
            /// <summary>Stop processing</summary>
            STOP
        }

        /// <summary>Callback for when a node is first visited.</summary>
        /// <param name="node">the node being visited.</param>
        /// <param name="depth">the depth of the node, relative to the root node. E.g., the root node has depth 0, and a child node of that will have depth 1.
        ///     </param>
        /// <returns>Filter decision</returns>
        public abstract NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth);

        /// <summary>Callback for when a node is last visited, after all of its descendants have been visited.</summary>
        /// <param name="node">the node being visited.</param>
        /// <param name="depth">the depth of the node, relative to the root node. E.g., the root node has depth 0, and a child node of that will have depth 1.
        ///     </param>
        /// <returns>Filter decision</returns>
        public abstract NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth);
    }

    public static class NodeFilterConstants {
    }
}
