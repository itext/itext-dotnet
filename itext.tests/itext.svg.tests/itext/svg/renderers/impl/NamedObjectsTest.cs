using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Node;
using iText.Svg.Converter;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    public class NamedObjectsTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void AddNamedObject() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            PdfPage pdfPage = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc, 1);
            INode parsedSvg = SvgConverter.Parse(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/NamedObjectsTest/names.svg", FileMode.Open
                , FileAccess.Read));
            ISvgNodeRenderer process = SvgConverter.Process(parsedSvg);
            SvgDrawContext drawContext = new SvgDrawContext();
            drawContext.PushCanvas(canvas);
            process.Draw(drawContext);
            doc.Close();
            NUnit.Framework.Assert.IsTrue(drawContext.GetNamedObject("name_svg") is PdfFormXObject);
            NUnit.Framework.Assert.IsTrue(drawContext.GetNamedObject("name_rect") is RectangleSvgNodeRenderer);
        }
    }
}
