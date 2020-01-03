/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>
    /// Use the
    /// <c>XmlTreeBuilder</c>
    /// when you want to parse XML without any of the HTML DOM rules being applied to the
    /// document.
    /// </summary>
    /// <remarks>
    /// Use the
    /// <c>XmlTreeBuilder</c>
    /// when you want to parse XML without any of the HTML DOM rules being applied to the
    /// document.
    /// <para />
    /// Usage example:
    /// <c>Document xmlDoc = Jsoup.parse(html, baseUrl, Parser.xmlParser());</c>
    /// </remarks>
    /// <author>Jonathan Hedley</author>
    public class XmlTreeBuilder : TreeBuilder {
        internal override void InitialiseParse(String input, String baseUri, ParseErrorList errors) {
            base.InitialiseParse(input, baseUri, errors);
            stack.Add(doc);
            // place the document onto the stack. differs from HtmlTreeBuilder (not on stack)
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
        }

        internal override bool Process(Token token) {
            // start tag, end tag, doctype, comment, character, eof
            switch (token.type) {
                case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                    Insert(token.AsStartTag());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                    PopStackToClose(token.AsEndTag());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment: {
                    Insert(token.AsComment());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                    Insert(token.AsCharacter());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype: {
                    Insert(token.AsDoctype());
                    break;
                }

                case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                    // could put some normalisation here if desired
                    break;
                }

                default: {
                    Validate.Fail("Unexpected token type: " + token.type);
                    break;
                }
            }
            return true;
        }

        private void InsertNode(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            CurrentElement().AppendChild(node);
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element Insert(Token.StartTag startTag) {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(startTag.Name(
                ));
            // todo: wonder if for xml parsing, should treat all tags as unknown? because it's not html.
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, baseUri, 
                startTag.attributes);
            InsertNode(el);
            if (startTag.IsSelfClosing()) {
                tokeniser.AcknowledgeSelfClosingFlag();
                if (!tag.IsKnownTag()) {
                    // unknown tag, remember this is self closing for output. see above.
                    tag.SetSelfClosing();
                }
            }
            else {
                stack.Add(el);
            }
            return el;
        }

        internal virtual void Insert(Token.Comment commentToken) {
            Comment comment = new Comment(commentToken.GetData(), baseUri);
            iText.StyledXmlParser.Jsoup.Nodes.Node insert = comment;
            if (commentToken.bogus) {
                // xml declarations are emitted as bogus comments (which is right for html, but not xml)
                // so we do a bit of a hack and parse the data as an element to pull the attributes out
                String data = comment.GetData();
                if (data.Length > 1 && (data.StartsWith("!") || data.StartsWith("?"))) {
                    Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<" + data.JSubstring(1, data.Length - 1) + ">", baseUri
                        , iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
                    iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Child(0);
                    insert = new XmlDeclaration(el.TagName(), comment.BaseUri(), data.StartsWith("!"));
                    insert.Attributes().AddAll(el.Attributes());
                }
            }
            InsertNode(insert);
        }

        internal virtual void Insert(Token.Character characterToken) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = new TextNode(characterToken.GetData(), baseUri);
            InsertNode(node);
        }

        internal virtual void Insert(Token.Doctype d) {
            DocumentType doctypeNode = new DocumentType(d.GetName(), d.GetPublicIdentifier(), d.GetSystemIdentifier(), 
                baseUri);
            InsertNode(doctypeNode);
        }

        /// <summary>If the stack contains an element with this tag's name, pop up the stack to remove the first occurrence.
        ///     </summary>
        /// <remarks>
        /// If the stack contains an element with this tag's name, pop up the stack to remove the first occurrence. If not
        /// found, skips.
        /// </remarks>
        /// <param name="endTag"/>
        private void PopStackToClose(Token.EndTag endTag) {
            String elName = endTag.Name();
            iText.StyledXmlParser.Jsoup.Nodes.Element firstFound = null;
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    firstFound = next;
                    break;
                }
            }
            if (firstFound == null) {
                return;
            }
            // not found, skip
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (next == firstFound) {
                    break;
                }
            }
        }

        internal virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, String 
            baseUri, ParseErrorList errors) {
            InitialiseParse(inputFragment, baseUri, errors);
            RunParser();
            return doc.ChildNodes();
        }
    }
}
