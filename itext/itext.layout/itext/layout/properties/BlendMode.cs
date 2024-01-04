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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Extgstate;

namespace iText.Layout.Properties {
    /// <summary>Defines all possible blend modes and their mapping to pdf names.</summary>
    public sealed class BlendMode {
        public static readonly iText.Layout.Properties.BlendMode NORMAL = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_NORMAL);

        public static readonly iText.Layout.Properties.BlendMode MULTIPLY = new iText.Layout.Properties.BlendMode(
            PdfExtGState.BM_MULTIPLY);

        public static readonly iText.Layout.Properties.BlendMode SCREEN = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_SCREEN);

        public static readonly iText.Layout.Properties.BlendMode OVERLAY = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_OVERLAY);

        public static readonly iText.Layout.Properties.BlendMode DARKEN = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_DARKEN);

        public static readonly iText.Layout.Properties.BlendMode LIGHTEN = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_LIGHTEN);

        public static readonly iText.Layout.Properties.BlendMode COLOR_DODGE = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_COLOR_DODGE);

        public static readonly iText.Layout.Properties.BlendMode COLOR_BURN = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_COLOR_BURN);

        public static readonly iText.Layout.Properties.BlendMode HARD_LIGHT = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_HARD_LIGHT);

        public static readonly iText.Layout.Properties.BlendMode SOFT_LIGHT = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_SOFT_LIGHT);

        public static readonly iText.Layout.Properties.BlendMode DIFFERENCE = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_DIFFERENCE);

        public static readonly iText.Layout.Properties.BlendMode EXCLUSION = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_EXCLUSION);

        public static readonly iText.Layout.Properties.BlendMode HUE = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_HUE);

        public static readonly iText.Layout.Properties.BlendMode SATURATION = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_SATURATION);

        public static readonly iText.Layout.Properties.BlendMode COLOR = new iText.Layout.Properties.BlendMode(PdfExtGState
            .BM_COLOR);

        public static readonly iText.Layout.Properties.BlendMode LUMINOSITY = new iText.Layout.Properties.BlendMode
            (PdfExtGState.BM_LUMINOSITY);

        // Standard separable blend modes
        // Standard nonseparable blend modes
        private readonly PdfName pdfRepresentation;

        internal BlendMode(PdfName pdfRepresentation) {
            this.pdfRepresentation = pdfRepresentation;
        }

        /// <summary>Get the pdf representation of the current blend mode.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// representation of the current blend mode.
        /// </returns>
        public PdfName GetPdfRepresentation() {
            return this.pdfRepresentation;
        }
    }
}
