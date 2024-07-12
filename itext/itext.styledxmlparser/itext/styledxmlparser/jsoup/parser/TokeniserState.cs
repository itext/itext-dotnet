/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
//\cond DO_NOT_DOCUMENT
    /// <summary>States and transition activations for the Tokeniser.</summary>
    internal abstract class TokeniserState {
//\cond DO_NOT_DOCUMENT
        internal static TokeniserState Data = new TokeniserState.DataTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CharacterReferenceInData = new TokeniserState.CharacterReferenceInDataTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState Rcdata = new TokeniserState.RcDataTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CharacterReferenceInRcdata = new TokeniserState.CharacterReferenceInRcdataTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState Rawtext = new TokeniserState.RawTextTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptData = new TokeniserState.ScriptDataTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState PLAINTEXT = new TokeniserState.PlainTextTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState TagOpen = new TokeniserState.TagOpenTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState EndTagOpen = new TokeniserState.EndTagOpenTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState TagName = new TokeniserState.TagNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState RcdataLessthanSign = new TokeniserState.RcDataLessThanSignTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState RCDATAEndTagOpen = new TokeniserState.RcDataEndTagOpenTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState RCDATAEndTagName = new TokeniserState.RcDataEndTagNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState RawtextLessthanSign = new TokeniserState.RawTextLessThanSignTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState RawtextEndTagOpen = new TokeniserState.RawTextEndTagOpenTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState RawtextEndTagName = new TokeniserState.RawTextEndTagNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataLessthanSign = new TokeniserState.ScriptDataLessThanSignTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEndTagOpen = new TokeniserState.ScriptDataEndTagOpenTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEndTagName = new TokeniserState.ScriptDataEndTagNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapeStart = new TokeniserState.ScriptDataEscapeStartTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapeStartDash = new TokeniserState.ScriptDataEscapeStartDashTS(
            );
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscaped = new TokeniserState.ScriptDataEscapedTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapedDash = new TokeniserState.ScriptDataEscapedDashTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapedDashDash = new TokeniserState.ScriptDataEscapedDashDashTS(
            );
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapedLessthanSign = new TokeniserState.ScriptDataEscapedLessThanSignTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapedEndTagOpen = new TokeniserState.ScriptDataEscapedEndTagOpenTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataEscapedEndTagName = new TokeniserState.ScriptDataEscapedEndTagNameTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataDoubleEscapeStart = new TokeniserState.ScriptDataDoubleEscapeStartTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataDoubleEscaped = new TokeniserState.ScriptDataDoubleEscapedTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataDoubleEscapedDash = new TokeniserState.ScriptDataDoubleEscapedDashTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataDoubleEscapedDashDash = new TokeniserState.ScriptDataDoubleEscapedDashDashTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataDoubleEscapedLessthanSign = new TokeniserState.ScriptDataDoubleEscapedLessThanSignTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState ScriptDataDoubleEscapeEnd = new TokeniserState.ScriptDataDoubleEscapeEndTS(
            );
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BeforeAttributeName = new TokeniserState.BeforeAttributeNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AttributeName = new TokeniserState.AttributeNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AfterAttributeName = new TokeniserState.AfterAttributeNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BeforeAttributeValue = new TokeniserState.BeforeAttributeValueTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AttributeValue_doubleQuoted = new TokeniserState.AttributeValueDoubleQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AttributeValue_singleQuoted = new TokeniserState.AttributeValueSingleQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AttributeValue_unquoted = new TokeniserState.AttributeValueUnquotedTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        // CharacterReferenceInAttributeValue state handled inline
        internal static TokeniserState AfterAttributeValue_quoted = new TokeniserState.AfterAttributeValueQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState SelfClosingStartTag = new TokeniserState.SelfClosingStartTagTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BogusComment = new TokeniserState.BogusCommentTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState MarkupDeclarationOpen = new TokeniserState.MarkupDeclarationOpenTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CommentStart = new TokeniserState.CommentStartTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CommentStartDash = new TokeniserState.CommentStartDashTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState Comment = new TokeniserState.CommentTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CommentEndDash = new TokeniserState.CommentEndDashTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CommentEnd = new TokeniserState.CommentEndTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CommentEndBang = new TokeniserState.CommentEndBangTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState Doctype = new TokeniserState.DocTypeTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BeforeDoctypeName = new TokeniserState.BeforeDocTypeNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState DoctypeName = new TokeniserState.DocTypeNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AfterDoctypeName = new TokeniserState.AfterDocTypeNameTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AfterDoctypePublicKeyword = new TokeniserState.AfterDocTypePublicKeywordTS(
            );
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BeforeDoctypePublicIdentifier = new TokeniserState.BeforeDocTypePublicIdentifierTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState DoctypePublicIdentifier_doubleQuoted = new TokeniserState.DocTypePublicIdentifierDoubleQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState DoctypePublicIdentifier_singleQuoted = new TokeniserState.DocTypePublicIdentifierSingleQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AfterDoctypePublicIdentifier = new TokeniserState.AfterDocTypePublicIdentifierTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BetweenDoctypePublicAndSystemIdentifiers = new TokeniserState.BetweenDocTypePublicAndSystemIdentifiersTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AfterDoctypeSystemKeyword = new TokeniserState.AfterDocTypeSystemKeywordTS(
            );
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BeforeDoctypeSystemIdentifier = new TokeniserState.BeforeDocTypeSystemIdentifierTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState DoctypeSystemIdentifier_doubleQuoted = new TokeniserState.DocTypeSystemIdentifierDoubleQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState DoctypeSystemIdentifier_singleQuoted = new TokeniserState.DocTypeSystemIdentifierSingleQuotedTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState AfterDoctypeSystemIdentifier = new TokeniserState.AfterDocTypeSystemIdentifierTS
            ();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState BogusDoctype = new TokeniserState.BogusDocTypeTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static TokeniserState CdataSection = new TokeniserState.CDataSectionTS();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract void Read(Tokeniser t, CharacterReader r);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const char nullChar = '\u0000';
//\endcond

//\cond DO_NOT_DOCUMENT
        // char searches. must be sorted, used in inSorted. MUST update TokenisetStateTest if more arrays are added.
        internal static readonly char[] attributeNameCharsSorted = new char[] { nullChar, '\t', '\n', '\f', '\r', 
            ' ', '"', '\'', '/', '<', '=', '>' };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly char[] attributeValueUnquoted = new char[] { nullChar, '\t', '\n', '\f', '\r', ' '
            , '"', '&', '\'', '<', '=', '>', '`' };
//\endcond

        private const char replacementChar = Tokeniser.replacementChar;

        private static readonly String replacementStr = Tokeniser.replacementChar.ToString();

        private const char eof = CharacterReader.EOF;

        /// <summary>Handles RawtextEndTagName, ScriptDataEndTagName, and ScriptDataEscapedEndTagName.</summary>
        /// <remarks>
        /// Handles RawtextEndTagName, ScriptDataEndTagName, and ScriptDataEscapedEndTagName. Same body impl, just
        /// different else exit transitions.
        /// </remarks>
        private static void HandleDataEndTag(Tokeniser t, CharacterReader r, TokeniserState elseTransition) {
            if (r.MatchesLetter()) {
                String name = r.ConsumeLetterSequence();
                t.tagPending.AppendTagName(name);
                t.dataBuffer.Append(name);
                return;
            }
            bool needsExitTransition = false;
            if (t.IsAppropriateEndTagToken() && !r.IsEmpty()) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(SelfClosingStartTag);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.dataBuffer.Append(c);
                        needsExitTransition = true;
                        break;
                    }
                }
            }
            else {
                needsExitTransition = true;
            }
            if (needsExitTransition) {
                t.Emit("</");
                t.Emit(t.dataBuffer);
                t.Transition(elseTransition);
            }
        }

        private static void ReadRawData(Tokeniser t, CharacterReader r, TokeniserState current, TokeniserState advance
            ) {
            switch (r.Current()) {
                case '<': {
                    t.AdvanceTransition(advance);
                    break;
                }

                case nullChar: {
                    t.Error(current);
                    r.Advance();
                    t.Emit(replacementChar);
                    break;
                }

                case eof: {
                    t.Emit(new Token.EOF());
                    break;
                }

                default: {
                    String data = r.ConsumeRawData();
                    t.Emit(data);
                    break;
                }
            }
        }

        private static void ReadCharRef(Tokeniser t, TokeniserState advance) {
            int[] c = t.ConsumeCharacterReference(null, false);
            if (c == null) {
                t.Emit('&');
            }
            else {
                t.Emit(c);
            }
            t.Transition(advance);
        }

        private static void ReadEndTag(Tokeniser t, CharacterReader r, TokeniserState a, TokeniserState b) {
            if (r.MatchesLetter()) {
                t.CreateTagPending(false);
                t.Transition(a);
            }
            else {
                t.Emit("</");
                t.Transition(b);
            }
        }

        private static void HandleDataDoubleEscapeTag(Tokeniser t, CharacterReader r, TokeniserState primary, TokeniserState
             fallback) {
            if (r.MatchesLetter()) {
                String name = r.ConsumeLetterSequence();
                t.dataBuffer.Append(name);
                t.Emit(name);
                return;
            }
            char c = r.Consume();
            switch (c) {
                case '\t':
                case '\n':
                case '\r':
                case '\f':
                case ' ':
                case '/':
                case '>': {
                    if (t.dataBuffer.ToString().Equals("script")) {
                        t.Transition(primary);
                    }
                    else {
                        t.Transition(fallback);
                    }
                    t.Emit(c);
                    break;
                }

                default: {
                    r.Unconsume();
                    t.Transition(fallback);
                    break;
                }
            }
        }

        private sealed class DataTS : TokeniserState {
            public override String ToString() {
                return "Data";
            }

//\cond DO_NOT_DOCUMENT
            // in data state, gather characters until a character reference or tag is found
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case '&': {
                        t.AdvanceTransition(CharacterReferenceInData);
                        break;
                    }

                    case '<': {
                        t.AdvanceTransition(TagOpen);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        // NOT replacement character (oddly?)
                        t.Emit(r.Consume());
                        break;
                    }

                    case eof: {
                        t.Emit(new Token.EOF());
                        break;
                    }

                    default: {
                        String data = r.ConsumeData();
                        t.Emit(data);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CharacterReferenceInDataTS : TokeniserState {
            public override String ToString() {
                return "CharacterReferenceInData";
            }

//\cond DO_NOT_DOCUMENT
            // from & in data
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadCharRef(t, Data);
            }
//\endcond
        }

        private sealed class RcDataTS : TokeniserState {
            public override String ToString() {
                return "Rcdata";
            }

//\cond DO_NOT_DOCUMENT
            /// handles data in title, textarea etc
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case '&': {
                        t.AdvanceTransition(CharacterReferenceInRcdata);
                        break;
                    }

                    case '<': {
                        t.AdvanceTransition(RcdataLessthanSign);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(replacementChar);
                        break;
                    }

                    case eof: {
                        t.Emit(new Token.EOF());
                        break;
                    }

                    default: {
                        String data = r.ConsumeData();
                        t.Emit(data);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CharacterReferenceInRcdataTS : TokeniserState {
            public override String ToString() {
                return "CharacterReferenceInRcdata";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadCharRef(t, Rcdata);
            }
//\endcond
        }

        private sealed class RawTextTS : TokeniserState {
            public override String ToString() {
                return "Rawtext";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadRawData(t, r, this, RawtextLessthanSign);
            }
//\endcond
        }

        private sealed class ScriptDataTS : TokeniserState {
            public override String ToString() {
                return "ScriptData";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadRawData(t, r, this, ScriptDataLessthanSign);
            }
//\endcond
        }

        private sealed class PlainTextTS : TokeniserState {
            public override String ToString() {
                return "PLAINTEXT";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(replacementChar);
                        break;
                    }

                    case eof: {
                        t.Emit(new Token.EOF());
                        break;
                    }

                    default: {
                        String data = r.ConsumeTo(nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class TagOpenTS : TokeniserState {
            public override String ToString() {
                return "TagOpen";
            }

//\cond DO_NOT_DOCUMENT
            // from < in data
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case '!': {
                        t.AdvanceTransition(MarkupDeclarationOpen);
                        break;
                    }

                    case '/': {
                        t.AdvanceTransition(EndTagOpen);
                        break;
                    }

                    case '?': {
                        t.CreateBogusCommentPending();
                        t.AdvanceTransition(BogusComment);
                        break;
                    }

                    default: {
                        if (r.MatchesLetter()) {
                            t.CreateTagPending(true);
                            t.Transition(TagName);
                        }
                        else {
                            t.Error(this);
                            t.Emit('<');
                            // char that got us here
                            t.Transition(Data);
                        }
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class EndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "EndTagOpen";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Emit("</");
                    t.Transition(Data);
                }
                else {
                    if (r.MatchesLetter()) {
                        t.CreateTagPending(false);
                        t.Transition(TagName);
                    }
                    else {
                        if (r.Matches('>')) {
                            t.Error(this);
                            t.AdvanceTransition(Data);
                        }
                        else {
                            t.Error(this);
                            t.CreateBogusCommentPending();
                            t.AdvanceTransition(BogusComment);
                        }
                    }
                }
            }
//\endcond
        }

        private sealed class TagNameTS : TokeniserState {
            public override String ToString() {
                return "TagName";
            }

//\cond DO_NOT_DOCUMENT
            // from < or </ in data, will have start or end tag pending
            internal override void Read(Tokeniser t, CharacterReader r) {
                // previous TagOpen state did NOT consume, will have a letter char in current
                //String tagName = r.consumeToAnySorted(tagCharsSorted).toLowerCase();
                String tagName = r.ConsumeTagName();
                t.tagPending.AppendTagName(tagName);
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(SelfClosingStartTag);
                        break;
                    }

                    case '<': {
                        // NOTE: out of spec, but clear author intent
                        r.Unconsume();
                        t.Error(this);
                        goto case 
                                                // intended fall through to next >
                                                '>';
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        // replacement
                        t.tagPending.AppendTagName(replacementStr);
                        break;
                    }

                    case eof: {
                        // should emit pending tag?
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        // buffer underrun
                        t.tagPending.AppendTagName(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class RcDataLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "RcdataLessthanSign";
            }

//\cond DO_NOT_DOCUMENT
            // from < in rcdata
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('/')) {
                    t.CreateTempBuffer();
                    t.AdvanceTransition(RCDATAEndTagOpen);
                }
                else {
                    if (r.MatchesLetter() && t.AppropriateEndTagName() != null && !r.ContainsIgnoreCase("</" + t.AppropriateEndTagName
                        ())) {
                        // diverge from spec: got a start tag, but there's no appropriate end tag (</title>), so rather than
                        // consuming to EOF; break out here
                        t.tagPending = t.CreateTagPending(false).Name(t.AppropriateEndTagName());
                        t.EmitTagPending();
                        t.Transition(TagOpen);
                    }
                    else {
                        // straight into TagOpen, as we came from < and looks like we're on a start tag
                        t.Emit("<");
                        t.Transition(Rcdata);
                    }
                }
            }
//\endcond
        }

        private sealed class RcDataEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "RCDATAEndTagOpen";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTagPending(false);
                    t.tagPending.AppendTagName(r.Current());
                    t.dataBuffer.Append(r.Current());
                    t.AdvanceTransition(RCDATAEndTagName);
                }
                else {
                    t.Emit("</");
                    t.Transition(Rcdata);
                }
            }
//\endcond
        }

        private sealed class RcDataEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "RCDATAEndTagName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    String name = r.ConsumeLetterSequence();
                    t.tagPending.AppendTagName(name);
                    t.dataBuffer.Append(name);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        if (t.IsAppropriateEndTagToken()) {
                            t.Transition(BeforeAttributeName);
                        }
                        else {
                            AnythingElse(t, r);
                        }
                        break;
                    }

                    case '/': {
                        if (t.IsAppropriateEndTagToken()) {
                            t.Transition(SelfClosingStartTag);
                        }
                        else {
                            AnythingElse(t, r);
                        }
                        break;
                    }

                    case '>': {
                        if (t.IsAppropriateEndTagToken()) {
                            t.EmitTagPending();
                            t.Transition(Data);
                        }
                        else {
                            AnythingElse(t, r);
                        }
                        break;
                    }

                    default: {
                        AnythingElse(t, r);
                        break;
                    }
                }
            }
