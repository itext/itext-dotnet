using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class OpacityTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/OpacityTest/";

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
    }
}
