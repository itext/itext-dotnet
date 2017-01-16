using System;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class AlignmentTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AlignmentTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AlignmentTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest01() {
            String outFileName = destinationFolder + "justifyAlignmentTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_justifyAlignmentTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            for (int i = 0; i < 21; i++) {
                paragraph.Add(new Text("Hello World! Hello People! " + "Hello Sky! Hello Sun! Hello Moon! Hello Stars!").SetBackgroundColor
                    (DeviceRgb.RED));
            }
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest02() {
            String outFileName = destinationFolder + "justifyAlignmentTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_justifyAlignmentTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            paragraph.Add(new Text("Hello World!")).Add(new Text(" ")).Add(new Text("Hello People! ")).Add("End");
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest03() {
            String outFileName = destinationFolder + "justifyAlignmentTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_justifyAlignmentTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            for (int i = 0; i < 21; i++) {
                paragraph.Add(new Text("Hello World! Hello People! " + "Hello Sky! Hello Sun! Hello Moon! Hello Stars!").SetBorder
                    (new SolidBorder(Color.GREEN, 0.1f))).SetMultipliedLeading(1);
            }
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest04() {
            String outFileName = destinationFolder + "justifyAlignmentTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_justifyAlignmentTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            for (int i = 0; i < 21; i++) {
                paragraph.Add(new Text("Hello World! Hello People! " + "Hello Sky! Hello Sun! Hello Moon! Hello Stars!")).
                    SetFixedLeading(24);
            }
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentForcedNewlinesTest01() {
            String outFileName = destinationFolder + "justifyAlignmentForcedNewlinesTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_justifyAlignmentForcedNewlinesTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            paragraph.Add("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
                 + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
                 + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
                 + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
                 + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
                );
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void JustifyAllTest01() {
            String outFileName = destinationFolder + "justifyAllTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_justifyAllTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
            paragraph.Add("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
                 + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
                 + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
                 + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
                 + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
                );
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlockAlignmentTest01() {
            String outFileName = destinationFolder + "blockAlignmentTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_blockAlignmentTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List list = new List(ListNumberingType.GREEK_LOWER);
            for (int i = 0; i < 10; i++) {
                list.Add("Item # " + (i + 1));
            }
            list.SetWidth(250);
            list.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            list.SetBackgroundColor(Color.GREEN);
            document.Add(list);
            list.SetHorizontalAlignment(HorizontalAlignment.RIGHT).SetBackgroundColor(Color.RED);
            list.SetTextAlignment(TextAlignment.CENTER);
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlockAlignmentTest02() {
            String outFileName = destinationFolder + "blockAlignmentTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_blockAlignmentTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div div = new Div();
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(sourceFolder + "Desert.jpg"
                )));
            iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .RIGHT);
            iText.Layout.Element.Image image2 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .CENTER);
            iText.Layout.Element.Image image3 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .LEFT);
            div.Add(image1).Add(image2).Add(image3);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageAlignmentTest01() {
            String outFileName = destinationFolder + "imageAlignmentTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageAlignmentTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(sourceFolder + "Desert.jpg"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .RIGHT);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
