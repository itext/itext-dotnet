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
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Properties {
    /// <summary>A POJO that describes the underline of a layout element.</summary>
    /// <remarks>
    /// A POJO that describes the underline of a layout element.
    /// This class is to be used as a property for an element or renderer,
    /// as the value for
    /// <see cref="Property.UNDERLINE"/>
    /// </remarks>
    public class Underline {
        protected internal TransparentColor transparentColor;

        protected internal float thickness;

        protected internal float thicknessMul;

        protected internal float yPosition;

        protected internal float yPositionMul;

        protected internal int lineCapStyle = PdfCanvasConstants.LineCapStyle.BUTT;

        /// <summary>Creates an Underline.</summary>
        /// <remarks>
        /// Creates an Underline. Both the thickness and vertical positioning under
        /// the text element's base line can be set to a fixed value, or a variable
        /// one depending on the element's font size.
        /// If you want a fixed-width thickness, set <c>thicknessMul</c> to 0;
        /// if you want a thickness solely dependent on the font size, set
        /// <c>thickness</c> to 0.
        /// Mutatis mutandis for the y-position.
        /// </remarks>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of the underline
        /// </param>
        /// <param name="thickness">a float defining the minimum thickness in points of the underline</param>
        /// <param name="thicknessMul">a float defining the font size dependent component of the thickness of the underline
        ///     </param>
        /// <param name="yPosition">a float defining the default absolute vertical distance in points from the text's base line
        ///     </param>
        /// <param name="yPositionMul">a float defining the font size dependent component of the vertical positioning of the underline
        ///     </param>
        /// <param name="lineCapStyle">
        /// the way the underline finishes at its edges.
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineCapStyle"/>
        /// </param>
        public Underline(Color color, float thickness, float thicknessMul, float yPosition, float yPositionMul, int
             lineCapStyle)
            : this(color, 1f, thickness, thicknessMul, yPosition, yPositionMul, lineCapStyle) {
        }

        /// <summary>Creates an Underline.</summary>
        /// <remarks>
        /// Creates an Underline. Both the thickness and vertical positioning under
        /// the text element's base line can be set to a fixed value, or a variable
        /// one depending on the element's font size.
        /// If you want a fixed-width thickness, set <c>thicknessMul</c> to 0;
        /// if you want a thickness solely dependent on the font size, set
        /// <c>thickness</c> to 0.
        /// Mutatis mutandis for the y-position.
        /// </remarks>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of the underline
        /// </param>
        /// <param name="opacity">a float defining the opacity of the underline; a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent
        ///     </param>
        /// <param name="thickness">a float defining the minimum thickness in points of the underline</param>
        /// <param name="thicknessMul">a float defining the font size dependent component of the thickness of the underline
        ///     </param>
        /// <param name="yPosition">a float defining the default absolute vertical distance in points from the text's base line
        ///     </param>
        /// <param name="yPositionMul">a float defining the font size dependent component of the vertical positioning of the underline
        ///     </param>
        /// <param name="lineCapStyle">
        /// the way the underline finishes at its edges.
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineCapStyle"/>
        /// </param>
        public Underline(Color color, float opacity, float thickness, float thicknessMul, float yPosition, float yPositionMul
            , int lineCapStyle) {
            this.transparentColor = new TransparentColor(color, opacity);
            this.thickness = thickness;
            this.thicknessMul = thicknessMul;
            this.yPosition = yPosition;
            this.yPositionMul = yPositionMul;
            this.lineCapStyle = lineCapStyle;
        }

        /// <summary>Gets the color of the underline.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </returns>
        public virtual Color GetColor() {
            return transparentColor.GetColor();
        }

        /// <summary>Gets the opacity of the underline color.</summary>
        /// <returns>a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent</returns>
        public virtual float GetOpacity() {
            return transparentColor.GetOpacity();
        }

        /// <summary>Gets the total thickness of the underline (fixed + variable part).</summary>
        /// <param name="fontSize">the font size for which to calculate the variable thickness</param>
        /// <returns>the total thickness, as a <c>float</c>, in points</returns>
        public virtual float GetThickness(float fontSize) {
            return thickness + thicknessMul * fontSize;
        }

        /// <summary>Gets the vertical position of the underline (fixed + variable part).</summary>
        /// <param name="fontSize">the font size for which to calculate the variable position</param>
        /// <returns>the y-position, as a <c>float</c>, in points</returns>
        public virtual float GetYPosition(float fontSize) {
            return yPosition + yPositionMul * fontSize;
        }

        /// <summary>Gets the multiplier for the vertical positioning of the text underline.</summary>
        /// <returns>the Y-position multiplier, as a <c>float</c></returns>
        public virtual float GetYPositionMul() {
            return yPositionMul;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineCapStyle"/>
        /// of the text underline.
        /// </summary>
        /// <returns>
        /// the line cap style, as an <c>int</c> referring to
        /// the values of
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.LineCapStyle"/>
        /// </returns>
        public virtual int GetLineCapStyle() {
            return lineCapStyle;
        }
    }
}
