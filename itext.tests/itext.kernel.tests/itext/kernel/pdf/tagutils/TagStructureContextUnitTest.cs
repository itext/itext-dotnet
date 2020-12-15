using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Tagutils {
    public class TagStructureContextUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoTagStructureInNonTaggedDocumentTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                TagStructureContext tagStructureContext = new TagStructureContext(pdfDocument);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.MUST_BE_A_TAGGED_DOCUMENT))
;
        }
    }
}
