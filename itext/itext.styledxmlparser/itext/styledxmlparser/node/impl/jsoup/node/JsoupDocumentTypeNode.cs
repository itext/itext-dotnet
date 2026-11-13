/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.IDocumentTypeNode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.DocumentType"/>
    /// class.
    /// </summary>
    public class JsoupDocumentTypeNode : JsoupNode, IDocumentTypeNode {
        /// <summary>
        /// Creates a new
        /// <see cref="JsoupDocumentTypeNode"/>
        /// instance.
        /// </summary>
        /// <param name="node">the node</param>
        public JsoupDocumentTypeNode(DocumentType node)
            : base(node) {
        }
    }
}
