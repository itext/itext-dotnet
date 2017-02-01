using System;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class OpacityTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/OpacityTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/OpacityTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BackgroundOpacityTest01() {
            String outFileName = destinationFolder + "backgroundOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_backgroundOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(200);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) background.").SetBackgroundColor
                (Color.RED, 0f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) background.").SetBackgroundColor
                (Color.RED, 0.3f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) background.").SetBackgroundColor
                (Color.RED, 0.5f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) background.").SetBackgroundColor
                (Color.RED, 0.7f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) background.").SetBackgroundColor
                (Color.RED, 1f));
            div.Add(new Paragraph("Simple text inside of the div with background.").SetBackgroundColor(Color.RED));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BorderOpacityTest01() {
            String outFileName = destinationFolder + "borderOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_borderOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(300);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) border.").SetBorder(new DoubleBorder
                (Color.RED, 7f, 0.0f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) border.").SetBorder(new DoubleBorder
                (Color.RED, 7f, 0.3f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) border.").SetBorder(new DoubleBorder
                (Color.RED, 7f, 0.5f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) border.").SetBorder(new DoubleBorder
                (Color.RED, 7f, 0.7f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) border.").SetBorder(new DoubleBorder
                (Color.RED, 7f, 1.0f)));
            div.Add(new Paragraph("Simple text inside of the div with border.").SetBorder(new DoubleBorder(Color.RED, 
                7f)));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextOpacityTest01() {
            String outFileName = destinationFolder + "textOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_textOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(300);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) text.").SetFontColor(Color.RED
                , 0f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) text.").SetFontColor(Color.RED
                , 0.3f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) text.").SetFontColor(Color.RED
                , 0.5f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) text.").SetFontColor(Color.RED
                , 0.7f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) text.").SetFontColor(Color.RED
                , 1f));
            div.Add(new Paragraph("Simple text inside of the div with text.").SetFontColor(Color.RED));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void UnderlineOpacityTest01() {
            String outFileName = destinationFolder + "underlineOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_underlineOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(300);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) underline.").SetUnderline(Color
                .RED, 0.0f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) underline.").SetUnderline(Color
                .RED, 0.3f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) underline.").SetUnderline(Color
                .RED, 0.5f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) underline.").SetUnderline(Color
                .RED, 0.7f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) underline.").SetUnderline(Color
                .RED, 1.0f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with underline.").SetUnderline(Color.RED, .75f, 0, 0, 
                -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextElementOpacity01() {
            ElementOpacityTest("text");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DivElementOpacity01() {
            ElementOpacityTest("div");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ParaElementOpacity01() {
            ElementOpacityTest("para");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageElementOpacity01() {
            ElementOpacityTest("image");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CellElementOpacity01() {
            ElementOpacityTest("cell");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TableElementOpacity01() {
            ElementOpacityTest("table");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ListElementOpacity01() {
            ElementOpacityTest("list");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ListItemElementOpacity01() {
            ElementOpacityTest("listItem");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void ElementOpacityTest(String elem) {
            String outFileName = destinationFolder + elem + "ElementOpacity01.pdf";
            String cmpFileName = sourceFolder + "cmp_" + elem + "ElementOpacity01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb divBackground = WebColors.GetRGBColor("#82abd6");
            DeviceRgb paraBackground = WebColors.GetRGBColor("#994ec7");
            DeviceRgb textBackground = WebColors.GetRGBColor("#009688");
            DeviceRgb tableBackground = WebColors.GetRGBColor("#ffc107");
            document.SetFontColor(Color.WHITE);
            Div div = new Div().SetBackgroundColor(divBackground);
            if ("div".Equals(elem)) {
                div.SetOpacity(0.3f);
            }
            div.Add(new Paragraph("direct div content"));
            Paragraph p = new Paragraph("direct paragraph content").SetBackgroundColor(paraBackground);
            if ("para".Equals(elem)) {
                p.SetOpacity(0.3f);
            }
            Text text = new Text("text content").SetBackgroundColor(textBackground);
            p.Add(text);
            if ("text".Equals(elem)) {
                text.SetOpacity(0.3f);
            }
            div.Add(p);
            iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            div.Add(image);
            if ("image".Equals(elem)) {
                image.SetOpacity(0.3f);
            }
            Table table = new Table(2).SetBackgroundColor(tableBackground);
            table.AddCell("Cell00");
            table.AddCell("Cell01");
            Cell cell10 = new Cell().Add("Cell10");
            if ("cell".Equals(elem)) {
                cell10.SetOpacity(0.3f);
            }
            table.AddCell(cell10);
            table.AddCell(new Cell().Add("Cell11"));
            if ("table".Equals(elem)) {
                table.SetOpacity(0.3f);
            }
            div.Add(table);
            List list = new List();
            if ("list".Equals(elem)) {
                list.SetOpacity(0.3f);
            }
            ListItem listItem = new ListItem("item 0");
            list.Add(listItem);
            if ("listItem".Equals(elem)) {
                listItem.SetOpacity(0.3f);
            }
            list.Add("item 1");
            div.Add(list);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
