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
using System;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.IAttribute"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Attribute"/>
    /// class.
    /// </summary>
    public class JsoupAttribute : IAttribute {
        /// <summary>
        /// The JSoup
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Attribute"/>
        /// instance.
        /// </summary>
        private iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupAttribute"/>
        /// instance.
        /// </summary>
        /// <param name="attribute">the attribute</param>
        public JsoupAttribute(iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute) {
            this.attribute = attribute;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IAttribute#getKey()
        */
        public virtual String GetKey() {
            return attribute.Key;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IAttribute#getValue()
        */
        public virtual String GetValue() {
            return attribute.Value;
        }
    }
}
