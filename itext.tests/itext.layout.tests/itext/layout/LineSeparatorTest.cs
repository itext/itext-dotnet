using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class LineSeparatorTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LineSeparatorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LineSeparatorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LineSeparatorWidthPercentageTest01() {
            String outFileName = destinationFolder + "lineSeparatorWidthPercentageTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_lineSeparatorWidthPercentageTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            ILineDrawer line1 = new SolidLine();
            line1.SetColor(Color.RED);
            ILineDrawer line2 = new SolidLine();
            document.Add(new LineSeparator(line1).SetWidth(50).SetMarginBottom(10));
            document.Add(new LineSeparator(line2).SetWidthPercent(50));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LineSeparatorBackgroundTest01() {
            String outFileName = destinationFolder + "lineSeparatorBackgroundTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_lineSeparatorBackgroundTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            Style style = new Style();
            style.SetBackgroundColor(Color.YELLOW);
            style.SetMargin(10);
            document.Add(new LineSeparator(new SolidLine()).AddStyle(style));
            document.Add(new LineSeparator(new DashedLine()).SetBackgroundColor(Color.RED));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RotatedLineSeparatorTest01() {
            String outFileName = destinationFolder + "rotatedLineSeparatorTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLineSeparatorTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            document.Add(new LineSeparator(new DashedLine()).SetBackgroundColor(Color.RED).SetRotationAngle(Math.PI / 
                2));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RotatedLineSeparatorTest02() {
            String outFileName = destinationFolder + "rotatedLineSeparatorTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_rotatedLineSeparatorTest02.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            document.Add(new Paragraph("Hello"));
            document.Add(new LineSeparator(new DashedLine()).SetWidth(100).SetHorizontalAlignment(HorizontalAlignment.
                CENTER).SetBackgroundColor(Color.GREEN).SetRotationAngle(Math.PI / 4));
            document.Add(new Paragraph("World"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
