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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css {
    /// <summary>The CSS context node.</summary>
    public abstract class CssContextNode : INode, IStylesContainer {
        /// <summary>The child nodes.</summary>
        private IList<INode> childNodes = new List<INode>();

        /// <summary>The parent node.</summary>
        private INode parentNode;

        /// <summary>The styles.</summary>
        private IDictionary<String, String> styles;

        /// <summary>
        /// Creates a new
        /// <see cref="CssContextNode"/>
        /// instance.
        /// </summary>
        /// <param name="parentNode">the parent node</param>
        public CssContextNode(INode parentNode) {
            this.parentNode = parentNode;
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
            childNodes.Add(node);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.INode#parentNode()
        */
        public virtual INode ParentNode() {
            return parentNode;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IStylesContainer#setStyles(java.util.Map)
        */
        public virtual void SetStyles(IDictionary<String, String> stringStringMap) {
            this.styles = stringStringMap;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IStylesContainer#getStyles()
        */
        public virtual IDictionary<String, String> GetStyles() {
            return this.styles;
        }
    }
}
