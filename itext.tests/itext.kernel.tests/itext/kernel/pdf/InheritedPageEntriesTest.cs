using System;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class InheritedPageEntriesTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/InheritedPageEntriesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/InheritedPageEntriesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddNewPageToDocumentWithInheritedPageRotationTest() {
            //TODO: update cmp-files when DEVSIX-3635 will be fixed
            String inputFileName = sourceFolder + "srcFileTestRotationInheritance.pdf";
            String outputFileName = destinationFolder + "addNewPageToDocumentWithInheritedPageRotation.pdf";
            String cmpFileName = sourceFolder + "cmp_addNewPageToDocumentWithInheritedPageRotation.pdf";
            PdfDocument outFile = new PdfDocument(new PdfReader(inputFileName), new PdfWriter(outputFileName));
            PdfPage page = outFile.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 16
                ).ShowText("Hello Helvetica!").EndText().SaveState();
            outFile.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
