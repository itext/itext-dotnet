using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Converter;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg {
    public class DeprecatedApiTest : ExtendedITextTest {
        //This test class can safely be removed in 7.2
        [NUnit.Framework.Test]
        public virtual void ProcessNullTest() {
            NUnit.Framework.Assert.That(() =>  {
                SvgConverter.Process(null);
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void ProcessNode() {
            INode svg = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), ""));
            IBranchSvgNodeRenderer node = (IBranchSvgNodeRenderer)SvgConverter.Process(svg).GetRootRenderer();
            NUnit.Framework.Assert.IsTrue(node is SvgTagSvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(0, node.GetChildren().Count);
            NUnit.Framework.Assert.IsNull(node.GetParent());
        }
    }
}
