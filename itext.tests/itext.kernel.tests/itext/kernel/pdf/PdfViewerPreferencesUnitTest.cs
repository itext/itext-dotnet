using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfViewerPreferencesUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void PrintScalingIsNullTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfViewerPreferences preferences = new PdfViewerPreferences();
                PdfName pdfName = PdfName.PrintScaling;
                PdfArray pdfArray = new PdfArray(pdfName);
                preferences.SetEnforce(pdfArray);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.PRINT_SCALING_ENFORCE_ENTRY_INVALID))
;
        }
    }
}
