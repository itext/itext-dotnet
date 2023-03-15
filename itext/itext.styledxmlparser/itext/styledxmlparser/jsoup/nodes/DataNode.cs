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
using System.Text;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A data node, for contents of style, script tags etc, where contents should not show in text().</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class DataNode : LeafNode {
        /// <summary>Create a new DataNode.</summary>
        /// <param name="data">data contents</param>
        public DataNode(String data) {
            value = data;
        }

        public override String NodeName() {
            return "#data";
        }

        /// <summary>Get the data contents of this node.</summary>
        /// <remarks>Get the data contents of this node. Will be unescaped and with original new lines, space etc.</remarks>
        /// <returns>data</returns>
        public virtual String GetWholeData() {
            return CoreValue();
        }

        /// <summary>Set the data contents of this node.</summary>
        /// <param name="data">unencoded data</param>
        /// <returns>this node, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.DataNode SetWholeData(String data) {
            CoreValue(data);
            return this;
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append(GetWholeData());
        }

        // data is not escaped in return from data nodes, so " in script, style is plain
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.DataNode)base.Clone();
        }

        /// <summary>Create a new DataNode from HTML encoded data.</summary>
        /// <param name="encodedData">encoded data</param>
        /// <param name="baseUri">base URI</param>
        /// <returns>new DataNode</returns>
        [System.ObsoleteAttribute(@"Unused, and will be removed in 1.15.1.")]
        public static iText.StyledXmlParser.Jsoup.Nodes.DataNode CreateFromEncoded(String encodedData, String baseUri
            ) {
            String data = Entities.Unescape(encodedData);
            return new iText.StyledXmlParser.Jsoup.Nodes.DataNode(data);
        }
    }
}
