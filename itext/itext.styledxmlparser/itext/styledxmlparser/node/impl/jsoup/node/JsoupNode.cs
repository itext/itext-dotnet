/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
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

//\cond DO_NOT_DOCUMENT
        /// <summary>The parent node.</summary>
        internal INode parentNode;
//\endcond

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
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Node.Impl.Jsoup.Node.JsoupNode));
                logger.LogError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_ADDING_CHILD_NODE);
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
