/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
