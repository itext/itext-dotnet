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
using System.IO;
using iText.Forms;
using iText.Forms.Exceptions;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Renderer.Checkboximpl;
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RadioTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/RadioTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/RadioTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicRadioTest() {
            String outPdf = DESTINATION_FOLDER + "basicRadio.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicRadio.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Radio formRadio1 = CreateRadioButton("form radio button 1", "form radio group", null, null, true, false);
                document.Add(formRadio1);
                Radio formRadio2 = CreateRadioButton("form radio button 2", "form radio group", null, null, false, false);
                document.Add(formRadio2);
                Radio flattenRadio1 = CreateRadioButton("flatten radio button 1", "flatten radio group", null, null, true, 
                    true);
                document.Add(flattenRadio1);
                Radio flattenRadio2 = CreateRadioButton("flatten radio button 2", "flatten radio group", null, null, false
                    , true);
                document.Add(flattenRadio2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyNameTest() {
            using (Document document = new Document(new PdfDocument(new PdfWriter(new MemoryStream())))) {
                Radio formRadio = CreateRadioButton("radio button 1", null, null, null, true, false);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(formRadio));
                NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.EMPTY_RADIO_GROUP_NAME, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EmptyValueTest() {
            using (Document document = new Document(new PdfDocument(new PdfWriter(new MemoryStream())))) {
                Radio formRadio = CreateRadioButton("", "radioGroup", null, null, true, false);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(formRadio));
                NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.APEARANCE_NAME_MUST_BE_PROVIDED, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MergeWithExistingFieldTest() {
            String srcPdf = SOURCE_FOLDER + "src_mergeWithExistingField.pdf";
            String outPdf = DESTINATION_FOLDER + "mergeWithExistingField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_mergeWithExistingField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf)))) {
                Radio formRadio1 = CreateRadioButton("radio1", "form radio group", new SolidBorder(ColorConstants.BLUE, 1)
                    , null, false, false);
                document.Add(formRadio1);
                Radio formRadio2 = CreateRadioButton("radio2", "form radio group", new SolidBorder(ColorConstants.BLUE, 1)
                    , null, false, false);
                document.Add(formRadio2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderBackgroundTest() {
            String outPdf = DESTINATION_FOLDER + "borderBackground.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderBackground.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Radio formRadio1 = CreateRadioButton("formRadio1", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 1), ColorConstants.GREEN, true, false);
                document.Add(formRadio1);
                Radio formRadio2 = CreateRadioButton("formRadio2", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 3), ColorConstants.GREEN, false, false);
                document.Add(formRadio2);
                Radio formRadio3 = CreateRadioButton("formRadio3", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 6), ColorConstants.GREEN, false, false);
                document.Add(formRadio3);
                Radio formRadio4 = CreateRadioButton("formRadio4", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 6), ColorConstants.GREEN, false, false);
                formRadio4.SetSize(20);
                document.Add(formRadio4);
                Radio formRadio5 = CreateRadioButton("formRadio5", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 6), ColorConstants.GREEN, false, false);
                formRadio5.SetSize(20);
                document.Add(formRadio5);
                Radio flattenRadio1 = CreateRadioButton("flattenRadio1", "form radio group", new SolidBorder(ColorConstants
                    .BLUE, 1), ColorConstants.GREEN, true, true);
                flattenRadio1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(flattenRadio1);
                Radio flattenRadio2 = CreateRadioButton("flattenRadio2", "form radio group", new SolidBorder(ColorConstants
                    .BLUE, 3), ColorConstants.GREEN, false, true);
                document.Add(flattenRadio2);
                Radio flattenRadio3 = CreateRadioButton("flattenRadio3", "form radio group", new SolidBorder(ColorConstants
                    .BLUE, 6), ColorConstants.GREEN, false, true);
                document.Add(flattenRadio3);
                Radio flattenRadio4 = CreateRadioButton("flattenRadio4", "form radio group", new SolidBorder(ColorConstants
                    .BLUE, 6), ColorConstants.GREEN, false, true);
                flattenRadio4.SetSize(20);
                document.Add(flattenRadio4);
                Radio flattenRadio5 = CreateRadioButton("flattenRadio5", "form radio group", new SolidBorder(ColorConstants
                    .BLUE, 6), ColorConstants.GREEN, false, true);
                flattenRadio5.SetSize(20);
                document.Add(flattenRadio5);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderBoxesTest() {
            String outPdf = DESTINATION_FOLDER + "borderBoxes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderBoxes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // BORDER_BOX
                Radio formRadio1 = CreateRadioButton("formRadio1", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 3), ColorConstants.GREEN, true, false);
                formRadio1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(formRadio1);
                // CONTENT_BOX
                Radio formRadio2 = CreateRadioButton("formRadio2", "form radio group", new SolidBorder(ColorConstants.BLUE
                    , 3), ColorConstants.GREEN, false, false);
                formRadio2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(formRadio2);
                // BORDER_BOX
                Radio flattenRadio1 = CreateRadioButton("flattenRadio1", "flatten radio group", new SolidBorder(ColorConstants
                    .BLUE, 3), ColorConstants.GREEN, true, true);
                flattenRadio1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(flattenRadio1);
                // CONTENT_BOX
                Radio flattenRadio2 = CreateRadioButton("flattenRadio2", "flatten radio group", new SolidBorder(ColorConstants
                    .BLUE, 3), ColorConstants.GREEN, false, true);
                flattenRadio2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(flattenRadio2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void DottedBorderTest() {
            String outPdf = DESTINATION_FOLDER + "dottedBorder.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_dottedBorder.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Radio formRadio1 = CreateRadioButton("formRadio1", "form radio group", new DottedBorder(ColorConstants.BLUE
                    , 3), ColorConstants.GREEN, true, false);
                formRadio1.SetSize(20);
                document.Add(formRadio1);
                Radio formRadio2 = CreateRadioButton("formRadio2", "form radio group", new DottedBorder(ColorConstants.BLUE
                    , 3), ColorConstants.GREEN, false, false);
                formRadio2.SetSize(20).SetProperty(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE, false);
                document.Add(formRadio2);
                Radio flattenRadio1 = CreateRadioButton("flattenRadio1", "flatten radio group", new DottedBorder(ColorConstants
                    .BLUE, 3), ColorConstants.GREEN, true, true);
                flattenRadio1.SetSize(20);
                document.Add(flattenRadio1);
                Radio flattenRadio2 = CreateRadioButton("flattenRadio2", "flatten radio group", new DottedBorder(ColorConstants
                    .BLUE, 3), ColorConstants.GREEN, false, true);
                flattenRadio2.SetSize(20).SetProperty(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE, false);
                document.Add(flattenRadio2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        // This is the test for TODO: DEVSIX-7425 - Border radius 50% doesn't draw rounded borders
        // showing different drawing of circles
        [NUnit.Framework.Test]
        public virtual void FormFieldRadioBorderCircleTest() {
            String outPdf = DESTINATION_FOLDER + "formFieldRadioBorderCircle.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_formFieldRadioBorderCircle.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Radio flattenRadio1 = CreateRadioButton("flattenRadio1", "flatten radio group", new SolidBorder(ColorConstants
                    .LIGHT_GRAY, 1), ColorConstants.GREEN, false, true);
                flattenRadio1.SetProperty(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE, true);
                document.Add(flattenRadio1);
                Radio flattenRadio2 = CreateRadioButton("flattenRadio2", "flatten radio group", new SolidBorder(ColorConstants
                    .LIGHT_GRAY, 1), ColorConstants.GREEN, false, true);
                flattenRadio2.SetProperty(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE, false);
                document.Add(flattenRadio2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT)]
        public virtual void BigRadioButtonTest() {
            String outPdf = DESTINATION_FOLDER + "bigRadioButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_bigRadioButton.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Radio flattenRadio1 = CreateRadioButton("flattenRadio1", "form radio group", new SolidBorder(ColorConstants
                    .BLUE, 1), ColorConstants.GREEN, true, true);
                flattenRadio1.SetSize(825f);
                document.Add(flattenRadio1);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void RadioWithMarginsTest() {
            String outPdf = DESTINATION_FOLDER + "radioWithMargins.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_radioWithMargins.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div().SetBackgroundColor(ColorConstants.PINK);
                Radio radio = CreateRadioButton("radio", "form radio group", new SolidBorder(ColorConstants.DARK_GRAY, 20)
                    , ColorConstants.WHITE, true, false);
                radio.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(20));
                radio.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
                radio.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(20));
                radio.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(20));
                radio.SetSize(100);
                div.Add(radio);
                document.Add(div);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void RadioWithPaddingsTest() {
            String outPdf = DESTINATION_FOLDER + "radioWithPaddings.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_radioWithPaddings.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div().SetBackgroundColor(ColorConstants.PINK);
                Radio radio = CreateRadioButton("radio", "form radio group", new SolidBorder(ColorConstants.DARK_GRAY, 20)
                    , ColorConstants.WHITE, true, false);
                radio.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(20));
                radio.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20));
                radio.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(20));
                radio.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(20));
                // Paddings are always 0 for radio buttons
                NUnit.Framework.Assert.AreEqual(radio.GetProperty<UnitValue>(Property.PADDING_BOTTOM), UnitValue.CreatePointValue
                    (0));
                NUnit.Framework.Assert.AreEqual(radio.GetProperty<UnitValue>(Property.PADDING_TOP), UnitValue.CreatePointValue
                    (0));
                NUnit.Framework.Assert.AreEqual(radio.GetProperty<UnitValue>(Property.PADDING_LEFT), UnitValue.CreatePointValue
                    (0));
                NUnit.Framework.Assert.AreEqual(radio.GetProperty<UnitValue>(Property.PADDING_RIGHT), UnitValue.CreatePointValue
                    (0));
                radio.SetSize(100);
                div.Add(radio);
                document.Add(div);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
        [NUnit.Framework.Test]
        public virtual void RadioButtonCheckTypeCheckTest()
        {
            String outPdf           = DESTINATION_FOLDER + "radioButtonCheckTypeCheck.pdf";
            String cmpPdf           = SOURCE_FOLDER + "cmp_radioButtonCheckTypeCheck.pdf";
            SolidBorder border      = new SolidBorder(ColorConstants.BLUE, 3);
            Color checkColor        = ColorConstants.BLUE;
            Color backColor         = ColorConstants.GREEN;
            CheckBoxType checkType  = Fields.Properties.CheckBoxType.CHECK;
            PdfString caValue       = new PdfString(PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING.GetByKey(checkType));
            using(PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf)))
            {
                PdfButtonFormField btnGroup = CreateRadioButtonGroup(border, checkColor, backColor, checkType, caValue, pdfDocument);
                // set selected radio button
                btnGroup.SetValue("formRadio1");
                pdfDocument.Close();
            }

            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
        [NUnit.Framework.Test]
        public virtual void RadioButtonCheckTypSquare()
        {
            String outPdf = DESTINATION_FOLDER + "radioButtonCheckTypeSquare.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_radioButtonCheckTypeSquare.pdf";
            SolidBorder border = new SolidBorder(ColorConstants.ORANGE, 3);
            Color checkColor = ColorConstants.ORANGE;
            Color backColor = ColorConstants.GRAY;
            CheckBoxType checkType = Fields.Properties.CheckBoxType.SQUARE;
            PdfString caValue = new PdfString(PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING.GetByKey(checkType));
            using(PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf)))
            {
                PdfButtonFormField btnGroup = CreateRadioButtonGroup(border, checkColor, backColor, checkType, caValue, pdfDocument);
                // change btnGroup to allow unselect 
                btnGroup.SetRadio(false);
                btnGroup.SetValue("formRadio2", false);
                pdfDocument.Close();
            }

            NUnit.Framework.Assert.IsTrue(File.Exists(outPdf));//.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER)); 
        }
        [NUnit.Framework.Test]
        public virtual void RadioButtonCheckTypSquare_NoSelection()
        {
            String outPdf = DESTINATION_FOLDER + "radioButtonCheckTypeSquare_NoSelection.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_radioButtonCheckTypeSquare_NoSelection.pdf";
            SolidBorder border = new SolidBorder(ColorConstants.ORANGE, 3);
            Color checkColor = ColorConstants.ORANGE;
            Color backColor = ColorConstants.GRAY;
            CheckBoxType checkType = Fields.Properties.CheckBoxType.SQUARE;
            PdfString caValue = new PdfString(PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING.GetByKey(checkType));
            using(PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf)))
            {
                PdfButtonFormField btnGroup = CreateRadioButtonGroup(border, checkColor, backColor, checkType, caValue, pdfDocument);
                // change btnGroup to allow unselect 
                btnGroup.SetRadio(false);
                btnGroup.SetValue("formRadio2", false);
                pdfDocument.Close();
            }

            NUnit.Framework.Assert.IsTrue(File.Exists(outPdf));//.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER)); 
        }

        [NUnit.Framework.Test]
        public virtual void RadioButtonCheckTypeStar_NoSelectionTest()
        {
            String outPdf = DESTINATION_FOLDER + "radioButtonCheckTypeStar_NoSelection.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_radioButtonCheckTypeStar_NoSelection.pdf";
            SolidBorder border = new SolidBorder(ColorConstants.ORANGE, 3);
            Color checkColor = ColorConstants.ORANGE;
            Color backColor = ColorConstants.GRAY;
            CheckBoxType checkType = Fields.Properties.CheckBoxType.STAR;
            PdfString caValue = new PdfString(PdfCheckBoxRenderingStrategy.ZAPFDINGBATS_CHECKBOX_MAPPING.GetByKey(checkType));
            using(PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf)))
            {
                PdfButtonFormField btnGroup = CreateRadioButtonGroup(border, checkColor, backColor, checkType, caValue, pdfDocument);
                // change btnGroup to allow unselect 
                btnGroup.SetRadio(false);
                btnGroup.SetValue("", false);
                pdfDocument.Close();
            }

            NUnit.Framework.Assert.IsTrue(File.Exists(outPdf));//.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER)); //.IsTrue(File.Exists(outPdf));//
        }
        private static PdfButtonFormField CreateRadioButtonGroup(SolidBorder border, Color checkColor, Color backColor, CheckBoxType checkType, PdfString caValue, PdfDocument pdfDocument)
        {
            pdfDocument.AddNewPage(PageSize.A4);
            var form = PdfAcroForm.GetAcroForm(pdfDocument, true);
            // create group
            var radioBuilder1 = new RadioFormFieldBuilder(pdfDocument, "form radio group");
            radioBuilder1.SetCheckType(checkType);
            var btnGroup = radioBuilder1.CreateRadioGroup();
            btnGroup.SetColor(ColorConstants.BLUE);
            var formField = btnGroup;
            formField.SetCheckType(checkType);
            // create button1
            var btn1 = radioBuilder1.CreateRadioButton("formRadio1", new Rectangle(0, 800, 20, 20));
            btn1.SetPage(1);
            btn1.SetBorderColor(border.GetColor());
            btn1.SetBorderWidth(border.GetWidth());
            btn1.SetBackgroundColor(backColor);
            btn1.SetColor(checkColor);
            btn1.SetParent(formField);
            btnGroup.AddKid(btn1);
            // set visible check type (highlight appearance in Acrobat)
            var mk1 = btn1.GetWidget().GetAppearanceCharacteristics();
            if(mk1 == null)
                mk1 = new PdfDictionary();
            mk1.Put(PdfName.CA, caValue);
            btn1.Put(PdfName.MK, mk1);
            // create button2
            var btn2 = radioBuilder1.CreateRadioButton("formRadio2", new Rectangle(22, 800, 20, 20));
            btn2.SetPage(1);
            btn2.SetBorderColor(border.GetColor());
            btn2.SetBorderWidth(border.GetWidth());
            btn2.SetBackgroundColor(backColor);
            btn2.SetColor(checkColor);
            btn2.SetParent(formField);
            //  set visible check type (highlight appearance in Acrobat)
            var mk2 = btn2.GetWidget().GetAppearanceCharacteristics();
            if(mk2 == null)
                mk2 = new PdfDictionary();
            mk2.Put(PdfName.CA, caValue);
            btn2.Put(PdfName.MK, mk2);
            btnGroup.AddKid(btn2);
            form.AddField(btnGroup);
            return btnGroup;
        }


        [NUnit.Framework.Test]
        public virtual void MultiPageRadioFieldTest() {
            String outPdf = DESTINATION_FOLDER + "multiPageCheckboxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_multiPageCheckBoxField.pdf";
            using (PdfDocument document = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(document, true);
                for (int i = 0; i < 10; i++) {
                    document.AddNewPage();
                    Rectangle rect = new Rectangle(210, 490, 150, 22);
                    PdfFormField group = new RadioFormFieldBuilder(document, "fing").CreateRadioGroup();
                    PdfFormAnnotation radio = new RadioFormFieldBuilder(document, "fing").SetWidgetRectangle(rect).CreateRadioButton
                        ("bing bong", rect);
                    PdfPage page = document.GetPage(i + 1);
                    group.AddKid(radio);
                    form.AddField(group, page);
                    if (i > 2) {
                        page.Flush();
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        private static Radio CreateRadioButton(String name, String groupName, Border border, Color backgroundColor
            , bool @checked, bool flatten) {
            Radio radio = new Radio(name, groupName);
            radio.SetBorder(border);
            radio.SetBackgroundColor(backgroundColor);
            radio.SetInteractive(!flatten);
            radio.SetChecked(@checked);
            return radio;
        }
    }
}
