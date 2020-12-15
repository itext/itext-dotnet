using System.IO;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    public class MetaDoUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InputStreamShallBeStartFromSpecificValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage(1);
                PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
                // InputStream value shall be start with 0x9AC6CDD7
                Stream inputStream = new MemoryStream(new byte[] {  });
                MetaDo metaDo = new MetaDo(inputStream, pdfCanvas);
                metaDo.ReadAll();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.NOT_A_PLACEABLE_WINDOWS_METAFILE))
;
        }
    }
}
