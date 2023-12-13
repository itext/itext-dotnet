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
    /// seen, and the tail method when all of the node's children have been visited. As an example, head can be used to
    /// create a start tag for a node, and tail to create the end tag.
    /// </remarks>
    public interface NodeVisitor {
        /// <summary>Callback for when a node is first visited.</summary>
        /// <param name="node">the node being visited.</param>
        /// <param name="depth">
        /// the depth of the node, relative to the root node. E.g., the root node has depth 0, and a child node
        /// of that will have depth 1.
        /// </param>
        void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth);

        /// <summary>Callback for when a node is last visited, after all of its descendants have been visited.</summary>
        /// <param name="node">the node being visited.</param>
        /// <param name="depth">
        /// the depth of the node, relative to the root node. E.g., the root node has depth 0, and a child node
        /// of that will have depth 1.
        /// </param>
        void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth);
    }
}
