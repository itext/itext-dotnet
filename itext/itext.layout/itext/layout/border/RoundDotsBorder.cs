/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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

namespace iText.Layout.Border {
    /// <summary>Draws a border with rounded dots aroudn the element it's been set to.</summary>
    /// <remarks>
    /// Draws a border with rounded dots aroudn the element it's been set to. For square dots see
    /// <see cref="DottedBorder"/>
    /// .
    /// </remarks>
    public class RoundDotsBorder : iText.Layout.Border.Border {
        private const float GAP_MODIFIER = 2.5f;

        /// <summary>Creates a RoundDotsBorder with the specified wit?dth and sets the color to black.</summary>
        /// <param name="width">width of the border</param>
        public RoundDotsBorder(float width)
            : base(width) {
        }

        /// <summary>Creates a RoundDotsBorder with the specified wit?dth and the specified color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        public RoundDotsBorder(Color color, float width)
            : base(color, width) {
        }

        public override int GetBorderType() {
            return iText.Layout.Border.Border.ROUND_DOTS;
        }

        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float borderWidthBefore
            , float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap);
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
            canvas.SetStrokeColor(color);
            canvas.SetLineWidth(width);
            canvas.SetLineCapStyle(PdfCanvasConstants.LineCapStyle.ROUND);
            canvas.SetLineDash(0, adjustedGap, adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke();
        }

        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = GetDotsGap(borderLength, initialGap);
            bool isHorizontal = false;
            if (Math.Abs(y2 - y1) < 0.0005f) {
                isHorizontal = true;
            }
            if (isHorizontal) {
                x2 -= width;
            }
            canvas.SetStrokeColor(color);
            canvas.SetLineWidth(width);
            canvas.SetLineCapStyle(PdfCanvasConstants.LineCapStyle.ROUND);
            canvas.SetLineDash(0, adjustedGap, adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke();
        }

        protected internal virtual float GetDotsGap(double distance, float initialGap) {
            double gapsNum = System.Math.Ceiling(distance / initialGap);
            return (float)(distance / gapsNum);
        }
    }
}