//\endcond

            private void AnythingElse(Tokeniser t, CharacterReader r) {
                t.Emit("</");
                t.Emit(t.dataBuffer);
                r.Unconsume();
                t.Transition(Rcdata);
            }
        }

        private sealed class RawTextLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "RawtextLessthanSign";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('/')) {
                    t.CreateTempBuffer();
                    t.AdvanceTransition(RawtextEndTagOpen);
                }
                else {
                    t.Emit('<');
                    t.Transition(Rawtext);
                }
            }
//\endcond
        }

        private sealed class RawTextEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "RawtextEndTagOpen";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadEndTag(t, r, RawtextEndTagName, Rawtext);
            }
//\endcond
        }

        private sealed class RawTextEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "RawtextEndTagName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataEndTag(t, r, Rawtext);
            }
//\endcond
        }

        private sealed class ScriptDataLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataLessthanSign";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Consume()) {
                    case '/': {
                        t.CreateTempBuffer();
                        t.Transition(ScriptDataEndTagOpen);
                        break;
                    }

                    case '!': {
                        t.Emit("<!");
                        t.Transition(ScriptDataEscapeStart);
                        break;
                    }

                    case eof: {
                        t.Emit("<");
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Emit("<");
                        r.Unconsume();
                        t.Transition(ScriptData);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEndTagOpen";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadEndTag(t, r, ScriptDataEndTagName, ScriptData);
            }
