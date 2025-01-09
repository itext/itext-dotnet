/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Test TextNodes</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class TextNodeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestBlank() {
            TextNode one = new TextNode("");
            TextNode two = new TextNode("     ");
            TextNode three = new TextNode("  \n\n   ");
            TextNode four = new TextNode("Hello");
            TextNode five = new TextNode("  \nHello ");
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
            tn.Attr(tn.NodeName(), "kablam &");
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
            NUnit.Framework.Assert.AreSame(tn.Parent(), tail.Parent());
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

        [NUnit.Framework.Test]
        public virtual void TestLeadNodesHaveNoChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>Hello there</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.Select("div").First();
            TextNode tn = (TextNode)div.ChildNode(0);
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = tn.ChildNodes();
            NUnit.Framework.Assert.AreEqual(0, nodes.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestSpaceNormalise() {
            // https://github.com/jhy/jsoup/issues/1309
            String whole = "Two  spaces";
            String norm = "Two spaces";
            TextNode tn = new TextNode(whole);
            // there are 2 spaces between the words
            NUnit.Framework.Assert.AreEqual(whole, tn.GetWholeText());
            NUnit.Framework.Assert.AreEqual(norm, tn.Text());
            NUnit.Framework.Assert.AreEqual(norm, tn.OuterHtml());
            NUnit.Framework.Assert.AreEqual(norm, tn.ToString());
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element("p");
            el.AppendChild(tn);
            // this used to change the context
            //tn.setParentNode(el); // set any parent
            NUnit.Framework.Assert.AreEqual(whole, tn.GetWholeText());
            NUnit.Framework.Assert.AreEqual(norm, tn.Text());
            NUnit.Framework.Assert.AreEqual(norm, tn.OuterHtml());
            NUnit.Framework.Assert.AreEqual(norm, tn.ToString());
            NUnit.Framework.Assert.AreEqual("<p>" + norm + "</p>", el.OuterHtml());
            NUnit.Framework.Assert.AreEqual(norm, el.Html());
            NUnit.Framework.Assert.AreEqual(whole, el.WholeText());
        }

        [NUnit.Framework.Test]
        public virtual void TestClone() {
            // https://github.com/jhy/jsoup/issues/1176
            TextNode x = new TextNode("zzz");
            TextNode y = (TextNode)x.Clone();
            NUnit.Framework.Assert.AreNotSame(x, y);
            NUnit.Framework.Assert.AreEqual(x.OuterHtml(), y.OuterHtml());
            y.Text("yyy");
            NUnit.Framework.Assert.AreNotEqual(x.OuterHtml(), y.OuterHtml());
            NUnit.Framework.Assert.AreEqual("zzz", x.Text());
            x.Attributes();
            // already cloned so no impact
            y.Text("xxx");
            NUnit.Framework.Assert.AreEqual("zzz", x.Text());
            NUnit.Framework.Assert.AreEqual("xxx", y.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestCloneAfterAttributesHit() {
            // https://github.com/jhy/jsoup/issues/1176
            TextNode x = new TextNode("zzz");
            x.Attributes();
            // moves content from leafnode value to attributes, which were missed in clone
            TextNode y = (TextNode)x.Clone();
            y.Text("xxx");
            NUnit.Framework.Assert.AreEqual("zzz", x.Text());
            NUnit.Framework.Assert.AreEqual("xxx", y.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestHasTextWhenIterating() {
            // https://github.com/jhy/jsoup/issues/1170
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>One <p>Two <p>Three");
            bool foundFirst = false;
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in doc.GetAllElements()) {
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in el.ChildNodes()) {
                    if (node is TextNode) {
                        TextNode textNode = (TextNode)node;
                        NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsBlank(textNode.Text()));
                        if (!foundFirst) {
                            foundFirst = true;
                            NUnit.Framework.Assert.AreEqual("One ", textNode.Text());
                            NUnit.Framework.Assert.AreEqual("One ", textNode.GetWholeText());
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsTrue(foundFirst);
        }
    }
}
