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
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>HTML Tree Builder; creates a DOM from Tokens.</summary>
    public class HtmlTreeBuilder : TreeBuilder {
        // tag searches
        public static readonly String[] TagsSearchInScope = new String[] { "applet", "caption", "html", "table", "td"
            , "th", "marquee", "object" };

        private static readonly String[] TagSearchList = new String[] { "ol", "ul" };

        private static readonly String[] TagSearchButton = new String[] { "button" };

        private static readonly String[] TagSearchTableScope = new String[] { "html", "table" };

        private static readonly String[] TagSearchSelectScope = new String[] { "optgroup", "option" };

        private static readonly String[] TagSearchEndTags = new String[] { "dd", "dt", "li", "option", "optgroup", 
            "p", "rp", "rt" };

        private static readonly String[] TagSearchSpecial = new String[] { "address", "applet", "area", "article", 
            "aside", "base", "basefont", "bgsound", "blockquote", "body", "br", "button", "caption", "center", "col"
            , "colgroup", "command", "dd", "details", "dir", "div", "dl", "dt", "embed", "fieldset", "figcaption", 
            "figure", "footer", "form", "frame", "frameset", "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", 
            "hgroup", "hr", "html", "iframe", "img", "input", "isindex", "li", "link", "listing", "marquee", "menu"
            , "meta", "nav", "noembed", "noframes", "noscript", "object", "ol", "p", "param", "plaintext", "pre", 
            "script", "section", "select", "style", "summary", "table", "tbody", "td", "textarea", "tfoot", "th", 
            "thead", "title", "tr", "ul", "wbr", "xmp" };

        private HtmlTreeBuilderState state;

        // the current state
        private HtmlTreeBuilderState originalState;

        // original / marked state
        private bool baseUriSetFromDoc = false;

        private iText.StyledXmlParser.Jsoup.Nodes.Element headElement;

        // the current head element
        private FormElement formElement;

        // the current form element
        private iText.StyledXmlParser.Jsoup.Nodes.Element contextElement;

        // fragment parse context -- could be null even if fragment parsing
        private List<iText.StyledXmlParser.Jsoup.Nodes.Element> formattingElements = new List<iText.StyledXmlParser.Jsoup.Nodes.Element
            >();

        // active (open) formatting elements
        private IList<String> pendingTableCharacters = new List<String>();

        // chars in table to be shifted out
        private Token.EndTag emptyEnd = new Token.EndTag();

        // reused empty end tag
        private bool framesetOk = true;

        // if ok to go into frameset
        private bool fosterInserts = false;

        // if next inserts should be fostered
        private bool fragmentParsing = false;

        // if parsing a fragment of html
        internal HtmlTreeBuilder() {
        }

        internal override Document Parse(String input, String baseUri, ParseErrorList errors) {
            state = HtmlTreeBuilderState.Initial;
            baseUriSetFromDoc = false;
            return base.Parse(input, baseUri, errors);
        }

        internal virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String inputFragment, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri, ParseErrorList errors) {
            // context may be null
            state = HtmlTreeBuilderState.Initial;
            InitialiseParse(inputFragment, baseUri, errors);
            contextElement = context;
            fragmentParsing = true;
            iText.StyledXmlParser.Jsoup.Nodes.Element root = null;
            if (context != null) {
                if (context.OwnerDocument() != null) {
                    // quirks setup:
                    doc.QuirksMode(context.OwnerDocument().QuirksMode());
                }
                // initialise the tokeniser state:
                String contextTag = context.TagName();
                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(contextTag, "title", "textarea")) {
                    tokeniser.Transition(TokeniserState.Rcdata);
                }
                else {
                    if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(contextTag, "iframe", "noembed", "noframes", "style", 
                        "xmp")) {
                        tokeniser.Transition(TokeniserState.Rawtext);
                    }
                    else {
                        if (contextTag.Equals("script")) {
                            tokeniser.Transition(TokeniserState.ScriptData);
                        }
                        else {
                            if (contextTag.Equals(("noscript"))) {
                                tokeniser.Transition(TokeniserState.Data);
                            }
                            else {
                                // if scripting enabled, rawtext
                                if (contextTag.Equals("plaintext")) {
                                    tokeniser.Transition(TokeniserState.Data);
                                }
                                else {
                                    tokeniser.Transition(TokeniserState.Data);
                                }
                            }
                        }
                    }
                }
                // default
                root = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("html"
                    ), baseUri);
                doc.AppendChild(root);
                stack.Add(root);
                ResetInsertionMode();
                // setup form element to nearest form on context (up ancestor chain). ensures form controls are associated
                // with form correctly
                Elements contextChain = context.Parents();
                contextChain.Add(0, context);
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element parent in contextChain) {
                    if (parent is FormElement) {
                        formElement = (FormElement)parent;
                        break;
                    }
                }
            }
            RunParser();
            if (context != null && root != null) {
                return root.ChildNodes();
            }
            else {
                return doc.ChildNodes();
            }
        }

        internal override bool Process(Token token) {
            currentToken = token;
            return this.state.Process(token, this);
        }

        internal virtual bool Process(Token token, HtmlTreeBuilderState state) {
            currentToken = token;
            return state.Process(token, this);
        }

        internal virtual void Transition(HtmlTreeBuilderState state) {
            this.state = state;
        }

        internal virtual HtmlTreeBuilderState State() {
            return state;
        }

        internal virtual void MarkInsertionMode() {
            originalState = state;
        }

        internal virtual HtmlTreeBuilderState OriginalState() {
            return originalState;
        }

        internal virtual void FramesetOk(bool framesetOk) {
            this.framesetOk = framesetOk;
        }

        internal virtual bool FramesetOk() {
            return framesetOk;
        }

        internal virtual Document GetDocument() {
            return doc;
        }

        internal virtual String GetBaseUri() {
            return baseUri;
        }

        internal virtual void MaybeSetBaseUri(iText.StyledXmlParser.Jsoup.Nodes.Element @base) {
            if (baseUriSetFromDoc) {
                // only listen to the first <base href> in parse
                return;
            }
            String href = @base.AbsUrl("href");
            if (href.Length != 0) {
                // ignore <base target> etc
                baseUri = href;
                baseUriSetFromDoc = true;
                doc.SetBaseUri(href);
            }
        }

        // set on the doc so doc.createElement(Tag) will get updated base, and to update all descendants
        internal virtual bool IsFragmentParsing() {
            return fragmentParsing;
        }

        internal virtual void Error(HtmlTreeBuilderState state) {
            if (errors.CanAddError()) {
                errors.Add(new ParseError(reader.Pos(), "Unexpected token [{0}] when in state [{1}]", currentToken.TokenType
                    (), state));
            }
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element Insert(Token.StartTag startTag) {
            // handle empty unknown tags
            // when the spec expects an empty tag, will directly hit insertEmpty, so won't generate this fake end tag.
            if (startTag.IsSelfClosing()) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = InsertEmpty(startTag);
                stack.Add(el);
                tokeniser.Transition(TokeniserState.Data);
                // handles <script />, otherwise needs breakout steps from script data
                tokeniser.Emit(((Token.Tag)emptyEnd.Reset()).Name(el.TagName()));
                // ensure we get out of whatever state we are in. emitted for yielded processing
                return el;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element el_1 = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(startTag.Name()), baseUri, startTag.attributes);
            Insert(el_1);
            return el_1;
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element InsertStartTag(String startTagName) {
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(startTagName), baseUri);
            Insert(el);
            return el;
        }

        internal virtual void Insert(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            InsertNode(el);
            stack.Add(el);
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element InsertEmpty(Token.StartTag startTag) {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(startTag.Name(
                ));
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, baseUri, 
                startTag.attributes);
            InsertNode(el);
            if (startTag.IsSelfClosing()) {
                if (tag.IsKnownTag()) {
                    if (tag.IsSelfClosing()) {
                        tokeniser.AcknowledgeSelfClosingFlag();
                    }
                }
                else {
                    // if not acked, promulagates error
                    // unknown tag, remember this is self closing for output
                    tag.SetSelfClosing();
                    tokeniser.AcknowledgeSelfClosingFlag();
                }
            }
            // not an distinct error
            return el;
        }

        internal virtual FormElement InsertForm(Token.StartTag startTag, bool onStack) {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(startTag.Name(
                ));
            FormElement el = new FormElement(tag, baseUri, startTag.attributes);
            SetFormElement(el);
            InsertNode(el);
            if (onStack) {
                stack.Add(el);
            }
            return el;
        }

        internal virtual void Insert(Token.Comment commentToken) {
            Comment comment = new Comment(commentToken.GetData(), baseUri);
            InsertNode(comment);
        }

        internal virtual void Insert(Token.Character characterToken) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node;
            // characters in script and style go in as datanodes, not text nodes
            String tagName = CurrentElement().TagName();
            if (tagName.Equals("script") || tagName.Equals("style")) {
                node = new DataNode(characterToken.GetData(), baseUri);
            }
            else {
                node = new TextNode(characterToken.GetData(), baseUri);
            }
            CurrentElement().AppendChild(node);
        }

        // doesn't use insertNode, because we don't foster these; and will always have a stack.
        private void InsertNode(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            // if the stack hasn't been set up yet, elements (doctype, comments) go into the doc
            if (stack.Count == 0) {
                doc.AppendChild(node);
            }
            else {
                if (IsFosterInserts()) {
                    InsertInFosterParent(node);
                }
                else {
                    CurrentElement().AppendChild(node);
                }
            }
            // connect form controls to their form element
            if (node is iText.StyledXmlParser.Jsoup.Nodes.Element && ((iText.StyledXmlParser.Jsoup.Nodes.Element)node)
                .Tag().IsFormListed()) {
                if (formElement != null) {
                    formElement.AddElement((iText.StyledXmlParser.Jsoup.Nodes.Element)node);
                }
            }
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element Pop() {
            int size = stack.Count;
            return stack.JRemoveAt(size - 1);
        }

        internal virtual void Push(iText.StyledXmlParser.Jsoup.Nodes.Element element) {
            stack.Add(element);
        }

        internal virtual List<iText.StyledXmlParser.Jsoup.Nodes.Element> GetStack() {
            return stack;
        }

        internal virtual bool OnStack(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            return IsElementInQueue(stack, el);
        }

        private bool IsElementInQueue(List<iText.StyledXmlParser.Jsoup.Nodes.Element> queue, iText.StyledXmlParser.Jsoup.Nodes.Element
             element) {
            for (int pos = queue.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = queue[pos];
                if (next == element) {
                    return true;
                }
            }
            return false;
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetFromStack(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    return next;
                }
            }
            return null;
        }

        internal virtual bool RemoveFromStack(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next == el) {
                    stack.JRemoveAt(pos);
                    return true;
                }
            }
            return false;
        }

        internal virtual void PopStackToClose(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (next.NodeName().Equals(elName)) {
                    break;
                }
            }
        }

        internal virtual void PopStackToClose(params String[] elNames) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                stack.JRemoveAt(pos);
                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(next.NodeName(), elNames)) {
                    break;
                }
            }
        }

        internal virtual void PopStackToBefore(String elName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next.NodeName().Equals(elName)) {
                    break;
                }
                else {
                    stack.JRemoveAt(pos);
                }
            }
        }

        internal virtual void ClearStackToTableContext() {
            ClearStackToContext("table");
        }

        internal virtual void ClearStackToTableBodyContext() {
            ClearStackToContext("tbody", "tfoot", "thead");
        }

        internal virtual void ClearStackToTableRowContext() {
            ClearStackToContext("tr");
        }

        private void ClearStackToContext(params String[] nodeNames) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(next.NodeName(), nodeNames) || next.NodeName().Equals
                    ("html")) {
                    break;
                }
                else {
                    stack.JRemoveAt(pos);
                }
            }
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element AboveOnStack(iText.StyledXmlParser.Jsoup.Nodes.Element
             el) {
            System.Diagnostics.Debug.Assert(OnStack(el));
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = stack[pos];
                if (next == el) {
                    return stack[pos - 1];
                }
            }
            return null;
        }

        internal virtual void InsertOnStackAfter(iText.StyledXmlParser.Jsoup.Nodes.Element after, iText.StyledXmlParser.Jsoup.Nodes.Element
             @in) {
            int i = stack.LastIndexOf(after);
            Validate.IsTrue(i != -1);
            stack.Add(i + 1, @in);
        }

        internal virtual void ReplaceOnStack(iText.StyledXmlParser.Jsoup.Nodes.Element @out, iText.StyledXmlParser.Jsoup.Nodes.Element
             @in) {
            ReplaceInQueue(stack, @out, @in);
        }

        private void ReplaceInQueue(List<iText.StyledXmlParser.Jsoup.Nodes.Element> queue, iText.StyledXmlParser.Jsoup.Nodes.Element
             @out, iText.StyledXmlParser.Jsoup.Nodes.Element @in) {
            int i = queue.LastIndexOf(@out);
            Validate.IsTrue(i != -1);
            queue[i] = @in;
        }

        internal virtual void ResetInsertionMode() {
            bool last = false;
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element node = stack[pos];
                if (pos == 0) {
                    last = true;
                    node = contextElement;
                }
                String name = node.NodeName();
                if ("select".Equals(name)) {
                    Transition(HtmlTreeBuilderState.InSelect);
                    break;
                }
                else {
                    // frag
                    if (("td".Equals(name) || "th".Equals(name) && !last)) {
                        Transition(HtmlTreeBuilderState.InCell);
                        break;
                    }
                    else {
                        if ("tr".Equals(name)) {
                            Transition(HtmlTreeBuilderState.InRow);
                            break;
                        }
                        else {
                            if ("tbody".Equals(name) || "thead".Equals(name) || "tfoot".Equals(name)) {
                                Transition(HtmlTreeBuilderState.InTableBody);
                                break;
                            }
                            else {
                                if ("caption".Equals(name)) {
                                    Transition(HtmlTreeBuilderState.InCaption);
                                    break;
                                }
                                else {
                                    if ("colgroup".Equals(name)) {
                                        Transition(HtmlTreeBuilderState.InColumnGroup);
                                        break;
                                    }
                                    else {
                                        // frag
                                        if ("table".Equals(name)) {
                                            Transition(HtmlTreeBuilderState.InTable);
                                            break;
                                        }
                                        else {
                                            if ("head".Equals(name)) {
                                                Transition(HtmlTreeBuilderState.InBody);
                                                break;
                                            }
                                            else {
                                                // frag
                                                if ("body".Equals(name)) {
                                                    Transition(HtmlTreeBuilderState.InBody);
                                                    break;
                                                }
                                                else {
                                                    if ("frameset".Equals(name)) {
                                                        Transition(HtmlTreeBuilderState.InFrameset);
                                                        break;
                                                    }
                                                    else {
                                                        // frag
                                                        if ("html".Equals(name)) {
                                                            Transition(HtmlTreeBuilderState.BeforeHead);
                                                            break;
                                                        }
                                                        else {
                                                            // frag
                                                            if (last) {
                                                                Transition(HtmlTreeBuilderState.InBody);
                                                                break;
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

        // frag
        // todo: tidy up in specific scope methods
        private String[] specificScopeTarget = new String[] { null };

        private bool InSpecificScope(String targetName, String[] baseTypes, String[] extraTypes) {
            specificScopeTarget[0] = targetName;
            return InSpecificScope(specificScopeTarget, baseTypes, extraTypes);
        }

        private bool InSpecificScope(String[] targetNames, String[] baseTypes, String[] extraTypes) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[pos];
                String elName = el.NodeName();
                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(elName, targetNames)) {
                    return true;
                }
                if (iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(elName, baseTypes)) {
                    return false;
                }
                if (extraTypes != null && iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(elName, extraTypes)) {
                    return false;
                }
            }
            Validate.Fail("Should not be reachable");
            return false;
        }

        internal virtual bool InScope(String[] targetNames) {
            return InSpecificScope(targetNames, TagsSearchInScope, null);
        }

        internal virtual bool InScope(String targetName) {
            return InScope(targetName, null);
        }

        internal virtual bool InScope(String targetName, String[] extras) {
            return InSpecificScope(targetName, TagsSearchInScope, extras);
        }

        // todo: in mathml namespace: mi, mo, mn, ms, mtext annotation-xml
        // todo: in svg namespace: forignOjbect, desc, title
        internal virtual bool InListItemScope(String targetName) {
            return InScope(targetName, TagSearchList);
        }

        internal virtual bool InButtonScope(String targetName) {
            return InScope(targetName, TagSearchButton);
        }

        internal virtual bool InTableScope(String targetName) {
            return InSpecificScope(targetName, TagSearchTableScope, null);
        }

        internal virtual bool InSelectScope(String targetName) {
            for (int pos = stack.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = stack[pos];
                String elName = el.NodeName();
                if (elName.Equals(targetName)) {
                    return true;
                }
                if (!iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(elName, TagSearchSelectScope)) {
                    // all elements except
                    return false;
                }
            }
            Validate.Fail("Should not be reachable");
            return false;
        }

        internal virtual void SetHeadElement(iText.StyledXmlParser.Jsoup.Nodes.Element headElement) {
            this.headElement = headElement;
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetHeadElement() {
            return headElement;
        }

        internal virtual bool IsFosterInserts() {
            return fosterInserts;
        }

        internal virtual void SetFosterInserts(bool fosterInserts) {
            this.fosterInserts = fosterInserts;
        }

        internal virtual FormElement GetFormElement() {
            return formElement;
        }

        internal virtual void SetFormElement(FormElement formElement) {
            this.formElement = formElement;
        }

        internal virtual void NewPendingTableCharacters() {
            pendingTableCharacters = new List<String>();
        }

        internal virtual IList<String> GetPendingTableCharacters() {
            return pendingTableCharacters;
        }

        internal virtual void SetPendingTableCharacters(IList<String> pendingTableCharacters) {
            this.pendingTableCharacters = pendingTableCharacters;
        }

        /// <summary>11.2.5.2 Closing elements that have implied end tags</summary>
        /// <remarks>
        /// 11.2.5.2 Closing elements that have implied end tags
        /// <para />
        /// When the steps below require the UA to generate implied end tags, then, while the current node is a dd element, a
        /// dt element, an li element, an option element, an optgroup element, a p element, an rp element, or an rt element,
        /// the UA must pop the current node off the stack of open elements.
        /// </remarks>
        /// <param name="excludeTag">
        /// If a step requires the UA to generate implied end tags but lists an element to exclude from the
        /// process, then the UA must perform the above steps as if that element was not in the above list.
        /// </param>
        internal virtual void GenerateImpliedEndTags(String excludeTag) {
            while ((excludeTag != null && !CurrentElement().NodeName().Equals(excludeTag)) && iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .In(CurrentElement().NodeName(), TagSearchEndTags)) {
                Pop();
            }
        }

        internal virtual void GenerateImpliedEndTags() {
            GenerateImpliedEndTags(null);
        }

        internal virtual bool IsSpecial(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            // todo: mathml's mi, mo, mn
            // todo: svg's foreigObject, desc, title
            String name = el.NodeName();
            return iText.StyledXmlParser.Jsoup.Helper.StringUtil.In(name, TagSearchSpecial);
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element LastFormattingElement() {
            return formattingElements.Count > 0 ? formattingElements[formattingElements.Count - 1] : null;
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element RemoveLastFormattingElement() {
            int size = formattingElements.Count;
            if (size > 0) {
                return formattingElements.JRemoveAt(size - 1);
            }
            else {
                return null;
            }
        }

        // active formatting elements
        internal virtual void PushActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element @in) {
            int numSeen = 0;
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = formattingElements[pos];
                if (el == null) {
                    // marker
                    break;
                }
                if (IsSameFormattingElement(@in, el)) {
                    numSeen++;
                }
                if (numSeen == 3) {
                    formattingElements.JRemoveAt(pos);
                    break;
                }
            }
            formattingElements.Add(@in);
        }

        private bool IsSameFormattingElement(iText.StyledXmlParser.Jsoup.Nodes.Element a, iText.StyledXmlParser.Jsoup.Nodes.Element
             b) {
            // same if: same namespace, tag, and attributes. Element.equals only checks tag, might in future check children
            return a.NodeName().Equals(b.NodeName()) && 
                        // a.namespace().equals(b.namespace()) &&
                        a.Attributes().Equals(b.Attributes());
        }

        // todo: namespaces
        internal virtual void ReconstructFormattingElements() {
            iText.StyledXmlParser.Jsoup.Nodes.Element last = LastFormattingElement();
            if (last == null || OnStack(last)) {
                return;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element entry = last;
            int size = formattingElements.Count;
            int pos = size - 1;
            bool skip = false;
            while (true) {
                if (pos == 0) {
                    // step 4. if none before, skip to 8
                    skip = true;
                    break;
                }
                entry = formattingElements[--pos];
                // step 5. one earlier than entry
                if (entry == null || OnStack(entry)) {
                    // step 6 - neither marker nor on stack
                    break;
                }
            }
            // jump to 8, else continue back to 4
            while (true) {
                if (!skip) {
                    // step 7: on later than entry
                    entry = formattingElements[++pos];
                }
                Validate.NotNull(entry);
                // should not occur, as we break at last element
                // 8. create new element from element, 9 insert into current node, onto stack
                skip = false;
                // can only skip increment from 4.
                iText.StyledXmlParser.Jsoup.Nodes.Element newEl = InsertStartTag(entry.NodeName());
                // todo: avoid fostering here?
                // newEl.namespace(entry.namespace()); // todo: namespaces
                newEl.Attributes().AddAll(entry.Attributes());
                // 10. replace entry with new entry
                formattingElements[pos] = newEl;
                // 11
                if (pos == size - 1) {
                    // if not last entry in list, jump to 7
                    break;
                }
            }
        }

        internal virtual void ClearFormattingElementsToLastMarker() {
            while (!formattingElements.IsEmpty()) {
                iText.StyledXmlParser.Jsoup.Nodes.Element el = RemoveLastFormattingElement();
                if (el == null) {
                    break;
                }
            }
        }

        internal virtual void RemoveFromActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = formattingElements[pos];
                if (next == el) {
                    formattingElements.JRemoveAt(pos);
                    break;
                }
            }
        }

        internal virtual bool IsInActiveFormattingElements(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            return IsElementInQueue(formattingElements, el);
        }

        internal virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetActiveFormattingElement(String nodeName) {
            for (int pos = formattingElements.Count - 1; pos >= 0; pos--) {
                iText.StyledXmlParser.Jsoup.Nodes.Element next = formattingElements[pos];
                if (next == null) {
                    // scope marker
                    break;
                }
                else {
                    if (next.NodeName().Equals(nodeName)) {
                        return next;
                    }
                }
            }
            return null;
        }

        internal virtual void ReplaceActiveFormattingElement(iText.StyledXmlParser.Jsoup.Nodes.Element @out, iText.StyledXmlParser.Jsoup.Nodes.Element
             @in) {
            ReplaceInQueue(formattingElements, @out, @in);
        }

        internal virtual void InsertMarkerToFormattingElements() {
            formattingElements.Add(null);
        }

        internal virtual void InsertInFosterParent(iText.StyledXmlParser.Jsoup.Nodes.Node @in) {
            iText.StyledXmlParser.Jsoup.Nodes.Element fosterParent;
            iText.StyledXmlParser.Jsoup.Nodes.Element lastTable = GetFromStack("table");
            bool isLastTableParent = false;
            if (lastTable != null) {
                if (lastTable.Parent() != null) {
                    fosterParent = (iText.StyledXmlParser.Jsoup.Nodes.Element)lastTable.Parent();
                    isLastTableParent = true;
                }
                else {
                    fosterParent = AboveOnStack(lastTable);
                }
            }
            else {
                // no table == frag
                fosterParent = stack[0];
            }
            if (isLastTableParent) {
                Validate.NotNull(lastTable);
                // last table cannot be null by this point.
                lastTable.Before(@in);
            }
            else {
                fosterParent.AppendChild(@in);
            }
        }

        public override String ToString() {
            return "TreeBuilder{" + "currentToken=" + currentToken + ", state=" + state + ", currentElement=" + CurrentElement
                () + '}';
        }
    }
}
