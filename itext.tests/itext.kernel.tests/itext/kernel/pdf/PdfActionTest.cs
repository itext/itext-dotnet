using System;
using iText.Kernel.Pdf.Action;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfActionTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfActionTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfActionTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ActionTest01() {
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + "actionTest01.pdf"), true);
            document.GetCatalog().SetOpenAction(PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(String.Format("Please open document {0} and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest01.pdf", "http://itextpdf.com"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ActionTest02() {
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + "actionTest02.pdf"), false);
            document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(String.Format("Please open document {0} at page 2 and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest02.pdf", "http://itextpdf.com"));
        }

        private PdfDocument CreateDocument(PdfWriter writer, bool flushPages) {
            PdfDocument document = new PdfDocument(writer);
            PdfPage p1 = document.AddNewPage();
            PdfStream str1 = p1.GetFirstContentStream();
            str1.GetOutputStream().WriteString("1 0 0 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p1.Flush();
            }
            PdfPage p2 = document.AddNewPage();
            PdfStream str2 = p2.GetFirstContentStream();
            str2.GetOutputStream().WriteString("0 1 0 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p2.Flush();
            }
            PdfPage p3 = document.AddNewPage();
            PdfStream str3 = p3.GetFirstContentStream();
            str3.GetOutputStream().WriteString("0 0 1 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p3.Flush();
            }
            return document;
        }
    }
}
