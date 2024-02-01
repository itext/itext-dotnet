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
using System;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Colors {
    public class Separation : Color {
        public Separation(PdfSpecialCs.Separation cs)
            : this(cs, 1f) {
        }

        public Separation(PdfSpecialCs.Separation cs, float value)
            : base(cs, new float[] { value }) {
        }

        /// <summary>Creates a color in a new separation color space.</summary>
        /// <param name="name">the name for the separation color</param>
        /// <param name="alternateCs">the alternative color space</param>
        /// <param name="tintTransform">the function to transform color to the alternate colorspace</param>
        /// <param name="value">the color value</param>
        public Separation(String name, PdfColorSpace alternateCs, IPdfFunction tintTransform, float value)
            : this(new PdfSpecialCs.Separation(name, alternateCs, tintTransform), value) {
        }
    }
}
