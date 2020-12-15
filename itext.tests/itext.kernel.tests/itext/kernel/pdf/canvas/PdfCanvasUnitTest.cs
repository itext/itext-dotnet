using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    public class PdfCanvasUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnbalancedSaveRestoreStateOperatorsUnexpectedRestoreTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream();
                PdfResources pdfResources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
                NUnit.Framework.Assert.IsTrue(pdfCanvas.gsStack.IsEmpty());
                pdfCanvas.RestoreState();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNBALANCED_SAVE_RESTORE_STATE_OPERATORS))
;
        }

        [NUnit.Framework.Test]
        public virtual void UnbalancedLayerOperatorUnexpectedEndTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream();
                PdfResources pdfResources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
                pdfCanvas.EndLayer();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNBALANCED_LAYER_OPERATORS))
;
        }

        [NUnit.Framework.Test]
        public virtual void UnbalancedBeginAndMarkedOperatorsUnexpectedEndTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfStream pdfStream = new PdfStream();
                PdfResources pdfResources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfStream, pdfResources, pdfDocument);
                pdfCanvas.EndMarkedContent();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNBALANCED_BEGIN_END_MARKED_CONTENT_OPERATORS))
;
        }

        [NUnit.Framework.Test]
        public virtual void FontAndSizeShouldBeSetBeforeShowTextTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
                pdfCanvas.ShowText("text");
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT))
;
        }

        [NUnit.Framework.Test]
        public virtual void FontAndSizeShouldBeSetBeforeShowTextTest02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
                PdfArray pdfArray = new PdfArray();
                pdfCanvas.ShowText(pdfArray);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT))
;
        }
    }
}
