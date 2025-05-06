/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append("<").Append(isProcessingInstruction ? "!" : "?").Append(CoreValue());
            GetWholeDeclaration(accum, @out);
            accum.Append(isProcessingInstruction ? "!" : "?").Append(">");
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }
//\endcond

        public override String ToString() {
            return OuterHtml();
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.XmlDeclaration)base.Clone();
        }
    }
}
