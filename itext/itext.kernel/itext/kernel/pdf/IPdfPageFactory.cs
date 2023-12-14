/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Interface used to create instances of
    /// <see cref="PdfPage"/>.
    /// </summary>
    public interface IPdfPageFactory {
        /// <summary>
        /// Create
        /// <see cref="PdfPage"/>
        /// on the base of the page
        /// <see cref="PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="PdfDictionary"/>
        /// object on which the
        /// <see cref="PdfPage"/>
        /// will be based
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfPage"/>
        /// </returns>
        PdfPage CreatePdfPage(PdfDictionary pdfObject);

        /// <summary>
        /// Create
        /// <see cref="PdfPage"/>
        /// with given page size and add it to the
        /// <see cref="PdfDocument"/>.
        /// </summary>
        /// <param name="pdfDocument">
        /// 
        /// <see cref="PdfDocument"/>
        /// to add page
        /// </param>
        /// <param name="pageSize">
        /// 
        /// <see cref="iText.Kernel.Geom.PageSize"/>
        /// of the created page
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfPage"/>
        /// </returns>
        PdfPage CreatePdfPage(PdfDocument pdfDocument, PageSize pageSize);
    }
}
