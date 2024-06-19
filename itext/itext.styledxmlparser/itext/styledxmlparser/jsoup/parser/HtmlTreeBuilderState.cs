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
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
//\cond DO_NOT_DOCUMENT
    /// <summary>The Tree Builder's current state.</summary>
    /// <remarks>The Tree Builder's current state. Each state embodies the processing for the state, and transitions to other states.
    ///     </remarks>
    internal abstract class HtmlTreeBuilderState {
        public static HtmlTreeBuilderState Initial = new HtmlTreeBuilderState.InitialBS();

        public static HtmlTreeBuilderState BeforeHtml = new HtmlTreeBuilderState.BeforeHtmlBS();

        public static HtmlTreeBuilderState BeforeHead = new HtmlTreeBuilderState.BeforeHeadBS();

        public static HtmlTreeBuilderState InHead = new HtmlTreeBuilderState.InHeadBS();

        public static HtmlTreeBuilderState InHeadNoscript = new HtmlTreeBuilderState.InHeadNoScriptBS();

        public static HtmlTreeBuilderState AfterHead = new HtmlTreeBuilderState.AfterHeadBS();

        public static HtmlTreeBuilderState InBody = new HtmlTreeBuilderState.InBodyBS();

        public static HtmlTreeBuilderState Text = new HtmlTreeBuilderState.TextBS();

        public static HtmlTreeBuilderState InTable = new HtmlTreeBuilderState.InTableBS();

        public static HtmlTreeBuilderState InTableText = new HtmlTreeBuilderState.InTableTextBS();

        public static HtmlTreeBuilderState InCaption = new HtmlTreeBuilderState.InCaptionBS();

        public static HtmlTreeBuilderState InColumnGroup = new HtmlTreeBuilderState.InColumnGroupBS();

        public static HtmlTreeBuilderState InTableBody = new HtmlTreeBuilderState.InTableBodyBS();

        public static HtmlTreeBuilderState InRow = new HtmlTreeBuilderState.InRowBS();

        public static HtmlTreeBuilderState InCell = new HtmlTreeBuilderState.InCellBS();

        public static HtmlTreeBuilderState InSelect = new HtmlTreeBuilderState.InSelectBS();

        public static HtmlTreeBuilderState InSelectInTable = new HtmlTreeBuilderState.InSelectInTableBS();

        public static HtmlTreeBuilderState AfterBody = new HtmlTreeBuilderState.AfterBodyBS();

        public static HtmlTreeBuilderState InFrameset = new HtmlTreeBuilderState.InFrameSetBS();

        public static HtmlTreeBuilderState AfterFrameset = new HtmlTreeBuilderState.AfterFrameSetBS();

        public static HtmlTreeBuilderState AfterAfterBody = new HtmlTreeBuilderState.AfterAfterBodyBS();

        public static HtmlTreeBuilderState AfterAfterFrameset = new HtmlTreeBuilderState.AfterAfterFrameSetBS();

        public static HtmlTreeBuilderState ForeignContent = new HtmlTreeBuilderState.ForeignContentBS();

        private static readonly String nullString = '\u0000'.ToString();

//\cond DO_NOT_DOCUMENT
        internal abstract bool Process(Token t, HtmlTreeBuilder tb);
//\endcond

        private static bool IsWhitespace(Token t) {
            if (t.IsCharacter()) {
                String data = t.AsCharacter().GetData();
                return iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsBlank(data);
            }
            return false;
        }

        private static bool IsWhitespace(String data) {
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsBlank(data);
        }

        private static void HandleRcData(Token.StartTag startTag, HtmlTreeBuilder tb) {
            tb.tokeniser.Transition(TokeniserState.Rcdata);
            tb.MarkInsertionMode();
            tb.Transition(Text);
            tb.Insert(startTag);
        }

        private static void HandleRawtext(Token.StartTag startTag, HtmlTreeBuilder tb) {
            tb.tokeniser.Transition(TokeniserState.Rawtext);
            tb.MarkInsertionMode();
            tb.Transition(Text);
            tb.Insert(startTag);
        }

//\cond DO_NOT_DOCUMENT
        // lists of tags to search through
        internal sealed class Constants {
//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InHeadEmpty = new String[] { "base", "basefont", "bgsound", "command", "link"
                 };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InHeadRaw = new String[] { "noframes", "style" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InHeadEnd = new String[] { "body", "br", "html" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] AfterHeadBody = new String[] { "body", "html" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] BeforeHtmlToHead = new String[] { "body", "br", "head", "html" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InHeadNoScriptHead = new String[] { "basefont", "bgsound", "link", "meta"
                , "noframes", "style" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartToHead = new String[] { "base", "basefont", "bgsound", "command"
                , "link", "meta", "noframes", "script", "style", "title" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartPClosers = new String[] { "address", "article", "aside", "blockquote"
                , "center", "details", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup"
                , "menu", "nav", "ol", "p", "section", "summary", "ul" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] Headings = new String[] { "h1", "h2", "h3", "h4", "h5", "h6" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartLiBreakers = new String[] { "address", "div", "p" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] DdDt = new String[] { "dd", "dt" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] Formatters = new String[] { "b", "big", "code", "em", "font", "i", "s", 
                "small", "strike", "strong", "tt", "u" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartApplets = new String[] { "applet", "marquee", "object" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartEmptyFormatters = new String[] { "area", "br", "embed", "img"
                , "keygen", "wbr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartMedia = new String[] { "param", "source", "track" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartInputAttribs = new String[] { "action", "name", "prompt" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyStartDrop = new String[] { "caption", "col", "colgroup", "frame", 
                "head", "tbody", "td", "tfoot", "th", "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyEndClosers = new String[] { "address", "article", "aside", "blockquote"
                , "button", "center", "details", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header"
                , "hgroup", "listing", "menu", "nav", "ol", "pre", "section", "summary", "ul" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyEndAdoptionFormatters = new String[] { "a", "b", "big", "code", "em"
                , "font", "i", "nobr", "s", "small", "strike", "strong", "tt", "u" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InBodyEndTableFosters = new String[] { "table", "tbody", "tfoot", "thead"
                , "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableToBody = new String[] { "tbody", "tfoot", "thead" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableAddBody = new String[] { "td", "th", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableToHead = new String[] { "script", "style" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InCellNames = new String[] { "td", "th" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InCellBody = new String[] { "body", "caption", "col", "colgroup", "html"
                 };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InCellTable = new String[] { "table", "tbody", "tfoot", "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InCellCol = new String[] { "caption", "col", "colgroup", "tbody", "td", 
                "tfoot", "th", "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableEndErr = new String[] { "body", "caption", "col", "colgroup", "html"
                , "tbody", "td", "tfoot", "th", "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableFoster = new String[] { "table", "tbody", "tfoot", "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableBodyExit = new String[] { "caption", "col", "colgroup", "tbody", 
                "tfoot", "thead" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableBodyEndIgnore = new String[] { "body", "caption", "col", "colgroup"
                , "html", "td", "th", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InRowMissing = new String[] { "caption", "col", "colgroup", "tbody", "tfoot"
                , "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InRowIgnore = new String[] { "body", "caption", "col", "colgroup", "html"
                , "td", "th" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InSelectEnd = new String[] { "input", "keygen", "textarea" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InSelecTableEnd = new String[] { "caption", "table", "tbody", "td", "tfoot"
                , "th", "thead", "tr" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InTableEndIgnore = new String[] { "tbody", "tfoot", "thead" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InHeadNoscriptIgnore = new String[] { "head", "noscript" };
//\endcond

//\cond DO_NOT_DOCUMENT
            internal static readonly String[] InCaptionIgnore = new String[] { "body", "col", "colgroup", "html", "tbody"
                , "td", "tfoot", "th", "thead", "tr" };
//\endcond
        }
//\endcond

        private sealed class InitialBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "Initial";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    return true;
                }
                else {
                    // ignore whitespace until we get the first content
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (t.IsDoctype()) {
                            Token.Doctype d = t.AsDoctype();
                            DocumentType doctype = new DocumentType(tb.settings.NormalizeTag(d.GetName()), d.GetPublicIdentifier(), d.
                                GetSystemIdentifier());
                            doctype.SetPubSysKey(d.GetPubSysKey());
                            tb.GetDocument().AppendChild(doctype);
                            if (d.IsForceQuirks()) {
                                tb.GetDocument().QuirksMode(QuirksMode.quirks);
                            }
                            tb.Transition(BeforeHtml);
                        }
                        else {
                            tb.Transition(BeforeHtml);
                            return tb.Process(t);
                        }
                    }
                }
                // re-process token
                return true;
            }
//\endcond
        }

        private sealed class BeforeHtmlBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "BeforeHtml";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsDoctype()) {
                    tb.Error(this);
                    return false;
                }
                else {
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (IsWhitespace(t)) {
                            tb.Insert(t.AsCharacter());
                        }
                        else {
                            // out of spec - include whitespace
                            if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html")) {
                                tb.Insert(t.AsStartTag());
                                tb.Transition(BeforeHead);
                            }
                            else {
                                if (t.IsEndTag() && (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsEndTag().NormalName(), HtmlTreeBuilderState.Constants
                                    .BeforeHtmlToHead))) {
                                    return AnythingElse(t, tb);
                                }
                                else {
                                    if (t.IsEndTag()) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        return AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.InsertStartTag("html");
                tb.Transition(BeforeHead);
                return tb.Process(t);
            }
        }

        private sealed class BeforeHeadBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "BeforeHead";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                }
                else {
                    // out of spec - include whitespace
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (t.IsDoctype()) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html")) {
                                return InBody.Process(t, tb);
                            }
                            else {
                                // does not transition
                                if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("head")) {
                                    iText.StyledXmlParser.Jsoup.Nodes.Element head = tb.Insert(t.AsStartTag());
                                    tb.SetHeadElement(head);
                                    tb.Transition(InHead);
                                }
                                else {
                                    if (t.IsEndTag() && (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsEndTag().NormalName(), HtmlTreeBuilderState.Constants
                                        .BeforeHtmlToHead))) {
                                        tb.ProcessStartTag("head");
                                        return tb.Process(t);
                                    }
                                    else {
                                        if (t.IsEndTag()) {
                                            tb.Error(this);
                                            return false;
                                        }
                                        else {
                                            tb.ProcessStartTag("head");
                                            return tb.Process(t);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class InHeadBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InHead";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                    // out of spec - include whitespace
                    return true;
                }
                String name;
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment: {
                        tb.Insert(t.AsComment());
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype: {
                        tb.Error(this);
                        return false;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        Token.StartTag start = t.AsStartTag();
                        name = start.NormalName();
                        if (name.Equals("html")) {
                            return InBody.Process(t, tb);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InHeadEmpty
                                )) {
                                iText.StyledXmlParser.Jsoup.Nodes.Element el = tb.InsertEmpty(start);
                                // jsoup special: update base the first time it is seen
                                if (name.Equals("base") && el.HasAttr("href")) {
                                    tb.MaybeSetBaseUri(el);
                                }
                            }
                            else {
                                if (name.Equals("meta")) {
                                    tb.InsertEmpty(start);
                                }
                                else {
                                    if (name.Equals("title")) {
                                        HandleRcData(start, tb);
                                    }
                                    else {
                                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InHeadRaw
                                            )) {
                                            HandleRawtext(start, tb);
                                        }
                                        else {
                                            if (name.Equals("noscript")) {
                                                // else if noscript && scripting flag = true: rawtext (jsoup doesn't run script, to handle as noscript)
                                                tb.Insert(start);
                                                tb.Transition(InHeadNoscript);
                                            }
                                            else {
                                                if (name.Equals("script")) {
                                                    // skips some script rules as won't execute them
                                                    tb.tokeniser.Transition(TokeniserState.ScriptData);
                                                    tb.MarkInsertionMode();
                                                    tb.Transition(Text);
                                                    tb.Insert(start);
                                                }
                                                else {
                                                    if (name.Equals("head")) {
                                                        tb.Error(this);
                                                        return false;
                                                    }
                                                    else {
                                                        return AnythingElse(t, tb);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        Token.EndTag end = t.AsEndTag();
                        name = end.NormalName();
                        if (name.Equals("head")) {
                            tb.Pop();
                            tb.Transition(AfterHead);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InHeadEnd
                                )) {
                                return AnythingElse(t, tb);
                            }
                            else {
                                tb.Error(this);
                                return false;
                            }
                        }
                        break;
                    }

                    default: {
                        return AnythingElse(t, tb);
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, TreeBuilder tb) {
                tb.ProcessEndTag("head");
                return tb.Process(t);
            }
        }

        private sealed class InHeadNoScriptBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InHeadNoscript";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsDoctype()) {
                    tb.Error(this);
                }
                else {
                    if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html")) {
                        return tb.Process(t, InBody);
                    }
                    else {
                        if (t.IsEndTag() && t.AsEndTag().NormalName().Equals("noscript")) {
                            tb.Pop();
                            tb.Transition(InHead);
                        }
                        else {
                            if (IsWhitespace(t) || t.IsComment() || (t.IsStartTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil
                                .InSorted(t.AsStartTag().NormalName(), HtmlTreeBuilderState.Constants.InHeadNoScriptHead))) {
                                return tb.Process(t, InHead);
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().NormalName().Equals("br")) {
                                    return AnythingElse(t, tb);
                                }
                                else {
                                    if ((t.IsStartTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsStartTag().NormalName(
                                        ), HtmlTreeBuilderState.Constants.InHeadNoscriptIgnore)) || t.IsEndTag()) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        return AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                // note that this deviates from spec, which is to pop out of noscript and reprocess in head:
                // https://html.spec.whatwg.org/multipage/parsing.html#parsing-main-inheadnoscript
                // allows content to be inserted as data
                tb.Error(this);
                tb.Insert(new Token.Character().Data(t.ToString()));
                return true;
            }
        }

        private sealed class AfterHeadBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "AfterHead";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                }
                else {
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (t.IsDoctype()) {
                            tb.Error(this);
                        }
                        else {
                            if (t.IsStartTag()) {
                                Token.StartTag startTag = t.AsStartTag();
                                String name = startTag.NormalName();
                                if (name.Equals("html")) {
                                    return tb.Process(t, InBody);
                                }
                                else {
                                    if (name.Equals("body")) {
                                        tb.Insert(startTag);
                                        tb.FramesetOk(false);
                                        tb.Transition(InBody);
                                    }
                                    else {
                                        if (name.Equals("frameset")) {
                                            tb.Insert(startTag);
                                            tb.Transition(InFrameset);
                                        }
                                        else {
                                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartToHead
                                                )) {
                                                tb.Error(this);
                                                iText.StyledXmlParser.Jsoup.Nodes.Element head = tb.GetHeadElement();
                                                tb.Push(head);
                                                tb.Process(t, InHead);
                                                tb.RemoveFromStack(head);
                                            }
                                            else {
                                                if (name.Equals("head")) {
                                                    tb.Error(this);
                                                    return false;
                                                }
                                                else {
                                                    AnythingElse(t, tb);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                if (t.IsEndTag()) {
                                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsEndTag().NormalName(), HtmlTreeBuilderState.Constants
                                        .AfterHeadBody)) {
                                        AnythingElse(t, tb);
                                    }
                                    else {
                                        tb.Error(this);
                                        return false;
                                    }
                                }
                                else {
                                    AnythingElse(t, tb);
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.ProcessStartTag("body");
                tb.FramesetOk(true);
                return tb.Process(t);
            }
        }

        private sealed class InBodyBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InBody";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                        Token.Character c = t.AsCharacter();
                        if (c.GetData().Equals(nullString)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (tb.FramesetOk() && IsWhitespace(c)) {
                                // don't check if whitespace if frames already closed
                                tb.ReconstructFormattingElements();
                                tb.Insert(c);
                            }
                            else {
                                tb.ReconstructFormattingElements();
                                tb.Insert(c);
                                tb.FramesetOk(false);
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment: {
                        tb.Insert(t.AsComment());
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype: {
                        tb.Error(this);
                        return false;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        return InBodyStartTag(t, tb);
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        return InBodyEndTag(t, tb);
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                        // stop parsing
                        break;
                    }
                }
                return true;
            }
//\endcond

            private bool InBodyStartTag(Token t, HtmlTreeBuilder tb) {
                Token.StartTag startTag = t.AsStartTag();
                String name = startTag.NormalName();
                List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack;
                iText.StyledXmlParser.Jsoup.Nodes.Element el;
                switch (name) {
                    case "a": {
                        if (tb.GetActiveFormattingElement("a") != null) {
                            tb.Error(this);
                            tb.ProcessEndTag("a");
                            // still on stack?
                            iText.StyledXmlParser.Jsoup.Nodes.Element remainingA = tb.GetFromStack("a");
                            if (remainingA != null) {
                                tb.RemoveFromActiveFormattingElements(remainingA);
                                tb.RemoveFromStack(remainingA);
                            }
                        }
                        tb.ReconstructFormattingElements();
                        el = tb.Insert(startTag);
                        tb.PushActiveFormattingElements(el);
                        break;
                    }

                    case "span": {
                        // same as final else, but short circuits lots of checks
                        tb.ReconstructFormattingElements();
                        tb.Insert(startTag);
                        break;
                    }

                    case "li": {
                        tb.FramesetOk(false);
                        stack = tb.GetStack();
                        for (int i = stack.Count - 1; i > 0; i--) {
                            el = stack[i];
                            if (el.NormalName().Equals("li")) {
                                tb.ProcessEndTag("li");
                                break;
                            }
                            if (tb.IsSpecial(el) && !iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(el.NormalName(), HtmlTreeBuilderState.Constants
                                .InBodyStartLiBreakers)) {
                                break;
                            }
                        }
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.Insert(startTag);
                        break;
                    }

                    case "html": {
                        tb.Error(this);
                        // merge attributes onto real html
                        iText.StyledXmlParser.Jsoup.Nodes.Element html = tb.GetStack()[0];
                        if (startTag.HasAttributes()) {
                            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in startTag.attributes) {
                                if (!html.HasAttr(attribute.Key)) {
                                    html.Attributes().Put(attribute);
                                }
                            }
                        }
                        break;
                    }

                    case "body": {
                        tb.Error(this);
                        stack = tb.GetStack();
                        if (stack.Count == 1 || (stack.Count > 2 && !stack[1].NormalName().Equals("body"))) {
                            // only in fragment case
                            return false;
                        }
                        else {
                            // ignore
                            tb.FramesetOk(false);
                            iText.StyledXmlParser.Jsoup.Nodes.Element body = stack[1];
                            if (startTag.HasAttributes()) {
                                foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in startTag.attributes) {
                                    if (!body.HasAttr(attribute.Key)) {
                                        body.Attributes().Put(attribute);
                                    }
                                }
                            }
                        }
                        break;
                    }

                    case "frameset": {
                        tb.Error(this);
                        stack = tb.GetStack();
                        if (stack.Count == 1 || (stack.Count > 2 && !stack[1].NormalName().Equals("body"))) {
                            // only in fragment case
                            return false;
                        }
                        else {
                            // ignore
                            if (!tb.FramesetOk()) {
                                return false;
                            }
                            else {
                                // ignore frameset
                                iText.StyledXmlParser.Jsoup.Nodes.Element second = stack[1];
                                if (second.Parent() != null) {
                                    second.Remove();
                                }
                                // pop up to html element
                                while (stack.Count > 1) {
                                    stack.JRemoveAt(stack.Count - 1);
                                }
                                tb.Insert(startTag);
                                tb.Transition(InFrameset);
                            }
                        }
                        break;
                    }

                    case "form": {
                        if (tb.GetFormElement() != null) {
                            tb.Error(this);
                            return false;
                        }
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.InsertForm(startTag, true);
                        break;
                    }

                    case "plaintext": {
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.Insert(startTag);
                        tb.tokeniser.Transition(TokeniserState.PLAINTEXT);
                        // once in, never gets out
                        break;
                    }

                    case "button": {
                        if (tb.InButtonScope("button")) {
                            // close and reprocess
                            tb.Error(this);
                            tb.ProcessEndTag("button");
                            tb.Process(startTag);
                        }
                        else {
                            tb.ReconstructFormattingElements();
                            tb.Insert(startTag);
                            tb.FramesetOk(false);
                        }
                        break;
                    }

                    case "nobr": {
                        tb.ReconstructFormattingElements();
                        if (tb.InScope("nobr")) {
                            tb.Error(this);
                            tb.ProcessEndTag("nobr");
                            tb.ReconstructFormattingElements();
                        }
                        el = tb.Insert(startTag);
                        tb.PushActiveFormattingElements(el);
                        break;
                    }

                    case "table": {
                        if (tb.GetDocument().QuirksMode() != QuirksMode.quirks && tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.Insert(startTag);
                        tb.FramesetOk(false);
                        tb.Transition(InTable);
                        break;
                    }

                    case "input": {
                        tb.ReconstructFormattingElements();
                        el = tb.InsertEmpty(startTag);
                        if (!el.Attr("type").EqualsIgnoreCase("hidden")) {
                            tb.FramesetOk(false);
                        }
                        break;
                    }

                    case "hr": {
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.InsertEmpty(startTag);
                        tb.FramesetOk(false);
                        break;
                    }

                    case "image": {
                        if (tb.GetFromStack("svg") == null) {
                            return tb.Process(startTag.Name("img"));
                        }
                        else {
                            // change <image> to <img>, unless in svg
                            tb.Insert(startTag);
                        }
                        break;
                    }

                    case "isindex": {
                        // how much do we care about the early 90s?
                        tb.Error(this);
                        if (tb.GetFormElement() != null) {
                            return false;
                        }
                        tb.ProcessStartTag("form");
                        if (startTag.HasAttribute("action")) {
                            iText.StyledXmlParser.Jsoup.Nodes.Element form = tb.GetFormElement();
                            form.Attr("action", startTag.attributes.Get("action"));
                        }
                        tb.ProcessStartTag("hr");
                        tb.ProcessStartTag("label");
                        // hope you like english.
                        String prompt = startTag.HasAttribute("prompt") ? startTag.attributes.Get("prompt") : "This is a searchable index. Enter search keywords: ";
                        tb.Process(new Token.Character().Data(prompt));
                        // input
                        Attributes inputAttribs = new Attributes();
                        if (startTag.HasAttributes()) {
                            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attr in startTag.attributes) {
                                if (!iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(attr.Key, HtmlTreeBuilderState.Constants.InBodyStartInputAttribs
                                    )) {
                                    inputAttribs.Put(attr);
                                }
                            }
                        }
                        inputAttribs.Put("name", "isindex");
                        tb.ProcessStartTag("input", inputAttribs);
                        tb.ProcessEndTag("label");
                        tb.ProcessStartTag("hr");
                        tb.ProcessEndTag("form");
                        break;
                    }

                    case "textarea": {
                        tb.Insert(startTag);
                        if (!startTag.IsSelfClosing()) {
                            tb.tokeniser.Transition(TokeniserState.Rcdata);
                            tb.MarkInsertionMode();
                            tb.FramesetOk(false);
                            tb.Transition(Text);
                        }
                        break;
                    }

                    case "xmp": {
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.ReconstructFormattingElements();
                        tb.FramesetOk(false);
                        HandleRawtext(startTag, tb);
                        break;
                    }

                    case "iframe": {
                        tb.FramesetOk(false);
                        HandleRawtext(startTag, tb);
                        break;
                    }

                    case "noembed": {
                        // also handle noscript if script enabled
                        HandleRawtext(startTag, tb);
                        break;
                    }

                    case "select": {
                        tb.ReconstructFormattingElements();
                        tb.Insert(startTag);
                        tb.FramesetOk(false);
                        HtmlTreeBuilderState state = tb.State();
                        if (state.Equals(InTable) || state.Equals(InCaption) || state.Equals(InTableBody) || state.Equals(InRow) ||
                             state.Equals(InCell)) {
                            tb.Transition(InSelectInTable);
                        }
                        else {
                            tb.Transition(InSelect);
                        }
                        break;
                    }

                    case "math": {
                        tb.ReconstructFormattingElements();
                        tb.Insert(startTag);
                        break;
                    }

                    case "svg": {
                        tb.ReconstructFormattingElements();
                        tb.Insert(startTag);
                        break;
                    }

                    // static final String[] Headings = new String[]{"h1", "h2", "h3", "h4", "h5", "h6"};
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6": {
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(tb.CurrentElement().NormalName(), HtmlTreeBuilderState.Constants
                            .Headings)) {
                            tb.Error(this);
                            tb.Pop();
                        }
                        tb.Insert(startTag);
                        break;
                    }

                    // static final String[] InBodyStartPreListing = new String[]{"listing", "pre"};
                    case "pre":
                    case "listing": {
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.Insert(startTag);
                        tb.reader.MatchConsume("\n");
                        // ignore LF if next token
                        tb.FramesetOk(false);
                        break;
                    }

                    // static final String[] DdDt = new String[]{"dd", "dt"};
                    case "dd":
                    case "dt": {
                        tb.FramesetOk(false);
                        stack = tb.GetStack();
                        for (int i = stack.Count - 1; i > 0; i--) {
                            el = stack[i];
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(el.NormalName(), HtmlTreeBuilderState.Constants
                                .DdDt)) {
                                tb.ProcessEndTag(el.NormalName());
                                break;
                            }
                            if (tb.IsSpecial(el) && !iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(el.NormalName(), HtmlTreeBuilderState.Constants
                                .InBodyStartLiBreakers)) {
                                break;
                            }
                        }
                        if (tb.InButtonScope("p")) {
                            tb.ProcessEndTag("p");
                        }
                        tb.Insert(startTag);
                        break;
                    }

                    // static final String[] InBodyStartOptions = new String[]{"optgroup", "option"};
                    case "optgroup":
                    case "option": {
                        if (tb.CurrentElement().NormalName().Equals("option")) {
                            tb.ProcessEndTag("option");
                        }
                        tb.ReconstructFormattingElements();
                        tb.Insert(startTag);
                        break;
                    }

                    // static final String[] InBodyStartRuby = new String[]{"rp", "rt"};
                    case "rp":
                    case "rt": {
                        if (tb.InScope("ruby")) {
                            tb.GenerateImpliedEndTags();
                            if (!tb.CurrentElement().NormalName().Equals("ruby")) {
                                tb.Error(this);
                                tb.PopStackToBefore("ruby");
                            }
                            // i.e. close up to but not include name
                            tb.Insert(startTag);
                        }
                        break;
                    }

                    default: {
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartEmptyFormatters
                            )) {
                            tb.ReconstructFormattingElements();
                            tb.InsertEmpty(startTag);
                            tb.FramesetOk(false);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartPClosers
                                )) {
                                if (tb.InButtonScope("p")) {
                                    tb.ProcessEndTag("p");
                                }
                                tb.Insert(startTag);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartToHead
                                    )) {
                                    return tb.Process(t, InHead);
                                }
                                else {
                                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.Formatters
                                        )) {
                                        tb.ReconstructFormattingElements();
                                        el = tb.Insert(startTag);
                                        tb.PushActiveFormattingElements(el);
                                    }
                                    else {
                                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartApplets
                                            )) {
                                            tb.ReconstructFormattingElements();
                                            tb.Insert(startTag);
                                            tb.InsertMarkerToFormattingElements();
                                            tb.FramesetOk(false);
                                        }
                                        else {
                                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartMedia
                                                )) {
                                                tb.InsertEmpty(startTag);
                                            }
                                            else {
                                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartDrop
                                                    )) {
                                                    tb.Error(this);
                                                    return false;
                                                }
                                                else {
                                                    tb.ReconstructFormattingElements();
                                                    tb.Insert(startTag);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
                return true;
            }

            private bool InBodyEndTag(Token t, HtmlTreeBuilder tb) {
                Token.EndTag endTag = t.AsEndTag();
                String name = endTag.NormalName();
                switch (name) {
                    case "sarcasm":
                    // *sigh*
                    case "span": {
                        // same as final fall through, but saves short circuit
                        return AnyOtherEndTag(t, tb);
                    }

                    case "li": {
                        if (!tb.InListItemScope(name)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.GenerateImpliedEndTags(name);
                            if (!tb.CurrentElement().NormalName().Equals(name)) {
                                tb.Error(this);
                            }
                            tb.PopStackToClose(name);
                        }
                        break;
                    }

                    case "body": {
                        if (!tb.InScope("body")) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.Transition(AfterBody);
                        }
                        break;
                    }

                    case "html": {
                        bool notIgnored = tb.ProcessEndTag("body");
                        if (notIgnored) {
                            return tb.Process(endTag);
                        }
                        break;
                    }

                    case "form": {
                        iText.StyledXmlParser.Jsoup.Nodes.Element currentForm = tb.GetFormElement();
                        tb.SetFormElement(null);
                        if (currentForm == null || !tb.InScope(name)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.GenerateImpliedEndTags();
                            if (!tb.CurrentElement().NormalName().Equals(name)) {
                                tb.Error(this);
                            }
                            // remove currentForm from stack. will shift anything under up.
                            tb.RemoveFromStack(currentForm);
                        }
                        break;
                    }

                    case "p": {
                        if (!tb.InButtonScope(name)) {
                            tb.Error(this);
                            tb.ProcessStartTag(name);
                            // if no p to close, creates an empty <p></p>
                            return tb.Process(endTag);
                        }
                        else {
                            tb.GenerateImpliedEndTags(name);
                            if (!tb.CurrentElement().NormalName().Equals(name)) {
                                tb.Error(this);
                            }
                            tb.PopStackToClose(name);
                        }
                        break;
                    }

                    case "dd":
                    case "dt": {
                        if (!tb.InScope(name)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.GenerateImpliedEndTags(name);
                            if (!tb.CurrentElement().NormalName().Equals(name)) {
                                tb.Error(this);
                            }
                            tb.PopStackToClose(name);
                        }
                        break;
                    }

                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6": {
                        if (!tb.InScope(HtmlTreeBuilderState.Constants.Headings)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.GenerateImpliedEndTags(name);
                            if (!tb.CurrentElement().NormalName().Equals(name)) {
                                tb.Error(this);
                            }
                            tb.PopStackToClose(HtmlTreeBuilderState.Constants.Headings);
                        }
                        break;
                    }

                    case "br": {
                        tb.Error(this);
                        tb.ProcessStartTag("br");
                        return false;
                    }

                    default: {
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyEndAdoptionFormatters
                            )) {
                            return InBodyEndTagAdoption(t, tb);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyEndClosers
                                )) {
                                if (!tb.InScope(name)) {
                                    // nothing to close
                                    tb.Error(this);
                                    return false;
                                }
                                else {
                                    tb.GenerateImpliedEndTags();
                                    if (!tb.CurrentElement().NormalName().Equals(name)) {
                                        tb.Error(this);
                                    }
                                    tb.PopStackToClose(name);
                                }
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartApplets
                                    )) {
                                    if (!tb.InScope("name")) {
                                        if (!tb.InScope(name)) {
                                            tb.Error(this);
                                            return false;
                                        }
                                        tb.GenerateImpliedEndTags();
                                        if (!tb.CurrentElement().NormalName().Equals(name)) {
                                            tb.Error(this);
                                        }
                                        tb.PopStackToClose(name);
                                        tb.ClearFormattingElementsToLastMarker();
                                    }
                                }
                                else {
                                    return AnyOtherEndTag(t, tb);
                                }
                            }
                        }
                        break;
                    }
                }
                return true;
            }

