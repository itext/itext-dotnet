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
using iText.Forms.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAcroFormIntegrationTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfAcroFormIntegrationTest/";

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.CANNOT_CREATE_FORMFIELD)]
        public virtual void OrphandNamelessFormFieldTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "orphanedFormField.pdf"))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                // 2 out of 3 have been gathered
                NUnit.Framework.Assert.AreEqual(2, form.GetDirectFormFields().Count);
            }
        }
    }
}
