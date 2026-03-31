/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPseudoClassHasSelectorItemTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void HasDescendantSelectorItemTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector("p"));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div id='host'><p id='p1'></p></div>");
            IElementNode host = FindElementById(documentNode, "host");
            NUnit.Framework.Assert.IsFalse(item.Matches(documentNode));
            NUnit.Framework.Assert.IsTrue(item.Matches(host));
            NUnit.Framework.Assert.IsFalse(item.Matches(null));
        }

        [NUnit.Framework.Test]
        public virtual void HasDirectChildSelectorItemTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("> p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docDirect = htmlParser.Parse("<div id='host'><p id='p1'></p></div>");
            IElementNode hostDirect = FindElementById(docDirect, "host");
            IDocumentNode docNested = htmlParser.Parse("<div id='host'><span><p id='p1'></p></span></div>");
            IElementNode hostNested = FindElementById(docNested, "host");
            NUnit.Framework.Assert.IsTrue(item.Matches(hostDirect));
            NUnit.Framework.Assert.IsFalse(item.Matches(hostNested));
        }

        [NUnit.Framework.Test]
        public virtual void HasNextSiblingSelectorItemTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("+ p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<span id='a'></span>" + "<p id='b'></p>" + "<p id='c'></p>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            NUnit.Framework.Assert.IsTrue(item.Matches(a));
        }

        [NUnit.Framework.Test]
        public virtual void HasNextSiblingSelectorItemSkipsNonElementNodesTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("+ p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<span id='a'></span>" + " some text " + "<p id='b'></p>" +
                 "</div>");
            IElementNode a = FindElementById(doc, "a");
            NUnit.Framework.Assert.IsTrue(item.Matches(a));
        }

        [NUnit.Framework.Test]
        public virtual void HasFollowingSiblingSelectorItemTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("~ p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<span id='a'></span>" + "<span id='x'></span>" + "<p id='b'></p>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            NUnit.Framework.Assert.IsTrue(item.Matches(a));
        }

        [NUnit.Framework.Test]
        public virtual void HasNegativeCaseNoMatchingElementsTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector("p.needle"));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div id='host'><p class='other'></p></div>");
            IElementNode host = FindElementById(documentNode, "host");
            NUnit.Framework.Assert.IsFalse(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasSelectorWithCommaInAttributeTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("span[title='a,b']")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><span title='a,b'></span></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsTrue(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasSelectorIntegrationViaCssSelectorTest() {
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div>" + "<div id='a'><p></p></div>" + "<div id='b'><span></span></div>"
                 + "</div>");
            IElementNode a = FindElementById(documentNode, "a");
            IElementNode b = FindElementById(documentNode, "b");
            CssSelector selector = new CssSelector("div:has(p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void HasDoesNotMatchBasedOnScopeElementItselfTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector("div"));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'></div>");
            IElementNode host = FindElementById(doc, "host");
            // Must be false because the scope element itself should not be considered a "descendant".
            NUnit.Framework.Assert.IsFalse(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasDescendantDeepNestedSelectorItemTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector("p"));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><span><em><p id='p1'></p></em></span></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsTrue(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasDirectChildDoesNotMatchNestedDescendantTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("> p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><span><p id='p1'></p></span></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsFalse(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasNextSiblingIsOnlyNextElementSiblingTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("+ p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<span id='a'></span>" + "<span id='x'></span>" + "<p id='b'></p>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            // Next element sibling is <span id='x'>, so "+ p" must be false.
            NUnit.Framework.Assert.IsFalse(item.Matches(a));
        }

        [NUnit.Framework.Test]
        public virtual void HasFollowingSiblingSkipsUntilMatchTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("~ p")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<span id='a'></span>" + "<span id='x'></span>" + "<p id='b'></p>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            // "~ p" should match any following element sibling, not necessarily the next.
            NUnit.Framework.Assert.IsTrue(item.Matches(a));
        }

        [NUnit.Framework.Test]
        public virtual void HasSiblingSelectorsReturnFalseWhenNoParentTest() {
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='root'></div>");
            IElementNode root = FindElementById(doc, "root");
            CssPseudoClassHasSelectorItem nextSibling = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("+ p")[0]);
            CssPseudoClassHasSelectorItem followingSibling = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                ("~ p")[0]);
            NUnit.Framework.Assert.IsFalse(nextSibling.Matches(root));
            NUnit.Framework.Assert.IsFalse(followingSibling.Matches(root));
        }

        [NUnit.Framework.Test]
        public virtual void HasSupportsAttributeSelectorInArgumentsTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector("[data-x]"));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><span data-x='1'></span></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsTrue(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasSupportsMultiStepSelectorInArgumentsTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector("div > p"));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'><div><p></p></div></section>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsTrue(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasIntegrationWithLeadingCombinatorViaCssSelectorTest() {
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<div id='a'><p></p></div>" + "<div id='b'><span><p></p></span></div>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector("div:has(> p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void HasCombinatorOnlyReturnsFalseTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                (">")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><p></p></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsFalse(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasSelectorListCommaSeparatedTest() {
            // :has(p, span) should match if element has either p OR span descendant
            CssSelector selector = new CssSelector("div:has(p, span)");
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<div id='a'><p></p></div>" + "<div id='b'><span></span></div>"
                 + "<div id='c'><em></em></div>" + "</div>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            IElementNode c = FindElementById(doc, "c");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            // has <p>
            NUnit.Framework.Assert.IsTrue(selector.Matches(b));
            // has <span>
            NUnit.Framework.Assert.IsFalse(selector.Matches(c));
        }

        // has neither
        [NUnit.Framework.Test]
        public virtual void HasSelectorWithMultipleCombinatorsDoesNotMatchWhenHierarchyDiffersTest() {
            CssSelector selector = new CssSelector("div:has(div > p > span)");
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<div id='a'><p></p><div><span></span></div></div>" + "<div id='b'><span></span></div>"
                 + "<div id='c'><em></em></div>" + "</div>");
            IElementNode a = FindElementById(doc, "a");
            NUnit.Framework.Assert.IsFalse(selector.Matches(a));
        }

        [NUnit.Framework.Test]
        public virtual void HasSpecificityWithSelectorListTest() {
            // Specificity should be the maximum of all selectors
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(JavaUtil.ArraysAsList((ICssSelector
                )new CssSelector("p"), (ICssSelector)new CssSelector("#id")), "p, #id");
            // #id has higher specificity (0,1,0,0) than p (0,0,0,1)
            NUnit.Framework.Assert.AreEqual(new CssSelector("#id").CalculateSpecificity(), item.GetSpecificity());
        }

        [NUnit.Framework.Test]
        public virtual void HasMalformedCombinatorOnlyTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(CssSelectorParser.ParseCommaSeparatedSelectors
                (">")[0]);
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><p></p></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsFalse(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasWithPseudoElementReturnsNullTest() {
            // Pseudo-elements in :has() should not be supported
            CssPseudoClassSelectorItem item = CssPseudoClassSelectorItem.Create("has", "p::before");
            NUnit.Framework.Assert.IsNull(item);
        }

        [NUnit.Framework.Test]
        public virtual void HasEmptyArgumentsReturnsFalseTest() {
            CssPseudoClassHasSelectorItem item = new CssPseudoClassHasSelectorItem(new CssSelector(""));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div id='host'><p></p></div>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsFalse(item.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasNoSpaceAfterLeadingCombinatorTest() {
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<div id='a'><p></p></div>" + "<div id='b'><span></span><p></p></div>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            NUnit.Framework.Assert.IsTrue(new CssSelector("div:has(>p)").Matches(a));
            NUnit.Framework.Assert.IsTrue(new CssSelector("div:has(>p)").Matches(b));
            // direct children are <span> and <p> in b? actually b has direct <span> and <p>
            NUnit.Framework.Assert.IsTrue(new CssSelector("div:has(>p)").Matches(b));
            NUnit.Framework.Assert.IsFalse(new CssSelector("div:has(+p)").Matches(a));
            // next element sibling is b (div), not p
            NUnit.Framework.Assert.IsFalse(new CssSelector("div:has(~p)").Matches(a));
        }

        // following siblings are divs, not p
        [NUnit.Framework.Test]
        public virtual void HasSelectorListWithMixedLeadingCombinatorsTest() {
            // div:has(> p, +span, em) should match:
            // - by direct child p
            // - OR by next sibling span
            // - OR by descendant em
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section>" + "<div id='x'><p></p></div>" + "<div id='y'></div>" + "<div id='z'><em></em></div>"
                 + "<span id='s'></span>" + "</section>");
            IElementNode x = FindElementById(doc, "x");
            IElementNode y = FindElementById(doc, "y");
            IElementNode z = FindElementById(doc, "z");
            CssSelector selector = new CssSelector("div:has(> p, +span, em)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(x));
            // has direct child <p>
            NUnit.Framework.Assert.IsFalse(selector.Matches(y));
            // no direct <p>, no descendant <em>, next sibling is z (div) not span
            NUnit.Framework.Assert.IsTrue(selector.Matches(z));
        }

        // has descendant <em>
        [NUnit.Framework.Test]
        public virtual void HasDeepRelativeSelectorTest() {
            // :has(> div > p) requires a direct child div which itself has a direct child p.
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docOk = htmlParser.Parse("<section id='host'><div><p></p></div></section>");
            IElementNode hostOk = FindElementById(docOk, "host");
            IDocumentNode docBad = htmlParser.Parse("<section id='host'><span><p></p></span></section>");
            IElementNode hostBad = FindElementById(docBad, "host");
            CssSelector selector = new CssSelector("section:has(> div > p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(hostOk));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBad));
        }

        [NUnit.Framework.Test]
        public virtual void HasChainedNextSiblingRelativeSelectorTest() {
            // :has(+ div > p) requires that the next element sibling is a div that has a direct child p.
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docOk = htmlParser.Parse("<section>" + "<span id='host'></span>" + "<div><p></p></div>" + "</section>"
                );
            IElementNode hostOk = FindElementById(docOk, "host");
            IDocumentNode docBadNextIsNotDiv = htmlParser.Parse("<section>" + "<span id='host'></span>" + "<span><p></p></span>"
                 + "</section>");
            IElementNode hostBadNextIsNotDiv = FindElementById(docBadNextIsNotDiv, "host");
            IDocumentNode docBadDivWithoutP = htmlParser.Parse("<section>" + "<span id='host'></span>" + "<div><span></span></div>"
                 + "</section>");
            IElementNode hostBadDivWithoutP = FindElementById(docBadDivWithoutP, "host");
            CssSelector selector = new CssSelector("span:has(+ div > p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(hostOk));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBadNextIsNotDiv));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBadDivWithoutP));
        }

        [NUnit.Framework.Test]
        public virtual void HasChainedFollowingSiblingRelativeSelectorTest() {
            // :has(~ div > p) requires that some following element sibling is a div that has a direct child p.
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docOk = htmlParser.Parse("<section>" + "<span id='host'></span>" + "<span></span>" + "<div><p></p></div>"
                 + "</section>");
            IElementNode hostOk = FindElementById(docOk, "host");
            IDocumentNode docBadNoFollowingDiv = htmlParser.Parse("<section>" + "<span id='host'></span>" + "<span></span>"
                 + "<span><p></p></span>" + "</section>");
            IElementNode hostBadNoFollowingDiv = FindElementById(docBadNoFollowingDiv, "host");
            IDocumentNode docBadDivWithoutP = htmlParser.Parse("<section>" + "<span id='host'></span>" + "<div><span></span></div>"
                 + "<div><span></span></div>" + "</section>");
            IElementNode hostBadDivWithoutP = FindElementById(docBadDivWithoutP, "host");
            CssSelector selector = new CssSelector("span:has(~ div > p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(hostOk));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBadNoFollowingDiv));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBadDivWithoutP));
        }

        [NUnit.Framework.Test]
        public virtual void HasNestedHasInFullSelectorShouldThrowTest() {
            // Factory returns null for nested :has(), and parser treats unsupported pseudo selector as an error.
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CssSelector("div:has(p:has(span))"));
        }

        [NUnit.Framework.Test]
        public virtual void HasNestedNotSelectorTest() {
            // div:has(p:not(.ignore))
            CssSelector selector = new CssSelector("div:has(p:not(.ignore))");
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<div>" + "<div id='a'><p>Valid</p></div>" + "<div id='b'><p class='ignore'>Ignored</p></div>"
                 + "</div>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void HasWithComplexDescendantSelectorTest() {
            // div:has(section p) means: match a div that contains a section which contains a p
            // The section must be INSIDE the div, not outside
            IXmlParser htmlParser = new JsoupHtmlParser();
            // Case 1: section is INSIDE the div -> should match
            IDocumentNode docMatch = htmlParser.Parse("<div id='host'>" + "<section>" + "<p></p>" + "</section>" + "</div>"
                );
            IElementNode hostMatch = FindElementById(docMatch, "host");
            // Case 2: section is OUTSIDE the div (ancestor) -> should NOT match
            // because :has() only searches descendants
            IDocumentNode docNoMatch = htmlParser.Parse("<section>" + "<div id='host'>" + "<p></p>" + "</div>" + "</section>"
                );
            IElementNode hostNoMatch = FindElementById(docNoMatch, "host");
            CssSelector selector = new CssSelector("div:has(section p)");
            // Should match: div contains section > p
            NUnit.Framework.Assert.IsTrue(selector.Matches(hostMatch));
            // Should NOT match: section is ancestor, not descendant
            // This is correct behavior per CSS spec
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostNoMatch));
        }

        [NUnit.Framework.Test]
        public virtual void HasWithNestedStructureRequiringBothLevelsTest() {
            // div:has(ul li.active) - match div that has a ul containing li.active
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docMatch = htmlParser.Parse("<div id='a'>" + "<ul>" + "<li class='active'>Item</li>" + "</ul>"
                 + "</div>");
            IDocumentNode docNoUl = htmlParser.Parse("<div id='b'>" + "<li class='active'>Item without ul parent</li>"
                 + "</div>");
            IDocumentNode docNoActiveClass = htmlParser.Parse("<div id='c'>" + "<ul>" + "<li>Item without active class</li>"
                 + "</ul>" + "</div>");
            IElementNode a = FindElementById(docMatch, "a");
            IElementNode b = FindElementById(docNoUl, "b");
            IElementNode c = FindElementById(docNoActiveClass, "c");
            CssSelector selector = new CssSelector("div:has(ul li.active)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            // has ul > li.active
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
            // li.active exists but not inside ul
            NUnit.Framework.Assert.IsFalse(selector.Matches(c));
        }

        // ul > li exists but not .active
        [NUnit.Framework.Test]
        public virtual void HasWithAdjacentSiblingSelectorInArgumentsTest() {
            // div:has(span + p) should match only when a <p> exists whose immediately preceding sibling is <span>
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docOk = htmlParser.Parse("<div id='host'>" + "<span></span>" + "<p></p>" + "</div>");
            IElementNode hostOk = FindElementById(docOk, "host");
            IDocumentNode docBadNotAdjacent = htmlParser.Parse("<div id='host'>" + "<span></span>" + "<em></em>" + "<p></p>"
                 + "</div>");
            IElementNode hostBadNotAdjacent = FindElementById(docBadNotAdjacent, "host");
            CssSelector selector = new CssSelector("div:has(span + p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(hostOk));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBadNotAdjacent));
        }

        [NUnit.Framework.Test]
        public virtual void HasWithGeneralSiblingSelectorInArgumentsTest() {
            // div:has(span ~ p) should match when a <p> exists that has any preceding <span> sibling
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode docOk = htmlParser.Parse("<div id='host'>" + "<span></span>" + "<em></em>" + "<p></p>" + "</div>"
                );
            IElementNode hostOk = FindElementById(docOk, "host");
            IDocumentNode docBadSpanAfterP = htmlParser.Parse("<div id='host'>" + "<p></p>" + "<span></span>" + "</div>"
                );
            IElementNode hostBadSpanAfterP = FindElementById(docBadSpanAfterP, "host");
            CssSelector selector = new CssSelector("div:has(span ~ p)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(hostOk));
            NUnit.Framework.Assert.IsFalse(selector.Matches(hostBadSpanAfterP));
        }

        [NUnit.Framework.Test]
        public virtual void HasRelativeSelectorWithDescendantCombinatorStepMatchesDeepDescendantTest() {
            // Covers CssPseudoClassHasSelectorItem#fillNextScopesByCombinator case ' '
            // via a relative selector that includes a descendant combinator step:
            // :has(> div p) => first step '>' then step ' ' (descendant).
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'>" + "<div>" + "<span><p class='target'></p></span>"
                 + "</div>" + "</section>");
            IElementNode host = FindElementById(doc, "host");
            CssSelector selector = new CssSelector("section:has(> div p.target)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasRelativeSelectorWithDescendantCombinatorStepDoesNotEscapeChildScopeTest() {
            // Negative case for :has(> div p): p exists, but not as a descendant of the matched "div" scope.
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'>" + "<div></div>" + "<p class='target'></p>" + "</section>"
                );
            IElementNode host = FindElementById(doc, "host");
            CssSelector selector = new CssSelector("section:has(> div p.target)");
            NUnit.Framework.Assert.IsFalse(selector.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasArgumentSelectorDoesNotEscapeHasScopeWhenMatchingAncestorsTest() {
            // Verifies CssSelector#matchesWithinScope boundary is respected inside :has().
            // The candidate <p> is inside the host <div>, but "body p" requires an ancestor "body".
            // Since ancestor traversal is limited to the :has() scope element, this must be false.
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<body>" + "<div id='host'><p></p></div>" + "</body>");
            IElementNode host = FindElementById(doc, "host");
            CssSelector selector = new CssSelector("div:has(body p)");
            NUnit.Framework.Assert.IsFalse(selector.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasRelativeSelectorDescendantStepWorksAcrossMultipleCurrentScopesTest() {
            // Ensures the descendant step (' ') is applied for EACH current scope when there are multiple
            // scopes after a previous step (e.g. after '> div' there may be multiple div scopes).
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'>" + "<div id='d1'><span><em><p class='target'></p></em></span></div>"
                 + "<div id='d2'><span><p></p></span></div>" + "</section>");
            IElementNode host = FindElementById(doc, "host");
            CssSelector selector = new CssSelector("section:has(> div p.target)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void HasRelativeSelectorDescendantStepReturnsFalseWhenNoDescendantMatchesInAnyScopeTest() {
            // Negative counterpart to the test above.
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'>" + "<div><span><p></p></span></div>" + "<div><span><p></p></span></div>"
                 + "</section>");
            IElementNode host = FindElementById(doc, "host");
            CssSelector selector = new CssSelector("section:has(> div p.target)");
            NUnit.Framework.Assert.IsFalse(selector.Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyHasTest() {
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'>" + "<div id='d1'><span><em><p class='target'></p></em></span></div>"
                 + "<div id='d2'><span><p></p></span></div>" + "</section>");
            IElementNode host = FindElementById(doc, "host");
            NUnit.Framework.Assert.IsFalse(new CssSelector("has()").Matches(host));
            NUnit.Framework.Assert.IsFalse(new CssSelector("has(,)").Matches(host));
        }

        [NUnit.Framework.Test]
        public virtual void TrippleHasTest() {
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode doc = htmlParser.Parse("<section id='host'>" + "<div id='d1' class='c1'><span><em><p class='target'><span class=\"c2\"></span></p></em></span></div>"
                 + "<div id='d2'><span><p></p></span></div>" + "</section>");
            IElementNode host = FindElementById(doc, "host");
            CssSelector selector = new CssSelector(":has(.c1, > .target, > .c2)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
        }

        private static IElementNode FindElementById(INode root, String id) {
            if (root is IElementNode) {
                IElementNode el = (IElementNode)root;
                String attr = el.GetAttribute("id");
                if (id.Equals(attr)) {
                    return el;
                }
            }
            foreach (INode child in root.ChildNodes()) {
                IElementNode found = FindElementById(child, id);
                if (found != null) {
                    return found;
                }
            }
            return null;
        }
    }
}
