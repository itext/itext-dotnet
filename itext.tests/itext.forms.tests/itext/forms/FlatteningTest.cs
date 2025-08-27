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
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FlatteningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FlatteningTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/forms/FlatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FlatteningFormFieldNoSubtypeInAPTest() {
            String src = sourceFolder + "formFieldNoSubtypeInAPTest.pdf";
            String dest = destinationFolder + "flatteningFormFieldNoSubtypeInAPTest.pdf";
            String cmp = sourceFolder + "cmp_flatteningFormFieldNoSubtypeInAPTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfFormCreator.GetAcroForm(doc, false).FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlatteningPdfWithButtons() {
            String src = sourceFolder + "flatteningPdfWithButtons.pdf";
            String dest = destinationFolder + "flatteningPdfWithButtonsOutput.pdf";
            String cmp = sourceFolder + "cmp_flatteningPdfWithButtons.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfFont font = PdfFontFactory.CreateFont();
                PdfFormField field = form.GetField("myPushButton");
                field.SetValue("push button", font, 12);
                field.RegenerateField();
                PdfFormField field2 = form.GetField("myCheckBox");
                field2.SetValue("check box", font, 12);
                field2.RegenerateField();
                RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, "answer");
                PdfButtonFormField radioGroup = builder.CreateRadioGroup();
                radioGroup.SetValue("answer 1");
                form.AddField(radioGroup);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlatteningPdfWithFields() {
            String src = sourceFolder + "flatteningPdfWithFields.pdf";
            String dest = destinationFolder + "flatteningPdfWithFields.pdf";
            String cmp = sourceFolder + "cmp_flatteningPdfWithFields.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfFont font = PdfFontFactory.CreateFont();
                IDictionary<PdfName, PdfObject> appearance = new Dictionary<PdfName, PdfObject>();
                appearance.Put(PdfName.CA, new PdfString("wrong text"));
                PdfFormField inputField = form.GetField("inputField");
                inputField.GetPdfObject().Put(PdfName.MK, new PdfDictionary(appearance));
                inputField.SetValue("input field regenerated", font, 12);
                inputField.RegenerateField();
                PdfFormField comboBoxField = form.GetField("comboBoxField");
                inputField.GetPdfObject().Put(PdfName.MK, new PdfDictionary(appearance));
                comboBoxField.SetValue("Red", font, 12);
                comboBoxField.RegenerateField();
                PdfFormField textAreaField = form.GetField("textAreaField");
                textAreaField.GetPdfObject().Put(PdfName.MK, new PdfDictionary(appearance));
                textAreaField.SetValue("text area field regenerated", font, 12);
                textAreaField.RegenerateField();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.N_ENTRY_IS_REQUIRED_FOR_APPEARANCE_DICTIONARY)]
        public virtual void FormFlatteningTestWithoutNEntry() {
            String filename = "formFlatteningTestWithoutNEntry";
            String src = sourceFolder + filename + ".pdf";
            String dest = destinationFolder + filename + "_flattened.pdf";
            String cmp = sourceFolder + "cmp_" + filename + "_flattened.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, false);
            acroForm.SetGenerateAppearance(false);
            acroForm.FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void HiddenFieldsFlatten() {
            //TODO: Adapt assertion after DEVSIX-3079 is fixed
            String filename = "hiddenField";
            String src = sourceFolder + filename + ".pdf";
            String dest = destinationFolder + filename + "_flattened.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, true);
            acroForm.GetField("hiddenField").GetPdfObject().Put(PdfName.F, new PdfNumber(2));
            acroForm.FlattenFields();
            String textAfterFlatten = PdfTextExtractor.GetTextFromPage(document.GetPage(1));
            document.Close();
            NUnit.Framework.Assert.IsTrue(textAfterFlatten.Contains("hiddenFieldValue"), "Pdf does not contain the expected text"
                );
        }
    }
}
