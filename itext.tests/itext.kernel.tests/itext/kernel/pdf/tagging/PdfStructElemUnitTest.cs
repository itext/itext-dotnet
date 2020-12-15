using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfStructElemUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoParentObjectTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary parent = new PdfDictionary();
                PdfArray kid = new PdfArray();
                PdfStructElem.AddKidObject(parent, 1, kid);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_SHALL_CONTAIN_PARENT_OBJECT))
;
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationHasNoReferenceToPageTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfName pdfName = new PdfName("test");
                PdfAnnotation annotation = new Pdf3DAnnotation(new Rectangle(100, 100), pdfName);
                PdfStructElem structElem = new PdfStructElem(pdfDoc, pdfName, annotation);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.ANNOTATION_SHALL_HAVE_REFERENCE_TO_PAGE))
;
        }
    }
}
