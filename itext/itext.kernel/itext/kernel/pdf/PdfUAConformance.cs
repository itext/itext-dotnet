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
    /// <summary>PDF/UA is a conformance for PDF files that ensures the files are accessible to all users.</summary>
    /// <remarks>
    /// PDF/UA is a conformance for PDF files that ensures the files are accessible to all users.
    /// It contains an enumeration of all the PDF/UA conformance currently supported by iText.
    /// </remarks>
    public sealed class PdfUAConformance {
        /// <summary>PDF/UA-1 conformance</summary>
        public static readonly iText.Kernel.Pdf.PdfUAConformance PDF_UA_1 = new iText.Kernel.Pdf.PdfUAConformance(
            "1");

        private readonly String part;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="PdfUAConformance"/>
        /// instance.
        /// </summary>
        /// <param name="part">the part of the PDF/UA conformance</param>
        internal PdfUAConformance(String part) {
            this.part = part;
        }
//\endcond

        /// <summary>Get the part of the PDF/UA conformance.</summary>
        /// <returns>the part of the PDF/UA conformance</returns>
        public String GetPart() {
            return this.part;
        }
    }
}
