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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA2LayoutOcgTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2LayoutOcgTest/";

        [NUnit.Framework.SetUp]
        public virtual void Configure() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CheckIfOcgForPdfA2Works() {
            String fileName = "createdOcgPdfA.pdf";
            Stream colorStream = FileUtil.GetInputStreamForFile(sourceFolder + "color/sRGB_CS_profile.icm");
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp/PdfA2LayoutOcgTest/cmp_" + fileName;
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outFileName), PdfAConformance.PDF_A_2A, new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", colorStream));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            pdfDoc.AddNewPage();
            iText.Layout.Element.Image image1 = new Image(ImageDataFactory.Create(sourceFolder + "images/manualTransparency_for_png.png"
                ));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDoc, 1);
            iText.Layout.Canvas canvas1 = new iText.Layout.Canvas(pdfCanvas, new Rectangle(0, 0, 590, 420));
            PdfLayer imageLayer1 = new PdfLayer("*SomeTest_image$here@.1", pdfDoc);
            imageLayer1.SetOn(true);
            pdfCanvas.BeginLayer(imageLayer1);
            canvas1.Add(image1);
            pdfCanvas.EndLayer();
            canvas1.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outFileName));
        }
        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    }
}
