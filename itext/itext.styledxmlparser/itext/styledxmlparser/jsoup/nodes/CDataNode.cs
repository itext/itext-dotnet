/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append("<![CDATA[").Append(GetWholeText());
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
            try {
                accum.Append("]]>");
            }
            catch (System.IO.IOException e) {
                throw new UncheckedIOException(e);
            }
        }
//\endcond

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.CDataNode)base.Clone();
        }
    }
}
