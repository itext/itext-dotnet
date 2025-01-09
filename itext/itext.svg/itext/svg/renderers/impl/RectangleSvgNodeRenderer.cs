/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;rect&gt; tag.
    /// </summary>
    public class RectangleSvgNodeRenderer : AbstractSvgNodeRenderer {
        private float x = 0f;

        private float y = 0f;

        private float width;

        private float height;

        private bool rxPresent = false;

        private bool ryPresent = false;

        private float rx = 0f;

        private float ry = 0f;

        /// <summary>Constructs a RectangleSvgNodeRenderer.</summary>
        public RectangleSvgNodeRenderer() {
            attributesAndStyles = new Dictionary<String, String>();
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas cv = context.GetCurrentCanvas();
            cv.WriteLiteral("% rect\n");
            SetParameters(context);
            bool singleValuePresent = (rxPresent && !ryPresent) || (!rxPresent && ryPresent);
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

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            SetParameters(context);
            return new Rectangle(this.x, this.y, this.width, this.height);
        }

        private void SetParameters(SvgDrawContext context) {
            if (GetAttribute(SvgConstants.Attributes.X) != null) {
                x = ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.X), context);
            }
            if (GetAttribute(SvgConstants.Attributes.Y) != null) {
                y = ParseVerticalLength(GetAttribute(SvgConstants.Attributes.Y), context);
            }
            width = ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.WIDTH), context);
            height = ParseVerticalLength(GetAttribute(SvgConstants.Attributes.HEIGHT), context);
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.RX)) {
                rx = CheckRadius(ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.RX), context), width);
                rxPresent = true;
            }
            if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.RY)) {
                ry = CheckRadius(ParseVerticalLength(GetAttribute(SvgConstants.Attributes.RY), context), height);
                ryPresent = true;
            }
        }

        private void Arc(float x1, float y1, float x2, float y2, float startAng, float extent, PdfCanvas cv) {
            IList<double[]> ar = PdfCanvas.BezierArc(x1, y1, x2, y2, startAng, extent);
            if (!ar.IsEmpty()) {
                double[] pt;
                for (int k = 0; k < ar.Count; ++k) {
                    pt = ar[k];
                    cv.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// In case of a circular radius, the calculation in
        /// <see cref="CheckRadius(float, float)"/>
        /// isn't enough: the radius cannot be more than half of the <b>smallest</b>
        /// dimension.
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        internal virtual float FindCircularRadius(float rx, float ry, float width, float height) {
            // see https://www.w3.org/TR/SVG/shapes.html#RectElementRYAttribute
            float maxRadius = Math.Min(width, height) / 2f;
            float biggestRadius = Math.Max(rx, ry);
            return Math.Min(maxRadius, biggestRadius);
        }
//\endcond

        public override ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.RectangleSvgNodeRenderer copy = new iText.Svg.Renderers.Impl.RectangleSvgNodeRenderer
                ();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
