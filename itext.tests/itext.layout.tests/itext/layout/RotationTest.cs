using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class RotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/RotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/RotationTest/";

        public const String cmpPrefix = "cmp_";

        private const String para1Text = "The first published account of what would evolve into the Mafia in the United States came in the spring of 1869. "
             + "The New Orleans Times reported that the city's Second District had become overrun by \"well-known and notorious Sicilian murderers, "
             + "counterfeiters and burglars, who, in the last month, have formed a sort of general co-partnership or stock company for the plunder "
             + "and disturbance of the city.\" Emigration from southern Italy to the Americas was primarily to Brazil and Argentina, and New Orleans "
             + "had a heavy volume of port traffic to and from both locales.";

        private const String para2Text = "Mafia groups in the United States first became influential in the New York City area, gradually progressing from small neighborhood"
             + " operations in Italian ghettos to citywide and eventually national organizations. The Black Hand was a name given to an extortion method used "
             + "in Italian neighborhoods at the turn of the 20th century. It has been sometimes mistaken for the Mafia itself, which it is not. Although the Black"
             + " Hand was a criminal society, there were many small Black Hand gangs.";

        private const String para3Text = "From the 1890s to the 1900s (decade) in New York City, the Sicilian Mafia developed into the Five Points Gang and were very powerful in the"
             + " Little Italy of the Lower East Side. They were often in conflict with the Jewish Eastmans of the same area.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedTextRotationTest01() {
            String outFileName = destinationFolder + "fixedTextRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedTextRotationTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            SolidBorder border = new SolidBorder(0.5f);
            int x1 = 350;
            int y1 = 600;
            int width1 = 100;
            document.Add(new Paragraph("text to be rotatedg").SetMargin(0).SetRotationAngle((Math.PI / 6)).SetFixedPosition
                (x1, y1, width1).SetBorder(border));
            document.Add(new Paragraph("text to be rotatedg").SetMargin(0).SetFixedPosition(x1, y1, width1).SetBorder(
                border));
            String longText = "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo"
                 + "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooong text";
            int x2 = 50;
            int y2 = 300;
            int width2 = 450;
            document.Add(new Paragraph(longText).SetMargin(0).SetRotationAngle((Math.PI / 6)).SetFixedPosition(x2, y2, 
                width2));
            document.Add(new Paragraph(longText).SetMargin(0).SetFixedPosition(x2, y2, width2));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedTextRotationTest02() {
            String outFileName = destinationFolder + "fixedTextRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedTextRotationTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            String longText = "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo"
                 + "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooong text";
            document.Add(new Paragraph(longText).SetMargin(0).SetRotationAngle(-(Math.PI / 6)).SetFixedPosition(50, 50
                , 450));
            document.Add(new Paragraph(longText).SetMargin(0).SetFixedPosition(50, 50, 450));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedTextRotationTest03() {
            String outFileName = destinationFolder + "fixedTextRotationTest03.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedTextRotationTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            String simpleText = "text simple text";
            float x = 50;
            float y = 380;
            float width = 200;
            document.Add(new Paragraph(simpleText).SetMargin(0).SetRotationAngle((Math.PI / 2)).SetFixedPosition(x, y, 
                width));
            document.Add(new Paragraph(simpleText).SetMargin(0).SetFixedPosition(x, y, width));
            PdfCanvas canvas = new PdfCanvas(pdfDocument.GetFirstPage());
            DrawCross(canvas, x, y);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedTextRotationTest04() {
            String outFileName = destinationFolder + "fixedTextRotationTest04.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedTextRotationTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            String simpleText = "text simple text";
            float x = 50;
            float y = 380;
            float width = 100;
            document.Add(new Paragraph(simpleText).SetMargin(0).SetRotationAngle(-(Math.PI / 4)).SetBackgroundColor(Color
                .RED).SetFixedPosition(x, y, width));
            PdfCanvas canvas = new PdfCanvas(pdfDocument.GetFirstPage());
            DrawCross(canvas, x, y);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [NUnit.Framework.Test]
        public virtual void StaticTextRotationTest01() {
            String outFileName = destinationFolder + "staticTextRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "staticTextRotationTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph();
            for (int i = 0; i < 7; ++i) {
                p.Add(para2Text);
            }
            document.Add(p.SetRotationAngle((68 * Math.PI / 180)).SetBackgroundColor(Color.BLUE));
            document.Add(new Paragraph("text line text line text line text line text line text line text line text line text line text line text line"
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void StaticTextRotationTest02() {
            String outFileName = destinationFolder + "staticTextRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "staticTextRotationTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph(para2Text).SetRotationAngle((Math.PI / 12)));
            document.Add(new Paragraph(new Text(para2Text).SetBackgroundColor(Color.GREEN)).SetRotationAngle((-Math.PI
                 / 12)).SetBackgroundColor(Color.BLUE));
            document.Add(new Paragraph(para3Text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void StaticTextRotationTest03() {
            String outFileName = destinationFolder + "staticTextRotationTest03.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "staticTextRotationTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph(para2Text).SetRotationAngle((Math.PI / 6)).SetBackgroundColor(Color.RED));
            document.Add(new Paragraph(para2Text).SetRotationAngle((-Math.PI / 3)));
            document.Add(new Paragraph(para3Text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void StaticTextRotationTest04() {
            String outFileName = destinationFolder + "staticTextRotationTest04.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "staticTextRotationTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph("short text string").SetRotationAngle((Math.PI / 6)).SetBackgroundColor(Color.RED
                ));
            document.Add(new Paragraph(para3Text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SplitTextRotationTest01() {
            String outFileName = destinationFolder + "splitTextRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "splitTextRotationTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph(para1Text).SetRotationAngle((Math.PI / 4)));
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph(para2Text).SetRotationAngle((-Math.PI / 3)));
            document.Add(new Paragraph(para3Text));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void SplitTextRotationTest02() {
            String outFileName = destinationFolder + "splitTextRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "splitTextRotationTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph(para1Text));
            document.Add(new Paragraph(para1Text));
            String extremelyLongText = "";
            for (int i = 0; i < 300; ++i) {
                extremelyLongText += para2Text;
            }
            document.Add(new Paragraph(extremelyLongText).SetRotationAngle(Math.PI / 2));
            document.Add(new Paragraph(extremelyLongText).SetRotationAngle(Math.PI / 4));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [NUnit.Framework.Test]
        public virtual void RotationInfiniteLoopTest01() {
            String fileName = "rotationInfiniteLoopTest01.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetDefaultPageSize(PageSize.A5.Rotate());
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph(para1Text).SetRotationAngle((Math.PI / 2)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [NUnit.Framework.Test]
        public virtual void RotationInfiniteLoopTest02() {
            String fileName = "rotationInfiniteLoopTest02.pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetDefaultPageSize(PageSize.A5.Rotate());
            Document document = new Document(pdfDocument);
            document.Add(new List().Add(para1Text).SetRotationAngle((Math.PI / 2)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableRotationTest02() {
            String outFileName = destinationFolder + "tableRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "tableRotationTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 50, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1").SetRotationAngle
                ((Math.PI / 2)))).AddCell(new Cell().Add(new Paragraph("cell 1, 2").SetRotationAngle((Math.PI / 3)))).
                AddCell(new Cell().Add(new Paragraph("cell 2, 1").SetRotationAngle((Math.PI / 3)))).AddCell(new Cell()
                .Add(new Paragraph("cell 2, 2").SetRotationAngle((Math.PI))));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void TableRotationTest03() {
            String outFileName = destinationFolder + "tableRotationTest03.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "tableRotationTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(new float[] { 25, 50 }).AddCell(new Cell().Add(new Paragraph("cell 1, 1").SetRotationAngle
                ((Math.PI / 2)))).AddCell(new Cell().Add(new Paragraph("cell 1, 2").SetRotationAngle((Math.PI / 3)))).
                AddCell(new Cell().Add(new Paragraph("cell 2, 1"))).AddCell(new Cell().Add(new Paragraph("cell 2, 2"))
                ).AddCell(new Cell().Add(new Paragraph("cell 3, 1").SetRotationAngle(-(Math.PI / 2)))).AddCell(new Cell
                ().Add(new Paragraph("cell 3, 2").SetRotationAngle((Math.PI))));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CellRotationTest01() {
            String outFileName = destinationFolder + "cellRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "cellRotationTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Table table = new Table(1).SetWidth(50).AddCell(new Cell().Add(new Paragraph("Hello")).SetRotationAngle(Math
                .PI * 70 / 180.0).SetBackgroundColor(Color.GREEN));
            doc.Add(table);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivRotationTest01() {
            String outFileName = destinationFolder + "divRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "divRotationTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Div div = new Div().SetBackgroundColor(Color.GREEN);
            div.Add(new Paragraph(para1Text).SetBackgroundColor(Color.RED)).SetRotationAngle(Math.PI / 4);
            doc.Add(div);
            div = new Div();
            div.Add(new Paragraph(para1Text)).SetRotationAngle(Math.PI / 2);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void DivRotationTest02() {
            String outFileName = destinationFolder + "divRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "divRotationTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph(para1Text));
            doc.Add(new Paragraph(para1Text));
            doc.Add(new Paragraph(para1Text));
            String extremelyLongText = "";
            for (int i = 0; i < 300; ++i) {
                extremelyLongText += para2Text;
            }
            doc.Add(new Div().Add(new Paragraph(extremelyLongText)).SetRotationAngle(Math.PI / 2));
            doc.Add(new Div().Add(new Paragraph(extremelyLongText)).SetRotationAngle(Math.PI / 4));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ListRotationTest01() {
            String outFileName = destinationFolder + "listRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "listRotationTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph(para1Text));
            List list = new List().SetRotationAngle(3 * Math.PI / 4).SetBackgroundColor(Color.GREEN);
            list.Add(new ListItem("text of first list item"));
            list.Add("text of second list item");
            list.Add("text of third list item");
            doc.Add(list);
            doc.Add(new Paragraph(para2Text));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ListRotationTest02() {
            String outFileName = destinationFolder + "listRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "listRotationTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph(para1Text));
            doc.Add(new Paragraph(para1Text));
            doc.Add(new Paragraph(para1Text));
            List list = new List().SetRotationAngle(Math.PI / 2).SetBackgroundColor(Color.GREEN);
            String itemText = "list item text long item txt list item text long item txt list item text long item txt list item text long item txt list item text long item txt END";
            for (int i = 0; i < 10; ++i) {
                list.Add(itemText);
            }
            doc.Add(list);
            doc.Add(new Paragraph(para2Text));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AlignedTextRotationTest01() {
            String outFileName = destinationFolder + "alignedTextRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "alignedTextRotationTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph(para1Text));
            Paragraph p = new Paragraph();
            p.Add("texttext").SetTextAlignment(TextAlignment.CENTER).SetHorizontalAlignment(HorizontalAlignment.CENTER
                );
            p.SetRotationAngle(Math.PI / 4);
            doc.Add(p);
            doc.Add(new Paragraph(para3Text));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InnerRotationTest01() {
            String outFileName = destinationFolder + "innerRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "innerRotationTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Div().SetBackgroundColor(Color.GREEN).SetHeight(300).SetWidth(300).Add(new Div().SetBackgroundColor
                (Color.RED).SetHeight(100).SetWidth(100).SetRotationAngle(Math.PI / 4)).SetRotationAngle(Math.PI / 8));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [NUnit.Framework.Test]
        public virtual void InnerRotationTest02() {
            String outFileName = destinationFolder + "innerRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "innerRotationTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(6400, 6400));
            String longText = para1Text + para2Text + para3Text;
            String extremeLongText = longText + longText + longText;
            doc.Add(new Div().SetBackgroundColor(Color.GREEN).SetMinHeight(300).SetWidth(300).Add(new Div().SetBackgroundColor
                (Color.RED).SetWidth(30).SetRotationAngle(5 * Math.PI / 16).Add(new Paragraph(extremeLongText))).Add(new 
                Paragraph("smaaaaaaaaaaaaaaaaaaaall taaaaaaaaaaaaaaaaaaalk")).Add(new Paragraph("smaaaaaaaaaaaaaaaaaaaall taaaaaaaaaaaaaaaaaaalk"
                )).SetRotationAngle(Math.PI / 8));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedWidthRotationTest01() {
            //TODO: currently is incorrect. See DEVSIX-988
            String outFileName = destinationFolder + "fixedWidthRotationTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedWidthRotationTest01.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Text text = new Text("Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me."
                );
            Div d = new Div().SetWidth(300).SetBorder(new SolidBorder(Color.RED, 5)).SetPadding(5);
            Paragraph p = new Paragraph(text).SetWidth(600).SetRotationAngle(Math.PI / 2).SetBorder(new SolidBorder(Color
                .BLUE, 5));
            doc.Add(d.Add(p));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedWidthRotationTest02() {
            //TODO: currently is incorrect. See DEVSIX-988
            String outFileName = destinationFolder + "fixedWidthRotationTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedWidthRotationTest02.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Text text = new Text("Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me."
                );
            Div d = new Div().SetWidth(300).SetBorder(new SolidBorder(Color.RED, 5)).SetPadding(5);
            Paragraph p = new Paragraph(text).SetWidth(500).SetRotationAngle(Math.PI * 3 / 8).SetBorder(new SolidBorder
                (Color.BLUE, 5));
            doc.Add(d.Add(p));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedWidthRotationTest03() {
            //TODO: currently is incorrect. See DEVSIX-988
            String outFileName = destinationFolder + "fixedWidthRotationTest03.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "fixedWidthRotationTest03.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Text text = new Text("Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me."
                );
            Div d = new Div().SetWidth(300).SetBorder(new SolidBorder(Color.RED, 5)).SetPadding(5);
            Div d1 = new Div().Add(new Paragraph(text)).SetWidth(500).SetRotationAngle(Math.PI * 5 / 8).SetBorder(new 
                SolidBorder(Color.BLUE, 5));
            doc.Add(d.Add(d1));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MarginsRotatedTest01() {
            //TODO: currently is incorrect. See DEVSIX-989
            String outFileName = destinationFolder + "marginsRotatedTest01.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "marginsRotatedTest01.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            Text text = new Text("Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me."
                );
            Div d = new Div().SetWidth(400).SetBorder(new SolidBorder(Color.RED, 5));
            Div d1 = new Div().Add(new Paragraph(text)).SetWidth(200).SetRotationAngle(Math.PI / 4).SetMargins(100, 10
                , 100, 10).SetBorder(new SolidBorder(Color.BLUE, 5));
            doc.Add(d.Add(d1));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MarginsRotatedTest02() {
            //TODO: currently is incorrect. See DEVSIX-989
            String outFileName = destinationFolder + "marginsRotatedTest02.pdf";
            String cmpFileName = sourceFolder + cmpPrefix + "marginsRotatedTest02.pdf";
            Document doc = new Document(new PdfDocument(new PdfWriter(outFileName)));
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            Text text = new Text("Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me."
                );
            Div d = new Div().SetWidth(400).SetBorder(new SolidBorder(Color.RED, 5));
            Div d1 = new Div().Add(new Paragraph(text)).SetWidth(200).SetRotationAngle(Math.PI / 4).SetMargins(100, 10
                , 100, 10).SetBorder(new SolidBorder(Color.BLUE, 5));
            doc.Add(d.Add(d1).Add(new Paragraph("Hello").SetMargin(50).SetBorder(new SolidBorder(Color.GREEN, 5))));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private void DrawCross(PdfCanvas canvas, float x, float y) {
            DrawLine(canvas, x - 50, y, x + 50, y);
            DrawLine(canvas, x, y - 50, x, y + 50);
        }

        private void DrawLine(PdfCanvas canvas, float x1, float y1, float x2, float y2) {
            canvas.SaveState().SetLineWidth(0.5f).SetLineDash(3).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
        }
    }
}
