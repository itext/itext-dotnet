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
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>An XML Declaration.</summary>
    public class XmlDeclaration : LeafNode {
        private readonly bool isProcessingInstruction;

        // <! if true, <? if false, declaration (and last data char should be ?)
        /// <summary>Create a new XML declaration</summary>
        /// <param name="name">of declaration</param>
        /// <param name="isProcessingInstruction">is processing instruction</param>
        public XmlDeclaration(String name, bool isProcessingInstruction) {
            Validate.NotNull(name);
            value = name;
            this.isProcessingInstruction = isProcessingInstruction;
        }

        public override String NodeName() {
            return "#declaration";
        }

        /// <summary>Get the name of this declaration.</summary>
        /// <returns>name of this declaration.</returns>
        public virtual String Name() {
            return CoreValue();
        }

        /// <summary>Get the unencoded XML declaration.</summary>
        /// <returns>XML declaration</returns>
        public virtual String GetWholeDeclaration() {
            StringBuilder sb = iText.StyledXmlParser.Jsoup.Internal.StringUtil.BorrowBuilder();
            try {
                GetWholeDeclaration(sb, new OutputSettings());
            }
            catch (System.IO.IOException e) {
                throw new SerializationException(e);
            }
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.ReleaseBuilder(sb).Trim();
        }

        private void GetWholeDeclaration(StringBuilder accum, OutputSettings @out) {
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in Attributes()) {
                if (!attribute.Key.Equals(NodeName())) {
                    // skips coreValue (name)
                    accum.Append(' ');
                    attribute.Html(accum, @out);
                }
            }
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append("<").Append(isProcessingInstruction ? "!" : "?").Append(CoreValue());
            GetWholeDeclaration(accum, @out);
            accum.Append(isProcessingInstruction ? "!" : "?").Append(">");
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.XmlDeclaration)base.Clone();
        }
    }
}
