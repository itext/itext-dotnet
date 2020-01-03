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
