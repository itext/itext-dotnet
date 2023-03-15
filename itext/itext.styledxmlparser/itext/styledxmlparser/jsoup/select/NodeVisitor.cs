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
    /// <summary>Node visitor interface.</summary>
    /// <remarks>
    /// Node visitor interface. Provide an implementing class to
    /// <see cref="NodeTraversor"/>
    /// to iterate through nodes.
    /// <para />
    /// This interface provides two methods,
    /// <c>head</c>
    /// and
    /// <c>tail</c>
    /// . The head method is called when the node is first
    /// seen, and the tail method when all of the node's children have been visited. As an example,
    /// <c>head</c>
    /// can be used to
    /// emit a start tag for a node, and
    /// <c>tail</c>
    /// to create the end tag.
    /// </remarks>
    public interface NodeVisitor {
        /// <summary>Callback for when a node is first visited.</summary>
        /// <remarks>
        /// Callback for when a node is first visited.
        /// <para />
        /// The node may be modified (e.g.
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Node.Attr(System.String)"/>
        /// or replaced
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Node.ReplaceWith(iText.StyledXmlParser.Jsoup.Nodes.Node)"/>
        /// ). If it's
        /// <c>instanceOf Element</c>
        /// , you may cast it to an
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Element"/>
        /// and access those methods.
        /// <para />
        /// Note that nodes may not be removed during traversal using this method; use
        /// <see cref="NodeTraversor.Filter(NodeFilter, iText.StyledXmlParser.Jsoup.Nodes.Node)"/>
        /// with a
        /// <see cref="FilterResult.REMOVE"/>
        /// return instead.
        /// </remarks>
        /// <param name="node">the node being visited.</param>
        /// <param name="depth">
        /// the depth of the node, relative to the root node. E.g., the root node has depth 0, and a child node
        /// of that will have depth 1.
        /// </param>
        void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth);

        /// <summary>Callback for when a node is last visited, after all of its descendants have been visited.</summary>
        /// <remarks>
        /// Callback for when a node is last visited, after all of its descendants have been visited.
        /// <para />
        /// Note that replacement with
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Node.ReplaceWith(iText.StyledXmlParser.Jsoup.Nodes.Node)"/>
        /// is not supported in
        /// <c>tail</c>.
        /// </remarks>
        /// <param name="node">the node being visited.</param>
        /// <param name="depth">
        /// the depth of the node, relative to the root node. E.g., the root node has depth 0, and a child node
        /// of that will have depth 1.
        /// </param>
        void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth);
    }
}
