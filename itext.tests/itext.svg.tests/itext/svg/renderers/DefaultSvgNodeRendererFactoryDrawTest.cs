using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Dummy.Factories;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Renderers {
    public class DefaultSvgNodeRendererFactoryDrawTest {
        private ISvgNodeRendererFactory fact;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            fact = new DefaultSvgNodeRendererFactory(new DummySvgNodeMapper());
        }

        [NUnit.Framework.Test]
        public virtual void BasicProcessedRendererTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("processable"), "");
            IElementNode tag = new JsoupElementNode(element);
            ISvgNodeRenderer renderer = fact.CreateSvgNodeRendererForTag(tag, null);
            NUnit.Framework.Assert.IsTrue(renderer is DummyProcessableSvgNodeRenderer);
            renderer.Draw(new SvgDrawContext());
            DummyProcessableSvgNodeRenderer processed = (DummyProcessableSvgNodeRenderer)renderer;
            NUnit.Framework.Assert.IsTrue(processed.IsProcessed());
        }

        [NUnit.Framework.Test]
        public virtual void NestedProcessedRendererTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element parentEl = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("processable"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element childEl = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("processable"), "");
            IElementNode parentTag = new JsoupElementNode(parentEl);
            IElementNode childTag = new JsoupElementNode(childEl);
            ISvgNodeRenderer parentRenderer = fact.CreateSvgNodeRendererForTag(parentTag, null);
            ISvgNodeRenderer childRenderer = fact.CreateSvgNodeRendererForTag(childTag, parentRenderer);
            parentRenderer.Draw(new SvgDrawContext());
            DummyProcessableSvgNodeRenderer parentProcessed = (DummyProcessableSvgNodeRenderer)parentRenderer;
            NUnit.Framework.Assert.IsTrue(parentProcessed.IsProcessed());
            DummyProcessableSvgNodeRenderer childProcessed = (DummyProcessableSvgNodeRenderer)childRenderer;
            // child is not processed unless instructed thus in its parent
            NUnit.Framework.Assert.IsFalse(childProcessed.IsProcessed());
        }
    }
}
