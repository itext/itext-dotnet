/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Draws a dotted border around the element it has been set to.</summary>
    /// <remarks>
    /// Draws a dotted border around the element it has been set to. Do note that this border draw square dots,
    /// if you want to draw round dots, see
    /// <see cref="RoundDotsBorder"/>
    /// .
    /// </remarks>
    public class DottedBorder : Border {
        /// <summary>The modifier to be applied on the width to have the initial gap size</summary>
        private const float GAP_MODIFIER = 1.5f;

        /// <summary>Creates a DottedBorder instance with the specified width.</summary>
        /// <remarks>Creates a DottedBorder instance with the specified width. The color is set to the default: black.
        ///     </remarks>
        /// <param name="width">width of the border</param>
        public DottedBorder(float width)
            : base(width) {
        }

        /// <summary>Creates a DottedBorder instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        public DottedBorder(Color color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a DottedBorder with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">width of the border</param>
        public DottedBorder(Color color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return Border.DOTTED;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap + width);
            if (adjustedGap > width) {
                adjustedGap -= width;
            }
            float widthHalf = width / 2;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
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
            canvas.SetLineDash(width, adjustedGap, width + adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState
                ();
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float horizontalRadius1
            , float verticalRadius1, float horizontalRadius2, float verticalRadius2, Border.Side defaultSide, float
             borderWidthBefore, float borderWidthAfter) {
            float curv = 0.447f;
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap);
            if (adjustedGap > width) {
                adjustedGap -= width;
            }
            // Points (x0, y0) and (x3, y3) are used to produce Bezier curve
            float x0 = x1;
            float y0 = y1;
            float x3 = x2;
            float y3 = y2;
            float innerRadiusBefore;
            float innerRadiusFirst;
            float innerRadiusSecond;
            float innerRadiusAfter;
            float widthHalf = width / 2;
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(width, adjustedGap, width + adjustedGap / 2);
            Point clipPoint1;
            Point clipPoint2;
            Point clipPoint;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    innerRadiusBefore = Math.Max(0, horizontalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, verticalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, verticalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, horizontalRadius2 - borderWidthAfter);
                    x0 -= borderWidthBefore / 2;
                    y0 -= innerRadiusFirst;
                    x3 += borderWidthAfter / 2;
                    y3 -= innerRadiusSecond;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 - borderWidthBefore, y1 + width), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0 + 10, y0));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 + borderWidthAfter, y2 + width), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3 - 10, y3));
                    if (clipPoint1.x > clipPoint2.x) {
                        clipPoint = GetIntersectionPoint(new Point(x1 - borderWidthBefore, y1 + width), clipPoint1, clipPoint2, new 
                            Point(x2 + borderWidthAfter, y2 + width));
                        canvas.MoveTo(x1 - borderWidthBefore, y1 + width).LineTo(clipPoint.x, clipPoint.y).LineTo(x2 + borderWidthAfter
                            , y2 + width).LineTo(x1 - borderWidthBefore, y1 + width);
                    }
                    else {
                        canvas.MoveTo(x1 - borderWidthBefore, y1 + width).LineTo(clipPoint1.x, clipPoint1.y).LineTo(clipPoint2.x, 
                            clipPoint2.y).LineTo(x2 + borderWidthAfter, y2 + width).LineTo(x1 - borderWidthBefore, y1 + width);
                    }
                    canvas.Clip().NewPath();
                    x1 += innerRadiusBefore;
                    y1 += widthHalf;
                    x2 -= innerRadiusAfter;
                    y2 += widthHalf;
                    canvas.MoveTo(x0, y0).CurveTo(x0, y0 + innerRadiusFirst * curv, x1 - innerRadiusBefore * curv, y1, x1, y1)
                        .LineTo(x2, y2).CurveTo(x2 + innerRadiusAfter * curv, y2, x3, y3 + innerRadiusSecond * curv, x3, y3);
                    break;
                }

                case Border.Side.RIGHT: {
                    innerRadiusBefore = Math.Max(0, verticalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, horizontalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, horizontalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, verticalRadius2 - borderWidthAfter);
                    x0 -= innerRadiusFirst;
                    y0 += borderWidthBefore / 2;
                    x3 -= innerRadiusSecond;
                    y3 -= borderWidthAfter;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 + width, y1 + borderWidthBefore), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0, y0 - 10));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 + width, y2 - borderWidthAfter), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3, y3 - 10));
                    if (clipPoint1.y < clipPoint2.y) {
                        clipPoint = GetIntersectionPoint(new Point(x1 + width, y1 + borderWidthBefore), clipPoint1, clipPoint2, new 
                            Point(x2 + width, y2 - borderWidthAfter));
                        canvas.MoveTo(x1 + width, y1 + borderWidthBefore).LineTo(clipPoint.x, clipPoint.y).LineTo(x2 + width, y2 -
                             borderWidthAfter).LineTo(x1 + width, y1 + borderWidthBefore).Clip().NewPath();
                    }
                    else {
                        canvas.MoveTo(x1 + width, y1 + borderWidthBefore).LineTo(clipPoint1.x, clipPoint1.y).LineTo(clipPoint2.x, 
                            clipPoint2.y).LineTo(x2 + width, y2 - borderWidthAfter).LineTo(x1 + width, y1 + borderWidthBefore).Clip
                            ().NewPath();
                    }
                    canvas.Clip().NewPath();
                    x1 += widthHalf;
                    y1 -= innerRadiusBefore;
                    x2 += widthHalf;
                    y2 += innerRadiusAfter;
                    canvas.MoveTo(x0, y0).CurveTo(x0 + innerRadiusFirst * curv, y0, x1, y1 + innerRadiusBefore * curv, x1, y1)
                        .LineTo(x2, y2).CurveTo(x2, y2 - innerRadiusAfter * curv, x3 + innerRadiusSecond * curv, y3, x3, y3);
                    break;
                }

                case Border.Side.BOTTOM: {
                    innerRadiusBefore = Math.Max(0, horizontalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, verticalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, verticalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, horizontalRadius2 - borderWidthAfter);
                    x0 += borderWidthBefore / 2;
                    y0 += innerRadiusFirst;
                    x3 -= borderWidthAfter / 2;
                    y3 += innerRadiusSecond;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 + borderWidthBefore, y1 - width), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0 - 10, y0));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 - borderWidthAfter, y2 - width), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3 + 10, y3));
                    if (clipPoint1.x < clipPoint2.x) {
                        clipPoint = GetIntersectionPoint(new Point(x1 + borderWidthBefore, y1 - width), clipPoint1, clipPoint2, new 
                            Point(x2 - borderWidthAfter, y2 - width));
                        canvas.MoveTo(x1 + borderWidthBefore, y1 - width).LineTo(clipPoint.x, clipPoint.y).LineTo(x2 - borderWidthAfter
                            , y2 - width).LineTo(x1 + borderWidthBefore, y1 - width);
                    }
                    else {
                        canvas.MoveTo(x1 + borderWidthBefore, y1 - width).LineTo(clipPoint1.x, clipPoint1.y).LineTo(clipPoint2.x, 
                            clipPoint2.y).LineTo(x2 - borderWidthAfter, y2 - width).LineTo(x1 + borderWidthBefore, y1 - width);
                    }
                    canvas.Clip().NewPath();
                    x1 -= innerRadiusBefore;
                    y1 -= widthHalf;
                    x2 += innerRadiusAfter;
                    y2 -= widthHalf;
                    canvas.MoveTo(x0, y0).CurveTo(x0, y0 - innerRadiusFirst * curv, x1 + innerRadiusBefore * curv, y1, x1, y1)
                        .LineTo(x2, y2).CurveTo(x2 - innerRadiusAfter * curv, y2, x3, y3 - innerRadiusSecond * curv, x3, y3);
                    break;
                }

                case Border.Side.LEFT: {
                    innerRadiusBefore = Math.Max(0, verticalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, horizontalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, horizontalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, verticalRadius2 - borderWidthAfter);
                    x0 += innerRadiusFirst;
                    y0 -= borderWidthBefore / 2;
                    x3 += innerRadiusSecond;
                    y3 += borderWidthAfter;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 - width, y1 - borderWidthBefore), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0, y0 + 10));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 - width, y2 + borderWidthAfter), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3, y3 + 10));
                    if (clipPoint1.y > clipPoint2.y) {
                        clipPoint = GetIntersectionPoint(new Point(x1 - width, y1 - borderWidthBefore), clipPoint1, clipPoint2, new 
                            Point(x2 - width, y2 + borderWidthAfter));
                        canvas.MoveTo(x1 - width, y1 - borderWidthBefore).LineTo(clipPoint.x, clipPoint.y).LineTo(x2 - width, y2 +
                             borderWidthAfter).LineTo(x1 - width, y1 - borderWidthBefore);
                    }
                    else {
                        canvas.MoveTo(x1 - width, y1 - borderWidthBefore).LineTo(clipPoint1.x, clipPoint1.y).LineTo(clipPoint2.x, 
                            clipPoint2.y).LineTo(x2 - width, y2 + borderWidthAfter).LineTo(x1 - width, y1 - borderWidthBefore);
                    }
                    canvas.Clip().NewPath();
                    x1 -= widthHalf;
                    y1 += innerRadiusBefore;
                    x2 -= widthHalf;
                    y2 -= innerRadiusAfter;
                    canvas.MoveTo(x0, y0).CurveTo(x0 - innerRadiusFirst * curv, y0, x1, y1 - innerRadiusBefore * curv, x1, y1)
                        .LineTo(x2, y2).CurveTo(x2, y2 + innerRadiusAfter * curv, x3 - innerRadiusSecond * curv, y3, x3, y3);
                    break;
                }
            }
            canvas.Stroke().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap + width);
            if (adjustedGap > width) {
                adjustedGap -= width;
            }
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(width, adjustedGap, width + adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState
                ();
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
            double gapsNum = Math.Ceiling(distance / initialGap);
            if (gapsNum == 0) {
                return initialGap;
            }
            return (float)(distance / gapsNum);
        }
    }
}
