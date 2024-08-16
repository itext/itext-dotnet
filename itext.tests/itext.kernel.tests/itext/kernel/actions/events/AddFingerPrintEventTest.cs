using iText.Commons.Actions.Data;
using iText.IO.Source;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Actions.Events {
    [NUnit.Framework.Category("UnitTest")]
    public class AddFingerPrintEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullDocumentTest() {
            AddFingerPrintEvent addFingerPrintEvent = new AddFingerPrintEvent(null);
            NUnit.Framework.Assert.DoesNotThrow(() => addFingerPrintEvent.DoAction());
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FINGERPRINT_DISABLED_BUT_NO_REQUIRED_LICENCE)]
        public virtual void DisableFingerPrintAGPLTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetFingerPrint().DisableFingerPrint();
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Close());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void EnabledFingerPrintAGPLTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Close());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DisableFingerPrintNoProcessorForProductTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    ProductData productData = new ProductData("public product name", "product name", "1", 2000, 2024);
                    doc.GetFingerPrint().RegisterProduct(productData);
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Close());
                }
            }
        }
    }
}
