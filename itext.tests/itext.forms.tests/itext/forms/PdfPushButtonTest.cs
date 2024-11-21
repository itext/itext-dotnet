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
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfPushButtonTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfPushButtonTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfPushButtonTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SetFontSizePushButtonWithDisplayTest() {
            String outPdf = DESTINATION_FOLDER + "pushButtonWithDisplay.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_pushButtonWithDisplay.pdf";
            using (PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                doc.AddNewPage();
                Rectangle rectangle = new Rectangle(150, 400, 400, 100);
                PdfButtonFormField button = new PushButtonFormFieldBuilder(doc, "button").SetWidgetRectangle(rectangle).SetPage
                    (1).CreatePushButton();
                button.SetFontSize(50);
                button.SetValue("value", "some display text");
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                acroForm.AddField(button);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
