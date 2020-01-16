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
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>The Tree Builder's current state.</summary>
    /// <remarks>The Tree Builder's current state. Each state embodies the processing for the state, and transitions to other states.
    ///     </remarks>
    internal abstract class HtmlTreeBuilderState {
        private sealed class _HtmlTreeBuilderState_60 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_60() {
            }

            internal override String GetName() {
                return "Initial";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
                    return true;
                }
                else {
                    // ignore whitespace
                    if (t.IsComment()) {
                        tb.Insert(t.AsComment());
                    }
                    else {
                        if (t.IsDoctype()) {
                            // todo: parse error check on expected doctypes
                            // todo: quirk state check on doctype ids
                            Token.Doctype d = t.AsDoctype();
                            DocumentType doctype = new DocumentType(d.GetName(), d.GetPublicIdentifier(), d.GetSystemIdentifier(), tb.
                                GetBaseUri());
                            tb.GetDocument().AppendChild(doctype);
                            if (d.IsForceQuirks()) {
                                tb.GetDocument().QuirksMode(QuirksMode.quirks);
                            }
                            tb.Transition(HtmlTreeBuilderState.BeforeHtml);
                        }
                        else {
                            // todo: check not iframe srcdoc
                            tb.Transition(HtmlTreeBuilderState.BeforeHtml);
                            return tb.Process(t);
                        }
                    }
                }
                // re-process token
                return true;
            }
        }

        internal static HtmlTreeBuilderState Initial = new _HtmlTreeBuilderState_60();

        private sealed class _HtmlTreeBuilderState_90 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_90() {
            }

            internal override String GetName() {
                return "BeforeHtml";
            }

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
                        if (HtmlTreeBuilderState.IsWhitespace(t)) {
                            return true;
                        }
                        else {
                            // ignore whitespace
                            if (t.IsStartTag() && t.AsStartTag().Name().Equals("html")) {
                                tb.Insert(t.AsStartTag());
                                tb.Transition(HtmlTreeBuilderState.BeforeHead);
                            }
                            else {
                                if (t.IsEndTag() && (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsEndTag().Name(), "head", "body", 
                                    "html", "br"))) {
                                    return this.AnythingElse(t, tb);
                                }
                                else {
                                    if (t.IsEndTag()) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        return this.AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.InsertStartTag("html");
                tb.Transition(HtmlTreeBuilderState.BeforeHead);
                return tb.Process(t);
            }
        }

        internal static HtmlTreeBuilderState BeforeHtml = new _HtmlTreeBuilderState_90();

        private sealed class _HtmlTreeBuilderState_126 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_126() {
            }

            internal override String GetName() {
                return "BeforeHead";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
                    return true;
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
                            if (t.IsStartTag() && t.AsStartTag().Name().Equals("html")) {
                                return HtmlTreeBuilderState.InBody.Process(t, tb);
                            }
                            else {
                                // does not transition
                                if (t.IsStartTag() && t.AsStartTag().Name().Equals("head")) {
                                    iText.StyledXmlParser.Jsoup.Nodes.Element head = tb.Insert(t.AsStartTag());
                                    tb.SetHeadElement(head);
                                    tb.Transition(HtmlTreeBuilderState.InHead);
                                }
                                else {
                                    if (t.IsEndTag() && (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsEndTag().Name(), "head", "body", 
                                        "html", "br"))) {
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
        }

        internal static HtmlTreeBuilderState BeforeHead = new _HtmlTreeBuilderState_126();

        private sealed class _HtmlTreeBuilderState_161 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_161() {
            }

            internal override String GetName() {
                return "InHead";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
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
                        return false;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        Token.StartTag start = t.AsStartTag();
                        name = start.Name();
                        if (name.Equals("html")) {
                            return HtmlTreeBuilderState.InBody.Process(t, tb);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "base", "basefont", "bgsound", "command", "link"
                                )) {
                                iText.StyledXmlParser.Jsoup.Nodes.Element el = tb.InsertEmpty(start);
                                // jsoup special: update base the frist time it is seen
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
                                        HtmlTreeBuilderState.HandleRcData(start, tb);
                                    }
                                    else {
                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "noframes", "style")) {
                                            HtmlTreeBuilderState.HandleRawtext(start, tb);
                                        }
                                        else {
                                            if (name.Equals("noscript")) {
                                                // else if noscript && scripting flag = true: rawtext (jsoup doesn't run script, to handle as noscript)
                                                tb.Insert(start);
                                                tb.Transition(HtmlTreeBuilderState.InHeadNoscript);
                                            }
                                            else {
                                                if (name.Equals("script")) {
                                                    // skips some script rules as won't execute them
                                                    tb.tokeniser.Transition(TokeniserState.ScriptData);
                                                    tb.MarkInsertionMode();
                                                    tb.Transition(HtmlTreeBuilderState.Text);
                                                    tb.Insert(start);
                                                }
                                                else {
                                                    if (name.Equals("head")) {
                                                        tb.Error(this);
                                                        return false;
                                                    }
                                                    else {
                                                        return this.AnythingElse(t, tb);
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
                        name = end.Name();
                        if (name.Equals("head")) {
                            tb.Pop();
                            tb.Transition(HtmlTreeBuilderState.AfterHead);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "body", "html", "br")) {
                                return this.AnythingElse(t, tb);
                            }
                            else {
                                tb.Error(this);
                                return false;
                            }
                        }
                        break;
                    }

                    default: {
                        return this.AnythingElse(t, tb);
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, TreeBuilder tb) {
                tb.ProcessEndTag("head");
                return tb.Process(t);
            }
        }

        internal static HtmlTreeBuilderState InHead = new _HtmlTreeBuilderState_161();

        private sealed class _HtmlTreeBuilderState_240 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_240() {
            }

            internal override String GetName() {
                return "InHeadNoscript";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsDoctype()) {
                    tb.Error(this);
                }
                else {
                    if (t.IsStartTag() && t.AsStartTag().Name().Equals("html")) {
                        return tb.Process(t, HtmlTreeBuilderState.InBody);
                    }
                    else {
                        if (t.IsEndTag() && t.AsEndTag().Name().Equals("noscript")) {
                            tb.Pop();
                            tb.Transition(HtmlTreeBuilderState.InHead);
                        }
                        else {
                            if (HtmlTreeBuilderState.IsWhitespace(t) || t.IsComment() || (t.IsStartTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil
                                .In(t.AsStartTag().Name(), "basefont", "bgsound", "link", "meta", "noframes", "style"))) {
                                return tb.Process(t, HtmlTreeBuilderState.InHead);
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().Name().Equals("br")) {
                                    return this.AnythingElse(t, tb);
                                }
                                else {
                                    if ((t.IsStartTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsStartTag().Name(), "head", "noscript"
                                        )) || t.IsEndTag()) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        return this.AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Error(this);
                tb.Insert(new Token.Character().Data(t.ToString()));
                return true;
            }
        }

        internal static HtmlTreeBuilderState InHeadNoscript = new _HtmlTreeBuilderState_240();

        private sealed class _HtmlTreeBuilderState_276 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_276() {
            }

            internal override String GetName() {
                return "AfterHead";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
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
                                name = startTag.Name();
                                if (name.Equals("html")) {
                                    return tb.Process(t, HtmlTreeBuilderState.InBody);
                                }
                                else {
                                    if (name.Equals("body")) {
                                        tb.Insert(startTag);
                                        tb.FramesetOk(false);
                                        tb.Transition(HtmlTreeBuilderState.InBody);
                                    }
                                    else {
                                        if (name.Equals("frameset")) {
                                            tb.Insert(startTag);
                                            tb.Transition(HtmlTreeBuilderState.InFrameset);
                                        }
                                        else {
                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "base", "basefont", "bgsound", "link", "meta", 
                                                "noframes", "script", "style", "title")) {
                                                tb.Error(this);
                                                iText.StyledXmlParser.Jsoup.Nodes.Element head = tb.GetHeadElement();
                                                tb.Push(head);
                                                tb.Process(t, HtmlTreeBuilderState.InHead);
                                                tb.RemoveFromStack(head);
                                            }
                                            else {
                                                if (name.Equals("head")) {
                                                    tb.Error(this);
                                                    return false;
                                                }
                                                else {
                                                    this.AnythingElse(t, tb);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                if (t.IsEndTag()) {
                                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsEndTag().Name(), "body", "html")) {
                                        this.AnythingElse(t, tb);
                                    }
                                    else {
                                        tb.Error(this);
                                        return false;
                                    }
                                }
                                else {
                                    this.AnythingElse(t, tb);
                                }
                            }
                        }
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.ProcessStartTag("body");
                tb.FramesetOk(true);
                return tb.Process(t);
            }
        }

        internal static HtmlTreeBuilderState AfterHead = new _HtmlTreeBuilderState_276();

        private sealed class _HtmlTreeBuilderState_335 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_335() {
            }

            internal override String GetName() {
                return "InBody";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                        Token.Character c = t.AsCharacter();
                        if (c.GetData().Equals(HtmlTreeBuilderState.nullString)) {
                            // todo confirm that check
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (tb.FramesetOk() && HtmlTreeBuilderState.IsWhitespace(c)) {
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
                        Token.StartTag startTag = t.AsStartTag();
                        name = startTag.Name();
                        if (name.Equals("a")) {
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
                            iText.StyledXmlParser.Jsoup.Nodes.Element a = tb.Insert(startTag);
                            tb.PushActiveFormattingElements(a);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartEmptyFormatters
                                )) {
                                tb.ReconstructFormattingElements();
                                tb.InsertEmpty(startTag);
                                tb.FramesetOk(false);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartPClosers
                                    )) {
                                    if (tb.InButtonScope("p")) {
                                        tb.ProcessEndTag("p");
                                    }
                                    tb.Insert(startTag);
                                }
                                else {
                                    if (name.Equals("span")) {
                                        // same as final else, but short circuits lots of checks
                                        tb.ReconstructFormattingElements();
                                        tb.Insert(startTag);
                                    }
                                    else {
                                        if (name.Equals("li")) {
                                            tb.FramesetOk(false);
                                            List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                                            for (int i = stack.Count - 1; i > 0; i--) {
                                                iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[i];
                                                if (el.NodeName().Equals("li")) {
                                                    tb.ProcessEndTag("li");
                                                    break;
                                                }
                                                if (tb.IsSpecial(el) && !iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(el.NodeName(), HtmlTreeBuilderState.Constants
                                                    .InBodyStartLiBreakers)) {
                                                    break;
                                                }
                                            }
                                            if (tb.InButtonScope("p")) {
                                                tb.ProcessEndTag("p");
                                            }
                                            tb.Insert(startTag);
                                        }
                                        else {
                                            if (name.Equals("html")) {
                                                tb.Error(this);
                                                // merge attributes onto real html
                                                iText.StyledXmlParser.Jsoup.Nodes.Element html = tb.GetStack()[0];
                                                foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in startTag.GetAttributes()) {
                                                    if (!html.HasAttr(attribute.Key)) {
                                                        html.Attributes().Put(attribute);
                                                    }
                                                }
                                            }
                                            else {
                                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartToHead
                                                    )) {
                                                    return tb.Process(t, HtmlTreeBuilderState.InHead);
                                                }
                                                else {
                                                    if (name.Equals("body")) {
                                                        tb.Error(this);
                                                        List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                                                        if (stack.Count == 1 || (stack.Count > 2 && !stack[1].NodeName().Equals("body"))) {
                                                            // only in fragment case
                                                            return false;
                                                        }
                                                        else {
                                                            // ignore
                                                            tb.FramesetOk(false);
                                                            iText.StyledXmlParser.Jsoup.Nodes.Element body = stack[1];
                                                            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in startTag.GetAttributes()) {
                                                                if (!body.HasAttr(attribute.Key)) {
                                                                    body.Attributes().Put(attribute);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        if (name.Equals("frameset")) {
                                                            tb.Error(this);
                                                            List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                                                            if (stack.Count == 1 || (stack.Count > 2 && !stack[1].NodeName().Equals("body"))) {
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
                                                                    tb.Transition(HtmlTreeBuilderState.InFrameset);
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.Headings)) {
                                                                if (tb.InButtonScope("p")) {
                                                                    tb.ProcessEndTag("p");
                                                                }
                                                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(tb.CurrentElement().NodeName(), HtmlTreeBuilderState.Constants
                                                                    .Headings)) {
                                                                    tb.Error(this);
                                                                    tb.Pop();
                                                                }
                                                                tb.Insert(startTag);
                                                            }
                                                            else {
                                                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartPreListing
                                                                    )) {
                                                                    if (tb.InButtonScope("p")) {
                                                                        tb.ProcessEndTag("p");
                                                                    }
                                                                    tb.Insert(startTag);
                                                                    // todo: ignore LF if next token
                                                                    tb.FramesetOk(false);
                                                                }
                                                                else {
                                                                    if (name.Equals("form")) {
                                                                        if (tb.GetFormElement() != null) {
                                                                            tb.Error(this);
                                                                            return false;
                                                                        }
                                                                        if (tb.InButtonScope("p")) {
                                                                            tb.ProcessEndTag("p");
                                                                        }
                                                                        tb.InsertForm(startTag, true);
                                                                    }
                                                                    else {
                                                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.DdDt)) {
                                                                            tb.FramesetOk(false);
                                                                            List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                                                                            for (int i = stack.Count - 1; i > 0; i--) {
                                                                                iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[i];
                                                                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(el.NodeName(), HtmlTreeBuilderState.Constants.DdDt
                                                                                    )) {
                                                                                    tb.ProcessEndTag(el.NodeName());
                                                                                    break;
                                                                                }
                                                                                if (tb.IsSpecial(el) && !iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(el.NodeName(), HtmlTreeBuilderState.Constants
                                                                                    .InBodyStartLiBreakers)) {
                                                                                    break;
                                                                                }
                                                                            }
                                                                            if (tb.InButtonScope("p")) {
                                                                                tb.ProcessEndTag("p");
                                                                            }
                                                                            tb.Insert(startTag);
                                                                        }
                                                                        else {
                                                                            if (name.Equals("plaintext")) {
                                                                                if (tb.InButtonScope("p")) {
                                                                                    tb.ProcessEndTag("p");
                                                                                }
                                                                                tb.Insert(startTag);
                                                                                tb.tokeniser.Transition(TokeniserState.PLAINTEXT);
                                                                            }
                                                                            else {
                                                                                // once in, never gets out
                                                                                if (name.Equals("button")) {
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
                                                                                }
                                                                                else {
                                                                                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.Formatters
                                                                                        )) {
                                                                                        tb.ReconstructFormattingElements();
                                                                                        iText.StyledXmlParser.Jsoup.Nodes.Element el = tb.Insert(startTag);
                                                                                        tb.PushActiveFormattingElements(el);
                                                                                    }
                                                                                    else {
                                                                                        if (name.Equals("nobr")) {
                                                                                            tb.ReconstructFormattingElements();
                                                                                            if (tb.InScope("nobr")) {
                                                                                                tb.Error(this);
                                                                                                tb.ProcessEndTag("nobr");
                                                                                                tb.ReconstructFormattingElements();
                                                                                            }
                                                                                            iText.StyledXmlParser.Jsoup.Nodes.Element el = tb.Insert(startTag);
                                                                                            tb.PushActiveFormattingElements(el);
                                                                                        }
                                                                                        else {
                                                                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartApplets
                                                                                                )) {
                                                                                                tb.ReconstructFormattingElements();
                                                                                                tb.Insert(startTag);
                                                                                                tb.InsertMarkerToFormattingElements();
                                                                                                tb.FramesetOk(false);
                                                                                            }
                                                                                            else {
                                                                                                if (name.Equals("table")) {
                                                                                                    if (tb.GetDocument().QuirksMode() != QuirksMode.quirks && tb.InButtonScope("p")) {
                                                                                                        tb.ProcessEndTag("p");
                                                                                                    }
                                                                                                    tb.Insert(startTag);
                                                                                                    tb.FramesetOk(false);
                                                                                                    tb.Transition(HtmlTreeBuilderState.InTable);
                                                                                                }
                                                                                                else {
                                                                                                    if (name.Equals("input")) {
                                                                                                        tb.ReconstructFormattingElements();
                                                                                                        iText.StyledXmlParser.Jsoup.Nodes.Element el = tb.InsertEmpty(startTag);
                                                                                                        if (!el.Attr("type").EqualsIgnoreCase("hidden")) {
                                                                                                            tb.FramesetOk(false);
                                                                                                        }
                                                                                                    }
                                                                                                    else {
                                                                                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartMedia
                                                                                                            )) {
                                                                                                            tb.InsertEmpty(startTag);
                                                                                                        }
                                                                                                        else {
                                                                                                            if (name.Equals("hr")) {
                                                                                                                if (tb.InButtonScope("p")) {
                                                                                                                    tb.ProcessEndTag("p");
                                                                                                                }
                                                                                                                tb.InsertEmpty(startTag);
                                                                                                                tb.FramesetOk(false);
                                                                                                            }
                                                                                                            else {
                                                                                                                if (name.Equals("image")) {
                                                                                                                    if (tb.GetFromStack("svg") == null) {
                                                                                                                        return tb.Process(startTag.Name("img"));
                                                                                                                    }
                                                                                                                    else {
                                                                                                                        // change <image> to <img>, unless in svg
                                                                                                                        tb.Insert(startTag);
                                                                                                                    }
                                                                                                                }
                                                                                                                else {
                                                                                                                    if (name.Equals("isindex")) {
                                                                                                                        // how much do we care about the early 90s?
                                                                                                                        tb.Error(this);
                                                                                                                        if (tb.GetFormElement() != null) {
                                                                                                                            return false;
                                                                                                                        }
                                                                                                                        tb.tokeniser.AcknowledgeSelfClosingFlag();
                                                                                                                        tb.ProcessStartTag("form");
                                                                                                                        if (startTag.attributes.HasKey("action")) {
                                                                                                                            iText.StyledXmlParser.Jsoup.Nodes.Element form = tb.GetFormElement();
                                                                                                                            form.Attr("action", startTag.attributes.Get("action"));
                                                                                                                        }
                                                                                                                        tb.ProcessStartTag("hr");
                                                                                                                        tb.ProcessStartTag("label");
                                                                                                                        // hope you like english.
                                                                                                                        String prompt = startTag.attributes.HasKey("prompt") ? startTag.attributes.Get("prompt") : "This is a searchable index. Enter search keywords: ";
                                                                                                                        tb.Process(new Token.Character().Data(prompt));
                                                                                                                        // input
                                                                                                                        Attributes inputAttribs = new Attributes();
                                                                                                                        foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attr in startTag.attributes) {
                                                                                                                            if (!iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(attr.Key, HtmlTreeBuilderState.Constants.InBodyStartInputAttribs
                                                                                                                                )) {
                                                                                                                                inputAttribs.Put(attr);
                                                                                                                            }
                                                                                                                        }
                                                                                                                        inputAttribs.Put("name", "isindex");
                                                                                                                        tb.ProcessStartTag("input", inputAttribs);
                                                                                                                        tb.ProcessEndTag("label");
                                                                                                                        tb.ProcessStartTag("hr");
                                                                                                                        tb.ProcessEndTag("form");
                                                                                                                    }
                                                                                                                    else {
                                                                                                                        if (name.Equals("textarea")) {
                                                                                                                            tb.Insert(startTag);
                                                                                                                            // todo: If the next token is a U+000A LINE FEED (LF) character token, then ignore that token and move on to the next one. (Newlines at the start of textarea elements are ignored as an authoring convenience.)
                                                                                                                            tb.tokeniser.Transition(TokeniserState.Rcdata);
                                                                                                                            tb.MarkInsertionMode();
                                                                                                                            tb.FramesetOk(false);
                                                                                                                            tb.Transition(HtmlTreeBuilderState.Text);
                                                                                                                        }
                                                                                                                        else {
                                                                                                                            if (name.Equals("xmp")) {
                                                                                                                                if (tb.InButtonScope("p")) {
                                                                                                                                    tb.ProcessEndTag("p");
                                                                                                                                }
                                                                                                                                tb.ReconstructFormattingElements();
                                                                                                                                tb.FramesetOk(false);
                                                                                                                                HtmlTreeBuilderState.HandleRawtext(startTag, tb);
                                                                                                                            }
                                                                                                                            else {
                                                                                                                                if (name.Equals("iframe")) {
                                                                                                                                    tb.FramesetOk(false);
                                                                                                                                    HtmlTreeBuilderState.HandleRawtext(startTag, tb);
                                                                                                                                }
                                                                                                                                else {
                                                                                                                                    if (name.Equals("noembed")) {
                                                                                                                                        // also handle noscript if script enabled
                                                                                                                                        HtmlTreeBuilderState.HandleRawtext(startTag, tb);
                                                                                                                                    }
                                                                                                                                    else {
                                                                                                                                        if (name.Equals("select")) {
                                                                                                                                            tb.ReconstructFormattingElements();
                                                                                                                                            tb.Insert(startTag);
                                                                                                                                            tb.FramesetOk(false);
                                                                                                                                            HtmlTreeBuilderState state = tb.State();
                                                                                                                                            if (state.Equals(HtmlTreeBuilderState.InTable) || state.Equals(HtmlTreeBuilderState.InCaption) || state.Equals
                                                                                                                                                (HtmlTreeBuilderState.InTableBody) || state.Equals(HtmlTreeBuilderState.InRow) || state.Equals(HtmlTreeBuilderState
                                                                                                                                                .InCell)) {
                                                                                                                                                tb.Transition(HtmlTreeBuilderState.InSelectInTable);
                                                                                                                                            }
                                                                                                                                            else {
                                                                                                                                                tb.Transition(HtmlTreeBuilderState.InSelect);
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else {
                                                                                                                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartOptions
                                                                                                                                                )) {
                                                                                                                                                if (tb.CurrentElement().NodeName().Equals("option")) {
                                                                                                                                                    tb.ProcessEndTag("option");
                                                                                                                                                }
                                                                                                                                                tb.ReconstructFormattingElements();
                                                                                                                                                tb.Insert(startTag);
                                                                                                                                            }
                                                                                                                                            else {
                                                                                                                                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartRuby
                                                                                                                                                    )) {
                                                                                                                                                    if (tb.InScope("ruby")) {
                                                                                                                                                        tb.GenerateImpliedEndTags();
                                                                                                                                                        if (!tb.CurrentElement().NodeName().Equals("ruby")) {
                                                                                                                                                            tb.Error(this);
                                                                                                                                                            tb.PopStackToBefore("ruby");
                                                                                                                                                        }
                                                                                                                                                        // i.e. close up to but not include name
                                                                                                                                                        tb.Insert(startTag);
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else {
                                                                                                                                                    if (name.Equals("math")) {
                                                                                                                                                        tb.ReconstructFormattingElements();
                                                                                                                                                        // todo: handle A start tag whose tag name is "math" (i.e. foreign, mathml)
                                                                                                                                                        tb.Insert(startTag);
                                                                                                                                                        tb.tokeniser.AcknowledgeSelfClosingFlag();
                                                                                                                                                    }
                                                                                                                                                    else {
                                                                                                                                                        if (name.Equals("svg")) {
                                                                                                                                                            tb.ReconstructFormattingElements();
                                                                                                                                                            // todo: handle A start tag whose tag name is "svg" (xlink, svg)
                                                                                                                                                            tb.Insert(startTag);
                                                                                                                                                            tb.tokeniser.AcknowledgeSelfClosingFlag();
                                                                                                                                                        }
                                                                                                                                                        else {
                                                                                                                                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartDrop
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
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
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
                        Token.EndTag endTag = t.AsEndTag();
                        name = endTag.Name();
                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyEndAdoptionFormatters
                            )) {
                            // Adoption Agency Algorithm.
                            for (int i = 0; i < 8; i++) {
                                iText.StyledXmlParser.Jsoup.Nodes.Element formatEl = tb.GetActiveFormattingElement(name);
                                if (formatEl == null) {
                                    return this.AnyOtherEndTag(t, tb);
                                }
                                else {
                                    if (!tb.OnStack(formatEl)) {
                                        tb.Error(this);
                                        tb.RemoveFromActiveFormattingElements(formatEl);
                                        return true;
                                    }
                                    else {
                                        if (!tb.InScope(formatEl.NodeName())) {
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
                                List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                                // the spec doesn't limit to < 64, but in degenerate cases (9000+ stack depth) this prevents
                                // run-aways
                                int stackSize = stack.Count;
                                for (int si = 0; si < stackSize && si < 64; si++) {
                                    iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[si];
                                    if (el == formatEl) {
                                        commonAncestor = stack[si - 1];
                                        seenFormattingElement = true;
                                    }
                                    else {
                                        if (seenFormattingElement && tb.IsSpecial(el)) {
                                            furthestBlock = el;
                                            break;
                                        }
                                    }
                                }
                                if (furthestBlock == null) {
                                    tb.PopStackToClose(formatEl.NodeName());
                                    tb.RemoveFromActiveFormattingElements(formatEl);
                                    return true;
                                }
                                // todo: Let a bookmark note the position of the formatting element in the list of active formatting elements relative to the elements on either side of it in the list.
                                // does that mean: int pos of format el in list?
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
                                        .ValueOf(node.NodeName()), tb.GetBaseUri());
                                    tb.ReplaceActiveFormattingElement(node, replacement);
                                    tb.ReplaceOnStack(node, replacement);
                                    node = replacement;
                                    if (lastNode == furthestBlock) {
                                    }
                                    // todo: move the aforementioned bookmark to be immediately after the new node in the list of active formatting elements.
                                    // not getting how this bookmark both straddles the element above, but is inbetween here...
                                    if (lastNode.Parent() != null) {
                                        lastNode.Remove();
                                    }
                                    node.AppendChild(lastNode);
                                    lastNode = node;
                                }
                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(commonAncestor.NodeName(), HtmlTreeBuilderState.Constants
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
                                iText.StyledXmlParser.Jsoup.Nodes.Element adopter = new iText.StyledXmlParser.Jsoup.Nodes.Element(formatEl
                                    .Tag(), tb.GetBaseUri());
                                adopter.Attributes().AddAll(formatEl.Attributes());
                                iText.StyledXmlParser.Jsoup.Nodes.Node[] childNodes = furthestBlock.ChildNodes().ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node
                                    [furthestBlock.ChildNodeSize()]);
                                foreach (iText.StyledXmlParser.Jsoup.Nodes.Node childNode in childNodes) {
                                    adopter.AppendChild(childNode);
                                }
                                // append will reparent. thus the clone to avoid concurrent mod.
                                furthestBlock.AppendChild(adopter);
                                tb.RemoveFromActiveFormattingElements(formatEl);
                                // todo: insert the new element into the list of active formatting elements at the position of the aforementioned bookmark.
                                tb.RemoveFromStack(formatEl);
                                tb.InsertOnStackAfter(furthestBlock, adopter);
                            }
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyEndClosers
                                )) {
                                if (!tb.InScope(name)) {
                                    // nothing to close
                                    tb.Error(this);
                                    return false;
                                }
                                else {
                                    tb.GenerateImpliedEndTags();
                                    if (!tb.CurrentElement().NodeName().Equals(name)) {
                                        tb.Error(this);
                                    }
                                    tb.PopStackToClose(name);
                                }
                            }
                            else {
                                if (name.Equals("span")) {
                                    // same as final fall through, but saves short circuit
                                    return this.AnyOtherEndTag(t, tb);
                                }
                                else {
                                    if (name.Equals("li")) {
                                        if (!tb.InListItemScope(name)) {
                                            tb.Error(this);
                                            return false;
                                        }
                                        else {
                                            tb.GenerateImpliedEndTags(name);
                                            if (!tb.CurrentElement().NodeName().Equals(name)) {
                                                tb.Error(this);
                                            }
                                            tb.PopStackToClose(name);
                                        }
                                    }
                                    else {
                                        if (name.Equals("body")) {
                                            if (!tb.InScope("body")) {
                                                tb.Error(this);
                                                return false;
                                            }
                                            else {
                                                // todo: error if stack contains something not dd, dt, li, optgroup, option, p, rp, rt, tbody, td, tfoot, th, thead, tr, body, html
                                                tb.Transition(HtmlTreeBuilderState.AfterBody);
                                            }
                                        }
                                        else {
                                            if (name.Equals("html")) {
                                                bool notIgnored = tb.ProcessEndTag("body");
                                                if (notIgnored) {
                                                    return tb.Process(endTag);
                                                }
                                            }
                                            else {
                                                if (name.Equals("form")) {
                                                    iText.StyledXmlParser.Jsoup.Nodes.Element currentForm = tb.GetFormElement();
                                                    tb.SetFormElement(null);
                                                    if (currentForm == null || !tb.InScope(name)) {
                                                        tb.Error(this);
                                                        return false;
                                                    }
                                                    else {
                                                        tb.GenerateImpliedEndTags();
                                                        if (!tb.CurrentElement().NodeName().Equals(name)) {
                                                            tb.Error(this);
                                                        }
                                                        // remove currentForm from stack. will shift anything under up.
                                                        tb.RemoveFromStack(currentForm);
                                                    }
                                                }
                                                else {
                                                    if (name.Equals("p")) {
                                                        if (!tb.InButtonScope(name)) {
                                                            tb.Error(this);
                                                            tb.ProcessStartTag(name);
                                                            // if no p to close, creates an empty <p></p>
                                                            return tb.Process(endTag);
                                                        }
                                                        else {
                                                            tb.GenerateImpliedEndTags(name);
                                                            if (!tb.CurrentElement().NodeName().Equals(name)) {
                                                                tb.Error(this);
                                                            }
                                                            tb.PopStackToClose(name);
                                                        }
                                                    }
                                                    else {
                                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.DdDt)) {
                                                            if (!tb.InScope(name)) {
                                                                tb.Error(this);
                                                                return false;
                                                            }
                                                            else {
                                                                tb.GenerateImpliedEndTags(name);
                                                                if (!tb.CurrentElement().NodeName().Equals(name)) {
                                                                    tb.Error(this);
                                                                }
                                                                tb.PopStackToClose(name);
                                                            }
                                                        }
                                                        else {
                                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.Headings)) {
                                                                if (!tb.InScope(HtmlTreeBuilderState.Constants.Headings)) {
                                                                    tb.Error(this);
                                                                    return false;
                                                                }
                                                                else {
                                                                    tb.GenerateImpliedEndTags(name);
                                                                    if (!tb.CurrentElement().NodeName().Equals(name)) {
                                                                        tb.Error(this);
                                                                    }
                                                                    tb.PopStackToClose(HtmlTreeBuilderState.Constants.Headings);
                                                                }
                                                            }
                                                            else {
                                                                if (name.Equals("sarcasm")) {
                                                                    // *sigh*
                                                                    return this.AnyOtherEndTag(t, tb);
                                                                }
                                                                else {
                                                                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.InSorted(name, HtmlTreeBuilderState.Constants.InBodyStartApplets
                                                                        )) {
                                                                        if (!tb.InScope("name")) {
                                                                            if (!tb.InScope(name)) {
                                                                                tb.Error(this);
                                                                                return false;
                                                                            }
                                                                            tb.GenerateImpliedEndTags();
                                                                            if (!tb.CurrentElement().NodeName().Equals(name)) {
                                                                                tb.Error(this);
                                                                            }
                                                                            tb.PopStackToClose(name);
                                                                            tb.ClearFormattingElementsToLastMarker();
                                                                        }
                                                                    }
                                                                    else {
                                                                        if (name.Equals("br")) {
                                                                            tb.Error(this);
                                                                            tb.ProcessStartTag("br");
                                                                            return false;
                                                                        }
                                                                        else {
                                                                            return this.AnyOtherEndTag(t, tb);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
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

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                        // todo: error if stack contains something not dd, dt, li, p, tbody, td, tfoot, th, thead, tr, body, html
                        // stop parsing
                        break;
                    }
                }
                return true;
            }

            internal bool AnyOtherEndTag(Token t, HtmlTreeBuilder tb) {
                String name = t.AsEndTag().Name();
                List<iText.StyledXmlParser.Jsoup.Nodes.Element> stack = tb.GetStack();
                for (int pos = stack.Count - 1; pos >= 0; pos--) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element node = stack[pos];
                    if (node.NodeName().Equals(name)) {
                        tb.GenerateImpliedEndTags(name);
                        if (!name.Equals(tb.CurrentElement().NodeName())) {
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
        }

        internal static HtmlTreeBuilderState InBody = new _HtmlTreeBuilderState_335();

        private sealed class _HtmlTreeBuilderState_881 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_881() {
            }

            internal override String GetName() {
                return "Text";
            }

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
        }

        internal static HtmlTreeBuilderState Text = new _HtmlTreeBuilderState_881();

        private sealed class _HtmlTreeBuilderState_907 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_907() {
            }

            internal override String GetName() {
                return "InTable";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                if (t.IsCharacter()) {
                    tb.NewPendingTableCharacters();
                    tb.MarkInsertionMode();
                    tb.Transition(HtmlTreeBuilderState.InTableText);
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
                                name = startTag.Name();
                                if (name.Equals("caption")) {
                                    tb.ClearStackToTableContext();
                                    tb.InsertMarkerToFormattingElements();
                                    tb.Insert(startTag);
                                    tb.Transition(HtmlTreeBuilderState.InCaption);
                                }
                                else {
                                    if (name.Equals("colgroup")) {
                                        tb.ClearStackToTableContext();
                                        tb.Insert(startTag);
                                        tb.Transition(HtmlTreeBuilderState.InColumnGroup);
                                    }
                                    else {
                                        if (name.Equals("col")) {
                                            tb.ProcessStartTag("colgroup");
                                            return tb.Process(t);
                                        }
                                        else {
                                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "tbody", "tfoot", "thead")) {
                                                tb.ClearStackToTableContext();
                                                tb.Insert(startTag);
                                                tb.Transition(HtmlTreeBuilderState.InTableBody);
                                            }
                                            else {
                                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "td", "th", "tr")) {
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
                                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "style", "script")) {
                                                            return tb.Process(t, HtmlTreeBuilderState.InHead);
                                                        }
                                                        else {
                                                            if (name.Equals("input")) {
                                                                if (!startTag.attributes.Get("type").EqualsIgnoreCase("hidden")) {
                                                                    return this.AnythingElse(t, tb);
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
                                                                    return this.AnythingElse(t, tb);
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
                                // todo: check if should return processed http://www.whatwg.org/specs/web-apps/current-work/multipage/tree-construction.html#parsing-main-intable
                                if (t.IsEndTag()) {
                                    Token.EndTag endTag = t.AsEndTag();
                                    name = endTag.Name();
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
                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "body", "caption", "col", "colgroup", "html", "tbody"
                                            , "td", "tfoot", "th", "thead", "tr")) {
                                            tb.Error(this);
                                            return false;
                                        }
                                        else {
                                            return this.AnythingElse(t, tb);
                                        }
                                    }
                                    return true;
                                }
                                else {
                                    // todo: as above todo
                                    if (t.IsEOF()) {
                                        if (tb.CurrentElement().NodeName().Equals("html")) {
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
                return this.AnythingElse(t, tb);
            }

            internal bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Error(this);
                bool processed;
                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(tb.CurrentElement().NodeName(), "table", "tbody", "tfoot"
                    , "thead", "tr")) {
                    tb.SetFosterInserts(true);
                    processed = tb.Process(t, HtmlTreeBuilderState.InBody);
                    tb.SetFosterInserts(false);
                }
                else {
                    processed = tb.Process(t, HtmlTreeBuilderState.InBody);
                }
                return processed;
            }
        }

        internal static HtmlTreeBuilderState InTable = new _HtmlTreeBuilderState_907();

        private sealed class _HtmlTreeBuilderState_1015 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1015() {
            }

            internal override String GetName() {
                return "InTableText";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                        Token.Character c = t.AsCharacter();
                        if (c.GetData().Equals(HtmlTreeBuilderState.nullString)) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            tb.GetPendingTableCharacters().Add(c.GetData());
                        }
                        break;
                    }

                    default: {
                        // todo - don't really like the way these table character data lists are built
                        if (tb.GetPendingTableCharacters().Count > 0) {
                            foreach (String character in tb.GetPendingTableCharacters()) {
                                if (!HtmlTreeBuilderState.IsWhitespace(character)) {
                                    // InTable anything else section:
                                    tb.Error(this);
                                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(tb.CurrentElement().NodeName(), "table", "tbody", "tfoot"
                                        , "thead", "tr")) {
                                        tb.SetFosterInserts(true);
                                        tb.Process(new Token.Character().Data(character), HtmlTreeBuilderState.InBody);
                                        tb.SetFosterInserts(false);
                                    }
                                    else {
                                        tb.Process(new Token.Character().Data(character), HtmlTreeBuilderState.InBody);
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
                }
                return true;
            }
        }

        internal static HtmlTreeBuilderState InTableText = new _HtmlTreeBuilderState_1015();

        private sealed class _HtmlTreeBuilderState_1059 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1059() {
            }

            internal override String GetName() {
                return "InCaption";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsEndTag() && t.AsEndTag().Name().Equals("caption")) {
                    Token.EndTag endTag = t.AsEndTag();
                    String name = endTag.Name();
                    if (!tb.InTableScope(name)) {
                        tb.Error(this);
                        return false;
                    }
                    else {
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement().NodeName().Equals("caption")) {
                            tb.Error(this);
                        }
                        tb.PopStackToClose("caption");
                        tb.ClearFormattingElementsToLastMarker();
                        tb.Transition(HtmlTreeBuilderState.InTable);
                    }
                }
                else {
                    if ((t.IsStartTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsStartTag().Name(), "caption", 
                        "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr") || t.IsEndTag() && t.AsEndTag().Name()
                        .Equals("table"))) {
                        tb.Error(this);
                        bool processed = tb.ProcessEndTag("caption");
                        if (processed) {
                            return tb.Process(t);
                        }
                    }
                    else {
                        if (t.IsEndTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsEndTag().Name(), "body", "col", "colgroup"
                            , "html", "tbody", "td", "tfoot", "th", "thead", "tr")) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            return tb.Process(t, HtmlTreeBuilderState.InBody);
                        }
                    }
                }
                return true;
            }
        }

        internal static HtmlTreeBuilderState InCaption = new _HtmlTreeBuilderState_1059();

        private sealed class _HtmlTreeBuilderState_1101 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1101() {
            }

            internal override String GetName() {
                return "InColumnGroup";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
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
                        name = startTag.Name();
                        if (name.Equals("html")) {
                            return tb.Process(t, HtmlTreeBuilderState.InBody);
                        }
                        else {
                            if (name.Equals("col")) {
                                tb.InsertEmpty(startTag);
                            }
                            else {
                                return this.AnythingElse(t, tb);
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        Token.EndTag endTag = t.AsEndTag();
                        name = endTag.Name();
                        if (name.Equals("colgroup")) {
                            if (tb.CurrentElement().NodeName().Equals("html")) {
                                // frag case
                                tb.Error(this);
                                return false;
                            }
                            else {
                                tb.Pop();
                                tb.Transition(HtmlTreeBuilderState.InTable);
                            }
                        }
                        else {
                            return this.AnythingElse(t, tb);
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                        if (tb.CurrentElement().NodeName().Equals("html")) {
                            return true;
                        }
                        else {
                            // stop parsing; frag case
                            return this.AnythingElse(t, tb);
                        }
                        goto default;
                    }

                    default: {
                        return this.AnythingElse(t, tb);
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, TreeBuilder tb) {
                bool processed = tb.ProcessEndTag("colgroup");
                if (processed) {
                    // only ignored in frag case
                    return tb.Process(t);
                }
                return true;
            }
        }

        internal static HtmlTreeBuilderState InColumnGroup = new _HtmlTreeBuilderState_1101();

        private sealed class _HtmlTreeBuilderState_1164 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1164() {
            }

            internal override String GetName() {
                return "InTableBody";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.StartTag: {
                        Token.StartTag startTag = t.AsStartTag();
                        name = startTag.Name();
                        if (name.Equals("tr")) {
                            tb.ClearStackToTableBodyContext();
                            tb.Insert(startTag);
                            tb.Transition(HtmlTreeBuilderState.InRow);
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "th", "td")) {
                                tb.Error(this);
                                tb.ProcessStartTag("tr");
                                return tb.Process(startTag);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "caption", "col", "colgroup", "tbody", "tfoot", 
                                    "thead")) {
                                    return this.ExitTableBody(t, tb);
                                }
                                else {
                                    return this.AnythingElse(t, tb);
                                }
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EndTag: {
                        Token.EndTag endTag = t.AsEndTag();
                        name = endTag.Name();
                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "tbody", "tfoot", "thead")) {
                            if (!tb.InTableScope(name)) {
                                tb.Error(this);
                                return false;
                            }
                            else {
                                tb.ClearStackToTableBodyContext();
                                tb.Pop();
                                tb.Transition(HtmlTreeBuilderState.InTable);
                            }
                        }
                        else {
                            if (name.Equals("table")) {
                                return this.ExitTableBody(t, tb);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "body", "caption", "col", "colgroup", "html", "td"
                                    , "th", "tr")) {
                                    tb.Error(this);
                                    return false;
                                }
                                else {
                                    return this.AnythingElse(t, tb);
                                }
                            }
                        }
                        break;
                    }

                    default: {
                        return this.AnythingElse(t, tb);
                    }
                }
                return true;
            }

            private bool ExitTableBody(Token t, HtmlTreeBuilder tb) {
                if (!(tb.InTableScope("tbody") || tb.InTableScope("thead") || tb.InScope("tfoot"))) {
                    // frag case
                    tb.Error(this);
                    return false;
                }
                tb.ClearStackToTableBodyContext();
                tb.ProcessEndTag(tb.CurrentElement().NodeName());
                // tbody, tfoot, thead
                return tb.Process(t);
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, HtmlTreeBuilderState.InTable);
            }
        }

        internal static HtmlTreeBuilderState InTableBody = new _HtmlTreeBuilderState_1164();

        private sealed class _HtmlTreeBuilderState_1232 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1232() {
            }

            internal override String GetName() {
                return "InRow";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsStartTag()) {
                    Token.StartTag startTag = t.AsStartTag();
                    String name = startTag.Name();
                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "th", "td")) {
                        tb.ClearStackToTableRowContext();
                        tb.Insert(startTag);
                        tb.Transition(HtmlTreeBuilderState.InCell);
                        tb.InsertMarkerToFormattingElements();
                    }
                    else {
                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "caption", "col", "colgroup", "tbody", "tfoot", 
                            "thead", "tr")) {
                            return this.HandleMissingTr(t, tb);
                        }
                        else {
                            return this.AnythingElse(t, tb);
                        }
                    }
                }
                else {
                    if (t.IsEndTag()) {
                        Token.EndTag endTag = t.AsEndTag();
                        String name = endTag.Name();
                        if (name.Equals("tr")) {
                            if (!tb.InTableScope(name)) {
                                tb.Error(this);
                                // frag
                                return false;
                            }
                            tb.ClearStackToTableRowContext();
                            tb.Pop();
                            // tr
                            tb.Transition(HtmlTreeBuilderState.InTableBody);
                        }
                        else {
                            if (name.Equals("table")) {
                                return this.HandleMissingTr(t, tb);
                            }
                            else {
                                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "tbody", "tfoot", "thead")) {
                                    if (!tb.InTableScope(name)) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    tb.ProcessEndTag("tr");
                                    return tb.Process(t);
                                }
                                else {
                                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "body", "caption", "col", "colgroup", "html", "td"
                                        , "th")) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        return this.AnythingElse(t, tb);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        return this.AnythingElse(t, tb);
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, HtmlTreeBuilderState.InTable);
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

        internal static HtmlTreeBuilderState InRow = new _HtmlTreeBuilderState_1232();

        private sealed class _HtmlTreeBuilderState_1300 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1300() {
            }

            internal override String GetName() {
                return "InCell";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsEndTag()) {
                    Token.EndTag endTag = t.AsEndTag();
                    String name = endTag.Name();
                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "td", "th")) {
                        if (!tb.InTableScope(name)) {
                            tb.Error(this);
                            tb.Transition(HtmlTreeBuilderState.InRow);
                            // might not be in scope if empty: <td /> and processing fake end tag
                            return false;
                        }
                        tb.GenerateImpliedEndTags();
                        if (!tb.CurrentElement().NodeName().Equals(name)) {
                            tb.Error(this);
                        }
                        tb.PopStackToClose(name);
                        tb.ClearFormattingElementsToLastMarker();
                        tb.Transition(HtmlTreeBuilderState.InRow);
                    }
                    else {
                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "body", "caption", "col", "colgroup", "html")) {
                            tb.Error(this);
                            return false;
                        }
                        else {
                            if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "table", "tbody", "tfoot", "thead", "tr")) {
                                if (!tb.InTableScope(name)) {
                                    tb.Error(this);
                                    return false;
                                }
                                this.CloseCell(tb);
                                return tb.Process(t);
                            }
                            else {
                                return this.AnythingElse(t, tb);
                            }
                        }
                    }
                }
                else {
                    if (t.IsStartTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsStartTag().Name(), "caption", "col"
                        , "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr")) {
                        if (!(tb.InTableScope("td") || tb.InTableScope("th"))) {
                            tb.Error(this);
                            return false;
                        }
                        this.CloseCell(tb);
                        return tb.Process(t);
                    }
                    else {
                        return this.AnythingElse(t, tb);
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                return tb.Process(t, HtmlTreeBuilderState.InBody);
            }

            private void CloseCell(HtmlTreeBuilder tb) {
                if (tb.InTableScope("td")) {
                    tb.ProcessEndTag("td");
                }
                else {
                    tb.ProcessEndTag("th");
                }
            }
        }

        internal static HtmlTreeBuilderState InCell = new _HtmlTreeBuilderState_1300();

        private sealed class _HtmlTreeBuilderState_1364 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1364() {
            }

            // only here if th or td in scope
            internal override String GetName() {
                return "InSelect";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                switch (t.type) {
                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.Character: {
                        Token.Character c = t.AsCharacter();
                        if (c.GetData().Equals(HtmlTreeBuilderState.nullString)) {
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
                        name = start.Name();
                        if (name.Equals("html")) {
                            return tb.Process(start, HtmlTreeBuilderState.InBody);
                        }
                        else {
                            if (name.Equals("option")) {
                                tb.ProcessEndTag("option");
                                tb.Insert(start);
                            }
                            else {
                                if (name.Equals("optgroup")) {
                                    if (tb.CurrentElement().NodeName().Equals("option")) {
                                        tb.ProcessEndTag("option");
                                    }
                                    else {
                                        if (tb.CurrentElement().NodeName().Equals("optgroup")) {
                                            tb.ProcessEndTag("optgroup");
                                        }
                                    }
                                    tb.Insert(start);
                                }
                                else {
                                    if (name.Equals("select")) {
                                        tb.Error(this);
                                        return tb.ProcessEndTag("select");
                                    }
                                    else {
                                        if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, "input", "keygen", "textarea")) {
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
                                                return tb.Process(t, HtmlTreeBuilderState.InHead);
                                            }
                                            else {
                                                return this.AnythingElse(t, tb);
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
                        name = end.Name();
                        if (name.Equals("optgroup")) {
                            if (tb.CurrentElement().NodeName().Equals("option") && tb.AboveOnStack(tb.CurrentElement()) != null && tb.
                                AboveOnStack(tb.CurrentElement()).NodeName().Equals("optgroup")) {
                                tb.ProcessEndTag("option");
                            }
                            if (tb.CurrentElement().NodeName().Equals("optgroup")) {
                                tb.Pop();
                            }
                            else {
                                tb.Error(this);
                            }
                        }
                        else {
                            if (name.Equals("option")) {
                                if (tb.CurrentElement().NodeName().Equals("option")) {
                                    tb.Pop();
                                }
                                else {
                                    tb.Error(this);
                                }
                            }
                            else {
                                if (name.Equals("select")) {
                                    if (!tb.InSelectScope(name)) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        tb.PopStackToClose(name);
                                        tb.ResetInsertionMode();
                                    }
                                }
                                else {
                                    return this.AnythingElse(t, tb);
                                }
                            }
                        }
                        break;
                    }

                    case iText.StyledXmlParser.Jsoup.Parser.TokenType.EOF: {
                        if (!tb.CurrentElement().NodeName().Equals("html")) {
                            tb.Error(this);
                        }
                        break;
                    }

                    default: {
                        return this.AnythingElse(t, tb);
                    }
                }
                return true;
            }

            private bool AnythingElse(Token t, HtmlTreeBuilder tb) {
                tb.Error(this);
                return false;
            }
        }

        internal static HtmlTreeBuilderState InSelect = new _HtmlTreeBuilderState_1364();

        private sealed class _HtmlTreeBuilderState_1460 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1460() {
            }

            internal override String GetName() {
                return "InSelectInTable";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsStartTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsStartTag().Name(), "caption", "table"
                    , "tbody", "tfoot", "thead", "tr", "td", "th")) {
                    tb.Error(this);
                    tb.ProcessEndTag("select");
                    return tb.Process(t);
                }
                else {
                    if (t.IsEndTag() && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(t.AsEndTag().Name(), "caption", "table"
                        , "tbody", "tfoot", "thead", "tr", "td", "th")) {
                        tb.Error(this);
                        if (tb.InTableScope(t.AsEndTag().Name())) {
                            tb.ProcessEndTag("select");
                            return (tb.Process(t));
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return tb.Process(t, HtmlTreeBuilderState.InSelect);
                    }
                }
            }
        }

        internal static HtmlTreeBuilderState InSelectInTable = new _HtmlTreeBuilderState_1460();

        private sealed class _HtmlTreeBuilderState_1485 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1485() {
            }

            internal override String GetName() {
                return "AfterBody";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
                    return tb.Process(t, HtmlTreeBuilderState.InBody);
                }
                else {
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
                            if (t.IsStartTag() && t.AsStartTag().Name().Equals("html")) {
                                return tb.Process(t, HtmlTreeBuilderState.InBody);
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().Name().Equals("html")) {
                                    if (tb.IsFragmentParsing()) {
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        tb.Transition(HtmlTreeBuilderState.AfterAfterBody);
                                    }
                                }
                                else {
                                    if (t.IsEOF()) {
                                    }
                                    else {
                                        // chillax! we're done
                                        tb.Error(this);
                                        tb.Transition(HtmlTreeBuilderState.InBody);
                                        return tb.Process(t);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        internal static HtmlTreeBuilderState AfterBody = new _HtmlTreeBuilderState_1485();

        private sealed class _HtmlTreeBuilderState_1520 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1520() {
            }

            internal override String GetName() {
                return "InFrameset";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                String name;
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
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
                                name = start.Name();
                                if (name.Equals("html")) {
                                    return tb.Process(start, HtmlTreeBuilderState.InBody);
                                }
                                else {
                                    if (name.Equals("frameset")) {
                                        tb.Insert(start);
                                    }
                                    else {
                                        if (name.Equals("frame")) {
                                            tb.InsertEmpty(start);
                                        }
                                        else {
                                            if (name.Equals("noframes")) {
                                                return tb.Process(start, HtmlTreeBuilderState.InHead);
                                            }
                                            else {
                                                tb.Error(this);
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().Name().Equals("frameset")) {
                                    if (tb.CurrentElement().NodeName().Equals("html")) {
                                        // frag
                                        tb.Error(this);
                                        return false;
                                    }
                                    else {
                                        tb.Pop();
                                        if (!tb.IsFragmentParsing() && !tb.CurrentElement().NodeName().Equals("frameset")) {
                                            tb.Transition(HtmlTreeBuilderState.AfterFrameset);
                                        }
                                    }
                                }
                                else {
                                    if (t.IsEOF()) {
                                        if (!tb.CurrentElement().NodeName().Equals("html")) {
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
        }

        internal static HtmlTreeBuilderState InFrameset = new _HtmlTreeBuilderState_1520();

        private sealed class _HtmlTreeBuilderState_1574 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1574() {
            }

            internal override String GetName() {
                return "AfterFrameset";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (HtmlTreeBuilderState.IsWhitespace(t)) {
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
                            if (t.IsStartTag() && t.AsStartTag().Name().Equals("html")) {
                                return tb.Process(t, HtmlTreeBuilderState.InBody);
                            }
                            else {
                                if (t.IsEndTag() && t.AsEndTag().Name().Equals("html")) {
                                    tb.Transition(HtmlTreeBuilderState.AfterAfterFrameset);
                                }
                                else {
                                    if (t.IsStartTag() && t.AsStartTag().Name().Equals("noframes")) {
                                        return tb.Process(t, HtmlTreeBuilderState.InHead);
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
        }

        internal static HtmlTreeBuilderState AfterFrameset = new _HtmlTreeBuilderState_1574();

        private sealed class _HtmlTreeBuilderState_1605 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1605() {
            }

            internal override String GetName() {
                return "AfterAfterBody";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsComment()) {
                    tb.Insert(t.AsComment());
                }
                else {
                    if (t.IsDoctype() || HtmlTreeBuilderState.IsWhitespace(t) || (t.IsStartTag() && t.AsStartTag().Name().Equals
                        ("html"))) {
                        return tb.Process(t, HtmlTreeBuilderState.InBody);
                    }
                    else {
                        if (t.IsEOF()) {
                        }
                        else {
                            // nice work chuck
                            tb.Error(this);
                            tb.Transition(HtmlTreeBuilderState.InBody);
                            return tb.Process(t);
                        }
                    }
                }
                return true;
            }
        }

        internal static HtmlTreeBuilderState AfterAfterBody = new _HtmlTreeBuilderState_1605();

        private sealed class _HtmlTreeBuilderState_1628 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1628() {
            }

            internal override String GetName() {
                return "AfterAfterFrameset";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                if (t.IsComment()) {
                    tb.Insert(t.AsComment());
                }
                else {
                    if (t.IsDoctype() || HtmlTreeBuilderState.IsWhitespace(t) || (t.IsStartTag() && t.AsStartTag().Name().Equals
                        ("html"))) {
                        return tb.Process(t, HtmlTreeBuilderState.InBody);
                    }
                    else {
                        if (t.IsEOF()) {
                        }
                        else {
                            // nice work chuck
                            if (t.IsStartTag() && t.AsStartTag().Name().Equals("noframes")) {
                                return tb.Process(t, HtmlTreeBuilderState.InHead);
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
        }

        internal static HtmlTreeBuilderState AfterAfterFrameset = new _HtmlTreeBuilderState_1628();

        private sealed class _HtmlTreeBuilderState_1652 : HtmlTreeBuilderState {
            public _HtmlTreeBuilderState_1652() {
            }

            internal override String GetName() {
                return "ForeignContent";
            }

            internal override bool Process(Token t, HtmlTreeBuilder tb) {
                return true;
            }
        }

        internal static HtmlTreeBuilderState ForeignContent = new _HtmlTreeBuilderState_1652();

        // todo: implement. Also; how do we get here?
        public override String ToString() {
            return GetName();
        }

        internal abstract String GetName();

        private static String nullString = '\u0000'.ToString();

        internal abstract bool Process(Token t, HtmlTreeBuilder tb);

        private static bool IsWhitespace(Token t) {
            if (t.IsCharacter()) {
                String data = t.AsCharacter().GetData();
                return IsWhitespace(data);
            }
            return false;
        }

        private static bool IsWhitespace(String data) {
            // todo: this checks more than spec - "\t", "\n", "\f", "\r", " "
            for (int i = 0; i < data.Length; i++) {
                char c = data[i];
                if (!iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace(c)) {
                    return false;
                }
            }
            return true;
        }

        private static void HandleRcData(Token.StartTag startTag, HtmlTreeBuilder tb) {
            tb.Insert(startTag);
            tb.tokeniser.Transition(TokeniserState.Rcdata);
            tb.MarkInsertionMode();
            tb.Transition(Text);
        }

        private static void HandleRawtext(Token.StartTag startTag, HtmlTreeBuilder tb) {
            tb.Insert(startTag);
            tb.tokeniser.Transition(TokeniserState.Rawtext);
            tb.MarkInsertionMode();
            tb.Transition(Text);
        }

        // lists of tags to search through. A little harder to read here, but causes less GC than dynamic varargs.
        // was contributing around 10% of parse GC load.
        private sealed class Constants {
            internal static readonly String[] InBodyStartToHead = new String[] { "base", "basefont", "bgsound", "command"
                , "link", "meta", "noframes", "script", "style", "title" };

            internal static readonly String[] InBodyStartPClosers = new String[] { "address", "article", "aside", "blockquote"
                , "center", "details", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup"
                , "menu", "nav", "ol", "p", "section", "summary", "ul" };

            internal static readonly String[] Headings = new String[] { "h1", "h2", "h3", "h4", "h5", "h6" };

            internal static readonly String[] InBodyStartPreListing = new String[] { "pre", "listing" };

            internal static readonly String[] InBodyStartLiBreakers = new String[] { "address", "div", "p" };

            internal static readonly String[] DdDt = new String[] { "dd", "dt" };

            internal static readonly String[] Formatters = new String[] { "b", "big", "code", "em", "font", "i", "s", 
                "small", "strike", "strong", "tt", "u" };

            internal static readonly String[] InBodyStartApplets = new String[] { "applet", "marquee", "object" };

            internal static readonly String[] InBodyStartEmptyFormatters = new String[] { "area", "br", "embed", "img"
                , "keygen", "wbr" };

            internal static readonly String[] InBodyStartMedia = new String[] { "param", "source", "track" };

            internal static readonly String[] InBodyStartInputAttribs = new String[] { "name", "action", "prompt" };

            internal static readonly String[] InBodyStartOptions = new String[] { "optgroup", "option" };

            internal static readonly String[] InBodyStartRuby = new String[] { "rp", "rt" };

            internal static readonly String[] InBodyStartDrop = new String[] { "caption", "col", "colgroup", "frame", 
                "head", "tbody", "td", "tfoot", "th", "thead", "tr" };

            internal static readonly String[] InBodyEndClosers = new String[] { "address", "article", "aside", "blockquote"
                , "button", "center", "details", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header"
                , "hgroup", "listing", "menu", "nav", "ol", "pre", "section", "summary", "ul" };

            internal static readonly String[] InBodyEndAdoptionFormatters = new String[] { "a", "b", "big", "code", "em"
                , "font", "i", "nobr", "s", "small", "strike", "strong", "tt", "u" };

            internal static readonly String[] InBodyEndTableFosters = new String[] { "table", "tbody", "tfoot", "thead"
                , "tr" };
        }
    }
}
