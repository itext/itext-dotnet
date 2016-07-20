using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class LineSeparatorTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/LineSeparatorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LineSeparatorTest/";

        [NUnit.Framework.TestFixtureSetUp]
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
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