//\endcond
        }

        private sealed class ScriptDataEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEndTagName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataEndTag(t, r, ScriptData);
            }
//\endcond
        }

        private sealed class ScriptDataEscapeStartTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapeStart";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('-')) {
                    t.Emit('-');
                    t.AdvanceTransition(ScriptDataEscapeStartDash);
                }
                else {
                    t.Transition(ScriptData);
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapeStartDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapeStartDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('-')) {
                    t.Emit('-');
                    t.AdvanceTransition(ScriptDataEscapedDashDash);
                }
                else {
                    t.Transition(ScriptData);
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapedTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscaped";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Transition(Data);
                    return;
                }
                switch (r.Current()) {
                    case '-': {
                        t.Emit('-');
                        t.AdvanceTransition(ScriptDataEscapedDash);
                        break;
                    }

                    case '<': {
                        t.AdvanceTransition(ScriptDataEscapedLessthanSign);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(replacementChar);
                        break;
                    }

                    default: {
                        String data = r.ConsumeToAny('-', '<', nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapedDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Transition(Data);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        t.Transition(ScriptDataEscapedDashDash);
                        break;
                    }

                    case '<': {
                        t.Transition(ScriptDataEscapedLessthanSign);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.Emit(replacementChar);
                        t.Transition(ScriptDataEscaped);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(ScriptDataEscaped);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapedDashDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedDashDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Transition(Data);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        break;
                    }

                    case '<': {
                        t.Transition(ScriptDataEscapedLessthanSign);
                        break;
                    }

                    case '>': {
                        t.Emit(c);
                        t.Transition(ScriptData);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.Emit(replacementChar);
                        t.Transition(ScriptDataEscaped);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(ScriptDataEscaped);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapedLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedLessthanSign";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTempBuffer();
                    t.dataBuffer.Append(r.Current());
                    t.Emit("<");
                    t.Emit(r.Current());
                    t.AdvanceTransition(ScriptDataDoubleEscapeStart);
                }
                else {
                    if (r.Matches('/')) {
                        t.CreateTempBuffer();
                        t.AdvanceTransition(ScriptDataEscapedEndTagOpen);
                    }
                    else {
                        t.Emit('<');
                        t.Transition(ScriptDataEscaped);
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapedEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedEndTagOpen";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTagPending(false);
                    t.tagPending.AppendTagName(r.Current());
                    t.dataBuffer.Append(r.Current());
                    t.AdvanceTransition(ScriptDataEscapedEndTagName);
                }
                else {
                    t.Emit("</");
                    t.Transition(ScriptDataEscaped);
                }
            }
//\endcond
        }

        private sealed class ScriptDataEscapedEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedEndTagName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataEndTag(t, r, ScriptDataEscaped);
            }
