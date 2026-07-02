/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Webpimagesupport {
    [NUnit.Framework.Category("IntegrationTest")]
    public class WebPIntegrationTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/webpimagesupport/WebpIntegrationTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/webpimagesupport/image/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> GetWebPImages() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "5_webp_ll", false, false }, new Object[] { "lossless"
                , false, false }, new Object[] { "lossyWebPImage", false, false }, new Object[] { "opaqueWebPImage", false
                , false }, new Object[] { "animatedWebPImage", false, false }, new Object[] { "displayP3Profile", true
                , true }, new Object[] { "linearRGBProfile", false, true }, 
                        // TODO DEVSIX-10022 - Support image orientation set in exif metadata
                        
                        // when modern browsers start supporting it
                        new Object[] { "orientation", false, false } });
        }

        [NUnit.Framework.TestCaseSource("GetWebPImages")]
        public virtual void WebpSimpleImageTest(String imageName, bool isImageBig, bool isPlatformDependent) {
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
                if (isImageBig) {
                    canvas.AddImageFittedIntoRectangle(img, new Rectangle(50, 50, 500, 700), false);
                }
                else {
                    canvas.AddImageAt(img, 50, 50, false);
                }
                canvas.Release();
            }
            pdfDocument.Close();
            if (isPlatformDependent) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outFileName, cmpFileName, DESTINATION_FOLDER
                    , 1));
            }
            else {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                    ));
            }
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
