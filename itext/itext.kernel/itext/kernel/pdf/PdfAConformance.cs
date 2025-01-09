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
using System;

namespace iText.Kernel.Pdf {
    /// <summary>PDF/A is a special variant of PDF designed specifically for long-term document preservation (the “A” stands for archive).
    ///     </summary>
    /// <remarks>
    /// PDF/A is a special variant of PDF designed specifically for long-term document preservation (the “A” stands for archive).
    /// <para />
    /// The class contains an enumeration of all the PDF/A conformance currently supported by iText.
    /// </remarks>
    public sealed class PdfAConformance {
        /// <summary>PDF/A-1A</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_1A = new iText.Kernel.Pdf.PdfAConformance("1"
            , "A");

        /// <summary>PDF/A-1B</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_1B = new iText.Kernel.Pdf.PdfAConformance("1"
            , "B");

        /// <summary>PDF/A-2A</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_2A = new iText.Kernel.Pdf.PdfAConformance("2"
            , "A");

        /// <summary>PDF/A-2B</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_2B = new iText.Kernel.Pdf.PdfAConformance("2"
            , "B");

        /// <summary>PDF/A-2U</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_2U = new iText.Kernel.Pdf.PdfAConformance("2"
            , "U");

        /// <summary>PDF/A-3A</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_3A = new iText.Kernel.Pdf.PdfAConformance("3"
            , "A");

        /// <summary>PDF/A-3B</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_3B = new iText.Kernel.Pdf.PdfAConformance("3"
            , "B");

        /// <summary>PDF/A-3U</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_3U = new iText.Kernel.Pdf.PdfAConformance("3"
            , "U");

        /// <summary>PDF/A-4</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_4 = new iText.Kernel.Pdf.PdfAConformance("4"
            , null);

        /// <summary>PDF/A-4E</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_4E = new iText.Kernel.Pdf.PdfAConformance("4"
            , "E");

        /// <summary>PDF/A-4F</summary>
        public static readonly iText.Kernel.Pdf.PdfAConformance PDF_A_4F = new iText.Kernel.Pdf.PdfAConformance("4"
            , "F");

        private readonly String part;

        private readonly String level;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="PdfAConformance"/>
        /// instance.
        /// </summary>
        /// <param name="part">the part of the PDF/A conformance</param>
        /// <param name="level">the level of the PDF/A conformance</param>
        internal PdfAConformance(String part, String level) {
            this.part = part;
            this.level = level;
        }
//\endcond

        /// <summary>Get the part of the PDF/A conformance.</summary>
        /// <returns>the part of the PDF/A conformance</returns>
        public String GetPart() {
            return this.part;
        }

        /// <summary>Get the level of the PDF/A conformance.</summary>
        /// <returns>the level of the PDF/A conformance</returns>
        public String GetLevel() {
            return this.level;
        }
    }
}
