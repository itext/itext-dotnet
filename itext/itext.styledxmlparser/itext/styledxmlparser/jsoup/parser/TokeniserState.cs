/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>States and transition activations for the Tokeniser.</summary>
    internal abstract class TokeniserState {
        private sealed class _TokeniserState_31 : TokeniserState {
            public _TokeniserState_31() {
            }

            public override String ToString() {
                return "Data";
            }

            // in data state, gather characters until a character reference or tag is found
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case '&': {
                        t.AdvanceTransition(TokeniserState.CharacterReferenceInData);
                        break;
                    }

                    case '<': {
                        t.AdvanceTransition(TokeniserState.TagOpen);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        // NOT replacement character (oddly?)
                        t.Emit(r.Consume());
                        break;
                    }

                    case TokeniserState.eof: {
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

        internal static TokeniserState Data = new _TokeniserState_31();

        private sealed class _TokeniserState_62 : TokeniserState {
            public _TokeniserState_62() {
            }

            public override String ToString() {
                return "CharacterReferenceInData";
            }

            // from & in data
            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadCharRef(t, TokeniserState.Data);
            }
        }

        internal static TokeniserState CharacterReferenceInData = new _TokeniserState_62();

        private sealed class _TokeniserState_75 : TokeniserState {
            public _TokeniserState_75() {
            }

            public override String ToString() {
                return "Rcdata";
            }

            /// handles data in title, textarea etc
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case '&': {
                        t.AdvanceTransition(TokeniserState.CharacterReferenceInRcdata);
                        break;
                    }

                    case '<': {
                        t.AdvanceTransition(TokeniserState.RcdataLessthanSign);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
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

        internal static TokeniserState Rcdata = new _TokeniserState_75();

        private sealed class _TokeniserState_107 : TokeniserState {
            public _TokeniserState_107() {
            }

            public override String ToString() {
                return "CharacterReferenceInRcdata";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadCharRef(t, TokeniserState.Rcdata);
            }
        }

        internal static TokeniserState CharacterReferenceInRcdata = new _TokeniserState_107();

        private sealed class _TokeniserState_119 : TokeniserState {
            public _TokeniserState_119() {
            }

            public override String ToString() {
                return "Rawtext";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadRawData(t, r, this, TokeniserState.RawtextLessthanSign);
            }
        }

        internal static TokeniserState Rawtext = new _TokeniserState_119();

        private sealed class _TokeniserState_131 : TokeniserState {
            public _TokeniserState_131() {
            }

            public override String ToString() {
                return "ScriptData";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadRawData(t, r, this, TokeniserState.ScriptDataLessthanSign);
            }
        }

        internal static TokeniserState ScriptData = new _TokeniserState_131();

        private sealed class _TokeniserState_143 : TokeniserState {
            public _TokeniserState_143() {
            }

            public override String ToString() {
                return "PLAINTEXT";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case TokeniserState.nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.Emit(new Token.EOF());
                        break;
                    }

                    default: {
                        String data = r.ConsumeTo(TokeniserState.nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState PLAINTEXT = new _TokeniserState_143();

        private sealed class _TokeniserState_168 : TokeniserState {
            public _TokeniserState_168() {
            }

            public override String ToString() {
                return "TagOpen";
            }

            // from < in data
            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Current()) {
                    case '!': {
                        t.AdvanceTransition(TokeniserState.MarkupDeclarationOpen);
                        break;
                    }

                    case '/': {
                        t.AdvanceTransition(TokeniserState.EndTagOpen);
                        break;
                    }

                    case '?': {
                        t.CreateBogusCommentPending();
                        t.AdvanceTransition(TokeniserState.BogusComment);
                        break;
                    }

                    default: {
                        if (r.MatchesLetter()) {
                            t.CreateTagPending(true);
                            t.Transition(TokeniserState.TagName);
                        }
                        else {
                            t.Error(this);
                            t.Emit('<');
                            // char that got us here
                            t.Transition(TokeniserState.Data);
                        }
                        break;
                    }
                }
            }
        }

        internal static TokeniserState TagOpen = new _TokeniserState_168();

        private sealed class _TokeniserState_202 : TokeniserState {
            public _TokeniserState_202() {
            }

            public override String ToString() {
                return "EndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Emit("</");
                    t.Transition(TokeniserState.Data);
                }
                else {
                    if (r.MatchesLetter()) {
                        t.CreateTagPending(false);
                        t.Transition(TokeniserState.TagName);
                    }
                    else {
                        if (r.Matches('>')) {
                            t.Error(this);
                            t.AdvanceTransition(TokeniserState.Data);
                        }
                        else {
                            t.Error(this);
                            t.CreateBogusCommentPending();
                            t.AdvanceTransition(TokeniserState.BogusComment);
                        }
                    }
                }
            }
        }

        internal static TokeniserState EndTagOpen = new _TokeniserState_202();

        private sealed class _TokeniserState_228 : TokeniserState {
            public _TokeniserState_228() {
            }

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
                        t.Transition(TokeniserState.BeforeAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(TokeniserState.SelfClosingStartTag);
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
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        // replacement
                        t.tagPending.AppendTagName(TokeniserState.replacementStr);
                        break;
                    }

                    case TokeniserState.eof: {
                        // should emit pending tag?
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
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

        internal static TokeniserState TagName = new _TokeniserState_228();

        private sealed class _TokeniserState_275 : TokeniserState {
            public _TokeniserState_275() {
            }

            public override String ToString() {
                return "RcdataLessthanSign";
            }

            // from < in rcdata
            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('/')) {
                    t.CreateTempBuffer();
                    t.AdvanceTransition(TokeniserState.RCDATAEndTagOpen);
                }
                else {
                    if (r.MatchesLetter() && t.AppropriateEndTagName() != null && !r.ContainsIgnoreCase("</" + t.AppropriateEndTagName
                        ())) {
                        // diverge from spec: got a start tag, but there's no appropriate end tag (</title>), so rather than
                        // consuming to EOF; break out here
                        t.tagPending = t.CreateTagPending(false).Name(t.AppropriateEndTagName());
                        t.EmitTagPending();
                        t.Transition(TokeniserState.TagOpen);
                    }
                    else {
                        // straight into TagOpen, as we came from < and looks like we're on a start tag
                        t.Emit("<");
                        t.Transition(TokeniserState.Rcdata);
                    }
                }
            }
        }

        internal static TokeniserState RcdataLessthanSign = new _TokeniserState_275();

        private sealed class _TokeniserState_300 : TokeniserState {
            public _TokeniserState_300() {
            }

            public override String ToString() {
                return "RCDATAEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTagPending(false);
                    t.tagPending.AppendTagName(r.Current());
                    t.dataBuffer.Append(r.Current());
                    t.AdvanceTransition(TokeniserState.RCDATAEndTagName);
                }
                else {
                    t.Emit("</");
                    t.Transition(TokeniserState.Rcdata);
                }
            }
        }

        internal static TokeniserState RCDATAEndTagOpen = new _TokeniserState_300();

        private sealed class _TokeniserState_320 : TokeniserState {
            public _TokeniserState_320() {
            }

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
                            t.Transition(TokeniserState.BeforeAttributeName);
                        }
                        else {
                            this.AnythingElse(t, r);
                        }
                        break;
                    }

                    case '/': {
                        if (t.IsAppropriateEndTagToken()) {
                            t.Transition(TokeniserState.SelfClosingStartTag);
                        }
                        else {
                            this.AnythingElse(t, r);
                        }
                        break;
                    }

                    case '>': {
                        if (t.IsAppropriateEndTagToken()) {
                            t.EmitTagPending();
                            t.Transition(TokeniserState.Data);
                        }
                        else {
                            this.AnythingElse(t, r);
                        }
                        break;
                    }

                    default: {
                        this.AnythingElse(t, r);
                        break;
                    }
                }
            }

            private void AnythingElse(Tokeniser t, CharacterReader r) {
                t.Emit("</");
                t.Emit(t.dataBuffer);
                r.Unconsume();
                t.Transition(TokeniserState.Rcdata);
            }
        }

        internal static TokeniserState RCDATAEndTagName = new _TokeniserState_320();

        private sealed class _TokeniserState_374 : TokeniserState {
            public _TokeniserState_374() {
            }

            public override String ToString() {
                return "RawtextLessthanSign";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('/')) {
                    t.CreateTempBuffer();
                    t.AdvanceTransition(TokeniserState.RawtextEndTagOpen);
                }
                else {
                    t.Emit('<');
                    t.Transition(TokeniserState.Rawtext);
                }
            }
        }

        internal static TokeniserState RawtextLessthanSign = new _TokeniserState_374();

        private sealed class _TokeniserState_392 : TokeniserState {
            public _TokeniserState_392() {
            }

            public override String ToString() {
                return "RawtextEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadEndTag(t, r, TokeniserState.RawtextEndTagName, TokeniserState.Rawtext);
            }
        }

        internal static TokeniserState RawtextEndTagOpen = new _TokeniserState_392();

        private sealed class _TokeniserState_404 : TokeniserState {
            public _TokeniserState_404() {
            }

            public override String ToString() {
                return "RawtextEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataEndTag(t, r, TokeniserState.Rawtext);
            }
        }

        internal static TokeniserState RawtextEndTagName = new _TokeniserState_404();

        private sealed class _TokeniserState_416 : TokeniserState {
            public _TokeniserState_416() {
            }

            public override String ToString() {
                return "ScriptDataLessthanSign";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                switch (r.Consume()) {
                    case '/': {
                        t.CreateTempBuffer();
                        t.Transition(TokeniserState.ScriptDataEndTagOpen);
                        break;
                    }

                    case '!': {
                        t.Emit("<!");
                        t.Transition(TokeniserState.ScriptDataEscapeStart);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.Emit("<");
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Emit("<");
                        r.Unconsume();
                        t.Transition(TokeniserState.ScriptData);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataLessthanSign = new _TokeniserState_416();

        private sealed class _TokeniserState_446 : TokeniserState {
            public _TokeniserState_446() {
            }

            public override String ToString() {
                return "ScriptDataEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadEndTag(t, r, TokeniserState.ScriptDataEndTagName, TokeniserState.ScriptData);
            }
        }

        internal static TokeniserState ScriptDataEndTagOpen = new _TokeniserState_446();

        private sealed class _TokeniserState_458 : TokeniserState {
            public _TokeniserState_458() {
            }

            public override String ToString() {
                return "ScriptDataEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataEndTag(t, r, TokeniserState.ScriptData);
            }
        }

        internal static TokeniserState ScriptDataEndTagName = new _TokeniserState_458();

        private sealed class _TokeniserState_470 : TokeniserState {
            public _TokeniserState_470() {
            }

            public override String ToString() {
                return "ScriptDataEscapeStart";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('-')) {
                    t.Emit('-');
                    t.AdvanceTransition(TokeniserState.ScriptDataEscapeStartDash);
                }
                else {
                    t.Transition(TokeniserState.ScriptData);
                }
            }
        }

        internal static TokeniserState ScriptDataEscapeStart = new _TokeniserState_470();

        private sealed class _TokeniserState_487 : TokeniserState {
            public _TokeniserState_487() {
            }

            public override String ToString() {
                return "ScriptDataEscapeStartDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('-')) {
                    t.Emit('-');
                    t.AdvanceTransition(TokeniserState.ScriptDataEscapedDashDash);
                }
                else {
                    t.Transition(TokeniserState.ScriptData);
                }
            }
        }

        internal static TokeniserState ScriptDataEscapeStartDash = new _TokeniserState_487();

        private sealed class _TokeniserState_504 : TokeniserState {
            public _TokeniserState_504() {
            }

            public override String ToString() {
                return "ScriptDataEscaped";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Transition(TokeniserState.Data);
                    return;
                }
                switch (r.Current()) {
                    case '-': {
                        t.Emit('-');
                        t.AdvanceTransition(TokeniserState.ScriptDataEscapedDash);
                        break;
                    }

                    case '<': {
                        t.AdvanceTransition(TokeniserState.ScriptDataEscapedLessthanSign);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(TokeniserState.replacementChar);
                        break;
                    }

                    default: {
                        String data = r.ConsumeToAny('-', '<', TokeniserState.nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataEscaped = new _TokeniserState_504();

        private sealed class _TokeniserState_538 : TokeniserState {
            public _TokeniserState_538() {
            }

            public override String ToString() {
                return "ScriptDataEscapedDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Transition(TokeniserState.Data);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataEscapedDashDash);
                        break;
                    }

                    case '<': {
                        t.Transition(TokeniserState.ScriptDataEscapedLessthanSign);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.Emit(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.ScriptDataEscaped);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataEscaped);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataEscapedDash = new _TokeniserState_538();

        private sealed class _TokeniserState_573 : TokeniserState {
            public _TokeniserState_573() {
            }

            public override String ToString() {
                return "ScriptDataEscapedDashDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.Transition(TokeniserState.Data);
                    return;
                }
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        break;
                    }

                    case '<': {
                        t.Transition(TokeniserState.ScriptDataEscapedLessthanSign);
                        break;
                    }

                    case '>': {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptData);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.Emit(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.ScriptDataEscaped);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataEscaped);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataEscapedDashDash = new _TokeniserState_573();

        private sealed class _TokeniserState_611 : TokeniserState {
            public _TokeniserState_611() {
            }

            public override String ToString() {
                return "ScriptDataEscapedLessthanSign";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTempBuffer();
                    t.dataBuffer.Append(r.Current());
                    t.Emit("<");
                    t.Emit(r.Current());
                    t.AdvanceTransition(TokeniserState.ScriptDataDoubleEscapeStart);
                }
                else {
                    if (r.Matches('/')) {
                        t.CreateTempBuffer();
                        t.AdvanceTransition(TokeniserState.ScriptDataEscapedEndTagOpen);
                    }
                    else {
                        t.Emit('<');
                        t.Transition(TokeniserState.ScriptDataEscaped);
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataEscapedLessthanSign = new _TokeniserState_611();

        private sealed class _TokeniserState_635 : TokeniserState {
            public _TokeniserState_635() {
            }

            public override String ToString() {
                return "ScriptDataEscapedEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTagPending(false);
                    t.tagPending.AppendTagName(r.Current());
                    t.dataBuffer.Append(r.Current());
                    t.AdvanceTransition(TokeniserState.ScriptDataEscapedEndTagName);
                }
                else {
                    t.Emit("</");
                    t.Transition(TokeniserState.ScriptDataEscaped);
                }
            }
        }

        internal static TokeniserState ScriptDataEscapedEndTagOpen = new _TokeniserState_635();

        private sealed class _TokeniserState_655 : TokeniserState {
            public _TokeniserState_655() {
            }

            public override String ToString() {
                return "ScriptDataEscapedEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataEndTag(t, r, TokeniserState.ScriptDataEscaped);
            }
        }

        internal static TokeniserState ScriptDataEscapedEndTagName = new _TokeniserState_655();

        private sealed class _TokeniserState_667 : TokeniserState {
            public _TokeniserState_667() {
            }

            public override String ToString() {
                return "ScriptDataDoubleEscapeStart";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataDoubleEscapeTag(t, r, TokeniserState.ScriptDataDoubleEscaped, TokeniserState.ScriptDataEscaped
                    );
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapeStart = new _TokeniserState_667();

        private sealed class _TokeniserState_679 : TokeniserState {
            public _TokeniserState_679() {
            }

            public override String ToString() {
                return "ScriptDataDoubleEscaped";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Current();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        t.AdvanceTransition(TokeniserState.ScriptDataDoubleEscapedDash);
                        break;
                    }

                    case '<': {
                        t.Emit(c);
                        t.AdvanceTransition(TokeniserState.ScriptDataDoubleEscapedLessthanSign);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.Emit(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        String data = r.ConsumeToAny('-', '<', TokeniserState.nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataDoubleEscaped = new _TokeniserState_679();

        private sealed class _TokeniserState_713 : TokeniserState {
            public _TokeniserState_713() {
            }

            public override String ToString() {
                return "ScriptDataDoubleEscapedDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataDoubleEscapedDashDash);
                        break;
                    }

                    case '<': {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataDoubleEscapedLessthanSign);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.Emit(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.ScriptDataDoubleEscaped);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataDoubleEscaped);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapedDash = new _TokeniserState_713();

        private sealed class _TokeniserState_747 : TokeniserState {
            public _TokeniserState_747() {
            }

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
                        t.Transition(TokeniserState.ScriptDataDoubleEscapedLessthanSign);
                        break;
                    }

                    case '>': {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptData);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.Emit(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.ScriptDataDoubleEscaped);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Emit(c);
                        t.Transition(TokeniserState.ScriptDataDoubleEscaped);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapedDashDash = new _TokeniserState_747();

        private sealed class _TokeniserState_784 : TokeniserState {
            public _TokeniserState_784() {
            }

            public override String ToString() {
                return "ScriptDataDoubleEscapedLessthanSign";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.Matches('/')) {
                    t.Emit('/');
                    t.CreateTempBuffer();
                    t.AdvanceTransition(TokeniserState.ScriptDataDoubleEscapeEnd);
                }
                else {
                    t.Transition(TokeniserState.ScriptDataDoubleEscaped);
                }
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapedLessthanSign = new _TokeniserState_784();

        private sealed class _TokeniserState_802 : TokeniserState {
            public _TokeniserState_802() {
            }

            public override String ToString() {
                return "ScriptDataDoubleEscapeEnd";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataDoubleEscapeTag(t, r, TokeniserState.ScriptDataEscaped, TokeniserState.ScriptDataDoubleEscaped
                    );
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapeEnd = new _TokeniserState_802();

        private sealed class _TokeniserState_814 : TokeniserState {
            public _TokeniserState_814() {
            }

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
                        t.Transition(TokeniserState.SelfClosingStartTag);
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
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        r.Unconsume();
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        t.Transition(TokeniserState.AttributeName);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '"':
                    case '\'':
                    case '=': {
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        t.tagPending.AppendAttributeName(c);
                        t.Transition(TokeniserState.AttributeName);
                        break;
                    }

                    default: {
                        // A-Z, anything else
                        t.tagPending.NewAttribute();
                        r.Unconsume();
                        t.Transition(TokeniserState.AttributeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BeforeAttributeName = new _TokeniserState_814();

        private sealed class _TokeniserState_868 : TokeniserState {
            public _TokeniserState_868() {
            }

            public override String ToString() {
                return "AttributeName";
            }

            // from before attribute name
            internal override void Read(Tokeniser t, CharacterReader r) {
                String name = r.ConsumeToAnySorted(TokeniserState.attributeNameCharsSorted);
                t.tagPending.AppendAttributeName(name);
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(TokeniserState.AfterAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(TokeniserState.SelfClosingStartTag);
                        break;
                    }

                    case '=': {
                        t.Transition(TokeniserState.BeforeAttributeValue);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeName(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
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

        internal static TokeniserState AttributeName = new _TokeniserState_868();

        private sealed class _TokeniserState_919 : TokeniserState {
            public _TokeniserState_919() {
            }

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
                        t.Transition(TokeniserState.SelfClosingStartTag);
                        break;
                    }

                    case '=': {
                        t.Transition(TokeniserState.BeforeAttributeValue);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeName(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.AttributeName);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '"':
                    case '\'':
                    case '<': {
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        t.tagPending.AppendAttributeName(c);
                        t.Transition(TokeniserState.AttributeName);
                        break;
                    }

                    default: {
                        // A-Z, anything else
                        t.tagPending.NewAttribute();
                        r.Unconsume();
                        t.Transition(TokeniserState.AttributeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState AfterAttributeName = new _TokeniserState_919();

        private sealed class _TokeniserState_971 : TokeniserState {
            public _TokeniserState_971() {
            }

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
                        t.Transition(TokeniserState.AttributeValue_doubleQuoted);
                        break;
                    }

                    case '&': {
                        r.Unconsume();
                        t.Transition(TokeniserState.AttributeValue_unquoted);
                        break;
                    }

                    case '\'': {
                        t.Transition(TokeniserState.AttributeValue_singleQuoted);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.AttributeValue_unquoted);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '<':
                    case '=':
                    case '`': {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(c);
                        t.Transition(TokeniserState.AttributeValue_unquoted);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Transition(TokeniserState.AttributeValue_unquoted);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BeforeAttributeValue = new _TokeniserState_971();

        private sealed class _TokeniserState_1027 : TokeniserState {
            public _TokeniserState_1027() {
            }

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
                    case '"': {
                        t.Transition(TokeniserState.AfterAttributeValue_quoted);
                        break;
                    }

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

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
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

        internal static TokeniserState AttributeValue_doubleQuoted = new _TokeniserState_1027();

        private sealed class _TokeniserState_1067 : TokeniserState {
            public _TokeniserState_1067() {
            }

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
                        t.Transition(TokeniserState.AfterAttributeValue_quoted);
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

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
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

        internal static TokeniserState AttributeValue_singleQuoted = new _TokeniserState_1067();

        private sealed class _TokeniserState_1107 : TokeniserState {
            public _TokeniserState_1107() {
            }

            public override String ToString() {
                return "AttributeValue_unquoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                String value = r.ConsumeToAnySorted(TokeniserState.attributeValueUnquoted);
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
                        t.Transition(TokeniserState.BeforeAttributeName);
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
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.AppendAttributeValue(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
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

        internal static TokeniserState AttributeValue_unquoted = new _TokeniserState_1107();

        private sealed class _TokeniserState_1162 : TokeniserState {
            public _TokeniserState_1162() {
            }

            // CharacterReferenceInAttributeValue state handled inline
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
                        t.Transition(TokeniserState.BeforeAttributeName);
                        break;
                    }

                    case '/': {
                        t.Transition(TokeniserState.SelfClosingStartTag);
                        break;
                    }

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Error(this);
                        t.Transition(TokeniserState.BeforeAttributeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState AfterAttributeValue_quoted = new _TokeniserState_1162();

        private sealed class _TokeniserState_1199 : TokeniserState {
            public _TokeniserState_1199() {
            }

            public override String ToString() {
                return "SelfClosingStartTag";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.tagPending.selfClosing = true;
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Error(this);
                        t.Transition(TokeniserState.BeforeAttributeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState SelfClosingStartTag = new _TokeniserState_1199();

        private sealed class _TokeniserState_1226 : TokeniserState {
            public _TokeniserState_1226() {
            }

            public override String ToString() {
                return "BogusComment";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                // rewind to capture character that lead us here
                r.Unconsume();
                t.commentPending.Append(r.ConsumeTo('>'));
                char next = r.Consume();
                if (next == '>' || next == TokeniserState.eof) {
                    t.EmitCommentPending();
                    t.Transition(TokeniserState.Data);
                }
            }
        }

        internal static TokeniserState BogusComment = new _TokeniserState_1226();

        private sealed class _TokeniserState_1245 : TokeniserState {
            public _TokeniserState_1245() {
            }

            public override String ToString() {
                return "MarkupDeclarationOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchConsume("--")) {
                    t.CreateCommentPending();
                    t.Transition(TokeniserState.CommentStart);
                }
                else {
                    if (r.MatchConsumeIgnoreCase("DOCTYPE")) {
                        t.Transition(TokeniserState.Doctype);
                    }
                    else {
                        if (r.MatchConsume("[CDATA[")) {
                            // is implemented properly, keep handling as cdata
                            //} else if (!t.currentNodeInHtmlNS() && r.matchConsume("[CDATA[")) {
                            t.CreateTempBuffer();
                            t.Transition(TokeniserState.CdataSection);
                        }
                        else {
                            t.Error(this);
                            t.CreateBogusCommentPending();
                            t.AdvanceTransition(TokeniserState.BogusComment);
                        }
                    }
                }
            }
        }

        internal static TokeniserState MarkupDeclarationOpen = new _TokeniserState_1245();

        private sealed class _TokeniserState_1271 : TokeniserState {
            public _TokeniserState_1271() {
            }

            // advance so this character gets in bogus comment data's rewind
            public override String ToString() {
                return "CommentStart";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Transition(TokeniserState.CommentStartDash);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.commentPending.Append(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        r.Unconsume();
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentStart = new _TokeniserState_1271();

        private sealed class _TokeniserState_1306 : TokeniserState {
            public _TokeniserState_1306() {
            }

            public override String ToString() {
                return "CommentStartDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Transition(TokeniserState.CommentStartDash);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.commentPending.Append(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentStartDash = new _TokeniserState_1306();

        private sealed class _TokeniserState_1341 : TokeniserState {
            public _TokeniserState_1341() {
            }

            public override String ToString() {
                return "Comment";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Current();
                switch (c) {
                    case '-': {
                        t.AdvanceTransition(TokeniserState.CommentEndDash);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        r.Advance();
                        t.commentPending.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append(r.ConsumeToAny('-', TokeniserState.nullChar));
                        break;
                    }
                }
            }
        }

        internal static TokeniserState Comment = new _TokeniserState_1341();

        private sealed class _TokeniserState_1370 : TokeniserState {
            public _TokeniserState_1370() {
            }

            public override String ToString() {
                return "CommentEndDash";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.Transition(TokeniserState.CommentEnd);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.commentPending.Append('-').Append(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append('-').Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentEndDash = new _TokeniserState_1370();

        private sealed class _TokeniserState_1400 : TokeniserState {
            public _TokeniserState_1400() {
            }

            public override String ToString() {
                return "CommentEnd";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.commentPending.Append("--").Append(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }

                    case '!': {
                        t.Error(this);
                        t.Transition(TokeniserState.CommentEndBang);
                        break;
                    }

                    case '-': {
                        t.Error(this);
                        t.commentPending.Append('-');
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.commentPending.Append("--").Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentEnd = new _TokeniserState_1400();

        private sealed class _TokeniserState_1440 : TokeniserState {
            public _TokeniserState_1440() {
            }

            public override String ToString() {
                return "CommentEndBang";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.commentPending.Append("--!");
                        t.Transition(TokeniserState.CommentEndDash);
                        break;
                    }

                    case '>': {
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.commentPending.Append("--!").Append(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.commentPending.Append("--!").Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentEndBang = new _TokeniserState_1440();

        private sealed class _TokeniserState_1475 : TokeniserState {
            public _TokeniserState_1475() {
            }

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
                        t.Transition(TokeniserState.BeforeDoctypeName);
                        break;
                    }

                    case TokeniserState.eof: {
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
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.Transition(TokeniserState.BeforeDoctypeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState Doctype = new _TokeniserState_1475();

        private sealed class _TokeniserState_1509 : TokeniserState {
            public _TokeniserState_1509() {
            }

            public override String ToString() {
                return "BeforeDoctypeName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateDoctypePending();
                    t.Transition(TokeniserState.DoctypeName);
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

                    case TokeniserState.nullChar: {
                        // ignore whitespace
                        t.Error(this);
                        t.CreateDoctypePending();
                        t.doctypePending.name.Append(TokeniserState.replacementChar);
                        t.Transition(TokeniserState.DoctypeName);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.CreateDoctypePending();
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.CreateDoctypePending();
                        t.doctypePending.name.Append(c);
                        t.Transition(TokeniserState.DoctypeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BeforeDoctypeName = new _TokeniserState_1509();

        private sealed class _TokeniserState_1551 : TokeniserState {
            public _TokeniserState_1551() {
            }

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
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(TokeniserState.AfterDoctypeName);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.doctypePending.name.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.doctypePending.name.Append(c);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState DoctypeName = new _TokeniserState_1551();

        private sealed class _TokeniserState_1593 : TokeniserState {
            public _TokeniserState_1593() {
            }

            public override String ToString() {
                return "AfterDoctypeName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.IsEmpty()) {
                    t.EofError(this);
                    t.doctypePending.forceQuirks = true;
                    t.EmitDoctypePending();
                    t.Transition(TokeniserState.Data);
                    return;
                }
                if (r.MatchesAny('\t', '\n', '\r', '\f', ' ')) {
                    r.Advance();
                }
                else {
                    // ignore whitespace
                    if (r.Matches('>')) {
                        t.EmitDoctypePending();
                        t.AdvanceTransition(TokeniserState.Data);
                    }
                    else {
                        if (r.MatchConsumeIgnoreCase(DocumentType.PUBLIC_KEY)) {
                            t.doctypePending.pubSysKey = DocumentType.PUBLIC_KEY;
                            t.Transition(TokeniserState.AfterDoctypePublicKeyword);
                        }
                        else {
                            if (r.MatchConsumeIgnoreCase(DocumentType.SYSTEM_KEY)) {
                                t.doctypePending.pubSysKey = DocumentType.SYSTEM_KEY;
                                t.Transition(TokeniserState.AfterDoctypeSystemKeyword);
                            }
                            else {
                                t.Error(this);
                                t.doctypePending.forceQuirks = true;
                                t.AdvanceTransition(TokeniserState.BogusDoctype);
                            }
                        }
                    }
                }
            }
        }

        internal static TokeniserState AfterDoctypeName = new _TokeniserState_1593();

        private sealed class _TokeniserState_1628 : TokeniserState {
            public _TokeniserState_1628() {
            }

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
                        t.Transition(TokeniserState.BeforeDoctypePublicIdentifier);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // set public id to empty string
                        t.Transition(TokeniserState.DoctypePublicIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // set public id to empty string
                        t.Transition(TokeniserState.DoctypePublicIdentifier_singleQuoted);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(TokeniserState.BogusDoctype);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState AfterDoctypePublicKeyword = new _TokeniserState_1628();

        private sealed class _TokeniserState_1675 : TokeniserState {
            public _TokeniserState_1675() {
            }

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
                        t.Transition(TokeniserState.DoctypePublicIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        // set public id to empty string
                        t.Transition(TokeniserState.DoctypePublicIdentifier_singleQuoted);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(TokeniserState.BogusDoctype);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BeforeDoctypePublicIdentifier = new _TokeniserState_1675();

        private sealed class _TokeniserState_1719 : TokeniserState {
            public _TokeniserState_1719() {
            }

            public override String ToString() {
                return "DoctypePublicIdentifier_doubleQuoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '"': {
                        t.Transition(TokeniserState.AfterDoctypePublicIdentifier);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.doctypePending.publicIdentifier.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.doctypePending.publicIdentifier.Append(c);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState DoctypePublicIdentifier_doubleQuoted = new _TokeniserState_1719();

        private sealed class _TokeniserState_1754 : TokeniserState {
            public _TokeniserState_1754() {
            }

            public override String ToString() {
                return "DoctypePublicIdentifier_singleQuoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\'': {
                        t.Transition(TokeniserState.AfterDoctypePublicIdentifier);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.doctypePending.publicIdentifier.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.doctypePending.publicIdentifier.Append(c);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState DoctypePublicIdentifier_singleQuoted = new _TokeniserState_1754();

        private sealed class _TokeniserState_1789 : TokeniserState {
            public _TokeniserState_1789() {
            }

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
                        t.Transition(TokeniserState.BetweenDoctypePublicAndSystemIdentifiers);
                        break;
                    }

                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // system id empty
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // system id empty
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(TokeniserState.BogusDoctype);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState AfterDoctypePublicIdentifier = new _TokeniserState_1789();

        private sealed class _TokeniserState_1834 : TokeniserState {
            public _TokeniserState_1834() {
            }

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
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // system id empty
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // system id empty
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(TokeniserState.BogusDoctype);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BetweenDoctypePublicAndSystemIdentifiers = new _TokeniserState_1834();

        private sealed class _TokeniserState_1878 : TokeniserState {
            public _TokeniserState_1878() {
            }

            public override String ToString() {
                return "AfterDoctypeSystemKeyword";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ': {
                        t.Transition(TokeniserState.BeforeDoctypeSystemIdentifier);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case '"': {
                        t.Error(this);
                        // system id empty
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        t.Error(this);
                        // system id empty
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
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

        internal static TokeniserState AfterDoctypeSystemKeyword = new _TokeniserState_1878();

        private sealed class _TokeniserState_1925 : TokeniserState {
            public _TokeniserState_1925() {
            }

            public override String ToString() {
                return "BeforeDoctypeSystemIdentifier";
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
                        // set system id to empty string
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_doubleQuoted);
                        break;
                    }

                    case '\'': {
                        // set public id to empty string
                        t.Transition(TokeniserState.DoctypeSystemIdentifier_singleQuoted);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.Transition(TokeniserState.BogusDoctype);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BeforeDoctypeSystemIdentifier = new _TokeniserState_1925();

        private sealed class _TokeniserState_1969 : TokeniserState {
            public _TokeniserState_1969() {
            }

            public override String ToString() {
                return "DoctypeSystemIdentifier_doubleQuoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '"': {
                        t.Transition(TokeniserState.AfterDoctypeSystemIdentifier);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.doctypePending.systemIdentifier.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.doctypePending.systemIdentifier.Append(c);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState DoctypeSystemIdentifier_doubleQuoted = new _TokeniserState_1969();

        private sealed class _TokeniserState_2004 : TokeniserState {
            public _TokeniserState_2004() {
            }

            public override String ToString() {
                return "DoctypeSystemIdentifier_singleQuoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '\'': {
                        t.Transition(TokeniserState.AfterDoctypeSystemIdentifier);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.doctypePending.systemIdentifier.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case '>': {
                        t.Error(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.doctypePending.systemIdentifier.Append(c);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState DoctypeSystemIdentifier_singleQuoted = new _TokeniserState_2004();

        private sealed class _TokeniserState_2039 : TokeniserState {
            public _TokeniserState_2039() {
            }

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

                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.doctypePending.forceQuirks = true;
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.Error(this);
                        t.Transition(TokeniserState.BogusDoctype);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState AfterDoctypeSystemIdentifier = new _TokeniserState_2039();

        private sealed class _TokeniserState_2073 : TokeniserState {
            public _TokeniserState_2073() {
            }

            // NOT force quirks
            public override String ToString() {
                return "BogusDoctype";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '>': {
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EmitDoctypePending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        // ignore char
                        break;
                    }
                }
            }
        }

        internal static TokeniserState BogusDoctype = new _TokeniserState_2073();

        private sealed class _TokeniserState_2098 : TokeniserState {
            public _TokeniserState_2098() {
            }

            public override String ToString() {
                return "CdataSection";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                String data = r.ConsumeTo("]]>");
                t.dataBuffer.Append(data);
                if (r.MatchConsume("]]>") || r.IsEmpty()) {
                    t.Emit(new Token.CData(t.dataBuffer.ToString()));
                    t.Transition(TokeniserState.Data);
                }
            }
        }

        internal static TokeniserState CdataSection = new _TokeniserState_2098();

        // otherwise, buffer underrun, stay in data section
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
    }
}
