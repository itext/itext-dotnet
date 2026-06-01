using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Webpimagesupport {
    [NUnit.Framework.Category("IntegrationTest")]
    public class WebPIntegrationTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/webpimagesupport/WebpIntegrationTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext//webpimagesupport/image/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> GetWebPImages() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "5_webp_ll" }, new Object[] { "lossless" }, new 
                Object[] { "lossyWebPImage" }, new Object[] { "opaqueWebPImage" }, new Object[] { "animatedWebPImage" }
                 });
        }

        [NUnit.Framework.TestCaseSource("GetWebPImages")]
        public virtual void WebpSimpleImageTest(String imageName) {
            String imageFileName = SOURCE_FOLDER + imageName + ".webp";
            String outFileName = DESTINATION_FOLDER + imageName + "Pdf.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + imageName + "Pdf.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName, new WriterProperties()));
            using (Stream fis = FileUtil.GetInputStreamForFile(imageFileName)) {
                byte[] imageBytes = StreamUtil.InputStreamToArray(fis);
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                ImageData img = ImageDataFactory.Create(imageBytes);
                NUnit.Framework.Assert.AreEqual(ImageType.WEBP, img.GetOriginalType());
                canvas.AddImageAt(img, 50, 50, false);
                canvas.Release();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WebpPSimpleImageUrlTest() {
            String imageFileName = SOURCE_FOLDER + "lossless.webp";
            String outFileName = DESTINATION_FOLDER + "losslessUrlPdf.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_losslessPdf.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName, new WriterProperties()));
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.Create(UrlUtil.ToURL(imageFileName));
            NUnit.Framework.Assert.AreEqual(ImageType.WEBP, img.GetOriginalType());
            canvas.AddImageAt(img, 50, 50, false);
            canvas.Release();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }
    }
}
