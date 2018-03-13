using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Dummy.Renderers.Impl {
    public class DummyProcessableSvgNodeRenderer : DummySvgNodeRenderer {
        private bool processed = false;

        public override void Draw(SvgDrawContext context) {
            if (processed) {
                throw new SvgProcessingException("Cannot process svg renderer twice");
            }
            processed = true;
        }

        public virtual bool IsProcessed() {
            return processed;
        }
    }
}
