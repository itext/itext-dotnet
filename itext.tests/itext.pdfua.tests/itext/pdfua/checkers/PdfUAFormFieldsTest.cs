/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAFormFieldsTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAFormFieldTest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBox(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => new CheckBox("name"));
            framework.AssertBothValid("testCheckBox");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxWithCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(conformance);
                cb.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
                cb.SetBackgroundColor(ColorConstants.YELLOW);
                return cb;
            }
            );
            framework.AssertBothValid("testCheckBoxWithCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxChecked(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(conformance);
                cb.SetChecked(true);
                return cb;
            }
            );
            framework.AssertBothValid("testCheckBoxChecked");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxCheckedAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(conformance);
                cb.GetAccessibilityProperties().SetAlternateDescription("Yello");
                cb.SetChecked(true);
                return cb;
            }
            );
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxCheckedCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(conformance);
                cb.SetChecked(true);
                cb.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                cb.SetBackgroundColor(ColorConstants.GREEN);
                cb.SetCheckBoxType(CheckBoxType.STAR);
                cb.SetSize(20);
                return cb;
            }
            );
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(conformance);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                return checkBox;
            }
            );
            framework.AssertBothValid("testCheckBoxInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractiveCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(conformance);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                checkBox.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                checkBox.SetBackgroundColor(ColorConstants.GREEN);
                checkBox.SetSize(20);
                checkBox.SetCheckBoxType(CheckBoxType.SQUARE);
                return checkBox;
            }
            );
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractiveCustomAppearanceChecked(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(conformance);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                checkBox.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                checkBox.SetBackgroundColor(ColorConstants.GREEN);
                checkBox.SetSize(20);
                checkBox.SetChecked(true);
                checkBox.SetCheckBoxType(CheckBoxType.SQUARE);
                return checkBox;
            }
            );
            framework.AssertBothValid("checkBoxInteractiveCustomAppChecked");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButton(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => new Radio("name"));
            framework.AssertBothValid("testRadioButton");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonChecked(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonChecked");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearanceChecked(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                radio.SetChecked(true);
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroup(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => new Radio("name", "group"));
            framework.AddSuppliers((document) => new Radio("name2", "group"));
            framework.AssertBothValid("testRadioButtonGroup");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
            );
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
            );
            framework.AssertBothValid("testRadioButtonGroupCustom");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearanceChecked(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
            );
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetChecked(true);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
            );
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
            );
            framework.AssertBothValid("testRadioButtonInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCheckedInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonCheckedInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name", "group");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                radio.SetInteractive(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonCustomAppearanceInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearanceCheckedInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name", "Group");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                radio.SetInteractive(true);
                return radio;
            }
            );
            framework.AssertBothValid("radioBtnCustomAppCheckedInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
            );
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name2", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                return r;
            }
            );
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                r.SetBackgroundColor(ColorConstants.GREEN);
                r.SetInteractive(true);
                return r;
            }
            );
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
            );
            framework.AssertBothValid("radioBtnCustomAppInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearanceCheckedInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                r.SetBackgroundColor(ColorConstants.GREEN);
                r.SetInteractive(true);
                return r;
            }
            );
            framework.AddSuppliers((document) => {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetChecked(true);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                r.SetInteractive(true);
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
            );
            framework.AssertBothValid("radioBtnCustomAppGrCheckedInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButton(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(GetFont());
                return b;
            }
            );
            framework.AssertBothValid("testButton");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(GetFont());
                b.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                b.SetBackgroundColor(ColorConstants.GREEN);
                return b;
            }
            );
            framework.AssertBothValid("testButtonCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonSingleLine(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetFont(GetFont());
                b.SetSingleLineValue("Click me?");
                return b;
            }
            );
            framework.AssertBothValid("testButtonSingleLine");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                Paragraph p = new Paragraph("Click me?").SetFont(GetFont()).SetBorder(new SolidBorder(ColorConstants.CYAN, 
                    2));
                b.Add(p);
                return b;
            }
            );
            framework.AssertBothValid("testButtonCustomContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContentIsAlsoForm(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                CheckBox cb = new CheckBox("name2");
                cb.SetChecked(true);
                b.Add(cb);
                return b;
            }
            );
            framework.AssertBothValid("testButtonCustomContentIsAlsoForm");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(GetFont());
                b.SetInteractive(true);
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                return b;
            }
            );
            framework.AssertBothValid("testButtonInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(GetFont());
                b.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                b.SetInteractive(true);
                b.SetBackgroundColor(ColorConstants.GREEN);
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                return b;
            }
            );
            framework.AssertBothValid("testButtonCustomAppearanceInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonSingleLineInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetFont(GetFont());
                b.SetSingleLineValue("Click me?");
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                b.SetInteractive(true);
                return b;
            }
            );
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContentInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                Paragraph p = new Paragraph("Click me?").SetFont(GetFont()).SetBorder(new SolidBorder(ColorConstants.CYAN, 
                    2));
                b.Add(p);
                b.SetFont(GetFont());
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                b.SetInteractive(true);
                return b;
            }
            );
            framework.AssertBothValid("testButtonCustomContentInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContentIsAlsoFormInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetFont(GetFont());
                CheckBox cb = new CheckBox("name2");
                cb.SetChecked(true);
                cb.SetInteractive(true);
                b.Add(cb);
                b.SetInteractive(true);
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                cb.GetAccessibilityProperties().SetAlternateDescription("Check me checkbox");
                return b;
            }
            );
            framework.AssertBothValid("testButtonCustomContentIsAlsoFormInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputField(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                return inputField;
            }
            );
            framework.AssertBothValid("testInputField");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithValue(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                inputField.SetValue("Hello");
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldWithValue");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(GetFont());
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldWithCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndValue(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(GetFont());
                inputField.SetValue("Hello");
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndPlaceHolder(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(GetFont());
                inputField.SetPlaceholder(new Paragraph("Placeholder").SetFont(GetFont()));
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndPlaceHolder");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithValueInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                inputField.SetValue("Hello");
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldWithValueInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(GetFont());
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }
            );
            framework.AssertBothValid("inputFieldCustomAppInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndValueInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(GetFont());
                inputField.SetValue("Hello");
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }
            );
            framework.AssertBothValid("inputFieldCustomAppValueInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndPlaceHolderInteractive(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(GetFont());
                inputField.SetPlaceholder(new Paragraph("Placeholder").SetFont(GetFont()));
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }
            );
            framework.AssertBothValid("inpFieldCustomAppPlaceholderInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextArea(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                return textArea;
            }
            );
            framework.AssertBothValid("testTextArea");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithValue(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                textArea.SetValue("Hello");
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaWithValue");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(GetFont());
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaWithCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndValue(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(GetFont());
                textArea.SetValue("Hello");
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndPlaceHolder(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(GetFont());
                textArea.SetPlaceholder(new Paragraph("Placeholder").SetFont(GetFont()));
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndPlaceHolder");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithValueInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                textArea.SetValue("Hello");
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaWithValueInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(GetFont());
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }
            );
            framework.AssertBothValid("textAreaWithCustomAppearanceInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndValueInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(GetFont());
                textArea.SetValue("Hello");
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }
            );
            framework.AssertBothValid("textAreaCustomAppValueInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndPlaceHolderInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(GetFont());
                textArea.SetPlaceholder(new Paragraph("Placeholder").SetFont(GetFont()));
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }
            );
            framework.AssertBothValid("textAreaCustomAppPlaceHolderInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBox(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(GetFont());
                list.AddOption("value1");
                list.AddOption("value2");
                return list;
            }
            );
            framework.AssertBothValid("testListBox");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(GetFont());
                list.AddOption("value1");
                list.AddOption("value2");
                return list;
            }
            );
            framework.AssertBothValid("testListBoxCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearanceSelected(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(GetFont());
                list.AddOption("value1", true);
                list.AddOption("value2");
                return list;
            }
            );
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(GetFont());
                list.AddOption("value1");
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.AddOption("value2");
                list.SetInteractive(true);
                return list;
            }
            );
            framework.AssertBothValid("testListBoxInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.SetFont(GetFont());
                list.SetInteractive(true);
                list.AddOption("value1");
                list.AddOption("value2");
                return list;
            }
            );
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearanceSelectedInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(GetFont());
                list.SetInteractive(true);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.AddOption("value1", true);
                list.AddOption("value2");
                return list;
            }
            );
            framework.AssertBothValid("listBoxCustomAppSelectedInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBox(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(GetFont());
                list.AddOption(new SelectFieldItem("value1"));
                list.AddOption(new SelectFieldItem("value2"));
                return list;
            }
            );
            framework.AssertBothValid("testComboBox");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(GetFont());
                list.AddOption(new SelectFieldItem("value1"));
                list.AddOption(new SelectFieldItem("value2"));
                return list;
            }
            );
            framework.AssertBothValid("testComboBoxCustomAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearanceSelected(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(GetFont());
                list.AddOption(new SelectFieldItem("Value 1"), true);
                list.AddOption(new SelectFieldItem("Value 1"), false);
                return list;
            }
            );
            framework.AssertBothValid("testComboBoxCustomAppearanceSelected");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(GetFont());
                list.AddOption(new SelectFieldItem("Value 1"));
                list.AddOption(new SelectFieldItem("Value 2"));
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.SetInteractive(true);
                return list;
            }
            );
            framework.AssertBothValid("testComboBoxInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.SetFont(GetFont());
                list.SetInteractive(true);
                list.AddOption(new SelectFieldItem("Value 1"));
                list.AddOption(new SelectFieldItem("Value 2"));
                return list;
            }
            );
            framework.AssertBothValid("comboBoxCustomAppearanceInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearanceSelectedInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(GetFont());
                list.SetInteractive(true);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.AddOption(new SelectFieldItem("hello1"), true);
                list.AddOption(new SelectFieldItem("hello1"), false);
                return list;
            }
            );
            framework.AssertBothValid("comboBoxCustomAppInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearance(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                appearance.SetContent("Hello");
                return appearance;
            }
            );
            framework.AssertBothValid("testSignatureAppearance");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceText(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                return appearance;
            }
            );
            framework.AssertBothValid("signatureAppearanceSignedAppearanceText");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithCustomContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                Div div = new Div();
                div.Add(new Paragraph("Hello").SetFont(GetFont()));
                appearance.SetContent(div);
                return appearance;
            }
            );
            framework.AssertBothValid("signatureAppearanceWithCustomContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                appearance.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                appearance.SetBackgroundColor(ColorConstants.GREEN);
                return appearance;
            }
            );
            framework.AssertBothValid("signAppSignedAppCustomAppText");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                appearance.SetContent("Hello");
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }
            );
            framework.AssertBothValid("testSignatureAppearanceInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceTextInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }
            );
            framework.AssertBothValid("signAppSignedTextInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithCustomContentInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                Div div = new Div();
                div.Add(new Paragraph("Hello").SetFont(GetFont()));
                appearance.SetContent(div);
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }
            );
            framework.AssertBothValid("signedAppearanceTextInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignedAndCustomAppearanceTextInteractive(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                appearance.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                appearance.SetBackgroundColor(ColorConstants.GREEN);
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }
            );
            framework.AssertBothValid("signedCustomAppTextInteractive");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveCheckBoxNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetInteractive(true);
                return cb;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveRadioButtonNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveRadioButtonNoAltDescr", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveRadioButtonNoAltDescr", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveButtonNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetInteractive(true);
                b.SetFont(GetFont());
                return b;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveInputFieldNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetInteractive(true);
                inputField.SetFont(GetFont());
                return inputField;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveInputFieldNoAltDescr", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveInputFieldNoAltDescr", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveTextAreaNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetInteractive(true);
                textArea.SetFont(GetFont());
                return textArea;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveListBoxNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetInteractive(true);
                list.SetFont(GetFont());
                return list;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveComboBoxNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetInteractive(true);
                list.SetFont(GetFont());
                return list;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveSignatureAppearanceNoAlternativeDescription(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetInteractive(true);
                appearance.SetFont(GetFont());
                return appearance;
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("interactiveSignAppearanceNoAltDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("interactiveSignAppearanceNoAltDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(conformance);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
            );
            framework.AssertBothValid("testCheckBoxDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxArtifactRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(conformance);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return cb;
            }
            );
            framework.AssertBothValid("testCheckBoxArtifactRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name1", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
            );
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name2", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
            );
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name3", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonArtifactRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name1", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
            );
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name2", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
            );
            framework.AddSuppliers((document) => {
                Radio radio = new Radio("name3", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
            );
            framework.AssertBothValid("testRadioButtonArtifactRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                b.SetValue("Click me");
                b.GetAccessibilityProperties().SetAlternateDescription("Hello");
                b.SetFont(GetFont());
                return b;
            }
            );
            framework.AddSuppliers((document) => {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                b.SetFont(GetFont());
                return b;
            }
            );
            framework.AssertBothValid("testButtonDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                inputField.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Hello");
                inputField.SetValue("Hello");
                return inputField;
            }
            );
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                inputField.GetAccessibilityProperties().SetRole(StandardRoles.P);
                inputField.SetValue("Hello");
                return inputField;
            }
            );
            framework.AddSuppliers((document) => {
                InputField inputField = new InputField("name");
                inputField.SetFont(GetFont());
                inputField.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                inputField.SetValue("Hello");
                return inputField;
            }
            );
            framework.AssertBothValid("testInputFieldDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                textArea.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return textArea;
            }
            );
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                textArea.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return textArea;
            }
            );
            framework.AddSuppliers((document) => {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(GetFont());
                textArea.GetAccessibilityProperties().SetRole(StandardRoles.P);
                return textArea;
            }
            );
            framework.AssertBothValid("testTextAreaDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(GetFont());
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                return list;
            }
            );
            framework.AddSuppliers((document) => {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(GetFont());
                list.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return list;
            }
            );
            framework.AssertBothValid("testListBoxDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(GetFont());
                list.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                list.AddOption(new SelectFieldItem("value1"));
                list.AddOption(new SelectFieldItem("value2"));
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return list;
            }
            );
            framework.AddSuppliers((document) => {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(GetFont());
                list.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return list;
            }
            );
            framework.AssertBothValid("testComboBoxDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceDifferentRole(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                appearance.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                appearance.SetContent("Hello");
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }
            );
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(GetFont());
                appearance.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                appearance.SetContent("Hello");
                return appearance;
            }
            );
            framework.AssertBothValid("testSignatureAppearanceDifferentRole");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextBuilderWithTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("testTextBuilderWithTu");
            }
            else {
                framework.AssertBothFail("testTextBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextBuilderNoTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("testTextBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("testTextBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderWithTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).CreateComboBox();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("testChoiceBuilderWithTu");
            }
            else {
                framework.AssertBothFail("testChoiceBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderNoTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).CreateComboBox();
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("tesChoicetBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("tesChoicetBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderWithTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreatePushButton();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("testButtonBuilderWithTu");
            }
            else {
                framework.AssertBothFail("testButtonBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreatePushButton();
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("testButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("testButtonBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTuNotVisible(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreatePushButton();
                IList<PdfFormAnnotation> annList = field.GetChildFormAnnotations();
                annList[0].SetVisibility(PdfFormAnnotation.HIDDEN);
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("testButtonBuilderNoTuNotVisible");
            }
            else {
                framework.AssertBothFail("testButtonBuilderNoTuNotVisible", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonBuilderNoTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, "Radio");
                PdfButtonFormField radioGroup = builder.CreateRadioGroup();
                PdfFormAnnotation radioAnnotation = builder.CreateRadioButton("AP", new Rectangle(100, 100, 100, 100));
                PdfFormAnnotation radioAnnotation2 = builder.CreateRadioButton("AP2", new Rectangle(100, 200, 100, 100));
                radioGroup.AddKid(radioAnnotation);
                radioGroup.AddKid(radioAnnotation2);
                form.AddField(radioGroup);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("testRadioButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("testRadioButtonBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonBuilderWithTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, "Radio");
                PdfButtonFormField radioGroup = builder.CreateRadioGroup();
                PdfFormAnnotation radioAnnotation = builder.CreateRadioButton("AP", new Rectangle(100, 100, 100, 100));
                PdfFormAnnotation radioAnnotation2 = builder.CreateRadioButton("AP2", new Rectangle(100, 200, 100, 100));
                radioGroup.AddKid(radioAnnotation);
                radioGroup.AddKid(radioAnnotation2);
                radioGroup.SetAlternativeName("Some radio group");
                form.AddField(radioGroup);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("testRadioButtonBuilderWithTu");
            }
            else {
                framework.AssertBothFail("testRadioButtonBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderWithTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreateSignature();
                field.SetValue("some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("testSignatureBuilderWithTu");
            }
            else {
                framework.AssertBothFail("testSignatureBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderNoTu(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreateSignature();
                field.SetValue("some value");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("testSignatureBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothFail("testSignatureBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldWithAltEntry(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("FormFieldAltDescription");
            }
            else {
                framework.AssertBothFail("FormFieldAltDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldWithContentsEntry(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.GetFirstFormAnnotation().SetAlternativeDescription("Some alt");
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("formFieldContentsDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    );
            }
            else {
                framework.AssertBothValid("formFieldContentsDescription");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldAsStream(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfObject page = pdfDoc.AddNewPage().GetPdfObject();
                PdfStream streamObj = new PdfStream();
                streamObj.Put(PdfName.Subtype, PdfName.Widget);
                streamObj.Put(PdfName.T, new PdfString("hi"));
                streamObj.Put(PdfName.TU, new PdfString("some text"));
                streamObj.Put(PdfName.Contents, new PdfString("hello"));
                streamObj.Put(PdfName.P, page);
                PdfDictionary objRef = new PdfDictionary();
                objRef.Put(PdfName.Obj, streamObj);
                objRef.Put(PdfName.Type, PdfName.OBJR);
                PdfDictionary parentDic = new PdfDictionary();
                parentDic.Put(PdfName.P, pdfDoc.GetStructTreeRoot().GetPdfObject());
                parentDic.Put(PdfName.S, PdfName.Form);
                parentDic.Put(PdfName.Type, PdfName.StructElem);
                parentDic.Put(PdfName.Pg, page);
                PdfArray k = new PdfArray();
                k.Add(objRef);
                parentDic.Put(PdfName.K, k);
                if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                    pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(parentDic));
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new PdfStructElem(parentDic));
                }
            }
            );
            framework.AssertBothValid("FormFieldAsStream");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SeveralWidgetKidsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfObject page = pdfDoc.AddNewPage().GetPdfObject();
                PdfStream streamObj = new PdfStream();
                streamObj.Put(PdfName.Subtype, PdfName.Widget);
                streamObj.Put(PdfName.T, new PdfString("hi"));
                streamObj.Put(PdfName.TU, new PdfString("some text"));
                streamObj.Put(PdfName.Contents, new PdfString("hello"));
                streamObj.Put(PdfName.P, page);
                PdfDictionary objRef = new PdfDictionary();
                objRef.Put(PdfName.Obj, streamObj);
                objRef.Put(PdfName.Type, PdfName.OBJR);
                PdfDictionary parentDic = new PdfDictionary();
                parentDic.Put(PdfName.P, pdfDoc.GetStructTreeRoot().GetPdfObject());
                parentDic.Put(PdfName.S, PdfName.Form);
                parentDic.Put(PdfName.Type, PdfName.StructElem);
                parentDic.Put(PdfName.Pg, page);
                PdfStructElem elem = new PdfStructElem(parentDic);
                elem.AddKid(new PdfStructElem(objRef));
                elem.AddKid(new PdfStructElem(objRef));
                elem.AddKid(new PdfStructElem(objRef));
                if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("severalWidgetKids", PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_WITHOUT_ROLE_SHALL_CONTAIN_ONE_WIDGET
                    );
            }
            else {
                framework.AssertBothFail("severalWidgetKids", PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_SHALL_CONTAIN_AT_MOST_ONE_WIDGET
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SeveralWidgetKidsWithRoleTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfObject page = pdfDoc.AddNewPage().GetPdfObject();
                PdfStream streamObj = new PdfStream();
                streamObj.Put(PdfName.Subtype, PdfName.Widget);
                streamObj.Put(PdfName.T, new PdfString("hi"));
                streamObj.Put(PdfName.TU, new PdfString("some text"));
                streamObj.Put(PdfName.Contents, new PdfString("hello"));
                streamObj.Put(PdfName.P, page);
                PdfDictionary objRef = new PdfDictionary();
                objRef.Put(PdfName.Obj, streamObj);
                objRef.Put(PdfName.Type, PdfName.OBJR);
                PdfDictionary parentDic = new PdfDictionary();
                parentDic.Put(PdfName.P, pdfDoc.GetStructTreeRoot().GetPdfObject());
                parentDic.Put(PdfName.S, PdfName.Form);
                parentDic.Put(PdfName.Type, PdfName.StructElem);
                parentDic.Put(PdfName.Pg, page);
                PdfStructElem elem = new PdfStructElem(parentDic);
                elem.AddKid(new PdfStructElem(objRef));
                elem.AddKid(new PdfStructElem(objRef));
                elem.AddKid(new PdfStructElem(objRef));
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfStructTreeRoot.ConvertRoleToPdfName("PrintField"));
                attributes.Put(PdfStructTreeRoot.ConvertRoleToPdfName("Role"), new PdfName("pb"));
                elem.SetAttributes(attributes);
                if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("severalWidgetKidsWithRole");
            }
            else {
                framework.AssertBothFail("severalWidgetKidsWithRole", PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_SHALL_CONTAIN_AT_MOST_ONE_WIDGET
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetNeitherFormNorArtifactTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfDictionary page = pdfDoc.AddNewPage().GetPdfObject();
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.Contents, new PdfString("hello"));
                widget.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                widget.Put(PdfName.P, page);
                widget.Put(PdfName.StructParent, new PdfNumber(0));
                page.Put(PdfName.Annots, new PdfArray(widget));
                PdfDictionary objRef = new PdfDictionary();
                objRef.Put(PdfName.Obj, widget);
                objRef.Put(PdfName.Type, PdfName.OBJR);
                PdfDictionary parentDic = new PdfDictionary();
                parentDic.Put(PdfName.P, pdfDoc.GetStructTreeRoot().GetPdfObject());
                parentDic.Put(PdfName.S, PdfName.P);
                parentDic.Put(PdfName.Type, PdfName.StructElem);
                parentDic.Put(PdfName.Pg, page);
                parentDic.Put(PdfName.K, objRef);
                ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new PdfStructElem(parentDic));
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("widgetNeitherFormNorArtifact", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                    );
            }
            else {
                // TODO DEVSIX-9580. VeraPDF claims the document to be valid, although it's not.
                //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
                framework.AssertOnlyITextFail("widgetNeitherFormNorArtifact", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetNeitherFormNorArtifactInAcroformTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfDictionary page = pdfDoc.AddNewPage().GetPdfObject();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.Contents, new PdfString("hello"));
                widget.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                widget.Put(PdfName.P, page);
                widget.Put(PdfName.StructParent, new PdfNumber(0));
                widget.MakeIndirect(pdfDoc);
                field.AddKid(PdfFormCreator.CreateFormAnnotation(widget));
                form.AddField(field);
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(page, 0);
                TagTreePointer p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent
                    ());
                p.SetRole(StandardRoles.P);
            }
            );
            framework.AssertBothFail("widgetNeitherFormNorArtifactInAcroform", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetIsArtifactInAcroformTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfDictionary page = pdfDoc.AddNewPage().GetPdfObject();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.Contents, new PdfString("hello"));
                widget.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                widget.Put(PdfName.P, page);
                widget.Put(PdfName.StructParent, new PdfNumber(0));
                widget.MakeIndirect(pdfDoc);
                field.AddKid(PdfFormCreator.CreateFormAnnotation(widget));
                form.AddField(field);
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(page, 0);
                TagTreePointer p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent
                    ());
                p.SetRole(StandardRoles.ARTIFACT);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothFail("widgetIsArtifactInAcroform", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                    );
            }
            else {
                framework.AssertBothValid("widgetIsArtifactInAcroform");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetLabelNoContentsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfObject page = pdfDoc.AddNewPage().GetPdfObject();
                PdfStream streamObj = new PdfStream();
                streamObj.Put(PdfName.Subtype, PdfName.Widget);
                streamObj.Put(PdfName.T, new PdfString("hi"));
                streamObj.Put(PdfName.TU, new PdfString("some text"));
                streamObj.Put(PdfName.P, page);
                PdfDictionary objRef = new PdfDictionary();
                objRef.Put(PdfName.Obj, streamObj);
                objRef.Put(PdfName.Type, PdfName.OBJR);
                PdfDictionary parentDic = new PdfDictionary();
                parentDic.Put(PdfName.P, pdfDoc.GetStructTreeRoot().GetPdfObject());
                parentDic.Put(PdfName.S, PdfName.Form);
                parentDic.Put(PdfName.Type, PdfName.StructElem);
                parentDic.Put(PdfName.Pg, page);
                parentDic.Put(PdfName.K, objRef);
                PdfStructElem elem = new PdfStructElem(parentDic);
                elem.AddKid(new PdfStructElem(pdfDoc, PdfName.Lbl));
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfStructTreeRoot.ConvertRoleToPdfName("PrintField"));
                attributes.Put(PdfStructTreeRoot.ConvertRoleToPdfName("Role"), new PdfName("pb"));
                elem.SetAttributes(attributes);
                if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            framework.AssertBothValid("widgetLabelNoContentsTest");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AdditionalActionAndContentsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfObject page = pdfDoc.AddNewPage().GetPdfObject();
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.T, new PdfString("hi"));
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.Contents, new PdfString("hello"));
                widget.Put(PdfName.AA, new PdfDictionary());
                widget.Put(PdfName.P, page);
                PdfDictionary objRef = new PdfDictionary();
                objRef.Put(PdfName.Obj, widget);
                objRef.Put(PdfName.Type, PdfName.OBJR);
                PdfDictionary parentDic = new PdfDictionary();
                parentDic.Put(PdfName.P, pdfDoc.GetStructTreeRoot().GetPdfObject());
                parentDic.Put(PdfName.S, PdfName.Form);
                parentDic.Put(PdfName.Type, PdfName.StructElem);
                parentDic.Put(PdfName.Pg, page);
                parentDic.Put(PdfName.K, objRef);
                PdfStructElem elem = new PdfStructElem(parentDic);
                if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            framework.AssertBothValid("additionalActionAndContents");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AdditionalActionNoContentsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                TagTreePointer p = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                p.AddTag(StandardRoles.FORM);
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.T, new PdfString("hi"));
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.AA, new PdfDictionary());
                widget.Put(PdfName.P, page.GetPdfObject());
                page.AddAnnotation(PdfAnnotation.MakeAnnotation(widget));
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(page.GetPdfObject(), 0);
                p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent());
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfStructTreeRoot.ConvertRoleToPdfName("PrintField"));
                attributes.Put(PdfStructTreeRoot.ConvertRoleToPdfName("Role"), new PdfName("pb"));
                p.GetProperties().AddAttributes(new PdfStructureAttributes(attributes));
                p.AddTag(StandardRoles.LBL);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("additionalActionNoContents");
            }
            else {
                framework.AssertBothFail("additionalActionNoContents", PdfUAExceptionMessageConstants.WIDGET_WITH_AA_SHALL_PROVIDE_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AdditionalActionNoContentsAcroformTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfDictionary page = pdfDoc.AddNewPage().GetPdfObject();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.AA, new PdfDictionary());
                widget.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                widget.Put(PdfName.P, page);
                widget.Put(PdfName.StructParent, new PdfNumber(0));
                widget.MakeIndirect(pdfDoc);
                field.AddKid(PdfFormCreator.CreateFormAnnotation(widget));
                field.SetAlternativeName("Alt");
                form.AddField(field);
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(page, 0);
                TagTreePointer p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent
                    ());
                PdfDictionary attributes = new PdfDictionary();
                attributes.Put(PdfName.O, PdfStructTreeRoot.ConvertRoleToPdfName("PrintField"));
                attributes.Put(PdfStructTreeRoot.ConvertRoleToPdfName("Role"), new PdfName("pb"));
                p.GetProperties().AddAttributes(new PdfStructureAttributes(attributes));
                p.AddTag(StandardRoles.LBL);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("additionalActionNoContentsAcroform");
            }
            else {
                framework.AssertBothFail("additionalActionNoContentsAcroform", PdfUAExceptionMessageConstants.WIDGET_WITH_AA_SHALL_PROVIDE_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NoContentsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                TagTreePointer p = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                p.AddTag(StandardRoles.FORM);
                PdfDictionary widget = new PdfDictionary();
                widget.Put(PdfName.Subtype, PdfName.Widget);
                widget.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                widget.Put(PdfName.T, new PdfString("hi"));
                widget.Put(PdfName.TU, new PdfString("some text"));
                widget.Put(PdfName.P, page.GetPdfObject());
                page.AddAnnotation(PdfAnnotation.MakeAnnotation(widget));
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("noContents");
            }
            else {
                framework.AssertBothFail("noContents", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVPositiveTest1(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                String value = "Red\rBlue\r";
                field.SetValue(value);
                String richText = "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"color:#FF0000;\">Red&#13;</p>" 
                    + "<p style=\"color:#1E487C;\">Blue&#13;</p></body>";
                field.SetRichText(new PdfString(richText, PdfEncodings.PDF_DOC_ENCODING));
                field.GetFirstFormAnnotation().SetAlternativeDescription("alternate description");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            framework.AssertBothValid("textFieldRVAndVPositiveTest1");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVPositiveTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.SetRichText(new PdfStream("<p>Some value</p>".GetBytes(), CompressionConstants.NO_COMPRESSION));
                field.GetFirstFormAnnotation().SetAlternativeDescription("alternate description");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            framework.AssertBothValid("textFieldRVAndVPositiveTest2");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVPositiveTest3(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                String value = "\n\nThe following word\nis in bold.\n\n";
                field.SetValue(value);
                String richText = "<field1>\n" + "<body xmlns=\"http://www.w3.org/1999/xhtml\">\n" + "<p>The following <span style=\"font-weight:bold\">word</span>\n"
                     + "is in bold.</p>\n" + "</body>\n" + "</field1>";
                field.SetRichText(new PdfString(richText.GetBytes(System.Text.Encoding.UTF8)).SetHexWriting(true));
                field.GetFirstFormAnnotation().SetAlternativeDescription("alternate description");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            framework.AssertBothValid("textFieldRVAndVPositiveTest3");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVNegativeTest1(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetRichText(new PdfString("<p>Some value</p>", PdfEncodings.UTF8));
                field.GetFirstFormAnnotation().SetAlternativeDescription("alternate description");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("textFieldRVAndVNegativeTest1");
            }
            else {
                framework.AssertBothFail("textFieldRVAndVNegativeTest1", PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVNegativeTest2(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.SetRichText(new PdfStream("<p>Some different value</p>".GetBytes(System.Text.Encoding.UTF8), CompressionConstants
                    .NO_COMPRESSION));
                field.GetFirstFormAnnotation().SetAlternativeDescription("alternate description");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("textFieldRVAndVNegativeTest2");
            }
            else {
                framework.AssertBothFail("textFieldRVAndVNegativeTest2", PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVNegativeTest3(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.SetRichText(new PdfString("<p>Some different value</p>"));
                field.GetFirstFormAnnotation().SetAlternativeDescription("alternate description");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            if (conformance.ConformsTo(PdfUAConformance.PDF_UA_1)) {
                framework.AssertBothValid("textFieldRVAndVNegativeTest3");
            }
            else {
                framework.AssertBothFail("textFieldRVAndVNegativeTest3", PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SignatureAppearanceWithImage(PdfConformance conformance) {
            // TODO DEVSIX-9023 Support "Signature fields" UA-2 rules
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                Div div = new Div();
                Image img;
                try {
                    img = new Image(ImageDataFactory.Create(DOG));
                }
                catch (UriFormatException e) {
                    throw new PdfException(e.Message);
                }
                div.Add(img);
                appearance.SetContent(div);
                appearance.SetInteractive(true);
                appearance.SetAlternativeDescription("Alternative Description");
                return appearance;
            }
            );
            framework.AssertBothValid("signatureAppearanceWithImage");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SignatureAppearanceWithLineSeparator(PdfConformance conformance) {
            // TODO DEVSIX-9023 Support "Signature fields" UA-2 rules
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                Div div = new Div();
                LineSeparator line = new LineSeparator(new SolidLine(3));
                div.Add(line);
                appearance.SetContent(div);
                appearance.SetInteractive(true);
                appearance.SetAlternativeDescription("Alternative Description");
                return appearance;
            }
            );
            framework.AssertBothValid("signatureAppearanceLineSep");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SignatureAppearanceBackgroundImage(PdfConformance conformance) {
            // TODO DEVSIX-9023 Support "Signature fields" UA-2 rules
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                try {
                    appearance.SetFont(GetFont());
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(DOG));
                    BackgroundImage backgroundImage = new BackgroundImage.Builder().SetImage(xObject).Build();
                    backgroundImage.GetBackgroundSize().SetBackgroundSizeToValues(UnitValue.CreatePointValue(100), UnitValue.CreatePointValue
                        (100));
                    Div div = new Div();
                    div.Add(new Paragraph("Some text"));
                    appearance.SetContent(div).SetFontSize(50).SetBorder(new SolidBorder(ColorConstants.YELLOW, 10)).SetHeight
                        (200).SetWidth(300);
                    appearance.SetBackgroundImage(backgroundImage);
                    appearance.SetAlternativeDescription("Alternative Description");
                    appearance.SetInteractive(true);
                }
                catch (UriFormatException e) {
                    throw new PdfException(e.Message);
                }
                return appearance;
            }
            );
            framework.AssertBothValid("signatureAppearanceBackgroundImage");
        }

        private PdfFont GetFont() {
            try {
                return PdfFontFactory.CreateFont(FONT);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }
    }
}
