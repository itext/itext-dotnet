using iText.Kernel.Font;
using iText.StyledXmlParser.Exceptions;
using iText.Svg;

namespace iText.Svg.Renderers.Impl {
    public class TextLeafSvgNodeRendererIntegrationTest {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void GetContentLengthBaseTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            toTest.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "10");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 22.78f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void GetContentLengthNoValueTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 27.336f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void GetContentLengthNaNTest() {
            NUnit.Framework.Assert.That(() =>  {
                TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
                toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
                toTest.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "spice");
                PdfFont font = PdfFontFactory.CreateFont();
                float actual = toTest.GetTextContentLength(12, font);
                float expected = 27.336f;
                NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>())
;
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void GetContentLengthNegativeTest() {
            TextLeafSvgNodeRenderer toTest = new TextLeafSvgNodeRenderer();
            toTest.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, "Hello");
            toTest.SetAttribute(SvgConstants.Attributes.FONT_SIZE, "-10");
            PdfFont font = PdfFontFactory.CreateFont();
            float actual = toTest.GetTextContentLength(12, font);
            float expected = 27.336f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-6f);
        }
    }
}
