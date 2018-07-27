using System;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class NoDrawOperationSvgNodeRendererUnitTest {
        [NUnit.Framework.Test]
        public virtual void DontDrawTest() {
            NUnit.Framework.Assert.That(() =>  {
                NoDrawOperationSvgNodeRenderer renderer = new NoDrawOperationSvgNodeRenderer();
                renderer.DoDraw(null);
            }
            , NUnit.Framework.Throws.TypeOf<NotSupportedException>().With.Message.EqualTo(SvgLogMessageConstant.DRAW_NO_DRAW));
;
        }

        [NUnit.Framework.Test]
        public virtual void DeepCopyTest() {
            NoDrawOperationSvgNodeRenderer expected = new NoDrawOperationSvgNodeRenderer();
            expected.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            ISvgNodeRenderer actual = expected.CreateDeepCopy();
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
