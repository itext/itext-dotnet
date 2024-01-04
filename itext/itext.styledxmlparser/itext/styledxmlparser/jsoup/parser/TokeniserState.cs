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
    /// <summary>States and transition activations for the Tokeniser.</summary>
    internal abstract class TokeniserState {
        internal static TokeniserState Data = new TokeniserState.DataTS();

        internal static TokeniserState CharacterReferenceInData = new TokeniserState.CharacterReferenceInDataTS();

        internal static TokeniserState Rcdata = new TokeniserState.RcDataTS();

        internal static TokeniserState CharacterReferenceInRcdata = new TokeniserState.CharacterReferenceInRcdataTS
            ();

        internal static TokeniserState Rawtext = new TokeniserState.RawTextTS();

        internal static TokeniserState ScriptData = new TokeniserState.ScriptDataTS();

        internal static TokeniserState PLAINTEXT = new TokeniserState.PlainTextTS();

        internal static TokeniserState TagOpen = new TokeniserState.TagOpenTS();

        internal static TokeniserState EndTagOpen = new TokeniserState.EndTagOpenTS();

        internal static TokeniserState TagName = new TokeniserState.TagNameTS();

        internal static TokeniserState RcdataLessthanSign = new TokeniserState.RcDataLessThanSignTS();

        internal static TokeniserState RCDATAEndTagOpen = new TokeniserState.RcDataEndTagOpenTS();

        internal static TokeniserState RCDATAEndTagName = new TokeniserState.RcDataEndTagNameTS();

        internal static TokeniserState RawtextLessthanSign = new TokeniserState.RawTextLessThanSignTS();

        internal static TokeniserState RawtextEndTagOpen = new TokeniserState.RawTextEndTagOpenTS();

        internal static TokeniserState RawtextEndTagName = new TokeniserState.RawTextEndTagNameTS();

        internal static TokeniserState ScriptDataLessthanSign = new TokeniserState.ScriptDataLessThanSignTS();

        internal static TokeniserState ScriptDataEndTagOpen = new TokeniserState.ScriptDataEndTagOpenTS();

        internal static TokeniserState ScriptDataEndTagName = new TokeniserState.ScriptDataEndTagNameTS();

        internal static TokeniserState ScriptDataEscapeStart = new TokeniserState.ScriptDataEscapeStartTS();

        internal static TokeniserState ScriptDataEscapeStartDash = new TokeniserState.ScriptDataEscapeStartDashTS(
            );

        internal static TokeniserState ScriptDataEscaped = new TokeniserState.ScriptDataEscapedTS();

        internal static TokeniserState ScriptDataEscapedDash = new TokeniserState.ScriptDataEscapedDashTS();

        internal static TokeniserState ScriptDataEscapedDashDash = new TokeniserState.ScriptDataEscapedDashDashTS(
            );

        internal static TokeniserState ScriptDataEscapedLessthanSign = new TokeniserState.ScriptDataEscapedLessThanSignTS
            ();

        internal static TokeniserState ScriptDataEscapedEndTagOpen = new TokeniserState.ScriptDataEscapedEndTagOpenTS
            ();

        internal static TokeniserState ScriptDataEscapedEndTagName = new TokeniserState.ScriptDataEscapedEndTagNameTS
            ();

        internal static TokeniserState ScriptDataDoubleEscapeStart = new TokeniserState.ScriptDataDoubleEscapeStartTS
            ();

        internal static TokeniserState ScriptDataDoubleEscaped = new TokeniserState.ScriptDataDoubleEscapedTS();

        internal static TokeniserState ScriptDataDoubleEscapedDash = new TokeniserState.ScriptDataDoubleEscapedDashTS
            ();

        internal static TokeniserState ScriptDataDoubleEscapedDashDash = new TokeniserState.ScriptDataDoubleEscapedDashDashTS
            ();

        internal static TokeniserState ScriptDataDoubleEscapedLessthanSign = new TokeniserState.ScriptDataDoubleEscapedLessThanSignTS
            ();

        internal static TokeniserState ScriptDataDoubleEscapeEnd = new TokeniserState.ScriptDataDoubleEscapeEndTS(
            );

        internal static TokeniserState BeforeAttributeName = new TokeniserState.BeforeAttributeNameTS();

        internal static TokeniserState AttributeName = new TokeniserState.AttributeNameTS();

        internal static TokeniserState AfterAttributeName = new TokeniserState.AfterAttributeNameTS();

        internal static TokeniserState BeforeAttributeValue = new TokeniserState.BeforeAttributeValueTS();

        internal static TokeniserState AttributeValue_doubleQuoted = new TokeniserState.AttributeValueDoubleQuotedTS
            ();

        internal static TokeniserState AttributeValue_singleQuoted = new TokeniserState.AttributeValueSingleQuotedTS
            ();

        internal static TokeniserState AttributeValue_unquoted = new TokeniserState.AttributeValueUnquotedTS();

        // CharacterReferenceInAttributeValue state handled inline
        internal static TokeniserState AfterAttributeValue_quoted = new TokeniserState.AfterAttributeValueQuotedTS
            ();

        internal static TokeniserState SelfClosingStartTag = new TokeniserState.SelfClosingStartTagTS();

        internal static TokeniserState BogusComment = new TokeniserState.BogusCommentTS();

        internal static TokeniserState MarkupDeclarationOpen = new TokeniserState.MarkupDeclarationOpenTS();

        internal static TokeniserState CommentStart = new TokeniserState.CommentStartTS();

        internal static TokeniserState CommentStartDash = new TokeniserState.CommentStartDashTS();

        internal static TokeniserState Comment = new TokeniserState.CommentTS();

        internal static TokeniserState CommentEndDash = new TokeniserState.CommentEndDashTS();

        internal static TokeniserState CommentEnd = new TokeniserState.CommentEndTS();

        internal static TokeniserState CommentEndBang = new TokeniserState.CommentEndBangTS();

        internal static TokeniserState Doctype = new TokeniserState.DocTypeTS();

        internal static TokeniserState BeforeDoctypeName = new TokeniserState.BeforeDocTypeNameTS();

        internal static TokeniserState DoctypeName = new TokeniserState.DocTypeNameTS();

        internal static TokeniserState AfterDoctypeName = new TokeniserState.AfterDocTypeNameTS();

        internal static TokeniserState AfterDoctypePublicKeyword = new TokeniserState.AfterDocTypePublicKeywordTS(
            );

        internal static TokeniserState BeforeDoctypePublicIdentifier = new TokeniserState.BeforeDocTypePublicIdentifierTS
            ();

        internal static TokeniserState DoctypePublicIdentifier_doubleQuoted = new TokeniserState.DocTypePublicIdentifierDoubleQuotedTS
            ();

        internal static TokeniserState DoctypePublicIdentifier_singleQuoted = new TokeniserState.DocTypePublicIdentifierSingleQuotedTS
            ();

        internal static TokeniserState AfterDoctypePublicIdentifier = new TokeniserState.AfterDocTypePublicIdentifierTS
            ();

        internal static TokeniserState BetweenDoctypePublicAndSystemIdentifiers = new TokeniserState.BetweenDocTypePublicAndSystemIdentifiersTS
            ();

        internal static TokeniserState AfterDoctypeSystemKeyword = new TokeniserState.AfterDocTypeSystemKeywordTS(
            );

        internal static TokeniserState BeforeDoctypeSystemIdentifier = new TokeniserState.BeforeDocTypeSystemIdentifierTS
            ();

        internal static TokeniserState DoctypeSystemIdentifier_doubleQuoted = new TokeniserState.DocTypeSystemIdentifierDoubleQuotedTS
            ();

        internal static TokeniserState DoctypeSystemIdentifier_singleQuoted = new TokeniserState.DocTypeSystemIdentifierSingleQuotedTS
            ();

        internal static TokeniserState AfterDoctypeSystemIdentifier = new TokeniserState.AfterDocTypeSystemIdentifierTS
            ();

        internal static TokeniserState BogusDoctype = new TokeniserState.BogusDocTypeTS();

        internal static TokeniserState CdataSection = new TokeniserState.CDataSectionTS();

        internal abstract void Read(Tokeniser t, CharacterReader r);

        internal const char nullChar = '\u0000';

        // char searches. must be sorted, used in inSorted. MUST update TokenisetStateTest if more arrays are added.
        internal static readonly char[] attributeNameCharsSorted = new char[] { nullChar, '\t', '\n', '\f', '\r', 
            ' ', '"', '\'', '/', '<', '=', '>' };

        internal static readonly char[] attributeValueUnquoted = new char[] { nullChar, '\t', '\n', '\f', '\r', ' '
            , '"', '&', '\'', '<', '=', '>', '`' };

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
        }

        private sealed class CharacterReferenceInDataTS : TokeniserState {
            public override String ToString() {
                return "CharacterReferenceInData";
            }

            // from & in data
            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadCharRef(t, Data);
            }
        }

        private sealed class RcDataTS : TokeniserState {
            public override String ToString() {
                return "Rcdata";
            }

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
        }

        private sealed class CharacterReferenceInRcdataTS : TokeniserState {
            public override String ToString() {
                return "CharacterReferenceInRcdata";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadCharRef(t, Rcdata);
            }
        }

        private sealed class RawTextTS : TokeniserState {
            public override String ToString() {
                return "Rawtext";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadRawData(t, r, this, RawtextLessthanSign);
            }
        }

        private sealed class ScriptDataTS : TokeniserState {
            public override String ToString() {
                return "ScriptData";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadRawData(t, r, this, ScriptDataLessthanSign);
            }
        }

        private sealed class PlainTextTS : TokeniserState {
            public override String ToString() {
                return "PLAINTEXT";
            }

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
        }

        private sealed class TagOpenTS : TokeniserState {
            public override String ToString() {
                return "TagOpen";
            }

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
        }

        private sealed class EndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "EndTagOpen";
            }

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
        }

        private sealed class TagNameTS : TokeniserState {
            public override String ToString() {
                return "TagName";
            }

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
        }

        private sealed class RcDataLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "RcdataLessthanSign";
            }

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
        }

        private sealed class RcDataEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "RCDATAEndTagOpen";
            }

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
        }

        private sealed class RcDataEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "RCDATAEndTagName";
            }

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
        }

        private sealed class RawTextEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "RawtextEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadEndTag(t, r, RawtextEndTagName, Rawtext);
            }
        }

        private sealed class RawTextEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "RawtextEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataEndTag(t, r, Rawtext);
            }
        }

        private sealed class ScriptDataLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataLessthanSign";
            }

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
        }

        private sealed class ScriptDataEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                ReadEndTag(t, r, ScriptDataEndTagName, ScriptData);
            }
        }

        private sealed class ScriptDataEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataEndTag(t, r, ScriptData);
            }
        }

        private sealed class ScriptDataEscapeStartTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapeStart";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('-')) {
                    t.Emit('-');
                    t.AdvanceTransition(ScriptDataEscapeStartDash);
                }
                else {
                    t.Transition(ScriptData);
                }
            }
        }

        private sealed class ScriptDataEscapeStartDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapeStartDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('-')) {
                    t.Emit('-');
                    t.AdvanceTransition(ScriptDataEscapedDashDash);
                }
                else {
                    t.Transition(ScriptData);
                }
            }
        }

        private sealed class ScriptDataEscapedTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscaped";
            }

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
        }

        private sealed class ScriptDataEscapedDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedDash";
            }

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
        }

        private sealed class ScriptDataEscapedDashDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedDashDash";
            }

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
        }

        private sealed class ScriptDataEscapedLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedLessthanSign";
            }

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
        }

        private sealed class ScriptDataEscapedEndTagOpenTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedEndTagOpen";
            }

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
        }

        private sealed class ScriptDataEscapedEndTagNameTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataEscapedEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataEndTag(t, r, ScriptDataEscaped);
            }
        }

        private sealed class ScriptDataDoubleEscapeStartTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapeStart";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataDoubleEscapeTag(t, r, ScriptDataDoubleEscaped, ScriptDataEscaped);
            }
        }

        private sealed class ScriptDataDoubleEscapedTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscaped";
            }

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
        }

        private sealed class ScriptDataDoubleEscapedDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapedDash";
            }

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
        }

        private sealed class ScriptDataDoubleEscapedDashDashTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapedDashDash";
            }

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
        }

        private sealed class ScriptDataDoubleEscapedLessThanSignTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapedLessthanSign";
            }

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
        }

        private sealed class ScriptDataDoubleEscapeEndTS : TokeniserState {
            public override String ToString() {
                return "ScriptDataDoubleEscapeEnd";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                HandleDataDoubleEscapeTag(t, r, ScriptDataEscaped, ScriptDataDoubleEscaped);
            }
        }

        private sealed class BeforeAttributeNameTS : TokeniserState {
            public override String ToString() {
                return "BeforeAttributeName";
            }

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
        }

        private sealed class AttributeNameTS : TokeniserState {
            public override String ToString() {
                return "AttributeName";
            }

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
        }

        private sealed class AfterAttributeNameTS : TokeniserState {
            public override String ToString() {
                return "AfterAttributeName";
            }

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
        }

        private sealed class BeforeAttributeValueTS : TokeniserState {
            public override String ToString() {
                return "BeforeAttributeValue";
            }

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
        }

        private sealed class AttributeValueDoubleQuotedTS : TokeniserState {
            public override String ToString() {
                return "AttributeValue_doubleQuoted";
            }

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
        }

        private sealed class AttributeValueSingleQuotedTS : TokeniserState {
            public override String ToString() {
                return "AttributeValue_singleQuoted";
            }

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
        }

        private sealed class AttributeValueUnquotedTS : TokeniserState {
            public override String ToString() {
                return "AttributeValue_unquoted";
            }

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
        }

        private sealed class AfterAttributeValueQuotedTS : TokeniserState {
            public override String ToString() {
                return "AfterAttributeValue_quoted";
            }

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
        }

        private sealed class SelfClosingStartTagTS : TokeniserState {
            public override String ToString() {
                return "SelfClosingStartTag";
            }

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
        }

        private sealed class BogusCommentTS : TokeniserState {
            public override String ToString() {
                return "BogusComment";
            }

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
        }

        private sealed class MarkupDeclarationOpenTS : TokeniserState {
            public override String ToString() {
                return "MarkupDeclarationOpen";
            }

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
            // advance so this character gets in bogus comment data's rewind
        }

        private sealed class CommentStartTS : TokeniserState {
            public override String ToString() {
                return "CommentStart";
            }

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
        }

        private sealed class CommentStartDashTS : TokeniserState {
            public override String ToString() {
                return "CommentStartDash";
            }

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
        }

        private sealed class CommentTS : TokeniserState {
            public override String ToString() {
                return "Comment";
            }

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
        }

        private sealed class CommentEndDashTS : TokeniserState {
            public override String ToString() {
                return "CommentEndDash";
            }

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
        }

        private sealed class CommentEndTS : TokeniserState {
            public override String ToString() {
                return "CommentEnd";
            }

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
        }

        private sealed class CommentEndBangTS : TokeniserState {
            public override String ToString() {
                return "CommentEndBang";
            }

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
        }

        private sealed class DocTypeTS : TokeniserState {
            public override String ToString() {
                return "Doctype";
            }

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
        }

        private sealed class BeforeDocTypeNameTS : TokeniserState {
            public override String ToString() {
                return "BeforeDoctypeName";
            }

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
        }

        private sealed class DocTypeNameTS : TokeniserState {
            public override String ToString() {
                return "DoctypeName";
            }

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
        }

        private sealed class AfterDocTypeNameTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypeName";
            }

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
        }

        private sealed class AfterDocTypePublicKeywordTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypePublicKeyword";
            }

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
        }

        private sealed class BeforeDocTypePublicIdentifierTS : TokeniserState {
            public override String ToString() {
                return "BeforeDoctypePublicIdentifier";
            }

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
        }

        private sealed class DocTypePublicIdentifierDoubleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypePublicIdentifier_doubleQuoted";
            }

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
        }

        private sealed class DocTypePublicIdentifierSingleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypePublicIdentifier_singleQuoted";
            }

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
        }

        private sealed class AfterDocTypePublicIdentifierTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypePublicIdentifier";
            }

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
        }

        private sealed class BetweenDocTypePublicAndSystemIdentifiersTS : TokeniserState {
            public override String ToString() {
                return "BetweenDoctypePublicAndSystemIdentifiers";
            }

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
        }

        private sealed class AfterDocTypeSystemKeywordTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypeSystemKeyword";
            }

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
        }

        private sealed class BeforeDocTypeSystemIdentifierTS : TokeniserState {
            public override String ToString() {
                return "BeforeDoctypeSystemIdentifier";
            }

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
        }

        private sealed class DocTypeSystemIdentifierDoubleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypeSystemIdentifier_doubleQuoted";
            }

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
        }

        private sealed class DocTypeSystemIdentifierSingleQuotedTS : TokeniserState {
            public override String ToString() {
                return "DoctypeSystemIdentifier_singleQuoted";
            }

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
        }

        private sealed class AfterDocTypeSystemIdentifierTS : TokeniserState {
            public override String ToString() {
                return "AfterDoctypeSystemIdentifier";
            }

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
            // NOT force quirks
        }

        private sealed class BogusDocTypeTS : TokeniserState {
            public override String ToString() {
                return "BogusDoctype";
            }

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
        }

        private sealed class CDataSectionTS : TokeniserState {
            public override String ToString() {
                return "CdataSection";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                String data = r.ConsumeTo("]]>");
                t.dataBuffer.Append(data);
                if (r.MatchConsume("]]>") || r.IsEmpty()) {
                    t.Emit(new Token.CData(t.dataBuffer.ToString()));
                    t.Transition(Data);
                }
            }
            // otherwise, buffer underrun, stay in data section
        }
    }
}
