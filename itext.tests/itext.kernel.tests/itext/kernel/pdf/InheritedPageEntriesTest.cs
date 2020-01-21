using System;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class InheritedPageEntriesTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/InheritedPageEntriesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/InheritedPageEntriesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
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

        [NUnit.Framework.Test]
        public virtual void MediaBoxInheritance() {
            String inputFileName = sourceFolder + "mediaBoxInheritanceTestSource.pdf";
            PdfDocument outFile = new PdfDocument(new PdfReader(inputFileName));
            PdfObject mediaBox = outFile.GetPage(1).GetPdfObject().Get(PdfName.MediaBox);
            //Check if MediaBox in Page is absent
            NUnit.Framework.Assert.IsNull(mediaBox);
            PdfArray array = outFile.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages).GetAsArray(PdfName.MediaBox
                );
            Rectangle rectangle = array.ToRectangle();
            Rectangle pageRect = outFile.GetPage(1).GetMediaBox();
            outFile.Close();
            NUnit.Framework.Assert.IsTrue(rectangle.EqualsWithEpsilon(pageRect));
        }

        [NUnit.Framework.Test]
        public virtual void CropBoxInheritance() {
            String inputFileName = sourceFolder + "cropBoxInheritanceTestSource.pdf";
            PdfDocument outFile = new PdfDocument(new PdfReader(inputFileName));
            PdfObject cropBox = outFile.GetPage(1).GetPdfObject().Get(PdfName.CropBox);
            //Check if CropBox in Page is absent
            NUnit.Framework.Assert.IsNull(cropBox);
            PdfArray array = outFile.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages).GetAsArray(PdfName.CropBox
                );
            Rectangle rectangle = array.ToRectangle();
            Rectangle pageRect = outFile.GetPage(1).GetCropBox();
            outFile.Close();
            NUnit.Framework.Assert.IsTrue(rectangle.EqualsWithEpsilon(pageRect));
        }
    }
}
