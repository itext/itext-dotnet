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
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Readers the input stream into tokens.</summary>
    internal sealed class Tokeniser {
        internal const char replacementChar = '\uFFFD';

        // replaces null character
        private static readonly char[] notCharRefCharsSorted = new char[] { '\t', '\n', '\r', '\f', ' ', '<', '&' };

        static Tokeniser() {
            JavaUtil.Sort(notCharRefCharsSorted);
        }

        private CharacterReader reader;

        // html input
        private ParseErrorList errors;

        // errors found while tokenising
        private TokeniserState state = TokeniserState.Data;

        // current tokenisation state
        private Token emitPending;

        // the token we are about to emit on next read
        private bool isEmitPending = false;

        private String charsString = null;

        // characters pending an emit. Will fall to charsBuilder if more than one
        private StringBuilder charsBuilder = new StringBuilder(1024);

        // buffers characters to output as one token, if more than one emit per read
        internal StringBuilder dataBuffer = new StringBuilder(1024);

        // buffers data looking for </script>
        internal Token.Tag tagPending;

        // tag we are building up
        internal Token.StartTag startPending = new Token.StartTag();

        internal Token.EndTag endPending = new Token.EndTag();

        internal Token.Character charPending = new Token.Character();

        internal Token.Doctype doctypePending = new Token.Doctype();

        // doctype building up
        internal Token.Comment commentPending = new Token.Comment();

        // comment building up
        private String lastStartTag;

        // the last start tag emitted, to test appropriate end tag
        private bool selfClosingFlagAcknowledged = true;

        internal Tokeniser(CharacterReader reader, ParseErrorList errors) {
            this.reader = reader;
            this.errors = errors;
        }

        internal Token Read() {
            if (!selfClosingFlagAcknowledged) {
                Error("Self closing flag not acknowledged");
                selfClosingFlagAcknowledged = true;
            }
            while (!isEmitPending) {
                state.Read(this, reader);
            }
            // if emit is pending, a non-character token was found: return any chars in buffer, and leave token for next read:
            if (charsBuilder.Length > 0) {
                String str = charsBuilder.ToString();
                charsBuilder.Delete(0, charsBuilder.Length);
                charsString = null;
                return charPending.Data(str);
            }
            else {
                if (charsString != null) {
                    Token token = charPending.Data(charsString);
                    charsString = null;
                    return token;
                }
                else {
                    isEmitPending = false;
                    return emitPending;
                }
            }
        }

        internal void Emit(Token token) {
            Validate.IsFalse(isEmitPending, "There is an unread token pending!");
            emitPending = token;
            isEmitPending = true;
            if (token.type == iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag) {
                Token.StartTag startTag = (Token.StartTag)token;
                lastStartTag = startTag.tagName;
                if (startTag.selfClosing) {
                    selfClosingFlagAcknowledged = false;
                }
            }
            else {
                if (token.type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag) {
                    Token.EndTag endTag = (Token.EndTag)token;
                    if (endTag.attributes != null) {
                        Error("Attributes incorrectly present on end tag");
                    }
                }
            }
        }

        internal void Emit(String str) {
            // buffer strings up until last string token found, to emit only one token for a run of character refs etc.
            // does not set isEmitPending; read checks that
            if (charsString == null) {
                charsString = str;
            }
            else {
                if (charsBuilder.Length == 0) {
                    // switching to string builder as more than one emit before read
                    charsBuilder.Append(charsString);
                }
                charsBuilder.Append(str);
            }
        }

        internal void Emit(char[] chars) {
            Emit(JavaUtil.GetStringForChars(chars));
        }

        internal void Emit(char c) {
            Emit(c.ToString());
        }

        internal TokeniserState GetState() {
            return state;
        }

        internal void Transition(TokeniserState state) {
            this.state = state;
        }

        internal void AdvanceTransition(TokeniserState state) {
            reader.Advance();
            this.state = state;
        }

        internal void AcknowledgeSelfClosingFlag() {
            selfClosingFlagAcknowledged = true;
        }

        private readonly char[] charRefHolder = new char[1];

        // holder to not have to keep creating arrays
        internal char[] ConsumeCharacterReference(char? additionalAllowedCharacter, bool inAttribute) {
            if (reader.IsEmpty()) {
                return null;
            }
            if (additionalAllowedCharacter != null && additionalAllowedCharacter == reader.Current()) {
                return null;
            }
            if (reader.MatchesAnySorted(notCharRefCharsSorted)) {
                return null;
            }
            char[] charRef = charRefHolder;
            reader.Mark();
            if (reader.MatchConsume("#")) {
                // numbered
                bool isHexMode = reader.MatchConsumeIgnoreCase("X");
                String numRef = isHexMode ? reader.ConsumeHexSequence() : reader.ConsumeDigitSequence();
                if (numRef.Length == 0) {
                    // didn't match anything
                    CharacterReferenceError("numeric reference with no numerals");
                    reader.RewindToMark();
                    return null;
                }
                if (!reader.MatchConsume(";")) {
                    CharacterReferenceError("missing semicolon");
                }
                // missing semi
                int charval = -1;
                try {
                    int @base = isHexMode ? 16 : 10;
                    charval = Convert.ToInt32(numRef, @base);
                }
                catch (FormatException) {
                }
                // skip
                if (charval == -1 || (charval >= 0xD800 && charval <= 0xDFFF) || charval > 0x10FFFF) {
                    CharacterReferenceError("character outside of valid range");
                    charRef[0] = replacementChar;
                    return charRef;
                }
                else {
                    // todo: implement number replacement table
                    // todo: check for extra illegal unicode points as parse errors
                    if (charval < iText.IO.Util.TextUtil.CHARACTER_MIN_SUPPLEMENTARY_CODE_POINT) {
                        charRef[0] = (char)charval;
                        return charRef;
                    }
                    else {
                        return iText.IO.Util.TextUtil.ToChars(charval);
                    }
                }
            }
            else {
                // named
                // get as many letters as possible, and look for matching entities.
                String nameRef = reader.ConsumeLetterThenDigitSequence();
                bool looksLegit = reader.Matches(';');
                // found if a base named entity without a ;, or an extended entity with the ;.
                bool found = (Entities.IsBaseNamedEntity(nameRef) || (Entities.IsNamedEntity(nameRef) && looksLegit));
                if (!found) {
                    reader.RewindToMark();
                    if (looksLegit) {
                        // named with semicolon
                        CharacterReferenceError(MessageFormatUtil.Format("invalid named referenece " + PortUtil.EscapedSingleBracket
                             + "{0}" + PortUtil.EscapedSingleBracket, nameRef));
                    }
                    return null;
                }
                if (inAttribute && (reader.MatchesLetter() || reader.MatchesDigit() || reader.MatchesAny('=', '-', '_'))) {
                    // don't want that to match
                    reader.RewindToMark();
                    return null;
                }
                if (!reader.MatchConsume(";")) {
                    CharacterReferenceError("missing semicolon");
                }
                // missing semi
                charRef[0] = (char)Entities.GetCharacterByName(nameRef);
                return charRef;
            }
        }

        internal Token.Tag CreateTagPending(bool start) {
            tagPending = (Token.Tag)(start ? startPending.Reset() : endPending.Reset());
            return tagPending;
        }

        internal void EmitTagPending() {
            tagPending.FinaliseTag();
            Emit(tagPending);
        }

        internal void CreateCommentPending() {
            commentPending.Reset();
        }

        internal void EmitCommentPending() {
            Emit(commentPending);
        }

        internal void CreateDoctypePending() {
            doctypePending.Reset();
        }

        internal void EmitDoctypePending() {
            Emit(doctypePending);
        }

        internal void CreateTempBuffer() {
            Token.Reset(dataBuffer);
        }

        internal bool IsAppropriateEndTagToken() {
            return lastStartTag != null && tagPending.tagName.Equals(lastStartTag);
        }

        internal String AppropriateEndTagName() {
            if (lastStartTag == null) {
                return null;
            }
            return lastStartTag;
        }

        internal void Error(TokeniserState state) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), "Unexpected character " + PortUtil.EscapedSingleBracket + "{0}" + 
                    PortUtil.EscapedSingleBracket + " in input state [{}]", reader.Current(), state));
            }
        }

        internal void EofError(TokeniserState state) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), "Unexpectedly reached end of file (EOF) in input state [{0}]", state
                    ));
            }
        }

        private void CharacterReferenceError(String message) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), "Invalid character reference: {0}", message));
            }
        }

        private void Error(String errorMsg) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), errorMsg));
            }
        }

        internal bool CurrentNodeInHtmlNS() {
            // todo: implement namespaces correctly
            return true;
        }

        // Element currentNode = currentNode();
        // return currentNode != null && currentNode.namespace().equals("HTML");
        /// <summary>Utility method to consume reader and unescape entities found within.</summary>
        /// <param name="inAttribute"/>
        /// <returns>unescaped string from reader</returns>
        internal String UnescapeEntities(bool inAttribute) {
            StringBuilder builder = new StringBuilder();
            while (!reader.IsEmpty()) {
                builder.Append(reader.ConsumeTo('&'));
                if (reader.Matches('&')) {
                    reader.Consume();
                    char[] c = ConsumeCharacterReference(null, inAttribute);
                    if (c == null || c.Length == 0) {
                        builder.Append('&');
                    }
                    else {
                        builder.Append(c);
                    }
                }
            }
            return builder.ToString();
        }
    }
}
