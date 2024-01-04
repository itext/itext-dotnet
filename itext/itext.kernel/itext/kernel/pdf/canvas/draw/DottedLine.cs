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

namespace iText.Kernel.Pdf.Canvas.Draw {
    /// <summary>
    /// Implementation of
    /// <see cref="ILineDrawer"/>
    /// which draws a dotted horizontal line along
    /// the bottom edge of the specified rectangle.
    /// </summary>
    public class DottedLine : ILineDrawer {
        /// <summary>the gap between the dots.</summary>
        protected internal float gap = 4;

        private float lineWidth = 1;

        private Color color = ColorConstants.BLACK;

        /// <summary>Constructs a dotted horizontal line which will be drawn along the bottom edge of the specified rectangle.
        ///     </summary>
        public DottedLine() {
        }

        /// <summary>Constructs a dotted horizontal line which will be drawn along the bottom edge of the specified rectangle.
        ///     </summary>
        /// <param name="lineWidth">the width of the line</param>
        /// <param name="gap">the gap between the center of the dots of the dotted line.</param>
        public DottedLine(float lineWidth, float gap) {
            this.lineWidth = lineWidth;
            this.gap = gap;
        }

        /// <summary>Constructs a dotted horizontal line which will be drawn along the bottom edge of the specified rectangle.
        ///     </summary>
        /// <param name="lineWidth">the width of the line</param>
        public DottedLine(float lineWidth) {
            this.lineWidth = lineWidth;
        }

        public virtual void Draw(PdfCanvas canvas, Rectangle drawArea) {
            canvas.SaveState().SetLineWidth(lineWidth).SetStrokeColor(color).SetLineDash(0, gap, gap / 2).SetLineCapStyle
                (PdfCanvasConstants.LineCapStyle.ROUND).MoveTo(drawArea.GetX(), drawArea.GetY() + lineWidth / 2).LineTo
                (drawArea.GetX() + drawArea.GetWidth(), drawArea.GetY() + lineWidth / 2).Stroke().RestoreState();
        }

        /// <summary>Getter for the gap between the center of the dots of the dotted line.</summary>
        /// <returns>the gap between the center of the dots</returns>
        public virtual float GetGap() {
            return gap;
        }

        /// <summary>Setter for the gap between the center of the dots of the dotted line.</summary>
        /// <param name="gap">the gap between the center of the dots</param>
        public virtual void SetGap(float gap) {
            this.gap = gap;
        }

        /// <summary>Gets line width in points</summary>
        /// <returns>line thickness</returns>
        public virtual float GetLineWidth() {
            return lineWidth;
        }

        /// <summary>Sets line width in points</summary>
        /// <param name="lineWidth">new line width</param>
        public virtual void SetLineWidth(float lineWidth) {
            this.lineWidth = lineWidth;
        }

        public virtual Color GetColor() {
            return color;
        }

        public virtual void SetColor(Color color) {
            this.color = color;
        }
    }
}
