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
