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
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Integration;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Helper {
    public class DataUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCharset() {
            NUnit.Framework.Assert.AreEqual("utf-8", DataUtil.GetCharsetFromContentType("text/html;charset=utf-8 "));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html; charset=UTF-8"));
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=ISO-8859-1"
                ));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html"));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType(null));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html;charset=Unknown"));
        }

        [NUnit.Framework.Test]
        public virtual void TestQuotedCharset() {
            NUnit.Framework.Assert.AreEqual("utf-8", DataUtil.GetCharsetFromContentType("text/html; charset=\"utf-8\""
                ));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html;charset=\"UTF-8\"")
                );
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", DataUtil.GetCharsetFromContentType("text/html; charset=\"ISO-8859-1\""
                ));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset=\"Unsupported\""
                ));
            NUnit.Framework.Assert.AreEqual("UTF-8", DataUtil.GetCharsetFromContentType("text/html; charset='UTF-8'"));
        }

        [NUnit.Framework.Test]
        public virtual void DiscardsSpuriousByteOrderMark() {
            String html = "\uFEFF<html><head><title>One</title></head><body>Two</body></html>";
            ByteBuffer buffer = EncodingUtil.GetEncoding("UTF-8").Encode(html);
            Document doc = DataUtil.ParseByteData(buffer, "UTF-8", "http://foo.com/", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual("One", doc.Head().Text());
        }

        [NUnit.Framework.Test]
        public virtual void DiscardsSpuriousByteOrderMarkWhenNoCharsetSet() {
            String html = "\uFEFF<html><head><title>One</title></head><body>Two</body></html>";
            ByteBuffer buffer = EncodingUtil.GetEncoding("UTF-8").Encode(html);
            Document doc = DataUtil.ParseByteData(buffer, null, "http://foo.com/", iText.StyledXmlParser.Jsoup.Parser.Parser
                .HtmlParser());
            NUnit.Framework.Assert.AreEqual("One", doc.Head().Text());
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().DisplayName());
        }

        [NUnit.Framework.Test]
        public virtual void ShouldNotThrowExceptionOnEmptyCharset() {
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset="));
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset=;"));
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
            NUnit.Framework.Assert.AreEqual(null, DataUtil.GetCharsetFromContentType("text/html; charset=$HJKDF§$/("));
        }

        [NUnit.Framework.Test]
        public virtual void GeneratesMimeBoundaries() {
            String m1 = DataUtil.MimeBoundary();
            String m2 = DataUtil.MimeBoundary();
            NUnit.Framework.Assert.AreEqual(DataUtil.boundaryLength, m1.Length);
            NUnit.Framework.Assert.AreEqual(DataUtil.boundaryLength, m2.Length);
            NUnit.Framework.Assert.AreNotSame(m1, m2);
        }

        [NUnit.Framework.Test]
        public virtual void WrongMetaCharsetFallback() {
            try {
                byte[] input = "<html><head><meta charset=iso-8></head><body></body></html>".GetBytes("UTF-8");
                ByteBuffer inBuffer = ByteBuffer.Wrap(input);
                Document doc = DataUtil.ParseByteData(inBuffer, null, "http://example.com", iText.StyledXmlParser.Jsoup.Parser.Parser
                    .HtmlParser());
                String expected = "<html>\n" + " <head>\n" + "  <meta charset=\"iso-8\">\n" + " </head>\n" + " <body></body>\n"
                     + "</html>";
                NUnit.Framework.Assert.AreEqual(expected, doc.ToString());
            }
            catch (ArgumentException ex) {
                NUnit.Framework.Assert.Fail(ex.Message);
            }
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
    }
}
