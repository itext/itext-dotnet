/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.IDocumentNode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Document"/>
    /// class.
    /// </summary>
    public class JsoupDocumentNode : JsoupElementNode, IDocumentNode {
        /// <summary>The JSoup document instance.</summary>
        private Document document;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupDocumentNode"/>
        /// instance.
        /// </summary>
        /// <param name="document">the document</param>
        public JsoupDocumentNode(Document document)
            : base(document) {
            this.document = document;
        }

        /// <summary>Gets the JSoup document.</summary>
        /// <returns>the document</returns>
        public virtual Document GetDocument() {
            return document;
        }
    }
}
