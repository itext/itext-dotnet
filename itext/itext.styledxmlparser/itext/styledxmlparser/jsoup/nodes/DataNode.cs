/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Text;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A data node, for contents of style, script tags etc, where contents should not show in text().</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class DataNode : iText.StyledXmlParser.Jsoup.Nodes.Node {
        private const String DATA_KEY = "data";

        /// <summary>Create a new DataNode.</summary>
        /// <param name="data">data contents</param>
        /// <param name="baseUri">base URI</param>
        public DataNode(String data, String baseUri)
            : base(baseUri) {
            attributes.Put(DATA_KEY, data);
        }

        public override String NodeName() {
            return "#data";
        }

        /// <summary>Get the data contents of this node.</summary>
        /// <remarks>Get the data contents of this node. Will be unescaped and with original new lines, space etc.</remarks>
        /// <returns>data</returns>
        public virtual String GetWholeData() {
            return attributes.Get(DATA_KEY);
        }

        /// <summary>Set the data contents of this node.</summary>
        /// <param name="data">unencoded data</param>
        /// <returns>this node, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.DataNode SetWholeData(String data) {
            attributes.Put(DATA_KEY, data);
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

        /// <summary>Create a new DataNode from HTML encoded data.</summary>
        /// <param name="encodedData">encoded data</param>
        /// <param name="baseUri">bass URI</param>
        /// <returns>new DataNode</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.DataNode CreateFromEncoded(String encodedData, String baseUri
            ) {
            String data = Entities.Unescape(encodedData);
            return new iText.StyledXmlParser.Jsoup.Nodes.DataNode(data, baseUri);
        }
    }
}
