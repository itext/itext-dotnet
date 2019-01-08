using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class ParagraphTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ParagraphTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ParagraphTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CannotPlaceABigChunkOnALineTest01() {
            String outFileName = destinationFolder + "cannotPlaceABigChunkOnALineTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_cannotPlaceABigChunkOnALineTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0));
            p.Add(new Text("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa").SetBorder
                (new SolidBorder(ColorConstants.RED, 0)));
            p.Add(new Text("b").SetFontSize(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CannotPlaceABigChunkOnALineTest02() {
            String outFileName = destinationFolder + "cannotPlaceABigChunkOnALineTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_cannotPlaceABigChunkOnALineTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0));
            p.Add(new Text("smaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaall").SetFontSize(5).SetBorder
                (new SolidBorder(ColorConstants.RED, 0)));
            p.Add(new Text("biiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiig"
                ).SetFontSize(20).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WordWasSplitAndItWillFitOntoNextLineTest01() {
            String outFileName = destinationFolder + "wordWasSplitAndItWillFitOntoNextLineTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_wordWasSplitAndItWillFitOntoNextLineTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph().SetBorder(new SolidBorder(ColorConstants.YELLOW, 0)).SetTextAlignment(TextAlignment
                .RIGHT);
            for (int i = 0; i < 5; i++) {
                p.Add(new Text("aaaaaaaaaaaaaaaaaaaaa" + i).SetBorder(new SolidBorder(ColorConstants.BLUE, 0)));
            }
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
