using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfPagesTreeUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotGenerateTreeDocHasNoPagesTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                pdfDoc.Close();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.DOCUMENT_HAS_NO_PAGES))
;
        }
    }
}
