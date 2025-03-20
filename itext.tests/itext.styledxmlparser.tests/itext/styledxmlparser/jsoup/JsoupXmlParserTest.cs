/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.IO;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Jsoup {
    [NUnit.Framework.Category("UnitTest")]
    public class JsoupXmlParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestXmlDeclarationAndComment() {
            String xml = "<?xml version=\"1.0\" standalone=\"no\"?>\n" + "<!-- just declaration and comment -->";
            Stream stream = new MemoryStream(xml.GetBytes());
            IDocumentNode node = new JsoupXmlParser().Parse(stream, "UTF-8");
            NUnit.Framework.Assert.AreEqual(2, node.ChildNodes().Count);
            NUnit.Framework.Assert.IsTrue(node.ChildNodes()[0] is IXmlDeclarationNode);
            NUnit.Framework.Assert.IsTrue(node.ChildNodes()[1] is ITextNode);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_ADDING_CHILD_NODE)]
        public virtual void TestMessageAddingChild() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(null);
            NUnit.Framework.Assert.AreEqual(0, root.ChildNodes().Count);
        }
    }
}
