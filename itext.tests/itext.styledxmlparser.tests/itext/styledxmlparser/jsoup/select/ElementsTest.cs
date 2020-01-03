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
using System.Text;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Tests for ElementList.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
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
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a id=1 href='/foo'>One</a> <a id=2 href='http://jsoup.org'>Two</a>"
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
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a id=1 href='/foo'>One</a> <a id=2 href='http://jsoup.org/'>Two</a>"
                );
            Elements one = doc.Select("#1");
            Elements two = doc.Select("#2");
            Elements both = doc.Select("a");
            NUnit.Framework.Assert.AreEqual("", one.Attr("abs:href"));
            NUnit.Framework.Assert.AreEqual("http://jsoup.org/", two.Attr("abs:href"));
            NUnit.Framework.Assert.AreEqual("http://jsoup.org/", both.Attr("abs:href"));
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
            doc.Select("div").Traverse(new _NodeVisitor_303(accum));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p></div><div><#text></#text></div>", accum.ToString
                ());
        }

        private sealed class _NodeVisitor_303 : NodeVisitor {
            public _NodeVisitor_303(StringBuilder accum) {
                this.accum = accum;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("<" + node.NodeName() + ">");
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</" + node.NodeName() + ">");
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void Forms() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<form id=1><input name=q></form><div /><form id=2><input name=f></form>"
                );
            Elements els = doc.Select("*");
            NUnit.Framework.Assert.AreEqual(9, els.Count);
            IList<FormElement> forms = els.Forms();
            NUnit.Framework.Assert.AreEqual(2, forms.Count);
            NUnit.Framework.Assert.IsTrue(forms[0] != null);
            NUnit.Framework.Assert.IsTrue(forms[1] != null);
            NUnit.Framework.Assert.AreEqual("1", forms[0].Id());
            NUnit.Framework.Assert.AreEqual("2", forms[1].Id());
        }

        [NUnit.Framework.Test]
        public virtual void ClassWithHyphen() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p class='tab-nav'>Check</p>");
            Elements els = doc.GetElementsByClass("tab-nav");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.AreEqual("Check", els.Text());
        }
    }
}
