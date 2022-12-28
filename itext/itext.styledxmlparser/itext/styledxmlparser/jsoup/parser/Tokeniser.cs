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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Readers the input stream into tokens.</summary>
    internal sealed class Tokeniser {
        internal const char replacementChar = '\uFFFD';

        // replaces null character
        private static readonly char[] notCharRefCharsSorted = new char[] { '\t', '\n', '\r', '\f', ' ', '<', '&' };

        // Some illegal character escapes are parsed by browsers as windows-1252 instead. See issue #1034
        // https://html.spec.whatwg.org/multipage/parsing.html#numeric-character-reference-end-state
        internal const int win1252ExtensionsStart = 0x80;

        internal static readonly int[] win1252Extensions = new int[] { 
                // we could build this manually, but Windows-1252 is not a standard java charset so that could break on
                
                // some platforms - this table is verified with a test
                0x20AC, 0x0081, 0x201A, 0x0192, 0x201E, 0x2026, 0x2020, 0x2021, 0x02C6, 0x2030, 0x0160, 0x2039, 0x0152, 0x008D
            , 0x017D, 0x008F, 0x0090, 0x2018, 0x2019, 0x201C, 0x201D, 0x2022, 0x2013, 0x2014, 0x02DC, 0x2122, 0x0161
            , 0x203A, 0x0153, 0x009D, 0x017E, 0x0178 };

        static Tokeniser() {
            JavaUtil.Sort(notCharRefCharsSorted);
        }

        private readonly CharacterReader reader;

        // html input
        private readonly ParseErrorList errors;

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
        internal Tokeniser(CharacterReader reader, ParseErrorList errors) {
            this.reader = reader;
            this.errors = errors;
        }

        internal Token Read() {
            while (!isEmitPending) {
                state.Read(this, reader);
            }
            // if emit is pending, a non-character token was found: return any chars in buffer, and leave token for next read:
            StringBuilder cb = this.charsBuilder;
            if (cb.Length != 0) {
                String str = cb.ToString();
                cb.Delete(0, cb.Length);
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
            Validate.IsFalse(isEmitPending);
            emitPending = token;
            isEmitPending = true;
            if (token.type == iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag) {
                Token.StartTag startTag = (Token.StartTag)token;
                lastStartTag = startTag.tagName;
            }
            else {
                if (token.type == iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag) {
                    Token.EndTag endTag = (Token.EndTag)token;
                    if (endTag.HasAttributes()) {
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

        // variations to limit need to create temp strings
        internal void Emit(StringBuilder str) {
            if (charsString == null) {
                charsString = str.ToString();
            }
            else {
                if (charsBuilder.Length == 0) {
                    charsBuilder.Append(charsString);
                }
                charsBuilder.Append(str);
            }
        }

        internal void Emit(char c) {
            if (charsString == null) {
                charsString = c.ToString();
            }
            else {
                if (charsBuilder.Length == 0) {
                    charsBuilder.Append(charsString);
                }
                charsBuilder.Append(c);
            }
        }

        internal void Emit(char[] chars) {
            Emit(JavaUtil.GetStringForChars(chars));
        }

        internal void Emit(int[] codepoints) {
            // We have to do this conversion manually because .NET doesn't support creating String from int array.
            IList<char> chars = new List<char>();
            foreach (int codepoint in codepoints) {
                if ((int)(((uint)codepoint) >> 16) == 0) {
                    chars.Add((char)codepoint);
                }
                else {
                    char highSymbol = (char)(((int)(((uint)codepoint) >> 10)) + ('\uD800' - ((int)(((uint)0x010000) >> 10))));
                    chars.Add(highSymbol);
                    char lowSymbol = (char)((codepoint & 0x3ff) + '\uDC00');
                    chars.Add(lowSymbol);
                }
            }
            char[] charsArray = new char[chars.Count];
            for (int i = 0; i < chars.Count; i++) {
                charsArray[i] = (char)chars[i];
            }
            Emit(charsArray);
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

        private readonly int[] codepointHolder = new int[1];

        // holder to not have to keep creating arrays
        private readonly int[] multipointHolder = new int[2];

        internal int[] ConsumeCharacterReference(char? additionalAllowedCharacter, bool inAttribute) {
            if (reader.IsEmpty()) {
                return null;
            }
            if (additionalAllowedCharacter != null && additionalAllowedCharacter == reader.Current()) {
                return null;
            }
            if (reader.MatchesAnySorted(notCharRefCharsSorted)) {
                return null;
            }
            int[] codeRef = codepointHolder;
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
                reader.Unmark();
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
                    codeRef[0] = replacementChar;
                }
                else {
                    // fix illegal unicode characters to match browser behavior
                    if (charval >= win1252ExtensionsStart && charval < win1252ExtensionsStart + win1252Extensions.Length) {
                        CharacterReferenceError("character is not a valid unicode code point");
                        charval = win1252Extensions[charval - win1252ExtensionsStart];
                    }
                    codeRef[0] = charval;
                }
                return codeRef;
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
                        CharacterReferenceError("invalid named reference");
                    }
                    return null;
                }
                if (inAttribute && (reader.MatchesLetter() || reader.MatchesDigit() || reader.MatchesAny('=', '-', '_'))) {
                    // don't want that to match
                    reader.RewindToMark();
                    return null;
                }
                reader.Unmark();
                if (!reader.MatchConsume(";")) {
                    CharacterReferenceError("missing semicolon");
                }
                // missing semi
                int numChars = Entities.CodepointsForName(nameRef, multipointHolder);
                if (numChars == 1) {
                    codeRef[0] = multipointHolder[0];
                    return codeRef;
                }
                else {
                    if (numChars == 2) {
                        return multipointHolder;
                    }
                    else {
                        Validate.Fail("Unexpected characters returned for " + nameRef);
                        return multipointHolder;
                    }
                }
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

        internal void CreateBogusCommentPending() {
            commentPending.Reset();
            commentPending.bogus = true;
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
            return lastStartTag != null && tagPending.Name().EqualsIgnoreCase(lastStartTag);
        }

        internal String AppropriateEndTagName() {
            return lastStartTag;
        }

        // could be null
        internal void Error(TokeniserState state) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), "Unexpected character '{0}' in input state [{1}]", reader.Current(
                    ), state));
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

        internal void Error(String errorMsg) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), errorMsg));
            }
        }

        internal bool CurrentNodeInHtmlNS() {
            return true;
        }

        // Element currentNode = currentNode();
        // return currentNode != null && currentNode.namespace().equals("HTML");
        /// <summary>Utility method to consume reader and unescape entities found within.</summary>
        /// <param name="inAttribute">if the text to be unescaped is in an attribute</param>
        /// <returns>unescaped string from reader</returns>
        internal String UnescapeEntities(bool inAttribute) {
            StringBuilder builder = iText.StyledXmlParser.Jsoup.Internal.StringUtil.BorrowBuilder();
            while (!reader.IsEmpty()) {
                builder.Append(reader.ConsumeTo('&'));
                if (reader.Matches('&')) {
                    reader.Consume();
                    int[] c = ConsumeCharacterReference(null, inAttribute);
                    if (c == null || c.Length == 0) {
                        builder.Append('&');
                    }
                    else {
                        builder.AppendCodePoint(c[0]);
                        if (c.Length == 2) {
                            builder.AppendCodePoint(c[1]);
                        }
                    }
                }
            }
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.ReleaseBuilder(builder);
        }
    }
}
