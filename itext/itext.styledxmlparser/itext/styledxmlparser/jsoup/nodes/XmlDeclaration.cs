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
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>An XML Declaration.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class XmlDeclaration : iText.StyledXmlParser.Jsoup.Nodes.Node {
        private readonly String name;

        private readonly bool isProcessingInstruction;

        // <! if true, <? if false, declaration (and last data char should be ?)
        /// <summary>Create a new XML declaration</summary>
        /// <param name="name">of declaration</param>
        /// <param name="baseUri">base uri</param>
        /// <param name="isProcessingInstruction">is processing instruction</param>
        public XmlDeclaration(String name, String baseUri, bool isProcessingInstruction)
            : base(baseUri) {
            Validate.NotNull(name);
            this.name = name;
            this.isProcessingInstruction = isProcessingInstruction;
        }

        public override String NodeName() {
            return "#declaration";
        }

        /// <summary>Get the name of this declaration.</summary>
        /// <returns>name of this declaration.</returns>
        public virtual String Name() {
            return name;
        }

        /// <summary>Get the unencoded XML declaration.</summary>
        /// <returns>XML declaration</returns>
        public virtual String GetWholeDeclaration() {
            return attributes.Html().Trim();
        }

        // attr html starts with a " "
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append("<").Append(isProcessingInstruction ? "!" : "?").Append(name);
            attributes.Html(accum, @out);
            accum.Append(isProcessingInstruction ? "!" : "?").Append(">");
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }
    }
}
