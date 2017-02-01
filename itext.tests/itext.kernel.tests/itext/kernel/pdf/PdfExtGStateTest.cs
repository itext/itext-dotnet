using System;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfExtGStateTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfExtGStateTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/PdfExtGStateTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EgsTest1() {
            String destinationDocument = destinationFolder + "egsTest1.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationDocument));
            //Create page and canvas
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            //Create ExtGState and fill it with line width and font
            PdfExtGState egs = new PdfExtGState();
            egs.GetPdfObject().Put(PdfName.LW, new PdfNumber(5));
            PdfArray font = new PdfArray();
            PdfFont pdfFont = PdfFontFactory.CreateFont(FontConstants.COURIER);
            pdfFont.Flush();
            font.Add(pdfFont.GetPdfObject());
            font.Add(new PdfNumber(24));
            egs.GetPdfObject().Put(PdfName.Font, font);
            //Write ExtGState
            canvas.SetExtGState(egs);
            //Write text to check that font from ExtGState is applied
            canvas.BeginText();
            canvas.MoveText(50, 600);
            canvas.ShowText("Courier, 24pt");
            canvas.EndText();
            //Draw line to check if ine width is applied
            canvas.MoveTo(50, 500);
            canvas.LineTo(300, 500);
            canvas.Stroke();
            //Write text again to check that font from page resources and font from ExtGState is the same.
            canvas.BeginText();
            canvas.SetFontAndSize(pdfFont, 36);
            canvas.MoveText(50, 400);
            canvas.ShowText("Courier, 36pt");
            canvas.EndText();
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_egsTest1.pdf"
                , destinationFolder, "diff_"));
        }
    }
}
