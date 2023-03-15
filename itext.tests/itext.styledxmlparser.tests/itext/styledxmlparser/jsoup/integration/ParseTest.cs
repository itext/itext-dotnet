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
using System.IO;
using System.IO.Compression;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Parser;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;
using NUnit.Framework;

namespace iText.StyledXmlParser.Jsoup.Integration {
    /// <summary>Integration test: parses from real-world example HTML.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class ParseTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/jsoup";
        
            [NUnit.Framework.Test]
    public void TestSmhBizArticle() {
        FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/smh-biz-article-1.html.gz");
        Document doc = Jsoup.Parse(@in, "UTF-8",
                "http://www.smh.com.au/business/the-boards-next-fear-the-female-quota-20100106-lteq.html");
        NUnit.Framework.Assert.AreEqual("The board’s next fear: the female quota",
                doc.Title()); // note that the apos in the source is a literal ’ (8217), not escaped or '
        NUnit.Framework.Assert.AreEqual("en", doc.Select("html").Attr("xml:lang"));

        Elements articleBody = doc.Select(".articleBody > *");
        NUnit.Framework.Assert.AreEqual(17, articleBody.Count);
    }

    [NUnit.Framework.Test]
    public void TestNewsHomepage() {
        FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/news-com-au-home.html.gz");
        Document doc = Jsoup.Parse(@in, "UTF-8", "http://www.news.com.au/");
        NUnit.Framework.Assert.AreEqual("News.com.au | News from Australia and around the world online | NewsComAu", doc.Title());
        NUnit.Framework.Assert.AreEqual("Brace yourself for Metro meltdown", doc.Select(".id1225817868581 h4").Text().Trim());

        Element a = doc.Select("a[href=/entertainment/horoscopes]").First();
        NUnit.Framework.Assert.AreEqual("/entertainment/horoscopes", a.Attr("href"));
        NUnit.Framework.Assert.AreEqual("http://www.news.com.au/entertainment/horoscopes", a.Attr("abs:href"));

        Element hs = doc.Select("a[href*=naughty-corners-are-a-bad-idea]").First();
        NUnit.Framework.Assert.AreEqual(
                "http://www.heraldsun.com.au/news/naughty-corners-are-a-bad-idea-for-kids/story-e6frf7jo-1225817899003",
                hs.Attr("href"));
        NUnit.Framework.Assert.AreEqual(hs.Attr("href"), hs.Attr("abs:href"));
    }

    [NUnit.Framework.Test]
    public void TestGoogleSearchIpod() {
        FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/google-ipod.html.gz");
        Document doc = Jsoup.Parse(@in, "UTF-8", "http://www.google.com/search?hl=en&q=ipod&aq=f&oq=&aqi=g10");
        NUnit.Framework.Assert.AreEqual("ipod - Google Search", doc.Title());
        Elements results = doc.Select("h3.r > a");
        NUnit.Framework.Assert.AreEqual(12, results.Count);
        NUnit.Framework.Assert.AreEqual(
                "http://news.google.com/news?hl=en&q=ipod&um=1&ie=UTF-8&ei=uYlKS4SbBoGg6gPf-5XXCw&sa=X&oi=news_group&ct=title&resnum=1&ved=0CCIQsQQwAA",
                results[0].Attr("href"));
        NUnit.Framework.Assert.AreEqual("http://www.apple.com/itunes/",
                results[1].Attr("href"));
    }

    [NUnit.Framework.Test]
    public void TestYahooJp() {
        FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/yahoo-jp.html.gz");
        Document doc = Jsoup.Parse(@in, "UTF-8", "http://www.yahoo.co.jp/index.html"); // http charset is utf-8.
        NUnit.Framework.Assert.AreEqual("Yahoo! JAPAN", doc.Title());
        Element a = doc.Select("a[href=t/2322m2]").First();
        NUnit.Framework.Assert.AreEqual("http://www.yahoo.co.jp/_ylh=X3oDMTB0NWxnaGxsBF9TAzIwNzcyOTYyNjUEdGlkAzEyBHRtcGwDZ2Ex/t/2322m2",
                a.Attr("abs:href")); // session put into <base>
        NUnit.Framework.Assert.AreEqual("全国、人気の駅ランキング", a.Text());
    }

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
            NUnit.Framework.Assert.AreEqual("http://news.baidu.com/", newsLink.AbsUrl("href"));
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
            NUnit.Framework.Assert.AreNotEqual("新", doc.Text());
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
        public void TestNytArticle()
        {
            // has tags like <nyt_text>
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/nyt-article-1.html.gz");
            Document doc = Jsoup.Parse(@in, null, "http://www.nytimes.com/2010/07/26/business/global/26bp.html?hp");

            Element headline = doc.Select("nyt_headline[version=1.0]").First();
            NUnit.Framework.Assert.AreEqual("As BP Lays Out Future, It Will Not Include Hayward", headline.Text());
        }

