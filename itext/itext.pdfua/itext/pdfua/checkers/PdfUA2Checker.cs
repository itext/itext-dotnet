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
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Pdfua.Checkers.Utils;

namespace iText.Pdfua.Checkers {
    /// <summary>
    /// The class defines the requirements of the PDF/UA-2 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// The class defines the requirements of the PDF/UA-2 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// <para />
    /// The specification implemented by this class is ISO 14289-2.
    /// </remarks>
    public class PdfUA2Checker : PdfUAChecker {
        private readonly PdfDocument pdfDocument;

        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates
        /// <see cref="PdfUA2Checker"/>
        /// instance with PDF document which will be validated against PDF/UA-2 standard.
        /// </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public PdfUA2Checker(PdfDocument pdfDocument)
            : base() {
            this.pdfDocument = pdfDocument;
            this.context = new PdfUAValidationContext(pdfDocument);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override void Validate(IValidationContext context) {
        }

        // Empty for now.
        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return true;
        }
    }
}