//\cond DO_NOT_DOCUMENT
            internal bool AnyOtherEndTag(Token t, HtmlTreeBuilder tb) {
                String name = t.AsEndTag().normalName;
                // case insensitive search - goal is to preserve output case, not for the parse to be case sensitive
                List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                for (int pos = stack.Count - 1; pos >= 0; pos--) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element node = stack[pos];
                    if (node.NormalName().Equals(name)) {
                        tb.GenerateImpliedEndTags(name);
                        if (!name.Equals(tb.CurrentElement().NormalName())) {
                            tb.Error(this);
                        }
                        tb.PopStackToClose(name);
                        break;
                    }
                    else {
                        if (tb.IsSpecial(node)) {
                            tb.Error(this);
                            return false;
                        }
                    }
                }
                return true;
            }
//\endcond

            // Adoption Agency Algorithm.
            private bool InBodyEndTagAdoption(Token t, HtmlTreeBuilder tb) {
                Token.EndTag endTag = t.AsEndTag();
                String name = endTag.NormalName();
                List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                iText.StyledXmlParser.Jsoup.Nodes.Element el;
                for (int i = 0; i < 8; i++) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element formatEl = tb.GetActiveFormattingElement(name);
                    if (formatEl == null) {
                        return AnyOtherEndTag(t, tb);
                    }
                    else {
                        if (!tb.OnStack(formatEl)) {
                            tb.Error(this);
                            tb.RemoveFromActiveFormattingElements(formatEl);
                            return true;
                        }
                        else {
                            if (!tb.InScope(formatEl.NormalName())) {
                                tb.Error(this);
                                return false;
                            }
                            else {
                                if (tb.CurrentElement() != formatEl) {
                                    tb.Error(this);
                                }
                            }
                        }
                    }
                    iText.StyledXmlParser.Jsoup.Nodes.Element furthestBlock = null;
                    iText.StyledXmlParser.Jsoup.Nodes.Element commonAncestor = null;
                    bool seenFormattingElement = false;
                    // the spec doesn't limit to < 64, but in degenerate cases (9000+ stack depth) this prevents
                    // run-aways
                    int stackSize = stack.Count;
                    int bookmark = -1;
                    for (int si = 0; si < stackSize && si < 64; si++) {
                        el = stack[si];
                        if (el == formatEl) {
                            commonAncestor = stack[si - 1];
                            seenFormattingElement = true;
                            // Let a bookmark note the position of the formatting element in the list of active formatting elements relative to the elements on either side of it in the list.
                            bookmark = tb.PositionOfElement(el);
                        }
                        else {
                            if (seenFormattingElement && tb.IsSpecial(el)) {
                                furthestBlock = el;
                                break;
                            }
                        }
                    }
                    if (furthestBlock == null) {
                        tb.PopStackToClose(formatEl.NormalName());
                        tb.RemoveFromActiveFormattingElements(formatEl);
                        return true;
                    }
                    iText.StyledXmlParser.Jsoup.Nodes.Element node = furthestBlock;
                    iText.StyledXmlParser.Jsoup.Nodes.Element lastNode = furthestBlock;
                    for (int j = 0; j < 3; j++) {
                        if (tb.OnStack(node)) {
                            node = tb.AboveOnStack(node);
                        }
                        if (!tb.IsInActiveFormattingElements(node)) {
                            // note no bookmark check
                            tb.RemoveFromStack(node);
                            continue;
                        }
                        else {
                            if (node == formatEl) {
                                break;
                            }
                        }
                        iText.StyledXmlParser.Jsoup.Nodes.Element replacement = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                            .ValueOf(node.NodeName(), ParseSettings.preserveCase), tb.GetBaseUri());
                        // case will follow the original node (so honours ParseSettings)
                        tb.ReplaceActiveFormattingElement(node, replacement);
                        tb.ReplaceOnStack(node, replacement);
                        node = replacement;
                        if (lastNode == furthestBlock) {
                            // move the aforementioned bookmark to be immediately after the new node in the list of active formatting elements.
                            // not getting how this bookmark both straddles the element above, but is inbetween here...
                            bookmark = tb.PositionOfElement(node) + 1;
                        }
                        if (lastNode.Parent() != null) {
                            lastNode.Remove();
                        }
                        node.AppendChild(lastNode);
                        lastNode = node;
                    }
                    if (commonAncestor != null) {
                        // safety check, but would be an error if null
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(commonAncestor.NormalName(), HtmlTreeBuilderState.Constants
                            .InBodyEndTableFosters)) {
                            if (lastNode.Parent() != null) {
                                lastNode.Remove();
                            }
                            tb.InsertInFosterParent(lastNode);
                        }
                        else {
                            if (lastNode.Parent() != null) {
                                lastNode.Remove();
                            }
                            commonAncestor.AppendChild(lastNode);
                        }
                    }
                    iText.StyledXmlParser.Jsoup.Nodes.Element adopter = new iText.StyledXmlParser.Jsoup.Nodes.Element(formatEl
                        .Tag(), tb.GetBaseUri());
                    adopter.Attributes().AddAll(formatEl.Attributes());
                    iText.StyledXmlParser.Jsoup.Nodes.Node[] childNodes = furthestBlock.ChildNodes().ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node
                        [0]);
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Node childNode in childNodes) {
                        adopter.AppendChild(childNode);
                    }
                    // append will reparent. thus the clone to avoid concurrent mod.
                    furthestBlock.AppendChild(adopter);
                    tb.RemoveFromActiveFormattingElements(formatEl);
                    // insert the new element into the list of active formatting elements at the position of the aforementioned bookmark.
                    tb.PushWithBookmark(adopter, bookmark);
                    tb.RemoveFromStack(formatEl);
                    tb.InsertOnStackAfter(furthestBlock, adopter);
                }
                return true;
            }
        }

        private sealed class TextBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "Text";
            }

