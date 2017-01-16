using System;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class PositioningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PositioningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/PositioningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RelativePositioningTest01() {
            String outFileName = destinationFolder + "relativePositioningTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_relativePositioningTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(new DeviceGray(0), 5)).SetWidth(260).SetPaddings(20
                , 20, 20, 20).Add("Here is a line of text.").Add(new Text("This part is shifted\n up a bit,").SetRelativePosition
                (0, -10, 0, 0).SetBackgroundColor(new DeviceGray(0.8f))).Add("but the rest of the line is in its original position."
                );
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RelativePositioningTest02() {
            String outFileName = destinationFolder + "relativePositioningTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_relativePositioningTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(new DeviceGray(0), 5)).SetWidth(140).SetPaddings(20
                , 20, 20, 20).Add("Here is a line of text.").Add(new Text("This part is shifted\n up a bit,").SetRelativePosition
                (0, -10, 0, 0).SetBackgroundColor(new DeviceGray(0.8f))).Add("but the rest of the line is in its original position."
                ).SetRelativePosition(50, 0, 0, 0);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedPositioningTest01() {
            String outFileName = destinationFolder + "fixedPositioningTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_fixedPositioningTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List list = new List(ListNumberingType.ROMAN_UPPER).SetFixedPosition(2, 300, 300, 50).SetBackgroundColor(Color
                .BLUE).SetHeight(100);
            list.Add("Hello").Add("World").Add("!!!");
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FixedPositioningTest02() {
            String outFileName = destinationFolder + "fixedPositioningTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_fixedPositioningTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.GetPdfDocument().AddNewPage();
            new PdfCanvas(document.GetPdfDocument().GetPage(1)).SetFillColor(Color.BLACK).Rectangle(300, 300, 100, 100
                ).Fill().Release();
            Paragraph p = new Paragraph("Hello").SetBackgroundColor(Color.BLUE).SetHeight(100).SetFixedPosition(1, 300
                , 300, 100);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ShowTextAlignedTest01() {
            String outFileName = destinationFolder + "showTextAlignedTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_showTextAlignedTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
            String text = "textapqgaPQGatext";
            float width = 200;
            float x;
            float y;
            y = 700;
            x = 115;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.LEFT, VerticalAlignment.BOTTOM, (float)(Math.PI / 6 * 1
                ));
            x = 300;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.LEFT, VerticalAlignment.MIDDLE, (float)(Math.PI / 6 * 3
                ));
            x = 485;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.LEFT, VerticalAlignment.TOP, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.LEFT, VerticalAlignment.TOP, (float)(Math.PI / 6 * 5));
            y = 400;
            x = 115;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.CENTER, VerticalAlignment.BOTTOM, (float)(Math.PI / 6 *
                 2));
            x = 300;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.CENTER, VerticalAlignment.MIDDLE, (float)(Math.PI / 6 *
                 4));
            x = 485;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.CENTER, VerticalAlignment.TOP, (float)(Math.PI / 6 * 8)
                );
            y = 100;
            x = 115;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, (float)(Math.PI / 6 * 
                9));
            x = 300;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.RIGHT, VerticalAlignment.MIDDLE, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.RIGHT, VerticalAlignment.MIDDLE, (float)(Math.PI / 6 * 
                7));
            x = 485;
            DrawCross(canvas, x, y);
            document.ShowTextAligned(text, x, y, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
            document.ShowTextAligned(text, x, y, TextAlignment.RIGHT, VerticalAlignment.TOP, (float)(Math.PI / 6 * 6));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ShowTextAlignedTest02() {
            String outFileName = destinationFolder + "showTextAlignedTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_showTextAlignedTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ShowTextAlignedTest03() {
            String outFileName = destinationFolder + "showTextAlignedTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_showTextAlignedTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(sourceFolder + "bruno.jpg"));
            float width = img.GetImageScaledWidth();
            float height = img.GetImageScaledHeight();
            PdfFormXObject template = new PdfFormXObject(new Rectangle(width, height));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(template, pdfDoc);
            canvas.Add(img).ShowTextAligned("HELLO BRUNO", width / 2, height / 2, TextAlignment.CENTER);
            doc.Add(new iText.Layout.Element.Image(template));
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
