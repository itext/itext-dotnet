using System;
using System.Text;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class AutoTaggingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AutoTaggingTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AutoTaggingTest/";

        public const String imageName = "Desert.jpg";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TextInParagraphTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "textInParagraphTest01.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph p = CreateParagraph1();
            document.Add(p);
            for (int i = 0; i < 26; ++i) {
                document.Add(CreateParagraph2());
            }
            document.Close();
            CompareResult("textInParagraphTest01.pdf", "cmp_textInParagraphTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "imageTest01.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(sourceFolder + imageName));
            document.Add(image);
            document.Close();
            CompareResult("imageTest01.pdf", "cmp_imageTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DivTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "divTest01.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div = new Div();
            div.Add(CreateParagraph1());
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + imageName
                ));
            image.SetAutoScale(true);
            div.Add(image);
            div.Add(CreateParagraph2());
            div.Add(image);
            div.Add(CreateParagraph2());
            document.Add(div);
            document.Close();
            CompareResult("divTest01.pdf", "cmp_divTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TableTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "tableTest01.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Table table = new Table(3);
            table.AddCell(CreateParagraph1());
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + imageName
                ));
            image.SetAutoScale(true);
            table.AddCell(image);
            table.AddCell(CreateParagraph2());
            table.AddCell(image);
            table.AddCell(new Paragraph("abcdefghijklmnopqrstuvwxyz").SetFontColor(Color.GREEN));
            table.AddCell("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                 + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                 + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                 + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                );
            document.Add(table);
            document.Close();
            CompareResult("tableTest01.pdf", "cmp_tableTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TableTest02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "tableTest02.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Table table = new Table(3);
            for (int i = 0; i < 5; ++i) {
                table.AddCell(CreateParagraph2());
            }
            table.AddCell("little text");
            document.Add(table);
            document.Close();
            CompareResult("tableTest02.pdf", "cmp_tableTest02.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TableTest03() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "tableTest03.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Table table = new Table(3);
            Cell cell = new Cell(1, 3).Add(new Paragraph("full-width header"));
            cell.SetRole(PdfName.TH);
            table.AddHeaderCell(cell);
            for (int i = 0; i < 3; ++i) {
                cell = new Cell().Add(new Paragraph("header " + i));
                cell.SetRole(PdfName.TH);
                table.AddHeaderCell(cell);
            }
            for (int i = 0; i < 3; ++i) {
                table.AddFooterCell("footer " + i);
            }
            cell = new Cell(1, 3).Add(new Paragraph("full-width paragraph"));
            table.AddCell(cell);
            for (int i = 0; i < 5; ++i) {
                table.AddCell(CreateParagraph2());
            }
            table.AddCell(new Paragraph("little text"));
            document.Add(table);
            document.Close();
            CompareResult("tableTest03.pdf", "cmp_tableTest03.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TableTest04() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "tableTest04.pdf"));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            Table table = new Table(5, true);
            doc.Add(table);
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 4; j++) {
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
            CompareResult("tableTest04.pdf", "cmp_tableTest04.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TableTest05() {
            String outFileName = destinationFolder + "tableTest05.pdf";
            String cmpFileName = sourceFolder + "cmp_tableTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "tableTest05.pdf"));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            Table table = new Table(5, true);
            doc.Add(table);
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
            table.AddHeaderCell(cell);
            for (int i = 0; i < 5; ++i) {
                table.AddHeaderCell(new Cell().Add("Header " + (i + 1)));
            }
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
            CompareResult("tableTest05.pdf", "cmp_tableTest05.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void TableTest06() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "tableTest06.pdf"));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
            String shortTextContent = "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            String middleTextContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.";
            Table table = new Table(new float[] { 130, 130, 260 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1\n" 
                + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 1, 2\n" + shortTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 1, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph
                ("cell 2, 1\n" + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 2, 2\n" + shortTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 2, 3\n" + middleTextContent))).AddCell(new Cell(3, 2).Add
                (new Paragraph("cell 3:2, 1:3\n" + textContent + textContent))).AddCell(new Cell().Add(new Paragraph("cell 3, 3\n"
                 + textContent))).AddCell(new Cell().Add(new Paragraph("cell 4, 3\n" + textContent))).AddCell(new Cell
                ().Add(new Paragraph("cell 5, 3\n" + textContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 1\n"
                 + shortTextContent))).AddCell(new Cell().Add(new Paragraph("cell 6, 2\n" + shortTextContent))).AddCell
                (new Cell().Add(new Paragraph("cell 6, 3\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph
                ("cell 7, 1\n" + middleTextContent))).AddCell(new Cell().Add(new Paragraph("cell 7, 2\n" + middleTextContent
                ))).AddCell(new Cell().Add(new Paragraph("cell 7, 3\n" + middleTextContent)));
            doc.Add(table);
            doc.Close();
            CompareResult("tableTest06.pdf", "cmp_tableTest06.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void ListTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "listTest01.pdf"));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            List list = new List(ListNumberingType.DECIMAL);
            list.Add("item 1");
            list.Add("item 2");
            list.Add("item 3");
            doc.Add(list);
            doc.Close();
            CompareResult("listTest01.pdf", "cmp_listTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void ArtifactTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "artifactTest01.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            String watermarkText = "WATERMARK";
            Paragraph watermark = new Paragraph(watermarkText);
            watermark.SetFontColor(new DeviceGray(0.75f)).SetFontSize(72);
            document.ShowTextAligned(watermark, PageSize.A4.GetWidth() / 2, PageSize.A4.GetHeight() / 2, 1, TextAlignment
                .CENTER, VerticalAlignment.MIDDLE, (float)(Math.PI / 4));
            String textContent = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\n"
                 + "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\n" + "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin pharetra nonummy pede. Mauris et orci.\n";
            document.Add(new Paragraph(textContent + textContent + textContent));
            document.Add(new Paragraph(textContent + textContent + textContent));
            document.Close();
            CompareResult("artifactTest01.pdf", "cmp_artifactTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void ArtifactTest02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "artifactTest02.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Hello world"));
            Table table = new Table(5);
            for (int i = 0; i < 25; ++i) {
                table.AddCell(i.ToString());
            }
            table.SetRole(PdfName.Artifact);
            document.Add(table);
            document.Close();
            CompareResult("artifactTest02.pdf", "cmp_artifactTest02.pdf");
        }

        /// <summary>
        /// Document generation and result is the same in this test as in the textInParagraphTest01, except the partial flushing of
        /// tag structure.
        /// </summary>
        /// <remarks>
        /// Document generation and result is the same in this test as in the textInParagraphTest01, except the partial flushing of
        /// tag structure. So you can check the result by comparing resultant document with the one in textInParagraphTest01.
        /// </remarks>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushingTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "flushingTest01.pdf"));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph p = CreateParagraph1();
            document.Add(p);
            int pageToFlush = 1;
            for (int i = 0; i < 26; ++i) {
                if (i % 6 == 5) {
                    pdfDocument.GetPage(pageToFlush++).Flush();
                }
                document.Add(CreateParagraph2());
            }
            document.Close();
            CompareResult("flushingTest01.pdf", "cmp_flushingTest01.pdf");
        }

        /// <summary>
        /// Document generation and result is the same in this test as in the tableTest05, except the partial flushing of
        /// tag structure.
        /// </summary>
        /// <remarks>
        /// Document generation and result is the same in this test as in the tableTest05, except the partial flushing of
        /// tag structure. So you can check the result by comparing resultant document with the one in tableTest05.
        /// </remarks>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushingTest02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "flushingTest02.pdf"));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            Table table = new Table(5, true);
            doc.Add(table);
            //        TODO solve header/footer problems with tagging. Currently, partial flushing when header/footer is used leads to crash.
            Cell cell = new Cell(1, 5).Add(new Paragraph("Table XYZ (Continued)"));
            table.AddHeaderCell(cell);
            for (int i = 0; i < 5; ++i) {
                table.AddHeaderCell(new Cell().Add("Header " + (i + 1)));
            }
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
            CompareResult("flushingTest02.pdf", "cmp_flushingTest02.pdf");
        }

        /// <summary>
        /// Document generation and result is the same in this test as in the tableTest04, except the partial flushing of
        /// tag structure.
        /// </summary>
        /// <remarks>
        /// Document generation and result is the same in this test as in the tableTest04, except the partial flushing of
        /// tag structure. So you can check the result by comparing resultant document with the one in tableTest04.
        /// </remarks>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushingTest03() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "flushingTest03.pdf"));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            Table table = new Table(5, true);
            doc.Add(table);
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 4; j++) {
                    table.AddCell(new Cell().Add(new Paragraph(String.Format("Cell {0}, {1}", i + 1, j + 1))));
                }
                if (i % 10 == 0) {
                    table.Flush();
                    pdfDocument.GetTagStructureContext().FlushPageTags(pdfDocument.GetPage(1));
                    // This is a deliberate additional flush.
                    table.Flush();
                }
            }
            table.Complete();
            doc.Add(new Table(1).SetBorder(new SolidBorder(Color.ORANGE, 2)).AddCell("Is my occupied area correct?"));
            doc.Close();
            CompareResult("flushingTest03.pdf", "cmp_tableTest04.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        private Paragraph CreateParagraph1() {
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLD);
            Paragraph p = new Paragraph().Add("text chunk. ").Add("explicitly added separate text chunk");
            Text id = new Text("text chunk with specific font").SetFont(font).SetFontSize(8).SetTextRise(6);
            p.Add(id);
            return p;
        }

        private Paragraph CreateParagraph2() {
            Paragraph p;
            String alphabet = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder longTextBuilder = new StringBuilder();
            for (int i = 0; i < 26; ++i) {
                longTextBuilder.Append(alphabet);
            }
            String longText = longTextBuilder.ToString();
            p = new Paragraph(longText);
            return p;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        private void CompareResult(String outFileName, String cmpFileName) {
            CompareTool compareTool = new CompareTool();
            String outPdf = destinationFolder + outFileName;
            String cmpPdf = sourceFolder + cmpFileName;
            String contentDifferences = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff");
            String taggedStructureDifferences = compareTool.CompareTagStructures(outPdf, cmpPdf);
            String errorMessage = "";
            errorMessage += taggedStructureDifferences == null ? "" : taggedStructureDifferences + "\n";
            errorMessage += contentDifferences == null ? "" : contentDifferences;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
