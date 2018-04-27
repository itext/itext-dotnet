using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Node;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class NamedObjectsTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void AddNamedObject() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            float width = 100;
            float height = 100;
            PdfFormXObject pdfForm = new PdfFormXObject(new Rectangle(0, 0, width, height));
            PdfCanvas canvas = new PdfCanvas(pdfForm, doc);
            INode parsedSvg = SvgConverter.Parse(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/NamedObjectsTest/names.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessor processor = new DefaultSvgProcessor();
            ISvgProcessorResult result = processor.Process(parsedSvg);
            ISvgNodeRenderer process = result.GetRootRenderer();
            SvgDrawContext drawContext = new SvgDrawContext();
            ISvgNodeRenderer root = new PdfRootSvgNodeRenderer(process);
            drawContext.PushCanvas(canvas);
            root.Draw(drawContext);
            doc.Close();
            NUnit.Framework.Assert.IsTrue(drawContext.GetNamedObject("name_svg") is PdfFormXObject);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("name_rect") is RectangleSvgNodeRenderer);
        }
    }
}
