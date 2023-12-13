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
using System.Text;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Parse tokens for the Tokeniser.</summary>
    internal abstract class Token {
        internal iText.StyledXmlParser.Jsoup.Parser.TokenType type;

        private Token() {
        }

        internal virtual String TokenType() {
            return this.GetType().Name;
        }

        /// <summary>Reset the data represent by this token, for reuse.</summary>
        /// <remarks>
        /// Reset the data represent by this token, for reuse. Prevents the need to create transfer objects for every
        /// piece of data, which immediately get GCed.
        /// </remarks>
        internal abstract iText.StyledXmlParser.Jsoup.Parser.Token Reset();

        internal static void Reset(StringBuilder sb) {
            if (sb != null) {
                sb.Delete(0, sb.Length);
            }
        }

        internal sealed class Doctype : Token {
            internal readonly StringBuilder name = new StringBuilder();

            internal readonly StringBuilder publicIdentifier = new StringBuilder();

            internal readonly StringBuilder systemIdentifier = new StringBuilder();

            internal bool forceQuirks = false;

            internal Doctype() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype;
            }

            internal override Token Reset() {
                Reset(name);
                Reset(publicIdentifier);
                Reset(systemIdentifier);
                forceQuirks = false;
                return this;
            }

            internal String GetName() {
                return name.ToString();
            }

            internal String GetPublicIdentifier() {
                return publicIdentifier.ToString();
            }

            public String GetSystemIdentifier() {
                return systemIdentifier.ToString();
            }

            public bool IsForceQuirks() {
                return forceQuirks;
            }
        }

        internal abstract class Tag : Token {
            protected internal String tagName;

            private String pendingAttributeName;

            // attribute names are generally caught in one hop, not accumulated
            private StringBuilder pendingAttributeValue = new StringBuilder();

            // but values are accumulated, from e.g. & in hrefs
            private String pendingAttributeValueS;

            // try to get attr vals in one shot, vs Builder
            private bool hasEmptyAttributeValue = false;

            // distinguish boolean attribute from empty string value
            private bool hasPendingAttributeValue = false;

            internal bool selfClosing = false;

            internal Attributes attributes;

            // start tags get attributes on construction. End tags get attributes on first new attribute (but only for parser convenience, not used).
            internal override Token Reset() {
                tagName = null;
                pendingAttributeName = null;
                Reset(pendingAttributeValue);
                pendingAttributeValueS = null;
                hasEmptyAttributeValue = false;
                hasPendingAttributeValue = false;
                selfClosing = false;
                attributes = null;
                return this;
            }

            internal void NewAttribute() {
                if (attributes == null) {
                    attributes = new Attributes();
                }
                if (pendingAttributeName != null) {
                    iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute;
                    if (hasPendingAttributeValue) {
                        attribute = new iText.StyledXmlParser.Jsoup.Nodes.Attribute(pendingAttributeName, pendingAttributeValue.Length
                             > 0 ? pendingAttributeValue.ToString() : pendingAttributeValueS);
                    }
                    else {
                        if (hasEmptyAttributeValue) {
                            attribute = new iText.StyledXmlParser.Jsoup.Nodes.Attribute(pendingAttributeName, "");
                        }
                        else {
                            attribute = new BooleanAttribute(pendingAttributeName);
                        }
                    }
                    attributes.Put(attribute);
                }
                pendingAttributeName = null;
                hasEmptyAttributeValue = false;
                hasPendingAttributeValue = false;
                Reset(pendingAttributeValue);
                pendingAttributeValueS = null;
            }

            internal void FinaliseTag() {
                // finalises for emit
                if (pendingAttributeName != null) {
                    // todo: check if attribute name exists; if so, drop and error
                    NewAttribute();
                }
            }

            internal String Name() {
                Validate.IsFalse(tagName == null || tagName.Length == 0);
                return tagName;
            }

            internal Token.Tag Name(String name) {
                tagName = name;
                return this;
            }

            internal bool IsSelfClosing() {
                return selfClosing;
            }

            internal Attributes GetAttributes() {
                return attributes;
            }

            // these appenders are rarely hit in not null state-- caused by null chars.
            internal void AppendTagName(String append) {
                tagName = tagName == null ? append : tagName + append;
            }

            internal void AppendTagName(char append) {
                AppendTagName(append.ToString());
            }

            internal void AppendAttributeName(String append) {
                pendingAttributeName = pendingAttributeName == null ? append : pendingAttributeName + append;
            }

            internal void AppendAttributeName(char append) {
                AppendAttributeName(append.ToString());
            }

            internal void AppendAttributeValue(String append) {
                EnsureAttributeValue();
                if (pendingAttributeValue.Length == 0) {
                    pendingAttributeValueS = append;
                }
                else {
                    pendingAttributeValue.Append(append);
                }
            }

            internal void AppendAttributeValue(char append) {
                EnsureAttributeValue();
                pendingAttributeValue.Append(append);
            }

            internal void AppendAttributeValue(char[] append) {
                EnsureAttributeValue();
                pendingAttributeValue.Append(append);
            }

            internal void SetEmptyAttributeValue() {
                hasEmptyAttributeValue = true;
            }

            private void EnsureAttributeValue() {
                hasPendingAttributeValue = true;
                // if on second hit, we'll need to move to the builder
                if (pendingAttributeValueS != null) {
                    pendingAttributeValue.Append(pendingAttributeValueS);
                    pendingAttributeValueS = null;
                }
            }
        }

        internal sealed class StartTag : Token.Tag {
            internal StartTag()
                : base() {
                attributes = new Attributes();
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag;
            }

            internal override Token Reset() {
                base.Reset();
                attributes = new Attributes();
                // todo - would prefer these to be null, but need to check Element assertions
                return this;
            }

            internal Token.StartTag NameAttr(String name, Attributes attributes) {
                this.tagName = name;
                this.attributes = attributes;
                return this;
            }

            public override String ToString() {
                if (attributes != null && attributes.Size() > 0) {
                    return "<" + Name() + " " + attributes.ToString() + ">";
                }
                else {
                    return "<" + Name() + ">";
                }
            }
        }

        internal sealed class EndTag : Token.Tag {
            internal EndTag()
                : base() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag;
            }

            public override String ToString() {
                return "</" + Name() + ">";
            }
        }

        internal sealed class Comment : Token {
            internal readonly StringBuilder data = new StringBuilder();

            internal bool bogus = false;

            internal override Token Reset() {
                Reset(data);
                bogus = false;
                return this;
            }

            internal Comment() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment;
            }

            internal String GetData() {
                return data.ToString();
            }

            public override String ToString() {
                return "<!--" + GetData() + "-->";
            }
        }

        internal sealed class Character : Token {
            private String data;

            internal Character()
                : base() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.Character;
            }

            internal override Token Reset() {
                data = null;
                return this;
            }

            internal Token.Character Data(String data) {
                this.data = data;
                return this;
            }

            internal String GetData() {
                return data;
            }

            public override String ToString() {
                return GetData();
            }
        }

        internal sealed class EOF : Token {
            internal EOF() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF;
            }

            internal override Token Reset() {
                return this;
            }
        }

        internal bool IsDoctype() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype;
        }

        internal Token.Doctype AsDoctype() {
            return (Token.Doctype)this;
        }

        internal bool IsStartTag() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag;
        }

        internal Token.StartTag AsStartTag() {
            return (Token.StartTag)this;
        }

        internal bool IsEndTag() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag;
        }

        internal Token.EndTag AsEndTag() {
            return (Token.EndTag)this;
        }

        internal bool IsComment() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment;
        }

        internal Token.Comment AsComment() {
            return (Token.Comment)this;
        }

        internal bool IsCharacter() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Character;
        }

        internal Token.Character AsCharacter() {
            return (Token.Character)this;
        }

        internal bool IsEOF() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF;
        }
    }

    internal enum TokenType {
        Doctype,
        StartTag,
        EndTag,
        Comment,
        Character,
        EOF
    }
}
