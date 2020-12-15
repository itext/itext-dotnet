using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfObjectWrapperUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DirectObjectsCouldntBeWrappedTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary pdfDictionary = new PdfDictionary();
                NUnit.Framework.Assert.IsTrue(null == pdfDictionary.GetIndirectReference());
                // PdfCatalog is one of PdfObjectWrapper implementations. The issue could be reproduced on any of them, not only on this one
                PdfCatalog pdfCatalog = new PdfCatalog(pdfDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.OBJECT_MUST_BE_INDIRECT_TO_WORK_WITH_THIS_WRAPPER))
;
        }
    }
}
