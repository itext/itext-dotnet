using System;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>Tags mapped onto this renderer won't be drawn and will be excluded from the renderer tree when processed.
    ///     </summary>
    /// <remarks>
    /// Tags mapped onto this renderer won't be drawn and will be excluded from the renderer tree when processed.
    /// Different from being added to the ignored list as this Renderer will allow its children to be processed.
    /// </remarks>
    public class NoDrawOperationSvgNodeRenderer : AbstractSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            throw new NotSupportedException(SvgLogMessageConstant.DRAW_NO_DRAW);
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            NoDrawOperationSvgNodeRenderer copy = new NoDrawOperationSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
