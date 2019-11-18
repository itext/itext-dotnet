/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfFormFieldsHierarchyTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldsHierarchyTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldsHierarchyTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FillingFormWithKidsTest() {
            String srcPdf = sourceFolder + "formWithKids.pdf";
            String cmpPdf = sourceFolder + "cmp_fillingFormWithKidsTest.pdf";
            String outPdf = destinationFolder + "fillingFormWithKidsTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            IDictionary<String, PdfFormField> formFields = acroForm.GetFormFields();
            foreach (String key in formFields.Keys) {
                PdfFormField field = acroForm.GetField(key);
                field.SetValue(key);
            }
            pdfDocument.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
