using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class CollapsingMarginsTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/CollapsingMarginsTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/CollapsingMarginsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest01() {
            String outFileName = destinationFolder + "collapsingMarginsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 4);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(textByron);
            for (int i = 0; i < 5; i++) {
                p.Add(textByron);
            }
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(20);
            div2.SetMarginTop(150);
            div2.SetMarginBottom(150);
            Div div = new Div().SetMarginTop(20).SetMarginBottom(10).SetBackgroundColor(new DeviceRgb(78, 151, 205));
            div.Add(div1);
            div.Add(div2);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest02() {
            String outFileName = destinationFolder + "collapsingMarginsTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 3);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(textByron);
            for (int i = 0; i < 3; i++) {
                p.Add(textByron);
            }
            p.Add("When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n");
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(40);
            div2.SetMarginTop(20);
            div2.SetMarginBottom(150);
            Div div = new Div().SetMarginTop(20).SetMarginBottom(10).SetBackgroundColor(new DeviceRgb(78, 151, 205));
            div.Add(div1);
            div.Add(div2);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest03() {
            String outFileName = destinationFolder + "collapsingMarginsTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 3);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(textByron);
            for (int i = 0; i < 3; i++) {
                p.Add(textByron);
            }
            p.Add("When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "To do good to Mankind is the chivalrous plan,\n");
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(80);
            div2.SetMarginTop(80);
            div2.SetMarginBottom(150);
            doc.Add(div1);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsTest04() {
            String outFileName = destinationFolder + "collapsingMarginsTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DrawPageBorders(pdfDocument, 3);
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Document doc = new Document(pdfDocument);
            doc.SetProperty(Property.COLLAPSING_MARGINS, true);
            doc.Add(new Paragraph("marker text").SetMargin(0));
            Paragraph p = new Paragraph(textByron);
            for (int i = 0; i < 3; i++) {
                p.Add(textByron);
            }
            p.Add("When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "To do good to Mankind is the chivalrous plan,\n");
            p.Add(new Text("small text").SetFontSize(6));
            p.Add("\nAnd is always as nobly requited;\n" + "Then battle for Freedom wherever you can,\n" + "And, if not shot or hanged, you'll get knighted."
                );
            Div div1 = new Div();
            Div div2 = new Div();
            div1.Add(p).SetBackgroundColor(new DeviceRgb(65, 151, 29));
            div2.Add(p).SetBackgroundColor(new DeviceRgb(209, 247, 29));
            div1.SetMarginBottom(80);
            div2.SetMarginTop(80);
            div2.SetMarginBottom(150);
            doc.Add(div1);
            doc.Add(div2);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private void DrawPageBorders(PdfDocument pdfDocument, int pageNum) {
            for (int i = 1; i <= pageNum; ++i) {
                while (pdfDocument.GetNumberOfPages() < i) {
                    pdfDocument.AddNewPage();
                }
                PdfCanvas canvas = new PdfCanvas(pdfDocument.GetPage(i));
                canvas.SaveState();
                canvas.SetLineDash(5, 10);
                canvas.Rectangle(36, 36, 595 - 36 * 2, 842 - 36 * 2);
                canvas.Stroke();
                canvas.RestoreState();
            }
        }
    }
}
