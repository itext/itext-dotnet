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
    /// <see cref="iText.StyledXmlParser.Node.IDataNode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.DataNode"/>
    /// class.
    /// </summary>
    public class JsoupDataNode : JsoupNode, IDataNode {
        /// <summary>The JSoup data node instance.</summary>
        private DataNode dataNode;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupDataNode"/>
        /// instance.
        /// </summary>
        /// <param name="dataNode">the data node</param>
        public JsoupDataNode(DataNode dataNode)
            : base(dataNode) {
            this.dataNode = dataNode;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IDataNode#getWholeData()
        */
        public virtual String GetWholeData() {
            return dataNode.GetWholeData();
        }
    }
}
