using System;
using iText.IO;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class DefaultLayoutTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/DefaultLayoutTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/DefaultLayoutTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultipleAdditionsOfSameModelElementTest() {
            String outFileName = destinationFolder + "multipleAdditionsOfSameModelElementTest1.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleAdditionsOfSameModelElementTest1.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph("Hello. I am a paragraph. I want you to process me correctly");
            document.Add(p).Add(p).Add(new AreaBreak(PageSize.Default)).Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RendererTest01() {
            String outFileName = destinationFolder + "rendererTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_rendererTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            document.Add(new Paragraph(new Text(str).SetBackgroundColor(Color.RED)).SetBackgroundColor(Color.GREEN)).Add
                (new Paragraph(str)).Add(new AreaBreak(PageSize.Default)).Add(new Paragraph(str));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES)]
        public virtual void EmptyParagraphsTest01() {
            String outFileName = destinationFolder + "emptyParagraphsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyParagraphsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            // the next 3 lines should not cause any effect
            document.Add(new Paragraph());
            document.Add(new Paragraph().SetBackgroundColor(Color.GREEN));
            document.Add(new Paragraph().SetBorder(new SolidBorder(Color.BLUE, 3)));
            document.Add(new Paragraph("Hello! I'm the first paragraph added to the document. Am i right?").SetBackgroundColor
                (Color.RED).SetBorder(new SolidBorder(1)));
            document.Add(new Paragraph().SetHeight(50));
            document.Add(new Paragraph("Hello! I'm the second paragraph added to the document. Am i right?"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EmptyParagraphsTest02() {
            String outFileName = destinationFolder + "emptyParagraphsTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyParagraphsTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Hello, i'm the text of the first paragraph on the first line. Let's break me and meet on the next line!\nSee? I'm on the second line. Now let's create some empty lines,\n for example one\n\nor two\n\n\nor three\n\n\n\nNow let's do something else"
                ));
            document.Add(new Paragraph("\n\n\nLook, i'm the the text of the second paragraph. But before me and the first one there are three empty lines!"
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HeightTest01() {
            String outFileName = destinationFolder + "heightTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_heightTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            String textHelloWorld = "Hello World\n" + "Hello World\n" + "Hello World\n" + "Hello World\n" + "Hello World\n";
            Document doc = new Document(pdfDocument);
            List list = new List(ListNumberingType.DECIMAL);
            for (int i = 0; i < 10; i++) {
                list.Add(new ListItem("" + i));
            }
            list.SetHeight(60);
            list.SetBorder(new SolidBorder(0.5f));
            //list.setPaddingTop(100); // TODO
            doc.Add(list);
            doc.Add(new AreaBreak());
            doc.Add(list);
            doc.Add(new AreaBreak());
            Paragraph p = new Paragraph(textByron);
            //for (int i = 0; i < 15; i++) {
            p.Add(textByron);
            //}
            p.SetBorder(new SolidBorder(0.5f));
            p.SetHeight(1000);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(p);
            doc.Add(new AreaBreak());
            p.SetBorder(Border.NO_BORDER);
            Div div = new Div();
            div.SetBorder(new SolidBorder(0.5f));
            for (int i_1 = 0; i_1 < 5; i_1++) {
                div.Add(p);
            }
            div.SetHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(div);
            doc.Add(new AreaBreak());
            Table table = new Table(2);
            table.SetBorder(new SolidBorder(Color.RED, 2f));
            //        table.addCell(new Cell(2, 1).add(new Paragraph(textHelloWorld)));
            for (int i_2 = 0; i_2 < 4; i_2++) {
                table.AddCell(new Cell().Add(new Paragraph(textByron)).SetBorder(new SolidBorder(Color.YELLOW, 1)));
            }
            //        table.addCell(new Cell(1, 2).add(textByron));
            table.SetHeight(1700);
            doc.Add(table);
            doc.Add(new Paragraph("Hello"));
            doc.Add(new AreaBreak());
            doc.Add(table);
            doc.Add(new Paragraph("Hello"));
            doc.Add(new AreaBreak());
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            image.SetMaxHeight(100);
            doc.Add(image);
            doc.Add(new AreaBreak());
            doc.Add(image);
            doc.Add(new AreaBreak());
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void AddParagraphOnShortPage1() {
            String outFileName = destinationFolder + "addParagraphOnShortPage1.pdf";
            String cmpFileName = sourceFolder + "cmp_addParagraphOnShortPage1.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(500, 70));
            Paragraph p = new Paragraph();
            p.Add("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            p.Add(new Text("BBB").SetFontSize(30));
            p.Add("CCC");
            p.Add("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
            p.Add("EEE");
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void AddParagraphOnShortPage2() {
            String outFileName = destinationFolder + "addParagraphOnShortPage2.pdf";
            String cmpFileName = sourceFolder + "cmp_addParagraphOnShortPage2.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(300, 50));
            Paragraph p = new Paragraph();
            p.Add("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                );
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
