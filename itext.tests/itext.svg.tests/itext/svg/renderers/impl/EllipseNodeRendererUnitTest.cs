using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class EllipseNodeRendererUnitTest {
        [NUnit.Framework.Test]
        public virtual void DeepCopyTest() {
            EllipseSvgNodeRenderer expected = new EllipseSvgNodeRenderer();
            expected.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            ISvgNodeRenderer actual = expected.CreateDeepCopy();
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
