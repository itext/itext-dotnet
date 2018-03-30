using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Svg.Converter;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class SvgSvgNodeRendererUnitTest {
        [NUnit.Framework.Test]
        public virtual void CalculateOutermostViewportTest() {
            Rectangle expected = new Rectangle(0, 0, 600, 600);
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            SvgSvgNodeRenderer renderer = new SvgSvgNodeRenderer();
            Rectangle actual = renderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateOutermostViewportWithDifferentXYTest() {
            Rectangle expected = new Rectangle(10, 20, 600, 600);
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            SvgSvgNodeRenderer renderer = new SvgSvgNodeRenderer();
            Rectangle actual = renderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateNestedViewportSameAsParentTest() {
            Rectangle expected = new Rectangle(0, 0, 600, 600);
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            context.AddViewPort(expected);
            SvgSvgNodeRenderer parent = new SvgSvgNodeRenderer();
            SvgSvgNodeRenderer renderer = new SvgSvgNodeRenderer();
            renderer.SetParent(parent);
            Rectangle actual = renderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateNestedViewportDifferentFromParentTest() {
            Rectangle expected = new Rectangle(0, 0, 500, 500);
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            context.AddViewPort(expected);
            SvgSvgNodeRenderer parent = new SvgSvgNodeRenderer();
            SvgSvgNodeRenderer renderer = new SvgSvgNodeRenderer();
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("width", "500");
            styles.Put("height", "500");
            renderer.SetAttributesAndStyles(styles);
            renderer.SetParent(parent);
            Rectangle actual = renderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void NoBoundingBoxOnXObjectTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                    (0)));
                document.AddNewPage();
                ISvgNodeRenderer rootRenderer = SvgConverter.Process(SvgConverter.Parse("<svg />"));
                PdfFormXObject pdfForm = new PdfFormXObject(new PdfStream());
                PdfCanvas canvas = new PdfCanvas(pdfForm, document);
                SvgDrawContext context = new SvgDrawContext();
                context.PushCanvas(canvas);
                rootRenderer.Draw(context);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.ROOT_SVG_NO_BBOX));
;
        }

        [NUnit.Framework.Test]
        public virtual void CalculateOutermostTransformation() {
            AffineTransform expected = new AffineTransform(0.75d, 0d, 0d, -0.75d, 0d, 450d);
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(new Rectangle(0, 0, 600, 600));
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            SvgSvgNodeRenderer renderer = new SvgSvgNodeRenderer();
            context.AddViewPort(renderer.CalculateViewPort(context));
            AffineTransform actual = renderer.CalculateTransformation(context);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
