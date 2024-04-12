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
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    /// <summary>Representation of a lab color space.</summary>
    public class Lab : Color {
        /// <summary>
        /// Creates a lab color using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfCieBasedCs"/>
        /// color space.
        /// </summary>
        /// <param name="cs">Color space</param>
        public Lab(PdfCieBasedCs.Lab cs)
            : this(cs, new float[cs.GetNumberOfComponents()]) {
        }

        /// <summary>
        /// Creates a lab color using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfCieBasedCs"/>
        /// color space and color values.
        /// </summary>
        /// <param name="cs">Color space</param>
        /// <param name="value">Color values</param>
        public Lab(PdfCieBasedCs.Lab cs, float[] value)
            : base(cs, value) {
        }

        /// <summary>Creates a lab color using the given white point and color values.</summary>
        /// <param name="whitePoint">Color values for defining the white point</param>
        /// <param name="value">Color values</param>
        public Lab(float[] whitePoint, float[] value)
            : base(new PdfCieBasedCs.Lab(whitePoint), value) {
        }

        /// <summary>Creates a lab color using the given white point, black point and color values.</summary>
        /// <param name="whitePoint">Color values for defining the white point</param>
        /// <param name="blackPoint">Color values for defining the black point</param>
        /// <param name="range">Range for color</param>
        /// <param name="value">Color values</param>
        public Lab(float[] whitePoint, float[] blackPoint, float[] range, float[] value)
            : this(new PdfCieBasedCs.Lab(whitePoint, blackPoint, range), value) {
        }
    }
}
