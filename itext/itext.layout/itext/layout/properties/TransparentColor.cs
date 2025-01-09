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
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;

namespace iText.Layout.Properties {
    /// <summary>Represents a color with the specified opacity.</summary>
    public class TransparentColor {
        private Color color;

        private float opacity;

        /// <summary>
        /// Creates a new
        /// <see cref="TransparentColor"/>
        /// instance of certain fully opaque color.
        /// </summary>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of the created
        /// <see cref="TransparentColor"/>
        /// object
        /// </param>
        public TransparentColor(Color color) {
            this.color = color;
            this.opacity = 1f;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TransparentColor"/>.
        /// </summary>
        /// <param name="color">
        /// the
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of the created
        /// <see cref="TransparentColor"/>
        /// object
        /// </param>
        /// <param name="opacity">
        /// a float defining the opacity of the color; a float between 0 and 1,
        /// where 1 stands for fully opaque color and 0 - for fully transparent
        /// </param>
        public TransparentColor(Color color, float opacity) {
            this.color = color;
            this.opacity = opacity;
        }

        /// <summary>Gets the color.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </returns>
        public virtual Color GetColor() {
            return color;
        }

        /// <summary>Gets the opacity of color.</summary>
        /// <returns>a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent</returns>
        public virtual float GetOpacity() {
            return opacity;
        }

        /// <summary>Sets the opacity value for <b>non-stroking</b> operations in the transparent imaging model.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to be written to
        /// </param>
        public virtual void ApplyFillTransparency(PdfCanvas canvas) {
            ApplyTransparency(canvas, false);
        }

        /// <summary>Sets the opacity value for <b>stroking</b> operations in the transparent imaging model.</summary>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// to be written to
        /// </param>
        public virtual void ApplyStrokeTransparency(PdfCanvas canvas) {
            ApplyTransparency(canvas, true);
        }

        private void ApplyTransparency(PdfCanvas canvas, bool isStroke) {
            if (IsTransparent()) {
                PdfExtGState extGState = new PdfExtGState();
                if (isStroke) {
                    extGState.SetStrokeOpacity(opacity);
                }
                else {
                    extGState.SetFillOpacity(opacity);
                }
                canvas.SetExtGState(extGState);
            }
        }

        private bool IsTransparent() {
            return opacity < 1f;
        }
    }
}
