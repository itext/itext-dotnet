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
    /// <summary>Draws a dotted border around the element it has been set to.</summary>
    /// <remarks>
    /// Draws a dotted border around the element it has been set to. Do note that this border draw square dots,
    /// if you want to draw round dots, see
    /// <see cref="RoundDotsBorder"/>.
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
            float adjustedGap = base.GetDotsGap(borderLength, initialGap + width);
            if (adjustedGap > width) {
                adjustedGap -= width;
            }
            float[] startingPoints = GetStartingPointsForBorderSide(x1, y1, x2, y2, defaultSide);
            x1 = startingPoints[0];
            y1 = startingPoints[1];
            x2 = startingPoints[2];
            y2 = startingPoints[3];
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(width, adjustedGap, width + adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState
                ();
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
            if (adjustedGap > width) {
                adjustedGap -= width;
            }
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(width, adjustedGap, width + adjustedGap / 2);
            Rectangle boundingRectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            float[] horizontalRadii = new float[] { horizontalRadius1, horizontalRadius2 };
            float[] verticalRadii = new float[] { verticalRadius1, verticalRadius2 };
            DrawDiscontinuousBorders(canvas, boundingRectangle, horizontalRadii, verticalRadii, defaultSide, borderWidthBefore
                , borderWidthAfter);
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            float initialGap = width * GAP_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap + width);
            if (adjustedGap > width) {
                adjustedGap -= width;
            }
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(width, adjustedGap, width + adjustedGap / 2).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState
                ();
        }
    }
}
