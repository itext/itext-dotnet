using System;
using System.IO;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;

namespace iText.StyledXmlParser.Jsoup {
    public class JsoupXmlParserTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestXmlDeclarationAndComment() {
            String xml = "<?xml version=\"1.0\" standalone=\"no\"?>\n" + "<!-- just declaration and comment -->";
            Stream stream = new MemoryStream(xml.GetBytes());
            IDocumentNode node = new JsoupXmlParser().Parse(stream, "UTF-8");
            // only text (whitespace) child node shall be fetched.
            NUnit.Framework.Assert.AreEqual(1, node.ChildNodes().Count);
        }
    }
}
