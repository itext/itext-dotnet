using iText.Forms.Exceptions;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Fields {
    public class PdfFormFieldUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotGetRectangleIfKidsIsNullTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfDictionary pdfDictionary = new PdfDictionary();
                PdfFormField pdfFormField = new PdfFormField(pdfDocument);
                pdfFormField.GetRect(pdfDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(FormsExceptionMessageConstant.WRONG_FORM_FIELD_ADD_ANNOTATION_TO_THE_FIELD))
;
        }
    }
}
