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
    /// <summary>Representation of a CalGray color space.</summary>
    public class CalGray : Color {
        /// <summary>
        /// Creates a new CalGray color space using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfCieBasedCs"/>
        /// color space.
        /// </summary>
        /// <param name="cs">Color space</param>
        public CalGray(PdfCieBasedCs.CalGray cs)
            : this(cs, 0f) {
        }

        /// <summary>
        /// Creates a new CalGray color space using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfCieBasedCs"/>
        /// color space and color value.
        /// </summary>
        /// <param name="cs">Color space</param>
        /// <param name="value">Gray color value</param>
        public CalGray(PdfCieBasedCs.CalGray cs, float value)
            : base(cs, new float[] { value }) {
        }

        /// <summary>Creates a new CalGray color space using the given white point and color value.</summary>
        /// <param name="whitePoint">Color values for defining the white point</param>
        /// <param name="value">Gray color value</param>
        public CalGray(float[] whitePoint, float value)
            : base(new PdfCieBasedCs.CalGray(whitePoint), new float[] { value }) {
        }

        /// <summary>Creates a new CalGray color space using the given white point, black point, gamma and color value.
        ///     </summary>
        /// <param name="whitePoint">Color values for defining the white point</param>
        /// <param name="blackPoint">Color values for defining the black point</param>
        /// <param name="gamma">Gamma correction</param>
        /// <param name="value">Gray color value</param>
        public CalGray(float[] whitePoint, float[] blackPoint, float gamma, float value)
            : this(new PdfCieBasedCs.CalGray(whitePoint, blackPoint, gamma), value) {
        }
    }
}
