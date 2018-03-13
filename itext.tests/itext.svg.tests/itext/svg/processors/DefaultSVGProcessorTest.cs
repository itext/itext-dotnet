using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Processors {
    public class DefaultSVGProcessorTest {
        //Main success scenario
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestCorrect() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props);
            //setup expected
            ISvgNodeRenderer rootExpected = new DummySvgNodeRenderer("svg", null);
            rootExpected.AddChild(new DummySvgNodeRenderer("circle", rootExpected));
            rootExpected.AddChild(new DummySvgNodeRenderer("path", rootExpected));
            //Compare
            NUnit.Framework.Assert.IsTrue(TestUtil.CompareDummyRendererTrees(rootActual, rootExpected));
        }

        //Edge cases
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNodeHasNullChild() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(null);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props);
            //setup expected
            ISvgNodeRenderer rootExpected = new DummySvgNodeRenderer("svg", null);
        }

        [NUnit.Framework.Ignore("")]
        [NUnit.Framework.ExpectedException(typeof(SvgProcessingException))]
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestLoop() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), null);
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), null);
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), null);
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVG2 = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), null);
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            JsoupElementNode loopNode = new JsoupElementNode(jsoupSVG2);
            root.AddChild(loopNode);
            loopNode.AddChild(root);
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            //Expect an exception
            ISvgNodeRenderer rootActual = processor.Process(root, props);
        }

        [NUnit.Framework.ExpectedException(typeof(SvgProcessingException))]
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNullInput() {
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            processor.Process(null);
        }
    }
}
