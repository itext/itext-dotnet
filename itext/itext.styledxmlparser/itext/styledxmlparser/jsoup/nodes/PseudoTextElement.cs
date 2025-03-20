/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
