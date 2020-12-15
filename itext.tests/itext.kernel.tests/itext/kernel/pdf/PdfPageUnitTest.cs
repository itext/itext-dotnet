using System;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfPageUnitTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfPageUnitTest/";

        [NUnit.Framework.Test]
        public virtual void CannotGetMcidIfDocIsNotTagged() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.GetNextMcid();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.MUST_BE_A_TAGGED_DOCUMENT))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotSetPageLabelIfFirstPageLessThanOneTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.SetPageLabel(PageLabelNumberingStyle.DECIMAL_ARABIC_NUMERALS, "test_prefix", 0);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.IN_A_PAGE_LABEL_THE_PAGE_NUMBERS_MUST_BE_GREATER_OR_EQUAL_TO_1))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotFlushTagsIfNoTagStructureIsPresentTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.TryFlushPageTags();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.TAG_STRUCTURE_FLUSHING_FAILED_IT_MIGHT_BE_CORRUPTED))
;
        }

        [NUnit.Framework.Test]
        public virtual void MediaBoxAttributeIsNotPresentTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "mediaBoxAttributeIsNotPresentTest.pdf"
                    ));
                PdfObject mediaBox = pdfDoc.GetPage(1).GetPdfObject().Get(PdfName.MediaBox);
                NUnit.Framework.Assert.IsNull(mediaBox);
                PdfPage page = pdfDoc.GetPage(1);
                page.GetMediaBox();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_RETRIEVE_MEDIA_BOX_ATTRIBUTE))
;
        }
    }
}
