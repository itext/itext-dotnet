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
using System.IO;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Integration {
    /// <summary>Integration test: parses from real-world example HTML.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class ParseTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestSmhBizArticle() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/smh-biz-article-1.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8", "http://www.smh.com.au/business/the-boards-next-fear-the-female-quota-20100106-lteq.html"
                );
            NUnit.Framework.Assert.AreEqual("The board’s next fear: the female quota", doc.Title());
            // note that the apos in the source is a literal ’ (8217), not escaped or '
            NUnit.Framework.Assert.AreEqual("en", doc.Select("html").Attr("xml:lang"));
            Elements articleBody = doc.Select(".articleBody > *");
            NUnit.Framework.Assert.AreEqual(17, articleBody.Count);
        }

        // todo: more tests!
        [NUnit.Framework.Test]
        public virtual void TestNewsHomepage() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/news-com-au-home.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8", "http://www.news.com.au/");
            NUnit.Framework.Assert.AreEqual("News.com.au | News from Australia and around the world online | NewsComAu"
                , doc.Title());
            NUnit.Framework.Assert.AreEqual("Brace yourself for Metro meltdown", doc.Select(".id1225817868581 h4").Text
                ().Trim());
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a[href=/entertainment/horoscopes]").First();
            NUnit.Framework.Assert.AreEqual("/entertainment/horoscopes", a.Attr("href"));
            NUnit.Framework.Assert.AreEqual("http://www.news.com.au/entertainment/horoscopes", a.Attr("abs:href"));
            iText.StyledXmlParser.Jsoup.Nodes.Element hs = doc.Select("a[href*=naughty-corners-are-a-bad-idea]").First
                ();
            NUnit.Framework.Assert.AreEqual("http://www.heraldsun.com.au/news/naughty-corners-are-a-bad-idea-for-kids/story-e6frf7jo-1225817899003"
                , hs.Attr("href"));
            NUnit.Framework.Assert.AreEqual(hs.Attr("href"), hs.Attr("abs:href"));
        }

        [NUnit.Framework.Test]
        public virtual void TestGoogleSearchIpod() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/google-ipod.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8", "http://www.google.com/search?hl=en&q=ipod&aq=f&oq=&aqi=g10"
                );
            NUnit.Framework.Assert.AreEqual("ipod - Google Search", doc.Title());
            Elements results = doc.Select("h3.r > a");
            NUnit.Framework.Assert.AreEqual(12, results.Count);
            NUnit.Framework.Assert.AreEqual("http://news.google.com/news?hl=en&q=ipod&um=1&ie=UTF-8&ei=uYlKS4SbBoGg6gPf-5XXCw&sa=X&oi=news_group&ct=title&resnum=1&ved=0CCIQsQQwAA"
                , results[0].Attr("href"));
            NUnit.Framework.Assert.AreEqual("http://www.apple.com/itunes/", results[1].Attr("href"));
        }

        [NUnit.Framework.Test]
        public virtual void TestBinary() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/thumb.jpg");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            // nothing useful, but did not blow up
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("gd-jpeg"));
        }

        [NUnit.Framework.Test]
        public virtual void TestYahooJp() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/yahoo-jp.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8", "http://www.yahoo.co.jp/index.html");
            // http charset is utf-8.
            NUnit.Framework.Assert.AreEqual("Yahoo! JAPAN", doc.Title());
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a[href=t/2322m2]").First();
            NUnit.Framework.Assert.AreEqual("http://www.yahoo.co.jp/_ylh=X3oDMTB0NWxnaGxsBF9TAzIwNzcyOTYyNjUEdGlkAzEyBHRtcGwDZ2Ex/t/2322m2"
                , a.Attr("abs:href"));
            // session put into <base>
            NUnit.Framework.Assert.AreEqual("全国、人気の駅ランキング", a.Text());
        }

        private const String newsHref = "http://news.baidu.com/";

        [NUnit.Framework.Test]
        public virtual void TestBaidu() {
            // tests <meta http-equiv="Content-Type" content="text/html;charset=gb2312">
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/baidu-cn-home.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://www.baidu.com");
            // http charset is gb2312, but NOT specifying it, to test http-equiv parse
            iText.StyledXmlParser.Jsoup.Nodes.Element submit = doc.Select("#su").First();
            NUnit.Framework.Assert.AreEqual("百度一下", submit.Attr("value"));
            // test from attribute match
            submit = doc.Select("input[value=百度一下]").First();
            NUnit.Framework.Assert.AreEqual("su", submit.Id());
            iText.StyledXmlParser.Jsoup.Nodes.Element newsLink = doc.Select("a:contains(新)").First();
            NUnit.Framework.Assert.AreEqual(newsHref, newsLink.AbsUrl("href"));
            // check auto-detect from meta
            NUnit.Framework.Assert.AreEqual("GB2312", doc.OutputSettings().Charset().DisplayName());
            NUnit.Framework.Assert.AreEqual("<title>百度一下，你就知道      </title>", doc.Select("title").OuterHtml());
            doc.OutputSettings().Charset("ascii");
            NUnit.Framework.Assert.AreEqual("<title>&#x767e;&#x5ea6;&#x4e00;&#x4e0b;&#xff0c;&#x4f60;&#x5c31;&#x77e5;&#x9053;      </title>"
                , doc.Select("title").OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestBaiduVariant() {
            // tests <meta charset> when preceded by another <meta>
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/baidu-variant.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://www.baidu.com/");
            // http charset is gb2312, but NOT specifying it, to test http-equiv parse
            // check auto-detect from meta
            NUnit.Framework.Assert.AreEqual("GB2312", doc.OutputSettings().Charset().DisplayName());
            NUnit.Framework.Assert.AreEqual("<title>百度一下，你就知道</title>", doc.Select("title").OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestHtml5Charset() {
            // test that <meta charset="gb2312"> works
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/meta-charset-1.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com/");
            //gb2312, has html5 <meta charset>
            NUnit.Framework.Assert.AreEqual("新", doc.Text());
            NUnit.Framework.Assert.AreEqual("GB2312", doc.OutputSettings().Charset().DisplayName());
            // double check, no charset, falls back to utf8 which is incorrect
            @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/meta-charset-2.html");
            //
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            // gb2312, no charset
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().DisplayName());
            NUnit.Framework.Assert.IsFalse("新".Equals(doc.Text()));
            // confirm fallback to utf8
            @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/meta-charset-3.html");
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com/");
            // utf8, no charset
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().DisplayName());
            NUnit.Framework.Assert.AreEqual("新", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestBrokenHtml5CharsetWithASingleDoubleQuote() {
            Stream @in = InputStreamFrom("<html>\n" + "<head><meta charset=UTF-8\"></head>\n" + "<body></body>\n" + "</html>"
                );
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com/");
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().DisplayName());
        }

        [NUnit.Framework.Test]
        public virtual void TestNytArticle() {
            // has tags like <nyt_text>
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/nyt-article-1.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://www.nytimes.com/2010/07/26/business/global/26bp.html?hp"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element headline = doc.Select("nyt_headline[version=1.0]").First();
            NUnit.Framework.Assert.AreEqual("As BP Lays Out Future, It Will Not Include Hayward", headline.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestYahooArticle() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/yahoo-article-1.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8", "http://news.yahoo.com/s/nm/20100831/bs_nm/us_gm_china"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p:contains(Volt will be sold in the United States"
                ).First();
            NUnit.Framework.Assert.AreEqual("In July, GM said its electric Chevrolet Volt will be sold in the United States at $41,000 -- $8,000 more than its nearest competitor, the Nissan Leaf."
                , p.Text());
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleIntegerValueTest() {
            double? expectedString = 5.0;
            double? actualString = CssUtils.ParseDouble("5");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleManyCharsAfterDotTest() {
            double? expectedString = 5.123456789;
            double? actualString = CssUtils.ParseDouble("5.123456789");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleManyCharsAfterDotNegativeTest() {
            double? expectedString = -5.123456789;
            double? actualString = CssUtils.ParseDouble("-5.123456789");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleNullValueTest() {
            double? expectedString = null;
            double? actualString = CssUtils.ParseDouble(null);
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleNegativeTextTest() {
            double? expectedString = null;
            double? actualString = CssUtils.ParseDouble("text");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        public static Stream InputStreamFrom(String s) {
            try {
                return new MemoryStream(s.GetBytes("UTF-8"));
            }
            catch (ArgumentException e) {
                throw new Exception("Unsupported encoding", e);
            }
        }
    }
}
