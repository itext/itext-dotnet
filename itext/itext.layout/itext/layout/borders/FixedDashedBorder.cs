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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Draws a border with a specific dashes around the element it's been set to.</summary>
    public class FixedDashedBorder : Border {
        /// <summary>Default dash unitsOn and unitsOff value.</summary>
        public const float DEFAULT_UNITS_VALUE = 3;

        private readonly float unitsOn;

        private readonly float unitsOff;

        private readonly float phase;

        /// <summary>Creates a FixedDashedBorder with the specified width.</summary>
        /// <param name="width">width of the border</param>
        public FixedDashedBorder(float width)
            : this(ColorConstants.BLACK, width) {
        }

        /// <summary>Creates a FixedDashedBorder with the specified width and the specified color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        public FixedDashedBorder(Color color, float width)
            : this(color, width, 1f) {
        }

        /// <summary>Creates a FixedDashedBorder with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">the opacity which border should have</param>
        public FixedDashedBorder(Color color, float width, float opacity)
            : this(color, width, opacity, DEFAULT_UNITS_VALUE, DEFAULT_UNITS_VALUE, 0) {
        }

        /// <summary>Creates a FixedDashedBorder with the specified width, color, unitsOn, unitsOff and phase.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="unitsOn">the number of units that must be 'on'</param>
        /// <param name="unitsOff">the number of units that must be 'off'</param>
        /// <param name="phase">the value of the phase</param>
        public FixedDashedBorder(Color color, float width, float unitsOn, float unitsOff, float phase)
            : this(color, width, 1f, unitsOn, unitsOff, phase) {
        }

        /// <summary>Creates a FixedDashedBorder with the specified width, color, opacity, unitsOn, unitsOff and phase.
        ///     </summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">the opacity which border should have</param>
        /// <param name="unitsOn">the number of units that must be 'on'</param>
        /// <param name="unitsOff">the number of units that must be 'off'</param>
        /// <param name="phase">the value of the phase</param>
        public FixedDashedBorder(Color color, float width, float opacity, float unitsOn, float unitsOff, float phase
            )
            : base(color, width, opacity) {
            this.unitsOn = unitsOn;
            this.unitsOff = unitsOff;
            this.phase = phase;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float[] startingPoints = GetStartingPointsForBorderSide(x1, y1, x2, y2, defaultSide);
            x1 = startingPoints[0];
            y1 = startingPoints[1];
            x2 = startingPoints[2];
            y2 = startingPoints[3];
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(unitsOn, unitsOff, phase).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float horizontalRadius1
            , float verticalRadius1, float horizontalRadius2, float verticalRadius2, Border.Side defaultSide, float
             borderWidthBefore, float borderWidthAfter) {
            canvas.SaveState().SetLineWidth(width).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(unitsOn, unitsOff, phase);
            Rectangle boundingRectangle = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            float[] horizontalRadii = new float[] { horizontalRadius1, horizontalRadius2 };
            float[] verticalRadii = new float[] { verticalRadius1, verticalRadius2 };
            DrawDiscontinuousBorders(canvas, boundingRectangle, horizontalRadii, verticalRadii, defaultSide, borderWidthBefore
                , borderWidthAfter);
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            canvas.SaveState().SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineDash(unitsOn, unitsOff, phase).SetLineWidth(width).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState
                ();
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return Border.DASHED_FIXED;
        }
    }
}
