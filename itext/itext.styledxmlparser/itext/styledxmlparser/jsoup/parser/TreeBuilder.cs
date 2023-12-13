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
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <author>Jonathan Hedley</author>
    public abstract class TreeBuilder {
        internal CharacterReader reader;

        internal Tokeniser tokeniser;

        protected internal Document doc;

        // current doc we are building into
        protected internal List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack;

        // the stack of open elements
        protected internal String baseUri;

        // current base uri, for creating new elements
        internal Token currentToken;

        // currentToken is used only for error tracking.
        internal ParseErrorList errors;

        // null when not tracking errors
        private Token.StartTag start = new Token.StartTag();

        // start tag to process
        private Token.EndTag end = new Token.EndTag();

        internal virtual void InitialiseParse(String input, String baseUri, ParseErrorList errors) {
            Validate.NotNull(input, "String input must not be null");
            Validate.NotNull(baseUri, "BaseURI must not be null");
            doc = new Document(baseUri);
            reader = new CharacterReader(input);
            this.errors = errors;
            tokeniser = new Tokeniser(reader, errors);
            stack = new List<iText.StyledXmlParser.Jsoup.Nodes.Element>(32);
            this.baseUri = baseUri;
        }

        internal virtual Document Parse(String input, String baseUri) {
            return Parse(input, baseUri, ParseErrorList.NoTracking());
        }

        internal virtual Document Parse(String input, String baseUri, ParseErrorList errors) {
            InitialiseParse(input, baseUri, errors);
            RunParser();
            return doc;
        }

        protected internal virtual void RunParser() {
            while (true) {
                Token token = tokeniser.Read();
                Process(token);
                token.Reset();
                if (token.type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF) {
                    break;
                }
            }
        }

        internal abstract bool Process(Token token);

        protected internal virtual bool ProcessStartTag(String name) {
            if (currentToken == start) {
                // don't recycle an in-use token
                return Process(new Token.StartTag().Name(name));
            }
            return Process(((Token.Tag)start.Reset()).Name(name));
        }

        public virtual bool ProcessStartTag(String name, Attributes attrs) {
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
    }
}
