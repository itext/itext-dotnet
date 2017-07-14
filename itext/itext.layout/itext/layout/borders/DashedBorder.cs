/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Draws a border with dashes around the element it's been set to.</summary>
    public class DashedBorder : Border {
        /// <summary>The modifier to be applied on the width to have the dash size</summary>
        private const float DASH_MODIFIER = 5f;

        /// <summary>The modifier to be applied on the width to have the initial gap size</summary>
        private const float GAP_MODIFIER = 3.5f;

        /// <summary>Creates a DashedBorder with the specified width and sets the color to black.</summary>
        /// <param name="width">width of the border</param>
        public DashedBorder(float width)
            : base(width) {
        }

        /// <summary>Creates a DashedBorder with the specified width and the specified color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        public DashedBorder(Color color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a DashedBorder with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">width of the border</param>
        public DashedBorder(Color color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return Border.DASHED;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float borderWidthBefore
            , float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dash = width * DASH_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap + dash);
            if (adjustedGap > dash) {
                adjustedGap -= dash;
            }
            float widthHalf = width / 2;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2);
            switch (borderSide) {
                case Border.Side.TOP: {
                    y1 += widthHalf;
                    y2 += widthHalf;
                    break;
                }

                case Border.Side.RIGHT: {
                    x1 += widthHalf;
                    x2 += widthHalf;
                    break;
                }

                case Border.Side.BOTTOM: {
                    y1 -= widthHalf;
                    y2 -= widthHalf;
                    break;
                }

                case Border.Side.LEFT: {
                    x1 -= widthHalf;
                    x2 -= widthHalf;
                    break;
                }
            }
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(dash, adjustedGap, dash + adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState
                ();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2) {
            float initialGap = width * GAP_MODIFIER;
            float dash = width * DASH_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap + dash);
            if (adjustedGap > dash) {
                adjustedGap -= dash;
            }
            canvas.SaveState().SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(dash, adjustedGap, dash + adjustedGap / 2).SetLineWidth(width).MoveTo(x1, y1).LineTo(x2
                , y2).Stroke().RestoreState();
        }

        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float outerRadius, float
             borderWidthBefore, float borderWidthAfter) {
            float curv = 0.447f;
            float initialGap = width * GAP_MODIFIER;
            float dash = width * DASH_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap + dash);
            if (adjustedGap > dash) {
                adjustedGap -= dash;
            }
            float x0 = x1;
            float y0 = y1;
            float x3 = x2;
            float y3 = y2;
            float innerRadiusBefore = Math.Max(0, outerRadius - borderWidthBefore);
            float innerRadius = Math.Max(0, outerRadius - width);
            float innerRadiusAfter = Math.Max(0, outerRadius - borderWidthAfter);
            float widthHalf = width / 2;
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(dash, adjustedGap, dash + adjustedGap / 2);
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2);
            switch (borderSide) {
                case Border.Side.TOP: {
                    x0 -= borderWidthBefore / 2;
                    y0 -= innerRadius;
                    x3 += borderWidthAfter / 2;
                    y3 -= innerRadius;
                    x1 += innerRadiusBefore;
                    y1 += widthHalf;
                    x2 -= innerRadiusAfter;
                    y2 += widthHalf;
                    canvas.MoveTo(x0, y0).CurveTo(x0, y0 + innerRadius * curv, x1 - innerRadiusBefore * curv, y1, x1, y1).LineTo
                        (x2, y2).CurveTo(x2 + innerRadiusAfter * curv, y2, x3, y3 + innerRadius * curv, x3, y3);
                    break;
                }

                case Border.Side.RIGHT: {
                    x0 -= innerRadius;
                    y0 += borderWidthBefore / 2;
                    x3 -= innerRadius;
                    y3 -= borderWidthAfter;
                    x1 += widthHalf;
                    y1 -= innerRadiusBefore;
                    x2 += widthHalf;
                    y2 += innerRadiusAfter;
                    canvas.MoveTo(x0, y0).CurveTo(x0 + innerRadius * curv, y0, x1, y1 + innerRadiusBefore * curv, x1, y1).LineTo
                        (x2, y2).CurveTo(x2, y2 - innerRadiusAfter * curv, x3 + innerRadius * curv, y3, x3, y3);
                    break;
                }

                case Border.Side.BOTTOM: {
                    x0 += borderWidthBefore / 2;
                    y0 += innerRadius;
                    x3 -= borderWidthAfter / 2;
                    y3 += innerRadius;
                    x1 -= innerRadiusBefore;
                    y1 -= widthHalf;
                    x2 += innerRadiusAfter;
                    y2 -= widthHalf;
                    canvas.MoveTo(x0, y0).CurveTo(x0, y0 - innerRadius * curv, x1 + innerRadiusBefore * curv, y1, x1, y1).LineTo
                        (x2, y2).CurveTo(x2 - innerRadiusAfter * curv, y2, x3, y3 - innerRadius * curv, x3, y3);
                    break;
                }

                case Border.Side.LEFT: {
                    x0 += innerRadius;
                    y0 -= borderWidthBefore / 2;
                    x3 += innerRadius;
                    y3 -= borderWidthAfter;
                    x1 -= widthHalf;
                    y1 += innerRadiusBefore;
                    x2 -= widthHalf;
                    y2 -= innerRadiusAfter;
                    canvas.MoveTo(x0, y0).CurveTo(x0 - innerRadius * curv, y0, x1, y1 - innerRadiusBefore * curv, x1, y1).LineTo
                        (x2, y2).CurveTo(x2, y2 + innerRadiusAfter * curv, x3 - innerRadius * curv, y3, x3, y3);
                    break;
                }
            }
            canvas.Stroke().RestoreState();
        }

        /// <summary>Adjusts the size of the gap between dots</summary>
        /// <param name="distance">
        /// the
        /// <see cref="Border">border</see>
        /// length
        /// </param>
        /// <param name="initialGap">the initial size of the gap</param>
        /// <returns>the adjusted size of the gap</returns>
        protected internal virtual float GetDotsGap(double distance, float initialGap) {
            double gapsNum = System.Math.Ceiling(distance / initialGap);
            if (gapsNum == 0) {
                return initialGap;
            }
            return (float)(distance / gapsNum);
        }
    }
}
