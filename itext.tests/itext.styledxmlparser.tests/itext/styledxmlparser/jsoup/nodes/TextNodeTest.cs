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
using iText.StyledXmlParser.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Test TextNodes</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class TextNodeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestBlank() {
            TextNode one = new TextNode("", "");
            TextNode two = new TextNode("     ", "");
            TextNode three = new TextNode("  \n\n   ", "");
            TextNode four = new TextNode("Hello", "");
            TextNode five = new TextNode("  \nHello ", "");
            NUnit.Framework.Assert.IsTrue(one.IsBlank());
            NUnit.Framework.Assert.IsTrue(two.IsBlank());
            NUnit.Framework.Assert.IsTrue(three.IsBlank());
            NUnit.Framework.Assert.IsFalse(four.IsBlank());
            NUnit.Framework.Assert.IsFalse(five.IsBlank());
        }

        [NUnit.Framework.Test]
        public virtual void TestTextBean() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <span>two &amp;</span> three &amp;</p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element span = doc.Select("span").First();
            NUnit.Framework.Assert.AreEqual("two &", span.Text());
            TextNode spanText = (TextNode)span.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("two &", spanText.Text());
            TextNode tn = (TextNode)p.ChildNode(2);
            NUnit.Framework.Assert.AreEqual(" three &", tn.Text());
            tn.Text(" POW!");
            NUnit.Framework.Assert.AreEqual("One <span>two &amp;</span> POW!", TextUtil.StripNewlines(p.Html()));
            tn.Attr("text", "kablam &");
            NUnit.Framework.Assert.AreEqual("kablam &", tn.Text());
            NUnit.Framework.Assert.AreEqual("One <span>two &amp;</span>kablam &amp;", TextUtil.StripNewlines(p.Html())
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestSplitText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>Hello there</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            TextNode tn = (TextNode)div.ChildNode(0);
            TextNode tail = tn.SplitText(6);
            NUnit.Framework.Assert.AreEqual("Hello ", tn.GetWholeText());
            NUnit.Framework.Assert.AreEqual("there", tail.GetWholeText());
            tail.Text("there!");
            NUnit.Framework.Assert.AreEqual("Hello there!", div.Text());
            NUnit.Framework.Assert.IsTrue(tn.Parent() == tail.Parent());
        }

        [NUnit.Framework.Test]
        public virtual void TestSplitAnEmbolden() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>Hello there</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            TextNode tn = (TextNode)div.ChildNode(0);
            TextNode tail = tn.SplitText(6);
            tail.Wrap("<b></b>");
            NUnit.Framework.Assert.AreEqual("Hello <b>there</b>", TextUtil.StripNewlines(div.Html()));
        }

        // not great that we get \n<b>there there... must correct
        [NUnit.Framework.Test]
        public virtual void TestWithSupplementaryCharacter() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(new String(iText.IO.Util.TextUtil.ToChars(135361)));
            TextNode t = doc.Body().TextNodes()[0];
            NUnit.Framework.Assert.AreEqual(new String(iText.IO.Util.TextUtil.ToChars(135361)), t.OuterHtml().Trim());
        }
    }
}
