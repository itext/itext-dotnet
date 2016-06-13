using System;
using System.IO;
using iTextSharp.Kernel.Pdf.Action;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf {
    public class PdfActionTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itextsharp/kernel/pdf/PdfActionTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itextsharp/kernel/pdf/PdfActionTest/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ActionTest01() {
            PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "actionTest01.pdf", FileMode.Create));
            PdfDocument document = CreateDocument(writer, true);
            document.GetCatalog().SetOpenAction(PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(String.Format("Please open document {0} and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest01.pdf", "http://itextpdf.com"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ActionTest02() {
            PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "actionTest02.pdf", FileMode.Create));
            PdfDocument document = CreateDocument(writer, false);
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
