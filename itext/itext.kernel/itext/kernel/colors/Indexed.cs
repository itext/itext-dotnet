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
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    /// <summary>Representation of an indexed color space.</summary>
    public class Indexed : Color {
        /// <summary>
        /// Creates an indexed color using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>.
        /// </summary>
        /// <param name="colorSpace">Object containing the most common properties of color spaces</param>
        public Indexed(PdfColorSpace colorSpace)
            : this(colorSpace, 0) {
        }

        /// <summary>
        /// Creates an indexed color using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// and color values.
        /// </summary>
        /// <param name="colorSpace">Object containing the most common properties of color spaces</param>
        /// <param name="colorValue">Color values</param>
        public Indexed(PdfColorSpace colorSpace, int colorValue)
            : base(colorSpace, new float[] { colorValue }) {
        }
    }
}