//\endcond
        }

        private sealed class ScriptDataDoubleEscapeStartTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapeStart";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataDoubleEscapeTag(t, r, ScriptDataDoubleEscaped, ScriptDataEscaped);
            }
//\endcond
        }

        private sealed class ScriptDataDoubleEscapedTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscaped";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Current();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        t.AdvanceTransition(ScriptDataDoubleEscapedDash);
                        break;
                    }

                    case '<': {
                        t.Emit(c);
                        t.AdvanceTransition(ScriptDataDoubleEscapedLessthanSign);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        String data = r.ConsumeToAny('-', '<', nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataDoubleEscapedDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapedDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        t.Transition(ScriptDataDoubleEscapedDashDash);
                        break;
                    }

                    case '<': {
                        t.Emit(c);
                        t.Transition(ScriptDataDoubleEscapedLessthanSign);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.Emit(replacementChar);
                        t.Transition(ScriptDataDoubleEscaped);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(ScriptDataDoubleEscaped);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataDoubleEscapedDashDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapedDashDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        break;
                    }

                    case '<': {
                        t.Emit(c);
                        t.Transition(ScriptDataDoubleEscapedLessthanSign);
                        break;
                    }

                    case '>': {
                        t.Emit(c);
                        t.Transition(ScriptData);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.Emit(replacementChar);
                        t.Transition(ScriptDataDoubleEscaped);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(ScriptDataDoubleEscaped);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class ScriptDataDoubleEscapedLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapedLessthanSign";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('/')) {
                    t.Emit('/');
                    t.CreateTempBuffer();
                    t.AdvanceTransition(ScriptDataDoubleEscapeEnd);
                }
                else {
                    t.Transition(ScriptDataDoubleEscaped);
                }
            }
