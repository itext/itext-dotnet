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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Pdfa.Checker;

namespace iText.Pdfa {
    /// <summary>The class implements PDF page factory which is used for creating correct PDF/A documents.</summary>
    public class PdfAPageFactory : IPdfPageFactory {
        private readonly PdfAChecker checker;

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfAPageFactory"/>
        /// instance based on
        /// <see cref="iText.Pdfa.Checker.PdfAChecker"/>.
        /// </summary>
        /// <param name="checker">the PDF/A checker</param>
        public PdfAPageFactory(PdfAChecker checker) {
            this.checker = checker;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual PdfPage CreatePdfPage(PdfDictionary pdfObject) {
            return new PdfAPage(pdfObject, checker);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual PdfPage CreatePdfPage(PdfDocument pdfDocument, PageSize pageSize) {
            return new PdfAPage(pdfDocument, pageSize, checker);
        }
    }
}
