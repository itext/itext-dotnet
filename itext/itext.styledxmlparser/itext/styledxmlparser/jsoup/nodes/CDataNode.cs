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
using iText.StyledXmlParser.Jsoup;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A Character Data node, to support CDATA sections.</summary>
    public class CDataNode : TextNode {
        public CDataNode(String text)
            : base(text) {
        }

        public override String NodeName() {
            return "#cdata";
        }

        /// <summary>Get the unencoded, <b>non-normalized</b> text content of this CDataNode.</summary>
        /// <returns>unencoded, non-normalized text</returns>
        public override String Text() {
            return GetWholeText();
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append("<![CDATA[").Append(GetWholeText());
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
            try {
                accum.Append("]]>");
            }
            catch (System.IO.IOException e) {
                throw new UncheckedIOException(e);
            }
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.CDataNode)base.Clone();
        }
    }
}
