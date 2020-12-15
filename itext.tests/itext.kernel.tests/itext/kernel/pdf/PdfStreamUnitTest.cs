using System.IO;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfStreamUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotCreatePdfStreamWithoutDocumentTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfStream pdfStream = new PdfStream(null, null, 1);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CREATE_PDFSTREAM_BY_INPUT_STREAM_WITHOUT_PDF_DOCUMENT))
;
        }

        [NUnit.Framework.Test]
        public virtual void SetDataToPdfStreamWithInputStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                Stream inputStream = new MemoryStream(new byte[] {  });
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream(pdfDocument, inputStream, 1);
                pdfStream.SetData(new byte[] {  }, true);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_SET_DATA_TO_PDF_STREAM_WHICH_WAS_CREATED_BY_INPUT_STREAM))
;
        }
    }
}
