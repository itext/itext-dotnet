using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class GroupSvgNodeRendererUnitTest {
        [NUnit.Framework.Ignore("RND-880, list comparison fails")]
        [NUnit.Framework.Test]
        public virtual void DeepCopyTest() {
            GroupSvgNodeRenderer expected = new GroupSvgNodeRenderer();
            expected.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            expected.AddChild(new CircleSvgNodeRenderer());
            ISvgNodeRenderer actual = expected.CreateDeepCopy();
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
