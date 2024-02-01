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
using System.Text;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Tests for ElementList.</summary>
    public class ElementsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Filter() {
            String h = "<p>Excl</p><div class=headline><p>Hello</p><p>There</p></div><div class=headline><h1>Headline</h1></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements els = doc.Select(".headline").Select("p");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("Hello", els[0].Text());
            NUnit.Framework.Assert.AreEqual("There", els[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void Attributes() {
            String h = "<p title=foo><p title=bar><p class=foo><p class=bar>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements withTitle = doc.Select("p[title]");
            NUnit.Framework.Assert.AreEqual(2, withTitle.Count);
            NUnit.Framework.Assert.IsTrue(withTitle.HasAttr("title"));
            NUnit.Framework.Assert.IsFalse(withTitle.HasAttr("class"));
            NUnit.Framework.Assert.AreEqual("foo", withTitle.Attr("title"));
            withTitle.RemoveAttr("title");
            NUnit.Framework.Assert.AreEqual(2, withTitle.Count);
            // existing Elements are not reevaluated
            NUnit.Framework.Assert.AreEqual(0, doc.Select("p[title]").Count);
            Elements ps = doc.Select("p").Attr("style", "classy");
            NUnit.Framework.Assert.AreEqual(4, ps.Count);
            NUnit.Framework.Assert.AreEqual("classy", ps.Last().Attr("style"));
            NUnit.Framework.Assert.AreEqual("bar", ps.Last().Attr("class"));
        }

        [NUnit.Framework.Test]
        public virtual void HasAttr() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p title=foo><p title=bar><p class=foo><p class=bar>"
                );
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.IsTrue(ps.HasAttr("class"));
            NUnit.Framework.Assert.IsFalse(ps.HasAttr("style"));
        }

        [NUnit.Framework.Test]
        public virtual void HasAbsAttr() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a id=1 href='/foo'>One</a> <a id=2 href='https://jsoup.org'>Two</a>"
                );
            Elements one = doc.Select("#1");
            Elements two = doc.Select("#2");
            Elements both = doc.Select("a");
            NUnit.Framework.Assert.IsFalse(one.HasAttr("abs:href"));
            NUnit.Framework.Assert.IsTrue(two.HasAttr("abs:href"));
            NUnit.Framework.Assert.IsTrue(both.HasAttr("abs:href"));
        }

        // hits on #2
        [NUnit.Framework.Test]
        public virtual void Attr() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p title=foo><p title=bar><p class=foo><p class=bar>"
                );
            String classVal = doc.Select("p").Attr("class");
            NUnit.Framework.Assert.AreEqual("foo", classVal);
        }

        [NUnit.Framework.Test]
        public virtual void AbsAttr() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a id=1 href='/foo'>One</a> <a id=2 href='https://jsoup.org/'>Two</a>"
                );
            Elements one = doc.Select("#1");
            Elements two = doc.Select("#2");
            Elements both = doc.Select("a");
            NUnit.Framework.Assert.AreEqual("", one.Attr("abs:href"));
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/", two.Attr("abs:href"));
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/", both.Attr("abs:href"));
        }

        [NUnit.Framework.Test]
        public virtual void Classes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p class='mellow yellow'></p><p class='red green'></p>"
                );
            Elements els = doc.Select("p");
            NUnit.Framework.Assert.IsTrue(els.HasClass("red"));
            NUnit.Framework.Assert.IsFalse(els.HasClass("blue"));
            els.AddClass("blue");
            els.RemoveClass("yellow");
            els.ToggleClass("mellow");
            NUnit.Framework.Assert.AreEqual("blue", els[0].ClassName());
            NUnit.Framework.Assert.AreEqual("red green blue mellow", els[1].ClassName());
        }

        [NUnit.Framework.Test]
        public virtual void HasClassCaseInsensitive() {
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p Class=One>One <p class=Two>Two <p CLASS=THREE>THREE"
                ).Select("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element one = els[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element two = els[1];
            iText.StyledXmlParser.Jsoup.Nodes.Element thr = els[2];
            NUnit.Framework.Assert.IsTrue(one.HasClass("One"));
            NUnit.Framework.Assert.IsTrue(one.HasClass("ONE"));
            NUnit.Framework.Assert.IsTrue(two.HasClass("TWO"));
            NUnit.Framework.Assert.IsTrue(two.HasClass("Two"));
            NUnit.Framework.Assert.IsTrue(thr.HasClass("ThreE"));
            NUnit.Framework.Assert.IsTrue(thr.HasClass("three"));
        }

        [NUnit.Framework.Test]
        public virtual void Text() {
            String h = "<div><p>Hello<p>there<p>world</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("Hello there world", doc.Select("div > *").Text());
        }

        [NUnit.Framework.Test]
        public virtual void HasText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div><p></p></div>");
            Elements divs = doc.Select("div");
            NUnit.Framework.Assert.IsTrue(divs.HasText());
            NUnit.Framework.Assert.IsFalse(doc.Select("div + div").HasText());
        }

        [NUnit.Framework.Test]
        public virtual void Html() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div><p>There</p></div>");
            Elements divs = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("<p>Hello</p>\n<p>There</p>", divs.Html());
        }

        [NUnit.Framework.Test]
        public virtual void OuterHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div><p>There</p></div>");
            Elements divs = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("<div><p>Hello</p></div><div><p>There</p></div>", TextUtil.StripNewlines(divs
                .OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void SetHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One</p><p>Two</p><p>Three</p>");
            Elements ps = doc.Select("p");
            ps.Prepend("<b>Bold</b>").Append("<i>Ital</i>");
            NUnit.Framework.Assert.AreEqual("<p><b>Bold</b>Two<i>Ital</i></p>", TextUtil.StripNewlines(ps[1].OuterHtml
                ()));
            ps.Html("<span>Gone</span>");
            NUnit.Framework.Assert.AreEqual("<p><span>Gone</span></p>", TextUtil.StripNewlines(ps[1].OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void Val() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<input value='one' /><textarea>two</textarea>");
            Elements els = doc.Select("input, textarea");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("one", els.Val());
            NUnit.Framework.Assert.AreEqual("two", els.Last().Val());
            els.Val("three");
            NUnit.Framework.Assert.AreEqual("three", els.First().Val());
            NUnit.Framework.Assert.AreEqual("three", els.Last().Val());
            NUnit.Framework.Assert.AreEqual("<textarea>three</textarea>", els.Last().OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void Before() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>This <a>is</a> <a>jsoup</a>.</p>");
            doc.Select("a").Before("<span>foo</span>");
            NUnit.Framework.Assert.AreEqual("<p>This <span>foo</span><a>is</a> <span>foo</span><a>jsoup</a>.</p>", TextUtil
                .StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void After() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>This <a>is</a> <a>jsoup</a>.</p>");
            doc.Select("a").After("<span>foo</span>");
            NUnit.Framework.Assert.AreEqual("<p>This <a>is</a><span>foo</span> <a>jsoup</a><span>foo</span>.</p>", TextUtil
                .StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void Wrap() {
            String h = "<p><b>This</b> is <b>jsoup</b></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.Select("b").Wrap("<i></i>");
            NUnit.Framework.Assert.AreEqual("<p><i><b>This</b></i> is <i><b>jsoup</b></i></p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void WrapDiv() {
            String h = "<p><b>This</b> is <b>jsoup</b>.</p> <p>How do you like it?</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.Select("p").Wrap("<div></div>");
            NUnit.Framework.Assert.AreEqual("<div><p><b>This</b> is <b>jsoup</b>.</p></div> <div><p>How do you like it?</p></div>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void Unwrap() {
            String h = "<div><font>One</font> <font><a href=\"/\">Two</a></font></div";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.Select("font").Unwrap();
            NUnit.Framework.Assert.AreEqual("<div>One <a href=\"/\">Two</a></div>", TextUtil.StripNewlines(doc.Body().
                Html()));
        }

        [NUnit.Framework.Test]
        public virtual void UnwrapP() {
            String h = "<p><a>One</a> Two</p> Three <i>Four</i> <p>Fix <i>Six</i></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.Select("p").Unwrap();
            NUnit.Framework.Assert.AreEqual("<a>One</a> Two Three <i>Four</i> Fix <i>Six</i>", TextUtil.StripNewlines(
                doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void UnwrapKeepsSpace() {
            String h = "<p>One <span>two</span> <span>three</span> four</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.Select("span").Unwrap();
            NUnit.Framework.Assert.AreEqual("<p>One two three four</p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void Empty() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello <b>there</b></p> <p>now!</p></div>");
            doc.OutputSettings().PrettyPrint(false);
            doc.Select("p").Empty();
            NUnit.Framework.Assert.AreEqual("<div><p></p> <p></p></div>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void Remove() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello <b>there</b></p> jsoup <p>now!</p></div>"
                );
            doc.OutputSettings().PrettyPrint(false);
            doc.Select("p").Remove();
            NUnit.Framework.Assert.AreEqual("<div> jsoup </div>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void Eq() {
            String h = "<p>Hello<p>there<p>world";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("there", doc.Select("p").Eq(1).Text());
            NUnit.Framework.Assert.AreEqual("there", doc.Select("p")[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void Is() {
            String h = "<p>Hello<p title=foo>there<p>world";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.IsTrue(ps.Is("[title=foo]"));
            NUnit.Framework.Assert.IsFalse(ps.Is("[title=bar]"));
        }

        [NUnit.Framework.Test]
        public virtual void Parents() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><p>There</p>");
            Elements parents = doc.Select("p").Parents();
            NUnit.Framework.Assert.AreEqual(3, parents.Count);
            NUnit.Framework.Assert.AreEqual("div", parents[0].TagName());
            NUnit.Framework.Assert.AreEqual("body", parents[1].TagName());
            NUnit.Framework.Assert.AreEqual("html", parents[2].TagName());
        }

        [NUnit.Framework.Test]
        public virtual void Not() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>One</p></div> <div id=2><p><span>Two</span></p></div>"
                );
            Elements div1 = doc.Select("div").Not(":has(p > span)");
            NUnit.Framework.Assert.AreEqual(1, div1.Count);
            NUnit.Framework.Assert.AreEqual("1", div1.First().Id());
            Elements div2 = doc.Select("div").Not("#1");
            NUnit.Framework.Assert.AreEqual(1, div2.Count);
            NUnit.Framework.Assert.AreEqual("2", div2.First().Id());
        }

        [NUnit.Framework.Test]
        public virtual void TagNameSet() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello <i>there</i> <i>now</i></p>");
            doc.Select("i").TagName("em");
            NUnit.Framework.Assert.AreEqual("<p>Hello <em>there</em> <em>now</em></p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void Traverse() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There</div>");
            StringBuilder accum = new StringBuilder();
            doc.Select("div").Traverse(new _NodeVisitor_278(accum));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p></div><div><#text></#text></div>", accum.ToString
                ());
        }

        private sealed class _NodeVisitor_278 : NodeVisitor {
            public _NodeVisitor_278(StringBuilder accum) {
                this.accum = accum;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("<").Append(node.NodeName()).Append(">");
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</").Append(node.NodeName()).Append(">");
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void Forms() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<form id=1><input name=q></form><div /><form id=2><input name=f></form>"
                );
            Elements els = doc.Select("form, div");
            NUnit.Framework.Assert.AreEqual(3, els.Count);
            IList<FormElement> forms = els.Forms();
            NUnit.Framework.Assert.AreEqual(2, forms.Count);
            NUnit.Framework.Assert.IsNotNull(forms[0]);
            NUnit.Framework.Assert.IsNotNull(forms[1]);
            NUnit.Framework.Assert.AreEqual("1", forms[0].Id());
            NUnit.Framework.Assert.AreEqual("2", forms[1].Id());
        }

        [NUnit.Framework.Test]
        public virtual void Comments() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<!-- comment1 --><p><!-- comment2 --><p class=two><!-- comment3 -->"
                );
            IList<Comment> comments = doc.Select("p").Comments();
            NUnit.Framework.Assert.AreEqual(2, comments.Count);
            NUnit.Framework.Assert.AreEqual(" comment2 ", comments[0].GetData());
            NUnit.Framework.Assert.AreEqual(" comment3 ", comments[1].GetData());
            IList<Comment> comments1 = doc.Select("p.two").Comments();
            NUnit.Framework.Assert.AreEqual(1, comments1.Count);
            NUnit.Framework.Assert.AreEqual(" comment3 ", comments1[0].GetData());
        }

        [NUnit.Framework.Test]
        public virtual void TextNodes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("One<p>Two<a>Three</a><p>Four</p>Five");
            IList<TextNode> textNodes = doc.Select("p").TextNodes();
            NUnit.Framework.Assert.AreEqual(2, textNodes.Count);
            NUnit.Framework.Assert.AreEqual("Two", textNodes[0].Text());
            NUnit.Framework.Assert.AreEqual("Four", textNodes[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void DataNodes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One</p><script>Two</script><style>Three</style>"
                );
            IList<DataNode> dataNodes = doc.Select("p, script, style").DataNodes();
            NUnit.Framework.Assert.AreEqual(2, dataNodes.Count);
            NUnit.Framework.Assert.AreEqual("Two", dataNodes[0].GetWholeData());
            NUnit.Framework.Assert.AreEqual("Three", dataNodes[1].GetWholeData());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<head><script type=application/json><crux></script><script src=foo>Blah</script>"
                );
            Elements script = doc.Select("script[type=application/json]");
            IList<DataNode> scriptNode = script.DataNodes();
            NUnit.Framework.Assert.AreEqual(1, scriptNode.Count);
            DataNode dataNode = scriptNode[0];
            NUnit.Framework.Assert.AreEqual("<crux>", dataNode.GetWholeData());
            // check if they're live
            dataNode.SetWholeData("<cromulent>");
            NUnit.Framework.Assert.AreEqual("<script type=\"application/json\"><cromulent></script>", script.OuterHtml
                ());
        }

        [NUnit.Framework.Test]
        public virtual void NodesEmpty() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>");
            NUnit.Framework.Assert.AreEqual(0, doc.Select("form").TextNodes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ClassWithHyphen() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p class='tab-nav'>Check</p>");
            Elements els = doc.GetElementsByClass("tab-nav");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Check", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void Siblings() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>1<p>2<p>3<p>4<p>5<p>6</div><div><p>7<p>8<p>9<p>10<p>11<p>12</div>"
                );
            Elements els = doc.Select("p:eq(3)");
            // gets p4 and p10
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            Elements next = els.Next();
            NUnit.Framework.Assert.AreEqual(2, next.Count);
            NUnit.Framework.Assert.AreEqual("5", next.First().Text());
            NUnit.Framework.Assert.AreEqual("11", next.Last().Text());
            NUnit.Framework.Assert.AreEqual(0, els.Next("p:contains(6)").Count);
            Elements nextF = els.Next("p:contains(5)");
            NUnit.Framework.Assert.AreEqual(1, nextF.Count);
            NUnit.Framework.Assert.AreEqual("5", nextF.First().Text());
            Elements nextA = els.NextAll();
            NUnit.Framework.Assert.AreEqual(4, nextA.Count);
            NUnit.Framework.Assert.AreEqual("5", nextA.First().Text());
            NUnit.Framework.Assert.AreEqual("12", nextA.Last().Text());
            Elements nextAF = els.NextAll("p:contains(6)");
            NUnit.Framework.Assert.AreEqual(1, nextAF.Count);
            NUnit.Framework.Assert.AreEqual("6", nextAF.First().Text());
            Elements prev = els.Prev();
            NUnit.Framework.Assert.AreEqual(2, prev.Count);
            NUnit.Framework.Assert.AreEqual("3", prev.First().Text());
            NUnit.Framework.Assert.AreEqual("9", prev.Last().Text());
            NUnit.Framework.Assert.AreEqual(0, els.Prev("p:contains(1)").Count);
            Elements prevF = els.Prev("p:contains(3)");
            NUnit.Framework.Assert.AreEqual(1, prevF.Count);
            NUnit.Framework.Assert.AreEqual("3", prevF.First().Text());
            Elements prevA = els.PrevAll();
            NUnit.Framework.Assert.AreEqual(6, prevA.Count);
            NUnit.Framework.Assert.AreEqual("3", prevA.First().Text());
            NUnit.Framework.Assert.AreEqual("7", prevA.Last().Text());
            Elements prevAF = els.PrevAll("p:contains(1)");
            NUnit.Framework.Assert.AreEqual(1, prevAF.Count);
            NUnit.Framework.Assert.AreEqual("1", prevAF.First().Text());
        }

        [NUnit.Framework.Test]
        public virtual void EachText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>1<p>2<p>3<p>4<p>5<p>6</div><div><p>7<p>8<p>9<p>10<p>11<p>12<p></p></div>"
                );
            IList<String> divText = doc.Select("div").EachText();
            NUnit.Framework.Assert.AreEqual(2, divText.Count);
            NUnit.Framework.Assert.AreEqual("1 2 3 4 5 6", divText[0]);
            NUnit.Framework.Assert.AreEqual("7 8 9 10 11 12", divText[1]);
            IList<String> pText = doc.Select("p").EachText();
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(13, ps.Count);
            NUnit.Framework.Assert.AreEqual(12, pText.Count);
            // not 13, as last doesn't have text
            NUnit.Framework.Assert.AreEqual("1", pText[0]);
            NUnit.Framework.Assert.AreEqual("2", pText[1]);
            NUnit.Framework.Assert.AreEqual("5", pText[4]);
            NUnit.Framework.Assert.AreEqual("7", pText[6]);
            NUnit.Framework.Assert.AreEqual("12", pText[11]);
        }

        [NUnit.Framework.Test]
        public virtual void EachAttr() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><a href='/foo'>1</a><a href='http://example.com/bar'>2</a><a href=''>3</a><a>4</a>"
                , "http://example.com");
            IList<String> hrefAttrs = doc.Select("a").EachAttr("href");
            NUnit.Framework.Assert.AreEqual(3, hrefAttrs.Count);
            NUnit.Framework.Assert.AreEqual("/foo", hrefAttrs[0]);
            NUnit.Framework.Assert.AreEqual("http://example.com/bar", hrefAttrs[1]);
            NUnit.Framework.Assert.AreEqual("", hrefAttrs[2]);
            NUnit.Framework.Assert.AreEqual(4, doc.Select("a").Count);
            IList<String> absAttrs = doc.Select("a").EachAttr("abs:href");
            NUnit.Framework.Assert.AreEqual(3, absAttrs.Count);
            NUnit.Framework.Assert.AreEqual(3, absAttrs.Count);
            NUnit.Framework.Assert.AreEqual("http://example.com/foo", absAttrs[0]);
            NUnit.Framework.Assert.AreEqual("http://example.com/bar", absAttrs[1]);
            NUnit.Framework.Assert.AreEqual("http://example.com/", absAttrs[2]);
        }
    }
}
