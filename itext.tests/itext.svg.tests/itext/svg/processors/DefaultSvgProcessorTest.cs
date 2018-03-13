using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Css;
using iText.Svg.Exceptions;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Processors {
    public class DefaultSvgProcessorTest {
        //Main success scenario
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestCorrectSimple() {
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
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestCorrectNested() {
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
            INode nestedSvg = new JsoupElementNode(jsoupSVGRoot);
            nestedSvg.AddChild(new JsoupElementNode(jsoupSVGCircle));
            nestedSvg.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(nestedSvg);
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props);
            //setup expected
            ISvgNodeRenderer rootExpected = new DummySvgNodeRenderer("svg", null);
            rootExpected.AddChild(new DummySvgNodeRenderer("circle", rootExpected));
            rootExpected.AddChild(new DummySvgNodeRenderer("path", rootExpected));
            ISvgNodeRenderer nestedSvgRend = new DummySvgNodeRenderer("svg", rootExpected);
            nestedSvgRend.AddChild(new DummySvgNodeRenderer("circle", nestedSvgRend));
            nestedSvgRend.AddChild(new DummySvgNodeRenderer("circle", nestedSvgRend));
            rootExpected.AddChild(nestedSvgRend);
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
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

        [NUnit.Framework.Test]
        public virtual void DummyProcessingSvgTagIsNotRootOfInput() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupRandomElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("body"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            INode root = new JsoupElementNode(jsoupRandomElement);
            INode svg = new JsoupElementNode(jsoupSVGRoot);
            svg.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(svg);
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props);
            //setup expected
            ISvgNodeRenderer rootExpected = new DummySvgNodeRenderer("svg", null);
            rootExpected.AddChild(new DummySvgNodeRenderer("circle", rootExpected));
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingNoSvgTagInInput() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                    .ValueOf("polygon"), "");
                iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                    .ValueOf("circle"), "");
                INode root = new JsoupElementNode(jsoupSVGRoot);
                root.AddChild(new JsoupElementNode(jsoupSVGCircle));
                //Run
                DefaultSvgProcessor processor = new DefaultSvgProcessor();
                ISvgConverterProperties props = new DummySvgConverterProperties();
                ISvgNodeRenderer rootActual = processor.Process(root, props);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.NOROOT));
;
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNullInput() {
            NUnit.Framework.Assert.That(() =>  {
                DefaultSvgProcessor processor = new DefaultSvgProcessor();
                processor.Process(null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        [NUnit.Framework.Ignore("TODO: Decide on default behaviour. Blocked by RND-799\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestNoPassedProperties() {
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
            ISvgNodeRenderer rootActual = processor.Process(root);
            //setup expected
            ISvgNodeRenderer rootExpected = null;
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Ignore("TODO: Decide on default behaviour. Blocked by RND-799\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestPassedPropertiesNull() {
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
            ISvgNodeRenderer rootActual = processor.Process(root, null);
            //setup expected
            ISvgNodeRenderer rootExpected = null;
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Ignore("TODO: Decide on default behaviour. Blocked by RND-799\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestPassedPropertiesReturnNullValues() {
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
            ISvgConverterProperties convProps = new _ISvgConverterProperties_207();
            ISvgNodeRenderer rootActual = processor.Process(root, convProps);
            //setup expected
            ISvgNodeRenderer rootExpected = null;
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        private sealed class _ISvgConverterProperties_207 : ISvgConverterProperties {
            public _ISvgConverterProperties_207() {
            }

            public ICssResolver GetCssResolver() {
                return null;
            }

            public ISvgNodeRendererFactory GetRendererFactory() {
                return null;
            }
        }
    }
}
