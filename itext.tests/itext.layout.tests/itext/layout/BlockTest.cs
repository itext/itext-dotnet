using System;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class BlockTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/BlockTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BlockTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlockWithSetHeightProperties01() {
            String outFileName = destinationFolder + "blockWithSetHeightProperties01.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithSetHeightProperties01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph(textByron);
            for (int i = 0; i < 10; i++) {
                p.Add(textByron);
            }
            p.SetBorder(new SolidBorder(0.5f));
            doc.Add(new Paragraph("Default layout:"));
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set shorter than needed:"));
            p.SetHeight(1300);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's min height is set shorter than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.DeleteOwnProperty(Property.MAX_HEIGHT);
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetMinHeight(1300);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's max height is set shorter than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.DeleteOwnProperty(Property.MAX_HEIGHT);
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetMaxHeight(1300);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set bigger than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.DeleteOwnProperty(Property.MAX_HEIGHT);
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetHeight(2500);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's min height is set bigger than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.DeleteOwnProperty(Property.MAX_HEIGHT);
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetMinHeight(2500);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's max height is set bigger than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.DeleteOwnProperty(Property.MAX_HEIGHT);
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetMaxHeight(2500);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlockWithSetHeightProperties02() {
            String outFileName = destinationFolder + "blockWithSetHeightProperties02.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithSetHeightProperties02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph(textByron);
            Div div = new Div();
            div.SetBorder(new SolidBorder(Color.RED, 2));
            for (int i = 0; i < 5; i++) {
                div.Add(p);
            }
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 300);
            div.Add(image);
            doc.Add(new Paragraph("Default layout:"));
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's height is set shorter than needed:"));
            div.SetHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's min height is set shorter than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.DeleteOwnProperty(Property.MAX_HEIGHT);
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetMinHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's max height is set shorter than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.DeleteOwnProperty(Property.MAX_HEIGHT);
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetMaxHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's height is set bigger than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.DeleteOwnProperty(Property.MAX_HEIGHT);
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetHeight(2500);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's min height is set bigger than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.DeleteOwnProperty(Property.MAX_HEIGHT);
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetMinHeight(2500);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's max height is set bigger than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.DeleteOwnProperty(Property.MAX_HEIGHT);
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetMaxHeight(2500);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
