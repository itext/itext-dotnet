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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Tests that the selector selects correctly.</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class SelectorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestByTag() {
            // should be case insensitive
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><div id=2><p>Hello</p></div></div><DIV id=3>"
                ).Select("DIV");
            NUnit.Framework.Assert.AreEqual(3, els.Count);
            NUnit.Framework.Assert.AreEqual("1", els[0].Id());
            NUnit.Framework.Assert.AreEqual("2", els[1].Id());
            NUnit.Framework.Assert.AreEqual("3", els[2].Id());
            Elements none = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><div id=2><p>Hello</p></div></div><div id=3>"
                ).Select("span");
            NUnit.Framework.Assert.AreEqual(0, none.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestById() {
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p id=foo>Hello</p><p id=foo>Foo two!</p></div>"
                ).Select("#foo");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("Hello", els[0].Text());
            NUnit.Framework.Assert.AreEqual("Foo two!", els[1].Text());
            Elements none = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1></div>").Select("#foo");
            NUnit.Framework.Assert.AreEqual(0, none.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestByClass() {
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=0 class='ONE two'><p id=1 class='one'><p id=2 class='two'>"
                ).Select("P.One");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("0", els[0].Id());
            NUnit.Framework.Assert.AreEqual("1", els[1].Id());
            Elements none = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class='one'></div>").Select(".foo");
            NUnit.Framework.Assert.AreEqual(0, none.Count);
            Elements els2 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class='One-Two'></div>").Select(".one-two");
            NUnit.Framework.Assert.AreEqual(1, els2.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestByClassCaseInsensitive() {
            String html = "<p Class=foo>One <p Class=Foo>Two <p class=FOO>Three <p class=farp>Four";
            Elements elsFromClass = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select("P.Foo");
            Elements elsFromAttr = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select("p[class=foo]");
            NUnit.Framework.Assert.AreEqual(elsFromAttr.Count, elsFromClass.Count);
            NUnit.Framework.Assert.AreEqual(3, elsFromClass.Count);
            NUnit.Framework.Assert.AreEqual("Two", elsFromClass[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestNamespacedTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><abc:def id=1>Hello</abc:def></div> <abc:def class=bold id=2>There</abc:def>"
                );
            Elements byTag = doc.Select("abc|def");
            NUnit.Framework.Assert.AreEqual(2, byTag.Count);
            NUnit.Framework.Assert.AreEqual("1", byTag.First().Id());
            NUnit.Framework.Assert.AreEqual("2", byTag.Last().Id());
            Elements byAttr = doc.Select(".bold");
            NUnit.Framework.Assert.AreEqual(1, byAttr.Count);
            NUnit.Framework.Assert.AreEqual("2", byAttr.Last().Id());
            Elements byTagAttr = doc.Select("abc|def.bold");
            NUnit.Framework.Assert.AreEqual(1, byTagAttr.Count);
            NUnit.Framework.Assert.AreEqual("2", byTagAttr.Last().Id());
            Elements byContains = doc.Select("abc|def:contains(e)");
            NUnit.Framework.Assert.AreEqual(2, byContains.Count);
            NUnit.Framework.Assert.AreEqual("1", byContains.First().Id());
            NUnit.Framework.Assert.AreEqual("2", byContains.Last().Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestWildcardNamespacedTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><abc:def id=1>Hello</abc:def></div> <abc:def class=bold id=2>There</abc:def>"
                );
            Elements byTag = doc.Select("*|def");
            NUnit.Framework.Assert.AreEqual(2, byTag.Count);
            NUnit.Framework.Assert.AreEqual("1", byTag.First().Id());
            NUnit.Framework.Assert.AreEqual("2", byTag.Last().Id());
            Elements byAttr = doc.Select(".bold");
            NUnit.Framework.Assert.AreEqual(1, byAttr.Count);
            NUnit.Framework.Assert.AreEqual("2", byAttr.Last().Id());
            Elements byTagAttr = doc.Select("*|def.bold");
            NUnit.Framework.Assert.AreEqual(1, byTagAttr.Count);
            NUnit.Framework.Assert.AreEqual("2", byTagAttr.Last().Id());
            Elements byContains = doc.Select("*|def:contains(e)");
            NUnit.Framework.Assert.AreEqual(2, byContains.Count);
            NUnit.Framework.Assert.AreEqual("1", byContains.First().Id());
            NUnit.Framework.Assert.AreEqual("2", byContains.Last().Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestWildcardNamespacedXmlTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><Abc:Def id=1>Hello</Abc:Def></div> <Abc:Def class=bold id=2>There</abc:def>"
                , "", iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
            Elements byTag = doc.Select("*|Def");
            NUnit.Framework.Assert.AreEqual(2, byTag.Count);
            NUnit.Framework.Assert.AreEqual("1", byTag.First().Id());
            NUnit.Framework.Assert.AreEqual("2", byTag.Last().Id());
            Elements byAttr = doc.Select(".bold");
            NUnit.Framework.Assert.AreEqual(1, byAttr.Count);
            NUnit.Framework.Assert.AreEqual("2", byAttr.Last().Id());
            Elements byTagAttr = doc.Select("*|Def.bold");
            NUnit.Framework.Assert.AreEqual(1, byTagAttr.Count);
            NUnit.Framework.Assert.AreEqual("2", byTagAttr.Last().Id());
            Elements byContains = doc.Select("*|Def:contains(e)");
            NUnit.Framework.Assert.AreEqual(2, byContains.Count);
            NUnit.Framework.Assert.AreEqual("1", byContains.First().Id());
            NUnit.Framework.Assert.AreEqual("2", byContains.Last().Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestWildCardNamespacedCaseVariations() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<One:Two>One</One:Two><three:four>Two</three:four>"
                , "", iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
            Elements els1 = doc.Select("One|Two");
            Elements els2 = doc.Select("one|two");
            Elements els3 = doc.Select("Three|Four");
            Elements els4 = doc.Select("three|Four");
            NUnit.Framework.Assert.AreEqual(els1, els2);
            NUnit.Framework.Assert.AreEqual(els3, els4);
            NUnit.Framework.Assert.AreEqual("One", els1.Text());
            NUnit.Framework.Assert.AreEqual(1, els1.Count);
            NUnit.Framework.Assert.AreEqual("Two", els3.Text());
            NUnit.Framework.Assert.AreEqual(1, els2.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestByAttributeRegex() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p><img src=foo.png id=1><img src=bar.jpg id=2><img src=qux.JPEG id=3><img src=old.gif><img></p>"
                );
            Elements imgs = doc.Select("img[src~=(?i)\\.(png|jpe?g)]");
            NUnit.Framework.Assert.AreEqual(3, imgs.Count);
            NUnit.Framework.Assert.AreEqual("1", imgs[0].Id());
            NUnit.Framework.Assert.AreEqual("2", imgs[1].Id());
            NUnit.Framework.Assert.AreEqual("3", imgs[2].Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestByAttributeRegexCharacterClass() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p><img src=foo.png id=1><img src=bar.jpg id=2><img src=qux.JPEG id=3><img src=old.gif id=4></p>"
                );
            Elements imgs = doc.Select("img[src~=[o]]");
            NUnit.Framework.Assert.AreEqual(2, imgs.Count);
            NUnit.Framework.Assert.AreEqual("1", imgs[0].Id());
            NUnit.Framework.Assert.AreEqual("4", imgs[1].Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestByAttributeRegexCombined() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><table class=x><td>Hello</td></table></div>");
            Elements els = doc.Select("div table[class~=x|y]");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Hello", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestCombinedWithContains() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=1>One</p><p>Two +</p><p>Three +</p>");
            Elements els = doc.Select("p#1 + :contains(+)");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Two +", els.Text());
            NUnit.Framework.Assert.AreEqual("p", els.First().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestAllElements() {
            String h = "<div><p>Hello</p><p><b>there</b></p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements allDoc = doc.Select("*");
            Elements allUnderDiv = doc.Select("div *");
            NUnit.Framework.Assert.AreEqual(8, allDoc.Count);
            NUnit.Framework.Assert.AreEqual(3, allUnderDiv.Count);
            NUnit.Framework.Assert.AreEqual("p", allUnderDiv.First().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestAllWithClass() {
            String h = "<p class=first>One<p class=first>Two<p>Three";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements ps = doc.Select("*.first");
            NUnit.Framework.Assert.AreEqual(2, ps.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGroupOr() {
            String h = "<div title=foo /><div title=bar /><div /><p></p><img /><span title=qux>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements els = doc.Select("p,div,[title]");
            NUnit.Framework.Assert.AreEqual(5, els.Count);
            NUnit.Framework.Assert.AreEqual("div", els[0].TagName());
            NUnit.Framework.Assert.AreEqual("foo", els[0].Attr("title"));
            NUnit.Framework.Assert.AreEqual("div", els[1].TagName());
            NUnit.Framework.Assert.AreEqual("bar", els[1].Attr("title"));
            NUnit.Framework.Assert.AreEqual("div", els[2].TagName());
            NUnit.Framework.Assert.AreEqual(0, els[2].Attr("title").Length);
            // missing attributes come back as empty string
            NUnit.Framework.Assert.IsFalse(els[2].HasAttr("title"));
            NUnit.Framework.Assert.AreEqual("p", els[3].TagName());
            NUnit.Framework.Assert.AreEqual("span", els[4].TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestGroupOrAttribute() {
            String h = "<div id=1 /><div id=2 /><div title=foo /><div title=bar />";
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h).Select("[id],[title=foo]");
            NUnit.Framework.Assert.AreEqual(3, els.Count);
            NUnit.Framework.Assert.AreEqual("1", els[0].Id());
            NUnit.Framework.Assert.AreEqual("2", els[1].Id());
            NUnit.Framework.Assert.AreEqual("foo", els[2].Attr("title"));
        }

        [NUnit.Framework.Test]
        public virtual void Descendant() {
            String h = "<div class=head><p class=first>Hello</p><p>There</p></div><p>None</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element root = doc.GetElementsByClass("HEAD").First();
            Elements els = root.Select(".head p");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("Hello", els[0].Text());
            NUnit.Framework.Assert.AreEqual("There", els[1].Text());
            Elements p = root.Select("p.first");
            NUnit.Framework.Assert.AreEqual(1, p.Count);
            NUnit.Framework.Assert.AreEqual("Hello", p[0].Text());
            Elements empty = root.Select("p .first");
            // self, not descend, should not match
            NUnit.Framework.Assert.AreEqual(0, empty.Count);
            Elements aboveRoot = root.Select("body div.head");
            NUnit.Framework.Assert.AreEqual(0, aboveRoot.Count);
        }

        [NUnit.Framework.Test]
        public virtual void And() {
            String h = "<div id=1 class='foo bar' title=bar name=qux><p class=foo title=bar>Hello</p></div";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements div = doc.Select("div.foo");
            NUnit.Framework.Assert.AreEqual(1, div.Count);
            NUnit.Framework.Assert.AreEqual("div", div.First().TagName());
            Elements p = doc.Select("div .foo");
            // space indicates like "div *.foo"
            NUnit.Framework.Assert.AreEqual(1, p.Count);
            NUnit.Framework.Assert.AreEqual("p", p.First().TagName());
            Elements div2 = doc.Select("div#1.foo.bar[title=bar][name=qux]");
            // very specific!
            NUnit.Framework.Assert.AreEqual(1, div2.Count);
            NUnit.Framework.Assert.AreEqual("div", div2.First().TagName());
            Elements p2 = doc.Select("div *.foo");
            // space indicates like "div *.foo"
            NUnit.Framework.Assert.AreEqual(1, p2.Count);
            NUnit.Framework.Assert.AreEqual("p", p2.First().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void DeeperDescendant() {
            String h = "<div class=head><p><span class=first>Hello</div><div class=head><p class=first><span>Another</span><p>Again</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element root = doc.GetElementsByClass("head").First();
            Elements els = root.Select("div p .first");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Hello", els.First().Text());
            NUnit.Framework.Assert.AreEqual("span", els.First().TagName());
            Elements aboveRoot = root.Select("body p .first");
            NUnit.Framework.Assert.AreEqual(0, aboveRoot.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildElement() {
            String h = "<div id=1><div id=2><div id = 3></div></div></div><div id=4></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements divs = doc.Select("div > div");
            NUnit.Framework.Assert.AreEqual(2, divs.Count);
            NUnit.Framework.Assert.AreEqual("2", divs[0].Id());
            // 2 is child of 1
            NUnit.Framework.Assert.AreEqual("3", divs[1].Id());
            // 3 is child of 2
            Elements div2 = doc.Select("div#1 > div");
            NUnit.Framework.Assert.AreEqual(1, div2.Count);
            NUnit.Framework.Assert.AreEqual("2", div2[0].Id());
        }

        [NUnit.Framework.Test]
        public virtual void ParentWithClassChild() {
            String h = "<h1 class=foo><a href=1 /></h1><h1 class=foo><a href=2 class=bar /></h1><h1><a href=3 /></h1>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements allAs = doc.Select("h1 > a");
            NUnit.Framework.Assert.AreEqual(3, allAs.Count);
            NUnit.Framework.Assert.AreEqual("a", allAs.First().TagName());
            Elements fooAs = doc.Select("h1.foo > a");
            NUnit.Framework.Assert.AreEqual(2, fooAs.Count);
            NUnit.Framework.Assert.AreEqual("a", fooAs.First().TagName());
            Elements barAs = doc.Select("h1.foo > a.bar");
            NUnit.Framework.Assert.AreEqual(1, barAs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ParentChildStar() {
            String h = "<div id=1><p>Hello<p><b>there</b></p></div><div id=2><span>Hi</span></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements divChilds = doc.Select("div > *");
            NUnit.Framework.Assert.AreEqual(3, divChilds.Count);
            NUnit.Framework.Assert.AreEqual("p", divChilds[0].TagName());
            NUnit.Framework.Assert.AreEqual("p", divChilds[1].TagName());
            NUnit.Framework.Assert.AreEqual("span", divChilds[2].TagName());
        }

        [NUnit.Framework.Test]
        public virtual void MultiChildDescent() {
            String h = "<div id=foo><h1 class=bar><a href=http://example.com/>One</a></h1></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements els = doc.Select("div#foo > h1.bar > a[href*=example]");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("a", els.First().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void CaseInsensitive() {
            String h = "<dIv tItle=bAr><div>";
            // mixed case so a simple toLowerCase() on value doesn't catch
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual(2, doc.Select("DiV").Count);
            NUnit.Framework.Assert.AreEqual(1, doc.Select("DiV[TiTLE]").Count);
            NUnit.Framework.Assert.AreEqual(1, doc.Select("DiV[TiTLE=BAR]").Count);
            NUnit.Framework.Assert.AreEqual(0, doc.Select("DiV[TiTLE=BARBARELLA]").Count);
        }

        [NUnit.Framework.Test]
        public virtual void AdjacentSiblings() {
            String h = "<ol><li>One<li>Two<li>Three</ol>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements sibs = doc.Select("li + li");
            NUnit.Framework.Assert.AreEqual(2, sibs.Count);
            NUnit.Framework.Assert.AreEqual("Two", sibs[0].Text());
            NUnit.Framework.Assert.AreEqual("Three", sibs[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void AdjacentSiblingsWithId() {
            String h = "<ol><li id=1>One<li id=2>Two<li id=3>Three</ol>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements sibs = doc.Select("li#1 + li#2");
            NUnit.Framework.Assert.AreEqual(1, sibs.Count);
            NUnit.Framework.Assert.AreEqual("Two", sibs[0].Text());
        }

        [NUnit.Framework.Test]
        public virtual void NotAdjacent() {
            String h = "<ol><li id=1>One<li id=2>Two<li id=3>Three</ol>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements sibs = doc.Select("li#1 + li#3");
            NUnit.Framework.Assert.AreEqual(0, sibs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void MixCombinator() {
            String h = "<div class=foo><ol><li>One<li>Two<li>Three</ol></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements sibs = doc.Select("body > div.foo li + li");
            NUnit.Framework.Assert.AreEqual(2, sibs.Count);
            NUnit.Framework.Assert.AreEqual("Two", sibs[0].Text());
            NUnit.Framework.Assert.AreEqual("Three", sibs[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void MixCombinatorGroup() {
            String h = "<div class=foo><ol><li>One<li>Two<li>Three</ol></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements els = doc.Select(".foo > ol, ol > li + li");
            NUnit.Framework.Assert.AreEqual(3, els.Count);
            NUnit.Framework.Assert.AreEqual("ol", els[0].TagName());
            NUnit.Framework.Assert.AreEqual("Two", els[1].Text());
            NUnit.Framework.Assert.AreEqual("Three", els[2].Text());
        }

        [NUnit.Framework.Test]
        public virtual void GeneralSiblings() {
            String h = "<ol><li id=1>One<li id=2>Two<li id=3>Three</ol>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements els = doc.Select("#1 ~ #3");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Three", els.First().Text());
        }

        // for http://github.com/jhy/jsoup/issues#issue/10
        [NUnit.Framework.Test]
        public virtual void TestCharactersInIdAndClass() {
            // using CSS spec for identifiers (id and class): a-z0-9, -, _. NOT . (which is OK in html spec, but not css)
            String h = "<div><p id='a1-foo_bar'>One</p><p class='b2-qux_bif'>Two</p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element el1 = doc.GetElementById("a1-foo_bar");
            NUnit.Framework.Assert.AreEqual("One", el1.Text());
            iText.StyledXmlParser.Jsoup.Nodes.Element el2 = doc.GetElementsByClass("b2-qux_bif").First();
            NUnit.Framework.Assert.AreEqual("Two", el2.Text());
            iText.StyledXmlParser.Jsoup.Nodes.Element el3 = doc.Select("#a1-foo_bar").First();
            NUnit.Framework.Assert.AreEqual("One", el3.Text());
            iText.StyledXmlParser.Jsoup.Nodes.Element el4 = doc.Select(".b2-qux_bif").First();
            NUnit.Framework.Assert.AreEqual("Two", el4.Text());
        }

        // for http://github.com/jhy/jsoup/issues#issue/13
        [NUnit.Framework.Test]
        public virtual void TestSupportsLeadingCombinator() {
            String h = "<div><p><span>One</span><span>Two</span></p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("div > p").First();
            Elements spans = p.Select("> span");
            NUnit.Framework.Assert.AreEqual(2, spans.Count);
            NUnit.Framework.Assert.AreEqual("One", spans.First().Text());
            // make sure doesn't get nested
            h = "<div id=1><div id=2><div id=3></div></div></div>";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").Select(" > div").First();
            NUnit.Framework.Assert.AreEqual("2", div.Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestPseudoLessThan() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p><p>Two</p><p>Three</>p></div><div><p>Four</p>"
                );
            Elements ps = doc.Select("div p:lt(2)");
            NUnit.Framework.Assert.AreEqual(3, ps.Count);
            NUnit.Framework.Assert.AreEqual("One", ps[0].Text());
            NUnit.Framework.Assert.AreEqual("Two", ps[1].Text());
            NUnit.Framework.Assert.AreEqual("Four", ps[2].Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestPseudoGreaterThan() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p><p>Two</p><p>Three</p></div><div><p>Four</p>"
                );
            Elements ps = doc.Select("div p:gt(0)");
            NUnit.Framework.Assert.AreEqual(2, ps.Count);
            NUnit.Framework.Assert.AreEqual("Two", ps[0].Text());
            NUnit.Framework.Assert.AreEqual("Three", ps[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestPseudoEquals() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p><p>Two</p><p>Three</>p></div><div><p>Four</p>"
                );
            Elements ps = doc.Select("div p:eq(0)");
            NUnit.Framework.Assert.AreEqual(2, ps.Count);
            NUnit.Framework.Assert.AreEqual("One", ps[0].Text());
            NUnit.Framework.Assert.AreEqual("Four", ps[1].Text());
            Elements ps2 = doc.Select("div:eq(0) p:eq(0)");
            NUnit.Framework.Assert.AreEqual(1, ps2.Count);
            NUnit.Framework.Assert.AreEqual("One", ps2[0].Text());
            NUnit.Framework.Assert.AreEqual("p", ps2[0].TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestPseudoBetween() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p><p>Two</p><p>Three</>p></div><div><p>Four</p>"
                );
            Elements ps = doc.Select("div p:gt(0):lt(2)");
            NUnit.Framework.Assert.AreEqual(1, ps.Count);
            NUnit.Framework.Assert.AreEqual("Two", ps[0].Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestPseudoCombined() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class='foo'><p>One</p><p>Two</p></div><div><p>Three</p><p>Four</p></div>"
                );
            Elements ps = doc.Select("div.foo p:gt(0)");
            NUnit.Framework.Assert.AreEqual(1, ps.Count);
            NUnit.Framework.Assert.AreEqual("Two", ps[0].Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestPseudoHas() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=0><p><span>Hello</span></p></div> <div id=1><span class=foo>There</span></div> <div id=2><p>Not</p></div>"
                );
            Elements divs1 = doc.Select("div:has(span)");
            NUnit.Framework.Assert.AreEqual(2, divs1.Count);
            NUnit.Framework.Assert.AreEqual("0", divs1[0].Id());
            NUnit.Framework.Assert.AreEqual("1", divs1[1].Id());
            Elements divs2 = doc.Select("div:has([class])");
            NUnit.Framework.Assert.AreEqual(1, divs2.Count);
            NUnit.Framework.Assert.AreEqual("1", divs2[0].Id());
            Elements divs3 = doc.Select("div:has(span, p)");
            NUnit.Framework.Assert.AreEqual(3, divs3.Count);
            NUnit.Framework.Assert.AreEqual("0", divs3[0].Id());
            NUnit.Framework.Assert.AreEqual("1", divs3[1].Id());
            NUnit.Framework.Assert.AreEqual("2", divs3[2].Id());
            Elements els1 = doc.Body().Select(":has(p)");
            NUnit.Framework.Assert.AreEqual(3, els1.Count);
            // body, div, dib
            NUnit.Framework.Assert.AreEqual("body", els1.First().TagName());
            NUnit.Framework.Assert.AreEqual("0", els1[1].Id());
            NUnit.Framework.Assert.AreEqual("2", els1[2].Id());
            Elements els2 = doc.Body().Select(":has(> span)");
            NUnit.Framework.Assert.AreEqual(2, els2.Count);
            // p, div
            NUnit.Framework.Assert.AreEqual("p", els2.First().TagName());
            NUnit.Framework.Assert.AreEqual("1", els2[1].Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestNestedHas() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p><span>One</span></p></div> <div><p>Two</p></div>"
                );
            Elements divs = doc.Select("div:has(p:has(span))");
            NUnit.Framework.Assert.AreEqual(1, divs.Count);
            NUnit.Framework.Assert.AreEqual("One", divs.First().Text());
            // test matches in has
            divs = doc.Select("div:has(p:matches((?i)two))");
            NUnit.Framework.Assert.AreEqual(1, divs.Count);
            NUnit.Framework.Assert.AreEqual("div", divs.First().TagName());
            NUnit.Framework.Assert.AreEqual("Two", divs.First().Text());
            // test contains in has
            divs = doc.Select("div:has(p:contains(two))");
            NUnit.Framework.Assert.AreEqual(1, divs.Count);
            NUnit.Framework.Assert.AreEqual("div", divs.First().TagName());
            NUnit.Framework.Assert.AreEqual("Two", divs.First().Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestPsuedoContainsWithParentheses() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p id=1>This (is good)</p><p id=2>This is bad)</p>"
                );
            Elements ps1 = doc.Select("p:contains(this (is good))");
            NUnit.Framework.Assert.AreEqual(1, ps1.Count);
            NUnit.Framework.Assert.AreEqual("1", ps1.First().Id());
            Elements ps2 = doc.Select("p:contains(this is bad\\))");
            NUnit.Framework.Assert.AreEqual(1, ps2.Count);
            NUnit.Framework.Assert.AreEqual("2", ps2.First().Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestMatches() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=1>The <i>Rain</i></p> <p id=2>There are 99 bottles.</p> <p id=3>Harder (this)</p> <p id=4>Rain</p>"
                );
            Elements p1 = doc.Select("p:matches(The rain)");
            // no match, case sensitive
            NUnit.Framework.Assert.AreEqual(0, p1.Count);
            Elements p2 = doc.Select("p:matches((?i)the rain)");
            // case insense. should include root, html, body
            NUnit.Framework.Assert.AreEqual(1, p2.Count);
            NUnit.Framework.Assert.AreEqual("1", p2.First().Id());
            Elements p4 = doc.Select("p:matches((?i)^rain$)");
            // bounding
            NUnit.Framework.Assert.AreEqual(1, p4.Count);
            NUnit.Framework.Assert.AreEqual("4", p4.First().Id());
            Elements p5 = doc.Select("p:matches(\\d+)");
            NUnit.Framework.Assert.AreEqual(1, p5.Count);
            NUnit.Framework.Assert.AreEqual("2", p5.First().Id());
            Elements p6 = doc.Select("p:matches(\\w+\\s+\\(\\w+\\))");
            // test bracket matching
            NUnit.Framework.Assert.AreEqual(1, p6.Count);
            NUnit.Framework.Assert.AreEqual("3", p6.First().Id());
            Elements p7 = doc.Select("p:matches((?i)the):has(i)");
            // multi
            NUnit.Framework.Assert.AreEqual(1, p7.Count);
            NUnit.Framework.Assert.AreEqual("1", p7.First().Id());
        }

        [NUnit.Framework.Test]
        public virtual void MatchesOwn() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=1>Hello <b>there</b> now</p>");
            Elements p1 = doc.Select("p:matchesOwn((?i)hello now)");
            NUnit.Framework.Assert.AreEqual(1, p1.Count);
            NUnit.Framework.Assert.AreEqual("1", p1.First().Id());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("p:matchesOwn(there)").Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestRelaxedTags() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<abc_def id=1>Hello</abc_def> <abc-def id=2>There</abc-def>"
                );
            Elements el1 = doc.Select("abc_def");
            NUnit.Framework.Assert.AreEqual(1, el1.Count);
            NUnit.Framework.Assert.AreEqual("1", el1.First().Id());
            Elements el2 = doc.Select("abc-def");
            NUnit.Framework.Assert.AreEqual(1, el2.Count);
            NUnit.Framework.Assert.AreEqual("2", el2.First().Id());
        }

        [NUnit.Framework.Test]
        public virtual void NotParas() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=1>One</p> <p>Two</p> <p><span>Three</span></p>"
                );
            Elements el1 = doc.Select("p:not([id=1])");
            NUnit.Framework.Assert.AreEqual(2, el1.Count);
            NUnit.Framework.Assert.AreEqual("Two", el1.First().Text());
            NUnit.Framework.Assert.AreEqual("Three", el1.Last().Text());
            Elements el2 = doc.Select("p:not(:has(span))");
            NUnit.Framework.Assert.AreEqual(2, el2.Count);
            NUnit.Framework.Assert.AreEqual("One", el2.First().Text());
            NUnit.Framework.Assert.AreEqual("Two", el2.Last().Text());
        }

        [NUnit.Framework.Test]
        public virtual void NotAll() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Two</p> <p><span>Three</span></p>");
            Elements el1 = doc.Body().Select(":not(p)");
            // should just be the span
            NUnit.Framework.Assert.AreEqual(2, el1.Count);
            NUnit.Framework.Assert.AreEqual("body", el1.First().TagName());
            NUnit.Framework.Assert.AreEqual("span", el1.Last().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void NotClass() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class=left>One</div><div class=right id=1><p>Two</p></div>"
                );
            Elements el1 = doc.Select("div:not(.left)");
            NUnit.Framework.Assert.AreEqual(1, el1.Count);
            NUnit.Framework.Assert.AreEqual("1", el1.First().Id());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesCommasInSelector() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p name='1,2'>One</p><div>Two</div><ol><li>123</li><li>Text</li></ol>"
                );
            Elements ps = doc.Select("[name=1,2]");
            NUnit.Framework.Assert.AreEqual(1, ps.Count);
            Elements containers = doc.Select("div, li:matches([0-9,]+)");
            NUnit.Framework.Assert.AreEqual(2, containers.Count);
            NUnit.Framework.Assert.AreEqual("div", containers[0].TagName());
            NUnit.Framework.Assert.AreEqual("li", containers[1].TagName());
            NUnit.Framework.Assert.AreEqual("123", containers[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void SelectSupplementaryCharacter() {
            String s = new String(iText.IO.Util.TextUtil.ToChars(135361));
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div k" + s + "='" + s + "'>^" + s + "$/div>");
            NUnit.Framework.Assert.AreEqual("div", doc.Select("div[k" + s + "]").First().TagName());
            NUnit.Framework.Assert.AreEqual("div", doc.Select("div:containsOwn(" + s + ")").First().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void SelectClassWithSpace() {
            String html = "<div class=\"value\">class without space</div>\n" + "<div class=\"value \">class with space</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements found = doc.Select("div[class=value ]");
            NUnit.Framework.Assert.AreEqual(2, found.Count);
            NUnit.Framework.Assert.AreEqual("class without space", found[0].Text());
            NUnit.Framework.Assert.AreEqual("class with space", found[1].Text());
            found = doc.Select("div[class=\"value \"]");
            NUnit.Framework.Assert.AreEqual(2, found.Count);
            NUnit.Framework.Assert.AreEqual("class without space", found[0].Text());
            NUnit.Framework.Assert.AreEqual("class with space", found[1].Text());
            found = doc.Select("div[class=\"value\\ \"]");
            NUnit.Framework.Assert.AreEqual(0, found.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SelectSameElements() {
            String html = "<div>one</div><div>one</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("div");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            Elements subSelect = els.Select(":contains(one)");
            NUnit.Framework.Assert.AreEqual(2, subSelect.Count);
        }

        [NUnit.Framework.Test]
        public virtual void AttributeWithBrackets() {
            String html = "<div data='End]'>One</div> <div data='[Another)]]'>Two</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("One", doc.Select("div[data='End]']").First().Text());
            NUnit.Framework.Assert.AreEqual("Two", doc.Select("div[data='[Another)]]']").First().Text());
            NUnit.Framework.Assert.AreEqual("One", doc.Select("div[data=\"End]\"]").First().Text());
            NUnit.Framework.Assert.AreEqual("Two", doc.Select("div[data=\"[Another)]]\"]").First().Text());
        }

        [NUnit.Framework.Test]
        public virtual void ContainsWithQuote() {
            String html = "<p>One'One</p><p>One'Two</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("p:contains(One\\'One)");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("One'One", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void SelectFirst() {
            String html = "<p>One<p>Two<p>Three";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("One", doc.SelectFirst("p").Text());
        }

        [NUnit.Framework.Test]
        public virtual void SelectFirstWithAnd() {
            String html = "<p>One<p class=foo>Two<p>Three";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("Two", doc.SelectFirst("p.foo").Text());
        }

        [NUnit.Framework.Test]
        public virtual void SelectFirstWithOr() {
            String html = "<p>One<p>Two<p>Three<div>Four";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("One", doc.SelectFirst("p, div").Text());
        }

        [NUnit.Framework.Test]
        public virtual void MatchText() {
            String html = "<p>One<br>Two</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            String origHtml = doc.Html();
            Elements one = doc.Select("p:matchText:first-child");
            NUnit.Framework.Assert.AreEqual("One", one.First().Text());
            Elements two = doc.Select("p:matchText:last-child");
            NUnit.Framework.Assert.AreEqual("Two", two.First().Text());
            NUnit.Framework.Assert.AreEqual(origHtml, doc.Html());
            NUnit.Framework.Assert.AreEqual("Two", doc.Select("p:matchText + br + *").Text());
        }

        [NUnit.Framework.Test]
        public virtual void NthLastChildWithNoParent() {
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("Orphan"
                );
            Elements els = el.Select("p:nth-last-child(1)");
            NUnit.Framework.Assert.AreEqual(0, els.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SplitOnBr() {
            String html = "<div><p>One<br>Two<br>Three</p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("p:matchText");
            NUnit.Framework.Assert.AreEqual(3, els.Count);
            NUnit.Framework.Assert.AreEqual("One", els[0].Text());
            NUnit.Framework.Assert.AreEqual("Two", els[1].Text());
            NUnit.Framework.Assert.AreEqual("Three", els[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void MatchTextAttributes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p class=one>One<br>Two<p class=two>Three<br>Four"
                );
            Elements els = doc.Select("p.two:matchText:last-child");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Four", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void FindBetweenSpan() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p><span>One</span> Two <span>Three</span>");
            Elements els = doc.Select("span ~ p:matchText");
            // the Two becomes its own p, sibling of the span
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Two", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void StartsWithBeginsWithSpace() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<small><a href=\" mailto:abc@def.net\">(abc@def.net)</a></small>"
                );
            Elements els = doc.Select("a[href^=' mailto']");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
        }

        [NUnit.Framework.Test]
        public virtual void EndsWithEndsWithSpaces() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<small><a href=\" mailto:abc@def.net \">(abc@def.net)</a></small>"
                );
            Elements els = doc.Select("a[href$='.net ']");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
        }

        // https://github.com/jhy/jsoup/issues/1257
        private readonly String mixedCase = "<html xmlns:n=\"urn:ns\"><n:mixedCase>text</n:mixedCase></html>";

        private readonly String lowercase = "<html xmlns:n=\"urn:ns\"><n:lowercase>text</n:lowercase></html>";

        [NUnit.Framework.Test]
        public virtual void Html_mixed_case_simple_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(mixedCase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("mixedCase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Html_mixed_case_wildcard_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(mixedCase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual(1, doc.Select("*|mixedCase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Html_lowercase_simple_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(lowercase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("lowercase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Html_lowercase_wildcard_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(lowercase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual(1, doc.Select("*|lowercase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Xml_mixed_case_simple_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(mixedCase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("mixedCase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Xml_mixed_case_wildcard_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(mixedCase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual(1, doc.Select("*|mixedCase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Xml_lowercase_simple_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(lowercase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("lowercase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void Xml_lowercase_wildcard_name() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(lowercase, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual(1, doc.Select("*|lowercase").Count);
        }

        [NUnit.Framework.Test]
        public virtual void TrimSelector() {
            // https://github.com/jhy/jsoup/issues/1274
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p><span>Hello");
            Elements els = doc.Select(" p span ");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Hello", els.First().Text());
        }

        [NUnit.Framework.Test]
        public virtual void XmlWildcardNamespaceTest() {
            // https://github.com/jhy/jsoup/issues/1208
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<ns1:MyXmlTag>1111</ns1:MyXmlTag><ns2:MyXmlTag>2222</ns2:MyXmlTag>"
                , "", iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
            Elements select = doc.Select("*|MyXmlTag");
            NUnit.Framework.Assert.AreEqual(2, select.Count);
            NUnit.Framework.Assert.AreEqual("1111", select[0].Text());
            NUnit.Framework.Assert.AreEqual("2222", select[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void ChildElements() {
            // https://github.com/jhy/jsoup/issues/1292
            String html = "<body><span id=1>One <span id=2>Two</span></span></body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element outer = doc.SelectFirst("span");
            iText.StyledXmlParser.Jsoup.Nodes.Element span = outer.SelectFirst("span");
            iText.StyledXmlParser.Jsoup.Nodes.Element inner = outer.SelectFirst("* span");
            NUnit.Framework.Assert.AreEqual("1", outer.Id());
            NUnit.Framework.Assert.AreEqual("1", span.Id());
            NUnit.Framework.Assert.AreEqual("2", inner.Id());
            NUnit.Framework.Assert.AreEqual(outer, span);
            NUnit.Framework.Assert.AreNotEqual(outer, inner);
        }

        [NUnit.Framework.Test]
        public virtual void SelectFirstLevelChildrenOnly() {
            // testcase for https://github.com/jhy/jsoup/issues/984
            String html = "<div><span>One <span>Two</span></span> <span>Three <span>Four</span></span>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            NUnit.Framework.Assert.IsNotNull(div);
            // want to select One and Three only - the first level children
            Elements spans = div.Select(":root > span");
            NUnit.Framework.Assert.AreEqual(2, spans.Count);
            NUnit.Framework.Assert.AreEqual("One Two", spans[0].Text());
            NUnit.Framework.Assert.AreEqual("Three Four", spans[1].Text());
        }
    }
}
