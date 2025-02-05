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
using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>
    /// Class for
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// validation context.
    /// </summary>
    public class PdfDocumentValidationContext : IValidationContext {
        private readonly PdfDocument pdfDocument;

        private readonly ICollection<PdfFont> documentFonts;

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfDocumentValidationContext"/>
        /// based on document and document fonts.
        /// </summary>
        /// <param name="pdfDocument">the pdf document</param>
        /// <param name="documentFonts">the document fonts</param>
        public PdfDocumentValidationContext(PdfDocument pdfDocument, ICollection<PdfFont> documentFonts) {
            this.pdfDocument = pdfDocument;
            this.documentFonts = documentFonts;
        }

        /// <summary>Gets the pdf document.</summary>
        /// <returns>the pdf document</returns>
        public virtual PdfDocument GetPdfDocument() {
            return pdfDocument;
        }

        /// <summary>Gets the document fonts.</summary>
        /// <returns>the document fonts</returns>
        public virtual ICollection<PdfFont> GetDocumentFonts() {
            return documentFonts;
        }

        public virtual ValidationType GetType() {
            return ValidationType.PDF_DOCUMENT;
        }
    }
}
