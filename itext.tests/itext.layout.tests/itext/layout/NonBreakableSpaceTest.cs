using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class NonBreakableSpaceTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/NonBreakableSpaceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/NonBreakableSpaceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleParagraphTest() {
            String outFileName = destinationFolder + "simpleParagraphTest.pdf";
            String cmpFileName = sourceFolder + "cmp_simpleParagraphTest.pdf";
            String diffPrefix = "diff_simpleParagraphTest_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            document.Add(new Paragraph("aaa bbb\u00a0ccccccccccc").SetWidth(100).SetBorder(new SolidBorder(Color.RED, 
                10)));
            document.Add(new Paragraph("aaa bbb ccccccccccc").SetWidth(100).SetBorder(new SolidBorder(Color.GREEN, 10)
                ));
            document.Add(new Paragraph("aaaaaaa\u00a0bbbbbbbbbbb").SetWidth(100).SetBorder(new SolidBorder(Color.BLUE, 
                10)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ConsecutiveSpacesTest() {
            String outFileName = destinationFolder + "consecutiveSpacesTest.pdf";
            String cmpFileName = sourceFolder + "cmp_consecutiveSpacesTest.pdf";
            String diffPrefix = "diff_consecutiveSpacesTest_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            document.Add(new Paragraph("aaa\u00a0\u00a0\u00a0bbb").SetWidth(100).SetBorder(new SolidBorder(Color.RED, 
                10)));
            document.Add(new Paragraph("aaa\u00a0bbb").SetWidth(100).SetBorder(new SolidBorder(Color.GREEN, 10)));
            document.Add(new Paragraph("aaa   bbb").SetWidth(100).SetBorder(new SolidBorder(Color.BLUE, 10)));
            document.Add(new Paragraph("aaa bbb").SetWidth(100).SetBorder(new SolidBorder(Color.BLACK, 10)));
            Paragraph p = new Paragraph();
            p.Add("aaa\u00a0\u00a0\u00a0bbb").Add("ccc   ddd");
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }
    }
}
