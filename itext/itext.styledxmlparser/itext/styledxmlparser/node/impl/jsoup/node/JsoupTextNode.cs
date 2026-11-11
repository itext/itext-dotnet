/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.ITextNode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.TextNode"/>
    /// class.
    /// </summary>
    public class JsoupTextNode : JsoupNode, ITextNode {
        /// <summary>The text node.</summary>
        private TextNode textNode;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupTextNode"/>
        /// instance.
        /// </summary>
        /// <param name="textNode">the text node</param>
        public JsoupTextNode(TextNode textNode)
            : base(textNode) {
            this.textNode = textNode;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.ITextNode#wholeText()
        */
        public virtual String WholeText() {
            return textNode.GetWholeText();
        }
    }
}
