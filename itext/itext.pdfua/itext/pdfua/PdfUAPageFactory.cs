using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Pdfua {
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
}
