using iText.Forms.Fields.Merging;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class RegisterDefaultDiContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestDefaultRegistrationFormsModule() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            OnDuplicateFormFieldNameStrategy strategy = pdfDocument.GetDiContainer().GetInstance<OnDuplicateFormFieldNameStrategy
                >();
            NUnit.Framework.Assert.AreEqual(typeof(MergeFieldsStrategy), strategy.GetType());
        }
    }
}
