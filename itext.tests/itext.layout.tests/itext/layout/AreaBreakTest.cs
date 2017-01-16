using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class AreaBreakTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AreaBreakTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AreaBreakTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PageBreakTest1() {
            String outFileName = destinationFolder + "pageBreak1.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak1.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak());
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PageBreakTest2() {
            String outFileName = destinationFolder + "pageBreak2.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak2.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Hello World!")).Add(new AreaBreak(new PageSize(200, 200)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PageBreakTest03() {
            String outFileName = destinationFolder + "pageBreak3.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak3.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.SetRenderer(new ColumnDocumentRenderer(document, new Rectangle[] { new Rectangle(30, 30, 200, 600
                ), new Rectangle(300, 30, 200, 600) }));
            document.Add(new Paragraph("Hello World!")).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph(
                "New page hello world"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LastPageAreaBreakTest() {
            String inputFileName = sourceFolder + "input.pdf";
            String cmpFileName = sourceFolder + "cmp_lastPageAreaBreakTest.pdf";
            String outFileName = destinationFolder + "lastPageAreaBreakTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName), new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("Hello there on the last page!").SetFontSize
                (30).SetWidth(200).SetMarginTop(250));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
