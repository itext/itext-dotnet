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
