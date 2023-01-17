/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
    /// <summary>A text node.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class TextNode : LeafNode {
        /// <summary>Create a new TextNode representing the supplied (unencoded) text).</summary>
        /// <param name="text">raw text</param>
        /// <seealso cref="CreateFromEncoded(System.String)"/>
        public TextNode(String text) {
            value = text;
        }

        public override String NodeName() {
            return "#text";
        }

        /// <summary>Get the text content of this text node.</summary>
        /// <returns>Unencoded, normalised text.</returns>
        /// <seealso cref="GetWholeText()"/>
        public virtual String Text() {
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.NormaliseWhitespace(GetWholeText());
        }

        /// <summary>Set the text content of this text node.</summary>
        /// <param name="text">unencoded text</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.TextNode Text(String text) {
            CoreValue(text);
            return this;
        }

        /// <summary>Get the (unencoded) text of this text node, including any newlines and spaces present in the original.
        ///     </summary>
        /// <returns>text</returns>
        public virtual String GetWholeText() {
            return CoreValue();
        }

        /// <summary>Test if this text node is blank -- that is, empty or only whitespace (including newlines).</summary>
        /// <returns>true if this document is empty or only whitespace, false if it contains any text content.</returns>
        public virtual bool IsBlank() {
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsBlank(CoreValue());
        }

        /// <summary>Split this text node into two nodes at the specified string offset.</summary>
        /// <remarks>
        /// Split this text node into two nodes at the specified string offset. After splitting, this node will contain the
        /// original text up to the offset, and will have a new text node sibling containing the text after the offset.
        /// </remarks>
        /// <param name="offset">string offset point to split node at.</param>
        /// <returns>the newly created text node containing the text after the offset.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.TextNode SplitText(int offset) {
            String text = CoreValue();
            Validate.IsTrue(offset >= 0, "Split offset must be not be negative");
            Validate.IsTrue(offset < text.Length, "Split offset must not be greater than current text length");
            String head = text.JSubstring(0, offset);
            String tail = text.Substring(offset);
            Text(head);
            iText.StyledXmlParser.Jsoup.Nodes.TextNode tailNode = new iText.StyledXmlParser.Jsoup.Nodes.TextNode(tail);
            if (Parent() != null) {
                Parent().AddChildren(SiblingIndex() + 1, tailNode);
            }
            return tailNode;
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            bool prettyPrint = @out.PrettyPrint();
            if (prettyPrint && ((SiblingIndex() == 0 && parentNode is iText.StyledXmlParser.Jsoup.Nodes.Element && ((iText.StyledXmlParser.Jsoup.Nodes.Element
                )parentNode).Tag().FormatAsBlock() && !IsBlank()) || (@out.Outline() && SiblingNodes().Count > 0 && !IsBlank
                ()))) {
                Indent(accum, depth, @out);
            }
            bool normaliseWhite = prettyPrint && !iText.StyledXmlParser.Jsoup.Nodes.Element.PreserveWhitespace(parentNode
                );
            bool stripWhite = prettyPrint && parentNode is Document;
            Entities.Escape(accum, CoreValue(), @out, false, normaliseWhite, stripWhite);
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.TextNode)base.Clone();
        }

        /// <summary>Create a new TextNode from HTML encoded (aka escaped) data.</summary>
        /// <param name="encodedText">Text containing encoded HTML (e.g. &amp;lt;)</param>
        /// <returns>TextNode containing unencoded data (e.g. &lt;)</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.TextNode CreateFromEncoded(String encodedText) {
            String text = Entities.Unescape(encodedText);
            return new iText.StyledXmlParser.Jsoup.Nodes.TextNode(text);
        }

        internal static String NormaliseWhitespace(String text) {
            text = iText.StyledXmlParser.Jsoup.Internal.StringUtil.NormaliseWhitespace(text);
            return text;
        }

        internal static String StripLeadingWhitespace(String text) {
            return text.ReplaceFirst("^\\s+", "");
        }

        internal static bool LastCharIsWhitespace(StringBuilder sb) {
            return sb.Length != 0 && sb[sb.Length - 1] == ' ';
        }
    }
}
