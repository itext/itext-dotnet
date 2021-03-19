using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class BlockRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PAGE_WAS_FLUSHED_ACTION_WILL_NOT_BE_PERFORMED)]
        public virtual void ClippedAreaFlushedPageTest() {
            BlockRenderer blockRenderer = new DivRenderer(new Div());
            blockRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            blockRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 100));
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDocument.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            DrawContext context = new DrawContext(pdfDocument, pdfCanvas);
            pdfDocument.GetPage(1).Flush();
            blockRenderer.Draw(context);
            // This test checks that there is log message and there is no NPE so assertions are not required
            NUnit.Framework.Assert.IsTrue(true);
        }
    }
}