//\cond DO_NOT_DOCUMENT
            // in script, style etc. normally treated as data tags
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsCharacter()) {
                    tb.Insert(t.AsCharacter());
                }
                else {
                    if (t.IsEOF()) {
                        tb.Error(this);
                        // if current node is script: already started
                        tb.Pop();
                        tb.Transition(tb.OriginalState());
                        return tb.Process(t);
                    }
                    else {
                        if (t.IsEndTag()) {
                            // if: An end tag whose tag name is "script" -- scripting nesting level, if evaluating scripts
                            tb.Pop();
                            tb.Transition(tb.OriginalState());
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class InTableBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InTable";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsCharacter()) {
                    tb.NewPendingTableCharacters();
                    tb.MarkInsertionMode();
                    tb.Transition(InTableText);
                    return tb.Process(t);
                }
                else {
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                        return true;
                    }
                    else {
                        if (t.IsDoctype()) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (t.IsStartTag()) {
                                Token.StartTag startTag = t.AsStartTag();
                                String name = startTag.NormalName();
                                if (name.Equals("caption")) {
                                    tb.ClearStackToTableContext();
                                    tb.InsertMarkerToFormattingElements();
                                    tb.Insert(startTag);
                                    tb.Transition(InCaption);
                                }
                                else {
                                    if (name.Equals("colgroup")) {
                                        tb.ClearStackToTableContext();
                                        tb.Insert(startTag);
                                        tb.Transition(InColumnGroup);
                                    }
                                    else {
                                        if (name.Equals("col")) {
                                            tb.ProcessStartTag("colgroup");
                                            return tb.Process(t);
                                        }
                                        else {
                                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableToBody
                                                )) {
                                                tb.ClearStackToTableContext();
                                                tb.Insert(startTag);
                                                tb.Transition(InTableBody);
                                            }
                                            else {
                                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableAddBody
                                                    )) {
                                                    tb.ProcessStartTag("tbody");
                                                    return tb.Process(t);
                                                }
                                                else {
                                                    if (name.Equals("table")) {
                                                        tb.Error(this);
                                                        bool processed = tb.ProcessEndTag("table");
                                                        if (processed) {
                                                            // only ignored if in fragment
                                                            return tb.Process(t);
                                                        }
                                                    }
                                                    else {
                                                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableToHead
                                                            )) {
                                                            return tb.Process(t, InHead);
                                                        }
                                                        else {
                                                            if (name.Equals("input")) {
                                                                if (!(startTag.HasAttributes() && startTag.attributes.Get("type").EqualsIgnoreCase("hidden"))) {
                                                                    return AnythingElse(t, tb);
                                                                }
                                                                else {
                                                                    tb.InsertEmpty(startTag);
                                                                }
                                                            }
                                                            else {
                                                                if (name.Equals("form")) {
                                                                    tb.Error(this);
                                                                    if (tb.GetFormElement() != null) {
                                                                        return false;
                                                                    }
                                                                    else {
                                                                        tb.InsertForm(startTag, false);
                                                                    }
                                                                }
                                                                else {
                                                                    return AnythingElse(t, tb);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                return true;
                            }
                            else {
                                if (t.IsEndTag()) {
                                    Token.EndTag endTag = t.AsEndTag();
                                    String name = endTag.NormalName();
                                    if (name.Equals("table")) {
                                        if (!tb.InTableScope(name)) {
                                            tb.Error(this);
                                            return false;
                                        }
                                        else {
                                            tb.PopStackToClose("table");
                                        }
                                        tb.ResetInsertionMode();
                                    }
                                    else {
                                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableEndErr
                                            )) {
                                            tb.Error(this);
                                            return false;
                                        }
                                        else {
                                            return AnythingElse(t, tb);
                                        }
                                    }
                                    return true;
                                }
                                else {
                                    if (t.IsEOF()) {
                                        if (tb.CurrentElement().NormalName().Equals("html")) {
                                            tb.Error(this);
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                // stops parsing
                return AnythingElse(t, tb);
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Error(this);
                bool processed;
                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(tb.CurrentElement().NormalName(), HtmlTreeBuilderState.Constants
                    .InTableFoster)) {
                    tb.SetFosterInserts(true);
                    processed = tb.Process(t, InBody);
                    tb.SetFosterInserts(false);
                }
                else {
                    processed = tb.Process(t, InBody);
                }
                return processed;
            }
//\endcond
        }

        private sealed class InTableTextBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InTableText";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.type == iText.StyledXmlParser.Jsoup.Parser.TokenType.Character) {
                    Token.Character c = t.AsCharacter();
                    if (c.GetData().Equals(nullString)) {
                        tb.Error(this);
                        return false;
                    }
                    else {
                        tb.GetPendingTableCharacters().Add(c.GetData());
                    }
                }
                else {
                    if (tb.GetPendingTableCharacters().Count > 0) {
                        foreach (String character in tb.GetPendingTableCharacters()) {
                            if (!IsWhitespace(character)) {
                                // InTable anything else section:
                                tb.Error(this);
                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(tb.CurrentElement().NormalName(), HtmlTreeBuilderState.Constants
                                    .InTableFoster)) {
                                    tb.SetFosterInserts(true);
                                    tb.Process(new Token.Character().Data(character), InBody);
                                    tb.SetFosterInserts(false);
                                }
                                else {
                                    tb.Process(new Token.Character().Data(character), InBody);
                                }
                            }
                            else {
                                tb.Insert(new Token.Character().Data(character));
                            }
                        }
                        tb.NewPendingTableCharacters();
                    }
                    tb.Transition(tb.OriginalState());
                    return tb.Process(t);
                }
                return true;
            }
//\endcond
        }

        private sealed class InCaptionBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InCaption";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsEndTag() && t.AsEndTag().NormalName().Equals("caption")) {
                    Token.EndTag endTag = t.AsEndTag();
                    String name = endTag.NormalName();
                    if (!tb.InTableScope(name)) {
                        tb.Error(this);
                        return false;
                    }
                    else {
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement().NormalName().Equals("caption")) {
                            tb.Error(this);
                        }
                        tb.PopStackToClose("caption");
                        tb.ClearFormattingElementsToLastMarker();
                        tb.Transition(InTable);
                    }
                }
                else {
                    if ((t.IsStartTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsStartTag().NormalName(
                        ), HtmlTreeBuilderState.Constants.InCellCol) || t.IsEndTag() && t.AsEndTag().NormalName().Equals("table"
                        ))) {
                        tb.Error(this);
                        bool processed = tb.ProcessEndTag("caption");
                        if (processed) {
                            return tb.Process(t);
                        }
                    }
                    else {
                        if (t.IsEndTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsEndTag().NormalName(), HtmlTreeBuilderState.Constants
                            .InCaptionIgnore)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            return tb.Process(t, InBody);
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class InColumnGroupBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InColumnGroup";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                    return true;
                }
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment: {
                        tb.Insert(t.AsComment());
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype: {
                        tb.Error(this);
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        Token.StartTag startTag = t.AsStartTag();
                        switch (startTag.NormalName()) {
                            case "html": {
                                return tb.Process(t, InBody);
                            }

                            case "col": {
                                tb.InsertEmpty(startTag);
                                break;
                            }

                            default: {
                                return AnythingElse(t, tb);
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        Token.EndTag endTag = t.AsEndTag();
                        if (endTag.normalName.Equals("colgroup")) {
                            if (tb.CurrentElement().NormalName().Equals("html")) {
                                // frag case
                                tb.Error(this);
                                return false;
                            }
                            else {
                                tb.Pop();
                                tb.Transition(InTable);
                            }
                        }
                        else {
                            return AnythingElse(t, tb);
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                        if (tb.CurrentElement().NormalName().Equals("html")) {
                            return true;
                        }
                        else {
                            // stop parsing; frag case
                            return AnythingElse(t, tb);
                        }
                        goto default;
                    }

                    default: {
                        return AnythingElse(t, tb);
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, TreeBuilder tb) {
                bool processed = tb.ProcessEndTag("colgroup");
                if (processed) {
                    // only ignored in frag case
                    return tb.Process(t);
                }
                return true;
            }
        }

        private sealed class InTableBodyBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InTableBody";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        Token.StartTag startTag = t.AsStartTag();
                        name = startTag.NormalName();
                        if (name.Equals("template")) {
                            tb.Insert(startTag);
                        }
                        else {
                            if (name.Equals("tr")) {
                                tb.ClearStackToTableBodyContext();
                                tb.Insert(startTag);
                                tb.Transition(InRow);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InCellNames
                                    )) {
                                    tb.Error(this);
                                    tb.ProcessStartTag("tr");
                                    return tb.Process(startTag);
                                }
                                else {
                                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableBodyExit
                                        )) {
                                        return ExitTableBody(t, tb);
                                    }
                                    else {
                                        return AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        Token.EndTag endTag = t.AsEndTag();
                        name = endTag.NormalName();
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableEndIgnore
                            )) {
                            if (!tb.InTableScope(name)) {
                                tb.Error(this);
                                return false;
                            }
                            else {
                                tb.ClearStackToTableBodyContext();
                                tb.Pop();
                                tb.Transition(InTable);
                            }
                        }
                        else {
                            if (name.Equals("table")) {
                                return ExitTableBody(t, tb);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableBodyEndIgnore
                                    )) {
                                    tb.Error(this);
                                    return false;
                                }
                                else {
                                    return AnythingElse(t, tb);
                                }
                            }
                        }
                        break;
                    }

                    default: {
                        return AnythingElse(t, tb);
                    }
                }
                return true;
            }
