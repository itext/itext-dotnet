using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Pdfua.Checkers.Utils {
    [NUnit.Framework.Category("UnitTest")]
    [Obsolete]
    public class AnnotationCheckUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestIsAnnotationVisible() {
            NUnit.Framework.Assert.IsTrue(AnnotationCheckUtil.IsAnnotationVisible(new PdfDictionary()));
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationHandler() {
            AnnotationCheckUtil.AnnotationHandler handler = new AnnotationCheckUtil.AnnotationHandler(new PdfUAValidationContext
                (null));
            NUnit.Framework.Assert.IsNotNull(handler);
            NUnit.Framework.Assert.IsFalse(handler.Accept(null));
            NUnit.Framework.Assert.IsTrue(handler.Accept(new PdfMcrNumber(new PdfNumber(2), null)));
            NUnit.Framework.Assert.DoesNotThrow(() => handler.ProcessElement(null));
        }
    }
}
