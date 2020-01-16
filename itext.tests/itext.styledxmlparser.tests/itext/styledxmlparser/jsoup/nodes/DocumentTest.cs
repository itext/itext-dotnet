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
using System.Text;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Integration;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for Document.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class DocumentTest : ExtendedITextTest {
        private const String charsetUtf8 = "UTF-8";

        private const String charsetIso8859 = "ISO-8859-1";

        [NUnit.Framework.Test]
        public virtual void SetTextPreservesDocumentStructure() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello</p>");
            doc.Text("Replaced");
            NUnit.Framework.Assert.AreEqual("Replaced", doc.Text());
            NUnit.Framework.Assert.AreEqual("Replaced", doc.Body().Text());
            NUnit.Framework.Assert.AreEqual(1, doc.Select("head").Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestTitles() {
            Document noTitle = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello</p>");
            Document withTitle = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>First</title><title>Ignore</title><p>Hello</p>"
                );
            NUnit.Framework.Assert.AreEqual("", noTitle.Title());
            noTitle.Title("Hello");
            NUnit.Framework.Assert.AreEqual("Hello", noTitle.Title());
            NUnit.Framework.Assert.AreEqual("Hello", noTitle.Select("title").First().Text());
            NUnit.Framework.Assert.AreEqual("First", withTitle.Title());
            withTitle.Title("Hello");
            NUnit.Framework.Assert.AreEqual("Hello", withTitle.Title());
            NUnit.Framework.Assert.AreEqual("Hello", withTitle.Select("title").First().Text());
            Document normaliseTitle = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>   Hello\nthere   \n   now   \n"
                );
            NUnit.Framework.Assert.AreEqual("Hello there now", normaliseTitle.Title());
        }

        [NUnit.Framework.Test]
        public virtual void TestOutputEncoding() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p title=π>π & < > </p>");
            // default is utf-8
            NUnit.Framework.Assert.AreEqual("<p title=\"π\">π &amp; &lt; &gt; </p>", doc.Body().Html());
            NUnit.Framework.Assert.AreEqual("UTF-8", doc.OutputSettings().Charset().Name());
            doc.OutputSettings().Charset("ascii");
            NUnit.Framework.Assert.AreEqual(Entities.EscapeMode.@base, doc.OutputSettings().EscapeMode());
            NUnit.Framework.Assert.AreEqual("<p title=\"&#x3c0;\">&#x3c0; &amp; &lt; &gt; </p>", doc.Body().Html());
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.extended);
            NUnit.Framework.Assert.AreEqual("<p title=\"&pi;\">&pi; &amp; &lt; &gt; </p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestXhtmlReferences() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("&lt; &gt; &amp; &quot; &apos; &times;");
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.xhtml);
            NUnit.Framework.Assert.AreEqual("&lt; &gt; &amp; \" ' ×", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalisesStructure() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><head><script>one</script><noscript><p>two</p></noscript></head><body><p>three</p></body><p>four</p></html>"
                );
            NUnit.Framework.Assert.AreEqual("<html><head><script>one</script><noscript>&lt;p&gt;two</noscript></head><body><p>three</p><p>four</p></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestClone() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Hello</title> <p>One<p>Two");
            Document clone = (Document)doc.Clone();
            NUnit.Framework.Assert.AreEqual("<html><head><title>Hello</title> </head><body><p>One</p><p>Two</p></body></html>"
                , TextUtil.StripNewlines(clone.Html()));
            clone.Title("Hello there");
            clone.Select("p").First().Text("One more").Attr("id", "1");
            NUnit.Framework.Assert.AreEqual("<html><head><title>Hello there</title> </head><body><p id=\"1\">One more</p><p>Two</p></body></html>"
                , TextUtil.StripNewlines(clone.Html()));
            NUnit.Framework.Assert.AreEqual("<html><head><title>Hello</title> </head><body><p>One</p><p>Two</p></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestClonesDeclarations() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<!DOCTYPE html><html><head><title>Doctype test");
            Document clone = (Document)doc.Clone();
            NUnit.Framework.Assert.AreEqual(doc.Html(), clone.Html());
            NUnit.Framework.Assert.AreEqual("<!doctype html><html><head><title>Doctype test</title></head><body></body></html>"
                , TextUtil.StripNewlines(clone.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestLocation() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/yahoo-jp.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8", "http://www.yahoo.co.jp/index.html");
            String location = doc.Location();
            String baseUri = doc.BaseUri();
            NUnit.Framework.Assert.AreEqual("http://www.yahoo.co.jp/index.html", location);
            NUnit.Framework.Assert.AreEqual("http://www.yahoo.co.jp/_ylh=X3oDMTB0NWxnaGxsBF9TAzIwNzcyOTYyNjUEdGlkAzEyBHRtcGwDZ2Ex/"
                , baseUri);
            @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/nyt-article-1.html");
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, null, "http://www.nytimes.com/2010/07/26/business/global/26bp.html?hp"
                );
            location = doc.Location();
            baseUri = doc.BaseUri();
            NUnit.Framework.Assert.AreEqual("http://www.nytimes.com/2010/07/26/business/global/26bp.html?hp", location
                );
            NUnit.Framework.Assert.AreEqual("http://www.nytimes.com/2010/07/26/business/global/26bp.html?hp", baseUri);
        }

        [NUnit.Framework.Test]
        public virtual void TestHtmlAndXmlSyntax() {
            String h = "<!DOCTYPE html><body><img async checked='checked' src='&<>\"'>&lt;&gt;&amp;&quot;<foo />bar";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.html);
            NUnit.Framework.Assert.AreEqual("<!doctype html>\n" + "<html>\n" + " <head></head>\n" + " <body>\n" + "  <img async checked src=\"&amp;<>&quot;\">&lt;&gt;&amp;\"\n"
                 + "  <foo />bar\n" + " </body>\n" + "</html>", doc.Html());
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html>\n" + "<html>\n" + " <head></head>\n" + " <body>\n" + "  <img async=\"\" checked=\"checked\" src=\"&amp;<>&quot;\" />&lt;&gt;&amp;\"\n"
                 + "  <foo />bar\n" + " </body>\n" + "</html>", doc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void HtmlParseDefaultsToHtmlOutputSyntax() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("x");
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Jsoup.Nodes.Syntax.html, doc.OutputSettings().Syntax
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestHtmlAppendable() {
            String htmlContent = "<html><head><title>Hello</title></head><body><p>One</p><p>Two</p></body></html>";
            Document document = iText.StyledXmlParser.Jsoup.Jsoup.Parse(htmlContent);
            OutputSettings outputSettings = new OutputSettings();
            outputSettings.PrettyPrint(false);
            document.OutputSettings(outputSettings);
            NUnit.Framework.Assert.AreEqual(htmlContent, document.Html(new StringBuilder()).ToString());
        }

        // This test can take awhile to run.
        [NUnit.Framework.Test]
        public virtual void TestOverflowClone() {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 100000; i++) {
                builder.Insert(0, "<i>");
                builder.Append("</i>");
            }
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(builder.ToString());
            doc.Clone();
        }

        [NUnit.Framework.Test]
        public virtual void DocumentsWithSameContentAreEqual() {
            Document docA = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>One");
            Document docB = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>One");
            Document docC = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>Two");
            NUnit.Framework.Assert.IsFalse(docA.Equals(docB));
            NUnit.Framework.Assert.IsTrue(docA.Equals(docA));
            NUnit.Framework.Assert.AreEqual(docA.GetHashCode(), docA.GetHashCode());
            NUnit.Framework.Assert.IsFalse(docA.GetHashCode() == docC.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentsWithSameContentAreVerifialbe() {
            Document docA = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>One");
            Document docB = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>One");
            Document docC = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>Two");
            NUnit.Framework.Assert.IsTrue(docA.HasSameValue(docB));
            NUnit.Framework.Assert.IsFalse(docA.HasSameValue(docC));
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateUtf8() {
            Document doc = CreateHtmlDocument("changeThis");
            doc.UpdateMetaCharsetElement(true);
            doc.Charset(EncodingUtil.GetEncoding(charsetUtf8));
            String htmlCharsetUTF8 = "<html>\n" + " <head>\n" + "  <meta charset=\"" + charsetUtf8 + "\">\n" + " </head>\n"
                 + " <body></body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(htmlCharsetUTF8, doc.ToString());
            iText.StyledXmlParser.Jsoup.Nodes.Element selectedElement = doc.Select("meta[charset]").First();
            NUnit.Framework.Assert.AreEqual(charsetUtf8, doc.Charset().Name());
            NUnit.Framework.Assert.AreEqual(charsetUtf8, selectedElement.Attr("charset"));
            NUnit.Framework.Assert.AreEqual(doc.Charset(), doc.OutputSettings().Charset());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateIso8859() {
            Document doc = CreateHtmlDocument("changeThis");
            doc.UpdateMetaCharsetElement(true);
            doc.Charset(EncodingUtil.GetEncoding(charsetIso8859));
            String htmlCharsetISO = "<html>\n" + " <head>\n" + "  <meta charset=\"" + charsetIso8859 + "\">\n" + " </head>\n"
                 + " <body></body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(htmlCharsetISO, doc.ToString());
            iText.StyledXmlParser.Jsoup.Nodes.Element selectedElement = doc.Select("meta[charset]").First();
            NUnit.Framework.Assert.AreEqual(charsetIso8859, doc.Charset().Name());
            NUnit.Framework.Assert.AreEqual(charsetIso8859, selectedElement.Attr("charset"));
            NUnit.Framework.Assert.AreEqual(doc.Charset(), doc.OutputSettings().Charset());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateNoCharset() {
            Document docNoCharset = Document.CreateShell("");
            docNoCharset.UpdateMetaCharsetElement(true);
            docNoCharset.Charset(EncodingUtil.GetEncoding(charsetUtf8));
            NUnit.Framework.Assert.AreEqual(charsetUtf8, docNoCharset.Select("meta[charset]").First().Attr("charset"));
            String htmlCharsetUTF8 = "<html>\n" + " <head>\n" + "  <meta charset=\"" + charsetUtf8 + "\">\n" + " </head>\n"
                 + " <body></body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(htmlCharsetUTF8, docNoCharset.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateDisabled() {
            Document docDisabled = Document.CreateShell("");
            String htmlNoCharset = "<html>\n" + " <head></head>\n" + " <body></body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(htmlNoCharset, docDisabled.ToString());
            NUnit.Framework.Assert.IsNull(docDisabled.Select("meta[charset]").First());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateDisabledNoChanges() {
            Document doc = CreateHtmlDocument("dontTouch");
            String htmlCharset = "<html>\n" + " <head>\n" + "  <meta charset=\"dontTouch\">\n" + "  <meta name=\"charset\" content=\"dontTouch\">\n"
                 + " </head>\n" + " <body></body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(htmlCharset, doc.ToString());
            iText.StyledXmlParser.Jsoup.Nodes.Element selectedElement = doc.Select("meta[charset]").First();
            NUnit.Framework.Assert.IsNotNull(selectedElement);
            NUnit.Framework.Assert.AreEqual("dontTouch", selectedElement.Attr("charset"));
            selectedElement = doc.Select("meta[name=charset]").First();
            NUnit.Framework.Assert.IsNotNull(selectedElement);
            NUnit.Framework.Assert.AreEqual("dontTouch", selectedElement.Attr("content"));
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateEnabledAfterCharsetChange() {
            Document doc = CreateHtmlDocument("dontTouch");
            doc.Charset(EncodingUtil.GetEncoding(charsetUtf8));
            iText.StyledXmlParser.Jsoup.Nodes.Element selectedElement = doc.Select("meta[charset]").First();
            NUnit.Framework.Assert.AreEqual(charsetUtf8, selectedElement.Attr("charset"));
            NUnit.Framework.Assert.IsTrue(doc.Select("meta[name=charset]").IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateCleanup() {
            Document doc = CreateHtmlDocument("dontTouch");
            doc.UpdateMetaCharsetElement(true);
            doc.Charset(EncodingUtil.GetEncoding(charsetUtf8));
            String htmlCharsetUTF8 = "<html>\n" + " <head>\n" + "  <meta charset=\"" + charsetUtf8 + "\">\n" + " </head>\n"
                 + " <body></body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(htmlCharsetUTF8, doc.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateXmlUtf8() {
            Document doc = CreateXmlDocument("1.0", "changeThis", true);
            doc.UpdateMetaCharsetElement(true);
            doc.Charset(EncodingUtil.GetEncoding(charsetUtf8));
            String xmlCharsetUTF8 = "<?xml version=\"1.0\" encoding=\"" + charsetUtf8 + "\"?>\n" + "<root>\n" + " node\n"
                 + "</root>";
            NUnit.Framework.Assert.AreEqual(xmlCharsetUTF8, doc.ToString());
            XmlDeclaration selectedNode = (XmlDeclaration)doc.ChildNode(0);
            NUnit.Framework.Assert.AreEqual(charsetUtf8, doc.Charset().Name());
            NUnit.Framework.Assert.AreEqual(charsetUtf8, selectedNode.Attr("encoding"));
            NUnit.Framework.Assert.AreEqual(doc.Charset(), doc.OutputSettings().Charset());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateXmlIso8859() {
            Document doc = CreateXmlDocument("1.0", "changeThis", true);
            doc.UpdateMetaCharsetElement(true);
            doc.Charset(EncodingUtil.GetEncoding(charsetIso8859));
            String xmlCharsetISO = "<?xml version=\"1.0\" encoding=\"" + charsetIso8859 + "\"?>\n" + "<root>\n" + " node\n"
                 + "</root>";
            NUnit.Framework.Assert.AreEqual(xmlCharsetISO, doc.ToString());
            XmlDeclaration selectedNode = (XmlDeclaration)doc.ChildNode(0);
            NUnit.Framework.Assert.AreEqual(charsetIso8859, doc.Charset().Name());
            NUnit.Framework.Assert.AreEqual(charsetIso8859, selectedNode.Attr("encoding"));
            NUnit.Framework.Assert.AreEqual(doc.Charset(), doc.OutputSettings().Charset());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateXmlNoCharset() {
            Document doc = CreateXmlDocument("1.0", "none", false);
            doc.UpdateMetaCharsetElement(true);
            doc.Charset(EncodingUtil.GetEncoding(charsetUtf8));
            String xmlCharsetUTF8 = "<?xml version=\"1.0\" encoding=\"" + charsetUtf8 + "\"?>\n" + "<root>\n" + " node\n"
                 + "</root>";
            NUnit.Framework.Assert.AreEqual(xmlCharsetUTF8, doc.ToString());
            XmlDeclaration selectedNode = (XmlDeclaration)doc.ChildNode(0);
            NUnit.Framework.Assert.AreEqual(charsetUtf8, selectedNode.Attr("encoding"));
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateXmlDisabled() {
            Document doc = CreateXmlDocument("none", "none", false);
            String xmlNoCharset = "<root>\n" + " node\n" + "</root>";
            NUnit.Framework.Assert.AreEqual(xmlNoCharset, doc.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdateXmlDisabledNoChanges() {
            Document doc = CreateXmlDocument("dontTouch", "dontTouch", true);
            String xmlCharset = "<?xml version=\"dontTouch\" encoding=\"dontTouch\"?>\n" + "<root>\n" + " node\n" + "</root>";
            NUnit.Framework.Assert.AreEqual(xmlCharset, doc.ToString());
            XmlDeclaration selectedNode = (XmlDeclaration)doc.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("dontTouch", selectedNode.Attr("encoding"));
            NUnit.Framework.Assert.AreEqual("dontTouch", selectedNode.Attr("version"));
        }

        [NUnit.Framework.Test]
        public virtual void TestMetaCharsetUpdatedDisabledPerDefault() {
            Document doc = CreateHtmlDocument("none");
            NUnit.Framework.Assert.IsFalse(doc.UpdateMetaCharsetElement());
        }

        private Document CreateHtmlDocument(String charset) {
            Document doc = Document.CreateShell("");
            doc.Head().AppendElement("meta").Attr("charset", charset);
            doc.Head().AppendElement("meta").Attr("name", "charset").Attr("content", charset);
            return doc;
        }

        private Document CreateXmlDocument(String version, String charset, bool addDecl) {
            Document doc = new Document("");
            doc.AppendElement("root").Text("node");
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
            if (addDecl == true) {
                XmlDeclaration decl = new XmlDeclaration("xml", "", false);
                decl.Attr("version", version);
                decl.Attr("encoding", charset);
                doc.PrependChild(decl);
            }
            return doc;
        }

        [NUnit.Framework.Test]
        public virtual void TestShiftJisRoundtrip() {
            String input = "<html>" + "<head>" + "<meta http-equiv=\"content-type\" content=\"text/html; charset=Shift_JIS\" />"
                 + "</head>" + "<body>" + "before&nbsp;after" + "</body>" + "</html>";
            Stream @is = new MemoryStream(input.GetBytes(EncodingUtil.GetEncoding("ASCII")));
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@is, null, "http://example.com");
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.xhtml);
            String output = iText.IO.Util.JavaUtil.GetStringForBytes(doc.Html().GetBytes(doc.OutputSettings().Charset(
                )), doc.OutputSettings().Charset());
            NUnit.Framework.Assert.IsFalse(output.Contains("?"), "Should not have contained a '?'.");
            NUnit.Framework.Assert.IsTrue(output.Contains("&#xa0;") || output.Contains("&nbsp;"), "Should have contained a '&#xa0;' or a '&nbsp;'."
                );
        }
    }
}
