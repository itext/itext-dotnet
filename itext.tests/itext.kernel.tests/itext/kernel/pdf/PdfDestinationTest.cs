using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfDestinationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDestinationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDestinationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestTest01() {
            String srcFile = sourceFolder + "simpleNoLinks.pdf";
            String outFile = destinationFolder + "destTest01.pdf";
            String cmpFile = sourceFolder + "cmp_destTest01.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outFile));
            PdfPage firstPage = document.GetPage(1);
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkExplicitDest.SetAction(PdfAction.CreateGoTo(PdfExplicitDestination.CreateFit(document.GetPage(2))));
            firstPage.AddAnnotation(linkExplicitDest);
            PdfLinkAnnotation linkStringDest = new PdfLinkAnnotation(new Rectangle(35, 760, 160, 15));
            PdfExplicitDestination destToPage3 = PdfExplicitDestination.CreateFit(document.GetPage(3));
            String stringDest = "thirdPageDest";
            document.AddNamedDestination(stringDest, destToPage3.GetPdfObject());
            linkStringDest.SetAction(PdfAction.CreateGoTo(new PdfStringDestination(stringDest)));
            firstPage.AddAnnotation(linkStringDest);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestCopyingTest01() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest01.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest01.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(outFile));
            srcDoc.CopyPagesTo(iText.IO.Util.JavaUtil.ArraysAsList(1, 2, 3), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestCopyingTest02() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest02.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest02.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(outFile));
            srcDoc.CopyPagesTo(iText.IO.Util.JavaUtil.ArraysAsList(1), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestCopyingTest03() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest03.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest03.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(outFile));
            srcDoc.CopyPagesTo(iText.IO.Util.JavaUtil.ArraysAsList(1, 2), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestCopyingTest04() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest04.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest04.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(outFile));
            srcDoc.CopyPagesTo(iText.IO.Util.JavaUtil.ArraysAsList(1, 3), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DestCopyingTest05() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest05.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest05.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(outFile));
            srcDoc.CopyPagesTo(iText.IO.Util.JavaUtil.ArraysAsList(1, 2, 3, 1), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }
    }
}
