/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
