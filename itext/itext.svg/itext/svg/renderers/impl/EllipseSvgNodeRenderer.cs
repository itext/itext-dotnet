using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;circle&gt; tag.
    /// </summary>
    public class EllipseSvgNodeRenderer : AbstractSvgNodeRenderer {
        internal float cx;

        internal float cy;

        internal float rx;

        internal float ry;

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas cv = context.GetCurrentCanvas();
            if (SetParameters()) {
                cv.MoveTo(cx + rx, cy);
                DrawUtils.Arc(cx - rx, cy - ry, cx + rx, cy + ry, 0, 360, cv);
            }
        }

        /// <summary>
        /// Fetches a map of String values by calling getAttribute(Strng s) method
        /// and maps it's values to arc parmateter cx, cy , rx, ry respectively
        /// </summary>
        /// <returns>boolean values to indicate whether all values exit or not</returns>
        protected internal virtual bool SetParameters() {
            cx = 0;
            cy = 0;
            if (GetAttribute(SvgAttributeConstants.CX_ATTRIBUTE) != null) {
                cx = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.CX_ATTRIBUTE));
            }
            if (GetAttribute(SvgAttributeConstants.CY_ATTRIBUTE) != null) {
                cy = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.CY_ATTRIBUTE));
            }
            if (GetAttribute(SvgAttributeConstants.RX_ATTRIBUTE) != null && CssUtils.ParseAbsoluteLength(GetAttribute(
                SvgAttributeConstants.RX_ATTRIBUTE)) > 0) {
                rx = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.RX_ATTRIBUTE));
            }
            else {
                return false;
            }
            //No drawing if rx is absent
            if (GetAttribute(SvgAttributeConstants.RY_ATTRIBUTE) != null && CssUtils.ParseAbsoluteLength(GetAttribute(
                SvgAttributeConstants.RY_ATTRIBUTE)) > 0) {
                ry = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.RY_ATTRIBUTE));
            }
            else {
                return false;
            }
            //No drawing if ry is absent
            return true;
        }
    }
}
