using iText.Svg.Exceptions;

namespace iText.Svg.Renderers.Factories {
    public class DefaultSvgNodeRendererFactoryTest {
        [NUnit.Framework.Test]
        public virtual void CreateSvgNodeRenderer() {
            NUnit.Framework.Assert.That(() =>  {
                ISvgNodeRendererFactory nodeRendererFactory = new DefaultSvgNodeRendererFactory(null);
                nodeRendererFactory.CreateSvgNodeRendererForTag(null, null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TAGPARAMETERNULL));
;
        }
    }
}
