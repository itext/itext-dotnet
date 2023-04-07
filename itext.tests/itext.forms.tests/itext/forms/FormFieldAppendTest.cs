/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FormFieldAppendTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FormFieldAppendTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FormFieldAppendTest/";

        private static bool experimentalRenderingPreviousValue;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
            experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
        }

        [NUnit.Framework.Test]
        public virtual void FormFillingAppend_form_empty_Test() {
            String srcFilename = sourceFolder + "Form_Empty.pdf";
            String temp = destinationFolder + "temp_empty.pdf";
            String filename = destinationFolder + "formFillingAppend_form_empty.pdf";
            StampingProperties props = new StampingProperties();
            props.UseAppendMode();
            PdfDocument doc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(temp), props);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                field.SetValue("Test");
            }
            doc.Close();
            Flatten(temp, filename);
            FileInfo toDelete = new FileInfo(temp);
            toDelete.Delete();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFillingAppend_form_empty.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormFillingAppend_form_filled_Test() {
            String srcFilename = sourceFolder + "Form_Empty.pdf";
            String temp = destinationFolder + "temp_filled.pdf";
            String filename = destinationFolder + "formFillingAppend_form_filled.pdf";
            StampingProperties props = new StampingProperties();
            props.UseAppendMode();
            PdfDocument doc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(temp), props);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                field.SetValue("Different");
            }
            doc.Close();
            Flatten(temp, filename);
            new FileInfo(temp).Delete();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFillingAppend_form_filled.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        private void Flatten(String src, String dest) {
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            form.FlattenFields();
            doc.Close();
        }
    }
}
