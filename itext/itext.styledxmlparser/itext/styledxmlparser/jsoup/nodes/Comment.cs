/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Text;
using iText.StyledXmlParser.Jsoup.Parser;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A comment node.</summary>
    public class Comment : LeafNode {
        /// <summary>Create a new comment node.</summary>
        /// <param name="data">The contents of the comment</param>
        public Comment(String data) {
            value = data;
        }

        public override String NodeName() {
            return "#comment";
        }

        /// <summary>Get the contents of the comment.</summary>
        /// <returns>comment content</returns>
        public virtual String GetData() {
            return CoreValue();
        }

        public virtual iText.StyledXmlParser.Jsoup.Nodes.Comment SetData(String data) {
            CoreValue(data);
            return this;
        }

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            if (@out.PrettyPrint() && ((SiblingIndex() == 0 && parentNode is iText.StyledXmlParser.Jsoup.Nodes.Element
                 && ((iText.StyledXmlParser.Jsoup.Nodes.Element)parentNode).Tag().FormatAsBlock()) || (@out.Outline())
                )) {
                Indent(accum, depth, @out);
            }
            accum.Append("<!--").Append(GetData()).Append("-->");
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
            return (iText.StyledXmlParser.Jsoup.Nodes.Comment)base.Clone();
        }

        /// <summary>Check if this comment looks like an XML Declaration.</summary>
        /// <returns>true if it looks like, maybe, it's an XML Declaration.</returns>
        public virtual bool IsXmlDeclaration() {
            String data = GetData();
            return IsXmlDeclarationData(data);
        }

        private static bool IsXmlDeclarationData(String data) {
            return (data.Length > 1 && (data.StartsWith("!") || data.StartsWith("?")));
        }

        /// <summary>Attempt to cast this comment to an XML Declaration node.</summary>
        /// <returns>an XML declaration if it could be parsed as one, null otherwise.</returns>
        public virtual XmlDeclaration AsXmlDeclaration() {
            String data = GetData();
            XmlDeclaration decl = null;
            String declContent = data.JSubstring(1, data.Length - 1);
            // make sure this bogus comment is not immediately followed by another, treat as comment if so
            if (IsXmlDeclarationData(declContent)) {
                return null;
            }
            String fragment = "<" + declContent + ">";
            // use the HTML parser not XML, so we don't get into a recursive XML Declaration on contrived data
            Document doc = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser().Settings(ParseSettings.preserveCase)
                .ParseInput(fragment, BaseUri());
            if (doc.Body().Children().Count > 0) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Body().Child(0);
                decl = new XmlDeclaration(NodeUtils.Parser(doc).Settings().NormalizeTag(el.TagName()), data.StartsWith("!"
                    ));
                decl.Attributes().AddAll(el.Attributes());
            }
            return decl;
        }
    }
}
