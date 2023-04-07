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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFormFieldTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        // The first message for the case when the FormField is null,
        // the second message when the FormField is an indirect reference to null.
        [LogMessage(FormsLogMessageConstants.CANNOT_CREATE_FORMFIELD, Count = 2)]
        public virtual void NullFormFieldTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "nullFormField.pdf"));
            PdfAcroForm.GetAcroForm(pdfDoc, true);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldTest01() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            PdfFormField field = fields.Get("Text1");
            NUnit.Framework.Assert.AreEqual(4, fields.Count);
            NUnit.Framework.Assert.AreEqual("Text1", field.GetFieldName().ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("TestField", field.GetValue().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldTest02() {
            String filename = destinationFolder + "formFieldTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 490, 150, 22);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "fieldName").SetWidgetRectangle(rect).CreateText
                ();
            field.SetValue("some value");
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFieldTest02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldTest03() {
            String filename = destinationFolder + "formFieldTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"), new PdfWriter(filename
                ));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetFirstPage();
            Rectangle rect = new Rectangle(210, 490, 150, 22);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(rect).CreateText
                ();
            field.SetValue("some value");
            form.AddField(field, page);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFieldTest03.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldTest04() {
            String filename = destinationFolder + "formFieldTest04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"), new PdfWriter(filename
                ));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetFirstPage();
            Rectangle rect = new Rectangle(210, 490, 150, 22);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(rect).CreateText
                ();
            field.SetValue("some value in courier font").SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER)).SetFontSize
                (10);
            form.AddField(field, page);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFieldTest04.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldWithFloatBorderTest() {
            String filename = destinationFolder + "formFieldWithFloatBorder.pdf";
            String cmpFilename = sourceFolder + "cmp_formFieldWithFloatBorder.pdf";
            // In this test it's important to open the document in the acrobat and make sure that border width
            // does not change after clicking on the field. Acrobat doesn't support float border width therefore we round it
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(filename))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);
                PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDocument, "text field").SetWidgetRectangle(new 
                    Rectangle(100, 600, 100, 100)).CreateText();
                textFormField.SetValue("text field value");
                textFormField.GetFirstFormAnnotation().SetBorderWidth(5.25f);
                textFormField.GetFirstFormAnnotation().SetBorderColor(ColorConstants.RED);
                form.AddField(textFormField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TextFieldLeadingSpacesAreNotTrimmedTest() {
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String filename = destinationFolder + "textFieldLeadingSpacesAreNotTrimmed.pdf";
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
                pdfDoc.AddNewPage();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfPage page = pdfDoc.GetFirstPage();
                Rectangle rect = new Rectangle(210, 490, 300, 22);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(rect).CreateText
                    ();
                field.SetValue("        value with leading space");
                form.AddField(field, page);
                pdfDoc.Close();
                CompareTool compareTool = new CompareTool();
                String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_textFieldLeadingSpacesAreNotTrimmed.pdf"
                    , destinationFolder, "diff_");
                if (errorMessage != null) {
                    NUnit.Framework.Assert.Fail(errorMessage);
                }
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void UnicodeFormFieldTest() {
            String filename = sourceFolder + "unicodeFormFieldFile.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> formFields = form.GetAllFormFields();
            // 帐号1: account number 1
            String fieldName = "\u5E10\u53F71";
            NUnit.Framework.Assert.AreEqual(fieldName, formFields.Keys.ToArray(new String[1])[0]);
        }

        [NUnit.Framework.Test]
        public virtual void UnicodeFormFieldTest2() {
            String filename = sourceFolder + "unicodeFormFieldFile.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            // 帐号1: account number 1
            String fieldName = "\u5E10\u53F71";
            NUnit.Framework.Assert.IsNotNull(form.GetField(fieldName));
        }

        [NUnit.Framework.Test]
        public virtual void TextFieldValueInStreamTest() {
            String filename = sourceFolder + "textFieldValueInStream.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String fieldValue = form.GetField("fieldName").GetValueAsString();
            // Trailing newline is not trimmed which seems to match Acrobat's behavior on copy-paste
            NUnit.Framework.Assert.AreEqual("some value\n", fieldValue);
        }

        [NUnit.Framework.Test]
        public virtual void ChoiceFieldTest01() {
            String filename = destinationFolder + "choiceFieldTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 490, 150, 20);
            String[] options = new String[] { "First Item", "Second Item", "Third Item", "Fourth Item" };
            PdfChoiceFormField choice = new ChoiceFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(rect).SetOptions
                (options).CreateComboBox();
            choice.SetValue("First Item", true);
            form.AddField(choice);
            Rectangle rect1 = new Rectangle(210, 250, 150, 90);
            PdfChoiceFormField choice1 = new ChoiceFormFieldBuilder(pdfDoc, "TestField1").SetWidgetRectangle(rect1).SetOptions
                (options).CreateList();
            choice1.SetValue("Second Item", true);
            choice1.SetMultiSelect(true);
            form.AddField(choice1);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_choiceFieldTest01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ButtonFieldTest01() {
            String filename = destinationFolder + "buttonFieldTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(36, 700, 20, 20);
            Rectangle rect1 = new Rectangle(36, 680, 20, 20);
            String formFieldName = "TestGroup";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, formFieldName);
            PdfButtonFormField group = builder.CreateRadioGroup();
            group.SetValue("1", true);
            PdfFormAnnotation radio1 = builder.CreateRadioButton("1", rect);
            PdfFormAnnotation radio2 = builder.CreateRadioButton("2", rect1);
            group.AddKid(radio1);
            group.AddKid(radio2);
            form.AddField(group);
            PdfButtonFormField pushButton = new PushButtonFormFieldBuilder(pdfDoc, "push").SetWidgetRectangle(new Rectangle
                (36, 650, 40, 20)).SetCaption("Capcha").CreatePushButton();
            PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(pdfDoc, "TestCheck").SetWidgetRectangle(new Rectangle
                (36, 560, 20, 20)).CreateCheckBox();
            checkBox.SetValue("1", true);
            form.AddField(pushButton);
            form.AddField(checkBox);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_buttonFieldTest01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultRadiobuttonFieldTest() {
            String file = "defaultRadiobuttonFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            String formFieldName = "TestGroup";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, formFieldName);
            PdfButtonFormField group = builder.CreateRadioGroup();
            group.SetValue("1", true);
            group.AddKid(builder.CreateRadioButton("1", rect1));
            group.AddKid(builder.CreateRadioButton("2", rect2));
            form.AddField(group);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedRadiobuttonFieldTest() {
            String file = "customizedRadiobuttonFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            String formFieldName2 = "TestGroup2";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, formFieldName2);
            PdfButtonFormField group2 = builder.CreateRadioGroup();
            group2.SetValue("1", true);
            PdfFormAnnotation radio1 = builder.CreateRadioButton("1", rect1).SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            group2.AddKid(radio1);
            PdfFormAnnotation radio2 = new RadioFormFieldBuilder(pdfDoc, formFieldName2).CreateRadioButton("2", rect2)
                .SetBorderWidth(2).SetBorderColor(ColorConstants.RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility
                (PdfFormAnnotation.VISIBLE);
            group2.AddKid(radio2);
            form.AddField(group2);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedRadiobuttonWithGroupRegeneratingFieldTest() {
            String file = "customizedRadiobuttonWithGroupRegeneratingFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            String formFieldName2 = "TestGroup2";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, formFieldName2);
            PdfButtonFormField group2 = builder.CreateRadioGroup();
            group2.SetValue("1", true);
            PdfFormAnnotation radio1 = builder.CreateRadioButton("1", rect1).SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            PdfFormAnnotation radio2 = builder.CreateRadioButton("2", rect2).SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            group2.AddKid(radio1);
            group2.AddKid(radio2);
            group2.RegenerateField();
            form.AddField(group2);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedPushButtonFieldTest() {
            String file = "customizedPushButtonFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String itext = "itextpdf";
            PdfButtonFormField button = new PushButtonFormFieldBuilder(pdfDoc, itext).SetWidgetRectangle(new Rectangle
                (36, 500, 200, 200)).SetCaption(itext).CreatePushButton();
            button.SetFontSize(0);
            button.SetValue(itext);
            button.GetFirstFormAnnotation().SetBorderWidth(10).SetBorderColor(ColorConstants.GREEN).SetBackgroundColor
                (ColorConstants.GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            form.AddField(button);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedPushButtonField2Test() {
            String file = "customizedPushButtonField2Test.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String itext = "itextpdf";
            PdfButtonFormField button = new PushButtonFormFieldBuilder(pdfDoc, itext).SetWidgetRectangle(new Rectangle
                (36, 500, 300, 110)).SetCaption(itext).CreatePushButton();
            button.SetFontSize(0);
            button.SetValue(itext);
            button.GetFirstFormAnnotation().SetBorderWidth(10).SetBorderColor(ColorConstants.GREEN).SetBackgroundColor
                (ColorConstants.GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            form.AddField(button);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedPushButtonField3Test() {
            String file = "customizedPushButtonField3Test.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String text = "toolongtext";
            PdfButtonFormField button = new PushButtonFormFieldBuilder(pdfDoc, text).SetWidgetRectangle(new Rectangle(
                36, 500, 160, 300)).SetCaption(text).CreatePushButton();
            button.SetFontSize(40);
            button.SetValue(text);
            button.GetFirstFormAnnotation().SetBorderWidth(10).SetBorderColor(ColorConstants.GREEN).SetBackgroundColor
                (ColorConstants.GRAY).SetVisibility(PdfFormAnnotation.VISIBLE);
            form.AddField(button);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ButtonFieldTest02() {
            String filename = destinationFolder + "buttonFieldTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "buttonFieldTest02_input.pdf"), new PdfWriter
                (filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            ((PdfButtonFormField)form.GetField("push")).SetImage(sourceFolder + "Desert.jpg");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_buttonFieldTest02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RealFontSizeRegenerateAppearanceTest() {
            String sourceFilename = sourceFolder + "defaultAppearanceRealFontSize.pdf";
            String destFilename = destinationFolder + "realFontSizeRegenerateAppearance.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFilename), new PdfWriter(destFilename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.GetField("fieldName").RegenerateField();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destFilename, sourceFolder + "cmp_realFontSizeRegenerateAppearance.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldWithKidsTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField root = new NonTerminalFormFieldBuilder(pdfDoc, "root").CreateNonTerminalFormField();
            PdfFormField child = new NonTerminalFormFieldBuilder(pdfDoc, "child").CreateNonTerminalFormField();
            root.AddKid(child);
            PdfTextFormField text1 = new TextFormFieldBuilder(pdfDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                , 200, 20)).CreateText();
            text1.SetValue("test");
            child.AddKid(text1);
            form.AddField(root);
            NUnit.Framework.Assert.AreEqual(3, form.GetAllFormFields().Count);
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithDefaultResources() {
            String outPdf = destinationFolder + "fillFormWithDefaultResources.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithDefaultResources.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "formWithDefaultResources.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            PdfFormField field = fields.Get("Text1");
            field.SetValue("New value size must be 8");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFormTwiceWithoutResources() {
            String outPdf = destinationFolder + "fillFormWithoutResources.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithoutResources.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "formWithoutResources.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            PdfFormField field = fields.Get("Text1");
            field.SetValue("New value size must be 8").SetFontSize(8);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AutoScaleFontSizeInFormFields() {
            String outPdf = destinationFolder + "autoScaleFontSizeInFormFields.pdf";
            String cmpPdf = sourceFolder + "cmp_autoScaleFontSizeInFormFields.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField field = new TextFormFieldBuilder(pdfDoc, "name").SetWidgetRectangle(new Rectangle(36, 786, 80
                , 20)).CreateText().SetValue("TestValueAndALittleMore");
            field.SetFontSizeAutoScale();
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.NO_FIELDS_IN_ACROFORM)]
        public virtual void AcroFieldDictionaryNoFields() {
            String outPdf = destinationFolder + "acroFieldDictionaryNoFields.pdf";
            String cmpPdf = sourceFolder + "cmp_acroFieldDictionaryNoFields.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "acroFieldDictionaryNoFields.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm.GetAcroForm(pdfDoc, true);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-7264: Investigate 3 failed forms tests from 7.3/develop on .NET")]
        public virtual void RegenerateAppearance() {
            String input = "regenerateAppearance.pdf";
            String output = "regenerateAppearance.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + input), new PdfWriter(destinationFolder
                 + output), new StampingProperties().UseAppendMode());
            PdfAcroForm acro = PdfAcroForm.GetAcroForm(document, false);
            int i = 1;
            foreach (KeyValuePair<String, PdfFormField> entry in acro.GetAllFormFields()) {
                if (entry.Key.Contains("field")) {
                    PdfFormField field = entry.Value;
                    field.SetValue("test" + i++, false);
                }
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + output, sourceFolder 
                + "cmp_" + output, destinationFolder, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RegenerateAppearance2() {
            String input = "regenerateAppearance2.pdf";
            String output = "regenerateAppearance2.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + input), new PdfWriter(destinationFolder
                 + output), new StampingProperties().UseAppendMode());
            PdfAcroForm acro = PdfAcroForm.GetAcroForm(document, false);
            acro.SetNeedAppearances(true);
            PdfFormField field = acro.GetField("number");
            field.SetValue("20150044DR");
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + output, sourceFolder 
                + "cmp_" + output, destinationFolder, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlushedPagesTest() {
            String filename = destinationFolder + "flushedPagesTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.AddNewPage().Flush();
            pdfDoc.AddNewPage().Flush();
            pdfDoc.AddNewPage();
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "name").SetWidgetRectangle(new Rectangle(100, 100
                , 300, 20)).CreateText();
            field.SetValue("");
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_flushedPagesTest.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithDefaultResourcesUpdateFont() {
            String outPdf = destinationFolder + "fillFormWithDefaultResourcesUpdateFont.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithDefaultResourcesUpdateFont.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "formWithDefaultResources.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            PdfFormField field = fields.Get("Text1");
            field.SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER));
            field.SetValue("New value size must be 8, but with different font.");
            new Canvas(new PdfCanvas(pdfDoc.GetFirstPage()), new Rectangle(30, 500, 500, 200)).Add(new Paragraph("The text font after modification it via PDF viewer (e.g. Acrobat) shall be preserved."
                ));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormRegenerateWithInvalidDefaultAppearance01() {
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String testName = "formRegenerateWithInvalidDefaultAppearance01";
                String outPdf = destinationFolder + testName + ".pdf";
                String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
                String srcPdf = sourceFolder + "invalidDA.pdf";
                PdfWriter writer = new PdfWriter(outPdf);
                PdfReader reader = new PdfReader(srcPdf);
                PdfDocument pdfDoc = new PdfDocument(reader, writer);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
                fields.Get("Text1").SetValue("New field value");
                fields.Get("Text2").SetValue("New field value");
                fields.Get("Text3").SetValue("New field value");
                pdfDoc.Close();
                CompareTool compareTool = new CompareTool();
                String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
                if (errorMessage != null) {
                    NUnit.Framework.Assert.Fail(errorMessage);
                }
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase1() {
            //Create a document with formfields and paragraphs in both fonts, and fill them before closing the document
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String testName = "fillFieldWithHebrewCase1";
                String outPdf = destinationFolder + testName + ".pdf";
                String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
                PdfWriter writer = new PdfWriter(outPdf);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document document = new Document(pdfDoc);
                PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                    );
                hebrew.SetSubset(false);
                PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H);
                sileot.SetSubset(false);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                String text = "שלום וברכה";
                CreateAcroForm(pdfDoc, form, hebrew, text, 0);
                CreateAcroForm(pdfDoc, form, sileot, text, 3);
                AddParagraph(document, text, hebrew);
                AddParagraph(document, text, sileot);
                pdfDoc.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                     + testName + "_"));
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase2() {
            //Create a document with formfields and paragraphs in both fonts, and fill them after closing and reopening the document
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String testName = "fillFieldWithHebrewCase2";
                String outPdf = destinationFolder + testName + ".pdf";
                String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
                ByteArrayOutputStream baos = new ByteArrayOutputStream();
                PdfWriter writer = new PdfWriter(baos);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document document = new Document(pdfDoc);
                PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                    );
                hebrew.SetSubset(false);
                PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H);
                sileot.SetSubset(false);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                CreateAcroForm(pdfDoc, form, hebrew, null, 0);
                CreateAcroForm(pdfDoc, form, sileot, null, 3);
                String text = "שלום וברכה";
                AddParagraph(document, text, hebrew);
                AddParagraph(document, text, sileot);
                pdfDoc.Close();
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new PdfWriter(outPdf
                    ));
                FillAcroForm(pdfDocument, text);
                pdfDocument.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                     + testName + "_"));
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase3() {
            //Create a document with formfields in both fonts, and fill them before closing the document
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String testName = "fillFieldWithHebrewCase3";
                String outPdf = destinationFolder + testName + ".pdf";
                String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
                PdfWriter writer = new PdfWriter(outPdf);
                PdfDocument pdfDoc = new PdfDocument(writer);
                PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                    );
                hebrew.SetSubset(false);
                PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H);
                sileot.SetSubset(false);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                String text = "שלום וברכה";
                CreateAcroForm(pdfDoc, form, hebrew, text, 0);
                CreateAcroForm(pdfDoc, form, sileot, text, 3);
                pdfDoc.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                     + testName + "_"));
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase4() {
            //Create a document with formfields in both fonts, and fill them after closing and reopening the document
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String testName = "fillFieldWithHebrewCase4";
                String outPdf = destinationFolder + testName + ".pdf";
                String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
                ByteArrayOutputStream baos = new ByteArrayOutputStream();
                PdfWriter writer = new PdfWriter(baos);
                PdfDocument pdfDoc = new PdfDocument(writer);
                PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                    );
                hebrew.SetSubset(false);
                PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H);
                sileot.SetSubset(false);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                CreateAcroForm(pdfDoc, form, hebrew, null, 0);
                CreateAcroForm(pdfDoc, form, sileot, null, 3);
                pdfDoc.Close();
                String text = "שלום וברכה";
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new PdfWriter(outPdf
                    ));
                FillAcroForm(pdfDocument, text);
                pdfDocument.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                     + testName + "_"));
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithSameEmptyObjsForAppearance() {
            String outPdf = destinationFolder + "fillFormWithSameEmptyObjsForAppearance.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithSameEmptyObjsForAppearance.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "fillFormWithSameEmptyObjsForAppearance.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, false);
            acroForm.GetField("text_1").SetValue("Text 1!");
            acroForm.GetField("text_2").SetValue("Text 2!");
            acroForm.GetField("text.3").SetValue("Text 3!");
            acroForm.GetField("text.4").SetValue("Text 4!");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DashedBorderAppearanceTest() {
            String outPdf = destinationFolder + "dashedBorderAppearanceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_dashedBorderAppearanceTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField[] fields = new PdfTextFormField[3];
            String[] names = new String[] { "fieldNoPattern", "fieldEmptyPattern", "fieldSingleEntryPattern" };
            float y = 830;
            PdfDictionary borderDict = new PdfDictionary();
            borderDict.Put(PdfName.S, PdfName.D);
            PdfArray patternArray = new PdfArray();
            for (int i = 0; i < 3; i++) {
                if (i == 2) {
                    patternArray.Add(new PdfNumber(10));
                }
                if (i > 0) {
                    borderDict.Put(PdfName.D, patternArray);
                }
                fields[i] = new TextFormFieldBuilder(pdfDoc, names[i]).SetWidgetRectangle(new Rectangle(10, y -= 70, 200, 
                    50)).CreateText();
                fields[i].SetValue(names[i]);
                acroForm.AddField(fields[i]);
                fields[i].GetFirstFormAnnotation().SetBorderStyle(borderDict);
                fields[i].GetFirstFormAnnotation().SetBorderWidth(3);
                fields[i].GetFirstFormAnnotation().SetBorderColor(ColorConstants.CYAN);
                fields[i].GetFirstFormAnnotation().SetBackgroundColor(ColorConstants.MAGENTA);
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT)]
        public virtual void NoMaxLenWithSetCombFlagTest() {
            String outPdf = destinationFolder + "noMaxLenWithSetCombFlagTest.pdf";
            String cmpPdf = sourceFolder + "cmp_noMaxLenWithSetCombFlagTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField textField = new TextFormFieldBuilder(pdfDoc, "text").SetWidgetRectangle(new Rectangle(100
                , 500, 200, 200)).CreateText();
            textField.SetComb(true);
            // The line below should throw an exception, because the Comb flag may be set only if the MaxLen entry is present in the text field dictionary
            textField.SetValue("12345678");
            textField.SetMaxLen(1);
            form.AddField(textField);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MaxLenWithSetCombFlagAppearanceTest() {
            String srcPdf = sourceFolder + "maxLenFields.pdf";
            String outPdf = destinationFolder + "maxLenWithSetCombFlagAppearanceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_maxLenWithSetCombFlagAppearanceTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            form.GetField("text1").SetValue("123");
            form.GetField("text2").SetJustification(TextAlignment.CENTER).SetValue("123");
            form.GetField("text3").SetJustification(TextAlignment.RIGHT).SetValue("123");
            form.GetField("text4").SetValue("12345678");
            form.GetField("text5").SetValue("123456789101112131415161718");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PreserveFontPropsTest() {
            String srcPdf = sourceFolder + "preserveFontPropsTest.pdf";
            String outPdf = destinationFolder + "preserveFontPropsTest.pdf";
            String cmpPdf = sourceFolder + "cmp_preserveFontPropsTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            PdfFormField field1 = form.GetField("emptyField");
            field1.SetValue("Do fields on the left look the same?", field1.GetFont(), field1.GetFontSize());
            PdfFormField field2 = form.GetField("emptyField2");
            field2.SetValue("Do fields on the right look the same?", field2.GetFont(), field2.GetFontSize());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FontAutoSizeButtonFieldTest() {
            String outPdf = destinationFolder + "fontAutoSizeButtonFieldTest.pdf";
            String cmpPdf = sourceFolder + "cmp_fontAutoSizeButtonFieldTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String itext = "itextpdf";
            PdfButtonFormField button = new PushButtonFormFieldBuilder(pdfDoc, itext).SetWidgetRectangle(new Rectangle
                (36, 500, 200, 200)).SetCaption(itext).CreatePushButton();
            button.SetFontSize(0);
            button.GetFirstFormAnnotation().SetBackgroundColor(ColorConstants.GRAY);
            button.SetValue(itext);
            button.GetFirstFormAnnotation().SetVisibility(PdfFormAnnotation.VISIBLE_BUT_DOES_NOT_PRINT);
            form.AddField(button);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MaxLenInheritanceTest() {
            String srcPdf = sourceFolder + "maxLenInheritanceTest.pdf";
            String outPdf = destinationFolder + "maxLenInheritanceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_maxLenInheritanceTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.GetField("text").SetValue("iText!");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MaxLenDeepInheritanceTest() {
            String srcFilename = sourceFolder + "maxLenDeepInheritanceTest.pdf";
            String destFilename = destinationFolder + "maxLenDeepInheritanceTest.pdf";
            String cmpFilename = sourceFolder + "cmp_maxLenDeepInheritanceTest.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(destFilename));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(destDoc, false);
            acroForm.GetField("text.1.").SetColor(ColorConstants.RED);
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFilename, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MaxLenColoredTest() {
            String srcPdf = sourceFolder + "maxLenColoredTest.pdf";
            String outPdf = destinationFolder + "maxLenColoredTest.pdf";
            String cmpPdf = sourceFolder + "cmp_maxLenColoredTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            form.GetField("magenta").SetColor(ColorConstants.MAGENTA);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT, Count = 2)]
        public virtual void RegenerateMaxLenCombTest() {
            String srcPdf = sourceFolder + "regenerateMaxLenCombTest.pdf";
            String outPdf = destinationFolder + "regenerateMaxLenCombTest.pdf";
            String cmpPdf = sourceFolder + "cmp_regenerateMaxLenCombTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            for (int i = 0; i < 12; i++) {
                PdfTextFormField field = (PdfTextFormField)form.GetField("field " + i);
                if (i < 8) {
                    field.SetMaxLen(i < 4 ? 7 : 0);
                }
                if (i % 6 > 1) {
                    field.SetFieldFlag(PdfTextFormField.FF_COMB, i % 2 == 0);
                }
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WrapPrecedingContentOnFlattenTest() {
            String filename = destinationFolder + "wrapPrecedingContentOnFlattenTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFillColor(ColorConstants.MAGENTA);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField[] fields = new PdfTextFormField[4];
            for (int i = 0; i < 4; i++) {
                fields[i] = new TextFormFieldBuilder(pdfDoc, "black" + i).SetWidgetRectangle(new Rectangle(90, 700 - i * 100
                    , 150, 22)).CreateText();
                fields[i].SetValue("black");
            }
            form.AddField(fields[0]);
            form.AddField(fields[1]);
            Document doc = new Document(pdfDoc);
            doc.Add(new AreaBreak());
            canvas = new PdfCanvas(pdfDoc.GetPage(2));
            canvas.SetFillColor(ColorConstants.CYAN);
            form.AddField(fields[2]);
            form.AddField(fields[3], pdfDoc.GetFirstPage());
            form.FlattenFields();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_wrapPrecedingContentOnFlattenTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.MULTIPLE_VALUES_ON_A_NON_MULTISELECT_FIELD)]
        public virtual void PdfWithDifferentFieldsTest() {
            String fileName = destinationFolder + "pdfWithDifferentFieldsTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(fileName));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            pdfDoc.AddNewPage();
            PdfFormField emptyField = new NonTerminalFormFieldBuilder(pdfDoc, "empty").CreateNonTerminalFormField();
            form.AddField(emptyField);
            PdfArray options = new PdfArray();
            options.Add(new PdfString("1"));
            options.Add(new PdfString("2"));
            form.AddField(new ChoiceFormFieldBuilder(pdfDoc, "choice").SetWidgetRectangle(new Rectangle(36, 696, 20, 20
                )).SetOptions(options).CreateList().SetValue("1", true));
            // combo
            form.AddField(new ChoiceFormFieldBuilder(pdfDoc, "list").SetWidgetRectangle(new Rectangle(36, 666, 20, 20)
                ).SetOptions(new String[] { "1", "2", "3" }).CreateComboBox().SetValue("1", true));
            // list
            PdfChoiceFormField f = new ChoiceFormFieldBuilder(pdfDoc, "combo").SetWidgetRectangle(new Rectangle(36, 556
                , 50, 100)).SetOptions(new String[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }).CreateList();
            f.SetValue("9", true);
            f.SetValue("4");
            f.SetTopIndex(2);
            f.SetListSelected(new String[] { "3", "5" });
            form.AddField(f);
            // push button
            form.AddField(new PushButtonFormFieldBuilder(pdfDoc, "push button").SetWidgetRectangle(new Rectangle(36, 526
                , 80, 20)).SetCaption("push").CreatePushButton());
            // radio button
            String formFieldName = "radio group";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, formFieldName);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            radioGroup.SetValue("1", true);
            PdfFormAnnotation radio1 = builder.CreateRadioButton("1", new Rectangle(36, 496, 20, 20));
            radioGroup.AddKid(radio1);
            PdfFormAnnotation radio2 = builder.CreateRadioButton("2", new Rectangle(66, 496, 20, 20));
            radioGroup.AddKid(radio2);
            form.AddField(radioGroup);
            // signature
            PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").CreateSignature().SetValue("Signature"
                );
            signField.SetFontSize(20);
            form.AddField(signField);
            // text
            form.AddField(new TextFormFieldBuilder(pdfDoc, "text").SetWidgetRectangle(new Rectangle(36, 466, 80, 20)).
                CreateText().SetValue("text").SetValue("la la land"));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, sourceFolder + "cmp_pdfWithDifferentFieldsTest.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TextFieldWithWideUnicodeRange() {
            String filename = "textFieldWithWideUnicodeRange.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            pdfDoc.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(new TextFormFieldBuilder(pdfDoc, "text_helvetica").SetWidgetRectangle(new Rectangle(36, 400, 
                100, 40)).CreateText().SetValue("Helvetica"));
            PdfFont noto = PdfFontFactory.CreateFont(sourceFolder + "NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H);
            noto.SetSubset(false);
            String value = "aAáÁàÀăĂắẮằẰẵẴẳẲâÂấẤầẦẫẪǎǍåÅǻǺäÄǟǞãÃą" + "ĄāĀảẢạẠặẶẬæÆǽǼbBḃḂcCćĆčČċĊçÇdDd̂D̂ďĎḋḊḑḐđĐðÐeE" 
                + "éÉèÈĕĔêÊếẾềỀễỄěĚëËẽẼėĖęĘēĒẻẺẹẸệỆəƏfFḟḞgGǵǴğĞ" + "ǧǦġĠģĢḡḠǥǤhHȟȞḧḦħĦḥḤiIíÍìÌĭĬîÎǐǏïÏĩĨİįĮīĪỉỈị" + "ỊıjJĵĴǰJ̌kKḱḰǩǨķĶlLĺĹl̂L̂ľĽļĻłŁŀĿmMm̂M̂ṁṀnNńŃn̂N̂ňŇ"
                 + "ñÑṅṄņŅŋŊoOóÓòÒŏŎôÔốỐồỒỗỖǒǑöÖȫȪőŐõÕȯȮȱȰøØǿǾǫǪ" + "ǭǬōŌỏỎơƠớỚờỜọỌộỘœŒpPṗṖqQĸrRŕŔřŘŗŖsSśŚšŠṡṠşŞṣ" + "ṢșȘßẞtTťŤṫṪţŢțȚŧŦuUúÚùÙûÛǔǓůŮüÜűŰũŨųŲūŪủỦưƯứ"
                 + "ỨừỪữỮửỬựỰụỤvVwWẃẂẁẀŵŴẅẄxXẍẌyYýÝỳỲŷŶÿŸỹỸẏẎȳȲỷỶ" + "ỵỴzZźŹẑẐžŽżŻẓẒʒƷǯǮþÞŉ";
            PdfFormField textField = new TextFormFieldBuilder(pdfDoc, "text").SetWidgetRectangle(new Rectangle(36, 500
                , 400, 300)).CreateMultilineText().SetValue(value);
            textField.SetFont(noto).SetFontSize(12);
            form.AddField(textField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestMakeField() {
            NUnit.Framework.Assert.IsNull(PdfFormField.MakeFormField(new PdfNumber(1), null));
            NUnit.Framework.Assert.IsNull(PdfFormField.MakeFormField(new PdfArray(), null));
        }

        [NUnit.Framework.Test]
        public virtual void TestDaInAppendMode() {
            String testName = "testDaInAppendMode.pdf";
            String srcPdf = sourceFolder + testName;
            ByteArrayOutputStream outPdf = new ByteArrayOutputStream();
            int objectNumber;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf), new StampingProperties
                ().UseAppendMode())) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                PdfFormField field = form.GetField("magenta");
                field.SetFontSize(35);
                field.UpdateDefaultAppearance();
                objectNumber = field.GetPdfObject().GetIndirectReference().GetObjNumber();
            }
            PdfString da;
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(new MemoryStream(outPdf.ToArray())))) {
                da = ((PdfDictionary)pdfDoc_1.GetPdfObject(objectNumber)).GetAsString(PdfName.DA);
            }
            NUnit.Framework.Assert.AreEqual("/F1 35 Tf 1 0 1 rg", da.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SetPageNewField() {
            String filename = destinationFolder + "setPageNewField.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.AddNewPage();
            pdfDoc.AddNewPage();
            pdfDoc.AddNewPage();
            String fieldName = "field1";
            int pageNum = 2;
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField field1 = new TextFormFieldBuilder(pdfDoc, fieldName).SetWidgetRectangle(new Rectangle(90, 
                700, 150, 22)).CreateText();
            field1.SetValue("new field");
            field1.GetFirstFormAnnotation().SetPage(pageNum);
            form.AddField(field1);
            pdfDoc.Close();
            // -------------------------------------------
            PrintOutputPdfNameAndDir(filename);
            PdfDocument resPdf = new PdfDocument(new PdfReader(filename));
            PdfArray fieldsArr = resPdf.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).GetAsArray(PdfName
                .Fields);
            NUnit.Framework.Assert.AreEqual(1, fieldsArr.Size());
            PdfDictionary field = fieldsArr.GetAsDictionary(0);
            PdfDictionary fieldP = field.GetAsDictionary(PdfName.P);
            NUnit.Framework.Assert.AreEqual(resPdf.GetPage(2).GetPdfObject(), fieldP);
            NUnit.Framework.Assert.IsNull(resPdf.GetPage(1).GetPdfObject().GetAsArray(PdfName.Annots));
            PdfArray secondPageAnnots = resPdf.GetPage(2).GetPdfObject().GetAsArray(PdfName.Annots);
            NUnit.Framework.Assert.AreEqual(1, secondPageAnnots.Size());
            NUnit.Framework.Assert.AreEqual(field, secondPageAnnots.Get(0));
            NUnit.Framework.Assert.IsNull(resPdf.GetPage(3).GetPdfObject().GetAsArray(PdfName.Annots));
        }

        private void CreateAcroForm(PdfDocument pdfDoc, PdfAcroForm form, PdfFont font, String text, int offSet) {
            for (int x = offSet; x < (offSet + 3); x++) {
                Rectangle rect = new Rectangle(100 + (30 * x), 100 + (100 * x), 55, 30);
                PdfFormField field = new TextFormFieldBuilder(pdfDoc, "f-" + x).SetWidgetRectangle(rect).CreateText();
                field.SetValue("").SetJustification(TextAlignment.RIGHT).SetFont(font).SetFontSize(12.0f);
                if (text != null) {
                    field.SetValue(text);
                }
                form.AddField(field);
            }
        }

        private void AddParagraph(Document document, String text, PdfFont font) {
            document.Add(new Paragraph("Hello world ").Add(text).SetFont(font));
        }

        private void FillAcroForm(PdfDocument pdfDocument, String text) {
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            foreach (PdfFormField field in acroForm.GetAllFormFields().Values) {
                field.SetValue(text);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetFont2Ways() {
            String filename = destinationFolder + "setFont3Ways.pdf";
            String cmpFilename = sourceFolder + "cmp_setFont3Ways.pdf";
            String testString = "Don't cry over spilt milk";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H);
            Rectangle rect1 = new Rectangle(10, 700, 200, 25);
            Rectangle rect2 = new Rectangle(30, 600, 200, 25);
            PdfButtonFormField pushButton1 = new PushButtonFormFieldBuilder(pdfDocument, "Name1").SetWidgetRectangle(rect1
                ).SetCaption(testString).CreatePushButton();
            pushButton1.SetFont(font).SetFontSize(12);
            form.AddField(pushButton1);
            PdfButtonFormField pushButton2 = new PushButtonFormFieldBuilder(pdfDocument, "Name2").SetWidgetRectangle(rect2
                ).SetCaption(testString).CreatePushButton();
            pushButton2.SetFontAndSize(font, 12f);
            form.AddField(pushButton2);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        // Acrobat removes /NeedAppearances flag when document is opened and suggests to resave the document at once.
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT)]
        public virtual void AppendModeAppearance() {
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String inputFile = "appendModeAppearance.pdf";
                String outputFile = "appendModeAppearance.pdf";
                String line1 = "ABC";
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + inputFile), new PdfWriter(destinationFolder
                     + outputFile), new StampingProperties().UseAppendMode());
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, false);
                form.SetNeedAppearances(true);
                PdfFormField field;
                foreach (KeyValuePair<String, PdfFormField> entry in form.GetAllFormFields()) {
                    field = entry.Value;
                    field.SetValue(line1);
                }
                pdfDocument.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + outputFile, sourceFolder
                     + "cmp_" + outputFile, destinationFolder, "diff_"));
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillUnmergedTextFormField() {
            String file = sourceFolder + "fillUnmergedTextFormField.pdf";
            String outfile = destinationFolder + "fillUnmergedTextFormField.pdf";
            String text = "John";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(file), new PdfWriter(outfile));
            FillAcroForm(pdfDocument, text);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "fillUnmergedTextFormField.pdf"
                , sourceFolder + "cmp_" + "fillUnmergedTextFormField.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ChoiceFieldAutoSize01Test() {
            String filename = destinationFolder + "choiceFieldAutoSize01Test.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String[] options = new String[] { "First Item", "Second Item", "Third Item", "Fourth Item" };
            PdfFormField[] fields = new PdfFormField[] { new ChoiceFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle
                (new Rectangle(110, 750, 150, 20)).SetOptions(options).CreateComboBox().SetValue("First Item"), new ChoiceFormFieldBuilder
                (pdfDoc, "TestField1").SetWidgetRectangle(new Rectangle(310, 650, 150, 90)).SetOptions(options).CreateList
                ().SetValue("Second Item") };
            foreach (PdfFormField field in fields) {
                field.SetFontSize(0);
                field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
                form.AddField(field);
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_choiceFieldAutoSize01Test.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ChoiceFieldAutoSize02Test() {
            String filename = destinationFolder + "choiceFieldAutoSize02Test.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfArray options = new PdfArray();
            options.Add(new PdfString("First Item", PdfEncodings.UNICODE_BIG));
            options.Add(new PdfString("Second Item", PdfEncodings.UNICODE_BIG));
            options.Add(new PdfString("Third Item", PdfEncodings.UNICODE_BIG));
            form.AddField(new ChoiceFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(new Rectangle(110, 750, 150
                , 20)).SetOptions(options).CreateComboBox().SetValue("First Item", true));
            form.AddField(new ChoiceFormFieldBuilder(pdfDoc, "TestField1").SetWidgetRectangle(new Rectangle(310, 650, 
                150, 90)).SetOptions(options).CreateList().SetValue("Second Item", true));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_choiceFieldAutoSize02Test.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BorderWidthIndentSingleLineTest() {
            String filename = destinationFolder + "borderWidthIndentSingleLineTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "single").SetWidgetRectangle(new Rectangle(50, 700
                , 500, 120)).CreateText();
            field.SetValue("Does this text overlap the border?");
            field.SetFontSize(20);
            field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.RED);
            field.GetFirstFormAnnotation().SetBorderWidth(50);
            form.AddField(field);
            PdfTextFormField field2 = new TextFormFieldBuilder(pdfDoc, "singleAuto").SetWidgetRectangle(new Rectangle(
                50, 600, 500, 80)).CreateText();
            field2.SetValue("Does this autosize text overlap the border? Well it shouldn't! Does it fit accurately though?"
                );
            field2.SetFontSize(0);
            field2.GetFirstFormAnnotation().SetBorderColor(ColorConstants.RED);
            field2.GetFirstFormAnnotation().SetBorderWidth(20);
            form.AddField(field2);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_borderWidthIndentSingleLineTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET, Count = 3)]
        public virtual void ReleaseAcroformTest() {
            String srcFile = sourceFolder + "formFieldFile.pdf";
            String outPureStamping = destinationFolder + "formFieldFileStamping.pdf";
            String outStampingRelease = destinationFolder + "formFieldFileStampingRelease.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outPureStamping));
            PdfAcroForm.GetAcroForm(doc, false);
            // We open/close document to make sure that the results of release logic and simple overwriting coincide.
            doc.Close();
            using (PdfDocument stamperRelease = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outStampingRelease
                ))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(stamperRelease, false);
                form.Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outStampingRelease, outPureStamping, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddChildToFormFieldTest() {
            bool experimentalRenderingPreviousValue = ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING;
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = true;
            try {
                String outPdf = destinationFolder + "addChildToFormFieldTest.pdf";
                String cmpPdf = sourceFolder + "cmp_addChildToFormFieldTest.pdf";
                using (PdfDocument outputDoc = new PdfDocument(new PdfWriter(outPdf))) {
                    PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(outputDoc, true);
                    PdfFormField field = new TextFormFieldBuilder(outputDoc, "text1").SetWidgetRectangle(new Rectangle(100, 700
                        , 200, 20)).CreateText();
                    acroForm.AddField(field);
                    PdfFormField root = new TextFormFieldBuilder(outputDoc, "root").SetWidgetRectangle(new Rectangle(100, 600, 
                        200, 20)).CreateText().SetValue("root");
                    PdfFormField child = new TextFormFieldBuilder(outputDoc, "child").SetWidgetRectangle(new Rectangle(100, 500
                        , 200, 20)).CreateText().SetValue("child");
                    root.AddKid(child);
                    acroForm.AddField(root);
                    NUnit.Framework.Assert.AreEqual(2, acroForm.fields.Count);
                    PdfArray fieldKids = root.GetKids();
                    NUnit.Framework.Assert.AreEqual(2, fieldKids.Size());
                }
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
            }
            finally {
                ExperimentalFeatures.ENABLE_EXPERIMENTAL_TEXT_FORM_RENDERING = experimentalRenderingPreviousValue;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD)]
        public virtual void DuplicateFormTest() {
            String outPdf = destinationFolder + "duplicateFormTest.pdf";
            String inPdf = sourceFolder + "duplicateFormTestSource.pdf";
            String cmpPdf = sourceFolder + "cmp_duplicateFormTest.pdf";
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf), new PdfWriter(byteArrayOutputStream));
            PdfDocument pdfInnerDoc = new PdfDocument(new PdfReader(inPdf));
            pdfInnerDoc.CopyPagesTo(1, pdfInnerDoc.GetNumberOfPages(), pdfDocument, new PdfPageFormCopier());
            pdfInnerDoc.Close();
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(byteArrayOutputStream.ToArray())), new PdfWriter
                (outPdf));
            PdfAcroForm pdfAcroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            pdfAcroForm.GetField("checkbox").SetValue("Off");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetValueTest() {
            String outPdf = destinationFolder + "getValueTest.pdf";
            String cmpPdf = sourceFolder + "cmp_getValueTest.pdf";
            String srcPdf = sourceFolder + "getValueTest.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, false);
                foreach (AbstractPdfFormField field in acroForm.GetAllFormFieldsAndAnnotations()) {
                    if (field is PdfFormField && "child".Equals(field.GetPdfObject().Get(PdfName.V).ToString())) {
                        // Child has value "root" still because it doesn't contain T entry
                        NUnit.Framework.Assert.AreEqual("root", ((PdfFormField)field).GetValue().ToString());
                    }
                    field.RegenerateField();
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY)]
        public virtual void SetValueWithDisplayTest() {
            String outPdf = destinationFolder + "setValueWithDisplayTest.pdf";
            String cmpPdf = sourceFolder + "cmp_setValueWithDisplayTest.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField textField = new TextFormFieldBuilder(doc, "text").SetWidgetRectangle(new Rectangle(100, 700, 
                    200, 20)).CreateText();
                textField.SetValue("some text", "*****");
                textField.SetColor(ColorConstants.BLUE);
                acroForm.AddField(textField);
                PdfFormField textField2 = new TextFormFieldBuilder(doc, "text2").SetWidgetRectangle(new Rectangle(100, 650
                    , 100, 20)).CreateText();
                textField2.SetValue("some text", "*****");
                textField2.SetColor(ColorConstants.BLUE);
                textField2.SetValue("new value");
                acroForm.AddField(textField2);
                PdfFormField textField3 = new TextFormFieldBuilder(doc, "text3").SetWidgetRectangle(new Rectangle(250, 650
                    , 100, 20)).CreateText();
                textField3.SetValue("some text", null);
                acroForm.AddField(textField3);
                PdfFormField textField4 = new TextFormFieldBuilder(doc, "text4").SetWidgetRectangle(new Rectangle(400, 650
                    , 100, 20)).CreateText();
                textField4.SetValue("some other text", "");
                textField4.GetFirstFormAnnotation().SetBorderColor(ColorConstants.LIGHT_GRAY);
                acroForm.AddField(textField4);
                PdfButtonFormField pushButtonField = new PushButtonFormFieldBuilder(doc, "button").SetWidgetRectangle(new 
                    Rectangle(36, 600, 200, 20)).SetCaption("Click").CreatePushButton();
                pushButtonField.SetValue("Some button text", "*****");
                pushButtonField.SetColor(ColorConstants.BLUE);
                acroForm.AddField(pushButtonField);
                String[] options = new String[] { "First Item", "Second Item", "Third Item", "Fourth Item" };
                PdfChoiceFormField choiceField = new ChoiceFormFieldBuilder(doc, "choice").SetWidgetRectangle(new Rectangle
                    (36, 550, 200, 20)).SetOptions(options).CreateComboBox();
                choiceField.SetValue("First Item", "display value");
                choiceField.SetColor(ColorConstants.BLUE);
                acroForm.AddField(choiceField);
                RadioFormFieldBuilder builder = new RadioFormFieldBuilder(doc, "group");
                PdfButtonFormField radioGroupField = builder.CreateRadioGroup();
                PdfFormAnnotation radio = builder.CreateRadioButton("1", new Rectangle(36, 500, 20, 20));
                radioGroupField.AddKid(radio);
                radioGroupField.SetValue("1", "display value");
                acroForm.AddField(radioGroupField);
                PdfButtonFormField checkBoxField = new CheckBoxFormFieldBuilder(doc, "check").SetWidgetRectangle(new Rectangle
                    (36, 450, 20, 20)).CreateCheckBox();
                checkBoxField.SetValue("1", "display value");
                acroForm.AddField(checkBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.FIELD_VALUE_CANNOT_BE_NULL, Count = 2)]
        public virtual void SetNullValueTest() {
            String outPdf = destinationFolder + "setNullValueTest.pdf";
            String cmpPdf = sourceFolder + "cmp_setNullValueTest.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField textField = new TextFormFieldBuilder(doc, "text").SetWidgetRectangle(new Rectangle(100, 700, 
                    200, 20)).CreateText();
                textField.SetValue(null);
                textField.SetValue(null, "*****");
                acroForm.AddField(textField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetSigFlagsTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                form.SetSignatureFlag(1);
                NUnit.Framework.Assert.AreEqual(1, form.GetSignatureFlags());
            }
        }
    }
}
