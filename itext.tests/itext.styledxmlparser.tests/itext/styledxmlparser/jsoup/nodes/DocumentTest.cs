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
using System.IO;
using System.Text;
using System.Threading;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Parser;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for Document.</summary>
    [NUnit.Framework.Category("UnitTest")]
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
        public virtual void AccessorsWillNormalizeStructure() {
            Document doc = new Document("");
            NUnit.Framework.Assert.AreEqual("", doc.Html());
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            NUnit.Framework.Assert.AreEqual("body", body.TagName());
            iText.StyledXmlParser.Jsoup.Nodes.Element head = doc.Head();
            NUnit.Framework.Assert.AreEqual("head", head.TagName());
            NUnit.Framework.Assert.AreEqual("<html><head></head><body></body></html>", TextUtil.StripNewlines(doc.Html
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void AccessorsAreCaseInsensitive() {
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser().
                Settings(ParseSettings.preserveCase);
            Document doc = parser.ParseInput("<!DOCTYPE html><HTML><HEAD><TITLE>SHOUTY</TITLE></HEAD><BODY>HELLO</BODY></HTML>"
                , "");
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            NUnit.Framework.Assert.AreEqual("BODY", body.TagName());
            NUnit.Framework.Assert.AreEqual("body", body.NormalName());
            iText.StyledXmlParser.Jsoup.Nodes.Element head = doc.Head();
            NUnit.Framework.Assert.AreEqual("HEAD", head.TagName());
            NUnit.Framework.Assert.AreEqual("body", body.NormalName());
            iText.StyledXmlParser.Jsoup.Nodes.Element root = doc.SelectFirst("html");
            NUnit.Framework.Assert.AreEqual("HTML", root.TagName());
            NUnit.Framework.Assert.AreEqual("html", root.NormalName());
            NUnit.Framework.Assert.AreEqual("SHOUTY", doc.Title());
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
        public virtual void TestLocationFromString() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello");
            NUnit.Framework.Assert.AreEqual("", doc.Location());
        }

        [NUnit.Framework.Test]
        public virtual void TestHtmlAndXmlSyntax() {
            String h = "<!DOCTYPE html><body><img async checked='checked' src='&<>\"'>&lt;&gt;&amp;&quot;<foo />bar";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.html);
            NUnit.Framework.Assert.AreEqual("<!doctype html>\n" + "<html>\n" + " <head></head>\n" + " <body>\n" + "  <img async checked src=\"&amp;<>&quot;\">&lt;&gt;&amp;\"<foo />bar\n"
                 + " </body>\n" + "</html>", doc.Html());
            doc.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html>\n" + "<html>\n" + " <head></head>\n" + " <body>\n" + "  <img async=\"\" checked=\"checked\" src=\"&amp;&lt;>&quot;\" />&lt;&gt;&amp;\"<foo />bar\n"
                 + " </body>\n" + "</html>", doc.Html());
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

        [NUnit.Framework.Test]
        public virtual void TestOverflowClone() {
            StringBuilder sb = new StringBuilder();
            sb.Append("<head><base href='https://jsoup.org/'>");
            for (int i = 0; i < 100000; i++) {
                sb.Append("<div>");
            }
            sb.Append("<p>Hello <a href='/example.html'>there</a>");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(sb.ToString());
            String expectedLink = "https://jsoup.org/example.html";
            NUnit.Framework.Assert.AreEqual(expectedLink, doc.SelectFirst("a").Attr("abs:href"));
            Document clone = (Document)doc.Clone();
            doc.HasSameValue(clone);
            NUnit.Framework.Assert.AreEqual(expectedLink, clone.SelectFirst("a").Attr("abs:href"));
        }

        [NUnit.Framework.Test]
        public virtual void DocumentsWithSameContentAreEqual() {
            Document docA = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>One");
            Document docB = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>One");
            Document docC = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div/>Two");
            NUnit.Framework.Assert.AreNotEqual(docA, docB);
            NUnit.Framework.Assert.AreEqual(docA, docA);
            NUnit.Framework.Assert.AreEqual(docA.GetHashCode(), docA.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(docA.GetHashCode(), docC.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentsWithSameContentAreVerifiable() {
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
            if (addDecl) {
                XmlDeclaration decl = new XmlDeclaration("xml", false);
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
            Stream @is = new MemoryStream(input.GetBytes(System.Text.Encoding.ASCII));
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@is, null, "http://example.com");
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.xhtml);
            String output = iText.Commons.Utils.JavaUtil.GetStringForBytes(doc.Html().GetBytes(doc.OutputSettings().Charset
                ()), doc.OutputSettings().Charset());
            NUnit.Framework.Assert.IsFalse(output.Contains("?"));
            NUnit.Framework.Assert.IsTrue(output.Contains("&#xa0;") || output.Contains("&nbsp;"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseAndHtmlOnDifferentThreads() {
            String html = "<p>Alrighty then it's not \uD83D\uDCA9. <span>Next</span></p>";
            // 💩
            String asci = "<p>Alrighty then it's not &#x1f4a9;. <span>Next</span></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            String[] @out = new String[1];
            Elements p = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(html, p.OuterHtml());
            Thread thread = new Thread(() => {
                @out[0] = p.OuterHtml();
                doc.OutputSettings().Charset(System.Text.Encoding.ASCII);
            }
            );
            thread.Start();
            thread.Join();
            NUnit.Framework.Assert.AreEqual(html, @out[0]);
            NUnit.Framework.Assert.AreEqual(System.Text.Encoding.ASCII, doc.OutputSettings().Charset());
            NUnit.Framework.Assert.AreEqual(asci, p.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestDocumentTypeGet() {
            String html = "\n\n<!-- comment -->  <!doctype html><p>One</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            DocumentType documentType = doc.DocumentType();
            NUnit.Framework.Assert.IsNotNull(documentType);
            NUnit.Framework.Assert.AreEqual("html", documentType.Name());
        }

        [NUnit.Framework.Test]
        public virtual void FramesetSupportsBodyMethod() {
            String html = "<html><head><title>Frame Test</title></head><frameset id=id><frame src=foo.html></frameset>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element head = doc.Head();
            NUnit.Framework.Assert.IsNotNull(head);
            NUnit.Framework.Assert.AreEqual("Frame Test", doc.Title());
            // Frameset docs per html5 spec have no body element - but instead a frameset elelemt
            NUnit.Framework.Assert.IsNull(doc.SelectFirst("body"));
            iText.StyledXmlParser.Jsoup.Nodes.Element frameset = doc.SelectFirst("frameset");
            NUnit.Framework.Assert.IsNotNull(frameset);
            // the body() method returns body or frameset and does not otherwise modify the document
            // doing it in body() vs parse keeps the html close to original for round-trip option
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            NUnit.Framework.Assert.IsNotNull(body);
            NUnit.Framework.Assert.AreSame(frameset, body);
            NUnit.Framework.Assert.AreEqual("frame", body.Child(0).TagName());
            NUnit.Framework.Assert.IsNull(doc.SelectFirst("body"));
            // did not vivify a body element
            String expected = "<html>\n" + " <head>\n" + "  <title>Frame Test</title>\n" + " </head>\n" + " <frameset id=\"id\">\n"
                 + "  <frame src=\"foo.html\">\n" + " </frameset>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(expected, doc.Html());
        }
    }
}
