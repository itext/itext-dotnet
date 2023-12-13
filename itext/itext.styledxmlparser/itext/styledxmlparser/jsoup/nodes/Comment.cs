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
    /// <summary>A comment node.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Comment : iText.StyledXmlParser.Jsoup.Nodes.Node {
        private const String COMMENT_KEY = "comment";

        /// <summary>Create a new comment node.</summary>
        /// <param name="data">The contents of the comment</param>
        /// <param name="baseUri">base URI</param>
        public Comment(String data, String baseUri)
            : base(baseUri) {
            attributes.Put(COMMENT_KEY, data);
        }

        public override String NodeName() {
            return "#comment";
        }

        /// <summary>Get the contents of the comment.</summary>
        /// <returns>comment content</returns>
        public virtual String GetData() {
            return attributes.Get(COMMENT_KEY);
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            if (@out.PrettyPrint()) {
                Indent(accum, depth, @out);
            }
            accum.Append("<!--").Append(GetData()).Append("-->");
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }
    }
}
