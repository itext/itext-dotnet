/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
