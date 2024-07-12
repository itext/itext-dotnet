/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    /// <summary>
    /// Represents a
    /// <see cref="TextNode"/>
    /// as an
    /// <see cref="Element"/>
    /// , to enable text nodes to be selected with
    /// the
    /// <see cref="iText.StyledXmlParser.Jsoup.Select.Selector"/>
    /// 
    /// <c>:matchText</c>
    /// syntax.
    /// </summary>
    public class PseudoTextElement : iText.StyledXmlParser.Jsoup.Nodes.Element {
        public PseudoTextElement(iText.StyledXmlParser.Jsoup.Parser.Tag tag, String baseUri, Attributes attributes
            )
            : base(tag, baseUri, attributes) {
        }

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }
//\endcond
    }
}
