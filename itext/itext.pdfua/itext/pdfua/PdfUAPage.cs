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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Pdfua.Checkers;

namespace iText.Pdfua {
//\cond DO_NOT_DOCUMENT
    internal class PdfUAPage : PdfPage {
        private readonly PdfUAChecker checker;

        /// <summary>
        /// Creates new
        /// <see cref="PdfUAPage"/>
        /// instance.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// object on which the
        /// <see cref="PdfUAPage"/>
        /// will be based
        /// </param>
        /// <param name="checker">
        /// 
        /// <see cref="iText.Pdfua.Checkers.PdfUA1Checker"/>
        /// to check the requirements of the PDF/UA-1 standard
        /// </param>
        [System.ObsoleteAttribute(@"in favour of PdfUAPage(iText.Kernel.Pdf.PdfDictionary, iText.Pdfua.Checkers.PdfUAChecker)"
            )]
        protected internal PdfUAPage(PdfDictionary pdfObject, PdfUA1Checker checker)
            : base(pdfObject) {
            this.checker = checker;
        }

        /// <summary>
        /// Creates new
        /// <see cref="PdfUAPage"/>
        /// instance.
        /// </summary>
        /// <param name="pdfDocument">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object which will contain the
        /// <see cref="PdfUAPage"/>
        /// </param>
        /// <param name="pageSize">
        /// 
        /// <see cref="iText.Kernel.Geom.PageSize"/>
        /// the size of the
        /// <see cref="PdfUAPage"/>
        /// </param>
        /// <param name="checker">
        /// 
        /// <see cref="iText.Pdfua.Checkers.PdfUA1Checker"/>
        /// to check the requirements of the PDF/UA-1 standard
        /// </param>
        [System.ObsoleteAttribute(@"in favour of PdfUAPage(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Geom.PageSize, iText.Pdfua.Checkers.PdfUAChecker)"
            )]
        protected internal PdfUAPage(PdfDocument pdfDocument, PageSize pageSize, PdfUA1Checker checker)
            : base(pdfDocument, pageSize) {
            this.checker = checker;
        }

        /// <summary>
        /// Creates new
        /// <see cref="PdfUAPage"/>
        /// instance.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// object on which the
        /// <see cref="PdfUAPage"/>
        /// will be based
        /// </param>
        /// <param name="checker">
        /// 
        /// <see cref="iText.Pdfua.Checkers.PdfUAChecker"/>
        /// to check the requirements of the PDF/UA standard
        /// </param>
        protected internal PdfUAPage(PdfDictionary pdfObject, PdfUAChecker checker)
            : base(pdfObject) {
            this.checker = checker;
        }

        /// <summary>
        /// Creates new
        /// <see cref="PdfUAPage"/>
        /// instance.
        /// </summary>
        /// <param name="pdfDocument">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object which will contain the
        /// <see cref="PdfUAPage"/>
        /// </param>
        /// <param name="pageSize">
        /// 
        /// <see cref="iText.Kernel.Geom.PageSize"/>
        /// the size of the
        /// <see cref="PdfUAPage"/>
        /// </param>
        /// <param name="checker">
        /// 
        /// <see cref="iText.Pdfua.Checkers.PdfUAChecker"/>
        /// to check the requirements of the PDF/UA standard
        /// </param>
        protected internal PdfUAPage(PdfDocument pdfDocument, PageSize pageSize, PdfUAChecker checker)
            : base(pdfDocument, pageSize) {
            this.checker = checker;
        }

        public override void Flush(bool flushResourcesContentStreams) {
            if (GetDocument().IsClosing()) {
                base.Flush(flushResourcesContentStreams);
            }
            else {
                checker.WarnOnPageFlush();
            }
        }
    }
//\endcond
}
