using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class KeepTogetherTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/KeepTogetherTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/KeepTogetherTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherParagraphTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest01.pdf";
            String outFile = destinationFolder + "keepTogetherParagraphTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 29; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            Paragraph p1 = new Paragraph(str);
            p1.SetKeepTogether(true);
            doc.Add(p1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherParagraphTest02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherParagraphTest02.pdf";
            String outFile = destinationFolder + "keepTogetherParagraphTest02.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            String str = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaanasdadasdadaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            for (int i = 0; i < 5; i++) {
                str += str;
            }
            Paragraph p1 = new Paragraph(str);
            p1.SetKeepTogether(true);
            doc.Add(p1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherListTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherListTest01.pdf";
            String outFile = destinationFolder + "keepTogetherListTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            for (int i = 0; i < 28; i++) {
                doc.Add(new Paragraph("String number" + i));
            }
            List list = new List();
            list.Add("firstItem").Add("secondItem").Add("thirdItem").SetKeepTogether(true).SetListSymbol(ListNumberingType
                .DECIMAL);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KeepTogetherDivTest01() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest01.pdf";
            String outFile = destinationFolder + "keepTogetherDivTest01.pdf";
            PdfWriter writer = new PdfWriter(outFile);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph("Test String");
            for (int i = 0; i < 28; i++) {
                doc.Add(p);
            }
            Div div = new Div();
            div.Add(new Paragraph("first paragraph"));
            div.Add(new Paragraph("second paragraph"));
            div.Add(new Paragraph("third paragraph"));
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherDivTest02() {
            String cmpFileName = sourceFolder + "cmp_keepTogetherDivTest02.pdf";
            String outFile = destinationFolder + "keepTogetherDivTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFile));
            Document doc = new Document(pdfDoc);
            Rectangle[] columns = new Rectangle[] { new Rectangle(100, 100, 100, 500), new Rectangle(400, 100, 100, 500
                ) };
            doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
            Div div = new Div();
            doc.Add(new Paragraph("first string"));
            for (int i = 0; i < 130; i++) {
                div.Add(new Paragraph("String number " + i));
            }
            div.SetKeepTogether(true);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFileName, destinationFolder, 
                "diff"));
        }
    }
}
