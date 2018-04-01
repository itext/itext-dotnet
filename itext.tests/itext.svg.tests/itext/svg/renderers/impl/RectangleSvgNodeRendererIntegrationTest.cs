using System;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Svg.Converter;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class RectangleSvgNodeRendererIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RectangleSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RectangleSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicRectangleTest() {
            String filename = "basicRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicRectangleRxRyZeroTest() {
            String filename = "basicRectangleRxRyZeroTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='0' ry ='0' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRectangleRyZeroTest() {
            String filename = "basicCircularRoundedRectangleRyZeroTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='10' ry='0' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRectangleRxZeroTest() {
            String filename = "basicCircularRoundedRectangleRxZeroTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='0' ry='10' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRxRectangleTest() {
            String filename = "basicCircularRoundedRxRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='10' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRyRectangleTest() {
            String filename = "basicCircularRoundedRyRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' ry='15' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicEllipticalRoundedRectangleXTest() {
            String filename = "basicEllipticalRoundedRectangleXTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='20' ry='5' stroke='green' fill ='cyan' /></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicEllipticalRoundedRectangleYTest() {
            String filename = "basicEllipticalRoundedRectangleYTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='5' ry='10' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicEllipticalWidthCappedRoundedRectangleTest() {
            String filename = "basicEllipticalWidthCappedRoundedRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='50' ry='10' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicEllipticalHeightCappedRoundedRectangleTest() {
            String filename = "basicEllipticalHeightCappedRoundedRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='10' ry='45' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicEllipticalNegativeWidthRoundedRectangleTest() {
            String filename = "basicEllipticalNegativeWidthRoundedRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='-10' ry='15' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicEllipticalNegativeHeightRoundedRectangleTest() {
            String filename = "basicEllipticalNegativeHeightRoundedRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String contents = "<svg width='800' height='800'> <rect x='100' y='100' width='80' height='80' rx='10' ry='-15' stroke='green' fill ='cyan'/></svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ComplexRectangleTest() {
            String filename = "complexRectangleTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            String contents = "<?xml version=\"1.0\" standalone=\"no\"?>\n" + "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width='800' height='800'>\n"
                 + "\t<rect x='0' y='0' width='800' height='800' stroke= 'red' fill='white'/>\n" + "\t<line x1='0' y1='0' x2='100' y2='100' stroke-width='5' stroke='black' />\n"
                 + "\t<line x1='100' y1='100' x2='200' y2='100' stroke-width='5' stroke='black' />\n" + "\t<line x1='100' y1='100' x2='100' y2='200' stroke-width='5' stroke='black' />\n"
                 + "    <rect x='100' y='100' width='80' height='80' rx ='25' ry='15' \n" + "\tstroke='green' fill ='cyan'/>\n"
                 + "</svg>";
            SvgConverter.DrawOnDocument(contents, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }
    }
}
