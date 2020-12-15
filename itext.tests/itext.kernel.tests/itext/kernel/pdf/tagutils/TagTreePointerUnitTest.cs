using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Tagutils {
    public class TagTreePointerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void RootTagCannotBeRemovedTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                tagTreePointer.RemoveTag();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_REMOVE_DOCUMENT_ROOT_TAG))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotMoveToKidWithNonExistingRoleTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                tagTreePointer.MoveToKid(1, "role");
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.NO_KID_WITH_SUCH_ROLE))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotMoveToKidMcrTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                tagTreePointer.MoveToKid(1, "MCR");
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE))
;
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ENCOUNTERED_INVALID_MCR)]
        public virtual void CannotMoveToKidMcrTest02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                PdfStructElem parent = new PdfStructElem(pdfDoc, PdfName.MCR);
                parent.Put(PdfName.P, new PdfStructElem(pdfDoc, PdfName.MCR).GetPdfObject());
                PdfStructElem kid1 = new PdfStructElem(pdfDoc, PdfName.MCR);
                PdfMcrNumber kid2 = new PdfMcrNumber(new PdfNumber(1), parent);
                parent.AddKid(kid1);
                parent.AddKid(kid2);
                tagTreePointer.SetCurrentStructElem(parent);
                tagTreePointer.MoveToKid(1);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_MOVE_TO_MARKED_CONTENT_REFERENCE))
;
        }

        [NUnit.Framework.Test]
        public virtual void NoParentObjectInStructElemTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                PdfStructElem pdfStructElem = new PdfStructElem(pdfDoc, PdfName.MCR);
                tagTreePointer.SetCurrentStructElem(pdfStructElem);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.STRUCTURE_ELEMENT_SHALL_CONTAIN_PARENT_OBJECT))
;
        }

        [NUnit.Framework.Test]
        public virtual void PageMustBeInitializedBeforeNextMcidCreationTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                PdfStructElem pdfStructElem = new PdfStructElem(pdfDoc, PdfName.MCR);
                tagTreePointer.CreateNextMcidForStructElem(pdfStructElem, 1);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.PAGE_IS_NOT_SET_FOR_THE_PDF_TAG_STRUCTURE))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotMoveRootToParentTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                tagTreePointer.MoveToParent();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_MOVE_TO_PARENT_CURRENT_ELEMENT_IS_ROOT))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotRelocateRootTagTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                tagTreePointer.Relocate(tagTreePointer);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_RELOCATE_ROOT_TAG))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotFlushAlreadyFlushedPageTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = CreateTestDocument();
                TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
                PdfPage pdfPage = pdfDoc.AddNewPage(1);
                pdfPage.Flush();
                tagTreePointer.SetPageForTagging(pdfPage);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.PAGE_ALREADY_FLUSHED))
;
        }

        private static PdfDocument CreateTestDocument() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.SetTagged();
            return pdfDoc;
        }
    }
}
