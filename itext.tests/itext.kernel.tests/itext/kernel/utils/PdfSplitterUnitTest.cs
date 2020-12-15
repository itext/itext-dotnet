using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils {
    public class PdfSplitterUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SplitDocumentThatIsBeingWrittenTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfSplitter pdfSplitter = new PdfSplitter(pdfDocument);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_SPLIT_DOCUMENT_THAT_IS_BEING_WRITTEN))
;
        }
    }
}
