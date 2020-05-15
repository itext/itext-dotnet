/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01ASeparated() {
            String testName = "largeTableWithHeaderFooterTest01ASeparated.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20f);
            table.SetVerticalBorderSpacing(20f);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
                (ColorConstants.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1778")]
        public virtual void LargeTableWithHeaderFooterTest01CForcedPlacement() {
            String testName = "largeTableWithHeaderFooterTest01CForcedPlacement.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A6.Rotate());
            // separate
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20f);
            table.SetVerticalBorderSpacing(20f);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            pdfDoc.SetDefaultPageSize(new PageSize(420, 208));
            doc.Add(new AreaBreak());
            // collapse
            table = new Table(UnitValue.CreatePercentArray(5), true);
            doc.Add(table);
            cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
                (ColorConstants.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01DSeparated() {
            String testName = "largeTableWithHeaderFooterTest01DSeparated.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A6.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetSkipLastFooter(true);
            table.SetSkipFirstHeader(true);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")).SetHeight(30).SetBorderBottom(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddHeaderCell(cell);
            cell = new Cell(1, 5).Add(new Paragraph("Continue on next page")).SetHeight(30).SetBorderTop(new SolidBorder
                (ColorConstants.MAGENTA, 20));
            table.AddFooterCell(cell);
            for (int i = 0; i < 50; i++) {
                table.AddCell(new Cell().SetBorderLeft(new SolidBorder(ColorConstants.BLUE, 0.5f)).SetBorderRight(new SolidBorder
                    (ColorConstants.BLUE, 0.5f)).SetHeight(30).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 2 * i 
                    + 1 > 50 ? 50 : 2 * i + 1)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, (50 - 2 * i + 1 >= 0) ? 
                    50 - 2 * i + 1 : 0)).Add(new Paragraph((i + 1).ToString())));
                table.Flush();
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableWithHeaderFooterTest01ESeparated() {
            String testName = "largeTableWithHeaderFooterTest01ESeparated.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5), true);
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20f);
            table.SetVerticalBorderSpacing(20f);
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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableWithLayoutResultNothingTest01() {
            String testName = "largeTableWithLayoutResultNothingTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A1.Rotate());
            float[] colWidths = new float[] { 300, 150, 50, 100 };
            // the second column has colspan value as 2
            int numOfColumns = colWidths.Length - 1;
            int numOfRowsInARowGroup = 4;
            int[] widthsArray = new int[] { 10, 50, 1, 100 };
            // please also look at tableWithLayoutResultNothingTest01
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            Cell cell;
            for (int k = 0; k < widthsArray.Length; k++) {
                for (int j = 0; j < numOfRowsInARowGroup; j++) {
                    for (int i = 0; i < numOfColumns; i++) {
                        cell = new Cell(1, 1 + i % 2).Add(new Paragraph("Cell" + i));
                        cell.SetBorder(new SolidBorder(new DeviceGray(i / (float)numOfColumns), widthsArray[k]));
                        table.AddCell(cell);
                    }
                }
                table.Flush();
            }
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithLayoutResultNothingTest01() {
            String testName = "tableWithLayoutResultNothingTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A1.Rotate());
            float[] colWidths = new float[] { 300, 150, 50, 100 };
            int numOfColumns = colWidths.Length - 1;
            int numOfRowsInARowGroup = 4;
            int[] widthsArray = new int[] { 10, 50, 1, 100 };
            // please also look at largeTableWithLayoutResultNothingTest01
            Table table = new Table(UnitValue.CreatePointArray(colWidths), false);
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetFixedLayout();
            Cell cell;
            for (int k = 0; k < widthsArray.Length; k++) {
                for (int j = 0; j < numOfRowsInARowGroup; j++) {
                    for (int i = 0; i < numOfColumns; i++) {
                        cell = new Cell(1, 1 + i % 2).Add(new Paragraph("Cell" + i));
                        cell.SetBorder(new SolidBorder(new DeviceGray(i / (float)numOfColumns), widthsArray[k]));
                        table.AddCell(cell);
                    }
                }
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void LargeTableWithLayoutResultNothingTest02() {
            String testName = "largeTableWithLayoutResultNothingTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            float[] colWidths = new float[] { 200, 1, 2, 4 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            Cell cell1 = new Cell().Add(new Paragraph("Cell1"));
            Cell cell2 = new Cell().Add(new Paragraph("Cell2"));
            Cell cell3 = new Cell().Add(new Paragraph("Cell3"));
            Cell cell4 = new Cell().Add(new Paragraph("Cell4"));
            table.AddCell(cell1);
            table.AddCell(cell2);
            table.AddCell(cell3);
            table.AddCell(cell4);
            table.Flush();
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableWithLayoutResultNothingTest03() {
            String testName = "largeTableWithLayoutResultNothingTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            Cell cell1 = new Cell().Add(new Paragraph("Cell1"));
            Cell cell2 = new Cell().Add(new Paragraph("Cell2"));
            Cell cell3 = new Cell().Add(new Paragraph("Cell3"));
            Cell cell4 = new Cell().Add(new Paragraph("Cell4"));
            table.AddCell(cell1);
            table.AddCell(cell2);
            table.AddCell(cell3);
            table.AddCell(cell4);
            table.Flush();
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        private class DifferentPagesDocumentRenderer : DocumentRenderer {
            private int pageNum = 0;

            public DifferentPagesDocumentRenderer(Document document)
                : base(document) {
            }

            protected internal override PageSize AddNewPage(PageSize customPageSize) {
                PageSize newPageSize = null;
                switch (pageNum) {
                    case 0: {
                        newPageSize = PageSize.A4.Rotate();
                        break;
                    }

                    case 1: {
                        newPageSize = PageSize.A3.Rotate();
                        break;
                    }

                    case 2:
                    default: {
                        newPageSize = PageSize.A5.Rotate();
                        break;
                    }
                }
                return base.AddNewPage(newPageSize);
            }

            protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
                if (null != overflowResult && null != overflowResult.GetOccupiedArea()) {
                    pageNum = overflowResult.GetOccupiedArea().GetPageNumber();
                }
                return base.UpdateCurrentArea(overflowResult);
            }
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableSplitTest01() {
            String testName = "largeTableSplitTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            LargeTableSplitTest(outFileName, 100, 1, false, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableSplitSeparateTest() {
            String testName = "largeTableSplitSeparateTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            LargeTableSplitTest(outFileName, 100, 1, false, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableSplitFooterTest() {
            String testName = "largeTableSplitFooterTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            LargeTableSplitTest(outFileName, 280, 6, true, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        private void LargeTableSplitTest(String outFileName, float pageHeight, float rowsNumber, bool addFooter, bool
             separate) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, pageHeight));
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            if (addFooter) {
                Cell cell = new Cell(1, 4).Add(new Paragraph("Table footer: continue on next page"));
                table.AddFooterCell(cell);
            }
            if (separate) {
                table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            }
            doc.Add(table);
            for (int i = 0; i < rowsNumber; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))));
                table.Flush();
            }
            table.Complete();
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableWithTableBorderSplitTest() {
            String testName = "largeTableWithTableBorderSplitTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 100));
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            table.SetBorder(new SolidBorder(ColorConstants.BLUE, 2));
            for (int i = 0; i < 1; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))));
                table.Flush();
            }
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void LargeTableWithCellBordersSplitTest() {
            String testName = "largeTableWithCellBordersSplitTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 100));
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            for (int i = 0; i < 1; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))).SetBorderBottom(new SolidBorder(ColorConstants
                    .BLUE, 2)));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))).SetBorderBottom(new SolidBorder(ColorConstants
                    .RED, 5)));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))).SetBorderBottom(new SolidBorder(ColorConstants
                    .GREEN, 7)));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))).SetBorderBottom(new SolidBorder(ColorConstants
                    .BLUE, 10)));
                table.Flush();
            }
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableSplitFooter2Test() {
            String testName = "largeTableSplitFooter2Test.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 400));
            float[] colWidths = new float[] { 100 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetBorderTop(new SolidBorder(ColorConstants.YELLOW
                , 15)).SetBorderBottom(new SolidBorder(ColorConstants.GREEN, 35)));
            table.AddCell(new Cell().Add(new Paragraph("Cell1")).SetHeight(400).SetBorderBottom(new SolidBorder(ColorConstants
                .BLUE, 20)));
            table.Flush();
            table.AddCell(new Cell().Add(new Paragraph("Cell2")).SetHeight(200).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 10)));
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableCollapsingSplitTest() {
            String testName = "largeTableCollapsingSplitTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 400));
            float[] colWidths = new float[] { 100 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.Add(table);
            table.AddCell(new Cell().Add(new Paragraph("Cell1")).SetHeight(1000).SetBorderBottom(new SolidBorder(ColorConstants
                .BLUE, 20)));
            table.Flush();
            table.AddCell(new Cell().Add(new Paragraph("Cell2")).SetHeight(1000).SetBorderTop(new SolidBorder(ColorConstants
                .RED, 40)));
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableOnDifferentPages01() {
            String testName = "largeTableOnDifferentPages01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.SetRenderer(new LargeElementTest.DifferentPagesDocumentRenderer(doc));
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            // please also look at tableOnDifferentPages01
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            for (int i = 0; i < 28; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))));
                if (0 == i) {
                    doc.Add(table);
                }
                else {
                    table.Flush();
                }
            }
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableOnDifferentPages01() {
            String testName = "tableOnDifferentPages01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.SetRenderer(new LargeElementTest.DifferentPagesDocumentRenderer(doc));
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            // please also look at largeTableOnDifferentPages01
            Table table = new Table(UnitValue.CreatePointArray(colWidths));
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetFixedLayout();
            for (int i = 0; i < 28; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeTableOnDifferentPages01A() {
            String testName = "largeTableOnDifferentPages01A.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            doc.SetRenderer(new LargeElementTest.DifferentPagesDocumentRenderer(doc));
            doc.Add(table);
            table.AddFooterCell(new Cell(1, 4).Add(new Paragraph("Footer")));
            table.AddHeaderCell(new Cell(1, 4).Add(new Paragraph("Header")));
            for (int i = 0; i < 25; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 3))));
                table.Flush();
            }
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableOnDifferentPages02() {
            String testName = "tableOnDifferentPages02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.SetRenderer(new LargeElementTest.DifferentPagesDocumentRenderer(doc));
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            // please also look at largeTableOnDifferentPages01
            Table table = new Table(UnitValue.CreatePointArray(colWidths));
            table.SetWidth(UnitValue.CreatePointValue(400));
            table.SetFixedLayout();
            for (int i = 0; i < 28; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 1)]
        public virtual void ReuseLargeTableTest01() {
            String testName = "reuseLargeTableTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            float[] colWidths = new float[] { 200, -1, 20, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            table.SetWidth(UnitValue.CreatePercentValue(60));
            doc.SetRenderer(new LargeElementTest.DifferentPagesDocumentRenderer(doc));
            doc.Add(table);
            table.AddFooterCell(new Cell(1, 4).Add(new Paragraph("Footer")));
            table.AddHeaderCell(new Cell(1, 4).Add(new Paragraph("Header")));
            for (int i = 0; i < 25; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 1))));
                if (i != 24) {
                    table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 2))));
                    table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 3))));
                    table.Flush();
                }
            }
            table.Complete();
            // One can relayout the table (it still has footer, f.i.)
            LayoutResult relayoutResult = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext
                (new LayoutArea(0, new Rectangle(10000, 10000))));
            // But one cannot add content to the table anymore
            try {
                for (int i = 0; i < 25; i++) {
                    table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 0))));
                    NUnit.Framework.Assert.IsTrue(false, "The line above should have thrown an exception.");
                    table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 1))));
                    table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 2))));
                    table.AddCell(new Cell().Add(new Paragraph("Cell#" + (i * 4 + 3))));
                }
                doc.Add(table);
            }
            catch (PdfException e) {
                if (!e.Message.Equals(PdfException.CannotAddCellToCompletedLargeTable)) {
                    throw;
                }
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeEmptyTableTest() {
            String testName = "largeEmptyTableTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1), true);
            doc.Add(table);
            table.SetBorderTop(new SolidBorder(ColorConstants.ORANGE, 100)).SetBorderBottom(new SolidBorder(ColorConstants
                .MAGENTA, 150));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

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
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell")));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell")));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 8)]
        public virtual void LargeEmptyTableTest02Separated() {
            String testName = "largeEmptyTableTest02Separated.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(3), true);
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 3; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell")));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Add(new AreaBreak());
            table = new Table(UnitValue.CreatePercentArray(3), true);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddHeaderCell(new Cell().Add(new Paragraph("Header" + i)));
                table.AddFooterCell(new Cell().Add(new Paragraph("Footer" + i)));
            }
            table.AddCell(new Cell().Add(new Paragraph("Cell")));
            table.Complete();
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorder(new SolidBorder(ColorConstants
                .ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        // TODO DEVSIX-3953
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void LargeTableFooterNotFitTest() {
            String testName = "largeTableFooterNotFitTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 100));
            float[] colWidths = new float[] { 200, -1, 40, 40 };
            Table table = new Table(UnitValue.CreatePointArray(colWidths), true);
            Cell footerCell = new Cell(1, 4).Add(new Paragraph("Table footer: continue on next page"));
            table.AddFooterCell(footerCell);
            doc.Add(table);
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 0))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 1))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 2))));
                table.AddCell(new Cell().Add(new Paragraph("Cell" + (i * 4 + 3))));
                table.Flush();
            }
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }
    }
}
