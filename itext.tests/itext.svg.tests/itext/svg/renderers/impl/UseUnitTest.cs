using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class UseUnitTest {
        [NUnit.Framework.Test]
        public virtual void IsRendererDrawnTest() {
            DummySvgNodeRenderer renderer = new DummySvgNodeRenderer();
            SvgDrawContext context = new SvgDrawContext();
            context.AddNamedObject("dummy", renderer);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = pdfDocument.AddNewPage();
            context.PushCanvas(new PdfCanvas(page));
            ISvgNodeRenderer use = new UseSvgNodeRenderer();
            use.SetAttribute(SvgConstants.Attributes.HREF, "dummy");
            use.Draw(context);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsTrue(renderer.IsDrawn());
        }

        [NUnit.Framework.Test]
        public virtual void ReferenceNotFoundTest() {
            DummySvgNodeRenderer renderer = new DummySvgNodeRenderer();
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = pdfDocument.AddNewPage();
            context.PushCanvas(new PdfCanvas(page));
            ISvgNodeRenderer use = new UseSvgNodeRenderer();
            use.SetAttribute(SvgConstants.Attributes.HREF, "dummy");
            use.Draw(context);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsFalse(renderer.IsDrawn());
        }
    }
}
