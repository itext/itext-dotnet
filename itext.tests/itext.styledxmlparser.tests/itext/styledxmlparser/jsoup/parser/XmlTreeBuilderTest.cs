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
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Integration;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tests XmlTreeBuilder.</summary>
    /// <author>Jonathan Hedley</author>
    [NUnit.Framework.Category("UnitTest")]
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
            String xml = "<!DOCTYPE HTML><!-- a comment -->One <qux />Two";
            XmlTreeBuilder tb = new XmlTreeBuilder();
            Document doc = tb.Parse(xml, "http://foo.com/");
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE HTML><!-- a comment -->One <qux />Two", TextUtil.StripNewlines(
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
            NUnit.Framework.Assert.AreEqual("<?xml encoding=\"UTF-8\"?><body>One</body><!-- comment -->", doc.OuterHtml
                ());
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
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><data>äöåéü</data>", TextUtil
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
        public virtual void CaseSensitiveDeclaration() {
            String xml = "<?XML version='1' encoding='UTF-8' something='else'?>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            NUnit.Framework.Assert.AreEqual("<?XML version=\"1\" encoding=\"UTF-8\" something=\"else\"?>", doc.OuterHtml
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreatesValidProlog() {
            Document document = Document.CreateShell("");
            document.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
            document.Charset(System.Text.Encoding.UTF8);
            NUnit.Framework.Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<html>\n" + " <head></head>\n"
                 + " <body></body>\n" + "</html>", document.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void PreservesCaseByDefault() {
            String xml = "<CHECK>One</CHECK><TEST ID=1>Check</TEST>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            NUnit.Framework.Assert.AreEqual("<CHECK>One</CHECK><TEST ID=\"1\">Check</TEST>", TextUtil.StripNewlines(doc
                .Html()));
        }

        [NUnit.Framework.Test]
        public virtual void AppendPreservesCaseByDefault() {
            String xml = "<One>One</One>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            Elements one = doc.Select("One");
            one.Append("<Two ID=2>Two</Two>");
            NUnit.Framework.Assert.AreEqual("<One>One<Two ID=\"2\">Two</Two></One>", TextUtil.StripNewlines(doc.Html()
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DisablesPrettyPrintingByDefault() {
            String xml = "\n\n<div><one>One</one><one>\n Two</one>\n</div>\n ";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            NUnit.Framework.Assert.AreEqual(xml, doc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void CanNormalizeCase() {
            String xml = "<TEST ID=1>Check</TEST>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser().Settings(ParseSettings.htmlDefault));
            NUnit.Framework.Assert.AreEqual("<test id=\"1\">Check</test>", TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void NormalizesDiscordantTags() {
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser().Settings
                (ParseSettings.htmlDefault);
            Document document = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>test</DIV><p></p>", "", parser);
            NUnit.Framework.Assert.AreEqual("<div>test</div><p></p>", document.Html());
        }

        // was failing -> toString() = "<div>\n test\n <p></p>\n</div>"
        [NUnit.Framework.Test]
        public virtual void RoundTripsCdata() {
            String xml = "<div id=1><![CDATA[\n<html>\n <foo><&amp;]]></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            NUnit.Framework.Assert.AreEqual("<html>\n <foo><&amp;", div.Text());
            NUnit.Framework.Assert.AreEqual(0, div.Children().Count);
            NUnit.Framework.Assert.AreEqual(1, div.ChildNodeSize());
            // no elements, one text node
            NUnit.Framework.Assert.AreEqual("<div id=\"1\"><![CDATA[\n<html>\n <foo><&amp;]]></div>", div.OuterHtml());
            CDataNode cdata = (CDataNode)div.TextNodes()[0];
            NUnit.Framework.Assert.AreEqual("\n<html>\n <foo><&amp;", cdata.Text());
        }

        [NUnit.Framework.Test]
        public virtual void CdataPreservesWhiteSpace() {
            String xml = "<script type=\"text/javascript\">//<![CDATA[\n\n  foo();\n//]]></script>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            NUnit.Framework.Assert.AreEqual(xml, doc.OuterHtml());
            NUnit.Framework.Assert.AreEqual("//\n\n  foo();\n//", doc.SelectFirst("script").Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesDodgyXmlDecl() {
            String xml = "<?xml version='1.0'><val>One</val>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(xml, "", iText.StyledXmlParser.Jsoup.Parser.Parser.
                XmlParser());
            NUnit.Framework.Assert.AreEqual("One", doc.Select("val").Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesLTinScript() {
            // https://github.com/jhy/jsoup/issues/1139
            String html = "<script> var a=\"<?\"; var b=\"?>\"; </script>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual("<script> var a=\"<!--?\"; var b=\"?-->\"; </script>", doc.Html());
        }

        // converted from pseudo xmldecl to comment
        [NUnit.Framework.Test]
        public virtual void DropsDuplicateAttributes() {
            // case sensitive, so should drop Four and Five
            String html = "<p One=One ONE=Two one=Three One=Four ONE=Five two=Six two=Seven Two=Eight>Text</p>";
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser().SetTrackErrors
                (10);
            Document doc = parser.ParseInput(html, "");
            NUnit.Framework.Assert.AreEqual("<p One=\"One\" ONE=\"Two\" one=\"Three\" two=\"Six\" Two=\"Eight\">Text</p>"
                , doc.SelectFirst("p").OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void ReaderClosedAfterParse() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("Hello", "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            TreeBuilder treeBuilder = doc.Parser().GetTreeBuilder();
            NUnit.Framework.Assert.IsNull(treeBuilder.reader);
            NUnit.Framework.Assert.IsNull(treeBuilder.tokeniser);
        }

        [NUnit.Framework.Test]
        public virtual void XmlParserEnablesXmlOutputAndEscapes() {
            // Test that when using the XML parser, the output mode and escape mode default to XHTML entities
            // https://github.com/jhy/jsoup/issues/1420
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p one='&lt;two&gt;&copy'>Three</p>", "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            NUnit.Framework.Assert.AreEqual(doc.OutputSettings().Syntax(), iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml
                );
            NUnit.Framework.Assert.AreEqual(doc.OutputSettings().EscapeMode(), Entities.EscapeMode.xhtml);
            NUnit.Framework.Assert.AreEqual("<p one=\"&lt;two>©\">Three</p>", doc.Html());
        }

        // only the < should be escaped
        [NUnit.Framework.Test]
        public virtual void XmlSyntaxEscapesLtInAttributes() {
            // Regardless of the entity escape mode, make sure < is escaped in attributes when in XML
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p one='&lt;two&gt;&copy'>Three</p>", "", iText.StyledXmlParser.Jsoup.Parser.Parser
                .XmlParser());
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.extended);
            doc.OutputSettings().Charset("ascii");
            // to make sure &copy; is output
            NUnit.Framework.Assert.AreEqual(doc.OutputSettings().Syntax(), iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml
                );
            NUnit.Framework.Assert.AreEqual("<p one=\"&lt;two>&copy;\">Three</p>", doc.Html());
        }
    }
}
