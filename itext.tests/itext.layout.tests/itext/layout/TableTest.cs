/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TableTest : AbstractTableTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TableTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TableTest/";

        private const String TEXT_CONTENT = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
             + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";

        private const String SHORT_TEXT_CONTENT = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";

        private const String MIDDLE_TEXT_CONTENT = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
             + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest01() {
            String testName = "tableTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 2")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
            NUnit.Framework.Assert.AreEqual("Cell[row=0, col=0, rowspan=1, colspan=1]", table.GetCell(0, 0).ToString()
                );
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest02() {
            String testName = "tableTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new Cell().Add(new Paragraph("cell 2, 1"))).AddCell
                (new Cell().Add(new Paragraph("cell 2, 2"))).AddCell(new Cell().Add(new Paragraph("cell 3, 1"))).AddCell
                (new Cell());
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest03() {
            String testName = "tableTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String textContent1 = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n";
            String textContent2 = "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n"
                 + "Aenean nec lorem. In porttitor. Donec laoreet nonummy augue.\n" + "Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. Mauris eget neque at sem venenatis eleifend. Ut nonummy.\n";
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + textContent1
                ))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + textContent1 + textContent2))).AddCell(new Cell
                ().Add(new Paragraph("cell 2, 1\n" + textContent2 + textContent1))).AddCell(new Cell().Add(new Paragraph
                ("cell 2, 2\n" + textContent2)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest04() {
            String testName = "tableTest04.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                )));
            table.AddCell(new Cell(3, 1).Add(new Paragraph("cell 1, 2:3\n" + TEXT_CONTENT + TEXT_CONTENT + TEXT_CONTENT
                )));
            table.AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + TEXT_CONTENT)));
            table.AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest05() {
            String testName = "tableTest05.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell(3, 1).Add(new Paragraph("cell 1, 1:3\n"
                 + TEXT_CONTENT + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + 
                TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + TEXT_CONTENT))).AddCell(new Cell(
                ).Add(new Paragraph("cell 3, 2\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest06() {
            String testName = "tableTest06.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell(3, 1).Add(new Paragraph("cell 1, 2:3\n" + TEXT_CONTENT + TEXT_CONTENT))).AddCell(
                new Cell().Add(new Paragraph("cell 2, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n"
                 + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest07() {
            String testName = "tableTest07.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell(3, 1).Add(new Paragraph("cell 1, 1:3\n"
                 + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + TEXT_CONTENT)))
                .AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 3, 2\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest08() {
            String testName = "tableTest08.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell(3, 2).Add(new Paragraph("cell 1:2, 1:3\n"
                 + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 1, 3\n" + TEXT_CONTENT)))
                .AddCell(new Cell().Add(new Paragraph("cell 2, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 3, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 4, 3\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest09() {
            String testName = "tableTest09.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" 
                + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + shortTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 3\n" + middleTextContent))).AddCell(new Cell(3, 2).Add(new Paragraph
                ("cell 2:2, 1:3\n" + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 2, 3\n"
                 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 3\n" + middleTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph
                ("cell 5, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 5, 2\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 5, 3\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest10() {
            String testName = "tableTest10.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Table 1"));
            Table table = new Table(new float[] { 100, 100 }).AddCell(new Cell().Add(new Paragraph("1, 1"))).AddCell(new 
                Cell().Add(new Paragraph("1, 2"))).AddCell(new Cell().Add(new Paragraph("2, 1"))).AddCell(new Cell().Add
                (new Paragraph("2, 2")));
            doc.Add(table);
            doc.Add(new Paragraph("Table 2"));
            Table table2 = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("1, 1"))).AddCell(new 
                Cell().Add(new Paragraph("1, 2"))).AddCell(new Cell().Add(new Paragraph("2, 1"))).AddCell(new Cell().Add
                (new Paragraph("2, 2")));
            doc.Add(table2);
            doc.Add(new Paragraph("Table 3"));
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(sourceFolder + "itext.png"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 50);
            Table table3 = new Table(new float[] { 100, 100 }).AddCell(new Cell().Add(new Paragraph("1, 1"))).AddCell(
                new Cell().Add(image)).AddCell(new Cell().Add(new Paragraph("2, 1"))).AddCell(new Cell().Add(new Paragraph
                ("2, 2")));
            doc.Add(table3);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest11() {
            String testName = "tableTest11.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 2, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 3, 2\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether
                (true).Add(new Paragraph("cell 5, 1\n" + middleTextContent))).AddCell(new Cell().SetKeepTogether(true)
                .Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new 
                Paragraph("cell 6, 1\n" + middleTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph
                ("cell 6, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 1\n"
                 + middleTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 2\n" + middleTextContent
                )));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest12() {
            String testName = "tableTest12.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 2, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 3, 2\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether
                (true).Add(new Paragraph("cell 5, 1\n" + middleTextContent))).AddCell(new Cell().SetKeepTogether(true)
                .Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new 
                Paragraph("cell 6, 1\n" + middleTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph
                ("cell 6, 2\n" + shortTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 1\n"
                 + middleTextContent))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 7, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 8, 1\n" + middleTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 8, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 9, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 9, 2\n" + middleTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 10, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 10, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 11, 1\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 11, 2\n" + shortTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest13() {
            String testName = "tableTest13.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 2, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 3, 2\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 5, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 5, 2\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n" + middleTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 6, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 7, 1\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 7, 2\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest14() {
            String testName = "tableTest14.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell(3, 2).Add(new Paragraph("cell 1:2, 1:3\n"
                 + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 1, 3\n" + TEXT_CONTENT)))
                .AddCell(new Cell().Add(new Paragraph("cell 2, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 3, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 4, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 5, 1\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 5, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + middleTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 6, 3\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest15() {
            String testName = "tableTest15.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" 
                + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + shortTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph
                ("cell 2, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 2, 3\n" + middleTextContent))).AddCell(new Cell(3, 2).Add
                (new Paragraph("cell 3:2, 1:3\n" + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 3, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + TEXT_CONTENT))).
                AddCell(new Cell().Add(new Paragraph("cell 5, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 6, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 7, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 7, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 7, 3\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest16() {
            String testName = "tableTest16.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String longTextContent = "1. " + TEXT_CONTENT + "2. " + TEXT_CONTENT + "3. " + TEXT_CONTENT + "4. " + TEXT_CONTENT
                 + "5. " + TEXT_CONTENT + "6. " + TEXT_CONTENT + "7. " + TEXT_CONTENT + "8. " + TEXT_CONTENT + "9. " +
                 TEXT_CONTENT;
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + longTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + middleTextContent)).SetBorder(new SolidBorder
                (ColorConstants.RED, 2))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + middleTextContent + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + longTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WideFirstCellBorderDoesntAffectSecondCellTest() {
            String testName = "wideFirstCellBorderDoesntAffectSecondCellTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String longTextContent = "1. " + TEXT_CONTENT + "2. " + TEXT_CONTENT + "3. " + TEXT_CONTENT + "4. " + TEXT_CONTENT
                 + "5. " + TEXT_CONTENT + "6. " + TEXT_CONTENT + "7. " + TEXT_CONTENT + "8. " + TEXT_CONTENT + "9. " +
                 TEXT_CONTENT;
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 2")).SetBorder(new SolidBorder(ColorConstants.RED, 100))).AddCell
                (new Cell().Add(new Paragraph("cell 2, 1\n" + longTextContent))).AddCell(new Cell().Add(new Paragraph(
                "cell 2, 2\n" + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + longTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + longTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 2, 1\n" + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + longTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + longTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 2, 2\n" + longTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 1\n" + longTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + longTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void SimpleTableTest17() {
            String testName = "tableTest17.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 50, 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new Cell().Add(new Paragraph("cell 1, 3")));
            String longText = "Long text, very long text. ";
            for (int i = 0; i < 4; i++) {
                longText += longText;
            }
            table.AddCell(new Cell().Add(new Paragraph("cell 2.1\n" + longText).SetKeepTogether(true)));
            table.AddCell("cell 2.2\nShort text.");
            table.AddCell(new Cell().Add(new Paragraph("cell 2.3\n" + longText)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void SimpleTableTest18() {
            String testName = "tableTest18.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph(TEXT_CONTENT));
            Table table = new Table(new float[] { 50, 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new Cell().Add(new Paragraph("cell 1, 3")));
            String longText = "Long text, very long text. ";
            for (int i = 0; i < 4; i++) {
                longText += longText;
            }
            table.AddCell(new Cell().Add(new Paragraph("cell 2.1\n" + longText).SetKeepTogether(true)));
            table.AddCell("cell 2.2\nShort text.");
            table.AddCell(new Cell().Add(new Paragraph("cell 2.3\n" + longText)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void SimpleTableTest19() {
            String testName = "tableTest19.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell(3, 2).Add(new Paragraph("cell 1:2, 1:3\n"
                 + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 1, 3\n" + TEXT_CONTENT)))
                .AddCell(new Cell().Add(new Paragraph("cell 2, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 3, 3\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new iText.Layout.Element.Image(ImageDataFactory
                .Create(sourceFolder + "red.png")))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n" + SHORT_TEXT_CONTENT
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + MIDDLE_TEXT_CONTENT))).AddCell(new Cell().Add
                (new Paragraph("cell 5, 1\n" + SHORT_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 5, 2\n"
                 + SHORT_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 5, 3\n" + MIDDLE_TEXT_CONTENT))).AddCell
                (new Cell().Add(new Paragraph("cell 6, 1\n" + MIDDLE_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 6, 2\n" + MIDDLE_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + MIDDLE_TEXT_CONTENT
                )));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void SimpleTableTest20() {
            String testName = "tableTest20.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "red.png")))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n"
                 + SHORT_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + MIDDLE_TEXT_CONTENT))).AddCell
                (new Cell().Add(new Paragraph("cell 5, 1\n" + SHORT_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 5, 2\n" + SHORT_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 5, 3\n" + MIDDLE_TEXT_CONTENT
                ))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n" + MIDDLE_TEXT_CONTENT))).AddCell(new Cell().Add
                (new Paragraph("cell 6, 2\n" + MIDDLE_TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n"
                 + MIDDLE_TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void SimpleTableTest21() {
            String testName = "tableTest21.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            doc.Add(new Paragraph(TEXT_CONTENT));
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "red.png")))).AddCell(new Cell().Add(new Paragraph("cell 4, 2\n"
                 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + middleTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 5, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph
                ("cell 5, 2\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 5, 3\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n" + middleTextContent))).AddCell(new Cell().Add(new 
                Paragraph("cell 6, 2\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent
                )));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest22() {
            String testName = "tableTest22.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new UnitValue[] { UnitValue.CreatePointValue(30), UnitValue.CreatePointValue(30), 
                UnitValue.CreatePercentValue(30), UnitValue.CreatePercentValue(30) }).AddCell(new Cell().Add(new Paragraph
                ("cell 1, 1"))).AddCell(new Cell().Add(new Paragraph("cell 1, 2"))).AddCell(new Cell().Add(new Paragraph
                ("cell 1, 3"))).AddCell(new Cell().Add(new Paragraph("cell 1, 4")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTableTest23() {
            String testName = "tableTest23.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(2).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell(new Cell().Add(new 
                Paragraph("longer cell 1, 2"))).AddCell(new Cell().Add(new Paragraph("cell 1, 3"))).AddCell(new Cell()
                .Add(new Paragraph("cell 1, 4")));
            doc.Add(table);
            table = new Table(2).SetFixedLayout().AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell(new Cell
                ().Add(new Paragraph("longer cell 1, 2"))).AddCell(new Cell().Add(new Paragraph("cell 1, 3"))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 4")));
            doc.Add(table);
            table = new Table(2, true).AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell(new Cell().Add(new 
                Paragraph("longer cell 1, 2")));
            doc.Add(table);
            table.AddCell(new Cell().Add(new Paragraph("cell 1, 3"))).AddCell(new Cell().Add(new Paragraph("cell 1, 4"
                ))).Flush();
            table.Complete();
            table = new Table(2, true).SetFixedLayout().AddCell(new Cell().Add(new Paragraph("cell 1, 1"))).AddCell(new 
                Cell().Add(new Paragraph("longer cell 1, 2")));
            doc.Add(table);
            table.AddCell(new Cell().Add(new Paragraph("cell 1, 3"))).AddCell(new Cell().Add(new Paragraph("cell 1, 4"
                ))).Flush();
            table.Complete();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WidthInPercentShouldBeResetAfterOverflow() {
            String testName = "widthInPercentShouldBeResetAfterOverflow.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Div().SetHeight(730).SetWidth(523));
            Table table = new Table(2).UseAllAvailableWidth().SetFixedLayout().AddCell(new Cell().Add(new Paragraph("Hello"
                )).SetWidth(UnitValue.CreatePercentValue(20))).AddCell(new Cell().Add(new Paragraph("World")).SetWidth
                (UnitValue.CreatePercentValue(80)));
            // will be added on the first page
            doc.Add(table);
            // will be added on the second page
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest01() {
            String testName = "bigRowspanTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String longTextContent = "1. " + TEXT_CONTENT + "2. " + TEXT_CONTENT + "3. " + TEXT_CONTENT + "4. " + TEXT_CONTENT
                 + "5. " + TEXT_CONTENT + "6. " + TEXT_CONTENT + "7. " + TEXT_CONTENT + "8. " + TEXT_CONTENT + "9. " +
                 TEXT_CONTENT;
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n" + longTextContent))).AddCell(new Cell().Add
                (new Paragraph("cell 2, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n"
                 + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + middleTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 5, 1\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest02() {
            String testName = "bigRowspanTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String longTextContent = "1. " + TEXT_CONTENT + "2. " + TEXT_CONTENT + "3. " + TEXT_CONTENT + "4. " + TEXT_CONTENT
                 + "5. " + TEXT_CONTENT + "6. " + TEXT_CONTENT + "7. " + TEXT_CONTENT + "8. " + TEXT_CONTENT + "9. " +
                 TEXT_CONTENT;
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n" + longTextContent))).AddCell(new Cell().Add
                (new Paragraph("cell 2, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 5, 1\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest03() {
            String testName = "bigRowspanTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n" + middleTextContent))).AddCell(new Cell().Add
                (new Paragraph("cell 2, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell().Add(new Paragraph("cell 4, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph
                ("cell 5, 1\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest04() {
            String testName = "bigRowspanTest04.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String longTextContent = "1. " + TEXT_CONTENT + "2. " + TEXT_CONTENT + "3. " + TEXT_CONTENT + "4. " + TEXT_CONTENT
                 + "5. " + TEXT_CONTENT + "6. " + TEXT_CONTENT + "7. " + TEXT_CONTENT + "8. " + TEXT_CONTENT + "9. " +
                 TEXT_CONTENT;
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell(5, 1).Add(new Paragraph("cell 1, 2\n" + longTextContent))).AddCell(new Cell().Add
                (new Paragraph("cell 2, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell().SetKeepTogether(true).Add(new Paragraph("cell 4, 1\n" + TEXT_CONTENT))).AddCell
                (new Cell().Add(new Paragraph("cell 5, 1\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest05() {
            String testName = "bigRowspanTest05.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String longTextContent = "1. " + TEXT_CONTENT + "2. " + TEXT_CONTENT + "3. " + TEXT_CONTENT + "4. " + TEXT_CONTENT
                 + "5. " + TEXT_CONTENT + "6. " + TEXT_CONTENT + "7. " + TEXT_CONTENT + "8. " + TEXT_CONTENT + "9. " +
                 TEXT_CONTENT;
            Table table = new Table(new float[] { 250, 250 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" + TEXT_CONTENT
                ))).AddCell(new Cell(2, 1).Add(new Paragraph("cell 1, 1 and 2\n" + longTextContent))).AddCell(new Cell
                ().Add(new Paragraph("cell 2, 1\n" + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 3, 1\n"
                 + TEXT_CONTENT))).AddCell(new Cell().Add(new Paragraph("cell 3, 2\n" + TEXT_CONTENT)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest06() {
            String testName = "bigRowspanTest06.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().AddCell(new Cell(2, 1).Add
                (new Paragraph("col 1 row 2"))).AddCell(new Cell(2, 1).Add(new Paragraph("col 2 row 2"))).AddCell(new 
                Cell(1, 1).Add(new Paragraph("col 1 row 3"))).AddCell(new Cell(1, 1).Add(new Paragraph("col 2 row 3"))
                );
            table.SetBorderTop(new SolidBorder(ColorConstants.GREEN, 50)).SetBorderBottom(new SolidBorder(ColorConstants
                .ORANGE, 40));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowspanTest07() {
            String testName = "bigRowspanTest07.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            for (int i = 0; i < 100; i++) {
                Cell cell = new Cell();
                cell.Add(new Paragraph("Cell " + i));
                Cell cell2 = new Cell(2, 1);
                cell2.Add(new Paragraph("Cell with Rowspan"));
                Cell cell3 = new Cell();
                cell3.Add(new Paragraph("Cell " + i + ".2"));
                table.AddCell(cell);
                table.AddCell(cell2);
                table.AddCell(cell3);
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DifferentPageOrientationTest01() {
            String testName = "differentPageOrientationTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String textContent1 = "Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.";
            String textContent2 = "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.";
            String textContent3 = "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.";
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            for (int i = 0; i < 20; i++) {
                table.AddCell(new Cell().Add(new Paragraph(textContent1))).AddCell(new Cell().Add(new Paragraph(textContent3
                    ))).AddCell(new Cell().Add(new Paragraph(textContent2))).AddCell(new Cell().Add(new Paragraph(textContent3
                    ))).AddCell(new Cell().Add(new Paragraph(textContent2))).AddCell(new Cell().Add(new Paragraph(textContent1
                    ))).AddCell(new Cell().Add(new Paragraph(textContent2))).AddCell(new Cell().Add(new Paragraph(textContent1
                    ))).AddCell(new Cell().Add(new Paragraph(textContent3)));
            }
            doc.SetRenderer(new TableTest.RotatedDocumentRenderer(doc, pdfDoc));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ExtendLastRowTest01() {
            String testName = "extendLastRowTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(sourceFolder + "itext.png"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            for (int i = 0; i < 20; i++) {
                table.AddCell(image);
            }
            doc.Add(new Paragraph("Extend the last row on each page"));
            table.SetExtendBottomRow(true);
            doc.Add(table);
            doc.Add(new Paragraph("Extend all last rows on each page except final one"));
            table.SetExtendBottomRow(false);
            table.SetExtendBottomRowOnSplit(true);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void ToLargeElementWithKeepTogetherPropertyInTableTest01() {
            String testName = "toLargeElementWithKeepTogetherPropertyInTableTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue
                (100)).SetFixedLayout();
            Cell cell = new Cell();
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            String result = "";
            for (int i = 0; i < 53; i++) {
                result += str;
            }
            Paragraph p = new Paragraph(new Text(result));
            p.SetProperty(Property.KEEP_TOGETHER, true);
            cell.Add(p);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void ToLargeElementInTableTest01() {
            String testName = "toLargeElementInTableTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "toLargeElementInTableTest01.pdf"));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 5 });
            table.SetWidth(5).SetProperty(Property.TABLE_LAYOUT, "fixed");
            Cell cell = new Cell();
            Paragraph p = new Paragraph(new Text("a"));
            cell.Add(p);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NestedTablesCollapseTest01() {
            String testName = "nestedTablesCollapseTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Cell cell;
            Table outertable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            Table innertable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            // first row
            // column 1
            cell = new Cell().Add(new Paragraph("Record Ref:"));
            cell.SetBorder(Border.NO_BORDER);
            innertable.AddCell(cell);
            // column 2
            cell = new Cell().Add(new Paragraph("GN Staff"));
            cell.SetPaddingLeft(2);
            innertable.AddCell(cell);
            // spacing
            cell = new Cell(1, 2);
            cell.SetHeight(3);
            cell.SetBorder(Border.NO_BORDER);
            innertable.AddCell(cell);
            // second row
            // column 1
            cell = new Cell().Add(new Paragraph("Hospital:"));
            cell.SetBorder(Border.NO_BORDER);
            innertable.AddCell(cell);
            // column 2
            cell = new Cell().Add(new Paragraph("Derby Royal"));
            cell.SetPaddingLeft(2);
            innertable.AddCell(cell);
            // spacing
            cell = new Cell(1, 2);
            cell.SetHeight(3);
            cell.SetBorder(Border.NO_BORDER);
            innertable.AddCell(cell);
            // first nested table
            cell = new Cell().Add(innertable);
            outertable.AddCell(cell);
            // add the table
            doc.Add(outertable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NestedTableSkipHeaderFooterTest() {
            String testName = "nestedTableSkipHeaderFooter.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)")));
            table.AddFooterCell(new Cell(1, 5).Add(new Paragraph("Continue on next page")));
            table.SetSkipFirstHeader(true);
            table.SetSkipLastFooter(true);
            for (int i = 0; i < 350; i++) {
                table.AddCell(new Cell().Add(new Paragraph((i + 1).ToString())));
            }
            Table t = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            t.AddCell(new Cell().SetBorder(new SolidBorder(ColorConstants.RED, 1)).SetPaddings(3, 3, 3, 3).Add(table));
            doc.Add(t);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NestedTablesWithMarginsTest01() {
            String testName = "nestedTablesWithMarginsTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A8.Rotate());
            Table innerTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            for (int i = 0; i < 4; i++) {
                innerTable.AddCell(new Cell().Add(new Paragraph("Hello" + i)));
            }
            Table outerTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add
                (innerTable));
            outerTable.SetMarginTop(10);
            doc.Add(outerTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void SplitTableOnShortPage() {
            String testName = "splitTableOnShortPage.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(300, 98));
            doc.Add(new Paragraph("Table with setKeepTogether(true):"));
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.SetKeepTogether(true);
            Cell cell = new Cell(3, 1);
            cell.Add(new Paragraph("G"));
            cell.Add(new Paragraph("R"));
            cell.Add(new Paragraph("P"));
            table.AddCell(cell);
            table.AddCell("middle row 1");
            cell = new Cell(3, 1);
            cell.Add(new Paragraph("A"));
            cell.Add(new Paragraph("B"));
            cell.Add(new Paragraph("C"));
            table.AddCell(cell);
            table.AddCell("middle row 2");
            table.AddCell("middle row 3");
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table with setKeepTogether(false):"));
            table.SetKeepTogether(false);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SplitCellWithStyles() {
            String testName = "splitCellWithStyles.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            String text = "Make Gretzky Great Again";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetBorder(Border.NO_BORDER
                ).SetMarginTop(10).SetMarginBottom(10);
            Style cellStyle = new Style();
            cellStyle.SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER).SetBorderTop(new SolidBorder(ColorConstants
                .BLUE, 1)).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 1));
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph(JavaUtil.IntegerToString(i))).AddStyle(cellStyle));
                table.AddCell(new Cell().Add(new Paragraph(text)).AddStyle(cellStyle));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageInTableTest_HA() {
            String testName = "imageInTableTest_HA.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(sourceFolder + "itext.png"
                )));
            iText.Layout.Element.Image imageL = new iText.Layout.Element.Image(xObject);
            imageL.SetHorizontalAlignment(HorizontalAlignment.LEFT);
            iText.Layout.Element.Image imageC = new iText.Layout.Element.Image(xObject);
            imageC.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            iText.Layout.Element.Image imageR = new iText.Layout.Element.Image(xObject);
            imageR.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            doc.Add(new Paragraph("Table"));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(imageL
                )).AddCell(new Cell().Add(imageC)).AddCell(new Cell().Add(imageR));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CellAlignmentAndSplittingTest01() {
            String testName = "cellAlignmentAndSplittingTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            for (int i = 0; i < 20; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + " Liberté!\nÉgalité!\nFraternité!")).SetHeight(100).SetVerticalAlignment
                    (VerticalAlignment.MIDDLE));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CellAlignmentAndKeepTogetherTest01() {
            String testName = "cellAlignmentAndKeepTogetherTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            for (int i = 0; i < 20; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + " Liberté!\nÉgalité!\nFraternité!")).SetHeight(100).SetKeepTogether
                    (true).SetVerticalAlignment(VerticalAlignment.MIDDLE));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 3)]
        [NUnit.Framework.Test]
        public virtual void TableWithSetHeightProperties01() {
            String testName = "tableWithSetHeightProperties01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n";
            doc.Add(new Paragraph("Default layout:"));
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new 
                Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(
                new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1)
                .Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 
                1).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell(
                ).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell(
                ).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is bigger than needed:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetHeight(1700);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is shorter than needed:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetHeight(200);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Some cells' heights are set:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3)).SetHeight(300)).AddCell(new Cell().Add
                (new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2)
                .Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3)).SetHeight(40)).AddCell
                (new Cell(2, 1).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell
                (new Cell(2, 1).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell
                (new Cell().Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell
                (new Cell().Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)).SetHeight
                (20));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetHeight(1700);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 3)]
        [NUnit.Framework.Test]
        public virtual void TableWithSetHeightProperties02() {
            String testName = "tableWithSetHeightProperties02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n";
            doc.Add(new Paragraph("Default layout:"));
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new 
                Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(
                new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1)
                .Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 
                1).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell(
                ).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell(
                ).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's max height is bigger than needed:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetMaxHeight(1300);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's max height is shorter than needed:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetMaxHeight(300);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's min height is bigger than needed:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetMinHeight(1300);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's min height is shorter than needed:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.RED, 5))).AddCell(new Cell(2, 1).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY, 7))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 12))).AddCell(new Cell().Add(new Paragraph(
                textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 1)));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetMinHeight(300);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Some cells' heights are set:"));
            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(1, 2).Add(new Paragraph
                (textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3)).SetMinHeight(300)).AddCell(new Cell(
                ).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GREEN, 1))).AddCell(new Cell(
                1, 2).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 3)).SetMaxHeight(
                40)).AddCell(new Cell(2, 1).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.RED
                , 5))).AddCell(new Cell(2, 1).Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.GRAY
                , 7))).AddCell(new Cell().Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.BLUE, 
                12))).AddCell(new Cell().Add(new Paragraph(textByron)).SetBorder(new SolidBorder(ColorConstants.CYAN, 
                1)).SetMaxHeight(20));
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
            table.SetHeight(1700);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithSetHeightProperties03() {
            String testName = "tableWithSetHeightProperties03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n";
            String textFrance = "Liberte Egalite Fraternite";
            doc.Add(new Paragraph("Default layout:"));
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new 
                Paragraph(textFrance)).SetBackgroundColor(ColorConstants.RED)).AddCell(new Cell().Add(new Paragraph(textFrance
                )).SetBackgroundColor(ColorConstants.GREEN)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor
                (ColorConstants.BLUE));
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is bigger than needed:"));
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.RED)).AddCell(new Cell().Add(new Paragraph(textFrance)
                ).SetBackgroundColor(ColorConstants.GREEN)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor
                (ColorConstants.BLUE));
            table.SetHeight(600);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is bigger than needed and some cells have HEIGHT property:"));
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.RED)).AddCell(new Cell().Add(new Paragraph(textFrance)
                ).SetBackgroundColor(ColorConstants.GREEN).SetHeight(30)).AddCell(new Cell().Add(new Paragraph(textFrance
                )).SetBackgroundColor(ColorConstants.BLUE));
            table.SetHeight(600);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is bigger than needed and all cells have HEIGHT property:"));
            table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.RED).SetHeight(25)).AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.GREEN).SetHeight(75)).AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.BLUE).SetHeight(50));
            table.SetHeight(600);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is bigger than needed and some cells have HEIGHT property:"));
            table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.RED).SetHeight(25)).AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.BLUE)).AddCell(new Cell().Add(new Paragraph(textFrance
                )).SetBackgroundColor(ColorConstants.GREEN)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor
                (ColorConstants.RED)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor(ColorConstants
                .BLUE).SetHeight(50)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor(ColorConstants
                .GREEN));
            table.SetHeight(600);
            doc.Add(table);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Table's height is bigger than needed, some cells have big rowspan and HEIGHT property:"
                ));
            table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                (textFrance)).SetBackgroundColor(ColorConstants.RED)).AddCell(new Cell().Add(new Paragraph(textFrance)
                ).SetBackgroundColor(ColorConstants.BLUE)).AddCell(new Cell(2, 1).Add(new Paragraph(textFrance)).SetBackgroundColor
                (ColorConstants.GREEN)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor(ColorConstants
                .RED)).AddCell(new Cell().Add(new Paragraph(textFrance)).SetBackgroundColor(ColorConstants.GREEN).SetHeight
                (50));
            table.SetHeight(600);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithHeaderInTheBottomOfPageTest() {
            String testName = "tableWithHeaderInTheBottomOfPageTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("Text"));
            }
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10 }));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header One")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header Two")));
            table.AddCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("World")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void BigFooterTest01() {
            String testName = "bigFooterTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetHeight(650).SetBorderTop(new SolidBorder(ColorConstants
                .GREEN, 100)));
            table.AddCell(new Cell().Add(new Paragraph("Body")).SetHeight(30));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void BigFooterTest02() {
            String testName = "bigFooterTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetHeight(380).SetBackgroundColor(ColorConstants
                .YELLOW));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header")).SetHeight(380).SetBackgroundColor(ColorConstants
                .BLUE));
            table.AddCell(new Cell().Add(new Paragraph("Body")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithDocumentRelayoutTest() {
            String testName = "tableWithDocumentRelayoutTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4, false);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10 }));
            for (int i = 0; i < 40; i++) {
                table.AddCell(new Cell().Add(new Paragraph("" + (i + 1))));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithKeepTogetherOnCells() {
            String testName = "tableWithKeepTogetherOnCells.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1.3f, 1f, 1f, 1f, 1f, 1f, 1f }));
            table.SetWidth(UnitValue.CreatePercentValue(100)).SetFixedLayout();
            for (int i = 1; i <= 7 * 100; i++) {
                Cell cell = new Cell().SetKeepTogether(true).SetMinHeight(45).Add(new Paragraph("" + i));
                table.AddCell(cell);
            }
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTableTest01() {
            String testName = "emptyTableTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetBorderTop(new SolidBorder(ColorConstants
                .ORANGE, 50)).SetBorderBottom(new SolidBorder(ColorConstants.MAGENTA, 100)));
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().SetPadding(0)
                .SetMargin(0).SetBorder(Border.NO_BORDER)).AddCell(new Cell().SetPadding(0).SetMargin(0).SetBorder(Border
                .NO_BORDER)).AddCell(new Cell().SetPadding(0).SetMargin(0).SetBorder(Border.NO_BORDER)).AddCell(new Cell
                ().SetPadding(0).SetMargin(0).SetBorder(Border.NO_BORDER)).AddCell(new Cell().Add(new Paragraph("Hello"
                ))));
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetMinHeight(300).SetBorderRight
                (new SolidBorder(ColorConstants.ORANGE, 5)).SetBorderTop(new SolidBorder(100)).SetBorderBottom(new SolidBorder
                (ColorConstants.BLUE, 50)));
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTableTest02() {
            String testName = "emptyTableTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(1));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetVerticalBorderSpacing(20);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE, Count = 2)]
        public virtual void TableWithIncompleteFooter() {
            String testName = "tableWithIncompleteFooter.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            table.AddCell("Liberte");
            table.AddCell("Egalite");
            table.AddCell("Fraternite");
            table.AddFooterCell(new Cell(1, 2).Add(new Paragraph("Liberte Egalite")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN)]
        public virtual void TableWithCustomRendererTest01() {
            String testName = "tableWithCustomRendererTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 100));
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Cell No." + i)));
            }
            table.SetNextRenderer(new TableTest.CustomRenderer(table, new Table.RowRange(0, 10)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SkipLastRowTest() {
            // This test checks that the table occupies exactly one page and does not draw its footer.
            // A naive algorithm would have this table on two pages with only one row with data on the second page
            // However, as setSkipLastFooter is true, we can lay out that row with data on the first page and avoid unnecessary footer placement.
            String testName = "skipLastRowTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddHeaderCell("Header 1");
            table.AddHeaderCell("Header 2");
            table.AddFooterCell(new Cell(1, 2).Add(new Paragraph("Footer")));
            table.SetSkipLastFooter(true);
            for (int i = 0; i < 33; i++) {
                table.AddCell("text 1");
                table.AddCell("text 2");
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SkipFooterTest01() {
            String testName = "skipFooterTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            for (int i = 0; i < 19; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + " Liberté!\nÉgalité!\nFraternité!")).SetHeight(100));
            }
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer")).SetHeight(116).SetBackgroundColor(ColorConstants
                .RED));
            // the next line cause the reuse
            table.SetSkipLastFooter(true);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SkipHeaderTest01() {
            String testName = "skipHeaderTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdf);
            // construct a table
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            for (int i = 0; i < 2; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i + " Hello").SetFontSize(18)));
            }
            table.AddHeaderCell(new Cell().Add(new Paragraph(" Header")));
            table.SetSkipFirstHeader(true);
            // add meaningless text to occupy enough place
            for (int i = 0; i < 29; i++) {
                doc.Add(new Paragraph(i + " Hello"));
            }
            // add the table
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableSplitTest01() {
            String testName = "tableSplitTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            String gretzky = "Make Gretzky great again!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A8.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 15));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableSplitTest02() {
            String testName = "tableSplitTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            String gretzky = "Make Gretzky great again!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 15));
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(sourceFolder + "itext.png"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 50);
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(image));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableSplitTest03() {
            String testName = "tableSplitTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            String gretzky = "Make Gretzky great again!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A8.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 15));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableSplitTest04() {
            String testName = "tableSplitTest04.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            String gretzky = "Make Gretzky great again!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A7.Rotate());
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 15));
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(sourceFolder + "itext.png"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 50);
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell(2, 1).Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(image));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            table.AddCell(new Cell().Add(new Paragraph(gretzky)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void TableNothingResultTest() {
            String testName = "tableNothingResultTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.SetKeepTogether(true);
            for (int i = 0; i < 40; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello")));
                table.AddCell(new Cell().Add(new Paragraph("World")));
                table.StartNewRow();
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        public virtual void TableWithEmptyLastRowTest() {
            String testName = "tableWithEmptyLastRowTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.AddCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("World")));
            StartSeveralEmptyRows(table);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING)]
        public virtual void TableWithEmptyRowsBetweenFullRowsTest() {
            String testName = "tableWithEmptyRowsBetweenFullRowsTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.AddCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("World")));
            StartSeveralEmptyRows(table);
            table.AddCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("World")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING, Count = 2
            )]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        public virtual void TableWithEmptyRowAfterJustOneCellTest() {
            String testName = "tableWithEmptyRowAfterJustOneCellTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(3);
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j <= i; j++) {
                    table.AddCell(new Cell().Add(new Paragraph("Hello")));
                }
                StartSeveralEmptyRows(table);
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING, Count = 39
            )]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        public virtual void TableWithAlternatingRowsTest() {
            String testName = "tableWithAlternatingRowsTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            for (int i = 0; i < 40; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello")));
                table.AddCell(new Cell().Add(new Paragraph("World")));
                StartSeveralEmptyRows(table);
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ColoredTableWithColoredCellsTest() {
            String testName = "coloredTableWithColoredCellsTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.SetBackgroundColor(ColorConstants.RED);
            for (int i = 0; i < 40; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello")).SetBackgroundColor(ColorConstants.GREEN));
                table.StartNewRow();
            }
            table.AddCell(new Cell().Add(new Paragraph("Hello")).SetBackgroundColor(ColorConstants.GREEN));
            table.AddCell(new Cell().Add(new Paragraph("World")).SetBackgroundColor(ColorConstants.GREEN));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING, Count = 2
            )]
        public virtual void TableWithEmptyRowsAndSeparatedBordersTest() {
            String testName = "tableWithEmptyRowsAndSeparatedBordersTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.AddCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("World")));
            StartSeveralEmptyRows(table);
            table.AddCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("World")));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        // TODO DEVSIX-6020:Border-collapsing doesn't work in case startNewRow has been called
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING)]
        public virtual void TableWithCollapsedBordersTest() {
            String testName = "tableWithCollapsedBordersTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.AddCell(new Cell().Add(new Paragraph("Hello")).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 
                10)));
            table.AddCell(new Cell().Add(new Paragraph("World")).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 
                10)));
            StartSeveralEmptyRows(table);
            table.AddCell(new Cell().Add(new Paragraph("Hello")).SetBorderTop(new SolidBorder(ColorConstants.RED, 20))
                );
            table.AddCell(new Cell().Add(new Paragraph("World")).SetBorderTop(new SolidBorder(ColorConstants.RED, 20))
                );
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE)]
        public virtual void TableWithCollapsedBordersAndFooterTest() {
            String testName = "tableWithCollapsedBordersAndFooterTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 30 }));
            table.AddCell(new Cell().Add(new Paragraph("Hello")).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 
                10)));
            table.AddCell(new Cell().Add(new Paragraph("World")).SetBorderBottom(new SolidBorder(ColorConstants.BLUE, 
                10)));
            StartSeveralEmptyRows(table);
            table.AddFooterCell(new Cell().Add(new Paragraph("Hello")).SetBorderTop(new SolidBorder(ColorConstants.RED
                , 20)));
            table.AddFooterCell(new Cell().Add(new Paragraph("World")).SetBorderTop(new SolidBorder(ColorConstants.RED
                , 20)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AutoLayoutTest01() {
            String testName = "autoLayoutTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            // Initialize document
            Document doc = new Document(pdf);
            doc.Add(new Paragraph("Simple cell:"));
            Table table = new Table(new float[1]);
            table.AddCell("A cell");
            doc.Add(table);
            doc.Add(new Paragraph("A cell with bold text:"));
            table = new Table(new float[1]);
            table.AddCell("A cell").SetBold();
            doc.Add(table);
            doc.Add(new Paragraph("A cell with italic text:"));
            table = new Table(new float[1]);
            table.AddCell("A cell").SetItalic();
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AutoLayoutTest02() {
            String testName = "autoLayoutTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdf);
            doc.Add(new Paragraph("Simple cell:"));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 5, 95 }));
            table.AddCell(new Cell().Add(new Paragraph("Hellowor ld!")));
            table.AddCell(new Cell().Add(new Paragraph("Long long long Long long long Long long long Long long long text"
                )));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void AutoLayoutTest03() {
            String testName = "autoLayoutTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdf);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1 }));
            table.SetBorder(new SolidBorder(ColorConstants.RED, 100));
            for (int i = 0; i < 3; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello")));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FixedLayoutTest01() {
            String testName = "fixedLayoutTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            // Initialize document
            Document doc = new Document(pdf);
            doc.Add(new Paragraph("Simple table with proportional width. Ignore cell width, because sum(col[*]) < tableWidth:"
                ));
            Table table = new Table(new float[] { 1, 2, 3 }).SetFixedLayout().SetWidth(400);
            table.AddCell("1x");
            table.AddCell("2x");
            table.AddCell("3x");
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FixedLayoutTest02() {
            String testName = "fixedLayoutTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            // Initialize document
            Document doc = new Document(pdf);
            doc.Add(new Paragraph("Simple table with proportional width. Ignore table width, because sum(col[*]) > tableWidth."
                ));
            Table table = new Table(new float[] { 20, 40, 60 }).SetFixedLayout().SetWidth(10);
            table.AddCell("1x");
            table.AddCell("2x");
            table.AddCell("3x");
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void FixedPositionTest01() {
            String testName = "fixedPositionTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            //Initialize PDF document
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            // Initialize document
            Document doc = new Document(pdf);
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            for (int i = 0; i < 100; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello " + i)).SetBackgroundColor(ColorConstants.RED));
            }
            table.SetFixedPosition(150, 300, 200);
            table.SetHeight(300);
            table.SetBackgroundColor(ColorConstants.YELLOW);
            doc.Add(new Paragraph("The next table has fixed position and height property. However set height is shorter than needed and we can place table only partially."
                ));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetHeight(10);
            doc.Add(new Paragraph("The next table has fixed position and height property. However set height is shorter than needed and we cannot fully place even a cell."
                ));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void NestedTableLostContent() {
            // When the test was created, only first line of text was displayed on the first page
            String testName = "nestedTableLostContent.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdf);
            String text = "abacaba absa ";
            for (int i = 0; i < 7; i++) {
                text += text;
            }
            Table innerTable = new Table(UnitValue.CreatePointArray(new float[] { 50 }));
            innerTable.AddCell(text);
            Table outerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }));
            outerTable.AddCell(new Cell().Add(innerTable));
            outerTable.AddCell(new Cell().SetBackgroundColor(ColorConstants.RED).Add(new Div().SetMinHeight(850).SetKeepTogether
                (true)));
            doc.Add(outerTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void NestedTableMinMaxWidthException() {
            // When the test was created, an exception was thrown due to min-max width calculations for an inner table.
            // At some point isOriginalNonSplitRenderer was true for a parent renderer but false for the inner table renderer
            String testName = "nestedTableMinMaxWidthException.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdf);
            String text = "abacaba absa ";
            for (int i = 0; i < 9; i++) {
                text += text;
            }
            Table innerTable = new Table(UnitValue.CreatePointArray(new float[] { 50 }));
            innerTable.AddCell("Small text");
            innerTable.AddCell(new Cell().Add(new Paragraph(text)).SetKeepTogether(true));
            Table outerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1 }));
            outerTable.AddCell(new Cell().Add(innerTable));
            doc.Add(outerTable);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableMinMaxWidthTest01() {
            String testName = "tableMinMaxWidthTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 100 }));
            Cell cell = new Cell().SetWidth(UnitValue.CreatePointValue(216)).Add(new Paragraph("width:72pt"));
            cell.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(72));
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableMinMaxWidthTest02() {
            String testName = "tableMinMaxWidthTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 100 }));
            Cell cell = new Cell().SetWidth(UnitValue.CreatePointValue(216)).Add(new Paragraph("width:72pt"));
            cell.SetMaxWidth(72);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableMinMaxWidthTest03() {
            String testName = "tableMinMaxWidthTest03.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 100 }));
            Cell cell = new Cell().SetWidth(UnitValue.CreatePointValue(50)).Add(new Paragraph("width:72pt"));
            cell.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(72));
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableMinMaxWidthTest04() {
            String testName = "tableMinMaxWidthTest04.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 100 }));
            Cell cell = new Cell().SetWidth(UnitValue.CreatePointValue(50)).Add(new Paragraph("width:72pt"));
            cell.SetMinWidth(72);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableMinMaxWidthTest05() {
            String testName = "tableMinMaxWidthTest05.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1 }));
            table.SetWidth(UnitValue.CreatePercentValue(80));
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.AddCell(new Cell(1, 3).Add(new Paragraph("Cell with colspan 3")));
            table.AddCell(new Cell(2, 1).Add(new Paragraph("Cell with rowspan 2")));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 1")).SetMinWidth(200));
            table.AddCell(new Cell().Add(new Paragraph("row 1; cell 2")).SetMaxWidth(50));
            table.AddCell("row 2; cell 1");
            table.AddCell("row 2; cell 2");
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CellsWithEdgeCaseLeadingTest01() {
            String testName = "cellsWithEdgeCaseLeadingTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            SolidBorder border = new SolidBorder(1f);
            Table table = new Table(UnitValue.CreatePointArray(new float[] { 20, 20, 20, 20 }));
            Paragraph paragraph5 = new Paragraph(new Text("Cell5"));
            Paragraph paragraph6 = new Paragraph(new Text("Cell6"));
            Paragraph paragraph7 = new Paragraph(new Text("Cell7"));
            Paragraph paragraph8 = new Paragraph(new Text("Cell8"));
            Paragraph paragraph13 = new Paragraph("Cell13");
            Paragraph paragraph14 = new Paragraph(new Text(""));
            Paragraph paragraph15 = new Paragraph(new Text("Cell15VVVVVVVVV"));
            Paragraph paragraph16 = new Paragraph(new Text(""));
            Cell cell1 = new Cell().Add(new Paragraph().Add("Cell1")).SetBorder(border);
            Cell cell2 = new Cell().Add(new Paragraph().Add("Cell2")).SetBorder(border);
            Cell cell3 = new Cell().Add(new Paragraph().Add("Cell3")).SetBorder(border);
            Cell cell4 = new Cell().Add(new Paragraph().Add("Cell4")).SetBorder(border);
            Cell cell5 = new Cell().Add(paragraph5.SetFixedLeading(8)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell6 = new Cell().Add(paragraph6.SetFixedLeading(0)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell7 = new Cell().Add(paragraph7.SetFixedLeading(8)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell8 = new Cell().Add(paragraph8.SetFixedLeading(-4)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell9 = new Cell().Add(new Paragraph().Add("Cell9")).SetBorder(border);
            Cell cell10 = new Cell().Add(new Paragraph().Add("Cell10")).SetBorder(border);
            Cell cell11 = new Cell().Add(new Paragraph().Add("Cell11")).SetBorder(border);
            Cell cell12 = new Cell().Add(new Paragraph().Add("Cell12")).SetBorder(border);
            Cell cell13 = new Cell().Add(paragraph13.SetMultipliedLeading(-1)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell14 = new Cell().Add(paragraph14.SetMultipliedLeading(4)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell15 = new Cell().Add(paragraph15.SetMultipliedLeading(8)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell16 = new Cell().Add(paragraph16.SetMultipliedLeading(-4)).SetBorder(border).SetBackgroundColor(ColorConstants
                .LIGHT_GRAY);
            Cell cell17 = new Cell().Add(new Paragraph().Add("Cell17")).SetBorder(border);
            Cell cell18 = new Cell().Add(new Paragraph().Add("Cell18")).SetBorder(border);
            Cell cell19 = new Cell().Add(new Paragraph().Add("Cell19")).SetBorder(border);
            Cell cell20 = new Cell().Add(new Paragraph().Add("Cell20")).SetBorder(border);
            table.AddCell(cell1);
            table.AddCell(cell2);
            table.AddCell(cell3);
            table.AddCell(cell4);
            table.AddCell(cell5);
            table.AddCell(cell6);
            table.AddCell(cell7);
            table.AddCell(cell8);
            table.AddCell(cell9);
            table.AddCell(cell10);
            table.AddCell(cell11);
            table.AddCell(cell12);
            table.AddCell(cell13);
            table.AddCell(cell14);
            table.AddCell(cell15);
            table.AddCell(cell16);
            table.AddCell(cell17);
            table.AddCell(cell18);
            table.AddCell(cell19);
            table.AddCell(cell20);
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableMinMaxWidthTest06() {
            String testName = "tableMinMaxWidthTest06.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2));
            table.SetBorder(new SolidBorder(ColorConstants.RED, 1));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20);
            table.SetVerticalBorderSpacing(20);
            table.AddCell(new Cell().Add(new Paragraph("The cell with width 50. Number 1").SetWidth(50)));
            table.AddCell(new Cell().Add(new Paragraph("The cell with width 50. Number 1").SetWidth(50)));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void SplitTableMinMaxWidthTest01() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document doc = new Document(pdfDoc);
            Table table = new Table(2);
            for (int i = 0; i < 26; i++) {
                table.AddCell(new Cell().Add(new Paragraph("abba a")));
                table.AddCell(new Cell().Add(new Paragraph("ab ab ab")));
            }
            // not enough to place even if min-width approach is used
            float areaWidth = 20;
            LayoutResult result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(
                new LayoutArea(1, new Rectangle(areaWidth, 100))));
            TableRenderer overflowRenderer = (TableRenderer)result.GetOverflowRenderer();
            MinMaxWidth minMaxWidth = overflowRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(result.GetOccupiedArea().GetBBox().GetWidth(), minMaxWidth.GetMaxWidth(), 
                0.0001);
            NUnit.Framework.Assert.AreEqual(minMaxWidth.GetMaxWidth(), minMaxWidth.GetMinWidth(), 0.0001);
            // not enough to place using max-width approach, but more than required for min-width approach
            areaWidth = 70;
            result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(new LayoutArea
                (1, new Rectangle(areaWidth, 100))));
            overflowRenderer = (TableRenderer)result.GetOverflowRenderer();
            minMaxWidth = overflowRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(result.GetOccupiedArea().GetBBox().GetWidth(), minMaxWidth.GetMaxWidth(), 
                0.0001);
            NUnit.Framework.Assert.AreEqual(minMaxWidth.GetMaxWidth(), minMaxWidth.GetMinWidth(), 0.0001);
            // enough to place using max-width approach
            areaWidth = 400f;
            result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(new LayoutArea
                (1, new Rectangle(areaWidth, 100))));
            overflowRenderer = (TableRenderer)result.GetOverflowRenderer();
            minMaxWidth = overflowRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(result.GetOccupiedArea().GetBBox().GetWidth(), minMaxWidth.GetMaxWidth(), 
                0.0001);
            NUnit.Framework.Assert.AreEqual(minMaxWidth.GetMaxWidth(), minMaxWidth.GetMinWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void MarginPaddingTest01() {
            String testName = "marginPaddingTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("Body Cell 1")).SetBorder(new SolidBorder(30)));
            table.AddCell(new Cell().Add(new Paragraph("Body Cell 2")).SetBorder(new SolidBorder(30)));
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer Cell 1")).SetBorder(new SolidBorder(70)));
            table.AddFooterCell(new Cell().Add(new Paragraph("Footer Cell 2")).SetBorder(new SolidBorder(70)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header Cell 1")).SetBorder(new SolidBorder(70)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Header Cell 2")).SetBorder(new SolidBorder(70)));
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetMargin(20);
            table.SetPadding(20);
            table.SetBorder(new SolidBorder(ColorConstants.RED, 10));
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                ("Hello"))).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SpacingTest01() {
            String testName = "spacingTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            int n = 4;
            Table table = new Table(UnitValue.CreatePercentArray(n)).UseAllAvailableWidth();
            for (int j = 0; j < n; j++) {
                for (int i = 0; i < n; i++) {
                    table.AddCell(new Cell().Add(new Paragraph(j + "Body Cell" + i)));
                    table.AddFooterCell(new Cell().Add(new Paragraph(j + "Footer Cell 1")));
                    table.AddHeaderCell(new Cell().Add(new Paragraph(j + "Header Cell 1")));
                }
            }
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            table.SetHorizontalBorderSpacing(20f);
            table.SetVerticalBorderSpacing(20f);
            table.SetBorder(new SolidBorder(ColorConstants.RED, 10));
            doc.Add(table);
            doc.Add(new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().AddCell(new Cell().Add(new Paragraph
                ("Hello"))).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TaggedTableWithCaptionTest01() {
            String testName = "taggedTableWithCaptionTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            pdfDoc.SetTagged();
            Document doc = new Document(pdfDoc);
            Table table = CreateTestTable(2, 10, 2, 2, (UnitValue)null, BorderCollapsePropertyValue.SEPARATE, new Style
                ().SetBorder(new SolidBorder(ColorConstants.RED, 10)));
            Paragraph pCaption = new Paragraph("I'm a caption!").SetBackgroundColor(ColorConstants.CYAN);
            table.SetCaption(new Div().Add(pCaption));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WideCaptionTest01() {
            String testName = "wideCaptionTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = CreateTestTable(2, 3, 3, 3, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new Style
                ().SetBorder(new SolidBorder(ColorConstants.RED, 10)));
            // no caption
            AddTable(table, true, true, doc);
            // the caption as a paragraph
            Paragraph pCaption = new Paragraph("I'm a caption!").SetBackgroundColor(ColorConstants.CYAN);
            table.SetCaption(new Div().Add(pCaption).SetWidth(500));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            // the caption as a div
            Div divCaption = new Div().Add(pCaption).Add(pCaption).Add(pCaption).SetBackgroundColor(ColorConstants.MAGENTA
                ).SetWidth(500);
            table.SetCaption(divCaption).SetWidth(500);
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            // the caption as a table
            Table tableCaption = CreateTestTable(1, 1, 0, 0, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new 
                Style().SetBorder(new SolidBorder(ColorConstants.BLUE, 10)).SetBackgroundColor(ColorConstants.YELLOW))
                .SetWidth(500);
            table.SetCaption(new Div().Add(tableCaption).SetWidth(500));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SplitTableWithCaptionTest01() {
            String testName = "splitTableWithCaptionTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = CreateTestTable(2, 30, 3, 3, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new Style
                ().SetBorder(new SolidBorder(ColorConstants.RED, 10)));
            table.GetFooter().SetBorder(new SolidBorder(ColorConstants.ORANGE, 20));
            table.GetHeader().SetBorder(new SolidBorder(ColorConstants.ORANGE, 20));
            Paragraph pCaption = new Paragraph("I'm a caption!").SetBackgroundColor(ColorConstants.CYAN);
            // no caption
            AddTable(table, true, true, doc);
            // top caption
            table.SetCaption(new Div().Add(pCaption));
            AddTable(table, true, true, doc);
            // bottom caption
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CaptionedTableOfOnePageWithCollapsedBordersTest01() {
            String testName = "captionedTableOfOnePageWithCollapsedBordersTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = CreateTestTable(2, 10, 2, 2, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new Style
                ().SetBorder(new SolidBorder(ColorConstants.RED, 10)));
            table.GetHeader().SetBorder(new SolidBorder(ColorConstants.ORANGE, 5f));
            table.GetFooter().SetBorder(new SolidBorder(ColorConstants.ORANGE, 5f));
            // no caption
            AddTable(table, true, true, doc);
            // the caption as a paragraph
            Paragraph pCaption = new Paragraph("I'm a caption!").SetBackgroundColor(ColorConstants.CYAN);
            table.SetCaption(new Div().Add(pCaption));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            // the caption as a div
            Div divCaption = new Div().Add(pCaption).Add(pCaption).Add(pCaption).SetBackgroundColor(ColorConstants.MAGENTA
                );
            table.SetCaption(divCaption);
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            // the caption as a table
            Table tableCaption = CreateTestTable(1, 1, 0, 0, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new 
                Style().SetBorder(new SolidBorder(ColorConstants.BLUE, 10)).SetBackgroundColor(ColorConstants.YELLOW));
            table.SetCaption(new Div().Add(tableCaption));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithDifferentStylesOfCollapsedBordersTest() {
            String testName = "tableWithDifferentStylesOfCollapsedBordersTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = CreateTestTable(2, 10, 2, 2, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new Style
                ().SetBorder(new DashedBorder(ColorConstants.RED, 10)));
            table.GetHeader().SetBorder(new DottedBorder(ColorConstants.ORANGE, 5f));
            table.GetFooter().SetBorder(new RoundDotsBorder(ColorConstants.ORANGE, 5f));
            // no caption
            AddTable(table, true, true, doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CaptionedTableOfOnePageWithSeparatedBordersTest01() {
            String testName = "captionedTableOfOnePageWithSeparatedBordersTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = CreateTestTable(2, 10, 2, 2, (UnitValue)null, BorderCollapsePropertyValue.SEPARATE, new Style
                ().SetBorder(new SolidBorder(ColorConstants.RED, 10)));
            // no caption
            AddTable(table, true, true, doc);
            // the caption as a paragraph
            Paragraph pCaption = new Paragraph("I'm a caption!").SetBackgroundColor(ColorConstants.CYAN);
            table.SetCaption(new Div().Add(pCaption));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            // the caption as a div
            Div divCaption = new Div().Add(pCaption).Add(pCaption).Add(pCaption).SetBackgroundColor(ColorConstants.MAGENTA
                );
            table.SetCaption(divCaption);
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            // the caption as a table
            Table tableCaption = CreateTestTable(1, 1, 0, 0, (UnitValue)null, BorderCollapsePropertyValue.COLLAPSE, new 
                Style().SetBorder(new SolidBorder(ColorConstants.BLUE, 10)).SetBackgroundColor(ColorConstants.YELLOW));
            table.SetCaption(new Div().Add(tableCaption));
            AddTable(table, true, true, doc);
            table.GetCaption().SetProperty(Property.CAPTION_SIDE, CaptionSide.BOTTOM);
            AddTable(table, true, true, doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        private void AddTable(Table table, bool addParagraphBefore, bool addParagraphAfter, Document doc) {
            if (addParagraphBefore) {
                doc.Add(new Paragraph("I'm the paragraph placed before the table. I'm green and have no border.").SetBackgroundColor
                    (ColorConstants.GREEN));
            }
            doc.Add(table);
            if (addParagraphAfter) {
                doc.Add(new Paragraph("I'm the paragraph placed after the table. I'm green and have no border.").SetBackgroundColor
                    (ColorConstants.GREEN));
            }
            doc.Add(new AreaBreak());
        }

        private Table CreateTestTable(int colNum, int bodyRowNum, int headerRowNum, int footerRowNum, UnitValue width
            , BorderCollapsePropertyValue collapseValue, Style style) {
            Table table = new Table(colNum);
            if (null != width) {
                table.SetWidth(width);
            }
            if (null != style) {
                table.AddStyle(style);
            }
            if (BorderCollapsePropertyValue.SEPARATE.Equals(collapseValue)) {
                table.SetBorderCollapse(collapseValue);
            }
            for (int i = 0; i < bodyRowNum; i++) {
                for (int j = 0; j < colNum; j++) {
                    table.AddCell("Body Cell row " + i + " col " + j);
                }
            }
            for (int i = 0; i < headerRowNum; i++) {
                for (int j = 0; j < colNum; j++) {
                    table.AddHeaderCell("Header Cell row " + i + " col " + j);
                }
            }
            for (int i = 0; i < footerRowNum; i++) {
                for (int j = 0; j < colNum; j++) {
                    table.AddFooterCell("Footer Cell row " + i + " col " + j);
                }
            }
            return table;
        }

        [NUnit.Framework.Test]
        public virtual void SkipLastFooterAndProcessBigRowspanTest01() {
            String testName = "skipLastFooterAndProcessBigRowspanTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 140));
            Table table = new Table(2);
            table.SetSkipLastFooter(true);
            table.AddFooterCell(new Cell(1, 2).Add(new Paragraph("Footer")));
            table.AddCell(new Cell(3, 1).Add(new Paragraph(JavaUtil.IntegerToString(1))));
            for (int z = 0; z < 3; z++) {
                table.AddCell(new Cell().Add(new Paragraph(JavaUtil.IntegerToString(z))));
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SkipLastFooterAndProcessBigRowspanTest02() {
            String testName = "skipLastFooterAndProcessBigRowspanTest02.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            int numRows = 3;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(numRows);
            table.SetSkipLastFooter(true);
            table.AddHeaderCell(new Cell(1, numRows).Add(new Paragraph("Header")));
            table.AddFooterCell(new Cell(1, numRows).Add(new Paragraph("Footer")));
            for (int rows = 0; rows < 11; rows++) {
                table.AddCell(new Cell(numRows, 1).Add(new Paragraph("Filled Cell: " + JavaUtil.IntegerToString(rows) + ", 0"
                    )));
                //Number of cells to complete the table rows filling up to the cell of colSpan
                int numFillerCells = (numRows - 1) * numRows;
                for (int cells = 0; cells < numFillerCells; cells++) {
                    table.AddCell(new Cell().Add(new Paragraph("Filled Cell: " + JavaUtil.IntegerToString(rows) + ", " + JavaUtil.IntegerToString
                        (cells))));
                }
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SkipLastFooterOnShortPageTest01() {
            String testName = "skipLastFooterOnShortPageTest01.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(595, 120));
            Table table = new Table(2);
            table.SetSkipLastFooter(true);
            table.AddFooterCell(new Cell(1, 2).Add(new Paragraph("Footer")));
            for (int z = 0; z < 2; z++) {
                for (int i = 0; i < 2; i++) {
                    table.AddCell(new Cell().Add(new Paragraph(JavaUtil.IntegerToString(z))));
                }
            }
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FirstRowPartiallyFitWideBottomBorderTest() {
            String testName = "firstRowPartiallyFitWideBottomBorderTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4);
            Table table = new Table(1);
            table.SetBorderBottom(new SolidBorder(ColorConstants.RED, 250));
            Cell notFitCell = new Cell();
            notFitCell.Add(new Paragraph("Some text which should be big enough."));
            notFitCell.SetFontSize(100);
            table.AddCell(notFitCell);
            table.AddCell("row 2 col 1");
            table.AddCell("row 2 col 2");
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CollapseWithNextRowWiderThanWithTableBorderTest() {
            String testName = "collapseWithNextRowWiderThanWithTableBorderTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4);
            Table table = new Table(1);
            Cell cell1 = new Cell();
            cell1.Add(new Paragraph("Usual bottom border"));
            cell1.SetHeight(300);
            table.AddCell(cell1);
            Cell cell2 = new Cell();
            cell2.Add(new Paragraph("Top border: 600pt"));
            cell2.SetBorderTop(new SolidBorder(600));
            table.AddCell(cell2);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TableBottomBorderWideTest() {
            String testName = "tableBottomBorderWideTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(1).SetBorderBottom(new SolidBorder(ColorConstants.RED, 500)).AddCell(new Cell().Add
                (new Paragraph(TEXT_CONTENT + TEXT_CONTENT + TEXT_CONTENT + TEXT_CONTENT))).AddCell(new Cell().Add(new 
                Paragraph("Hello World")));
            doc.Add(table);
            doc.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            doc.Add(table);
            AddTableBelowToCheckThatOccupiedAreaIsCorrect(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CellWithBigRowspanCompletedRowTooTest() {
            String testName = "cellWithBigRowspanCompletedRowTooTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetHeight(700);
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(1));
            table.SetHorizontalBorderSpacing(5);
            table.SetVerticalBorderSpacing(5);
            table.AddCell(new Cell(7, 1).Add(new Paragraph("Rowspan 7")).SetBackgroundColor(ColorConstants.RED));
            for (int i = 0; i < 7; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Rowspan 1")));
            }
            // test separated borders when j == 0 and collapsed borders when j == 1
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            for (int j = 0; j < 2; j++) {
                doc.Add(div);
                doc.Add(table);
                if (0 == j) {
                    doc.Add(new AreaBreak());
                    table.SetBorderCollapse(BorderCollapsePropertyValue.COLLAPSE);
                }
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CellWithBigRowspanCompletedRowNotTest() {
            String testName = "cellWithBigRowspanCompletedRowNotTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Div div = new Div();
            div.SetHeight(700);
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(1));
            table.SetHorizontalBorderSpacing(5);
            table.SetVerticalBorderSpacing(5);
            table.AddCell(new Cell(7, 1).Add(new Paragraph("Rowspan 7")).SetBackgroundColor(ColorConstants.RED));
            table.AddCell(new Cell().Add(new Paragraph(TEXT_CONTENT)));
            for (int i = 0; i < 6; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Rowspan 1")));
            }
            // test separated borders when j == 0 and collapsed borders when j == 1
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            for (int j = 0; j < 2; j++) {
                doc.Add(div);
                doc.Add(table);
                if (0 == j) {
                    doc.Add(new AreaBreak());
                    table.SetBorderCollapse(BorderCollapsePropertyValue.COLLAPSE);
                }
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void InheritHeaderPropsWhileMinMaxWidthCalculationsTest() {
            String filename = "inheritHeaderPropsWhileMinMaxWidthCalculations.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + filename));
            Document document = new Document(pdf);
            Paragraph p = new Paragraph("Some text is placed at the beginning" + " of the page, so that page isn't being empty."
                );
            document.Add(p);
            Table table = new Table(new float[1]);
            // The header's text is longer than the body's text, hence the width
            // of the table will be calculated by the header.
            table.AddHeaderCell(new Cell().Add(new Paragraph("Hello")));
            table.AddCell(new Cell().Add(new Paragraph("He")));
            // If this property is not inherited while calculating min/max widths,
            // then while layouting header will request more space than the layout box's width
            table.GetHeader().SetBold();
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void InfiniteLoopOnUnfitCellAndBigRowspanTest() {
            String testName = "infiniteLoopOnUnfitCellAndBigRowspanTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Table table = new Table(38);
            table.UseAllAvailableWidth();
            table.SetFixedLayout();
            Cell cellNum1 = new Cell(1, 1);
            table.AddCell(cellNum1);
            Cell cellNum2 = new Cell(2, 2);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "itext.png"
                ));
            cellNum2.Add(img);
            table.AddCell(cellNum2);
            Cell cellNum3 = new Cell(2, 36);
            cellNum3.Add(new Paragraph("text"));
            table.AddCell(cellNum3);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void FirstRowNotFitBigRowspanTest() {
            String testName = "firstRowNotFitBigRowspanTest.pdf";
            String outFileName = destinationFolder + testName;
            String cmpFileName = sourceFolder + "cmp_" + testName;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, PageSize.A4);
            Table table = new Table(4);
            table.AddCell("row 1 col 1");
            Cell notFitCell = new Cell(2, 1);
            notFitCell.Add(new Paragraph("row 1-2 col 2"));
            notFitCell.SetFontSize(1000);
            table.AddCell(notFitCell);
            Cell fitCell = new Cell(2, 2);
            fitCell.Add(new Paragraph("row 1-2 col 3-4"));
            table.AddCell(fitCell);
            table.AddCell("row 2 col 1");
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , testName + "_diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowSpanTooFarFullTest() {
            String filename = "bigRowSpanTooFarFullTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + filename));
            Document document = new Document(pdf);
            Table table = new Table(2);
            int bigRowSpan = 5;
            table.AddCell(new Cell(bigRowSpan, 1).Add(new Paragraph("row span " + bigRowSpan)).SetBackgroundColor(ColorConstants
                .RED));
            for (int i = 0; i < bigRowSpan; i++) {
                table.AddCell(new Cell().Add(new Paragraph(JavaUtil.IntegerToString(i))).SetHeight(375).SetBackgroundColor
                    (ColorConstants.BLUE));
            }
            document.Add(table);
            document.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void BigRowSpanTooFarPartialTest() {
            String filename = "bigRowSpanTooFarPartialTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + filename));
            Document document = new Document(pdf);
            Table table = new Table(2);
            int bigRowSpan = 5;
            table.AddCell(new Cell(bigRowSpan, 1).Add(new Paragraph("row span " + bigRowSpan)).SetHeight(800).SetBackgroundColor
                (ColorConstants.RED));
            for (int i = 0; i < bigRowSpan; i++) {
                table.AddCell(new Cell().Add(new Paragraph(JavaUtil.IntegerToString(i))).SetHeight(375).SetBackgroundColor
                    (ColorConstants.BLUE));
            }
            document.Add(table);
            document.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void BigRowSpanTooFarNothingTest() {
            String filename = "bigRowSpanTooFarNothingTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + filename));
            Document document = new Document(pdf);
            Table table = new Table(2);
            int bigRowSpan = 5;
            table.AddCell(new Cell(bigRowSpan, 1).Add(new Paragraph("row span " + bigRowSpan)).SetHeight(800).SetKeepTogether
                (true).SetBackgroundColor(ColorConstants.RED));
            for (int i = 0; i < bigRowSpan; i++) {
                table.AddCell(new Cell().Add(new Paragraph(JavaUtil.IntegerToString(i))).SetHeight(375).SetBackgroundColor
                    (ColorConstants.BLUE));
            }
            document.Add(table);
            document.Add(new AreaBreak());
            table.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void SetWidthShouldBeRespectedTest() {
            // TODO DEVSIX-5916 The first cell's width is the same as the second one's, however, it's not respected
            String fileName = "setWidthShouldBeRespectedTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + fileName));
            Document doc = new Document(pdfDocument, new PageSize(842, 1400));
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(ColorConstants.GREEN, 90f));
            Cell cell;
            cell = new Cell().Add(new Paragraph("100pt"));
            cell.SetBorder(new SolidBorder(ColorConstants.BLUE, 20f));
            cell.SetWidth(100).SetMargin(0).SetPadding(0);
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("100pt"));
            cell.SetBorder(new SolidBorder(ColorConstants.RED, 120f));
            cell.SetWidth(100).SetMargin(0).SetPadding(0);
            table.AddCell(cell);
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder));
        }

        //creates 2 empty lines, where 2 is random number
        private static void StartSeveralEmptyRows(Table table) {
            table.StartNewRow();
            table.StartNewRow();
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void PreciseFittingBoldSimulatedTextInCellsTest() {
            String fileName = "preciseFittingBoldSimulatedTextInCells.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + fileName))) {
                using (Document doc = new Document(pdfDocument)) {
                    int numberOfColumns = 9;
                    Table table = new Table(UnitValue.CreatePercentArray(numberOfColumns));
                    table.UseAllAvailableWidth();
                    table.SetFixedLayout();
                    for (int i = 0; i < numberOfColumns; i++) {
                        table.AddCell(new Cell().Add(new Paragraph("Description").SetBold()));
                    }
                    doc.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void TableRelayoutTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document doc = new Document(pdfDoc)) {
                    float width = 142f;
                    Table table = new Table(1);
                    table.SetWidth(width);
                    table.SetFixedLayout();
                    Cell cell = new Cell();
                    cell.SetWidth(width);
                    cell.Add(new Paragraph("Testing, FinancialProfessional Associate adasdasdasdasada.gmail.com"));
                    table.AddCell(cell);
                    LayoutResult result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(
                        new LayoutArea(1, new Rectangle(0, 0, 10000, 10000.0F))));
                    Rectangle tableRect = result.GetOccupiedArea().GetBBox();
                    result = table.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(new LayoutArea
                        (1, new Rectangle(0, 0, 10000, 10000.0F))));
                    Rectangle tableRectRelayout = result.GetOccupiedArea().GetBBox();
                    NUnit.Framework.Assert.IsTrue(tableRect.EqualsWithEpsilon(tableRectRelayout));
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, LogLevel = LogLevelConstants.WARN)]
        public virtual void InfiniteLoopKeepTogetherTest() {
            String fileName = "infiniteLoopKeepTogether.pdf";
            float fontSize = 8;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + fileName))) {
                using (Document doc = new Document(pdfDoc)) {
                    doc.SetMargins(138, 20, 75, 20);
                    Table table = new Table(5);
                    table.SetKeepTogether(true);
                    for (int i = 0; i < 37; i++) {
                        table.AddCell(new Cell(1, 5).Add(new Paragraph(new Text("Cell"))).SetFontSize(fontSize));
                        table.StartNewRow();
                    }
                    Table commentsTable = new Table(1);
                    Cell commentsCell = new Cell().Add(new Paragraph(new Text("First line\nSecond line")));
                    commentsTable.AddCell(commentsCell);
                    Cell outerCommentsCell = new Cell(1, 5).SetFontSize(fontSize);
                    outerCommentsCell.Add(commentsTable);
                    table.AddCell(outerCommentsCell);
                    doc.Add(table);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder));
        }

        private class RotatedDocumentRenderer : DocumentRenderer {
            private readonly PdfDocument pdfDoc;

            public RotatedDocumentRenderer(Document doc, PdfDocument pdfDoc)
                : base(doc) {
                this.pdfDoc = pdfDoc;
            }

            protected internal override PageSize AddNewPage(PageSize customPageSize) {
                int currentNumberOfPages = document.GetPdfDocument().GetNumberOfPages();
                PageSize pageSize = currentNumberOfPages % 2 == 1 ? PageSize.A4.Rotate() : PageSize.A4;
                pdfDoc.AddNewPage(pageSize);
                return pageSize;
            }
        }

        internal class CustomRenderer : TableRenderer {
            public CustomRenderer(Table modelElement, Table.RowRange rowRange)
                : base(modelElement, rowRange) {
            }
        }
    }
}
