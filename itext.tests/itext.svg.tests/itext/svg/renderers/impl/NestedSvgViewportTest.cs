using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Svg.Converter;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class NestedSvgViewportTest {
        public virtual void NestedSvgViewPortTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            ISvgNodeRenderer rootRenderer = SvgConverter.Process(SvgConverter.Parse("<svg version=\"1.1\" baseProfile=\"full\" width=\"250\" height=\"250\" xmlns=\"http://www.w3.org/2000/svg\"><svg x=\"10\" y=\"10\" width=\"100\" height=\"100\"><line x1=\"900\" y1=\"300\" x2=\"1100\" y2=\"100\"stroke-width=\"25\"  /></svg></svg>"
                ));
            PdfFormXObject pdfForm = new PdfFormXObject(new PdfStream());
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            SvgDrawContext context = new SvgDrawContext();
            context.PushCanvas(canvas);
            context.AddViewPort(new Rectangle(0, 0, 1000, 1000));
            SvgSvgNodeRenderer nestedSvgRenderer = (SvgSvgNodeRenderer)rootRenderer.GetChildren()[0];
            nestedSvgRenderer.DoDraw(context);
            PdfResources resources = canvas.GetResources();
            PdfFormXObject xObject = resources.GetForm(new PdfName("Fm1"));
            PdfArray bBox = xObject.GetBBox();
            float[] actual = bBox.ToFloatArray();
            float[] expected = new float[] { 7.5f, 7.5f, 82.5f, 82.5f };
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
