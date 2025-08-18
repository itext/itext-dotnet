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
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RectangleIntegrationTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/forms/RectangleTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/RectangleTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void InitDestinationFolder() {
            ITextTest.CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfWithSignatureFields() {
            String outPdf = DESTINATION_FOLDER + "RectangleTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_RectangleTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "RectangleTest.pdf");
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            for (int i = 0; i <= 3; i++) {
                int rotation = 90 * i;
                PdfPage page = pdfDoc.AddNewPage();
                page.SetRotation(rotation);
                float x = 20;
                float y = 500;
                float width = 100;
                float height = 50;
                float spacing = 50;
                for (int j = 1; j <= 3; j++) {
                    Rectangle rect = new Rectangle(x, y, width, height);
                    String fieldName = "page" + i + "_Signature" + j;
                    PdfFormField signatureField = new SignatureFormFieldBuilder(pdfDoc, fieldName).SetPage(page).SetWidgetRectangle
                        (rect).CreateSignature();
                    form.AddField(signatureField);
                    x += width + spacing;
                }
            }
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
