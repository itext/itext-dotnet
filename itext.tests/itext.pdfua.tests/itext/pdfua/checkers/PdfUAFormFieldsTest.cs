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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Pdfua;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAFormFieldsTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUATest/PdfUAFormFieldTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAFormFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestCheckBox() {
            String dest = DESTINATION_FOLDER + "checkBoxLayout.pdf";
            Document document = CreateDocument(dest);
            CheckBox checkBox = new CheckBox("name");
            document.Add(checkBox);
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxInteractive() {
            String dest = DESTINATION_FOLDER + "checkBoxLayoutI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_checkBoxLayoutI.pdf";
            Document document = CreateDocument(dest);
            CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
            checkBox.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            document.Add(checkBox);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestRadioButton() {
            String dest = DESTINATION_FOLDER + "radioButtonLayout.pdf";
            String cmp = SOURCE_FOLDER + "cmp_radioButtonLayout.pdf";
            Document document = CreateDocument(dest);
            Radio radioButton = new Radio("name");
            document.Add(radioButton);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonInteractive() {
            String dest = DESTINATION_FOLDER + "radioButtonLayoutI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_radioButtonLayoutI.pdf";
            Document document = CreateDocument(dest);
            Radio radioButton = (Radio)new Radio("name", "empty").SetInteractive(true);
            document.Add(radioButton);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestRadioButtonGroup() {
            String dest = DESTINATION_FOLDER + "radioButtonGroup.pdf";
            String cmp = SOURCE_FOLDER + "cmp_radioButtonGroup.pdf";
            Document document = CreateDocument(dest);
            Radio radioButton = new Radio("name", "group");
            Radio radioButton2 = new Radio("name2", "group");
            document.Add(radioButton);
            document.Add(radioButton2);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupInteractive() {
            String dest = DESTINATION_FOLDER + "radioButtonGroupInteractiveI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_radioButtonGroupInteractiveI.pdf";
            Document document = CreateDocument(dest);
            Radio radioButton = (Radio)new Radio("name", "group").SetInteractive(true);
            Radio radioButton2 = (Radio)new Radio("name2", "group").SetInteractive(true);
            document.Add(radioButton);
            document.Add(radioButton2);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestButton() {
            String dest = DESTINATION_FOLDER + "buttonLayout.pdf";
            String cmp = SOURCE_FOLDER + "cmp_buttonLayout.pdf";
            Document document = CreateDocument(dest);
            Button button = new Button("name");
            button.SetFont(PdfFontFactory.CreateFont(FONT));
            button.SetValue("Click me");
            document.Add(button);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonInteractive() {
            String dest = DESTINATION_FOLDER + "buttonLayoutI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_buttonLayoutI.pdf";
            Document document = CreateDocument(dest);
            Button button = (Button)new Button("name").SetInteractive(true);
            button.SetFont(PdfFontFactory.CreateFont(FONT));
            button.SetValue("Click me");
            document.Add(button);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestInputField() {
            String dest = DESTINATION_FOLDER + "inputFieldLayout.pdf";
            String cmp = SOURCE_FOLDER + "cmp_inputFieldLayout.pdf";
            Document document = CreateDocument(dest);
            InputField text = new InputField("name");
            document.Add(text);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldInteractive() {
            String dest = DESTINATION_FOLDER + "inputFieldLayoutI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_inputFieldLayoutI.pdf";
            Document document = CreateDocument(dest);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            InputField text = (InputField)new InputField("name").SetFont(font).SetInteractive(true);
            document.Add(text);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestTextArea() {
            String dest = DESTINATION_FOLDER + "textAreaLayout.pdf";
            String cmp = SOURCE_FOLDER + "cmp_textAreaLayout.pdf";
            Document document = CreateDocument(dest);
            TextArea text = new TextArea("name");
            document.Add(text);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaInteractive() {
            String dest = DESTINATION_FOLDER + "textAreaLayoutI.pdf";
            String cmp = DESTINATION_FOLDER + "textAreaLayoutI.pdf";
            Document document = CreateDocument(dest);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            TextArea text = (TextArea)new TextArea("name").SetFont(font).SetInteractive(true);
            document.Add(text);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestListBox() {
            String dest = DESTINATION_FOLDER + "listBoxLayout.pdf";
            String cmp = DESTINATION_FOLDER + "cmp_listBoxLayout.pdf";
            Document document = CreateDocument(dest);
            ListBoxField list = new ListBoxField("name", 1, false);
            list.AddOption("value1");
            list.AddOption("value2");
            document.Add(list);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxInteractive() {
            String dest = DESTINATION_FOLDER + "listBoxLayoutI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_listBoxLayoutI.pdf";
            Document document = CreateDocument(dest);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            ListBoxField list = (ListBoxField)new ListBoxField("name", 1, false).SetFont(font).SetInteractive(true);
            list.AddOption("value1");
            list.AddOption("value2");
            document.Add(list);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8128")]
        public virtual void TestSelectField() {
            String dest = DESTINATION_FOLDER + "selectFieldLayout.pdf";
            String cmp = SOURCE_FOLDER + "cmp_selectFieldLayout.pdf";
            Document document = CreateDocument(dest);
            ListBoxField list = new ListBoxField("name", 1, false);
            list.AddOption("value1");
            list.AddOption("value2");
            document.Add(list);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void TestSelectFieldInteractive() {
            String dest = DESTINATION_FOLDER + "selectFieldLayoutI.pdf";
            String cmp = SOURCE_FOLDER + "cmp_selectFieldLayoutI.pdf";
            Document document = CreateDocument(dest);
            PdfFont font = PdfFontFactory.CreateFont(FONT);
            ListBoxField list = (ListBoxField)new ListBoxField("name", 1, false).SetFont(font).SetInteractive(true);
            list.AddOption("value1");
            list.AddOption("value2");
            document.Add(list);
            document.Close();
            AssertPdf(dest, cmp);
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxBuilderTest() {
            String dest = DESTINATION_FOLDER + "checkBoxBuilderTest.pdf";
            String cmp = SOURCE_FOLDER + "cmp_checkBoxBuilderTest.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            CheckBoxFormFieldBuilder builder = new CheckBoxFormFieldBuilder(document, "chk");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            builder.SetCheckType(CheckBoxType.CHECK);
            builder.SetWidgetRectangle(new Rectangle(200, 200, 20, 20));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            acroForm.AddField(builder.CreateCheckBox().SetValue("Yes"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160   Assert.assertNull(new VeraPdfValidator().validate(dest));
        [NUnit.Framework.Test]
        public virtual void RadioBuilderTest() {
            String dest = DESTINATION_FOLDER + "radioBuilder.pdf";
            String cmp = SOURCE_FOLDER + "cmp_radioBuilder.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(document, "radio");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            PdfFormField group = builder.CreateRadioGroup().SetValue("bing");
            group.AddKid(builder.CreateRadioButton("bing", new Rectangle(200, 200, 20, 20)));
            group.AddKid(builder.CreateRadioButton("bong", new Rectangle(230, 200, 20, 20)));
            acroForm.AddField(group);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160 Assert.assertNull(new VeraPdfValidator().validate(dest));
        [NUnit.Framework.Test]
        public virtual void InputTextFieldTest() {
            String dest = DESTINATION_FOLDER + "inputTextBuilder.pdf";
            String cmp = SOURCE_FOLDER + "cmp_inputTextBuilder.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            TextFormFieldBuilder builder = new TextFormFieldBuilder(document, "txt");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            builder.SetWidgetRectangle(new Rectangle(200, 200, 100, 20));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            PdfFormField f = builder.SetFont(PdfFontFactory.CreateFont(FONT)).CreateText();
            f.SetValue("Hello from text box");
            acroForm.AddField(f);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160  Assert.assertNull(new VeraPdfValidator().validate(dest));
        [NUnit.Framework.Test]
        public virtual void InputAreaFieldTest() {
            String dest = DESTINATION_FOLDER + "inputAreaBuilder.pdf";
            String cmp = SOURCE_FOLDER + "cmp_inputAreaBuilder.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            TextFormFieldBuilder builder = new TextFormFieldBuilder(document, "txt");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            builder.SetWidgetRectangle(new Rectangle(200, 200, 100, 200));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            PdfFormField f = builder.SetFont(PdfFontFactory.CreateFont(FONT)).CreateMultilineText();
            f.SetValue("Hello from text box");
            acroForm.AddField(f);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160  Assert.assertNull(new VeraPdfValidator().validate(dest));
        [NUnit.Framework.Test]
        public virtual void ListBoxFieldTest() {
            String dest = DESTINATION_FOLDER + "listBoxBuilder.pdf";
            String cmp = SOURCE_FOLDER + "cmp_ListboxBuilder.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(document, "txt");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            builder.SetWidgetRectangle(new Rectangle(200, 200, 100, 200));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            builder.SetOptions(new String[] { "opt 1", "opt 2", "opt 3" });
            PdfFormField f = builder.SetFont(PdfFontFactory.CreateFont(FONT)).CreateList();
            f.SetValue("opt 2");
            acroForm.AddField(f);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160  Assert.assertNull(new VeraPdfValidator().validate(dest));
        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldTest() {
            String dest = DESTINATION_FOLDER + "comboboxBuilder.pdf";
            String cmp = SOURCE_FOLDER + "cmp_comboboxBuilder.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(document, "txt");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            builder.SetWidgetRectangle(new Rectangle(200, 200, 100, 200));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            builder.SetOptions(new String[] { "opt 1", "opt 2", "opt 3" });
            PdfFormField f = builder.SetFont(PdfFontFactory.CreateFont(FONT)).CreateComboBox();
            f.SetValue("opt 2");
            acroForm.AddField(f);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160  Assert.assertNull(new VeraPdfValidator().validate(dest));
        [NUnit.Framework.Test]
        public virtual void ButtonTest() {
            String dest = DESTINATION_FOLDER + "buttonBuilder.pdf";
            String cmp = SOURCE_FOLDER + "cmp_buttonBuilder.pdf";
            PdfDocument document = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PushButtonFormFieldBuilder builder = new PushButtonFormFieldBuilder(document, "txt");
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            builder.SetWidgetRectangle(new Rectangle(200, 200, 100, 200));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(document, true);
            PdfFormField f = builder.SetFont(PdfFontFactory.CreateFont(FONT)).CreatePushButton();
            f.SetValue("Click me");
            acroForm.AddField(f);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        // TODO-DEVSIX-8160  formfield TU entry Assert.assertNull(new VeraPdfValidator().validate(dest));
        private static Document CreateDocument(String dest) {
            PdfDocument doc = new PdfUATestPdfDocument(new PdfWriter(dest, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            return new Document(doc);
        }

        private static void AssertPdf(String dest, String cmp) {
            // TODO-DEVSIX-8160 formfield TU entry Assert.assertNull(new VeraPdfValidator().validate(dest)); //
            //  Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }
    }
}
