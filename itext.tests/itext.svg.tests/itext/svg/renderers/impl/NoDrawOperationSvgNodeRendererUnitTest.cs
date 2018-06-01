using System;
using iText.Svg.Exceptions;

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
    }
}
