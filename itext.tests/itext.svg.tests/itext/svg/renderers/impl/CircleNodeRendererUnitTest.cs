using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class CircleNodeRendererUnitTest {
        [NUnit.Framework.Test]
        public virtual void DeepCopyTest() {
            CircleSvgNodeRenderer expected = new CircleSvgNodeRenderer();
            expected.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            ISvgNodeRenderer actual = expected.CreateDeepCopy();
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
