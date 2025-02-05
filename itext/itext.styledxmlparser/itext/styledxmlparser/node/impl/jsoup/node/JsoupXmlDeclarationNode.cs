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
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.IXmlDeclarationNode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.XmlDeclaration"/>
    /// class.
    /// </summary>
    public class JsoupXmlDeclarationNode : JsoupNode, IXmlDeclarationNode {
        private readonly IAttributes attributes;

        private readonly XmlDeclaration xmlDeclaration;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupXmlDeclarationNode"/>
        /// instance.
        /// </summary>
        /// <param name="xmlDeclaration">the xml declaration node</param>
        public JsoupXmlDeclarationNode(XmlDeclaration xmlDeclaration)
            : base(xmlDeclaration) {
            this.attributes = new JsoupAttributes(xmlDeclaration.Attributes());
            this.xmlDeclaration = xmlDeclaration;
        }

        public virtual IAttributes GetAttributes() {
            return attributes;
        }

        public virtual String GetAttribute(String key) {
            return attributes.GetAttribute(key);
        }

        public virtual String Name() {
            return xmlDeclaration.Name();
        }
    }
}