//\endcond
        }

        private sealed class ScriptDataDoubleEscapeEndTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapeEnd";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataDoubleEscapeTag(t, r, ScriptDataEscaped, ScriptDataDoubleEscaped);
            }
//\endcond
        }

        private sealed class BeforeAttributeNameTS : TokeniserState {
            public override String ToString() {
                return "BeforeAttributeName";
            }

//\cond DO_NOT_DOCUMENT
            // from tagname <xxx
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        break;
                    }

                    // ignore whitespace
                    case '/': {
                        t.Transition(SelfClosingStartTag);
                        break;
                    }

                    case '<': {
                        // NOTE: out of spec, but clear (spec has this as a part of the attribute name)
                        r.Unconsume();
                        t.Error(this);
                        goto case 
                                                // intended fall through as if >
                                                '>';
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        r.Unconsume();
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        t.Transition(AttributeName);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    case '"':
                    case '\'':
                    case '=': {
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        t.tagPending.AppendAttributeName(c);
                        t.Transition(AttributeName);
                        break;
                    }

                    default: {
                        // A-Z, anything else
                        t.tagPending.NewAttribute();
                        r.Unconsume();
                        t.Transition(AttributeName);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AttributeNameTS : TokeniserState {
            public override String ToString() {
                return "AttributeName";
            }

//\cond DO_NOT_DOCUMENT
            // from before attribute name
            internal override void Read(Tokeniser t, CharacterReader r) {
                String name = r.ConsumeToAnySorted(attributeNameCharsSorted);
                t.tagPending.AppendAttributeName(name);
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(AfterAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(SelfClosingStartTag);
                        break;
                    }

                    case '=': {
                        t.Transition(BeforeAttributeValue);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeName(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    case '"':
                    case '\'':
                    case '<': {
                        t.Error(this);
                        t.tagPending.AppendAttributeName(c);
                        break;
                    }

                    default: {
                        // buffer underrun
                        t.tagPending.AppendAttributeName(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AfterAttributeNameTS : TokeniserState {
            public override String ToString() {
                return "AfterAttributeName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        // ignore
                        break;
                    }

                    case '/': {
                        t.Transition(SelfClosingStartTag);
                        break;
                    }

                    case '=': {
                        t.Transition(BeforeAttributeValue);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeName(replacementChar);
                        t.Transition(AttributeName);
                        break;
                    }

                    case '\'':
                    case '"':
                    case '<': {
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        t.tagPending.AppendAttributeName(c);
                        t.Transition(AttributeName);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        // A-Z, anything else
                        t.tagPending.NewAttribute();
                        r.Unconsume();
                        t.Transition(AttributeName);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class BeforeAttributeValueTS : TokeniserState {
            public override String ToString() {
                return "BeforeAttributeValue";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        // ignore
                        break;
                    }

                    case '"': {
                        t.Transition(AttributeValue_doubleQuoted);
                        break;
                    }

                    case '&': {
                        r.Unconsume();
                        t.Transition(AttributeValue_unquoted);
                        break;
                    }

                    case '\'': {
                        t.Transition(AttributeValue_singleQuoted);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(replacementChar);
                        t.Transition(AttributeValue_unquoted);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case '<':
                    case '=':
                    case '`': {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(c);
                        t.Transition(AttributeValue_unquoted);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Transition(AttributeValue_unquoted);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AttributeValueDoubleQuotedTS : TokeniserState {
            public override String ToString() {
                return "AttributeValue_doubleQuoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                String value = r.ConsumeAttributeQuoted(false);
                if (value.Length > 0) {
                    t.tagPending.AppendAttributeValue(value);
                }
                else {
                    t.tagPending.SetEmptyAttributeValue();
                }
                char c = r.Consume();
                switch (c) {
                    case '&': {
                        int[] @ref = t.ConsumeCharacterReference('"', true);
                        if (@ref != null) {
                            t.tagPending.AppendAttributeValue(@ref);
                        }
                        else {
                            t.tagPending.AppendAttributeValue('&');
                        }
                        break;
                    }

                    case '"': {
                        t.Transition(AfterAttributeValue_quoted);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(replacementChar);
                        break;
                    }

                    default: {
                        // hit end of buffer in first read, still in attribute
                        t.tagPending.AppendAttributeValue(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AttributeValueSingleQuotedTS : TokeniserState {
            public override String ToString() {
                return "AttributeValue_singleQuoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                String value = r.ConsumeAttributeQuoted(true);
                if (value.Length > 0) {
                    t.tagPending.AppendAttributeValue(value);
                }
                else {
                    t.tagPending.SetEmptyAttributeValue();
                }
                char c = r.Consume();
                switch (c) {
                    case '\'': {
                        t.Transition(AfterAttributeValue_quoted);
                        break;
                    }

                    case '&': {
                        int[] @ref = t.ConsumeCharacterReference('\'', true);
                        if (@ref != null) {
                            t.tagPending.AppendAttributeValue(@ref);
                        }
                        else {
                            t.tagPending.AppendAttributeValue('&');
                        }
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        // hit end of buffer in first read, still in attribute
                        t.tagPending.AppendAttributeValue(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AttributeValueUnquotedTS : TokeniserState {
            public override String ToString() {
                return "AttributeValue_unquoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                String value = r.ConsumeToAnySorted(attributeValueUnquoted);
                if (value.Length > 0) {
                    t.tagPending.AppendAttributeValue(value);
                }
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeAttributeName);
                        break;
                    }

                    case '&': {
                        int[] @ref = t.ConsumeCharacterReference('>', true);
                        if (@ref != null) {
                            t.tagPending.AppendAttributeValue(@ref);
                        }
                        else {
                            t.tagPending.AppendAttributeValue('&');
                        }
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    case '"':
                    case '\'':
                    case '<':
                    case '=':
                    case '`': {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(c);
                        break;
                    }

                    default: {
                        // hit end of buffer in first read, still in attribute
                        t.tagPending.AppendAttributeValue(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AfterAttributeValueQuotedTS : TokeniserState {
            public override String ToString() {
                return "AfterAttributeValue_quoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(SelfClosingStartTag);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Error(this);
                        t.Transition(BeforeAttributeName);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class SelfClosingStartTagTS : TokeniserState {
            public override String ToString() {
                return "SelfClosingStartTag";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.tagPending.selfClosing = true;
                        t.EmitTagPending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Error(this);
                        t.Transition(BeforeAttributeName);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class BogusCommentTS : TokeniserState {
            public override String ToString() {
                return "BogusComment";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                // rewind to capture character that lead us here
                r.Unconsume();
                t.commentPending.Append(r.ConsumeTo('>'));
                char next = r.Consume();
                if (next == '>' || next == eof) {
                    t.EmitCommentPending();
                    t.Transition(Data);
                }
            }
//\endcond
        }

        private sealed class MarkupDeclarationOpenTS : TokeniserState {
            public override String ToString() {
                return "MarkupDeclarationOpen";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchConsume("--")) {
                    t.CreateCommentPending();
                    t.Transition(CommentStart);
                }
                else {
                    if (r.MatchConsumeIgnoreCase("DOCTYPE")) {
                        t.Transition(Doctype);
                    }
                    else {
                        if (r.MatchConsume("[CDATA[")) {
                            // is implemented properly, keep handling as cdata
                            //} else if (!t.currentNodeInHtmlNS() && r.matchConsume("[CDATA[")) {
                            t.CreateTempBuffer();
                            t.Transition(CdataSection);
                        }
                        else {
                            t.Error(this);
                            t.CreateBogusCommentPending();
                            t.AdvanceTransition(BogusComment);
                        }
                    }
                }
            }
//\endcond
            // advance so this character gets in bogus comment data's rewind
        }

        private sealed class CommentStartTS : TokeniserState {
            public override String ToString() {
                return "CommentStart";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    case '-': {
                        t.Transition(CommentStartDash);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.commentPending.Append(replacementChar);
                        t.Transition(Comment);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Transition(Comment);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CommentStartDashTS : TokeniserState {
            public override String ToString() {
                return "CommentStartDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Transition(CommentStartDash);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.commentPending.Append(replacementChar);
                        t.Transition(Comment);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append(c);
                        t.Transition(Comment);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CommentTS : TokeniserState {
            public override String ToString() {
                return "Comment";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Current();
                switch (c) {
                    case '-': {
                        t.AdvanceTransition(CommentEndDash);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.commentPending.Append(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append(r.ConsumeToAny('-', nullChar));
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CommentEndDashTS : TokeniserState {
            public override String ToString() {
                return "CommentEndDash";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Transition(CommentEnd);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.commentPending.Append('-').Append(replacementChar);
                        t.Transition(Comment);
                        break;
                    }

                    default: {
                        t.commentPending.Append('-').Append(c);
                        t.Transition(Comment);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CommentEndTS : TokeniserState {
            public override String ToString() {
                return "CommentEnd";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.commentPending.Append("--").Append(replacementChar);
                        t.Transition(Comment);
                        break;
                    }

                    case '!': {
                        t.Error(this);
                        t.Transition(CommentEndBang);
                        break;
                    }

                    case '-': {
                        t.Error(this);
                        t.commentPending.Append('-');
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.commentPending.Append("--").Append(c);
                        t.Transition(Comment);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CommentEndBangTS : TokeniserState {
            public override String ToString() {
                return "CommentEndBang";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.commentPending.Append("--!");
                        t.Transition(CommentEndDash);
                        break;
                    }

                    case '>': {
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.commentPending.Append("--!").Append(replacementChar);
                        t.Transition(Comment);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append("--!").Append(c);
                        t.Transition(Comment);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class DocTypeTS : TokeniserState {
            public override String ToString() {
                return "Doctype";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeDoctypeName);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        goto case 
                                                // note: fall through to > case
                                                '>';
                    }

                    case '>': {
                        // catch invalid <!DOCTYPE>
                        t.Error(this);
                        t.CreateDoctypePending();
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.Transition(BeforeDoctypeName);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class BeforeDocTypeNameTS : TokeniserState {
            public override String ToString() {
                return "BeforeDoctypeName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateDoctypePending();
                    t.Transition(DoctypeName);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        break;
                    }

                    // ignore whitespace
                    case nullChar: {
                        t.Error(this);
                        t.CreateDoctypePending();
                        t.doctypePending.name.Append(replacementChar);
                        t.Transition(DoctypeName);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.CreateDoctypePending();
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.CreateDoctypePending();
                        t.doctypePending.name.Append(c);
                        t.Transition(DoctypeName);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class DocTypeNameTS : TokeniserState {
            public override String ToString() {
                return "DoctypeName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    String name = r.ConsumeLetterSequence();
                    t.doctypePending.name.Append(name);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(AfterDoctypeName);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.doctypePending.name.Append(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.doctypePending.name.Append(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AfterDocTypeNameTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypeName";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.doctypePending.forceQuirks = true;
                    t.EmitDoctypePending();
                    t.Transition(Data);
                    return;
                }
                if (r.MatchesAny('\t', '\n', '\r', '\f', ' ')) {
                    r.Advance();
                }
                else {
                    // ignore whitespace
                    if (r.Matches('>')) {
                        t.EmitDoctypePending();
                        t.AdvanceTransition(Data);
                    }
                    else {
                        if (r.MatchConsumeIgnoreCase(DocumentType.PUBLIC_KEY)) {
                            t.doctypePending.pubSysKey = DocumentType.PUBLIC_KEY;
                            t.Transition(AfterDoctypePublicKeyword);
                        }
                        else {
                            if (r.MatchConsumeIgnoreCase(DocumentType.SYSTEM_KEY)) {
                                t.doctypePending.pubSysKey = DocumentType.SYSTEM_KEY;
                                t.Transition(AfterDoctypeSystemKeyword);
                            }
                            else {
                                t.Error(this);
                                t.doctypePending.forceQuirks = true;
                                t.AdvanceTransition(BogusDoctype);
                            }
                        }
                    }
                }
            }
//\endcond
        }

        private sealed class AfterDocTypePublicKeywordTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypePublicKeyword";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeDoctypePublicIdentifier);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // set public id to empty string
                        t.Transition(DoctypePublicIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // set public id to empty string
                        t.Transition(DoctypePublicIdentifier_singleQuoted);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(BogusDoctype);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class BeforeDocTypePublicIdentifierTS : TokeniserState {
            public override String ToString() {
                return "BeforeDoctypePublicIdentifier";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        break;
                    }

                    case '"': {
                        // set public id to empty string
                        t.Transition(DoctypePublicIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        // set public id to empty string
                        t.Transition(DoctypePublicIdentifier_singleQuoted);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(BogusDoctype);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class DocTypePublicIdentifierDoubleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypePublicIdentifier_doubleQuoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '"': {
                        t.Transition(AfterDoctypePublicIdentifier);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.doctypePending.publicIdentifier.Append(replacementChar);
                        break;
                    }

                    default: {
                        t.doctypePending.publicIdentifier.Append(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class DocTypePublicIdentifierSingleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypePublicIdentifier_singleQuoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\'': {
                        t.Transition(AfterDoctypePublicIdentifier);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.doctypePending.publicIdentifier.Append(replacementChar);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.doctypePending.publicIdentifier.Append(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AfterDocTypePublicIdentifierTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypePublicIdentifier";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BetweenDoctypePublicAndSystemIdentifiers);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // system id empty
                        t.Transition(DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // system id empty
                        t.Transition(DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(BogusDoctype);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class BetweenDocTypePublicAndSystemIdentifiersTS : TokeniserState {
            public override String ToString() {
                return "BetweenDoctypePublicAndSystemIdentifiers";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        break;
                    }

                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // system id empty
                        t.Transition(DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // system id empty
                        t.Transition(DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(BogusDoctype);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AfterDocTypeSystemKeywordTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypeSystemKeyword";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(BeforeDoctypeSystemIdentifier);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // system id empty
                        t.Transition(DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // system id empty
                        t.Transition(DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class BeforeDocTypeSystemIdentifierTS : TokeniserState {
            public override String ToString() {
                return "BeforeDoctypeSystemIdentifier";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        break;
                    }

                    case '"': {
                        // set system id to empty string
                        t.Transition(DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        // set public id to empty string
                        t.Transition(DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(BogusDoctype);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class DocTypeSystemIdentifierDoubleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypeSystemIdentifier_doubleQuoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.doctypePending.systemIdentifier.Append(replacementChar);
                        break;
                    }

                    case '"': {
                        t.Transition(AfterDoctypeSystemIdentifier);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.doctypePending.systemIdentifier.Append(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class DocTypeSystemIdentifierSingleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypeSystemIdentifier_singleQuoted";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\'': {
                        t.Transition(AfterDoctypeSystemIdentifier);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case nullChar: {
                        t.Error(this);
                        t.doctypePending.systemIdentifier.Append(replacementChar);
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.doctypePending.systemIdentifier.Append(c);
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class AfterDocTypeSystemIdentifierTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypeSystemIdentifier";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        break;
                    }

                    case eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.Transition(BogusDoctype);
                        break;
                    }
                }
            }
//\endcond
            // NOT force quirks
        }

        private sealed class BogusDocTypeTS : TokeniserState {
            public override String ToString() {
                return "BogusDoctype";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    case eof: {
                        t.EmitDoctypePending();
                        t.Transition(Data);
                        break;
                    }

                    default: {
                        // ignore char
                        break;
                    }
                }
            }
//\endcond
        }

        private sealed class CDataSectionTS : TokeniserState {
            public override String ToString() {
                return "CdataSection";
            }

//\cond DO_NOT_DOCUMENT
            internal override void Read(Tokeniser t, CharacterReader r) {
                String data = r.ConsumeTo("]]>");
                t.dataBuffer.Append(data);
                if (r.MatchConsume("]]>") || r.IsEmpty()) {
                    t.Emit(new Token.CData(t.dataBuffer.ToString()));
                    t.Transition(Data);
                }
            }
//\endcond
            // otherwise, buffer underrun, stay in data section
        }
    }
//\endcond
}
