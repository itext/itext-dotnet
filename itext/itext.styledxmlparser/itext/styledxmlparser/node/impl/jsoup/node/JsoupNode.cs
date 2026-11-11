/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.INode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Node"/>
    /// class.
    /// </summary>
    public class JsoupNode : INode {
        /// <summary>The JSoup node instance.</summary>
        private iText.StyledXmlParser.Jsoup.Nodes.Node node;

        /// <summary>The child nodes.</summary>
        private IList<INode> childNodes = new List<INode>();

        /// <summary>The parent node.</summary>
        internal INode parentNode;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupNode"/>
        /// instance.
        /// </summary>
        /// <param name="node">the node</param>
        public JsoupNode(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            this.node = node;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.INode#childNodes()
        */
        public virtual IList<INode> ChildNodes() {
            return JavaCollectionsUtil.UnmodifiableList(childNodes);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.INode#addChild(com.itextpdf.styledxmlparser.html.node.INode)
        */
        public virtual void AddChild(INode node) {
            if (node is iText.StyledXmlParser.Node.Impl.Jsoup.Node.JsoupNode) {
                childNodes.Add(node);
                ((iText.StyledXmlParser.Node.Impl.Jsoup.Node.JsoupNode)node).parentNode = this;
            }
            else {
                ILog logger = LogManager.GetLogger(typeof(iText.StyledXmlParser.Node.Impl.Jsoup.Node.JsoupNode));
                logger.Error(iText.StyledXmlParser.LogMessageConstant.ERROR_ADDING_CHILD_NODE);
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.INode#parentNode()
        */
        public virtual INode ParentNode() {
            return parentNode;
        }
    }
}
