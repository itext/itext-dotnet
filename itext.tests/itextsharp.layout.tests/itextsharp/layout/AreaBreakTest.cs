using System;
using System.IO;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Property;
using iTextSharp.Test;

namespace iTextSharp.Layout {
    public class AreaBreakTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itextsharp/layout/AreaBreakTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itextsharp/layout/AreaBreakTest/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PageBreakTest1() {
            String outFileName = destinationFolder + "pageBreak1.pdf";
            String cmpFileName = sourceFolder + "cmp_pageBreak1.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
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
