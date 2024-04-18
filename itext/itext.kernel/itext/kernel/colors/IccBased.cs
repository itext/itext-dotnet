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
using System.IO;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    /// <summary>Representation on an ICC Based color space.</summary>
    public class IccBased : Color {
        /// <summary>
        /// Creates an ICC Based color using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfCieBasedCs"/>
        /// color space.
        /// </summary>
        /// <param name="cs">Color space</param>
        public IccBased(PdfCieBasedCs.IccBased cs)
            : this(cs, new float[cs.GetNumberOfComponents()]) {
        }

        /// <summary>
        /// Creates an ICC Based color using the given
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfCieBasedCs"/>
        /// color space and color values.
        /// </summary>
        /// <param name="cs">Color space</param>
        /// <param name="value">Color values</param>
        public IccBased(PdfCieBasedCs.IccBased cs, float[] value)
            : base(cs, value) {
        }

        /// <summary>Creates IccBased color.</summary>
        /// <param name="iccStream">ICC profile stream. User is responsible for closing the stream.</param>
        public IccBased(Stream iccStream)
            : this(new PdfCieBasedCs.IccBased(iccStream), null) {
            colorValue = new float[GetNumberOfComponents()];
            for (int i = 0; i < GetNumberOfComponents(); i++) {
                colorValue[i] = 0f;
            }
        }

        /// <summary>Creates IccBased color.</summary>
        /// <param name="iccStream">ICC profile stream. User is responsible for closing the stream.</param>
        /// <param name="value">color value.</param>
        public IccBased(Stream iccStream, float[] value)
            : this(new PdfCieBasedCs.IccBased(iccStream), value) {
        }

        /// <summary>Creates an ICC Based color using the given ICC profile stream, range and color values.</summary>
        /// <param name="iccStream">ICC profile stream. User is responsible for closing the stream.</param>
        /// <param name="range">Range for color</param>
        /// <param name="value">Color values</param>
        public IccBased(Stream iccStream, float[] range, float[] value)
            : this(new PdfCieBasedCs.IccBased(iccStream, range), value) {
            if (GetNumberOfComponents() * 2 != range.Length) {
                throw new PdfException(KernelExceptionMessageConstant.INVALID_RANGE_ARRAY, this);
            }
        }
    }
}
