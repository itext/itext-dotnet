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
