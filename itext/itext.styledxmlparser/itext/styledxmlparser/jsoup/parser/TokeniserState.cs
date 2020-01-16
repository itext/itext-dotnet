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
using iText.IO.Util;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>States and transition activations for the Tokeniser.</summary>
    internal abstract class TokeniserState {
        private sealed class _TokeniserState_52 : TokeniserState {
            public _TokeniserState_52() {
            }

            internal override String GetName() {
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

        internal static TokeniserState Data = new _TokeniserState_52();

        private sealed class _TokeniserState_83 : TokeniserState {
            public _TokeniserState_83() {
            }

            internal override String GetName() {
                return "CharacterReferenceInData";
            }

            // from & in data
            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadCharRef(t, TokeniserState.Data);
            }
        }

        internal static TokeniserState CharacterReferenceInData = new _TokeniserState_83();

        private sealed class _TokeniserState_96 : TokeniserState {
            public _TokeniserState_96() {
            }

            internal override String GetName() {
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
                        String data = r.ConsumeToAny('&', '<', TokeniserState.nullChar);
                        t.Emit(data);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState Rcdata = new _TokeniserState_96();

        private sealed class _TokeniserState_128 : TokeniserState {
            public _TokeniserState_128() {
            }

            internal override String GetName() {
                return "CharacterReferenceInRcdata";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadCharRef(t, TokeniserState.Rcdata);
            }
        }

        internal static TokeniserState CharacterReferenceInRcdata = new _TokeniserState_128();

        private sealed class _TokeniserState_140 : TokeniserState {
            public _TokeniserState_140() {
            }

            internal override String GetName() {
                return "Rawtext";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadData(t, r, this, TokeniserState.RawtextLessthanSign);
            }
        }

        internal static TokeniserState Rawtext = new _TokeniserState_140();

        private sealed class _TokeniserState_152 : TokeniserState {
            public _TokeniserState_152() {
            }

            internal override String GetName() {
                return "ScriptData";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadData(t, r, this, TokeniserState.ScriptDataLessthanSign);
            }
        }

        internal static TokeniserState ScriptData = new _TokeniserState_152();

        private sealed class _TokeniserState_164 : TokeniserState {
            public _TokeniserState_164() {
            }

            internal override String GetName() {
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

        internal static TokeniserState PLAINTEXT = new _TokeniserState_164();

        private sealed class _TokeniserState_189 : TokeniserState {
            public _TokeniserState_189() {
            }

            internal override String GetName() {
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

        internal static TokeniserState TagOpen = new _TokeniserState_189();

        private sealed class _TokeniserState_222 : TokeniserState {
            public _TokeniserState_222() {
            }

            internal override String GetName() {
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
                            t.AdvanceTransition(TokeniserState.BogusComment);
                        }
                    }
                }
            }
        }

        internal static TokeniserState EndTagOpen = new _TokeniserState_222();

        private sealed class _TokeniserState_247 : TokeniserState {
            public _TokeniserState_247() {
            }

            internal override String GetName() {
                return "TagName";
            }

            // from < or </ in data, will have start or end tag pending
            internal override void Read(Tokeniser t, CharacterReader r) {
                // previous TagOpen state did NOT consume, will have a letter char in current
                //String tagName = r.consumeToAnySorted(tagCharsSorted).toLowerCase();
                String tagName = r.ConsumeTagName().ToLowerInvariant();
                t.tagPending.AppendTagName(tagName);
                switch (r.Consume()) {
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
                }
            }
        }

        internal static TokeniserState TagName = new _TokeniserState_247();

        private sealed class _TokeniserState_287 : TokeniserState {
            public _TokeniserState_287() {
            }

            // no default, as covered with above consumeToAny
            internal override String GetName() {
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
                        r.Unconsume();
                        // undo "<"
                        t.Transition(TokeniserState.Data);
                    }
                    else {
                        t.Emit("<");
                        t.Transition(TokeniserState.Rcdata);
                    }
                }
            }
        }

        internal static TokeniserState RcdataLessthanSign = new _TokeniserState_287();

        private sealed class _TokeniserState_313 : TokeniserState {
            public _TokeniserState_313() {
            }

            internal override String GetName() {
                return "RCDATAEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTagPending(false);
                    t.tagPending.AppendTagName(char.ToLower(r.Current()));
                    t.dataBuffer.Append(char.ToLower(r.Current()));
                    t.AdvanceTransition(TokeniserState.RCDATAEndTagName);
                }
                else {
                    t.Emit("</");
                    t.Transition(TokeniserState.Rcdata);
                }
            }
        }

        internal static TokeniserState RCDATAEndTagOpen = new _TokeniserState_313();

        private sealed class _TokeniserState_333 : TokeniserState {
            public _TokeniserState_333() {
            }

            internal override String GetName() {
                return "RCDATAEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    String name = r.ConsumeLetterSequence();
                    t.tagPending.AppendTagName(name.ToLowerInvariant());
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
                t.Emit("</" + t.dataBuffer.ToString());
                r.Unconsume();
                t.Transition(TokeniserState.Rcdata);
            }
        }

        internal static TokeniserState RCDATAEndTagName = new _TokeniserState_333();

        private sealed class _TokeniserState_386 : TokeniserState {
            public _TokeniserState_386() {
            }

            internal override String GetName() {
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

        internal static TokeniserState RawtextLessthanSign = new _TokeniserState_386();

        private sealed class _TokeniserState_404 : TokeniserState {
            public _TokeniserState_404() {
            }

            internal override String GetName() {
                return "RawtextEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadEndTag(t, r, TokeniserState.RawtextEndTagName, TokeniserState.Rawtext);
            }
        }

        internal static TokeniserState RawtextEndTagOpen = new _TokeniserState_404();

        private sealed class _TokeniserState_416 : TokeniserState {
            public _TokeniserState_416() {
            }

            internal override String GetName() {
                return "RawtextEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataEndTag(t, r, TokeniserState.Rawtext);
            }
        }

        internal static TokeniserState RawtextEndTagName = new _TokeniserState_416();

        private sealed class _TokeniserState_428 : TokeniserState {
            public _TokeniserState_428() {
            }

            internal override String GetName() {
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

                    default: {
                        t.Emit("<");
                        r.Unconsume();
                        t.Transition(TokeniserState.ScriptData);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState ScriptDataLessthanSign = new _TokeniserState_428();

        private sealed class _TokeniserState_453 : TokeniserState {
            public _TokeniserState_453() {
            }

            internal override String GetName() {
                return "ScriptDataEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.ReadEndTag(t, r, TokeniserState.ScriptDataEndTagName, TokeniserState.ScriptData);
            }
        }

        internal static TokeniserState ScriptDataEndTagOpen = new _TokeniserState_453();

        private sealed class _TokeniserState_465 : TokeniserState {
            public _TokeniserState_465() {
            }

            internal override String GetName() {
                return "ScriptDataEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataEndTag(t, r, TokeniserState.ScriptData);
            }
        }

        internal static TokeniserState ScriptDataEndTagName = new _TokeniserState_465();

        private sealed class _TokeniserState_477 : TokeniserState {
            public _TokeniserState_477() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataEscapeStart = new _TokeniserState_477();

        private sealed class _TokeniserState_494 : TokeniserState {
            public _TokeniserState_494() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataEscapeStartDash = new _TokeniserState_494();

        private sealed class _TokeniserState_511 : TokeniserState {
            public _TokeniserState_511() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataEscaped = new _TokeniserState_511();

        private sealed class _TokeniserState_545 : TokeniserState {
            public _TokeniserState_545() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataEscapedDash = new _TokeniserState_545();

        private sealed class _TokeniserState_580 : TokeniserState {
            public _TokeniserState_580() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataEscapedDashDash = new _TokeniserState_580();

        private sealed class _TokeniserState_618 : TokeniserState {
            public _TokeniserState_618() {
            }

            internal override String GetName() {
                return "ScriptDataEscapedLessthanSign";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTempBuffer();
                    t.dataBuffer.Append(char.ToLower(r.Current()));
                    t.Emit("<" + r.Current());
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

        internal static TokeniserState ScriptDataEscapedLessthanSign = new _TokeniserState_618();

        private sealed class _TokeniserState_641 : TokeniserState {
            public _TokeniserState_641() {
            }

            internal override String GetName() {
                return "ScriptDataEscapedEndTagOpen";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    t.CreateTagPending(false);
                    t.tagPending.AppendTagName(char.ToLower(r.Current()));
                    t.dataBuffer.Append(r.Current());
                    t.AdvanceTransition(TokeniserState.ScriptDataEscapedEndTagName);
                }
                else {
                    t.Emit("</");
                    t.Transition(TokeniserState.ScriptDataEscaped);
                }
            }
        }

        internal static TokeniserState ScriptDataEscapedEndTagOpen = new _TokeniserState_641();

        private sealed class _TokeniserState_661 : TokeniserState {
            public _TokeniserState_661() {
            }

            internal override String GetName() {
                return "ScriptDataEscapedEndTagName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataEndTag(t, r, TokeniserState.ScriptDataEscaped);
            }
        }

        internal static TokeniserState ScriptDataEscapedEndTagName = new _TokeniserState_661();

        private sealed class _TokeniserState_673 : TokeniserState {
            public _TokeniserState_673() {
            }

            internal override String GetName() {
                return "ScriptDataDoubleEscapeStart";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataDoubleEscapeTag(t, r, TokeniserState.ScriptDataDoubleEscaped, TokeniserState.ScriptDataEscaped
                    );
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapeStart = new _TokeniserState_673();

        private sealed class _TokeniserState_685 : TokeniserState {
            public _TokeniserState_685() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataDoubleEscaped = new _TokeniserState_685();

        private sealed class _TokeniserState_719 : TokeniserState {
            public _TokeniserState_719() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataDoubleEscapedDash = new _TokeniserState_719();

        private sealed class _TokeniserState_753 : TokeniserState {
            public _TokeniserState_753() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataDoubleEscapedDashDash = new _TokeniserState_753();

        private sealed class _TokeniserState_790 : TokeniserState {
            public _TokeniserState_790() {
            }

            internal override String GetName() {
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

        internal static TokeniserState ScriptDataDoubleEscapedLessthanSign = new _TokeniserState_790();

        private sealed class _TokeniserState_808 : TokeniserState {
            public _TokeniserState_808() {
            }

            internal override String GetName() {
                return "ScriptDataDoubleEscapeEnd";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                TokeniserState.HandleDataDoubleEscapeTag(t, r, TokeniserState.ScriptDataEscaped, TokeniserState.ScriptDataDoubleEscaped
                    );
            }
        }

        internal static TokeniserState ScriptDataDoubleEscapeEnd = new _TokeniserState_808();

        private sealed class _TokeniserState_820 : TokeniserState {
            public _TokeniserState_820() {
            }

            internal override String GetName() {
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

                    case '>': {
                        t.EmitTagPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    case TokeniserState.nullChar: {
                        t.Error(this);
                        t.tagPending.NewAttribute();
                        r.Unconsume();
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
                    case '<':
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

        internal static TokeniserState BeforeAttributeName = new _TokeniserState_820();

        private sealed class _TokeniserState_871 : TokeniserState {
            public _TokeniserState_871() {
            }

            internal override String GetName() {
                return "AttributeName";
            }

            // from before attribute name
            internal override void Read(Tokeniser t, CharacterReader r) {
                String name = r.ConsumeToAnySorted(TokeniserState.attributeNameCharsSorted);
                t.tagPending.AppendAttributeName(name.ToLowerInvariant());
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
                }
            }
        }

        internal static TokeniserState AttributeName = new _TokeniserState_871();

        private sealed class _TokeniserState_920 : TokeniserState {
            public _TokeniserState_920() {
            }

            // no default, as covered in consumeToAny
            internal override String GetName() {
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

        internal static TokeniserState AfterAttributeName = new _TokeniserState_920();

        private sealed class _TokeniserState_972 : TokeniserState {
            public _TokeniserState_972() {
            }

            internal override String GetName() {
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

        internal static TokeniserState BeforeAttributeValue = new _TokeniserState_972();

        private sealed class _TokeniserState_1028 : TokeniserState {
            public _TokeniserState_1028() {
            }

            internal override String GetName() {
                return "AttributeValue_doubleQuoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                String value = r.ConsumeToAny(TokeniserState.attributeDoubleValueCharsSorted);
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
                        char[] @ref = t.ConsumeCharacterReference('"', true);
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
                }
            }
        }

        internal static TokeniserState AttributeValue_doubleQuoted = new _TokeniserState_1028();

        private sealed class _TokeniserState_1067 : TokeniserState {
            public _TokeniserState_1067() {
            }

            // no default, handled in consume to any above
            internal override String GetName() {
                return "AttributeValue_singleQuoted";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                String value = r.ConsumeToAny(TokeniserState.attributeSingleValueCharsSorted);
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
                        char[] @ref = t.ConsumeCharacterReference('\'', true);
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
                }
            }
        }

        internal static TokeniserState AttributeValue_singleQuoted = new _TokeniserState_1067();

        private sealed class _TokeniserState_1106 : TokeniserState {
            public _TokeniserState_1106() {
            }

            // no default, handled in consume to any above
            internal override String GetName() {
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
                        char[] @ref = t.ConsumeCharacterReference('>', true);
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
                }
            }
        }

        internal static TokeniserState AttributeValue_unquoted = new _TokeniserState_1106();

        private sealed class _TokeniserState_1161 : TokeniserState {
            public _TokeniserState_1161() {
            }

            // no default, handled in consume to any above
            // CharacterReferenceInAttributeValue state handled inline
            internal override String GetName() {
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
                        t.Error(this);
                        r.Unconsume();
                        t.Transition(TokeniserState.BeforeAttributeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState AfterAttributeValue_quoted = new _TokeniserState_1161();

        private sealed class _TokeniserState_1198 : TokeniserState {
            public _TokeniserState_1198() {
            }

            internal override String GetName() {
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
                        t.Error(this);
                        t.Transition(TokeniserState.BeforeAttributeName);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState SelfClosingStartTag = new _TokeniserState_1198();

        private sealed class _TokeniserState_1224 : TokeniserState {
            public _TokeniserState_1224() {
            }

            internal override String GetName() {
                return "BogusComment";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                // todo: handle bogus comment starting from eof. when does that trigger?
                // rewind to capture character that lead us here
                r.Unconsume();
                Token.Comment comment = new Token.Comment();
                comment.bogus = true;
                comment.data.Append(r.ConsumeTo('>'));
                // todo: replace nullChar with replaceChar
                t.Emit(comment);
                t.AdvanceTransition(TokeniserState.Data);
            }
        }

        internal static TokeniserState BogusComment = new _TokeniserState_1224();

        private sealed class _TokeniserState_1244 : TokeniserState {
            public _TokeniserState_1244() {
            }

            internal override String GetName() {
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
                            // todo: should actually check current namepspace, and only non-html allows cdata. until namespace
                            // is implemented properly, keep handling as cdata
                            //} else if (!t.currentNodeInHtmlNS() && r.matchConsume("[CDATA[")) {
                            t.Transition(TokeniserState.CdataSection);
                        }
                        else {
                            t.Error(this);
                            t.AdvanceTransition(TokeniserState.BogusComment);
                        }
                    }
                }
            }
        }

        internal static TokeniserState MarkupDeclarationOpen = new _TokeniserState_1244();

        private sealed class _TokeniserState_1269 : TokeniserState {
            public _TokeniserState_1269() {
            }

            // advance so this character gets in bogus comment data's rewind
            internal override String GetName() {
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
                        t.commentPending.data.Append(TokeniserState.replacementChar);
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
                        t.commentPending.data.Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentStart = new _TokeniserState_1269();

        private sealed class _TokeniserState_1304 : TokeniserState {
            public _TokeniserState_1304() {
            }

            internal override String GetName() {
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
                        t.commentPending.data.Append(TokeniserState.replacementChar);
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
                        t.commentPending.data.Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentStartDash = new _TokeniserState_1304();

        private sealed class _TokeniserState_1339 : TokeniserState {
            public _TokeniserState_1339() {
            }

            internal override String GetName() {
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
                        t.commentPending.data.Append(TokeniserState.replacementChar);
                        break;
                    }

                    case TokeniserState.eof: {
                        t.EofError(this);
                        t.EmitCommentPending();
                        t.Transition(TokeniserState.Data);
                        break;
                    }

                    default: {
                        t.commentPending.data.Append(r.ConsumeToAny('-', TokeniserState.nullChar));
                        break;
                    }
                }
            }
        }

        internal static TokeniserState Comment = new _TokeniserState_1339();

        private sealed class _TokeniserState_1368 : TokeniserState {
            public _TokeniserState_1368() {
            }

            internal override String GetName() {
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
                        t.commentPending.data.Append('-').Append(TokeniserState.replacementChar);
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
                        t.commentPending.data.Append('-').Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentEndDash = new _TokeniserState_1368();

        private sealed class _TokeniserState_1398 : TokeniserState {
            public _TokeniserState_1398() {
            }

            internal override String GetName() {
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
                        t.commentPending.data.Append("--").Append(TokeniserState.replacementChar);
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
                        t.commentPending.data.Append('-');
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
                        t.commentPending.data.Append("--").Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentEnd = new _TokeniserState_1398();

        private sealed class _TokeniserState_1438 : TokeniserState {
            public _TokeniserState_1438() {
            }

            internal override String GetName() {
                return "CommentEndBang";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                char c = r.Consume();
                switch (c) {
                    case '-': {
                        t.commentPending.data.Append("--!");
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
                        t.commentPending.data.Append("--!").Append(TokeniserState.replacementChar);
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
                        t.commentPending.data.Append("--!").Append(c);
                        t.Transition(TokeniserState.Comment);
                        break;
                    }
                }
            }
        }

        internal static TokeniserState CommentEndBang = new _TokeniserState_1438();

        private sealed class _TokeniserState_1473 : TokeniserState {
            public _TokeniserState_1473() {
            }

            internal override String GetName() {
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

        internal static TokeniserState Doctype = new _TokeniserState_1473();

        private sealed class _TokeniserState_1507 : TokeniserState {
            public _TokeniserState_1507() {
            }

            internal override String GetName() {
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

        internal static TokeniserState BeforeDoctypeName = new _TokeniserState_1507();

        private sealed class _TokeniserState_1549 : TokeniserState {
            public _TokeniserState_1549() {
            }

            internal override String GetName() {
                return "DoctypeName";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                if (r.MatchesLetter()) {
                    String name = r.ConsumeLetterSequence();
                    t.doctypePending.name.Append(name.ToLowerInvariant());
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

        internal static TokeniserState DoctypeName = new _TokeniserState_1549();

        private sealed class _TokeniserState_1591 : TokeniserState {
            public _TokeniserState_1591() {
            }

            internal override String GetName() {
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
                        if (r.MatchConsumeIgnoreCase("PUBLIC")) {
                            t.Transition(TokeniserState.AfterDoctypePublicKeyword);
                        }
                        else {
                            if (r.MatchConsumeIgnoreCase("SYSTEM")) {
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

        internal static TokeniserState AfterDoctypeName = new _TokeniserState_1591();

        private sealed class _TokeniserState_1624 : TokeniserState {
            public _TokeniserState_1624() {
            }

            internal override String GetName() {
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

        internal static TokeniserState AfterDoctypePublicKeyword = new _TokeniserState_1624();

        private sealed class _TokeniserState_1671 : TokeniserState {
            public _TokeniserState_1671() {
            }

            internal override String GetName() {
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

        internal static TokeniserState BeforeDoctypePublicIdentifier = new _TokeniserState_1671();

        private sealed class _TokeniserState_1715 : TokeniserState {
            public _TokeniserState_1715() {
            }

            internal override String GetName() {
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

        internal static TokeniserState DoctypePublicIdentifier_doubleQuoted = new _TokeniserState_1715();

        private sealed class _TokeniserState_1750 : TokeniserState {
            public _TokeniserState_1750() {
            }

            internal override String GetName() {
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

        internal static TokeniserState DoctypePublicIdentifier_singleQuoted = new _TokeniserState_1750();

        private sealed class _TokeniserState_1785 : TokeniserState {
            public _TokeniserState_1785() {
            }

            internal override String GetName() {
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

        internal static TokeniserState AfterDoctypePublicIdentifier = new _TokeniserState_1785();

        private sealed class _TokeniserState_1830 : TokeniserState {
            public _TokeniserState_1830() {
            }

            internal override String GetName() {
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

        internal static TokeniserState BetweenDoctypePublicAndSystemIdentifiers = new _TokeniserState_1830();

        private sealed class _TokeniserState_1874 : TokeniserState {
            public _TokeniserState_1874() {
            }

            internal override String GetName() {
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

        internal static TokeniserState AfterDoctypeSystemKeyword = new _TokeniserState_1874();

        private sealed class _TokeniserState_1921 : TokeniserState {
            public _TokeniserState_1921() {
            }

            internal override String GetName() {
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

        internal static TokeniserState BeforeDoctypeSystemIdentifier = new _TokeniserState_1921();

        private sealed class _TokeniserState_1965 : TokeniserState {
            public _TokeniserState_1965() {
            }

            internal override String GetName() {
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

        internal static TokeniserState DoctypeSystemIdentifier_doubleQuoted = new _TokeniserState_1965();

        private sealed class _TokeniserState_2000 : TokeniserState {
            public _TokeniserState_2000() {
            }

            internal override String GetName() {
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

        internal static TokeniserState DoctypeSystemIdentifier_singleQuoted = new _TokeniserState_2000();

        private sealed class _TokeniserState_2035 : TokeniserState {
            public _TokeniserState_2035() {
            }

            internal override String GetName() {
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

        internal static TokeniserState AfterDoctypeSystemIdentifier = new _TokeniserState_2035();

        private sealed class _TokeniserState_2069 : TokeniserState {
            public _TokeniserState_2069() {
            }

            // NOT force quirks
            internal override String GetName() {
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

        internal static TokeniserState BogusDoctype = new _TokeniserState_2069();

        private sealed class _TokeniserState_2094 : TokeniserState {
            public _TokeniserState_2094() {
            }

            internal override String GetName() {
                return "CdataSection";
            }

            internal override void Read(Tokeniser t, CharacterReader r) {
                String data = r.ConsumeTo("]]>");
                t.Emit(data);
                r.MatchConsume("]]>");
                t.Transition(TokeniserState.Data);
            }
        }

        internal static TokeniserState CdataSection = new _TokeniserState_2094();

        public override String ToString() {
            return GetName();
        }

        internal abstract String GetName();

        internal abstract void Read(Tokeniser t, CharacterReader r);

        internal const char nullChar = '\u0000';

        private static readonly char[] attributeSingleValueCharsSorted = new char[] { '\'', '&', nullChar };

        private static readonly char[] attributeDoubleValueCharsSorted = new char[] { '"', '&', nullChar };

        private static readonly char[] attributeNameCharsSorted = new char[] { '\t', '\n', '\r', '\f', ' ', '/', '='
            , '>', nullChar, '"', '\'', '<' };

        private static readonly char[] attributeValueUnquoted = new char[] { '\t', '\n', '\r', '\f', ' ', '&', '>'
            , nullChar, '"', '\'', '<', '=', '`' };

        private const char replacementChar = Tokeniser.replacementChar;

        private static readonly String replacementStr = Tokeniser.replacementChar.ToString();

        private const char eof = CharacterReader.EOF;

        static TokeniserState() {
            JavaUtil.Sort(attributeSingleValueCharsSorted);
            JavaUtil.Sort(attributeDoubleValueCharsSorted);
            JavaUtil.Sort(attributeNameCharsSorted);
            JavaUtil.Sort(attributeValueUnquoted);
        }

        /// <summary>Handles RawtextEndTagName, ScriptDataEndTagName, and ScriptDataEscapedEndTagName.</summary>
        /// <remarks>
        /// Handles RawtextEndTagName, ScriptDataEndTagName, and ScriptDataEscapedEndTagName. Same body impl, just
        /// different else exit transitions.
        /// </remarks>
        private static void HandleDataEndTag(Tokeniser t, CharacterReader r, TokeniserState elseTransition) {
            if (r.MatchesLetter()) {
                String name = r.ConsumeLetterSequence();
                t.tagPending.AppendTagName(name.ToLowerInvariant());
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
                t.Emit("</" + t.dataBuffer.ToString());
                t.Transition(elseTransition);
            }
        }

        private static void ReadData(Tokeniser t, CharacterReader r, TokeniserState current, TokeniserState advance
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
                    String data = r.ConsumeToAny('<', nullChar);
                    t.Emit(data);
                    break;
                }
            }
        }

        private static void ReadCharRef(Tokeniser t, TokeniserState advance) {
            char[] c = t.ConsumeCharacterReference(null, false);
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
                t.dataBuffer.Append(name.ToLowerInvariant());
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
