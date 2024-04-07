using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Pdfua {
    internal class PdfUAPage : PdfPage {
        protected internal PdfUAPage(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        protected internal PdfUAPage(PdfDocument pdfDocument, PageSize pageSize)
            : base(pdfDocument, pageSize) {
        }

        public override void Flush(bool flushResourcesContentStreams) {
            PdfDocument document = GetDocument();
            if (((PdfUADocument)document).IsClosing()) {
                base.Flush(flushResourcesContentStreams);
                return;
            }
            ((PdfUADocument)document).WarnOnPageFlush();
        }
    }
}
