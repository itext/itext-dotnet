using System.IO;
using iText.StyledXmlParser.Node;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;

namespace iText.Svg.Renderers.Impl {
    public class NamedObjectsTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void AddNamedObject() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/NamedObjectsTest/names.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("name_svg") is SvgTagSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("name_rect") is RectangleSvgNodeRenderer);
        }
    }
}
