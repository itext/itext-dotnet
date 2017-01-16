using System;
using System.Text;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class OverflowTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/OverflowTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/OverflowTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextOverflowTest01() {
            String outFileName = destinationFolder + "textOverflowTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 1000; i++) {
                text.Append("This is a waaaaay tooo long text...");
            }
            Paragraph p = new Paragraph(text.ToString()).SetFont(PdfFontFactory.CreateFont(FontConstants.HELVETICA));
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextOverflowTest02() {
            String outFileName = destinationFolder + "textOverflowTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Text overflowText = new iText.Layout.Element.Text("This is a long-long and large text which will not overflow"
                ).SetFontSize(19).SetFontColor(Color.RED);
            iText.Layout.Element.Text followText = new iText.Layout.Element.Text("This is a text which follows overflowed text and will be wrapped"
                );
            document.Add(new Paragraph().Add(overflowText).Add(followText));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextOverflowTest03() {
            String outFileName = destinationFolder + "textOverflowTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Text overflowText = new iText.Layout.Element.Text("This is a long-long and large text which will overflow"
                ).SetFontSize(25).SetFontColor(Color.RED);
            iText.Layout.Element.Text followText = new iText.Layout.Element.Text("This is a text which follows overflowed text and will not be wrapped"
                );
            document.Add(new Paragraph().Add(overflowText).Add(followText));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TextOverflowTest04() {
            String outFileName = destinationFolder + "textOverflowTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_textOverflowTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("ThisIsALongTextWithNoSpacesSoSplittingShouldBeForcedInThisCase").SetFontSize(20
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
