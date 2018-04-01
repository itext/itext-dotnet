using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;rect&gt; tag.
    /// </summary>
    public class RectangleSvgNodeRenderer : AbstractSvgNodeRenderer {
        public RectangleSvgNodeRenderer() {
            attributesAndStyles = new Dictionary<String, String>();
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas cv = context.GetCurrentCanvas();
            cv.WriteLiteral("% rect\n");
            float x = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.X_ATTRIBUTE));
            float y = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.Y_ATTRIBUTE));
            float width = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.WIDTH_ATTRIBUTE));
            float height = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.HEIGHT_ATTRIBUTE));
            bool rxPresent = false;
            bool ryPresent = false;
            float rx = 0f;
            float ry = 0f;
            if (attributesAndStyles.ContainsKey(SvgAttributeConstants.RX_ATTRIBUTE)) {
                rx = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.RX_ATTRIBUTE));
                rxPresent = true;
            }
            if (attributesAndStyles.ContainsKey(SvgAttributeConstants.RY_ATTRIBUTE)) {
                ry = CssUtils.ParseAbsoluteLength(GetAttribute(SvgAttributeConstants.RY_ATTRIBUTE));
                ryPresent = true;
            }
            bool singleValuePresent = (rxPresent && !ryPresent) || (!rxPresent && ryPresent);
            // these checks should happen in all cases
            rx = CheckRadius(rx, width);
            ry = CheckRadius(ry, height);
            if (!rxPresent && !ryPresent) {
                cv.Rectangle(x, y, width, height);
            }
            else {
                if (singleValuePresent) {
                    cv.WriteLiteral("% circle rounded rect\n");
                    // only look for radius in case of circular rounding
                    float radius = FindCircularRadius(rx, ry, width, height);
                    cv.RoundRectangle(x, y, width, height, radius);
                }
                else {
                    cv.WriteLiteral("% ellipse rounded rect\n");
                    // TODO (DEVSIX-1878): this should actually be refactored into PdfCanvas.roundRectangle()
                    /*
                    
                    y+h    ->    ____________________________
                    /                            \
                    /                              \
                    y+h-ry -> /                                \
                    |                                |
                    |                                |
                    |                                |
                    |                                |
                    y+ry   -> \                                /
                    \                              /
                    y      ->   \____________________________/
                    ^  ^                          ^  ^
                    x  x+rx                  x+w-rx  x+w
                    
                    */
                    cv.MoveTo(x + rx, y);
                    cv.LineTo(x + width - rx, y);
                    Arc(x + width - 2 * rx, y, x + width, y + 2 * ry, -90, 90, cv);
                    cv.LineTo(x + width, y + height - ry);
                    Arc(x + width, y + height - 2 * ry, x + width - 2 * rx, y + height, 0, 90, cv);
                    cv.LineTo(x + rx, y + height);
                    Arc(x + 2 * rx, y + height, x, y + height - 2 * ry, 90, 90, cv);
                    cv.LineTo(x, y + ry);
                    Arc(x, y + 2 * ry, x + 2 * rx, y, 180, 90, cv);
                    cv.ClosePath();
                }
            }
        }

        private void Arc(float x1, float y1, float x2, float y2, float startAng, float extent, PdfCanvas cv) {
            IList<double[]> ar = PdfCanvas.BezierArc(x1, y1, x2, y2, startAng, extent);
            if (ar.IsEmpty()) {
                return;
            }
            double[] pt;
            for (int k = 0; k < ar.Count; ++k) {
                pt = ar[k];
                cv.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
            }
        }

        /// <summary>
        /// a radius must be positive, and cannot be more than half the distance in
        /// the dimension it is for.
        /// </summary>
        /// <remarks>
        /// a radius must be positive, and cannot be more than half the distance in
        /// the dimension it is for.
        /// e.g. rx &lt;= width / 2
        /// </remarks>
        internal virtual float CheckRadius(float radius, float distance) {
            if (radius <= 0f) {
                return 0f;
            }
            if (radius > distance / 2f) {
                return distance / 2f;
            }
            return radius;
        }

        /// <summary>
        /// In case of a circular radius, the calculation in
        /// <see cref="CheckRadius(float, float)"/>
        /// isn't enough: the radius cannot be more than half of the <b>smallest</b>
        /// dimension.
        /// This method assumes that
        /// <see cref="CheckRadius(float, float)"/>
        /// has already run, and it is
        /// silently assumed (though not necessary for this method) that either
        /// <paramref name="rx"/>
        /// or
        /// <paramref name="ry"/>
        /// is zero.
        /// </summary>
        internal virtual float FindCircularRadius(float rx, float ry, float width, float height) {
            // see https://www.w3.org/TR/SVG/shapes.html#RectElementRYAttribute
            float maxRadius = Math.Min(width, height) / 2f;
            float biggestRadius = Math.Max(rx, ry);
            return Math.Min(maxRadius, biggestRadius);
        }
    }
}
