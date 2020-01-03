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
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for Element (DOM stuff mostly).</summary>
    /// <author>Jonathan Hedley</author>
    public class ElementTest : ExtendedITextTest {
        private String reference = "<div id=div1><p>Hello</p><p>Another <b>element</b></p><div id=div2><img src=foo.png></div></div>";

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
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> empty = doc.GetElementsByTag("wtf");
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
        public virtual void TestBrHasSpace() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello<br>there</p>");
            NUnit.Framework.Assert.AreEqual("Hello there", doc.Text());
            NUnit.Framework.Assert.AreEqual("Hello there", doc.Select("p").First().OwnText());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello <br> there</p>");
            NUnit.Framework.Assert.AreEqual("Hello there", doc.Text());
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
            NUnit.Framework.Assert.IsTrue(0 == ps[0].ElementSiblingIndex());
            NUnit.Framework.Assert.IsTrue(1 == ps[1].ElementSiblingIndex());
            NUnit.Framework.Assert.IsTrue(2 == ps[2].ElementSiblingIndex());
        }

        [NUnit.Framework.Test]
        public virtual void TestElementSiblingIndexSameContent() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One</p>...<p>One</p>...<p>One</p>");
            Elements ps = doc.Select("p");
            NUnit.Framework.Assert.IsTrue(0 == ps[0].ElementSiblingIndex());
            NUnit.Framework.Assert.IsTrue(1 == ps[1].ElementSiblingIndex());
            NUnit.Framework.Assert.IsTrue(2 == ps[2].ElementSiblingIndex());
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
            ((iText.StyledXmlParser.Jsoup.Nodes.Element)div.AppendElement("P").Attr("class", "second")).Text("now");
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
            NUnit.Framework.Assert.AreEqual(1, attributes.Count, "There should be one attribute");
            NUnit.Framework.Assert.IsTrue(attributes[0] is BooleanAttribute, "Attribute should be boolean");
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
            NUnit.Framework.Assert.AreEqual("<div><div class=\"head\"><p>Hello</p><p>There!</p></div></div>", TextUtil
                .StripNewlines(doc.Body().Html()));
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
            NUnit.Framework.Assert.AreEqual(null, dataset.Get(""));
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
            NUnit.Framework.Assert.AreEqual(0, children.Count);
            // children is backed by div1.childNodes, moved, so should be 0 now
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
                div2.InsertChildren<iText.StyledXmlParser.Jsoup.Nodes.Node>(0, null);
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
            TextNode tn1 = new TextNode("Text4", "");
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
            NUnit.Framework.Assert.IsTrue(divA == doc.Select(divA.CssSelector()).First());
            NUnit.Framework.Assert.IsTrue(divB == doc.Select(divB.CssSelector()).First());
            NUnit.Framework.Assert.IsTrue(divC == doc.Select(divC.CssSelector()).First());
        }

        [NUnit.Framework.Test]
        public virtual void TestClassNames() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class=\"c1 c2\">C</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div")[0];
            NUnit.Framework.Assert.AreEqual("c1 c2", div.ClassName());
            ICollection<String> set1 = div.ClassNames();
            Object[] arr1 = set1.ToArray();
            NUnit.Framework.Assert.IsTrue(arr1.Length == 2);
            NUnit.Framework.Assert.AreEqual("c1", arr1[0]);
            NUnit.Framework.Assert.AreEqual("c2", arr1[1]);
            // Changes to the set should not be reflected in the Elements getters
            set1.Add("c3");
            NUnit.Framework.Assert.IsTrue(2 == div.ClassNames().Count);
            NUnit.Framework.Assert.AreEqual("c1 c2", div.ClassName());
            // Update the class names to a fresh set
            ICollection<String> newSet = new LinkedHashSet<String>(set1);
            newSet.Add("c3");
            div.ClassNames(newSet);
            NUnit.Framework.Assert.AreEqual("c1 c2 c3", div.ClassName());
            ICollection<String> set2 = div.ClassNames();
            Object[] arr2 = set2.ToArray();
            NUnit.Framework.Assert.IsTrue(arr2.Length == 3);
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
            NUnit.Framework.Assert.IsFalse(e0.Equals(e2));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e2));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e3));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e6));
            NUnit.Framework.Assert.IsFalse(e0.HasSameValue(e7));
            NUnit.Framework.Assert.AreEqual(e0.GetHashCode(), e0.GetHashCode());
            NUnit.Framework.Assert.IsFalse(e0.GetHashCode() == (e2.GetHashCode()));
            NUnit.Framework.Assert.IsFalse(e0.GetHashCode() == (e3).GetHashCode());
            NUnit.Framework.Assert.IsFalse(e0.GetHashCode() == (e6).GetHashCode());
            NUnit.Framework.Assert.IsFalse(e0.GetHashCode() == (e7).GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void TestRelativeUrls() {
            String html = "<body><a href='./one.html'>One</a> <a href='two.html'>two</a> <a href='../three.html'>Three</a> <a href='//example2.com/four/'>Four</a> <a href='https://example2.com/five/'>Five</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "http://example.com/bar/");
            Elements els = doc.Select("a");
            NUnit.Framework.Assert.AreEqual("http://example.com/bar/one.html", els[0].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example.com/bar/two.html", els[1].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example.com/three.html", els[2].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://example2.com/four/", els[3].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("https://example2.com/five/", els[4].AbsUrl("href"));
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
            String result = iText.IO.Util.StringUtil.ReplaceAll(doc.ToString(), "\\s+", "");
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
    }
}
