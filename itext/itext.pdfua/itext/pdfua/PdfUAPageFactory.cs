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

namespace iText.Pdfua {
//\cond DO_NOT_DOCUMENT
    internal class PdfUAPageFactory : IPdfPageFactory {
        public PdfUAPageFactory() {
        }

        //empty constructor
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// object on which the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// will be based
        /// </param>
        /// <returns>The pdf page.</returns>
        public virtual PdfPage CreatePdfPage(PdfDictionary pdfObject) {
            return new PdfUAPage(pdfObject);
        }

        /// <param name="pdfDocument">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to add page
        /// </param>
        /// <param name="pageSize">
        /// 
        /// <see cref="iText.Kernel.Geom.PageSize"/>
        /// of the created page
        /// </param>
        /// <returns>The Pdf page.</returns>
        public virtual PdfPage CreatePdfPage(PdfDocument pdfDocument, PageSize pageSize) {
            return new PdfUAPage(pdfDocument, pageSize);
        }
    }
//\endcond
}
