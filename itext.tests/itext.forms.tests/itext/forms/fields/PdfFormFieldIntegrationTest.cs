/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFormFieldIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/fields/PdfFormFieldIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/fields/PdfFormFieldIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectToPushButtonTest() {
            String outPdf = DESTINATION_FOLDER + "addFormXObjectToPushButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_addFormXObjectToPushButton.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "addFormXObjectToPushButton.pdf"), new 
                PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetPage(2);
            PdfFormXObject xObject = page.CopyAsFormXObject(pdfDoc);
            ((PdfButtonFormField)form.GetField("pushButton")).SetImageAsForm(xObject);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
