/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