        [NUnit.Framework.Test]
        public void TestYahooArticle()
        {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/yahoo-article-1.html.gz");
            Document doc = Jsoup.Parse(@in, "UTF-8", "http://news.yahoo.com/s/nm/20100831/bs_nm/us_gm_china");
            Element p = doc.Select("p:contains(Volt will be sold in the United States)").First();
            NUnit.Framework.Assert.AreEqual(
                "In July, GM said its electric Chevrolet Volt will be sold in the United States at $41,000 -- $8,000 more than its nearest competitor, the Nissan Leaf.",
                p.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestLowercaseUtf8Charset() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/lowercase-charset-test.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null);
            iText.StyledXmlParser.Jsoup.Nodes.Element form = doc.Select("#form").First();
            NUnit.Framework.Assert.AreEqual(2, form.Children().Count);
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().Name());
        }
        
        [NUnit.Framework.Test]
    public void TestXwiki() {
        // https://github.com/jhy/jsoup/issues/1324
        // this tests that when in CharacterReader we hit a buffer while marked, we preserve the mark when buffered up and can rewind
        FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/xwiki-1324.html.gz");
        Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "https://localhost/");
        NUnit.Framework.Assert.AreEqual("XWiki Jetty HSQLDB 12.1-SNAPSHOT", doc.Select("#xwikiplatformversion").Text());

        // was getting busted at =userdirectory, because it hit the bufferup point but the mark was then lost. so
        // updated to preserve the mark.
        String wantHtml = "<a class=\"list-group-item\" data-id=\"userdirectory\" href=\"/xwiki/bin/admin/XWiki/XWikiPreferences?editor=globaladmin&amp;section=userdirectory\" title=\"Customize the user directory live table.\">User Directory</a>";
        NUnit.Framework.Assert.AreEqual(wantHtml, doc.Select("[data-id=userdirectory]").OuterHtml());
    }

    [NUnit.Framework.Test]
    public void TestXwikiExpanded() {
        // https://github.com/jhy/jsoup/issues/1324
        // this tests that if there is a huge illegal character reference, we can get through a buffer and rewind, and still catch that it's an invalid refence,
        // and the parse tree is correct.
        FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/xwiki-edit.html.gz");
        iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser();
        Document doc = Jsoup.Parse(new GZipStream(new FileStream(@in.FullName, FileMode.Open, FileAccess.Read), CompressionMode.Decompress), "UTF-8", "https://localhost/", parser.SetTrackErrors(100));
        ParseErrorList errors = parser.GetErrors();

        NUnit.Framework.Assert.AreEqual("XWiki Jetty HSQLDB 12.1-SNAPSHOT", doc.Select("#xwikiplatformversion").Text());
        // not an invalid reference because did not look legit
        NUnit.Framework.Assert.AreEqual(0, errors.Count);

        // was getting busted at =userdirectory, because it hit the bufferup point but the mark was then lost. so
        // updated to preserve the mark.
        String wantHtml = "<a class=\"list-group-item\" data-id=\"userdirectory\" href=\"/xwiki/bin/admin/XWiki/XWikiPreferences?editor=globaladmin&amp;RIGHTHERERIGHTHERERIGHTHERERIGHTHERE";
        NUnit.Framework.Assert.IsTrue(doc.Select("[data-id=userdirectory]").OuterHtml().StartsWith(wantHtml));
    }

        [NUnit.Framework.Test]
        public virtual void TestWikiExpandedFromString() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/xwiki-edit.html.gz");
            String html = GetFileAsString(@in);
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("XWiki Jetty HSQLDB 12.1-SNAPSHOT", doc.Select("#xwikiplatformversion").Text
                ());
            String wantHtml = "<a class=\"list-group-item\" data-id=\"userdirectory\" href=\"/xwiki/bin/admin/XWiki/XWikiPreferences?editor=globaladmin&amp;RIGHTHERERIGHTHERERIGHTHERERIGHTHERE";
            NUnit.Framework.Assert.IsTrue(doc.Select("[data-id=userdirectory]").OuterHtml().StartsWith(wantHtml));
        }

        [NUnit.Framework.Test]
        public virtual void TestWikiFromString() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/xwiki-1324.html.gz");
            String html = GetFileAsString(@in);
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("XWiki Jetty HSQLDB 12.1-SNAPSHOT", doc.Select("#xwikiplatformversion").Text
                ());
            String wantHtml = "<a class=\"list-group-item\" data-id=\"userdirectory\" href=\"/xwiki/bin/admin/XWiki/XWikiPreferences?editor=globaladmin&amp;section=userdirectory\" title=\"Customize the user directory live table.\">User Directory</a>";
            NUnit.Framework.Assert.AreEqual(wantHtml, doc.Select("[data-id=userdirectory]").OuterHtml());
        }

        public static Stream InputStreamFrom(String s) {
            return new MemoryStream(s.GetBytes(System.Text.Encoding.UTF8));
        }

        public static String GetFileAsString(FileInfo file) {
            byte[] bytes;
            if (file.Name.EndsWith(".gz")) {
                Stream stream = new GZipStream(new FileStream(file.FullName, FileMode.Open, FileAccess.Read), System.IO.Compression.CompressionMode.Decompress);
                ByteBuffer byteBuffer = DataUtil.ReadToByteBuffer(stream, 0);
                bytes = ((byte[])byteBuffer.Array());
            }
            else {
                bytes = File.ReadAllBytes(file.FullName);
            }
            return JavaUtil.GetStringForBytes(bytes);
        }
    }
}
