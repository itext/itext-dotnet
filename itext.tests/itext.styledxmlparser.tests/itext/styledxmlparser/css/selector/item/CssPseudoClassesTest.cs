using System;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPseudoClassesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void IsMatchesAnySelectorInListTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div><p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector s = new CssSelector(":is(.c, p)");
            NUnit.Framework.Assert.IsTrue(s.Matches(a));
            NUnit.Framework.Assert.IsTrue(s.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void WhereMatchesAnySelectorInListTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div><p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector s = new CssSelector(":where(.c, p)");
            NUnit.Framework.Assert.IsTrue(s.Matches(a));
            NUnit.Framework.Assert.IsTrue(s.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void IsSupportsAttributeSelectorWithCommaInQuotedValueTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div>" + "<span id='s1' title='a,b'></span>" + "<span id='s2' title='a'></span>"
                 + "</div>");
            IElementNode s1 = FindElementById(doc, "s1");
            IElementNode s2 = FindElementById(doc, "s2");
            CssSelector selector = new CssSelector(":is([title='a,b'], #doesNotExist)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(s1));
            NUnit.Framework.Assert.IsFalse(selector.Matches(s2));
        }

        [NUnit.Framework.Test]
        public virtual void IsSupportsNestedFunctionalPseudoClassesTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='root'>" + "<div id='host' class='c'></div>" + "<p id='p1' class='x'></p>"
                 + "<p id='p2'></p>" + "</div>");
            IElementNode host = FindElementById(doc, "host");
            IElementNode p1 = FindElementById(doc, "p1");
            IElementNode p2 = FindElementById(doc, "p2");
            NUnit.Framework.Assert.IsNotNull(host);
            NUnit.Framework.Assert.IsNotNull(p1);
            NUnit.Framework.Assert.IsNotNull(p2);
            CssSelector selector = new CssSelector(":is(:not(.x), p.x)");
            // host has no .x => :not(.x) matches
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
            // p1 has .x => :not(.x) doesn't match, but p.x does
            NUnit.Framework.Assert.IsTrue(selector.Matches(p1));
            // p2 has no .x => :not(.x) matches
            NUnit.Framework.Assert.IsTrue(selector.Matches(p2));
        }

        [NUnit.Framework.Test]
        public virtual void WhereParsesWithExtraWhitespaceAndMatchesCorrectlyTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='host' class='c'></div>" + "<p id='p1'></p>");
            IElementNode host = FindElementById(doc, "host");
            IElementNode p1 = FindElementById(doc, "p1");
            CssSelector selector = new CssSelector(":where(  div.c  ,   #p1 )");
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
            NUnit.Framework.Assert.IsTrue(selector.Matches(p1));
        }

        [NUnit.Framework.Test]
        public virtual void IsCanBeUsedInsideHasArgumentsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='host'>" + "  <span id='s1' title='a,b'></span>" + "</div>" + "<div id='other'>"
                 + "  <em></em>" + "</div>");
            IElementNode host = FindElementById(doc, "host");
            IElementNode other = FindElementById(doc, "other");
            CssSelector selector = new CssSelector("div:has(:is(span[title='a,b'], p))");
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
            NUnit.Framework.Assert.IsFalse(selector.Matches(other));
        }

        [NUnit.Framework.Test]
        public virtual void WhereCanBeUsedInsideHasArgumentsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='host'>" + "  <p class='x'></p>" + "</div>" + "<div id='other'>"
                 + "  <span id='s2'></span>" + "</div>");
            IElementNode host = FindElementById(doc, "host");
            IElementNode other = FindElementById(doc, "other");
            CssSelector selector = new CssSelector("div:has(:where(p.x, span#s2))");
            NUnit.Framework.Assert.IsTrue(selector.Matches(host));
            NUnit.Framework.Assert.IsTrue(selector.Matches(other));
        }

        [NUnit.Framework.Test]
        public virtual void IsCanNestWhereTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>" + "<span id='c'></span>"
                );
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            IElementNode c = FindElementById(doc, "c");
            CssSelector selector = new CssSelector(":is(:where(.c, p), #doesNotExist)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsTrue(selector.Matches(b));
            NUnit.Framework.Assert.IsFalse(selector.Matches(c));
        }

        [NUnit.Framework.Test]
        public virtual void WhereCanNestIsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>" + "<span id='c'></span>"
                );
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            IElementNode c = FindElementById(doc, "c");
            CssSelector selector = new CssSelector(":where(:is(.c, p), #doesNotExist)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsTrue(selector.Matches(b));
            NUnit.Framework.Assert.IsFalse(selector.Matches(c));
        }

        [NUnit.Framework.Test]
        public virtual void IsCanNestWhereAndIsMultipleLevelsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b' class='x'></p>" + "<p id='c'></p>"
                );
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            IElementNode c = FindElementById(doc, "c");
            CssSelector selector = new CssSelector(":is(:where(div.c, :is(p.x, #nope)), #alsoNope)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsTrue(selector.Matches(b));
            NUnit.Framework.Assert.IsFalse(selector.Matches(c));
        }

        [NUnit.Framework.Test]
        public virtual void IsIgnoresRelativeSelectorInArgumentsAndStillMatchesValidOnesTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            // Per Selectors Level 4 forgiving selector list rules, invalid entries are ignored.
            CssSelector selector = new CssSelector(":is(> p, .c)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void WhereIgnoresRelativeSelectorInArgumentsAndStillMatchesValidOnesTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector(":where(+ p, .c)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void IsIgnoresUnsupportedPseudoClassInArgumentsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            // Unknown pseudo-class should not invalidate the whole :is(), it should be ignored.
            CssSelector selector = new CssSelector(":is(:unknownPseudo(.x), .c)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void WhereIgnoresUnsupportedPseudoClassInArgumentsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector(":where(:unknownPseudo(.x), .c)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void IsIgnoresSelectorWithPseudoElementInArgumentsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            // Selectors containing pseudo-elements are not allowed in :is(...), but in a forgiving list
            // they should be ignored rather than invalidating :is(...).
            CssSelector selector = new CssSelector(":is(div::before, .c)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void WhereIgnoresSelectorWithPseudoElementInArgumentsTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector(":where(p::after, .c)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void IsAllInvalidSelectorsInArgumentsMatchesNothingTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            // All entries invalid => selector-list is effectively empty => matches nothing.
            CssSelector selector = new CssSelector(":is(> p, ::before, :unknownPseudo(.x))");
            NUnit.Framework.Assert.IsFalse(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void WhereAllInvalidSelectorsInArgumentsMatchesNothingTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector(":where(+ p, ::after, :unknownPseudo(.x))");
            NUnit.Framework.Assert.IsFalse(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void IsEmptyArgumentsIsInvalidTest() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CssSelector(":is()"));
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CssSelector(":is(   )"));
        }

        [NUnit.Framework.Test]
        public virtual void WhereEmptyArgumentsIsInvalidTest() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CssSelector(":where()"));
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new CssSelector(":where(\n\t )"));
        }

        [NUnit.Framework.Test]
        public virtual void IsSpecificityUsesMaxOfArgumentsTest() {
            // :is(#id, .class) should have ID specificity (highest among arguments)
            CssSelector isWithId = new CssSelector(":is(#myId, .myClass)");
            // ID specificity = 1 << 20 = 1048576
            NUnit.Framework.Assert.AreEqual(1 << 20, isWithId.CalculateSpecificity());
            // :is(.a, .b) should have class specificity
            CssSelector isWithClasses = new CssSelector(":is(.a, .b)");
            // Class specificity = 1 << 10 = 1024
            NUnit.Framework.Assert.AreEqual(1 << 10, isWithClasses.CalculateSpecificity());
            // :is(div, p) should have element specificity
            CssSelector isWithElements = new CssSelector(":is(div, p)");
            // Element specificity = 1
            NUnit.Framework.Assert.AreEqual(1, isWithElements.CalculateSpecificity());
        }

        [NUnit.Framework.Test]
        public virtual void WhereSpecificityIsAlwaysZeroTest() {
            // :where() always contributes 0 specificity regardless of arguments
            CssSelector whereWithId = new CssSelector(":where(#myId, .myClass)");
            NUnit.Framework.Assert.AreEqual(0, whereWithId.CalculateSpecificity());
            CssSelector whereWithClasses = new CssSelector(":where(.a, .b)");
            NUnit.Framework.Assert.AreEqual(0, whereWithClasses.CalculateSpecificity());
        }

        [NUnit.Framework.Test]
        public virtual void CombinedSelectorSpecificityTest() {
            // div:is(.a, .b) should have element (1) + class (1024) = 1025
            CssSelector combined = new CssSelector("div:is(.a, .b)");
            NUnit.Framework.Assert.AreEqual(1 + (1 << 10), combined.CalculateSpecificity());
            // div:where(.a, #id) should have only element (1) since :where() = 0
            CssSelector combinedWhere = new CssSelector("div:where(.a, #id)");
            NUnit.Framework.Assert.AreEqual(1, combinedWhere.CalculateSpecificity());
        }

        [NUnit.Framework.Test]
        public virtual void IsExtraCommaCreatesEmptyEntryWhichIsIgnoredTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector(":is(.c,, #doesNotExist)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
        }

        [NUnit.Framework.Test]
        public virtual void WhereExtraCommaCreatesEmptyEntryWhichIsIgnoredTest() {
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode doc = parser.Parse("<div id='a' class='c'></div>" + "<p id='b'></p>");
            IElementNode a = FindElementById(doc, "a");
            IElementNode b = FindElementById(doc, "b");
            CssSelector selector = new CssSelector(":where(.c,, #doesNotExist)");
            NUnit.Framework.Assert.IsTrue(selector.Matches(a));
            NUnit.Framework.Assert.IsFalse(selector.Matches(b));
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