//\endcond

            private bool ExitTableBody(Token t, HtmlTreeBuilder tb) {
                if (!(tb.InTableScope("tbody") || tb.InTableScope("thead") || tb.InScope("tfoot"))) {
                    // frag case
                    tb.Error(this);
                    return false;
                }
                tb.ClearStackToTableBodyContext();
                tb.ProcessEndTag(tb.CurrentElement().NormalName());
                // tbody, tfoot, thead
                return tb.Process(t);
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, InTable);
            }
        }

        private sealed class InRowBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InRow";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsStartTag()) {
                    Token.StartTag startTag = t.AsStartTag();
                    String name = startTag.NormalName();
                    if (name.Equals("template")) {
                        tb.Insert(startTag);
                    }
                    else {
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InCellNames
                            )) {
                            tb.ClearStackToTableRowContext();
                            tb.Insert(startTag);
                            tb.Transition(InCell);
                            tb.InsertMarkerToFormattingElements();
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InRowMissing
                                )) {
                                return HandleMissingTr(t, tb);
                            }
                            else {
                                return AnythingElse(t, tb);
                            }
                        }
                    }
                }
                else {
                    if (t.IsEndTag()) {
                        Token.EndTag endTag = t.AsEndTag();
                        String name = endTag.NormalName();
                        if (name.Equals("tr")) {
                            if (!tb.InTableScope(name)) {
                                tb.Error(this);
                                // frag
                                return false;
                            }
                            tb.ClearStackToTableRowContext();
                            tb.Pop();
                            // tr
                            tb.Transition(InTableBody);
                        }
                        else {
                            if (name.Equals("table")) {
                                return HandleMissingTr(t, tb);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InTableToBody
                                    )) {
                                    if (!tb.InTableScope(name) || !tb.InTableScope("tr")) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    tb.ClearStackToTableRowContext();
                                    tb.Pop();
                                    // tr
                                    tb.Transition(InTableBody);
                                }
                                else {
                                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InRowIgnore
                                        )) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        return AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        return AnythingElse(t, tb);
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, InTable);
            }

            private bool HandleMissingTr(Token t, TreeBuilder tb) {
                bool processed = tb.ProcessEndTag("tr");
                if (processed) {
                    return tb.Process(t);
                }
                else {
                    return false;
                }
            }
        }

        private sealed class InCellBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InCell";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsEndTag()) {
                    Token.EndTag endTag = t.AsEndTag();
                    String name = endTag.NormalName();
                    if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InCellNames
                        )) {
                        if (!tb.InTableScope(name)) {
                            tb.Error(this);
                            tb.Transition(InRow);
                            // might not be in scope if empty: <td /> and processing fake end tag
                            return false;
                        }
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement().NormalName().Equals(name)) {
                            tb.Error(this);
                        }
                        tb.PopStackToClose(name);
                        tb.ClearFormattingElementsToLastMarker();
                        tb.Transition(InRow);
                    }
                    else {
                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InCellBody
                            )) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InCellTable
                                )) {
                                if (!tb.InTableScope(name)) {
                                    tb.Error(this);
                                    return false;
                                }
                                CloseCell(tb);
                                return tb.Process(t);
                            }
                            else {
                                return AnythingElse(t, tb);
                            }
                        }
                    }
                }
                else {
                    if (t.IsStartTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsStartTag().NormalName()
                        , HtmlTreeBuilderState.Constants.InCellCol)) {
                        if (!(tb.InTableScope("td") || tb.InTableScope("th"))) {
                            tb.Error(this);
                            return false;
                        }
                        CloseCell(tb);
                        return tb.Process(t);
                    }
                    else {
                        return AnythingElse(t, tb);
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, InBody);
            }

            private void CloseCell(HtmlTreeBuilder tb) {
                if (tb.InTableScope("td")) {
                    tb.ProcessEndTag("td");
                }
                else {
                    tb.ProcessEndTag("th");
                }
            }
            // only here if th or td in scope
        }

        private sealed class InSelectBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InSelect";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                        Token.Character c = t.AsCharacter();
                        if (c.GetData().Equals(nullString)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.Insert(c);
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Comment: {
                        tb.Insert(t.AsComment());
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Doctype: {
                        tb.Error(this);
                        return false;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        Token.StartTag start = t.AsStartTag();
                        name = start.NormalName();
                        if (name.Equals("html")) {
                            return tb.Process(start, InBody);
                        }
                        else {
                            if (name.Equals("option")) {
                                if (tb.CurrentElement().NormalName().Equals("option")) {
                                    tb.ProcessEndTag("option");
                                }
                                tb.Insert(start);
                            }
                            else {
                                if (name.Equals("optgroup")) {
                                    if (tb.CurrentElement().NormalName().Equals("option")) {
                                        tb.ProcessEndTag("option");
                                    }
                                    // pop option and flow to pop optgroup
                                    if (tb.CurrentElement().NormalName().Equals("optgroup")) {
                                        tb.ProcessEndTag("optgroup");
                                    }
                                    tb.Insert(start);
                                }
                                else {
                                    if (name.Equals("select")) {
                                        tb.Error(this);
                                        return tb.ProcessEndTag("select");
                                    }
                                    else {
                                        if (iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InSelectEnd
                                            )) {
                                            tb.Error(this);
                                            if (!tb.InSelectScope("select")) {
                                                return false;
                                            }
                                            // frag
                                            tb.ProcessEndTag("select");
                                            return tb.Process(start);
                                        }
                                        else {
                                            if (name.Equals("script")) {
                                                return tb.Process(t, InHead);
                                            }
                                            else {
                                                return AnythingElse(t, tb);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        Token.EndTag end = t.AsEndTag();
                        name = end.NormalName();
                        switch (name) {
                            case "optgroup": {
                                if (tb.CurrentElement().NormalName().Equals("option") && tb.AboveOnStack(tb.CurrentElement()) != null && tb
                                    .AboveOnStack(tb.CurrentElement()).NormalName().Equals("optgroup")) {
                                    tb.ProcessEndTag("option");
                                }
                                if (tb.CurrentElement().NormalName().Equals("optgroup")) {
                                    tb.Pop();
                                }
                                else {
                                    tb.Error(this);
                                }
                                break;
                            }

                            case "option": {
                                if (tb.CurrentElement().NormalName().Equals("option")) {
                                    tb.Pop();
                                }
                                else {
                                    tb.Error(this);
                                }
                                break;
                            }

                            case "select": {
                                if (!tb.InSelectScope(name)) {
                                    tb.Error(this);
                                    return false;
                                }
                                else {
                                    tb.PopStackToClose(name);
                                    tb.ResetInsertionMode();
                                }
                                break;
                            }

                            default: {
                                return AnythingElse(t, tb);
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                        if (!tb.CurrentElement().NormalName().Equals("html")) {
                            tb.Error(this);
                        }
                        break;
                    }

                    default: {
                        return AnythingElse(t, tb);
                    }
                }
                return true;
            }
//\endcond

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Error(this);
                return false;
            }
        }

        private sealed class InSelectInTableBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InSelectInTable";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsStartTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsStartTag().NormalName()
                    , HtmlTreeBuilderState.Constants.InSelecTableEnd)) {
                    tb.Error(this);
                    tb.ProcessEndTag("select");
                    return tb.Process(t);
                }
                else {
                    if (t.IsEndTag() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.InSorted(t.AsEndTag().NormalName(), HtmlTreeBuilderState.Constants
                        .InSelecTableEnd)) {
                        tb.Error(this);
                        if (tb.InTableScope(t.AsEndTag().NormalName())) {
                            tb.ProcessEndTag("select");
                            return (tb.Process(t));
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return tb.Process(t, InSelect);
                    }
                }
            }
//\endcond
        }

        private sealed class AfterBodyBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "AfterBody";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                }
                else {
                    // out of spec - include whitespace. spec would move into body
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        // into html node
                        if (t.IsDoctype()) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html")) {
                                return tb.Process(t, InBody);
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().NormalName().Equals("html")) {
                                    if (tb.IsFragmentParsing()) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        tb.Transition(AfterAfterBody);
                                    }
                                }
                                else {
                                    if (t.IsEOF()) {
                                    }
                                    else {
                                        // chillax! we're done
                                        tb.Error(this);
                                        tb.Transition(InBody);
                                        return tb.Process(t);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class InFrameSetBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "InFrameset";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                }
                else {
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (t.IsDoctype()) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (t.IsStartTag()) {
                                Token.StartTag start = t.AsStartTag();
                                switch (start.NormalName()) {
                                    case "html": {
                                        return tb.Process(start, InBody);
                                    }

                                    case "frameset": {
                                        tb.Insert(start);
                                        break;
                                    }

                                    case "frame": {
                                        tb.InsertEmpty(start);
                                        break;
                                    }

                                    case "noframes": {
                                        return tb.Process(start, InHead);
                                    }

                                    default: {
                                        tb.Error(this);
                                        return false;
                                    }
                                }
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().NormalName().Equals("frameset")) {
                                    if (tb.CurrentElement().NormalName().Equals("html")) {
                                        // frag
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        tb.Pop();
                                        if (!tb.IsFragmentParsing() && !tb.CurrentElement().NormalName().Equals("frameset")) {
                                            tb.Transition(AfterFrameset);
                                        }
                                    }
                                }
                                else {
                                    if (t.IsEOF()) {
                                        if (!tb.CurrentElement().NormalName().Equals("html")) {
                                            tb.Error(this);
                                            return true;
                                        }
                                    }
                                    else {
                                        tb.Error(this);
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class AfterFrameSetBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "AfterFrameset";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (IsWhitespace(t)) {
                    tb.Insert(t.AsCharacter());
                }
                else {
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (t.IsDoctype()) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html")) {
                                return tb.Process(t, InBody);
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().NormalName().Equals("html")) {
                                    tb.Transition(AfterAfterFrameset);
                                }
                                else {
                                    if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("noframes")) {
                                        return tb.Process(t, InHead);
                                    }
                                    else {
                                        if (t.IsEOF()) {
                                        }
                                        else {
                                            // cool your heels, we're complete
                                            tb.Error(this);
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class AfterAfterBodyBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "AfterAfterBody";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsComment()) {
                    tb.Insert(t.AsComment());
                }
                else {
                    if (t.IsDoctype() || (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html"))) {
                        return tb.Process(t, InBody);
                    }
                    else {
                        if (IsWhitespace(t)) {
                            // allows space after </html>, and put the body back on stack to allow subsequent tags if any
                            iText.StyledXmlParser.Jsoup.Nodes.Element html = tb.PopStackToClose("html");
                            tb.Insert(t.AsCharacter());
                            tb.stack.Add(html);
                            tb.stack.Add(html.SelectFirst("body"));
                        }
                        else {
                            if (t.IsEOF()) {
                            }
                            else {
                                // nice work chuck
                                tb.Error(this);
                                tb.Transition(InBody);
                                return tb.Process(t);
                            }
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class AfterAfterFrameSetBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "AfterAfterFrameset";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsComment()) {
                    tb.Insert(t.AsComment());
                }
                else {
                    if (t.IsDoctype() || IsWhitespace(t) || (t.IsStartTag() && t.AsStartTag().NormalName().Equals("html"))) {
                        return tb.Process(t, InBody);
                    }
                    else {
                        if (t.IsEOF()) {
                        }
                        else {
                            // nice work chuck
                            if (t.IsStartTag() && t.AsStartTag().NormalName().Equals("noframes")) {
                                return tb.Process(t, InHead);
                            }
                            else {
                                tb.Error(this);
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
//\endcond
        }

        private sealed class ForeignContentBS : HtmlTreeBuilderState {
            public override String ToString() {
                return "ForeignContent";
            }

//\cond DO_NOT_DOCUMENT
            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                return true;
            }
//\endcond
        }
    }
//\endcond
}
