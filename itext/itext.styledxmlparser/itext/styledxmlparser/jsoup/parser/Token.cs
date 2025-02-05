/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Internal;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Parse tokens for the Tokeniser.</summary>
    public abstract class Token {
//\cond DO_NOT_DOCUMENT
        internal iText.StyledXmlParser.Jsoup.Parser.TokenType type;
//\endcond

        private Token() {
        }

//\cond DO_NOT_DOCUMENT
        internal virtual String TokenType() {
            return this.GetType().Name;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Reset the data represent by this token, for reuse.</summary>
        /// <remarks>
        /// Reset the data represent by this token, for reuse. Prevents the need to create transfer objects for every
        /// piece of data, which immediately get GCed.
        /// </remarks>
        internal abstract iText.StyledXmlParser.Jsoup.Parser.Token Reset();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void Reset(StringBuilder sb) {
            if (sb != null) {
                sb.Delete(0, sb.Length);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class Doctype : Token {
//\cond DO_NOT_DOCUMENT
            internal readonly StringBuilder name = new StringBuilder();
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String pubSysKey = null;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly StringBuilder publicIdentifier = new StringBuilder();
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly StringBuilder systemIdentifier = new StringBuilder();
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool forceQuirks = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Doctype() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal override Token Reset() {
                Reset(name);
                pubSysKey = null;
                Reset(publicIdentifier);
                Reset(systemIdentifier);
                forceQuirks = false;
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String GetName() {
                return name.ToString();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String GetPubSysKey() {
                return pubSysKey;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String GetPublicIdentifier() {
                return publicIdentifier.ToString();
            }
//\endcond

            public String GetSystemIdentifier() {
                return systemIdentifier.ToString();
            }

            public bool IsForceQuirks() {
                return forceQuirks;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract class Tag : Token {
            protected internal String tagName;

            protected internal String normalName;

            // lc version of tag name, for case insensitive tree build
            private String pendingAttributeName;

            // attribute names are generally caught in one hop, not accumulated
            private StringBuilder pendingAttributeValue = new StringBuilder();

            // but values are accumulated, from e.g. & in hrefs
            private String pendingAttributeValueS;

            // try to get attr vals in one shot, vs Builder
            private bool hasEmptyAttributeValue = false;

            // distinguish boolean attribute from empty string value
            private bool hasPendingAttributeValue = false;

//\cond DO_NOT_DOCUMENT
            internal bool selfClosing = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Attributes attributes;
//\endcond

//\cond DO_NOT_DOCUMENT
            // start tags get attributes on construction. End tags get attributes on first new attribute (but only for parser convenience, not used).
            internal override Token Reset() {
                tagName = null;
                normalName = null;
                pendingAttributeName = null;
                Reset(pendingAttributeValue);
                pendingAttributeValueS = null;
                hasEmptyAttributeValue = false;
                hasPendingAttributeValue = false;
                selfClosing = false;
                attributes = null;
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void NewAttribute() {
                if (attributes == null) {
                    attributes = new Attributes();
                }
                if (pendingAttributeName != null) {
                    // the tokeniser has skipped whitespace control chars, but trimming could collapse to empty for other control codes, so verify here
                    pendingAttributeName = PortUtil.TrimControlCodes(pendingAttributeName);
                    if (pendingAttributeName.Length > 0) {
                        String value;
                        if (hasPendingAttributeValue) {
                            value = pendingAttributeValue.Length > 0 ? pendingAttributeValue.ToString() : pendingAttributeValueS;
                        }
                        else {
                            if (hasEmptyAttributeValue) {
                                value = "";
                            }
                            else {
                                value = null;
                            }
                        }
                        // note that we add, not put. So that the first is kept, and rest are deduped, once in a context where case sensitivity is known (the appropriate tree builder).
                        attributes.Add(pendingAttributeName, value);
                    }
                }
                pendingAttributeName = null;
                hasEmptyAttributeValue = false;
                hasPendingAttributeValue = false;
                Reset(pendingAttributeValue);
                pendingAttributeValueS = null;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool HasAttributes() {
                return attributes != null;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool HasAttribute(String key) {
                return attributes != null && attributes.HasKey(key);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void FinaliseTag() {
                // finalises for emit
                if (pendingAttributeName != null) {
                    NewAttribute();
                }
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            /// <summary>Preserves case</summary>
            internal String Name() {
                // preserves case, for input into Tag.valueOf (which may drop case)
                Validate.IsFalse(tagName == null || tagName.Length == 0);
                return tagName;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            /// <summary>Lower case</summary>
            internal String NormalName() {
                // lower case, used in tree building for working out where in tree it should go
                return normalName;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String ToStringName() {
                return tagName != null ? tagName : "[unset]";
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Token.Tag Name(String name) {
                tagName = name;
                normalName = Normalizer.LowerCase(name);
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool IsSelfClosing() {
                return selfClosing;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            // these appenders are rarely hit in not null state-- caused by null chars.
            internal void AppendTagName(String append) {
                tagName = tagName == null ? append : tagName + append;
                normalName = Normalizer.LowerCase(tagName);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendTagName(char append) {
                AppendTagName(append.ToString());
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendAttributeName(String append) {
                pendingAttributeName = pendingAttributeName == null ? append : pendingAttributeName + append;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendAttributeName(char append) {
                AppendAttributeName(append.ToString());
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendAttributeValue(String append) {
                EnsureAttributeValue();
                if (pendingAttributeValue.Length == 0) {
                    pendingAttributeValueS = append;
                }
                else {
                    pendingAttributeValue.Append(append);
                }
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendAttributeValue(char append) {
                EnsureAttributeValue();
                pendingAttributeValue.Append(append);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendAttributeValue(char[] append) {
                EnsureAttributeValue();
                pendingAttributeValue.Append(append);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void AppendAttributeValue(int[] appendCodepoints) {
                EnsureAttributeValue();
                foreach (int codepoint in appendCodepoints) {
                    pendingAttributeValue.AppendCodePoint(codepoint);
                }
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal void SetEmptyAttributeValue() {
                hasEmptyAttributeValue = true;
            }
//\endcond

            private void EnsureAttributeValue() {
                hasPendingAttributeValue = true;
                // if on second hit, we'll need to move to the builder
                if (pendingAttributeValueS != null) {
                    pendingAttributeValue.Append(pendingAttributeValueS);
                    pendingAttributeValueS = null;
                }
            }

            public abstract override String ToString();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class StartTag : Token.Tag {
//\cond DO_NOT_DOCUMENT
            internal StartTag()
                : base() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal override Token Reset() {
                base.Reset();
                attributes = null;
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Token.StartTag NameAttr(String name, Attributes attributes) {
                this.tagName = name;
                this.attributes = attributes;
                normalName = Normalizer.LowerCase(tagName);
                return this;
            }
//\endcond

            public override String ToString() {
                if (HasAttributes() && attributes.Size() > 0) {
                    return "<" + ToStringName() + " " + attributes.ToString() + ">";
                }
                else {
                    return "<" + ToStringName() + ">";
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class EndTag : Token.Tag {
//\cond DO_NOT_DOCUMENT
            internal EndTag()
                : base() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag;
            }
//\endcond

            public override String ToString() {
                return "</" + ToStringName() + ">";
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class Comment : Token {
            private readonly StringBuilder data = new StringBuilder();

            private String dataS;

//\cond DO_NOT_DOCUMENT
            // try to get in one shot
            internal bool bogus = false;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal override Token Reset() {
                Reset(data);
                dataS = null;
                bogus = false;
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Comment() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String GetData() {
                return dataS != null ? dataS : data.ToString();
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Token.Comment Append(String append) {
                EnsureData();
                if (data.Length == 0) {
                    dataS = append;
                }
                else {
                    data.Append(append);
                }
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Token.Comment Append(char append) {
                EnsureData();
                data.Append(append);
                return this;
            }
//\endcond

            private void EnsureData() {
                // if on second hit, we'll need to move to the builder
                if (dataS != null) {
                    data.Append(dataS);
                    dataS = null;
                }
            }

            public override String ToString() {
                return "<!--" + GetData() + "-->";
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class Character : Token {
            private String data;

//\cond DO_NOT_DOCUMENT
            internal Character()
                : base() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.Character;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal override Token Reset() {
                data = null;
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual Token.Character Data(String data) {
                this.data = data;
                return this;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual String GetData() {
                return data;
            }
//\endcond

            public override String ToString() {
                return GetData();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class CData : Token.Character {
//\cond DO_NOT_DOCUMENT
            internal CData(String data)
                : base() {
                this.Data(data);
            }
//\endcond

            public override String ToString() {
                return "<![CDATA[" + GetData() + "]]>";
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class EOF : Token {
//\cond DO_NOT_DOCUMENT
            internal EOF() {
                type = iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal override Token Reset() {
                return this;
            }
//\endcond

            public override String ToString() {
                return "";
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsDoctype() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Token.Doctype AsDoctype() {
            return (Token.Doctype)this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsStartTag() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Token.StartTag AsStartTag() {
            return (Token.StartTag)this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsEndTag() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Token.EndTag AsEndTag() {
            return (Token.EndTag)this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsComment() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Token.Comment AsComment() {
            return (Token.Comment)this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsCharacter() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Character;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsCData() {
            return this is Token.CData;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Token.Character AsCharacter() {
            return (Token.Character)this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool IsEOF() {
            return type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF;
        }
//\endcond
        // note no CData - treated in builder as an extension of Character
    }

    public enum TokenType {
        Doctype,
        StartTag,
        EndTag,
        Comment,
        Character,
        EOF
    }
}
