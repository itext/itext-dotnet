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
using iText.Forms.Exceptions;
using iText.Forms.Form;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InputFieldTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/InputFieldTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/InputFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicInputFieldTest() {
            String outPdf = DESTINATION_FOLDER + "basicInputField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicInputField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField formInputField = new InputField("form input field");
                formInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "form input field");
                document.Add(formInputField);
                InputField flattenInputField = new InputField("flatten input field");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten input field");
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void NoValueInputFieldTest() {
            String outPdf = DESTINATION_FOLDER + "noValueInputField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_noValueInputField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField flattenInputField = new InputField("no value input field");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, null);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, null);
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputFieldDoesNotFitTest() {
            String outPdf = DESTINATION_FOLDER + "inputFieldDoesNotFit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputFieldDoesNotFit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div();
                div.SetWidth(UnitValue.CreatePointValue(400));
                div.SetHeight(UnitValue.CreatePointValue(752));
                div.SetBackgroundColor(ColorConstants.PINK);
                document.Add(div);
                InputField flattenInputField = new InputField("input field does not fit");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "input field does not fit");
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputFieldWithLangTest() {
            String outPdf = DESTINATION_FOLDER + "inputFieldWithLang.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputFieldWithLang.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                document.GetPdfDocument().SetTagged();
                InputField flattenInputField = new InputField("input field with lang");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "input field with lang");
                flattenInputField.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, "random_lang");
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputFieldWithNullLangTest() {
            String outPdf = DESTINATION_FOLDER + "inputFieldWithNullLang.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputFieldWithNullLang.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                document.GetPdfDocument().SetTagged();
                InputField flattenInputField = new InputField("input field with null lang");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "input field with null lang");
                flattenInputField.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, null);
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputFieldWithPasswordTest() {
            String outPdf = DESTINATION_FOLDER + "inputFieldWithPassword.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputFieldWithPassword.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField formInputField = new InputField("form input field with password");
                formInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "form input field with password");
                formInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                formInputField.SetProperty(FormProperty.FORM_FIELD_PASSWORD_FLAG, true);
                document.Add(formInputField);
                InputField flattenInputField = new InputField("flatten input field with password");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten input field with password");
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_PASSWORD_FLAG, true);
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void HeightInputFieldTest() {
            String outPdf = DESTINATION_FOLDER + "heightInputField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_heightInputField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField flattenInputField = new InputField("flatten input field with height");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten input field with height");
                flattenInputField.SetProperty(Property.HEIGHT, new UnitValue(UnitValue.POINT, 100));
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MinHeightInputFieldTest() {
            String outPdf = DESTINATION_FOLDER + "minHeightInputField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_minHeightInputField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField flattenInputField = new InputField("flatten input field with height");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten input field with height");
                flattenInputField.SetProperty(Property.MIN_HEIGHT, new UnitValue(UnitValue.POINT, 100));
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MaxHeightInputFieldTest() {
            String outPdf = DESTINATION_FOLDER + "maxHeightInputField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_maxHeightInputField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField flattenInputField = new InputField("flatten input field with height");
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten input field with height");
                flattenInputField.SetProperty(Property.MAX_HEIGHT, new UnitValue(UnitValue.POINT, 10));
                flattenInputField.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputFieldWithJustificationTest() {
            String outPdf = DESTINATION_FOLDER + "inputFieldWithJustification.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputFieldWithJustification.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField flattenInputField = new InputField("input field");
                flattenInputField.SetValue("input field");
                flattenInputField.SetInteractive(true);
                flattenInputField.SetTextAlignment(TextAlignment.CENTER);
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputFieldWithBorderTest() {
            String outPdf = DESTINATION_FOLDER + "inputFieldWithBorder.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputFieldWithBorder.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                InputField flattenInputField = new InputField("input field");
                flattenInputField.SetValue("input field");
                flattenInputField.SetInteractive(true);
                flattenInputField.SetBorder(new DashedBorder(ColorConstants.ORANGE, 10));
                document.Add(flattenInputField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void RotationTest() {
            String outPdf = DESTINATION_FOLDER + "rotationTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_rotationTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                String value = "Long long text";
                Border border = new SolidBorder(ColorConstants.BLUE, 1);
                InputField inputField0 = new InputField("1");
                inputField0.SetValue(value);
                inputField0.SetHeight(50);
                inputField0.SetWidth(100);
                inputField0.SetBorder(border);
                inputField0.SetInteractive(true);
                document.Add(inputField0);
                InputField inputField90 = new InputField("1");
                inputField90.SetValue(value);
                inputField90.SetHeight(50);
                inputField90.SetWidth(100);
                inputField90.SetBorder(border);
                inputField90.SetInteractive(true);
                inputField90.SetRotation(90);
                document.Add(inputField90);
                InputField inputField180 = new InputField("1");
                inputField180.SetValue(value);
                inputField180.SetHeight(50);
                inputField180.SetWidth(100);
                inputField180.SetBorder(border);
                inputField180.SetInteractive(true);
                inputField180.SetRotation(180);
                document.Add(inputField180);
                InputField inputField270 = new InputField("1");
                inputField270.SetValue(value);
                inputField270.SetHeight(50);
                inputField270.SetWidth(100);
                inputField270.SetBorder(border);
                inputField270.SetInteractive(true);
                inputField270.SetRotation(270);
                document.Add(inputField270);
                InputField inputField45 = new InputField("1");
                Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => inputField45.SetRotation
                    (45));
                NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.INVALID_ROTATION_VALUE, exception.Message);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void BorderBoxesTest() {
            String outPdf = DESTINATION_FOLDER + "borderBoxes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderBoxes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // BORDER_BOX
                InputField interactiveInputField = new InputField("interactiveInputField").SetBorder(new SolidBorder(ColorConstants
                    .PINK, 10));
                interactiveInputField.SetWidth(200);
                interactiveInputField.SetInteractive(true);
                interactiveInputField.SetValue("interactive border box");
                interactiveInputField.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(interactiveInputField);
                // CONTENT_BOX
                InputField interactiveInputField2 = new InputField("interactiveInputField2").SetBorder(new SolidBorder(ColorConstants
                    .YELLOW, 10));
                interactiveInputField2.SetWidth(200);
                interactiveInputField2.SetInteractive(true);
                interactiveInputField2.SetValue("interactive content box");
                interactiveInputField2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(interactiveInputField2);
                // BORDER_BOX
                InputField flattenInputField = new InputField("flattenInputField").SetBorder(new SolidBorder(ColorConstants
                    .PINK, 10));
                flattenInputField.SetWidth(200);
                flattenInputField.SetInteractive(false);
                flattenInputField.SetValue("content box");
                flattenInputField.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(flattenInputField);
                // CONTENT_BOX
                InputField flattenInputField2 = new InputField("flattenInputField2").SetBorder(new SolidBorder(ColorConstants
                    .YELLOW, 10));
                flattenInputField2.SetWidth(200);
                flattenInputField2.SetInteractive(false);
                flattenInputField2.SetValue("content box");
                flattenInputField2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(flattenInputField2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
