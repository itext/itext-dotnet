/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Draws a border with rounded dots around the element it's been set to.</summary>
    /// <remarks>
    /// Draws a border with rounded dots around the element it's been set to. For square dots see
    /// <see cref="DottedBorder"/>.
    /// </remarks>
    public class RoundDotsBorder : Border {
        /// <summary>The modifier to be applied on the width to have the initial gap size</summary>
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

        /// <summary>Creates a RoundDotsBorder with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">width of the border</param>
        public RoundDotsBorder(Color color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return ROUND_DOTS;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap);
            float[] startingPoints = GetStartingPointsForBorderSide(x1, y1, x2, y2, defaultSide);
            x1 = startingPoints[0];
            y1 = startingPoints[1];
            x2 = startingPoints[2];
            y2 = startingPoints[3];
            canvas.SaveState().SetStrokeColor(transparentColor.GetColor()).SetLineWidth(width).SetLineCapStyle(PdfCanvasConstants.LineCapStyle
                .ROUND);
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(0, adjustedGap, adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap);
            bool isHorizontal = false;
            if (Math.Abs(y2 - y1) < 0.0005f) {
                isHorizontal = true;
            }
            if (isHorizontal) {
                x2 -= width;
            }
            canvas.SaveState();
            canvas.SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineWidth(width);
            canvas.SetLineCapStyle(PdfCanvasConstants.LineCapStyle.ROUND);
            canvas.SetLineDash(0, adjustedGap, adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke();
            canvas.RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float horizontalRadius1
            , float verticalRadius1, float horizontalRadius2, float verticalRadius2, Border.Side defaultSide, float
             borderWidthBefore, float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap);
            canvas.SaveState().SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineWidth(width).SetLineCapStyle(PdfCanvasConstants.LineCapStyle.ROUND).SetLineDash(0, adjustedGap
                , adjustedGap / 2);
            Rectangle boundingRectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            float[] horizontalRadii = new float[] { horizontalRadius1, horizontalRadius2 };
            float[] verticalRadii = new float[] { verticalRadius1, verticalRadius2 };
            DrawDiscontinuousBorders(canvas, boundingRectangle, horizontalRadii, verticalRadii, defaultSide, borderWidthBefore
                , borderWidthAfter);
        }
    }
}
