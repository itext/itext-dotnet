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
using iText.StyledXmlParser.Jsoup.Integration;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Helper {
    [NUnit.Framework.Category("UnitTest")]
    public class DataUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCharset() {
            NUnit.Framework.Assert.AreEqual("utf-8", DataUtil.GetCharsetFromContentType("text/html;charset=utf-8 "));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html; charset=UTF-8"));
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=ISO-8859-1"
                ));
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType("text/html"));
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType(null));
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType("text/html;charset=Unknown"));
        }

        [NUnit.Framework.Test]
        public virtual void TestQuotedCharset() {
            NUnit.Framework.Assert.AreEqual("utf-8", DataUtil.GetCharsetFromContentType("text/html; charset=\"utf-8\""
                ));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html;charset=\"UTF-8\"")
                );
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=\"ISO-8859-1\""
                ));
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType("text/html; charset=\"Unsupported\""));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html; charset='UTF-8'"));
        }

        private System.IO.Stream Stream(String data) {
            return new MemoryStream(data.GetBytes(System.Text.Encoding.UTF8));
        }

        private System.IO.Stream Stream(String data, String charset) {
            try {
                return new MemoryStream(data.GetBytes(charset));
            }
            catch (ArgumentException) {
                NUnit.Framework.Assert.Fail();
            }
            return null;
        }

        [NUnit.Framework.Test]
        public virtual void DiscardsSpuriousByteOrderMark() {
            String html = "\uFEFF<html><head><title>One</title></head><body>Two</body></html>";
            Document doc = DataUtil.ParseInputStream(Stream(html), "UTF-8", "http://foo.com/", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual("One", doc.Head().Text());
        }

        [NUnit.Framework.Test]
        public virtual void DiscardsSpuriousByteOrderMarkWhenNoCharsetSet() {
            String html = "\uFEFF<html><head><title>One</title></head><body>Two</body></html>";
            Document doc = DataUtil.ParseInputStream(Stream(html), null, "http://foo.com/", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual("One", doc.Head().Text());
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().DisplayName());
        }

        [NUnit.Framework.Test]
        public virtual void ShouldNotThrowExceptionOnEmptyCharset() {
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType("text/html; charset="));
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType("text/html; charset=;"));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldSelectFirstCharsetOnWeirdMultileCharsetsInMetaTags() {
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=ISO-8859-1, charset=1251"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldCorrectCharsetForDuplicateCharsetString() {
            NUnit.Framework.Assert.AreEqual("iso-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=charset=iso-8859-1"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldReturnNullForIllegalCharsetNames() {
            NUnit.Framework.Assert.IsNull(DataUtil.GetCharsetFromContentType("text/html; charset=$HJKDF§$/("));
        }

        [NUnit.Framework.Test]
        public virtual void WrongMetaCharsetFallback() {
            String html = "<html><head><meta charset=iso-8></head><body></body></html>";
            Document doc = DataUtil.ParseInputStream(Stream(html), null, "http://example.com", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            String expected = "<html>\n" + " <head>\n" + "  <meta charset=\"iso-8\">\n" + " </head>\n" + " <body></body>\n"
                 + "</html>";
            NUnit.Framework.Assert.AreEqual(expected, doc.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SecondMetaElementWithContentTypeContainsCharsetParameter() {
            String html = "<html><head>" + "<meta http-equiv=\"Content-Type\" content=\"text/html\">" + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=euc-kr\">"
                 + "</head><body>한국어</body></html>";
            Document doc = DataUtil.ParseInputStream(Stream(html, "euc-kr"), null, "http://example.com", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual("한국어", doc.Body().Text());
        }

        [NUnit.Framework.Test]
        public virtual void FirstMetaElementWithCharsetShouldBeUsedForDecoding() {
            String html = "<html><head>" + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">"
                 + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=koi8-u\">" + "</head><body>Übergrößenträger</body></html>";
            Document doc = DataUtil.ParseInputStream(Stream(html, "iso-8859-1"), null, "http://example.com", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual("Übergrößenträger", doc.Body().Text());
        }

        [NUnit.Framework.Test]
        public virtual void SupportsBOMinFiles() {
            // test files from http://www.i18nl10n.com/korean/utftest/
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf16be.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-16BE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
            @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf16le.html");
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-16LE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
            @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf32be.html");
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-32BE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
            @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf32le.html");
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.IsTrue(doc.Title().Contains("UTF-32LE"));
            NUnit.Framework.Assert.IsTrue(doc.Text().Contains("가각갂갃간갅"));
        }

        [NUnit.Framework.Test]
        public virtual void SupportsUTF8BOM() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf8.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.AreEqual("OK", doc.Head().Select("title").Text());
        }

        [NUnit.Framework.Test]
        public virtual void NoExtraNULLBytes() {
            byte[] b = "<html><head><meta charset=\"UTF-8\"></head><body><div><u>ü</u>ü</div></body></html>".GetBytes(
                "UTF-8");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(new MemoryStream(b), null, "");
            NUnit.Framework.Assert.IsFalse(doc.OuterHtml().Contains("\u0000"));
        }

        [NUnit.Framework.Test]
        public virtual void SupportsZippedUTF8BOM() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/bomtests/bom_utf8.html.gz");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://example.com");
            NUnit.Framework.Assert.AreEqual("OK", doc.Head().Select("title").Text());
            NUnit.Framework.Assert.AreEqual("There is a UTF8 BOM at the top (before the XML decl). If not read correctly, will look like a non-joining space."
                , doc.Body().Text());
        }

        [NUnit.Framework.Test]
        public virtual void SupportsXmlCharsetDeclaration() {
            String encoding = "iso-8859-1";
            System.IO.Stream soup = new MemoryStream(("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" + "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">"
                 + "<html xmlns=\"http://www.w3.org/1999/xhtml\" lang=\"en\" xml:lang=\"en\">Hellö Wörld!</html>").GetBytes
                (encoding));
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(soup, null, "");
            NUnit.Framework.Assert.AreEqual("Hellö Wörld!", doc.Body().Text());
        }

        [NUnit.Framework.Test]
        public virtual void LLoadsGzipFile() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/gzip.html.gz");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null);
            NUnit.Framework.Assert.AreEqual("Gzip test", doc.Title());
            NUnit.Framework.Assert.AreEqual("This is a gzipped HTML file.", doc.SelectFirst("p").Text());
        }

        [NUnit.Framework.Test]
        public virtual void LoadsZGzipFile() {
            // compressed on win, with z suffix
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/gzip.html.z");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null);
            NUnit.Framework.Assert.AreEqual("Gzip test", doc.Title());
            NUnit.Framework.Assert.AreEqual("This is a gzipped HTML file.", doc.SelectFirst("p").Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesFakeGzipFile() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/fake-gzip.html.gz");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null);
            NUnit.Framework.Assert.AreEqual("This is not gzipped", doc.Title());
            NUnit.Framework.Assert.AreEqual("And should still be readable.", doc.SelectFirst("p").Text());
        }
    }
}
