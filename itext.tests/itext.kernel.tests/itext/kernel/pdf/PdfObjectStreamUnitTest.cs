using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfObjectStreamUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotAddMoreObjectsThanMaxStreamSizeTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfObjectStream pdfObjectStream = new PdfObjectStream(pdfDocument);
                PdfNumber number = new PdfNumber(1);
                number.MakeIndirect(pdfDocument);
                for (int i = 0; i <= PdfObjectStream.MAX_OBJ_STREAM_SIZE; i++) {
                    pdfObjectStream.AddObject(number);
                }
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.PDF_OBJECT_STREAM_REACH_MAX_SIZE))
;
        }

        [NUnit.Framework.Test]
        public virtual void ObjectCanBeAddedToObjectStreamWithSizeLessThenMaxStreamSizeTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfObjectStream pdfObjectStream = new PdfObjectStream(pdfDocument);
            PdfNumber number = new PdfNumber(1);
            number.MakeIndirect(pdfDocument);
            for (int i = 0; i <= PdfObjectStream.MAX_OBJ_STREAM_SIZE - 1; i++) {
                pdfObjectStream.AddObject(number);
            }
            NUnit.Framework.Assert.IsTrue(true, "We don't expect to reach this line, since no exception should have been thrown"
                );
        }
    }
}
