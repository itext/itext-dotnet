using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;

namespace iText.Pdfa {
    public class PDFA2LayoutOcgTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PDFA2LayoutOcgTest/";

        [NUnit.Framework.SetUp]
        public virtual void Configure() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CheckIfOcgForPdfA2Works() {
            String fileName = "createdOcgPdfA.pdf";
            Stream colorStream = new FileStream(sourceFolder + "color/sRGB_CS_profile.icm", FileMode.Open, FileAccess.Read
                );
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp/PDFA2LayoutOcg/cmp_" + fileName;
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outFileName), PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", colorStream));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            pdfDoc.AddNewPage();
            iText.Layout.Element.Image image1 = new Image(ImageDataFactory.Create(sourceFolder + "images/manualTransparency_for_png.png"
                ));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDoc, 1);
            iText.Layout.Canvas canvas1 = new iText.Layout.Canvas(pdfCanvas, pdfDoc, new Rectangle(0, 0, 590, 420));
            PdfLayer imageLayer1 = new PdfLayer("*SomeTest_image$here@.1", pdfDoc);
            imageLayer1.SetOn(true);
            pdfCanvas.BeginLayer(imageLayer1);
            canvas1.Add(image1);
            pdfCanvas.EndLayer();
            canvas1.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
        }
    }
}
