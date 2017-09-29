/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
            doc.Add(table);
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 5; j++) {
                    table.AddCell(new Cell().Add(new Paragraph(MessageFormatUtil.Format("Cell {0}, {1}", i + 1, j + 1))));
                }
                if (i % 10 == 0) {
                    table.Flush();
                    // This is a deliberate additional flush.
                    table.Flush();
                }
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true).SetMargins(20, 20, 20, 20);
            doc.Add(table);
            for (int i = 0; i < 100; i++) {
                table.AddCell(new Cell().Add(new Paragraph(MessageFormatUtil.Format("Cell {0}", i + 1))));
                if (i % 7 == 0) {
                    table.Flush();
                }
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01E() {
            String testName = "largeTableWithHeaderFooterTest01E.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page"));
            table.AddFooterCell(cell);
            table.SetSkipFirstHeader(true);
            table.SetSkipLastFooter(true);
            for (int i = 0; i < 350; i++) {
                if (i % 10 == 0) {
                    doc.Add(table);
                }
                table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
            }
            // That's the trick. complete() is called when table has non-empty content, so the last row is better laid out.
            // Compare with #largeTableWithHeaderFooterTest01A. When we flush last row before calling complete(), we don't yet know
            // if there will be any more rows. Flushing last row implicitly by calling complete solves this problem.
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
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
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LargeEmptyTableTest() {
            String testName = "largeEmptyTableTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1), true);
            doc.Add(table);
            table.SetBorderTop(new SolidBorder(Color.ORANGE, 100)).SetBorderBottom(new SolidBorder(Color.MAGENTA, 150)
                );
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 8)]
        public virtual void LargeEmptyTableTest02() {
            String testName = "largeEmptyTableTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell")));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell")));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(Color.
                ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }
    }
}
