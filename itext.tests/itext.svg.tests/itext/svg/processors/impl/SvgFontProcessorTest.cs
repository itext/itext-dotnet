using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Processors;

namespace iText.Svg.Processors.Impl {
    public class SvgFontProcessorTest {
        /// <exception cref="System.IO.FileNotFoundException"/>
        [NUnit.Framework.Test]
        public virtual void AddFontFaceFontsTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\t@font-face{\n" + "\t\tfont-family:Courier;\n" + "\t\tsrc:local(Courier);\n"
                 + "\t}\n" + "  ", "");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            ISvgConverterProperties properties = new DefaultSvgConverterProperties(jSoupStyle);
            ProcessorContext context = new ProcessorContext(new DefaultSvgConverterProperties());
            ICssResolver cssResolver = properties.GetCssResolver();
            SvgFontProcessor svgFontProcessor = new SvgFontProcessor(context);
            svgFontProcessor.AddFontFaceFonts(cssResolver);
            FontInfo info = (FontInfo)context.GetTempFonts().GetFonts().ToArray()[0];
            NUnit.Framework.Assert.AreEqual("Courier", info.GetFontName());
        }
    }
}
