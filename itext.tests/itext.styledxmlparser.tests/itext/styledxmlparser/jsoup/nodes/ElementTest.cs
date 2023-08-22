/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for Element (DOM stuff mostly).</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class ElementTest : ExtendedITextTest {
        private String reference = "<div id=div1><p>Hello</p><p>Another <b>element</b></p><div id=div2><img src=foo.png></div></div>";

        private static void ValidateScriptContents(String src, iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            NUnit.Framework.Assert.AreEqual("", el.Text());
            // it's not text
            NUnit.Framework.Assert.AreEqual("", el.OwnText());
            NUnit.Framework.Assert.AreEqual("", el.WholeText());
            NUnit.Framework.Assert.AreEqual(src, el.Html());
            NUnit.Framework.Assert.AreEqual(src, el.Data());
        }

        private static void ValidateXmlScriptContents(iText.StyledXmlParser.Jsoup.Nodes.Element el) {
            NUnit.Framework.Assert.AreEqual("var foo = 5 < 2; var bar = 1 && 2;", el.Text());
            NUnit.Framework.Assert.AreEqual("var foo = 5 < 2; var bar = 1 && 2;", el.OwnText());
            NUnit.Framework.Assert.AreEqual("var foo = 5 < 2;\nvar bar = 1 && 2;", el.WholeText());
            NUnit.Framework.Assert.AreEqual("var foo = 5 &lt; 2;\nvar bar = 1 &amp;&amp; 2;", el.Html());
            NUnit.Framework.Assert.AreEqual("", el.Data());
        }

        [NUnit.Framework.Test]
        public virtual void TestId() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=Foo>");
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.SelectFirst("div");
            NUnit.Framework.Assert.AreEqual("Foo", el.Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetId() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=Boo>");
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.SelectFirst("div");
            el.Id("Foo");
            NUnit.Framework.Assert.AreEqual("Foo", el.Id());
        }

        [NUnit.Framework.Test]
        public virtual void GetElementsByTagName() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(reference);
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> divs = doc.GetElementsByTag("div");
            NUnit.Framework.Assert.AreEqual(2, divs.Count);
            NUnit.Framework.Assert.AreEqual("div1", divs[0].Id());
            NUnit.Framework.Assert.AreEqual("div2", divs[1].Id());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> ps = doc.GetElementsByTag("p");
            NUnit.Framework.Assert.AreEqual(2, ps.Count);
            NUnit.Framework.Assert.AreEqual("Hello", ((TextNode)ps[0].ChildNode(0)).GetWholeText());
            NUnit.Framework.Assert.AreEqual("Another ", ((TextNode)ps[1].ChildNode(0)).GetWholeText());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> ps2 = doc.GetElementsByTag("P");
            NUnit.Framework.Assert.AreEqual(ps, ps2);
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> imgs = doc.GetElementsByTag("img");
            NUnit.Framework.Assert.AreEqual("foo.png", imgs[0].Attr("src"));
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> empty = doc.GetElementsByTag("fff");
            NUnit.Framework.Assert.AreEqual(0, empty.Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetNamespacedElementsByTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><abc:def id=1>Hello</abc:def></div>");
            Elements els = doc.GetElementsByTag("abc:def");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("1", els.First().Id());
            NUnit.Framework.Assert.AreEqual("abc:def", els.First().TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetElementById() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(reference);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("div1");
            NUnit.Framework.Assert.AreEqual("div1", div.Id());
            NUnit.Framework.Assert.IsNull(doc.GetElementById("none"));
            Document doc2 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><div id=2><p>Hello <span id=2>world!</span></p></div></div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = doc2.GetElementById("2");
            NUnit.Framework.Assert.AreEqual("div", div2.TagName());
            // not the span
            iText.StyledXmlParser.Jsoup.Nodes.Element span = div2.Child(0).GetElementById("2");
            // called from <p> context should be span
            NUnit.Framework.Assert.AreEqual("span", span.TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(reference);
            NUnit.Framework.Assert.AreEqual("Hello Another element", doc.Text());
            NUnit.Framework.Assert.AreEqual("Another element", doc.GetElementsByTag("p")[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetChildText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello <b>there</b> now");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.AreEqual("Hello there now", p.Text());
            NUnit.Framework.Assert.AreEqual("Hello now", p.OwnText());
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalisesText() {
            String h = "<p>Hello<p>There.</p> \n <p>Here <b>is</b> \n s<b>om</b>e text.";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            String text = doc.Text();
            NUnit.Framework.Assert.AreEqual("Hello There. Here is some text.", text);
        }

        [NUnit.Framework.Test]
        public virtual void TestKeepsPreText() {
            String h = "<p>Hello \n \n there.</p> <div><pre>  What's \n\n  that?</pre>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("Hello there.   What's \n\n  that?", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestKeepsPreTextInCode() {
            String h = "<pre><code>code\n\ncode</code></pre>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("code\n\ncode", doc.Text());
            NUnit.Framework.Assert.AreEqual("<pre><code>code\n\ncode</code></pre>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestKeepsPreTextAtDepth() {
            String h = "<pre><code><span><b>code\n\ncode</b></span></code></pre>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("code\n\ncode", doc.Text());
            NUnit.Framework.Assert.AreEqual("<pre><code><span><b>code\n\ncode</b></span></code></pre>", doc.Body().Html
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestBrHasSpace() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello<br>there</p>");
            NUnit.Framework.Assert.AreEqual("Hello there", doc.Text());
            NUnit.Framework.Assert.AreEqual("Hello there", doc.Select("p").First().OwnText());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello <br> there</p>");
            NUnit.Framework.Assert.AreEqual("Hello there", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestWholeText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p> Hello\nthere &nbsp;  </p>");
            NUnit.Framework.Assert.AreEqual(" Hello\nthere    ", doc.WholeText());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello  \n  there</p>");
            NUnit.Framework.Assert.AreEqual("Hello  \n  there", doc.WholeText());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello  <div>\n  there</div></p>");
            NUnit.Framework.Assert.AreEqual("Hello  \n  there", doc.WholeText());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSiblings() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello<p id=1>there<p>this<p>is<p>an<p id=last>element</div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.GetElementById("1");
            NUnit.Framework.Assert.AreEqual("there", p.Text());
            NUnit.Framework.Assert.AreEqual("Hello", p.PreviousElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("this", p.NextElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("Hello", p.FirstElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("element", p.LastElementSibling().Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetSiblingsWithDuplicateContent() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello<p id=1>there<p>this<p>this<p>is<p>an<p id=last>element</div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.GetElementById("1");
            NUnit.Framework.Assert.AreEqual("there", p.Text());
            NUnit.Framework.Assert.AreEqual("Hello", p.PreviousElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("this", p.NextElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("this", p.NextElementSibling().NextElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("is", p.NextElementSibling().NextElementSibling().NextElementSibling().Text
                ());
            NUnit.Framework.Assert.AreEqual("Hello", p.FirstElementSibling().Text());
            NUnit.Framework.Assert.AreEqual("element", p.LastElementSibling().Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestFirstElementSiblingOnOrphan() {
            iText.StyledXmlParser.Jsoup.Nodes.Element p = new iText.StyledXmlParser.Jsoup.Nodes.Element("p");
            NUnit.Framework.Assert.AreSame(p, p.FirstElementSibling());
            NUnit.Framework.Assert.AreSame(p, p.LastElementSibling());
        }

        [NUnit.Framework.Test]
        public virtual void TestFirstAndLastSiblings() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element one = div.Child(0);
            iText.StyledXmlParser.Jsoup.Nodes.Element two = div.Child(1);
            iText.StyledXmlParser.Jsoup.Nodes.Element three = div.Child(2);
            NUnit.Framework.Assert.AreSame(one, one.FirstElementSibling());
            NUnit.Framework.Assert.AreSame(one, two.FirstElementSibling());
            NUnit.Framework.Assert.AreSame(three, three.LastElementSibling());
            NUnit.Framework.Assert.AreSame(three, two.LastElementSibling());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetParents() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello <span>there</span></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element span = doc.Select("span").First();
            Elements parents = span.Parents();
            NUnit.Framework.Assert.AreEqual(4, parents.Count);
            NUnit.Framework.Assert.AreEqual("p", parents[0].TagName());
            NUnit.Framework.Assert.AreEqual("div", parents[1].TagName());
            NUnit.Framework.Assert.AreEqual("body", parents[2].TagName());
            NUnit.Framework.Assert.AreEqual("html", parents[3].TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestElementSiblingIndex() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p>...<p>Two</p>...<p>Three</p>");
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(0, ps[0].ElementSiblingIndex());
            NUnit.Framework.Assert.AreEqual(1, ps[1].ElementSiblingIndex());
            NUnit.Framework.Assert.AreEqual(2, ps[2].ElementSiblingIndex());
        }

        [NUnit.Framework.Test]
        public virtual void TestElementSiblingIndexSameContent() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p>...<p>One</p>...<p>One</p>");
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(0, ps[0].ElementSiblingIndex());
            NUnit.Framework.Assert.AreEqual(1, ps[1].ElementSiblingIndex());
            NUnit.Framework.Assert.AreEqual(2, ps[2].ElementSiblingIndex());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetElementsWithClass() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class='mellow yellow'><span class=mellow>Hello <b class='yellow'>Yellow!</b></span><p>Empty</p></div>"
                );
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> els = doc.GetElementsByClass("mellow");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("div", els[0].TagName());
            NUnit.Framework.Assert.AreEqual("span", els[1].TagName());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> els2 = doc.GetElementsByClass("yellow");
            NUnit.Framework.Assert.AreEqual(2, els2.Count);
            NUnit.Framework.Assert.AreEqual("div", els2[0].TagName());
            NUnit.Framework.Assert.AreEqual("b", els2[1].TagName());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> none = doc.GetElementsByClass("solo");
            NUnit.Framework.Assert.AreEqual(0, none.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetElementsWithAttribute() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div style='bold'><p title=qux><p><b style></b></p></div>"
                );
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> els = doc.GetElementsByAttribute("style");
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("div", els[0].TagName());
            NUnit.Framework.Assert.AreEqual("b", els[1].TagName());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> none = doc.GetElementsByAttribute("class");
            NUnit.Framework.Assert.AreEqual(0, none.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestGetElementsWithAttributeDash() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<meta http-equiv=content-type value=utf8 id=1> <meta name=foo content=bar id=2> <div http-equiv=content-type value=utf8 id=3>"
                );
            Elements meta = doc.Select("meta[http-equiv=content-type], meta[charset]");
            NUnit.Framework.Assert.AreEqual(1, meta.Count);
            NUnit.Framework.Assert.AreEqual("1", meta.First().Id());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetElementsWithAttributeValue() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div style='bold'><p><p><b style></b></p></div>");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> els = doc.GetElementsByAttributeValue("style", "bold");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("div", els[0].TagName());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> none = doc.GetElementsByAttributeValue("style", "none");
            NUnit.Framework.Assert.AreEqual(0, none.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestClassDomMethods() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><span class=' mellow yellow '>Hello <b>Yellow</b></span></div>"
                );
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> els = doc.GetElementsByAttribute("class");
            iText.StyledXmlParser.Jsoup.Nodes.Element span = els[0];
            NUnit.Framework.Assert.AreEqual("mellow yellow", span.ClassName());
            NUnit.Framework.Assert.IsTrue(span.HasClass("mellow"));
            NUnit.Framework.Assert.IsTrue(span.HasClass("yellow"));
            ICollection<String> classes = span.ClassNames();
            NUnit.Framework.Assert.AreEqual(2, classes.Count);
            NUnit.Framework.Assert.IsTrue(classes.Contains("mellow"));
            NUnit.Framework.Assert.IsTrue(classes.Contains("yellow"));
            NUnit.Framework.Assert.AreEqual("", doc.ClassName());
            classes = doc.ClassNames();
            NUnit.Framework.Assert.AreEqual(0, classes.Count);
            NUnit.Framework.Assert.IsFalse(doc.HasClass("mellow"));
        }

        [NUnit.Framework.Test]
        public virtual void TestHasClassDomMethods() {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("a");
            Attributes attribs = new Attributes();
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, "", attribs
                );
            attribs.Put("class", "toto");
            bool hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", " toto");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", "toto ");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", "\ttoto ");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", "  toto ");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", "ab");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsFalse(hasClass);
            attribs.Put("class", "     ");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsFalse(hasClass);
            attribs.Put("class", "tototo");
            hasClass = el.HasClass("toto");
            NUnit.Framework.Assert.IsFalse(hasClass);
            attribs.Put("class", "raulpismuth  ");
            hasClass = el.HasClass("raulpismuth");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", " abcd  raulpismuth efgh ");
            hasClass = el.HasClass("raulpismuth");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", " abcd efgh raulpismuth");
            hasClass = el.HasClass("raulpismuth");
            NUnit.Framework.Assert.IsTrue(hasClass);
            attribs.Put("class", " abcd efgh raulpismuth ");
            hasClass = el.HasClass("raulpismuth");
            NUnit.Framework.Assert.IsTrue(hasClass);
        }

        [NUnit.Framework.Test]
        public virtual void TestClassUpdates() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class='mellow yellow'></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            div.AddClass("green");
            NUnit.Framework.Assert.AreEqual("mellow yellow green", div.ClassName());
            div.RemoveClass("red");
            // noop
            div.RemoveClass("yellow");
            NUnit.Framework.Assert.AreEqual("mellow green", div.ClassName());
            div.ToggleClass("green").ToggleClass("red");
            NUnit.Framework.Assert.AreEqual("mellow red", div.ClassName());
        }

        [NUnit.Framework.Test]
        public virtual void TestOuterHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div title='Tags &amp;c.'><img src=foo.png><p><!-- comment -->Hello<p>there"
                );
            NUnit.Framework.Assert.AreEqual("<html><head></head><body><div title=\"Tags &amp;c.\"><img src=\"foo.png\"><p><!-- comment -->Hello</p><p>there</p></div></body></html>"
                , TextUtil.StripNewlines(doc.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void TestInnerHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>\n <p>Hello</p> </div>");
            NUnit.Framework.Assert.AreEqual("<p>Hello</p>", doc.GetElementsByTag("div")[0].Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestFormatHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Format test</title><div><p>Hello <span>jsoup <span>users</span></span></p><p>Good.</p></div>"
                );
            NUnit.Framework.Assert.AreEqual("<html>\n <head>\n  <title>Format test</title>\n </head>\n <body>\n  <div>\n   <p>Hello <span>jsoup <span>users</span></span></p>\n   <p>Good.</p>\n  </div>\n </body>\n</html>"
                , doc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestFormatOutline() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Format test</title><div><p>Hello <span>jsoup <span>users</span></span></p><p>Good.</p></div>"
                );
            doc.OutputSettings().Outline(true);
            NUnit.Framework.Assert.AreEqual("<html>\n <head>\n  <title>Format test</title>\n </head>\n <body>\n  <div>\n   <p>\n    Hello \n    <span>\n     jsoup \n     <span>users</span>\n    </span>\n   </p>\n   <p>Good.</p>\n  </div>\n </body>\n</html>"
                , doc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetIndent() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello\nthere</p></div>");
            doc.OutputSettings().IndentAmount(0);
            NUnit.Framework.Assert.AreEqual("<html>\n<head></head>\n<body>\n<div>\n<p>Hello there</p>\n</div>\n</body>\n</html>"
                , doc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestNotPretty() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>   \n<p>Hello\n there\n</p></div>");
            doc.OutputSettings().PrettyPrint(false);
            NUnit.Framework.Assert.AreEqual("<html><head></head><body><div>   \n<p>Hello\n there\n</p></div></body></html>"
                , doc.Html());
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            NUnit.Framework.Assert.AreEqual("   \n<p>Hello\n there\n</p>", div.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestNotPrettyWithEnDashBody() {
            String html = "<div><span>1:15</span>&ndash;<span>2:15</span>&nbsp;p.m.</div>";
            Document document = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            document.OutputSettings().PrettyPrint(false);
            NUnit.Framework.Assert.AreEqual("<div><span>1:15</span>–<span>2:15</span>&nbsp;p.m.</div>", document.Body(
                ).Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestPrettyWithEnDashBody() {
            String html = "<div><span>1:15</span>&ndash;<span>2:15</span>&nbsp;p.m.</div>";
            Document document = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<div>\n <span>1:15</span>–<span>2:15</span>&nbsp;p.m.\n</div>", document.
                Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestPrettyAndOutlineWithEnDashBody() {
            String html = "<div><span>1:15</span>&ndash;<span>2:15</span>&nbsp;p.m.</div>";
            Document document = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            document.OutputSettings().Outline(true);
            NUnit.Framework.Assert.AreEqual("<div>\n <span>1:15</span>\n –\n <span>2:15</span>\n &nbsp;p.m.\n</div>", 
                document.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestBasicFormats() {
            String html = "<span>0</span>.<div><span>1</span>-<span>2</span><p><span>3</span>-<span>4</span><div>5</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<span>0</span>.\n" + "<div>\n" + " <span>1</span>-<span>2</span>\n" + " <p><span>3</span>-<span>4</span></p>\n"
                 + " <div>\n" + "  5\n" + " </div>\n" + "</div>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyElementFormatHtml() {
            // don't put newlines into empty blocks
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<section><div></div></section>");
            NUnit.Framework.Assert.AreEqual("<section>\n <div></div>\n</section>", doc.Select("section").First().OuterHtml
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestNoIndentOnScriptAndStyle() {
            // don't newline+indent closing </script> and </style> tags
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>one\ntwo</script>\n<style>three\nfour</style>"
                );
            NUnit.Framework.Assert.AreEqual("<script>one\ntwo</script> \n<style>three\nfour</style>", doc.Head().Html(
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestContainerOutput() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Hello there</title> <div><p>Hello</p><p>there</p></div> <div>Another</div>"
                );
            NUnit.Framework.Assert.AreEqual("<title>Hello there</title>", doc.Select("title").First().OuterHtml());
            NUnit.Framework.Assert.AreEqual("<div>\n <p>Hello</p>\n <p>there</p>\n</div>", doc.Select("div").First().OuterHtml
                ());
            NUnit.Framework.Assert.AreEqual("<div>\n <p>Hello</p>\n <p>there</p>\n</div> \n<div>\n Another\n</div>", doc
                .Select("body").First().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetText() {
            String h = "<div id=1>Hello <p>there <b>now</b></p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("Hello there now", doc.Text());
            // need to sort out node whitespace
            NUnit.Framework.Assert.AreEqual("there now", doc.Select("p")[0].Text());
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1").Text("Gone");
            NUnit.Framework.Assert.AreEqual("Gone", div.Text());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("p").Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestAddNewElement() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.AppendElement("p").Text("there");
            ((iText.StyledXmlParser.Jsoup.Nodes.Element)div.AppendElement("P").Attr("CLASS", "second")).Text("now");
            // manually specifying tag and attributes should maintain case based on parser settings
            NUnit.Framework.Assert.AreEqual("<html><head></head><body><div id=\"1\"><p>Hello</p><p>there</p><p class=\"second\">now</p></div></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
            // check sibling index (with short circuit on reindexChildren):
            Elements ps = doc.Select("p");
            for (int i = 0; i < ps.Count; i++) {
                NUnit.Framework.Assert.AreEqual(i, ps[i].siblingIndex);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestAddBooleanAttribute() {
            iText.StyledXmlParser.Jsoup.Nodes.Element div = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("div"), "");
            div.Attr("true", true);
            div.Attr("false", "value");
            div.Attr("false", false);
            NUnit.Framework.Assert.IsTrue(div.HasAttr("true"));
            NUnit.Framework.Assert.AreEqual("", div.Attr("true"));
            IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> attributes = div.Attributes().AsList();
            NUnit.Framework.Assert.AreEqual(1, attributes.Count);
            NUnit.Framework.Assert.IsFalse(div.HasAttr("false"));
            NUnit.Framework.Assert.AreEqual("<div true></div>", div.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestAppendRowToTable() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table><tr><td>1</td></tr></table>");
            iText.StyledXmlParser.Jsoup.Nodes.Element table = doc.Select("tbody").First();
            table.Append("<tr><td>2</td></tr>");
            NUnit.Framework.Assert.AreEqual("<table><tbody><tr><td>1</td></tr><tr><td>2</td></tr></tbody></table>", TextUtil
                .StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestPrependRowToTable() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table><tr><td>1</td></tr></table>");
            iText.StyledXmlParser.Jsoup.Nodes.Element table = doc.Select("tbody").First();
            table.Prepend("<tr><td>2</td></tr>");
            NUnit.Framework.Assert.AreEqual("<table><tbody><tr><td>2</td></tr><tr><td>1</td></tr></tbody></table>", TextUtil
                .StripNewlines(doc.Body().Html()));
            // check sibling index (reindexChildren):
            Elements ps = doc.Select("tr");
            for (int i = 0; i < ps.Count; i++) {
                NUnit.Framework.Assert.AreEqual(i, ps[i].siblingIndex);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPrependElement() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.PrependElement("p").Text("Before");
            NUnit.Framework.Assert.AreEqual("Before", div.Child(0).Text());
            NUnit.Framework.Assert.AreEqual("Hello", div.Child(1).Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestAddNewText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.AppendText(" there & now >");
            NUnit.Framework.Assert.AreEqual("<p>Hello</p> there &amp; now &gt;", TextUtil.StripNewlines(div.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestPrependText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.PrependText("there & now > ");
            NUnit.Framework.Assert.AreEqual("there & now > Hello", div.Text());
            NUnit.Framework.Assert.AreEqual("there &amp; now &gt; <p>Hello</p>", TextUtil.StripNewlines(div.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestThrowsOnAddNullText() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
                iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
                div.AppendText(null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestThrowsOnPrependNullText() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
                iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
                div.PrependText(null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestAddNewHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.Append("<p>there</p><p>now</p>");
            NUnit.Framework.Assert.AreEqual("<p>Hello</p><p>there</p><p>now</p>", TextUtil.StripNewlines(div.Html()));
            // check sibling index (no reindexChildren):
            Elements ps = doc.Select("p");
            for (int i = 0; i < ps.Count; i++) {
                NUnit.Framework.Assert.AreEqual(i, ps[i].siblingIndex);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPrependNewHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.Prepend("<p>there</p><p>now</p>");
            NUnit.Framework.Assert.AreEqual("<p>there</p><p>now</p><p>Hello</p>", TextUtil.StripNewlines(div.Html()));
            // check sibling index (reindexChildren):
            Elements ps = doc.Select("p");
            for (int i = 0; i < ps.Count; i++) {
                NUnit.Framework.Assert.AreEqual(i, ps[i].siblingIndex);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestSetHtml() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            div.Html("<p>there</p><p>now</p>");
            NUnit.Framework.Assert.AreEqual("<p>there</p><p>now</p>", TextUtil.StripNewlines(div.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestSetHtmlTitle() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><head id=2><title id=1></title></head></html>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element title = doc.GetElementById("1");
            title.Html("good");
            NUnit.Framework.Assert.AreEqual("good", title.Html());
            title.Html("<i>bad</i>");
            NUnit.Framework.Assert.AreEqual("&lt;i&gt;bad&lt;/i&gt;", title.Html());
            iText.StyledXmlParser.Jsoup.Nodes.Element head = doc.GetElementById("2");
            head.Html("<title><i>bad</i></title>");
            NUnit.Framework.Assert.AreEqual("<title>&lt;i&gt;bad&lt;/i&gt;</title>", head.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestWrap() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p><p>There</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            p.Wrap("<div class='head'></div>");
            NUnit.Framework.Assert.AreEqual("<div><div class=\"head\"><p>Hello</p></div><p>There</p></div>", TextUtil.
                StripNewlines(doc.Body().Html()));
            iText.StyledXmlParser.Jsoup.Nodes.Element ret = (iText.StyledXmlParser.Jsoup.Nodes.Element)p.Wrap("<div><div class=foo></div><p>What?</p></div>"
                );
            NUnit.Framework.Assert.AreEqual("<div><div class=\"head\"><div><div class=\"foo\"><p>Hello</p></div><p>What?</p></div></div><p>There</p></div>"
                , TextUtil.StripNewlines(doc.Body().Html()));
            NUnit.Framework.Assert.AreEqual(ret, p);
        }

        [NUnit.Framework.Test]
        public virtual void TestWrapNoop() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Node p = doc.Select("p").First();
            iText.StyledXmlParser.Jsoup.Nodes.Node wrapped = p.Wrap("Some junk");
            NUnit.Framework.Assert.AreSame(p, wrapped);
            NUnit.Framework.Assert.AreEqual("<div><p>Hello</p></div>", TextUtil.StripNewlines(doc.Body().Html()));
        }

        // should be a NOOP
        [NUnit.Framework.Test]
        public virtual void TestWrapOnOrphan() {
            iText.StyledXmlParser.Jsoup.Nodes.Element orphan = new iText.StyledXmlParser.Jsoup.Nodes.Element("span").Text
                ("Hello!");
            NUnit.Framework.Assert.IsFalse(orphan.HasParent());
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapped = (iText.StyledXmlParser.Jsoup.Nodes.Element)orphan.Wrap
                ("<div></div> There!");
            NUnit.Framework.Assert.AreSame(orphan, wrapped);
            NUnit.Framework.Assert.IsTrue(orphan.HasParent());
            // should now be in the DIV
            iText.StyledXmlParser.Jsoup.Nodes.Element parent = (iText.StyledXmlParser.Jsoup.Nodes.Element)orphan.Parent
                ();
            NUnit.Framework.Assert.IsNotNull(parent);
            NUnit.Framework.Assert.AreEqual("div", ((iText.StyledXmlParser.Jsoup.Nodes.Element)parent).TagName());
            NUnit.Framework.Assert.AreEqual("<div>\n <span>Hello!</span>\n</div>", parent.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestWrapArtificialStructure() {
            // div normally couldn't get into a p, but explicitly want to wrap
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello <i>there</i> now.");
            iText.StyledXmlParser.Jsoup.Nodes.Element i = doc.SelectFirst("i");
            i.Wrap("<div id=id1></div> quite");
            NUnit.Framework.Assert.AreEqual("div", ((iText.StyledXmlParser.Jsoup.Nodes.Element)i.Parent()).TagName());
            NUnit.Framework.Assert.AreEqual("<p>Hello <div id=\"id1\"><i>there</i></div> quite now.</p>", TextUtil.StripNewlines
                (doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void Before() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p><p>There</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p1 = doc.Select("p").First();
            p1.Before("<div>one</div><div>two</div>");
            NUnit.Framework.Assert.AreEqual("<div><div>one</div><div>two</div><p>Hello</p><p>There</p></div>", TextUtil
                .StripNewlines(doc.Body().Html()));
            doc.Select("p").Last().Before("<p>Three</p><!-- four -->");
            NUnit.Framework.Assert.AreEqual("<div><div>one</div><div>two</div><p>Hello</p><p>Three</p><!-- four --><p>There</p></div>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void After() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p><p>There</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p1 = doc.Select("p").First();
            p1.After("<div>one</div><div>two</div>");
            NUnit.Framework.Assert.AreEqual("<div><p>Hello</p><div>one</div><div>two</div><p>There</p></div>", TextUtil
                .StripNewlines(doc.Body().Html()));
            doc.Select("p").Last().After("<p>Three</p><!-- four -->");
            NUnit.Framework.Assert.AreEqual("<div><p>Hello</p><div>one</div><div>two</div><p>There</p><p>Three</p><!-- four --></div>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestWrapWithRemainder() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            p.Wrap("<div class='head'></div><p>There!</p>");
            NUnit.Framework.Assert.AreEqual("<div><div class=\"head\"><p>Hello</p></div><p>There!</p></div>", TextUtil
                .StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestWrapWithSimpleRemainder() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element body = (iText.StyledXmlParser.Jsoup.Nodes.Element)p.Parent();
            NUnit.Framework.Assert.IsNotNull(body);
            NUnit.Framework.Assert.AreEqual("body", body.TagName());
            p.Wrap("<div></div> There");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = (iText.StyledXmlParser.Jsoup.Nodes.Element)p.Parent();
            NUnit.Framework.Assert.IsNotNull(div);
            NUnit.Framework.Assert.AreEqual("div", div.TagName());
            NUnit.Framework.Assert.AreSame(div, p.Parent());
            NUnit.Framework.Assert.AreSame(body, div.Parent());
            NUnit.Framework.Assert.AreEqual("<div><p>Hello</p></div> There", TextUtil.StripNewlines(doc.Body().Html())
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestHasText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p><p></p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.IsTrue(div.HasText());
            NUnit.Framework.Assert.IsTrue(ps.First().HasText());
            NUnit.Framework.Assert.IsFalse(ps.Last().HasText());
        }

        [NUnit.Framework.Test]
        public virtual void Dataset() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1 data-name=jsoup class=new data-package=jar>Hello</div><p id=2>Hello</p>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            IDictionary<String, String> dataset = div.Dataset();
            Attributes attributes = div.Attributes();
            // size, get, set, add, remove
            NUnit.Framework.Assert.AreEqual(2, dataset.Count);
            NUnit.Framework.Assert.AreEqual("jsoup", dataset.Get("name"));
            NUnit.Framework.Assert.AreEqual("jar", dataset.Get("package"));
            dataset.Put("name", "jsoup updated");
            dataset.Put("language", "java");
            dataset.JRemove("package");
            NUnit.Framework.Assert.AreEqual(2, dataset.Count);
            NUnit.Framework.Assert.AreEqual(4, attributes.Size());
            NUnit.Framework.Assert.AreEqual("jsoup updated", attributes.Get("data-name"));
            NUnit.Framework.Assert.AreEqual("jsoup updated", dataset.Get("name"));
            NUnit.Framework.Assert.AreEqual("java", attributes.Get("data-language"));
            NUnit.Framework.Assert.AreEqual("java", dataset.Get("language"));
            attributes.Put("data-food", "bacon");
            NUnit.Framework.Assert.AreEqual(3, dataset.Count);
            NUnit.Framework.Assert.AreEqual("bacon", dataset.Get("food"));
            attributes.Put("data-", "empty");
            NUnit.Framework.Assert.IsNull(dataset.Get(""));
            // data- is not a data attribute
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.AreEqual(0, p.Dataset().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ParentlessToString() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<img src='foo'>");
            iText.StyledXmlParser.Jsoup.Nodes.Element img = doc.Select("img").First();
            NUnit.Framework.Assert.AreEqual("<img src=\"foo\">", img.ToString());
            img.Remove();
            // lost its parent
            NUnit.Framework.Assert.AreEqual("<img src=\"foo\">", img.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void OrphanDivToString() {
            iText.StyledXmlParser.Jsoup.Nodes.Element orphan = new iText.StyledXmlParser.Jsoup.Nodes.Element("div").Id
                ("foo").Text("Hello");
            NUnit.Framework.Assert.AreEqual("<div id=\"foo\">\n Hello\n</div>", orphan.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestClone() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p><span>Two</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p")[1];
            iText.StyledXmlParser.Jsoup.Nodes.Element clone = (iText.StyledXmlParser.Jsoup.Nodes.Element)p.Clone();
            NUnit.Framework.Assert.IsNull(clone.Parent());
            // should be orphaned
            NUnit.Framework.Assert.AreEqual(0, clone.siblingIndex);
            NUnit.Framework.Assert.AreEqual(1, p.siblingIndex);
            NUnit.Framework.Assert.IsNotNull(p.Parent());
            clone.Append("<span>Three");
            NUnit.Framework.Assert.AreEqual("<p><span>Two</span><span>Three</span></p>", TextUtil.StripNewlines(clone.
                OuterHtml()));
            NUnit.Framework.Assert.AreEqual("<div><p>One</p><p><span>Two</span></p></div>", TextUtil.StripNewlines(doc
                .Body().Html()));
            // not modified
            doc.Body().AppendChild(clone);
            // adopt
            NUnit.Framework.Assert.IsNotNull(clone.Parent());
            NUnit.Framework.Assert.AreEqual("<div><p>One</p><p><span>Two</span></p></div><p><span>Two</span><span>Three</span></p>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestClonesClassnames() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class='one two'></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            ICollection<String> classes = div.ClassNames();
            NUnit.Framework.Assert.AreEqual(2, classes.Count);
            NUnit.Framework.Assert.IsTrue(classes.Contains("one"));
            NUnit.Framework.Assert.IsTrue(classes.Contains("two"));
            iText.StyledXmlParser.Jsoup.Nodes.Element copy = (iText.StyledXmlParser.Jsoup.Nodes.Element)div.Clone();
            ICollection<String> copyClasses = copy.ClassNames();
            NUnit.Framework.Assert.AreEqual(2, copyClasses.Count);
            NUnit.Framework.Assert.IsTrue(copyClasses.Contains("one"));
            NUnit.Framework.Assert.IsTrue(copyClasses.Contains("two"));
            copyClasses.Add("three");
            copyClasses.Remove("one");
            NUnit.Framework.Assert.IsTrue(classes.Contains("one"));
            NUnit.Framework.Assert.IsFalse(classes.Contains("three"));
            NUnit.Framework.Assert.IsFalse(copyClasses.Contains("one"));
            NUnit.Framework.Assert.IsTrue(copyClasses.Contains("three"));
            NUnit.Framework.Assert.AreEqual("", div.Html());
            NUnit.Framework.Assert.AreEqual("", copy.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestShallowClone() {
            String @base = "http://example.com/";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1 class=one><p id=2 class=two>One", @base);
            iText.StyledXmlParser.Jsoup.Nodes.Element d = doc.SelectFirst("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            TextNode t = p.TextNodes()[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element d2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)d.ShallowClone();
            iText.StyledXmlParser.Jsoup.Nodes.Element p2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)p.ShallowClone();
            TextNode t2 = (TextNode)t.ShallowClone();
            NUnit.Framework.Assert.AreEqual(1, d.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(0, d2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(1, p.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(0, p2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual("", p2.Text());
            NUnit.Framework.Assert.AreEqual("One", t2.Text());
            NUnit.Framework.Assert.AreEqual("two", p2.ClassName());
            p2.RemoveClass("two");
            NUnit.Framework.Assert.AreEqual("two", p.ClassName());
            d2.Append("<p id=3>Three");
            NUnit.Framework.Assert.AreEqual(1, d2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual("Three", d2.Text());
            NUnit.Framework.Assert.AreEqual("One", d.Text());
            NUnit.Framework.Assert.AreEqual(@base, d2.BaseUri());
        }

        [NUnit.Framework.Test]
        public virtual void TestTagNameSet() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><i>Hello</i>");
            doc.Select("i").First().TagName("em");
            NUnit.Framework.Assert.AreEqual(0, doc.Select("i").Count);
            NUnit.Framework.Assert.AreEqual(1, doc.Select("em").Count);
            NUnit.Framework.Assert.AreEqual("<em>Hello</em>", doc.Select("div").First().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestHtmlContainsOuter() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Check</title> <div>Hello there</div>");
            doc.OutputSettings().IndentAmount(0);
            NUnit.Framework.Assert.IsTrue(doc.Html().Contains(doc.Select("title").OuterHtml()));
            NUnit.Framework.Assert.IsTrue(doc.Html().Contains(doc.Select("div").OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetTextNodes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <span>Two</span> Three <br> Four</p>");
            IList<TextNode> textNodes = doc.Select("p").First().TextNodes();
            NUnit.Framework.Assert.AreEqual(3, textNodes.Count);
            NUnit.Framework.Assert.AreEqual("One ", textNodes[0].Text());
            NUnit.Framework.Assert.AreEqual(" Three ", textNodes[1].Text());
            NUnit.Framework.Assert.AreEqual(" Four", textNodes[2].Text());
            NUnit.Framework.Assert.AreEqual(0, doc.Select("br").First().TextNodes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestManipulateTextNodes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <span>Two</span> Three <br> Four</p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            IList<TextNode> textNodes = p.TextNodes();
            textNodes[1].Text(" three-more ");
            textNodes[2].SplitText(3).Text("-ur");
            NUnit.Framework.Assert.AreEqual("One Two three-more Fo-ur", p.Text());
            NUnit.Framework.Assert.AreEqual("One three-more Fo-ur", p.OwnText());
            NUnit.Framework.Assert.AreEqual(4, p.TextNodes().Count);
        }

        // grew because of split
        [NUnit.Framework.Test]
        public virtual void TestGetDataNodes() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>One Two</script> <style>Three Four</style> <p>Fix Six</p>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element script = doc.Select("script").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element style = doc.Select("style").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            IList<DataNode> scriptData = script.DataNodes();
            NUnit.Framework.Assert.AreEqual(1, scriptData.Count);
            NUnit.Framework.Assert.AreEqual("One Two", scriptData[0].GetWholeData());
            IList<DataNode> styleData = style.DataNodes();
            NUnit.Framework.Assert.AreEqual(1, styleData.Count);
            NUnit.Framework.Assert.AreEqual("Three Four", styleData[0].GetWholeData());
            IList<DataNode> pData = p.DataNodes();
            NUnit.Framework.Assert.AreEqual(0, pData.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ElementIsNotASiblingOfItself() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p2 = doc.Select("p")[1];
            NUnit.Framework.Assert.AreEqual("Two", p2.Text());
            Elements els = p2.SiblingElements();
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual("<p>One</p>", els[0].OuterHtml());
            NUnit.Framework.Assert.AreEqual("<p>Three</p>", els[1].OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestChildThrowsIndexOutOfBoundsOnMissing() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p><p>Two</p></div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            NUnit.Framework.Assert.AreEqual(2, div.Children().Count);
            NUnit.Framework.Assert.AreEqual("One", div.Child(0).Text());
            try {
                div.Child(3);
                NUnit.Framework.Assert.Fail("Should throw index out of bounds");
            }
            catch (Exception) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void MoveByAppend() {
            // test for https://github.com/jhy/jsoup/issues/239
            // can empty an element and append its children to another element
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1>Text <p>One</p> Text <p>Two</p></div><div id=2></div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div1 = doc.Select("div")[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = doc.Select("div")[1];
            NUnit.Framework.Assert.AreEqual(4, div1.ChildNodeSize());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> children = div1.ChildNodes();
            NUnit.Framework.Assert.AreEqual(4, children.Count);
            div2.InsertChildren(0, children);
            NUnit.Framework.Assert.AreEqual(4, children.Count);
            // children is NOT backed by div1.childNodes but a wrapper, so should still be 4 (but re-parented)
            NUnit.Framework.Assert.AreEqual(0, div1.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(4, div2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual("<div id=\"1\"></div>\n<div id=\"2\">\n Text \n <p>One</p> Text \n <p>Two</p>\n</div>"
                , doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void InsertChildrenArgumentValidation() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1>Text <p>One</p> Text <p>Two</p></div><div id=2></div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div1 = doc.Select("div")[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = doc.Select("div")[1];
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> children = div1.ChildNodes();
            try {
                div2.InsertChildren(6, children);
                NUnit.Framework.Assert.Fail();
            }
            catch (ArgumentException) {
            }
            try {
                div2.InsertChildren(-5, children);
                NUnit.Framework.Assert.Fail();
            }
            catch (ArgumentException) {
            }
            try {
                div2.InsertChildren(0, (ICollection<iText.StyledXmlParser.Jsoup.Nodes.Node>)null);
                NUnit.Framework.Assert.Fail();
            }
            catch (ArgumentException) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void InsertChildrenAtPosition() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1>Text1 <p>One</p> Text2 <p>Two</p></div><div id=2>Text3 <p>Three</p></div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div1 = doc.Select("div")[0];
            Elements p1s = div1.Select("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = doc.Select("div")[1];
            NUnit.Framework.Assert.AreEqual(2, div2.ChildNodeSize());
            div2.InsertChildren(-1, p1s);
            NUnit.Framework.Assert.AreEqual(2, div1.ChildNodeSize());
            // moved two out
            NUnit.Framework.Assert.AreEqual(4, div2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(3, p1s[1].SiblingIndex());
            // should be last
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> els = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>();
            iText.StyledXmlParser.Jsoup.Nodes.Element el1 = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("span"), "").Text("Span1");
            iText.StyledXmlParser.Jsoup.Nodes.Element el2 = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("span"), "").Text("Span2");
            TextNode tn1 = new TextNode("Text4");
            els.Add(el1);
            els.Add(el2);
            els.Add(tn1);
            NUnit.Framework.Assert.IsNull(el1.Parent());
            div2.InsertChildren(-2, els);
            NUnit.Framework.Assert.AreEqual(div2, el1.Parent());
            NUnit.Framework.Assert.AreEqual(7, div2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(3, el1.SiblingIndex());
            NUnit.Framework.Assert.AreEqual(4, el2.SiblingIndex());
            NUnit.Framework.Assert.AreEqual(5, tn1.SiblingIndex());
        }

        [NUnit.Framework.Test]
        public virtual void InsertChildrenAsCopy() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1>Text <p>One</p> Text <p>Two</p></div><div id=2></div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div1 = doc.Select("div")[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = doc.Select("div")[1];
            Elements ps = (Elements)doc.Select("p").Clone();
            ps.First().Text("One cloned");
            div2.InsertChildren(-1, ps);
            NUnit.Framework.Assert.AreEqual(4, div1.ChildNodeSize());
            // not moved -- cloned
            NUnit.Framework.Assert.AreEqual(2, div2.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual("<div id=\"1\">Text <p>One</p> Text <p>Two</p></div><div id=\"2\"><p>One cloned</p><p>Two</p></div>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestCssPath() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=\"id1\">A</div><div>B</div><div class=\"c1 c2\">C</div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element divA = doc.Select("div")[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element divB = doc.Select("div")[1];
            iText.StyledXmlParser.Jsoup.Nodes.Element divC = doc.Select("div")[2];
            NUnit.Framework.Assert.AreEqual(divA.CssSelector(), "#id1");
            NUnit.Framework.Assert.AreEqual(divB.CssSelector(), "html > body > div:nth-child(2)");
            NUnit.Framework.Assert.AreEqual(divC.CssSelector(), "html > body > div.c1.c2");
            NUnit.Framework.Assert.AreSame(divA, doc.Select(divA.CssSelector()).First());
            NUnit.Framework.Assert.AreSame(divB, doc.Select(divB.CssSelector()).First());
            NUnit.Framework.Assert.AreSame(divC, doc.Select(divC.CssSelector()).First());
        }

        [NUnit.Framework.Test]
        public virtual void TestCssPathDuplicateIds() {
            // https://github.com/jhy/jsoup/issues/1147 - multiple elements with same ID, use the non-ID form
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<article><div id=dupe>A</div><div id=dupe>B</div><div id=dupe class=c1>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element divA = doc.Select("div")[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element divB = doc.Select("div")[1];
            iText.StyledXmlParser.Jsoup.Nodes.Element divC = doc.Select("div")[2];
            NUnit.Framework.Assert.AreEqual(divA.CssSelector(), "html > body > article > div:nth-child(1)");
            NUnit.Framework.Assert.AreEqual(divB.CssSelector(), "html > body > article > div:nth-child(2)");
            NUnit.Framework.Assert.AreEqual(divC.CssSelector(), "html > body > article > div.c1");
            NUnit.Framework.Assert.AreSame(divA, doc.Select(divA.CssSelector()).First());
            NUnit.Framework.Assert.AreSame(divB, doc.Select(divB.CssSelector()).First());
            NUnit.Framework.Assert.AreSame(divC, doc.Select(divC.CssSelector()).First());
        }

        [NUnit.Framework.Test]
        public virtual void TestClassNames() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class=\"c1 c2\">C</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div")[0];
            NUnit.Framework.Assert.AreEqual("c1 c2", div.ClassName());
            ICollection<String> set1 = div.ClassNames();
            Object[] arr1 = set1.ToArray();
            NUnit.Framework.Assert.AreEqual(2, arr1.Length);
            NUnit.Framework.Assert.AreEqual("c1", arr1[0]);
            NUnit.Framework.Assert.AreEqual("c2", arr1[1]);
            // Changes to the set should not be reflected in the Elements getters
            set1.Add("c3");
            NUnit.Framework.Assert.AreEqual(2, div.ClassNames().Count);
            NUnit.Framework.Assert.AreEqual("c1 c2", div.ClassName());
            // Update the class names to a fresh set
            ICollection<String> newSet = new LinkedHashSet<String>(set1);
            newSet.Add("c3");
            div.ClassNames(newSet);
            NUnit.Framework.Assert.AreEqual("c1 c2 c3", div.ClassName());
            ICollection<String> set2 = div.ClassNames();
            Object[] arr2 = set2.ToArray();
            NUnit.Framework.Assert.AreEqual(3, arr2.Length);
            NUnit.Framework.Assert.AreEqual("c1", arr2[0]);
            NUnit.Framework.Assert.AreEqual("c2", arr2[1]);
            NUnit.Framework.Assert.AreEqual("c3", arr2[2]);
        }

        [NUnit.Framework.Test]
        public virtual void TestHashAndEqualsAndValue() {
            // .equals and hashcode are identity. value is content.
            String doc1 = "<div id=1><p class=one>One</p><p class=one>One</p><p class=one>Two</p><p class=two>One</p></div>"
                 + "<div id=2><p class=one>One</p><p class=one>One</p><p class=one>Two</p><p class=two>One</p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(doc1);
            Elements els = doc.Select("p");
            /*
            for (Element el : els) {
            System.out.println(el.hashCode() + " - " + el.outerHtml());
            }
            
            0 1534787905 - <p class="one">One</p>
            1 1534787905 - <p class="one">One</p>
            2 1539683239 - <p class="one">Two</p>
            3 1535455211 - <p class="two">One</p>
            4 1534787905 - <p class="one">One</p>
            5 1534787905 - <p class="one">One</p>
            6 1539683239 - <p class="one">Two</p>
            7 1535455211 - <p class="two">One</p>
            */
            NUnit.Framework.Assert.AreEqual(8, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element e0 = els[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element e1 = els[1];
            iText.StyledXmlParser.Jsoup.Nodes.Element e2 = els[2];
            iText.StyledXmlParser.Jsoup.Nodes.Element e3 = els[3];
            iText.StyledXmlParser.Jsoup.Nodes.Element e4 = els[4];
            iText.StyledXmlParser.Jsoup.Nodes.Element e5 = els[5];
            iText.StyledXmlParser.Jsoup.Nodes.Element e6 = els[6];
            iText.StyledXmlParser.Jsoup.Nodes.Element e7 = els[7];
            NUnit.Framework.Assert.AreEqual(e0, e0);
            NUnit.Framework.Assert.IsTrue(e0.HasSameValue(e1));
            NUnit.Framework.Assert.IsTrue(e0.HasSameValue(e4));
            NUnit.Framework.Assert.IsTrue(e0.HasSameValue(e5));
            NUnit.Framework.Assert.AreNotEqual(e0, e2);
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e2));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e3));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e6));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e7));
            NUnit.Framework.Assert.AreEqual(e0.GetHashCode(), e0.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(e0.GetHashCode(), (e2.GetHashCode()));
            NUnit.Framework.Assert.AreNotEqual(e0.GetHashCode(), (e3).GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(e0.GetHashCode(), (e6).GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(e0.GetHashCode(), (e7).GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void TestRelativeUrls() {
            String html = "<body><a href='./one.html'>One</a> <a href='two.html'>two</a> <a href='../three.html'>Three</a> <a href='//example2.com/four/'>Four</a> <a href='https://example2.com/five/'>Five</a> <a>Six</a> <a href=''>Seven</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "http://example.com/bar/");
            Elements els = doc.Select("a");
            NUnit.Framework.Assert.AreEqual("http://example.com/bar/one.html", els[0].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example.com/bar/two.html", els[1].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example.com/three.html", els[2].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example2.com/four/", els[3].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("https://example2.com/five/", els[4].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("", els[5].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example.com/bar/", els[6].AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void TestRelativeIdnUrls() {
            String idn = "https://www.测试.测试/";
            String idnFoo = idn + "foo.html?bar";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href=''>One</a><a href='/bar.html?qux'>Two</a>"
                , idnFoo);
            Elements els = doc.Select("a");
            iText.StyledXmlParser.Jsoup.Nodes.Element one = els[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element two = els[1];
            String hrefOne = one.AbsUrl("href");
            String hrefTwo = two.AbsUrl("href");
            NUnit.Framework.Assert.AreEqual(idnFoo, hrefOne);
            NUnit.Framework.Assert.AreEqual("https://www.测试.测试/bar.html?qux", hrefTwo);
        }

        [NUnit.Framework.Test]
        public virtual void AppendMustCorrectlyMoveChildrenInsideOneParentElement() {
            Document doc = new Document("");
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.AppendElement("body");
            body.AppendElement("div1");
            body.AppendElement("div2");
            iText.StyledXmlParser.Jsoup.Nodes.Element div3 = body.AppendElement("div3");
            div3.Text("Check");
            iText.StyledXmlParser.Jsoup.Nodes.Element div4 = body.AppendElement("div4");
            List<iText.StyledXmlParser.Jsoup.Nodes.Element> toMove = new List<iText.StyledXmlParser.Jsoup.Nodes.Element
                >();
            toMove.Add(div3);
            toMove.Add(div4);
            body.InsertChildren(0, toMove);
            String result = iText.Commons.Utils.StringUtil.ReplaceAll(doc.ToString(), "\\s+", "");
            NUnit.Framework.Assert.AreEqual("<body><div3>Check</div3><div4></div4><div1></div1><div2></div2></body>", 
                result);
        }

        [NUnit.Framework.Test]
        public virtual void TestHashcodeIsStableWithContentChanges() {
            iText.StyledXmlParser.Jsoup.Nodes.Element root = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("root"), "");
            HashSet<iText.StyledXmlParser.Jsoup.Nodes.Element> set = new HashSet<iText.StyledXmlParser.Jsoup.Nodes.Element
                >();
            // Add root node:
            set.Add(root);
            root.AppendChild(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf
                ("a"), ""));
            NUnit.Framework.Assert.IsTrue(set.Contains(root));
        }

        [NUnit.Framework.Test]
        public virtual void TestNamespacedElements() {
            // Namespaces with ns:tag in HTML must be translated to ns|tag in CSS.
            String html = "<html><body><fb:comments /></body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "http://example.com/bar/");
            Elements els = doc.Select("fb|comments");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("html > body > fb|comments", els[0].CssSelector());
        }

        [NUnit.Framework.Test]
        public virtual void TestChainedRemoveAttributes() {
            String html = "<a one two three four>Text</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a").First();
            a.RemoveAttr("zero").RemoveAttr("one").RemoveAttr("two").RemoveAttr("three").RemoveAttr("four").RemoveAttr
                ("five");
            NUnit.Framework.Assert.AreEqual("<a>Text</a>", a.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestLoopedRemoveAttributes() {
            String html = "<a one two three four>Text</a><p foo>Two</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in doc.GetAllElements()) {
                el.ClearAttributes();
            }
            NUnit.Framework.Assert.AreEqual("<a>Text</a>\n<p>Two</p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestIs() {
            String html = "<div><p>One <a class=big>Two</a> Three</p><p>Another</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.IsTrue(p.Is("p"));
            NUnit.Framework.Assert.IsFalse(p.Is("div"));
            NUnit.Framework.Assert.IsTrue(p.Is("p:has(a)"));
            NUnit.Framework.Assert.IsFalse(p.Is("a"));
            // does not descend
            NUnit.Framework.Assert.IsTrue(p.Is("p:first-child"));
            NUnit.Framework.Assert.IsFalse(p.Is("p:last-child"));
            NUnit.Framework.Assert.IsTrue(p.Is("*"));
            NUnit.Framework.Assert.IsTrue(p.Is("div p"));
            iText.StyledXmlParser.Jsoup.Nodes.Element q = doc.Select("p").Last();
            NUnit.Framework.Assert.IsTrue(q.Is("p"));
            NUnit.Framework.Assert.IsTrue(q.Is("p ~ p"));
            NUnit.Framework.Assert.IsTrue(q.Is("p + p"));
            NUnit.Framework.Assert.IsTrue(q.Is("p:last-child"));
            NUnit.Framework.Assert.IsFalse(q.Is("p a"));
            NUnit.Framework.Assert.IsFalse(q.Is("a"));
        }

        [NUnit.Framework.Test]
        public virtual void TestEvalMethods() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One <a class=big>Two</a> Three</p><p>Another</p>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst(QueryParser.Parse(("p")));
            NUnit.Framework.Assert.AreEqual("One Three", p.OwnText());
            NUnit.Framework.Assert.IsTrue(p.Is(QueryParser.Parse("p")));
            Evaluator aEval = QueryParser.Parse("a");
            NUnit.Framework.Assert.IsFalse(p.Is(aEval));
            iText.StyledXmlParser.Jsoup.Nodes.Element a = p.SelectFirst(aEval);
            NUnit.Framework.Assert.AreEqual("div", a.Closest(QueryParser.Parse("div:has( > p)")).TagName());
            iText.StyledXmlParser.Jsoup.Nodes.Element body = p.Closest(QueryParser.Parse("body"));
            NUnit.Framework.Assert.AreEqual("body", body.NodeName());
        }

        [NUnit.Framework.Test]
        public virtual void TestClosest() {
            String html = "<article>\n" + "  <div id=div-01>Here is div-01\n" + "    <div id=div-02>Here is div-02\n" 
                + "      <div id=div-03>Here is div-03</div>\n" + "    </div>\n" + "  </div>\n" + "</article>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.SelectFirst("#div-03");
            NUnit.Framework.Assert.AreEqual("Here is div-03", el.Text());
            NUnit.Framework.Assert.AreEqual("div-03", el.Id());
            NUnit.Framework.Assert.AreEqual("div-02", el.Closest("#div-02").Id());
            NUnit.Framework.Assert.AreEqual(el, el.Closest("div div"));
            // closest div in a div is itself
            NUnit.Framework.Assert.AreEqual("div-01", el.Closest("article > div").Id());
            NUnit.Framework.Assert.AreEqual("article", el.Closest(":not(div)").TagName());
            NUnit.Framework.Assert.IsNull(el.Closest("p"));
        }

        [NUnit.Framework.Test]
        public virtual void ElementByTagName() {
            iText.StyledXmlParser.Jsoup.Nodes.Element a = new iText.StyledXmlParser.Jsoup.Nodes.Element("P");
            NUnit.Framework.Assert.AreEqual("P", a.TagName());
        }

        [NUnit.Framework.Test]
        public virtual void TestChildrenElements() {
            String html = "<div><p><a>One</a></p><p><a>Two</a></p>Three</div><span>Four</span><foo></foo><img>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element span = doc.Select("span").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element foo = doc.Select("foo").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element img = doc.Select("img").First();
            Elements docChildren = div.Children();
            NUnit.Framework.Assert.AreEqual(2, docChildren.Count);
            NUnit.Framework.Assert.AreEqual("<p><a>One</a></p>", docChildren[0].OuterHtml());
            NUnit.Framework.Assert.AreEqual("<p><a>Two</a></p>", docChildren[1].OuterHtml());
            NUnit.Framework.Assert.AreEqual(3, div.ChildNodes().Count);
            NUnit.Framework.Assert.AreEqual("Three", div.ChildNodes()[2].OuterHtml());
            NUnit.Framework.Assert.AreEqual(1, p.Children().Count);
            NUnit.Framework.Assert.AreEqual("One", p.Children().Text());
            NUnit.Framework.Assert.AreEqual(0, span.Children().Count);
            NUnit.Framework.Assert.AreEqual(1, span.ChildNodes().Count);
            NUnit.Framework.Assert.AreEqual("Four", span.ChildNodes()[0].OuterHtml());
            NUnit.Framework.Assert.AreEqual(0, foo.Children().Count);
            NUnit.Framework.Assert.AreEqual(0, foo.ChildNodes().Count);
            NUnit.Framework.Assert.AreEqual(0, img.Children().Count);
            NUnit.Framework.Assert.AreEqual(0, img.ChildNodes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestShadowElementsAreUpdated() {
            String html = "<div><p><a>One</a></p><p><a>Two</a></p>Three</div><span>Four</span><foo></foo><img>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            Elements els = div.Children();
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = div.ChildNodes();
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            // the two Ps
            NUnit.Framework.Assert.AreEqual(3, nodes.Count);
            // the "Three" textnode
            iText.StyledXmlParser.Jsoup.Nodes.Element p3 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("P3"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element p4 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("P4"
                );
            div.InsertChildren(1, p3);
            div.InsertChildren(3, p4);
            Elements els2 = div.Children();
            // first els should not have changed
            NUnit.Framework.Assert.AreEqual(2, els.Count);
            NUnit.Framework.Assert.AreEqual(4, els2.Count);
            NUnit.Framework.Assert.AreEqual("<p><a>One</a></p>\n" + "<p>P3</p>\n" + "<p><a>Two</a></p>\n" + "<p>P4</p>Three"
                , div.Html());
            NUnit.Framework.Assert.AreEqual("P3", els2[1].Text());
            NUnit.Framework.Assert.AreEqual("P4", els2[3].Text());
            p3.After("<span>Another</span");
            Elements els3 = div.Children();
            NUnit.Framework.Assert.AreEqual(5, els3.Count);
            NUnit.Framework.Assert.AreEqual("span", els3[2].TagName());
            NUnit.Framework.Assert.AreEqual("Another", els3[2].Text());
            NUnit.Framework.Assert.AreEqual("<p><a>One</a></p>\n" + "<p>P3</p><span>Another</span>\n" + "<p><a>Two</a></p>\n"
                 + "<p>P4</p>Three", div.Html());
        }

        [NUnit.Framework.Test]
        public virtual void ClassNamesAndAttributeNameIsCaseInsensitive() {
            String html = "<p Class='SomeText AnotherText'>One</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.AreEqual("SomeText AnotherText", p.ClassName());
            NUnit.Framework.Assert.IsTrue(p.ClassNames().Contains("SomeText"));
            NUnit.Framework.Assert.IsTrue(p.ClassNames().Contains("AnotherText"));
            NUnit.Framework.Assert.IsTrue(p.HasClass("SomeText"));
            NUnit.Framework.Assert.IsTrue(p.HasClass("sometext"));
            NUnit.Framework.Assert.IsTrue(p.HasClass("AnotherText"));
            NUnit.Framework.Assert.IsTrue(p.HasClass("anothertext"));
            iText.StyledXmlParser.Jsoup.Nodes.Element p1 = doc.Select(".SomeText").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p2 = doc.Select(".sometext").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p3 = doc.Select("[class=SomeText AnotherText]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p4 = doc.Select("[Class=SomeText AnotherText]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p5 = doc.Select("[class=sometext anothertext]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p6 = doc.Select("[class=SomeText AnotherText]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p7 = doc.Select("[class^=sometext]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p8 = doc.Select("[class$=nothertext]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p9 = doc.Select("[class^=sometext]").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p10 = doc.Select("[class$=AnotherText]").First();
            NUnit.Framework.Assert.AreEqual("One", p1.Text());
            NUnit.Framework.Assert.AreEqual(p1, p2);
            NUnit.Framework.Assert.AreEqual(p1, p3);
            NUnit.Framework.Assert.AreEqual(p1, p4);
            NUnit.Framework.Assert.AreEqual(p1, p5);
            NUnit.Framework.Assert.AreEqual(p1, p6);
            NUnit.Framework.Assert.AreEqual(p1, p7);
            NUnit.Framework.Assert.AreEqual(p1, p8);
            NUnit.Framework.Assert.AreEqual(p1, p9);
            NUnit.Framework.Assert.AreEqual(p1, p10);
        }

        [NUnit.Framework.Test]
        public virtual void TestAppendTo() {
            String parentHtml = "<div class='a'></div>";
            String childHtml = "<div class='b'></div><p>Two</p>";
            Document parentDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(parentHtml);
            iText.StyledXmlParser.Jsoup.Nodes.Element parent = parentDoc.Body();
            Document childDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(childHtml);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = childDoc.Select("div").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element p = childDoc.Select("p").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element appendTo1 = div.AppendTo(parent);
            NUnit.Framework.Assert.AreEqual(div, appendTo1);
            iText.StyledXmlParser.Jsoup.Nodes.Element appendTo2 = p.AppendTo(div);
            NUnit.Framework.Assert.AreEqual(p, appendTo2);
            NUnit.Framework.Assert.AreEqual("<div class=\"a\"></div>\n<div class=\"b\">\n <p>Two</p>\n</div>", parentDoc
                .Body().Html());
            NUnit.Framework.Assert.AreEqual("", childDoc.Body().Html());
        }

        // got moved out
        [NUnit.Framework.Test]
        public virtual void TestNormalizesNbspInText() {
            String escaped = "You can't always get what you&nbsp;want.";
            String withNbsp = "You can't always get what you want.";
            // there is an nbsp char in there
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>" + escaped);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.AreEqual("You can't always get what you want.", p.Text());
            // text is normalized
            NUnit.Framework.Assert.AreEqual("<p>" + escaped + "</p>", p.OuterHtml());
            // html / whole text keeps &nbsp;
            NUnit.Framework.Assert.AreEqual(withNbsp, p.TextNodes()[0].GetWholeText());
            NUnit.Framework.Assert.AreEqual(160, withNbsp[29]);
            iText.StyledXmlParser.Jsoup.Nodes.Element matched = doc.Select("p:contains(get what you want)").First();
            NUnit.Framework.Assert.AreEqual("p", matched.NodeName());
            NUnit.Framework.Assert.IsTrue(matched.Is(":containsOwn(get what you want)"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalizesInvisiblesInText() {
            String escaped = "This&shy;is&#x200b;one&shy;long&shy;word";
            String decoded = "This\u00ADis\u200Bone\u00ADlong\u00ADword";
            // browser would not display those soft hyphens / other chars, so we don't want them in the text
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>" + escaped);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            doc.OutputSettings().Charset("ascii");
            // so that the outer html is easier to see with escaped invisibles
            NUnit.Framework.Assert.AreEqual("Thisisonelongword", p.Text());
            // text is normalized
            NUnit.Framework.Assert.AreEqual("<p>" + escaped + "</p>", p.OuterHtml());
            // html / whole text keeps &shy etc;
            NUnit.Framework.Assert.AreEqual(decoded, p.TextNodes()[0].GetWholeText());
            iText.StyledXmlParser.Jsoup.Nodes.Element matched = doc.Select("p:contains(Thisisonelongword)").First();
            // really just oneloneword, no invisibles
            NUnit.Framework.Assert.AreEqual("p", matched.NodeName());
            NUnit.Framework.Assert.IsTrue(matched.Is(":containsOwn(Thisisonelongword)"));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveBeforeIndex() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><body><div><p>before1</p><p>before2</p><p>XXX</p><p>after1</p><p>after2</p></div></body></html>"
                , "");
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Select("body").First();
            Elements elems = body.Select("p:matchesOwn(XXX)");
            iText.StyledXmlParser.Jsoup.Nodes.Element xElem = elems.First();
            Elements beforeX = ((iText.StyledXmlParser.Jsoup.Nodes.Element)xElem.Parent()).GetElementsByIndexLessThan(
                xElem.ElementSiblingIndex());
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element p in beforeX) {
                p.Remove();
            }
            NUnit.Framework.Assert.AreEqual("<body><div><p>XXX</p><p>after1</p><p>after2</p></div></body>", TextUtil.StripNewlines
                (body.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveAfterIndex() {
            Document doc2 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><body><div><p>before1</p><p>before2</p><p>XXX</p><p>after1</p><p>after2</p></div></body></html>"
                , "");
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc2.Select("body").First();
            Elements elems = body.Select("p:matchesOwn(XXX)");
            iText.StyledXmlParser.Jsoup.Nodes.Element xElem = elems.First();
            Elements afterX = ((iText.StyledXmlParser.Jsoup.Nodes.Element)xElem.Parent()).GetElementsByIndexGreaterThan
                (xElem.ElementSiblingIndex());
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element p in afterX) {
                p.Remove();
            }
            NUnit.Framework.Assert.AreEqual("<body><div><p>before1</p><p>before2</p><p>XXX</p></div></body>", TextUtil
                .StripNewlines(body.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void WhiteSpaceClassElement() {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("a");
            Attributes attribs = new Attributes();
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, "", attribs
                );
            attribs.Put("class", "abc ");
            bool hasClass = el.HasClass("ab");
            NUnit.Framework.Assert.IsFalse(hasClass);
        }

        [NUnit.Framework.Test]
        public virtual void TestNextElementSiblingAfterClone() {
            // via https://github.com/jhy/jsoup/issues/951
            String html = "<!DOCTYPE html><html lang=\"en\"><head></head><body><div>Initial element</div></body></html>";
            String expectedText = "New element";
            String cloneExpect = "New element in clone";
            Document original = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Document clone = (Document)original.Clone();
            iText.StyledXmlParser.Jsoup.Nodes.Element originalElement = original.Body().Child(0);
            originalElement.After("<div>" + expectedText + "</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element originalNextElementSibling = originalElement.NextElementSibling(
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element originalNextSibling = (iText.StyledXmlParser.Jsoup.Nodes.Element
                )originalElement.NextSibling();
            NUnit.Framework.Assert.AreEqual(expectedText, originalNextElementSibling.Text());
            NUnit.Framework.Assert.AreEqual(expectedText, originalNextSibling.Text());
            iText.StyledXmlParser.Jsoup.Nodes.Element cloneElement = clone.Body().Child(0);
            cloneElement.After("<div>" + cloneExpect + "</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element cloneNextElementSibling = cloneElement.NextElementSibling();
            iText.StyledXmlParser.Jsoup.Nodes.Element cloneNextSibling = (iText.StyledXmlParser.Jsoup.Nodes.Element)cloneElement
                .NextSibling();
            NUnit.Framework.Assert.AreEqual(cloneExpect, cloneNextElementSibling.Text());
            NUnit.Framework.Assert.AreEqual(cloneExpect, cloneNextSibling.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestRemovingEmptyClassAttributeWhenLastClassRemoved() {
            // https://github.com/jhy/jsoup/issues/947
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<img class=\"one two\" />");
            iText.StyledXmlParser.Jsoup.Nodes.Element img = doc.Select("img").First();
            img.RemoveClass("one");
            img.RemoveClass("two");
            NUnit.Framework.Assert.IsFalse(doc.Body().Html().Contains("class=\"\""));
        }

        [NUnit.Framework.Test]
        public virtual void BooleanAttributeOutput() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<img src=foo noshade='' nohref async=async autofocus=false>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element img = doc.SelectFirst("img");
            NUnit.Framework.Assert.AreEqual("<img src=\"foo\" noshade nohref async autofocus=\"false\">", img.OuterHtml
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TextHasSpaceAfterBlockTags() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>One</div>Two");
            NUnit.Framework.Assert.AreEqual("One Two", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TextHasSpaceBetweenDivAndCenterTags() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>One</div><div>Two</div><center>Three</center><center>Four</center>"
                );
            NUnit.Framework.Assert.AreEqual("One Two Three Four", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestNextElementSiblings() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<ul id='ul'>" + "<li id='a'>a</li>" + "<li id='b'>b</li>"
                 + "<li id='c'>c</li>" + "</ul> Not An Element but a node" + "<div id='div'>" + "<li id='d'>d</li>" + 
                "</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element element = doc.GetElementById("a");
            Elements elementSiblings = element.NextElementSiblings();
            NUnit.Framework.Assert.IsNotNull(elementSiblings);
            NUnit.Framework.Assert.AreEqual(2, elementSiblings.Count);
            NUnit.Framework.Assert.AreEqual("b", elementSiblings[0].Id());
            NUnit.Framework.Assert.AreEqual("c", elementSiblings[1].Id());
            iText.StyledXmlParser.Jsoup.Nodes.Element element1 = doc.GetElementById("b");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings1 = element1.NextElementSiblings();
            NUnit.Framework.Assert.IsNotNull(elementSiblings1);
            NUnit.Framework.Assert.AreEqual(1, elementSiblings1.Count);
            NUnit.Framework.Assert.AreEqual("c", elementSiblings1[0].Id());
            iText.StyledXmlParser.Jsoup.Nodes.Element element2 = doc.GetElementById("c");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings2 = element2.NextElementSiblings();
            NUnit.Framework.Assert.AreEqual(0, elementSiblings2.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element ul = doc.GetElementById("ul");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings3 = ul.NextElementSiblings();
            NUnit.Framework.Assert.IsNotNull(elementSiblings3);
            NUnit.Framework.Assert.AreEqual(1, elementSiblings3.Count);
            NUnit.Framework.Assert.AreEqual("div", elementSiblings3[0].Id());
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("div");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings4 = div.NextElementSiblings();
            NUnit.Framework.Assert.AreEqual(0, elementSiblings4.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestPreviousElementSiblings() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<ul id='ul'>" + "<li id='a'>a</li>" + "<li id='b'>b</li>"
                 + "<li id='c'>c</li>" + "</ul>" + "<div id='div'>" + "<li id='d'>d</li>" + "</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element element = doc.GetElementById("b");
            Elements elementSiblings = element.PreviousElementSiblings();
            NUnit.Framework.Assert.IsNotNull(elementSiblings);
            NUnit.Framework.Assert.AreEqual(1, elementSiblings.Count);
            NUnit.Framework.Assert.AreEqual("a", elementSiblings[0].Id());
            iText.StyledXmlParser.Jsoup.Nodes.Element element1 = doc.GetElementById("a");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings1 = element1.PreviousElementSiblings();
            NUnit.Framework.Assert.AreEqual(0, elementSiblings1.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element element2 = doc.GetElementById("c");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings2 = element2.PreviousElementSiblings();
            NUnit.Framework.Assert.IsNotNull(elementSiblings2);
            NUnit.Framework.Assert.AreEqual(2, elementSiblings2.Count);
            NUnit.Framework.Assert.AreEqual("b", elementSiblings2[0].Id());
            NUnit.Framework.Assert.AreEqual("a", elementSiblings2[1].Id());
            iText.StyledXmlParser.Jsoup.Nodes.Element ul = doc.GetElementById("ul");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elementSiblings3 = ul.PreviousElementSiblings();
            NUnit.Framework.Assert.AreEqual(0, elementSiblings3.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestClearAttributes() {
            iText.StyledXmlParser.Jsoup.Nodes.Element el = ((iText.StyledXmlParser.Jsoup.Nodes.Element)new iText.StyledXmlParser.Jsoup.Nodes.Element
                ("a").Attr("href", "http://example.com")).Text("Hello");
            NUnit.Framework.Assert.AreEqual("<a href=\"http://example.com\">Hello</a>", el.OuterHtml());
            iText.StyledXmlParser.Jsoup.Nodes.Element el2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)el.ClearAttributes
                ();
            // really just force testing the return type is Element
            NUnit.Framework.Assert.AreSame(el, el2);
            NUnit.Framework.Assert.AreEqual("<a>Hello</a>", el2.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveAttr() {
            iText.StyledXmlParser.Jsoup.Nodes.Element el = ((iText.StyledXmlParser.Jsoup.Nodes.Element)new iText.StyledXmlParser.Jsoup.Nodes.Element
                ("a").Attr("href", "http://example.com").Attr("id", "1")).Text("Hello");
            NUnit.Framework.Assert.AreEqual("<a href=\"http://example.com\" id=\"1\">Hello</a>", el.OuterHtml());
            iText.StyledXmlParser.Jsoup.Nodes.Element el2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)el.RemoveAttr("href"
                );
            // really just force testing the return type is Element
            NUnit.Framework.Assert.AreSame(el, el2);
            NUnit.Framework.Assert.AreEqual("<a id=\"1\">Hello</a>", el2.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestRoot() {
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element("a");
            el.Append("<span>Hello</span>");
            NUnit.Framework.Assert.AreEqual("<a><span>Hello</span></a>", el.OuterHtml());
            iText.StyledXmlParser.Jsoup.Nodes.Element span = el.SelectFirst("span");
            NUnit.Framework.Assert.IsNotNull(span);
            iText.StyledXmlParser.Jsoup.Nodes.Element el2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)span.Root();
            NUnit.Framework.Assert.AreSame(el, el2);
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            NUnit.Framework.Assert.AreSame(doc, div.Root());
            NUnit.Framework.Assert.AreSame(doc, div.OwnerDocument());
        }

        [NUnit.Framework.Test]
        public virtual void TestTraverse() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            AtomicLong counter = new AtomicLong(0);
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)div.Traverse(new 
                _NodeVisitor_1765(counter));
            NUnit.Framework.Assert.AreEqual(7, counter.Get());
            NUnit.Framework.Assert.AreEqual(div2, div);
        }

        private sealed class _NodeVisitor_1765 : NodeVisitor {
            public _NodeVisitor_1765(AtomicLong counter) {
                this.counter = counter;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                counter.IncrementAndGet();
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
            }

            private readonly AtomicLong counter;
        }

        [NUnit.Framework.Test]
        public virtual void VoidTestFilterCallReturnsElement() {
            // doesn't actually test the filter so much as the return type for Element. See node.nodeFilter for an acutal test
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = (iText.StyledXmlParser.Jsoup.Nodes.Element)div.Filter(new 
                _NodeFilter_1787());
            NUnit.Framework.Assert.AreSame(div, div2);
        }

        private sealed class _NodeFilter_1787 : NodeFilter {
            public _NodeFilter_1787() {
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                return NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                return NodeFilter.FilterResult.CONTINUE;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoesntDeleteZWJWhenNormalizingText() {
            String text = "\uD83D\uDC69\u200D\uD83D\uDCBB\uD83E\uDD26\uD83C\uDFFB\u200D\u2642\uFE0F";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>" + text + "</p><div>One&zwj;Two</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element d = doc.SelectFirst("div");
            NUnit.Framework.Assert.AreEqual(12, p.Text().Length);
            NUnit.Framework.Assert.AreEqual(text, p.Text());
            NUnit.Framework.Assert.AreEqual(7, d.Text().Length);
            NUnit.Framework.Assert.AreEqual("One\u200DTwo", d.Text());
            iText.StyledXmlParser.Jsoup.Nodes.Element found = doc.SelectFirst("div:contains(One\u200DTwo)");
            NUnit.Framework.Assert.IsTrue(found.HasSameValue(d));
        }

        [NUnit.Framework.Test]
        public virtual void TestReparentSeperateNodes() {
            String html = "<div><p>One<p>Two";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element new1 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("Three"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element new2 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("Four"
                );
            doc.Body().InsertChildren(-1, new1, new2);
            NUnit.Framework.Assert.AreEqual("<div><p>One</p><p>Two</p></div><p>Three</p><p>Four</p>", TextUtil.StripNewlines
                (doc.Body().Html()));
            // note that these get moved from the above - as not copied
            doc.Body().InsertChildren(0, new1, new2);
            NUnit.Framework.Assert.AreEqual("<p>Three</p><p>Four</p><div><p>One</p><p>Two</p></div>", TextUtil.StripNewlines
                (doc.Body().Html()));
            doc.Body().InsertChildren(0, (iText.StyledXmlParser.Jsoup.Nodes.Node)new2.Clone(), (iText.StyledXmlParser.Jsoup.Nodes.Node
                )new1.Clone());
            NUnit.Framework.Assert.AreEqual("<p>Four</p><p>Three</p><p>Three</p><p>Four</p><div><p>One</p><p>Two</p></div>"
                , TextUtil.StripNewlines(doc.Body().Html()));
            // shifted to end
            doc.Body().AppendChild(new1);
            NUnit.Framework.Assert.AreEqual("<p>Four</p><p>Three</p><p>Four</p><div><p>One</p><p>Two</p></div><p>Three</p>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotActuallyAReparent() {
            // prep
            String html = "<div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element new1 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("One"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element new2 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("Two"
                );
            div.AddChildren(new1, new2);
            NUnit.Framework.Assert.AreEqual("<div><p>One</p><p>Two</p></div>", TextUtil.StripNewlines(div.OuterHtml())
                );
            // and the issue setup:
            iText.StyledXmlParser.Jsoup.Nodes.Element new3 = new iText.StyledXmlParser.Jsoup.Nodes.Element("p").Text("Three"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element wrap = new iText.StyledXmlParser.Jsoup.Nodes.Element("nav");
            wrap.AddChildren(0, new1, new3);
            NUnit.Framework.Assert.AreEqual("<nav><p>One</p><p>Three</p></nav>", TextUtil.StripNewlines(wrap.OuterHtml
                ()));
            div.AddChildren(wrap);
            // now should be that One moved into wrap, leaving Two in div.
            NUnit.Framework.Assert.AreEqual("<div><p>Two</p><nav><p>One</p><p>Three</p></nav></div>", TextUtil.StripNewlines
                (div.OuterHtml()));
            NUnit.Framework.Assert.AreEqual("<div><p>Two</p><nav><p>One</p><p>Three</p></nav></div>", TextUtil.StripNewlines
                (div.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void TestChildSizeWithMixedContent() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table><tbody>\n<tr>\n<td>15:00</td>\n<td>sport</td>\n</tr>\n</tbody></table>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element row = doc.SelectFirst("table tbody tr");
            NUnit.Framework.Assert.AreEqual(2, row.ChildrenSize());
            NUnit.Framework.Assert.AreEqual(5, row.ChildNodeSize());
        }

        [NUnit.Framework.Test]
        public virtual void IsBlock() {
            String html = "<div><p><span>Hello</span>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.IsTrue(doc.SelectFirst("div").IsBlock());
            NUnit.Framework.Assert.IsTrue(doc.SelectFirst("p").IsBlock());
            NUnit.Framework.Assert.IsFalse(doc.SelectFirst("span").IsBlock());
        }

        [NUnit.Framework.Test]
        public virtual void TestScriptTextHtmlSetAsData() {
            String src = "var foo = 5 < 2;\nvar bar = 1 && 2;";
            String html = "<script>" + src + "</script>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.SelectFirst("script");
            NUnit.Framework.Assert.IsNotNull(el);
            ValidateScriptContents(src, el);
            src = "var foo = 4 < 2;\nvar bar > 1 && 2;";
            el.Html(src);
            ValidateScriptContents(src, el);
            // special case for .text (in HTML; in XML will just be regular text)
            el.Text(src);
            ValidateScriptContents(src, el);
            // XML, no special treatment, get escaped correctly
            Document xml = iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser().ParseInput(html, "");
            iText.StyledXmlParser.Jsoup.Nodes.Element xEl = xml.SelectFirst("script");
            NUnit.Framework.Assert.IsNotNull(xEl);
            src = "var foo = 5 < 2;\nvar bar = 1 && 2;";
            String escaped = "var foo = 5 &lt; 2;\nvar bar = 1 &amp;&amp; 2;";
            ValidateXmlScriptContents(xEl);
            xEl.Text(src);
            ValidateXmlScriptContents(xEl);
            xEl.Html(src);
            ValidateXmlScriptContents(xEl);
            NUnit.Framework.Assert.AreEqual("<script>var foo = 4 < 2;\nvar bar > 1 && 2;</script>", el.OuterHtml());
            NUnit.Framework.Assert.AreEqual("<script>" + escaped + "</script>", xEl.OuterHtml());
        }

        // escaped in xml as no special treatment
        [NUnit.Framework.Test]
        public virtual void TestShallowCloneToString() {
            // https://github.com/jhy/jsoup/issues/1410
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p><i>Hello</i></p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element i = doc.SelectFirst("i");
            String pH = p.ShallowClone().ToString();
            String iH = i.ShallowClone().ToString();
            NUnit.Framework.Assert.AreEqual("<p></p>", pH);
            // shallow, so no I
            NUnit.Framework.Assert.AreEqual("<i></i>", iH);
            NUnit.Framework.Assert.AreEqual(p.OuterHtml(), p.ToString());
            NUnit.Framework.Assert.AreEqual(i.OuterHtml(), i.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void StyleHtmlRoundTrips() {
            String styleContents = "foo < bar > qux {color:white;}";
            String html = "<head><style>" + styleContents + "</style></head>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element head = doc.Head();
            iText.StyledXmlParser.Jsoup.Nodes.Element style = head.SelectFirst("style");
            NUnit.Framework.Assert.IsNotNull(style);
            NUnit.Framework.Assert.AreEqual(styleContents, style.Html());
            style.Html(styleContents);
            NUnit.Framework.Assert.AreEqual(styleContents, style.Html());
            NUnit.Framework.Assert.AreEqual("", style.Text());
            style.Text(styleContents);
            // pushes the HTML, not the Text
            NUnit.Framework.Assert.AreEqual("", style.Text());
            NUnit.Framework.Assert.AreEqual(styleContents, style.Html());
        }

        [NUnit.Framework.Test]
        public virtual void MoveChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div><div></div>");
            Elements divs = doc.Select("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = divs[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element b = divs[1];
            b.InsertChildren(-1, a.ChildNodes());
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div>\n <p>One</p>\n <p>Two</p>\n <p>Three</p>\n</div>", doc
                .Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void MoveChildrenToOuter() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div><div></div>");
            Elements divs = doc.Select("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = divs[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element b = doc.Body();
            b.InsertChildren(-1, a.ChildNodes());
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div></div>\n<p>One</p>\n<p>Two</p>\n<p>Three</p>", doc.Body
                ().Html());
        }

        [NUnit.Framework.Test]
        public virtual void AppendChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div><div><p>Four</div>"
                );
            Elements divs = doc.Select("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = divs[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element b = divs[1];
            b.AppendChildren(a.ChildNodes());
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div>\n <p>Four</p>\n <p>One</p>\n <p>Two</p>\n <p>Three</p>\n</div>"
                , doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void PrependChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div><div><p>Four</div>"
                );
            Elements divs = doc.Select("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = divs[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element b = divs[1];
            b.PrependChildren(a.ChildNodes());
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div>\n <p>One</p>\n <p>Two</p>\n <p>Three</p>\n <p>Four</p>\n</div>"
                , doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void LoopMoveChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div><div><p>Four</div>"
                );
            Elements divs = doc.Select("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = divs[0];
            iText.StyledXmlParser.Jsoup.Nodes.Element b = divs[1];
            iText.StyledXmlParser.Jsoup.Nodes.Element outer = (iText.StyledXmlParser.Jsoup.Nodes.Element)b.Parent();
            NUnit.Framework.Assert.IsNotNull(outer);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in a.ChildNodes()) {
                outer.AppendChild(node);
            }
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div>\n <p>Four</p>\n</div>\n<p>One</p>\n<p>Two</p>\n<p>Three</p>"
                , doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void AccessorsDoNotVivifyAttributes() {
            // internally, we don't want to create empty Attribute objects unless actually used for something
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p><a href=foo>One</a>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.SelectFirst("div");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.SelectFirst("a");
            // should not create attributes
            NUnit.Framework.Assert.AreEqual("", div.Attr("href"));
            p.RemoveAttr("href");
            Elements hrefs = doc.Select("[href]");
            NUnit.Framework.Assert.AreEqual(1, hrefs.Count);
            NUnit.Framework.Assert.IsFalse(div.HasAttributes());
            NUnit.Framework.Assert.IsFalse(p.HasAttributes());
            NUnit.Framework.Assert.IsTrue(a.HasAttributes());
        }

        [NUnit.Framework.Test]
        public virtual void ChildNodesAccessorDoesNotVivify() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p></p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            NUnit.Framework.Assert.IsFalse(p.HasChildNodes());
            NUnit.Framework.Assert.AreEqual(0, p.ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(0, p.ChildrenSize());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = p.ChildNodes();
            NUnit.Framework.Assert.AreEqual(0, childNodes.Count);
            Elements children = p.Children();
            NUnit.Framework.Assert.AreEqual(0, children.Count);
            NUnit.Framework.Assert.IsFalse(p.HasChildNodes());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyChildrenElementsIsModifiable() {
            // using unmodifiable empty in childElementList as short circuit, but people may be modifying Elements.
            iText.StyledXmlParser.Jsoup.Nodes.Element p = new iText.StyledXmlParser.Jsoup.Nodes.Element("p");
            Elements els = p.Children();
            NUnit.Framework.Assert.AreEqual(0, els.Count);
            els.Add(new iText.StyledXmlParser.Jsoup.Nodes.Element("a"));
            NUnit.Framework.Assert.AreEqual(1, els.Count);
        }
    }
}
