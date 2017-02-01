using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class DefaultLayoutTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/DefaultLayoutTest/";

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
        [LogMessage(iText.IO.LogMessageConstant.RECTANGLE_HAS_NEGATIVE_OR_ZERO_SIZES, Count = 1)]
        public virtual void EmptyParagraphsTest01() {
            String outFileName = destinationFolder + "emptyParagraphsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyParagraphsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph());
            // this line should not cause any effect
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
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
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
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
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
