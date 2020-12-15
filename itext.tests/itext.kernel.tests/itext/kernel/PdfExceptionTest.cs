using System;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel {
    public class PdfExceptionTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WhenCreatedPdfExceptionWrapsCauseWithUnknownExceptionMessageTest() {
            NUnit.Framework.Assert.That(() =>  {
                throw new PdfException(new Exception("itext"));
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNKNOWN_PDF_EXCEPTION))
;
        }
    }
}
