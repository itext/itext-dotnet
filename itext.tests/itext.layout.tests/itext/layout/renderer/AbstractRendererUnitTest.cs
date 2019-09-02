using System;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Test;

namespace iText.Layout.Renderer {
    public abstract class AbstractRendererUnitTest : ExtendedITextTest {
        // This also can be converted to a @Rule to have it all at hand in the future
        protected internal static Document CreateDocument() {
            return new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())));
        }

        protected internal static TextRenderer CreateLayoutedTextRenderer(String text, Document document) {
            TextRenderer renderer = (TextRenderer)new TextRenderer(new Text(text)).SetParent(document.GetRenderer());
            renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(1000, 1000))));
            return renderer;
        }

        protected internal static ImageRenderer CreateLayoutedImageRenderer(float width, float height, Document document
            ) {
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(width, height));
            Image img = new Image(xObject);
            ImageRenderer renderer = (ImageRenderer)new ImageRenderer(img).SetParent(document.GetRenderer());
            renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(1000, 1000))));
            return renderer;
        }

        protected internal static LayoutArea CreateLayoutArea(float width, float height) {
            return new LayoutArea(1, new Rectangle(width, height));
        }
    }
}
