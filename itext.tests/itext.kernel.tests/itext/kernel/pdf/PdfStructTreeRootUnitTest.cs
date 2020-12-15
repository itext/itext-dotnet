using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfStructTreeRootUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotMovePageInPartlyFlushedDocTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage(1);
                pdfPage.Flush();
                PdfStructTreeRoot pdfStructTreeRoot = new PdfStructTreeRoot(pdfDocument);
                pdfStructTreeRoot.Move(pdfPage, -1);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_MOVE_PAGES_IN_PARTLY_FLUSHED_DOCUMENT, 1)))
;
        }
    }
}
