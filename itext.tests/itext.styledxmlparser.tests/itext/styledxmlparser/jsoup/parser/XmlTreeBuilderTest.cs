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
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Integration;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tests XmlTreeBuilder.</summary>
    /// <author>Jonathan Hedley</author>
    public class XmlTreeBuilderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestSimpleXmlParse() {
            String xml = "<doc id=2 href='/bar'>Foo <br /><link>One</link><link>Two</link></doc>";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<doc id=\"2\" href=\"/bar\">Foo <br /><link>One</link><link>Two</link></doc>"
                , TextUtil.StripNewlines(doc.Html()));
            NUnit.Framework.Assert.AreEqual(doc.GetElementById("2").AbsUrl("href"), "http://foo.com/bar");
        }

        [NUnit.Framework.Test]
        public virtual void TestPopToClose() {
            // test: </val> closes Two, </bar> ignored
            String xml = "<doc><val>One<val>Two</val></bar>Three</doc>";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<doc><val>One<val>Two</val>Three</val></doc>", TextUtil.StripNewlines(doc
                .Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestCommentAndDocType() {
            String xml = "<!DOCTYPE html><!-- a comment -->One <qux />Two";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html><!-- a comment -->One <qux />Two", TextUtil.StripNewlines(
                doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestSupplyParserToJsoupClass() {
            String xml = "<doc><val>One<val>Two</val></bar>Three</doc>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "http://foo.com/", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("<doc><val>One<val>Two</val>Three</val></doc>", TextUtil.StripNewlines(doc
                .Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestSupplyParserToDataStream() {
            FileInfo xmlFile = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/xml-test.xml");
            Stream inStream = new FileStream(xmlFile.FullName, FileMode.Open, FileAccess.Read);
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(inStream, null, "http://foo.com", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("<doc><val>One<val>Two</val>Three</val></doc>", TextUtil.StripNewlines(doc
                .Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestDoesNotForceSelfClosingKnownTags() {
            // html will force "<br>one</br>" to logically "<br />One<br />". XML should be stay "<br>one</br> -- don't recognise tag.
            Document htmlDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<br>one</br>");
            NUnit.Framework.Assert.AreEqual("<br>one\n<br>", htmlDoc.Body().Html());
            Document xmlDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<br>one</br>", "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("<br>one</br>", xmlDoc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesXmlDeclarationAsDeclaration() {
            String html = "<?xml encoding='UTF-8' ?><body>One</body><!-- comment -->";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("<?xml encoding=\"UTF-8\"?> <body> One </body> <!-- comment -->", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .NormaliseWhitespace(doc.OuterHtml()));
            NUnit.Framework.Assert.AreEqual("#declaration", doc.ChildNode(0).NodeName());
            NUnit.Framework.Assert.AreEqual("#comment", doc.ChildNode(2).NodeName());
        }

        [NUnit.Framework.Test]
        public virtual void XmlFragment() {
            String xml = "<one src='/foo/' />Two<three><four /></three>";
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseXmlFragment
                (xml, "http://example.com/");
            NUnit.Framework.Assert.AreEqual(3, nodes.Count);
            NUnit.Framework.Assert.AreEqual("http://example.com/foo/", nodes[0].AbsUrl("src"));
            NUnit.Framework.Assert.AreEqual("one", nodes[0].NodeName());
            NUnit.Framework.Assert.AreEqual("Two", ((TextNode)nodes[1]).Text());
        }

        [NUnit.Framework.Test]
        public virtual void XmlParseDefaultsToHtmlOutputSyntax() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("x", "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml, doc.OutputSettings().Syntax(
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestDoesHandleEOFInTag() {
            String html = "<img src=asdf onerror=\"alert(1)\" x=";
            Document xmlDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("<img src=\"asdf\" onerror=\"alert(1)\" x=\"\" />", xmlDoc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestDetectCharsetEncodingDeclaration() {
            FileInfo xmlFile = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/xml-charset.xml");
            Stream inStream = new FileStream(xmlFile.FullName, FileMode.Open, FileAccess.Read);
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(inStream, null, "http://example.com/", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("ISO-8859-1", doc.Charset().Name());
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?> <data>äöåéü</data>", TextUtil
                .StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestParseDeclarationAttributes() {
            String xml = "<?xml version='1' encoding='UTF-8' something='else'?><val>One</val>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            XmlDeclaration decl = (XmlDeclaration)doc.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("1", decl.Attr("version"));
            NUnit.Framework.Assert.AreEqual("UTF-8", decl.Attr("encoding"));
            NUnit.Framework.Assert.AreEqual("else", decl.Attr("something"));
            NUnit.Framework.Assert.AreEqual("version=\"1\" encoding=\"UTF-8\" something=\"else\"", decl.GetWholeDeclaration
                ());
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1\" encoding=\"UTF-8\" something=\"else\"?>", decl.OuterHtml
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreatesValidProlog() {
            Document document = Document.CreateShell("");
            document.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
            document.Charset(EncodingUtil.GetEncoding("utf-8"));
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<html>\n" + " <head></head>\n"
                 + " <body></body>\n" + "</html>", document.OuterHtml());
        }
    }
}
