/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    [NUnit.Framework.Category("UnitTest")]
    public class CssTest : ExtendedITextTest {
        private Document html = null;

        private static String htmlString;

        [NUnit.Framework.OneTimeSetUp]
        public static void InitClass() {
            StringBuilder sb = new StringBuilder("<html><head></head><body>");
            sb.Append("<div id='pseudo'>");
            for (int i = 1; i <= 10; i++) {
                sb.Append(MessageFormatUtil.Format("<p>{0}</p>", i));
            }
            sb.Append("</div>");
            sb.Append("<div id='type'>");
            for (int i = 1; i <= 10; i++) {
                sb.Append(MessageFormatUtil.Format("<p>{0}</p>", i));
                sb.Append(MessageFormatUtil.Format("<span>{0}</span>", i));
                sb.Append(MessageFormatUtil.Format("<em>{0}</em>", i));
                sb.Append(MessageFormatUtil.Format("<svg>{0}</svg>", i));
            }
            sb.Append("</div>");
            sb.Append("<span id='onlySpan'><br /></span>");
            sb.Append("<p class='empty'><!-- Comment only is still empty! --></p>");
            sb.Append("<div id='only'>");
            sb.Append("Some text before the <em>only</em> child in this div");
            sb.Append("</div>");
            sb.Append("</body></html>");
            htmlString = sb.ToString();
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            html = iText.StyledXmlParser.Jsoup.Jsoup.Parse(htmlString);
        }

        [NUnit.Framework.Test]
        public virtual void FirstChild() {
            Check(html.Select("#pseudo :first-child"), "1");
            Check(html.Select("html:first-child"));
        }

        [NUnit.Framework.Test]
        public virtual void LastChild() {
            Check(html.Select("#pseudo :last-child"), "10");
            Check(html.Select("html:last-child"));
        }

        [NUnit.Framework.Test]
        public virtual void NthChild_simple() {
            for (int i = 1; i <= 10; i++) {
                Check(html.Select(MessageFormatUtil.Format("#pseudo :nth-child({0})", i)), i.ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NthOfType_unknownTag() {
            for (int i = 1; i <= 10; i++) {
                Check(html.Select(MessageFormatUtil.Format("#type svg:nth-of-type({0})", i)), i.ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NthLastChild_simple() {
            for (int i = 1; i <= 10; i++) {
                Check(html.Select(MessageFormatUtil.Format("#pseudo :nth-last-child({0})", i)), (11 - i).ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NthOfType_simple() {
            for (int i = 1; i <= 10; i++) {
                Check(html.Select(MessageFormatUtil.Format("#type p:nth-of-type({0})", i)), i.ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NthLastOfType_simple() {
            for (int i = 1; i <= 10; i++) {
                Check(html.Select(MessageFormatUtil.Format("#type :nth-last-of-type({0})", i)), (11 - i).ToString(), (11 -
                     i).ToString(), (11 - i).ToString(), (11 - i).ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NthChild_advanced() {
            Check(html.Select("#pseudo :nth-child(-5)"));
            Check(html.Select("#pseudo :nth-child(odd)"), "1", "3", "5", "7", "9");
            Check(html.Select("#pseudo :nth-child(2n-1)"), "1", "3", "5", "7", "9");
            Check(html.Select("#pseudo :nth-child(2n+1)"), "1", "3", "5", "7", "9");
            Check(html.Select("#pseudo :nth-child(2n+3)"), "3", "5", "7", "9");
            Check(html.Select("#pseudo :nth-child(even)"), "2", "4", "6", "8", "10");
            Check(html.Select("#pseudo :nth-child(2n)"), "2", "4", "6", "8", "10");
            Check(html.Select("#pseudo :nth-child(3n-1)"), "2", "5", "8");
            Check(html.Select("#pseudo :nth-child(-2n+5)"), "1", "3", "5");
            Check(html.Select("#pseudo :nth-child(+5)"), "5");
        }

        [NUnit.Framework.Test]
        public virtual void NthOfType_advanced() {
            Check(html.Select("#type :nth-of-type(-5)"));
            Check(html.Select("#type p:nth-of-type(odd)"), "1", "3", "5", "7", "9");
            Check(html.Select("#type em:nth-of-type(2n-1)"), "1", "3", "5", "7", "9");
            Check(html.Select("#type p:nth-of-type(2n+1)"), "1", "3", "5", "7", "9");
            Check(html.Select("#type span:nth-of-type(2n+3)"), "3", "5", "7", "9");
            Check(html.Select("#type p:nth-of-type(even)"), "2", "4", "6", "8", "10");
            Check(html.Select("#type p:nth-of-type(2n)"), "2", "4", "6", "8", "10");
            Check(html.Select("#type p:nth-of-type(3n-1)"), "2", "5", "8");
            Check(html.Select("#type p:nth-of-type(-2n+5)"), "1", "3", "5");
            Check(html.Select("#type :nth-of-type(+5)"), "5", "5", "5", "5");
        }

        [NUnit.Framework.Test]
        public virtual void NthLastChild_advanced() {
            Check(html.Select("#pseudo :nth-last-child(-5)"));
            Check(html.Select("#pseudo :nth-last-child(odd)"), "2", "4", "6", "8", "10");
            Check(html.Select("#pseudo :nth-last-child(2n-1)"), "2", "4", "6", "8", "10");
            Check(html.Select("#pseudo :nth-last-child(2n+1)"), "2", "4", "6", "8", "10");
            Check(html.Select("#pseudo :nth-last-child(2n+3)"), "2", "4", "6", "8");
            Check(html.Select("#pseudo :nth-last-child(even)"), "1", "3", "5", "7", "9");
            Check(html.Select("#pseudo :nth-last-child(2n)"), "1", "3", "5", "7", "9");
            Check(html.Select("#pseudo :nth-last-child(3n-1)"), "3", "6", "9");
            Check(html.Select("#pseudo :nth-last-child(-2n+5)"), "6", "8", "10");
            Check(html.Select("#pseudo :nth-last-child(+5)"), "6");
        }

        [NUnit.Framework.Test]
        public virtual void NthLastOfType_advanced() {
            Check(html.Select("#type :nth-last-of-type(-5)"));
            Check(html.Select("#type p:nth-last-of-type(odd)"), "2", "4", "6", "8", "10");
            Check(html.Select("#type em:nth-last-of-type(2n-1)"), "2", "4", "6", "8", "10");
            Check(html.Select("#type p:nth-last-of-type(2n+1)"), "2", "4", "6", "8", "10");
            Check(html.Select("#type span:nth-last-of-type(2n+3)"), "2", "4", "6", "8");
            Check(html.Select("#type p:nth-last-of-type(even)"), "1", "3", "5", "7", "9");
            Check(html.Select("#type p:nth-last-of-type(2n)"), "1", "3", "5", "7", "9");
            Check(html.Select("#type p:nth-last-of-type(3n-1)"), "3", "6", "9");
            Check(html.Select("#type span:nth-last-of-type(-2n+5)"), "6", "8", "10");
            Check(html.Select("#type :nth-last-of-type(+5)"), "6", "6", "6", "6");
        }

        [NUnit.Framework.Test]
        public virtual void FirstOfType() {
            Check(html.Select("div:not(#only) :first-of-type"), "1", "1", "1", "1", "1");
        }

        [NUnit.Framework.Test]
        public virtual void LastOfType() {
            Check(html.Select("div:not(#only) :last-of-type"), "10", "10", "10", "10", "10");
        }

        [NUnit.Framework.Test]
        public virtual void Empty() {
            Elements sel = html.Select(":empty");
            NUnit.Framework.Assert.AreEqual(3, sel.Count);
            NUnit.Framework.Assert.AreEqual("head", sel[0].TagName());
            NUnit.Framework.Assert.AreEqual("br", sel[1].TagName());
            NUnit.Framework.Assert.AreEqual("p", sel[2].TagName());
        }

        [NUnit.Framework.Test]
        public virtual void OnlyChild() {
            Elements sel = html.Select("span :only-child");
            NUnit.Framework.Assert.AreEqual(1, sel.Count);
            NUnit.Framework.Assert.AreEqual("br", sel[0].TagName());
            Check(html.Select("#only :only-child"), "only");
        }

        [NUnit.Framework.Test]
        public virtual void OnlyOfType() {
            Elements sel = html.Select(":only-of-type");
            NUnit.Framework.Assert.AreEqual(6, sel.Count);
            NUnit.Framework.Assert.AreEqual("head", sel[0].TagName());
            NUnit.Framework.Assert.AreEqual("body", sel[1].TagName());
            NUnit.Framework.Assert.AreEqual("span", sel[2].TagName());
            NUnit.Framework.Assert.AreEqual("br", sel[3].TagName());
            NUnit.Framework.Assert.AreEqual("p", sel[4].TagName());
            NUnit.Framework.Assert.IsTrue(sel[4].HasClass("empty"));
            NUnit.Framework.Assert.AreEqual("em", sel[5].TagName());
        }

        protected internal virtual void Check(Elements result, params String[] expectedContent) {
            NUnit.Framework.Assert.AreEqual(expectedContent.Length, result.Count);
            for (int i = 0; i < expectedContent.Length; i++) {
                NUnit.Framework.Assert.IsNotNull(result[i]);
                NUnit.Framework.Assert.AreEqual(expectedContent[i], result[i].OwnText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void Root() {
            Elements sel = html.Select(":root");
            NUnit.Framework.Assert.AreEqual(1, sel.Count);
            NUnit.Framework.Assert.IsNotNull(sel[0]);
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("html"), sel[0].Tag());
            Elements sel2 = html.Select("body").Select(":root");
            NUnit.Framework.Assert.AreEqual(1, sel2.Count);
            NUnit.Framework.Assert.IsNotNull(sel2[0]);
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("body"), sel2[0].Tag());
        }
    }
}
