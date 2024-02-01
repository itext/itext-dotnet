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
namespace iText.Kernel.Pdf {
    /// <summary>
    /// Implementation of
    /// <see cref="IConformanceLevel"/>
    /// interface for PDF/UA conformance level.
    /// </summary>
    /// <remarks>
    /// Implementation of
    /// <see cref="IConformanceLevel"/>
    /// interface for PDF/UA conformance level.
    /// <para />
    /// PDF/UA is a conformance level for PDF files that ensures the files are accessible.
    /// It contains an enumeration of all the PDF/UA conformance levels currently supported by iText.
    /// </remarks>
    public class PdfUAConformanceLevel : IConformanceLevel {
        /// <summary>PDF/UA conformance level PDF/UA-1.</summary>
        public static readonly iText.Kernel.Pdf.PdfUAConformanceLevel PDFUA_1 = new iText.Kernel.Pdf.PdfUAConformanceLevel
            (1);

        private readonly int version;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfUAConformanceLevel"/>
        /// instance.
        /// </summary>
        /// <param name="version">the version of the PDF/UA conformance level</param>
        private PdfUAConformanceLevel(int version) {
            this.version = version;
        }

        /// <summary>Get the version of the PDF/UA conformance level.</summary>
        /// <returns>the version of the PDF/UA conformance level</returns>
        public virtual int GetVersion() {
            return version;
        }
    }
}
