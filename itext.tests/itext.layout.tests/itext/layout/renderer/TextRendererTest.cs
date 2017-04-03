using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Test;

namespace iText.Layout.Renderer {
    public class TextRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NextRendererTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteBufferOutputStream()));
            pdfDoc.AddNewPage();
            Document doc = new Document(pdfDoc);
            RootRenderer documentRenderer = doc.GetRenderer();
            Text text = new Text("hello");
            text.SetNextRenderer(new TextRenderer(text));
            IRenderer textRenderer1 = text.GetRenderer().SetParent(documentRenderer);
            IRenderer textRenderer2 = text.GetRenderer().SetParent(documentRenderer);
            LayoutArea area = new LayoutArea(1, new Rectangle(100, 100, 100, 100));
            LayoutContext layoutContext = new LayoutContext(area);
            doc.Close();
            LayoutResult result1 = textRenderer1.Layout(layoutContext);
            LayoutResult result2 = textRenderer2.Layout(layoutContext);
            NUnit.Framework.Assert.AreEqual(result1.GetOccupiedArea(), result2.GetOccupiedArea());
        }
    }
}
