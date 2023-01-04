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
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <author>Jonathan Hedley</author>
    public abstract class TreeBuilder {
        protected internal iText.StyledXmlParser.Jsoup.Parser.Parser parser;

        internal CharacterReader reader;

        internal Tokeniser tokeniser;

        protected internal Document doc;

        // current doc we are building into
        protected internal List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack;

        // the stack of open elements
        protected internal String baseUri;

        // current base uri, for creating new elements
        protected internal Token currentToken;

        // currentToken is used only for error tracking.
        protected internal ParseSettings settings;

        private Token.StartTag start = new Token.StartTag();

        // start tag to process
        private Token.EndTag end = new Token.EndTag();

        internal abstract ParseSettings DefaultSettings();

        protected internal virtual void InitialiseParse(TextReader input, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            Validate.NotNull(input, "String input must not be null");
            Validate.NotNull(baseUri, "BaseURI must not be null");
            Validate.NotNull(parser);
            doc = new Document(baseUri);
            doc.Parser(parser);
            this.parser = parser;
            settings = parser.Settings();
            reader = new CharacterReader(input);
            currentToken = null;
            tokeniser = new Tokeniser(reader, parser.GetErrors());
            stack = new List<iText.StyledXmlParser.Jsoup.Nodes.Element>(32);
            this.baseUri = baseUri;
        }

        internal virtual Document Parse(TextReader input, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            InitialiseParse(input, baseUri, parser);
            RunParser();
            // tidy up - as the Parser and Treebuilder are retained in document for settings / fragments
            reader.Close();
            reader = null;
            tokeniser = null;
            stack = null;
            return doc;
        }

        /// <summary>Create a new copy of this TreeBuilder</summary>
        /// <returns>copy, ready for a new parse</returns>
        internal abstract TreeBuilder NewInstance();

        internal abstract IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser parser);

        protected internal virtual void RunParser() {
            Tokeniser tokeniser = this.tokeniser;
            iText.StyledXmlParser.Jsoup.Parser.TokenType eof = iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF;
            while (true) {
                Token token = tokeniser.Read();
                Process(token);
                token.Reset();
                if (token.type == eof) {
                    break;
                }
            }
        }

        protected internal abstract bool Process(Token token);

        protected internal virtual bool ProcessStartTag(String name) {
            Token.StartTag start = this.start;
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().Name(name));
            }
            return Process(((Token.Tag)start.Reset()).Name(name));
        }

        public virtual bool ProcessStartTag(String name, Attributes attrs) {
            Token.StartTag start = this.start;
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().NameAttr(name, attrs));
            }
            start.Reset();
            start.NameAttr(name, attrs);
            return Process(start);
        }

        protected internal virtual bool ProcessEndTag(String name) {
            if (currentToken == end) {
                // don't recycle an in-use token
                return Process(new Token.EndTag().Name(name));
            }
            return Process(((Token.Tag)end.Reset()).Name(name));
        }

        protected internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element CurrentElement() {
            int size = stack.Count;
            return size > 0 ? stack[size - 1] : null;
        }

        /// <summary>If the parser is tracking errors, add an error at the current position.</summary>
        /// <param name="msg">error message</param>
        protected internal virtual void Error(String msg) {
            ParseErrorList errors = parser.GetErrors();
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), msg));
            }
        }

        /// <summary>(An internal method, visible for Element.</summary>
        /// <remarks>
        /// (An internal method, visible for Element. For HTML parse, signals that script and style text should be treated as
        /// Data Nodes).
        /// </remarks>
        protected internal virtual bool IsContentForTagData(String normalName) {
            return false;
        }
    }
}
