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

namespace iText.Pdfa {
    /// <summary>The class is helper which used for PDF/A document to properly configure PDF document's info dictionary.
    ///     </summary>
    public class PdfADocumentInfoHelper : DocumentInfoHelper {
        private readonly PdfDocument pdfDocument;

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfADocumentInfoHelper"/>
        /// instance based on the document.
        /// </summary>
        /// <param name="pdfDocument">the pdf document which will use that helper</param>
        public PdfADocumentInfoHelper(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override bool ShouldAddDocumentInfoToTrailer() {
            if ("4".Equals(pdfDocument.GetConformance().GetAConformance().GetPart())) {
                // In case of PieceInfo == null don't add Info to trailer
                return pdfDocument.GetCatalog().GetPdfObject().Get(PdfName.PieceInfo) != null;
            }
            return true;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override void AdjustDocumentInfo(PdfDocumentInfo documentInfo) {
            if ("4".Equals(pdfDocument.GetConformance().GetAConformance().GetPart())) {
                if (pdfDocument.GetCatalog().GetPdfObject().Get(PdfName.PieceInfo) != null) {
                    // Leave only ModDate as required by 6.1.3 File trailer of pdf/a-4 spec
                    documentInfo.RemoveCreationDate();
                }
            }
        }
    }
}
