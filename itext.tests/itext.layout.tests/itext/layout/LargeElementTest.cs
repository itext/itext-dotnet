using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class LargeElementTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LargeElementTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LargeElementTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableTest01() {
            String testName = "largeTableTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(5, true);
            doc.Add(table);
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 5; j++) {
                    table.AddCell(new Cell().Add(new Paragraph(String.Format("Cell {0}, {1}", i + 1, j + 1))));
                }
                if (i % 10 == 0) {
                    table.Flush();
                    // This is a deliberate additional flush.
                    table.Flush();
                }
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableTest02() {
            String testName = "largeTableTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(5, true).SetMargins(20, 20, 20, 20);
            doc.Add(table);
            for (int i = 0; i < 100; i++) {
                table.AddCell(new Cell().Add(new Paragraph(String.Format("Cell {0}", i + 1))));
                if (i % 7 == 0) {
                    table.Flush();
                }
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01A() {
            String testName = "largeTableWithHeaderFooterTest01A.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(5, true);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page"));
            table.AddFooterCell(cell);
            table.SetSkipFirstHeader(true);
            table.SetSkipLastFooter(true);
            for (int i = 0; i < 350; i++) {
                table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01B() {
            String testName = "largeTableWithHeaderFooterTest01B.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(5, true);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page"));
            table.AddFooterCell(cell);
            table.SetSkipFirstHeader(true);
            table.SetSkipLastFooter(true);
            for (int i = 0; i < 350; i++) {
                table.Flush();
                table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
            }
            // That's the trick. complete() is called when table has non-empty content, so the last row is better laid out.
            // Compare with #largeTableWithHeaderFooterTest01A. When we flush last row before calling complete(), we don't yet know
            // if there will be any more rows. Flushing last row implicitly by calling complete solves this problem.
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01C() {
            String testName = "largeTableWithHeaderFooterTest01C.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A6.Rotate());
            Table table = new Table(5, true);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (Color.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (Color.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(Color.BLUE, 0.5f)).SetBorderRight(new SolidBorder(Color
                    .BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(Color.BLUE, 2 * i + 1 > 50 ? 50 : 2 * i + 
                    1)).SetBorderTop(new SolidBorder(Color.GREEN, (50 - 2 * i + 1 >= 0) ? 50 - 2 * i + 1 : 0)).Add(new Paragraph
                    ((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01D() {
            String testName = "largeTableWithHeaderFooterTest01D.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A6.Rotate());
            Table table = new Table(5, true);
            table.SetSkipLastFooter(true);
            table.SetSkipFirstHeader(true);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (Color.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (Color.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(Color.BLUE, 0.5f)).SetBorderRight(new SolidBorder(Color
                    .BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(Color.BLUE, 2 * i + 1 > 50 ? 50 : 2 * i + 
                    1)).SetBorderTop(new SolidBorder(Color.GREEN, (50 - 2 * i + 1 >= 0) ? 50 - 2 * i + 1 : 0)).Add(new Paragraph
                    ((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest02() {
            String testName = "largeTableWithHeaderFooterTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(5, true);
            doc.Add(table);
            for (int i = 0; i < 5; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header1 \n" + i)));
            }
            for (int i = 0; i < 5; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header2 \n" + i)));
            }
            for (int i = 0; i < 500; i++) {
                if (i % 5 == 0) {
                    table.Flush();
                }
                table.AddCell(new Cell().Add(new Paragraph("Test " + i)));
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest03() {
            String testName = "largeTableWithHeaderFooterTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(5, true);
            doc.Add(table);
            for (int i = 0; i < 5; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header \n" + i)));
            }
            for (int i = 0; i < 5; i++) {
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer \n" + i)));
            }
            for (int i = 0; i < 500; i++) {
                if (i % 5 == 0) {
                    table.Flush();
                }
                table.AddCell(new Cell().Add(new Paragraph("Test " + i)));
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest04() {
            String testName = "largeTableWithHeaderFooterTest04.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(5, true);
            doc.Add(table);
            for (int i = 0; i < 5; i++) {
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer \n" + i)));
            }
            for (int i = 0; i < 500; i++) {
                if (i % 5 == 0) {
                    table.Flush();
                }
                table.AddCell(new Cell().Add(new Paragraph("Test " + i)));
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 1)]
        public virtual void LargeEmptyTableTest() {
            String testName = "largeEmptyTableTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(1, true);
            doc.Add(table);
            table.SetBorderTop(new SolidBorder(Color.ORANGE, 100)).SetBorderBottom(new SolidBorder(Color.MAGENTA, 150)
                );
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }
    }
}
