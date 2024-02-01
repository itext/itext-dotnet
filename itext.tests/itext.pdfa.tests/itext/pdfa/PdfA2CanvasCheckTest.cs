/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA2CanvasCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2CanvasCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CanvasCheckTest1() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent)) {
                pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                    for (int i = 0; i < 29; i++) {
                        canvas.SaveState();
                    }
                }
                );
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.GRAPHICS_STATE_STACK_DEPTH_IS_GREATER_THAN_28
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanvasCheckTest2() {
            String outPdf = destinationFolder + "pdfA2b_canvasCheckTest2.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfA2CanvasCheckTest/cmp_pdfA2b_canvasCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent)) {
                pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
                for (int i = 0; i < 28; i++) {
                    canvas.SaveState();
                }
                for (int i = 0; i < 28; i++) {
                    canvas.RestoreState();
                }
            }
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanvasCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent)) {
                pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetRenderingIntent
                    (new PdfName("Test")));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.IF_SPECIFIED_RENDERING_SHALL_BE_ONE_OF_THE_FOLLOWING_RELATIVECOLORIMETRIC_ABSOLUTECOLORIMETRIC_PERCEPTUAL_OR_SATURATION
                    , e.Message);
            }
        }
    }
}
