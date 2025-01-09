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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Draw {
    /// <summary>
    /// The
    /// <see cref="ILineDrawer"/>
    /// defines a drawing operation on a
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
    /// </summary>
    /// <remarks>
    /// The
    /// <see cref="ILineDrawer"/>
    /// defines a drawing operation on a
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
    /// <br />
    /// This interface allows to customize the 'empty' space in a
    /// <c>com.itextpdf.layout.element.TabStop</c>
    /// through a Strategy design
    /// pattern
    /// </remarks>
    public interface ILineDrawer {
        /// <summary>
        /// Performs configurable drawing operations related to specific region
        /// coordinates on a canvas.
        /// </summary>
        /// <param name="canvas">the canvas to draw on</param>
        /// <param name="drawArea">
        /// the rectangle in relation to which to fulfill drawing
        /// instructions
        /// </param>
        void Draw(PdfCanvas canvas, Rectangle drawArea);

        /// <summary>Gets the width of the line</summary>
        /// <returns>width of the line</returns>
        float GetLineWidth();

        /// <summary>Sets line width in points</summary>
        /// <param name="lineWidth">new line width</param>
        void SetLineWidth(float lineWidth);

        /// <summary>Gets the color of the line</summary>
        /// <returns>color of the line</returns>
        Color GetColor();

        /// <summary>Sets line color</summary>
        /// <param name="color">new line color</param>
        void SetColor(Color color);
    }
}
