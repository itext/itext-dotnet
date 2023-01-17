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
