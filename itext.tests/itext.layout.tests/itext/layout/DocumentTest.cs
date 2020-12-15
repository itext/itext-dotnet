using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Test;

namespace iText.Layout {
    public class DocumentTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ExecuteActionInClosedDocTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                Document document = new Document(pdfDoc);
                Paragraph paragraph = new Paragraph("test");
                document.Add(paragraph);
                document.Close();
                document.CheckClosingStatus();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(LayoutExceptionMessageConstant.DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION))
;
        }
    }
}
