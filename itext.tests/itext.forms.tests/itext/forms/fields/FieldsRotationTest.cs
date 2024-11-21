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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FieldsRotationTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/fields/FieldsRotationTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/fields/FieldsRotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> RotationRelatedProperties() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { new int[] { 360, 90, 180, 270 }, new int[] { 
                0, 0, 0, 0 }, true, "fieldsOnRotatedPagesDefault" }, new Object[] { new int[] { 360, 90, 180, 270 }, new 
                int[] { 0, 0, 0, 0 }, false, "fieldsOnRotatedPages" }, new Object[] { new int[] { 0, 0, 0, 0 }, new int
                [] { 360, 90, 180, 270 }, true, "rotatedFieldsDefault" }, new Object[] { new int[] { 90, 90, 90, 90 }, 
                new int[] { 720, 90, 180, 270 }, true, "rotatedFieldsPage90Default" }, new Object[] { new int[] { 90, 
                90, 90, 90 }, new int[] { 0, -270, 180, -90 }, false, "rotatedFieldsPage90" }, new Object[] { new int[
                ] { 0, 90, 180, 270 }, new int[] { 0, 90, 180, 270 }, true, "rotatedFieldsOnRotatedPagesDefault" }, new 
                Object[] { new int[] { 0, 90, 180, 270 }, new int[] { 0, 90, 180, 270 }, false, "rotatedFieldsOnRotatedPages"
                 } });
        }

        [NUnit.Framework.TestCaseSource("RotationRelatedProperties")]
        public virtual void FieldRotationTest(int[] pageRotation, int[] fieldRotation, bool ignorePageRotation, String
             testName) {
            String outFileName = DESTINATION_FOLDER + testName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + testName + ".pdf";
            FillForm(pageRotation, fieldRotation, ignorePageRotation, outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private void FillForm(int[] pageRotation, int[] fieldRotation, bool ignorePageRotation, String outPdf) {
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document.GetPdfDocument(), true);
                for (int i = 1; i < 5; ++i) {
                    String caption = GenerateCaption(pageRotation[i - 1], fieldRotation[i - 1]);
                    document.GetPdfDocument().AddNewPage().SetRotation(pageRotation[i - 1]);
                    String buttonName = "button" + i;
                    PdfButtonFormField button = new PushButtonFormFieldBuilder(document.GetPdfDocument(), buttonName).SetWidgetRectangle
                        (new Rectangle(50, 570, 400, 200)).SetPage(i).CreatePushButton();
                    Button buttonField = new Button(buttonName);
                    buttonField.SetValue("button" + caption);
                    button.GetFirstFormAnnotation().SetFormFieldElement(buttonField).SetBorderColor(ColorConstants.GREEN).SetRotation
                        (fieldRotation[i - 1]);
                    form.AddField(button);
                    String textName = "text" + i;
                    PdfTextFormField text = new TextFormFieldBuilder(document.GetPdfDocument(), textName).SetWidgetRectangle(new 
                        Rectangle(50, 320, 400, 200)).SetPage(i).CreateText();
                    text.GetFirstFormAnnotation().SetBorderColor(ColorConstants.GREEN).SetRotation(fieldRotation[i - 1]);
                    form.AddField(text);
                    String signatureName = "signature" + i;
                    PdfSignatureFormField signature = new SignatureFormFieldBuilder(document.GetPdfDocument(), signatureName).
                        SetWidgetRectangle(new Rectangle(50, 70, 400, 200)).SetPage(i).CreateSignature();
                    SignatureFieldAppearance sigField = new SignatureFieldAppearance(signatureName).SetContent("signature" + caption
                        );
                    signature.SetIgnorePageRotation(ignorePageRotation).GetFirstFormAnnotation().SetFormFieldElement(sigField)
                        .SetBorderColor(ColorConstants.GREEN).SetRotation(fieldRotation[i - 1]);
                    form.AddField(signature);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillRotated90TextFormFieldTest() {
            String inPdf = SOURCE_FOLDER + "rotated90TextFormField.pdf";
            String outPdf = DESTINATION_FOLDER + "filledRotated90TextFormField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_filledRotated90TextFormField.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPdf), CompareTool.CreateTestPdfWriter(outPdf))
                ) {
                IDictionary<String, String> fieldValues = new Dictionary<String, String>();
                fieldValues.Put("textForm", "some text");
                FillAndFlattenForm(pdfDoc, fieldValues);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FillRotated270TextFormFieldTest() {
            String inPdf = SOURCE_FOLDER + "rotated270TextFormField.pdf";
            String outPdf = DESTINATION_FOLDER + "filledRotated270TextFormField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_filledRotated270TextFormField.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPdf), CompareTool.CreateTestPdfWriter(outPdf))
                ) {
                IDictionary<String, String> fieldValues = new Dictionary<String, String>();
                fieldValues.Put("textForm", "some text");
                FillAndFlattenForm(pdfDoc, fieldValues);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedButtonWithDisplayValueTest() {
            String outPdf = DESTINATION_FOLDER + "rotatedButtonWithDisplayValue.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_rotatedButtonWithDisplayValue.pdf";
            using (PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                doc.AddNewPage();
                Rectangle rectangle = new Rectangle(150, 300, 150, 400);
                PdfDictionary chars = new PdfDictionary();
                chars.Put(PdfName.R, new PdfNumber(90));
                PdfButtonFormField button = new PushButtonFormFieldBuilder(doc, "button").SetWidgetRectangle(rectangle).SetPage
                    (1).CreatePushButton();
                button.GetWidgets()[0].SetAppearanceCharacteristics(chars);
                button.SetFontSize(0);
                button.SetValue("value", "this text should take all space");
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                acroForm.AddField(button);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedCheckBoxTest() {
            String outPdf = DESTINATION_FOLDER + "rotatedCheckBox.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_rotatedCheckBox.pdf";
            using (PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                doc.AddNewPage();
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                PdfDictionary chars = new PdfDictionary();
                chars.Put(PdfName.R, new PdfNumber(90));
                Rectangle rectangle1 = new Rectangle(0, 300, 100, 100);
                PdfButtonFormField box1 = new CheckBoxFormFieldBuilder(doc, "checkBox1").SetPage(1).SetWidgetRectangle(rectangle1
                    ).CreateCheckBox();
                box1.GetWidgets()[0].SetAppearanceCharacteristics(chars);
                box1.SetCheckType(CheckBoxType.CHECK);
                box1.SetValue("ON");
                box1.SetFontSize(0);
                acroForm.AddField(box1);
                Rectangle rectangle2 = new Rectangle(100, 300, 100, 100);
                PdfButtonFormField box2 = new CheckBoxFormFieldBuilder(doc, "checkBox2").SetPage(1).SetWidgetRectangle(rectangle2
                    ).CreateCheckBox();
                box2.GetWidgets()[0].SetAppearanceCharacteristics(chars);
                box2.SetCheckType(CheckBoxType.STAR);
                box2.SetValue("ON");
                box2.SetFontSize(0);
                acroForm.AddField(box2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedChoiceBoxTest() {
            String outPdf = DESTINATION_FOLDER + "rotatedChoiceBox.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_rotatedChoiceBox.pdf";
            using (PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                doc.AddNewPage();
                Rectangle rectangle = new Rectangle(150, 500, 200, 150);
                PdfDictionary chars = new PdfDictionary();
                chars.Put(PdfName.R, new PdfNumber(90));
                PdfChoiceFormField choiceBoxField = new ChoiceFormFieldBuilder(doc, "choiceBox").SetPage(1).SetWidgetRectangle
                    (rectangle).SetOptions(new String[] { "option1", "option2", "option3", "option4", "option5" }).CreateList
                    ();
                choiceBoxField.GetWidgets()[0].SetAppearanceCharacteristics(chars);
                choiceBoxField.SetFontSize(0);
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                acroForm.AddField(choiceBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedComboBoxTest() {
            String outPdf = DESTINATION_FOLDER + "rotatedComboBox.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_rotatedComboBox.pdf";
            using (PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                doc.AddNewPage();
                Rectangle rectangle = new Rectangle(150, 500, 75, 150);
                PdfDictionary chars = new PdfDictionary();
                chars.Put(PdfName.R, new PdfNumber(90));
                PdfChoiceFormField choiceBoxField = new ChoiceFormFieldBuilder(doc, "choiceBox").SetPage(1).SetWidgetRectangle
                    (rectangle).SetOptions(new String[] { "option1", "option2", "longOption", "option3", "option4", "option5"
                     }).CreateComboBox();
                choiceBoxField.GetWidgets()[0].SetAppearanceCharacteristics(chars);
                choiceBoxField.SetValue("option1", true);
                choiceBoxField.SetFontSize(0);
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                acroForm.AddField(choiceBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        private static void FillAndFlattenForm(PdfDocument document, IDictionary<String, String> fields) {
            PdfAcroForm form = PdfAcroForm.GetAcroForm(document, true);
            form.SetNeedAppearances(true);
            foreach (KeyValuePair<String, String> field in fields) {
                PdfFormField acroField = form.GetField(field.Key);
                acroField.SetValue(field.Value);
            }
            form.FlattenFields();
        }

        private String GenerateCaption(int pageRotation, int fieldRotation) {
            String caption = ", page rotation: " + pageRotation + ", field rotation: " + fieldRotation;
            for (int i = 0; i < 3; ++i) {
                caption += caption;
            }
            return caption;
        }
    }
}
