using System;
using System.Text;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Processors.Impl;

namespace iText.Svg.Processors {
    public class DefaultSvgConverterPropertiesTest {
        [NUnit.Framework.Test]
        public virtual void GetCharsetNameRegressionTest() {
            String expected = Encoding.UTF8.Name();
            iText.StyledXmlParser.Jsoup.Nodes.Element ellipse = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("ellipse"), "");
            JsoupElementNode jSoupEllipse = new JsoupElementNode(ellipse);
            String actual = new DefaultSvgConverterProperties(jSoupEllipse).GetCharset();
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
