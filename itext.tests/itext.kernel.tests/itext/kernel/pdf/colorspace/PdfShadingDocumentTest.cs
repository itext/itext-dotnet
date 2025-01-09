/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace.Shading;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfShadingDocumentTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/colorspace/PdfShadingDocumentTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AxialDocumentTest() {
            String dest = DESTINATION_FOLDER + "axialDoc.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest))) {
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(pdfPage);
                int x = 36;
                int y = 400;
                int side = 500;
                float[] green = new float[] { 0, 255, 0 };
                float[] blue = new float[] { 0, 0, 255 };
                PdfAxialShading axial = new PdfAxialShading(new PdfDeviceCs.Rgb(), x, y, green, x + side, y, blue);
                PdfPattern.Shading shading = new PdfPattern.Shading(axial);
                canvas.SetFillColorShading(shading);
                canvas.MoveTo(x, y);
                canvas.LineTo(x + side, y);
                canvas.LineTo(x + (side / 2), (float)(y + (side * Math.Sin(Math.PI / 3))));
                canvas.ClosePathFillStroke();
                PdfDictionary pdfObject = pdfDocument.GetPage(1).GetResources().GetPdfObject();
                NUnit.Framework.Assert.IsTrue(pdfObject.ContainsKey(PdfName.Pattern));
            }
        }
    }
}
