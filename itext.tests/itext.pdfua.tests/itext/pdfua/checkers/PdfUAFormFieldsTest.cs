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
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
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

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBox() {
            framework.AddSuppliers(new _Generator_80());
            framework.AssertBothValid("testCheckBox.pdf");
        }

        private sealed class _Generator_80 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_80() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxWithCustomAppearance() {
            framework.AddSuppliers(new _Generator_91());
            framework.AssertBothValid("testCheckBoxWithCustomAppearance.pdf");
        }

        private sealed class _Generator_91 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_91() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
                cb.SetBackgroundColor(ColorConstants.YELLOW);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxChecked() {
            framework.AddSuppliers(new _Generator_106());
            framework.AssertBothValid("testCheckBox");
        }

        private sealed class _Generator_106 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_106() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.SetChecked(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxCheckedAlternativeDescription() {
            framework.AddSuppliers(new _Generator_120());
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription");
        }

        private sealed class _Generator_120 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_120() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.GetAccessibilityProperties().SetAlternateDescription("Yello");
                cb.SetChecked(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxCheckedCustomAppearance() {
            framework.AddSuppliers(new _Generator_135());
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance");
        }

        private sealed class _Generator_135 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_135() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.SetChecked(true);
                cb.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                cb.SetBackgroundColor(ColorConstants.GREEN);
                cb.SetCheckBoxType(CheckBoxType.STAR);
                cb.SetSize(20);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxInteractive() {
            framework.AddSuppliers(new _Generator_153());
            framework.AssertBothValid("testCheckBoxInteractive");
        }

        private sealed class _Generator_153 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_153() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                return checkBox;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxInteractiveCustomAppearance() {
            framework.AddSuppliers(new _Generator_167());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance");
        }

        private sealed class _Generator_167 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_167() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                checkBox.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                checkBox.SetBackgroundColor(ColorConstants.GREEN);
                checkBox.SetSize(20);
                checkBox.SetCheckBoxType(CheckBoxType.SQUARE);
                return checkBox;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxInteractiveCustomAppearanceChecked() {
            framework.AddSuppliers(new _Generator_185());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearanceChecked");
        }

        private sealed class _Generator_185 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_185() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                checkBox.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                checkBox.SetBackgroundColor(ColorConstants.GREEN);
                checkBox.SetSize(20);
                checkBox.SetChecked(true);
                checkBox.SetCheckBoxType(CheckBoxType.SQUARE);
                return checkBox;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButton() {
            framework.AddSuppliers(new _Generator_204());
            framework.AssertBothValid("testRadioButton");
        }

        private sealed class _Generator_204 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_204() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonChecked() {
            framework.AddSuppliers(new _Generator_215());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_215 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_215() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearance() {
            framework.AddSuppliers(new _Generator_228());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_228 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_228() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearanceChecked() {
            framework.AddSuppliers(new _Generator_243());
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked");
        }

        private sealed class _Generator_243 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_243() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroup() {
            framework.AddSuppliers(new _Generator_259());
            framework.AddSuppliers(new _Generator_265());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_259 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_259() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_265 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_265() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearance() {
            framework.AddSuppliers(new _Generator_277());
            framework.AddSuppliers(new _Generator_287());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_277 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_277() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_287 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_287() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearanceChecked() {
            framework.AddSuppliers(new _Generator_302());
            framework.AddSuppliers(new _Generator_312());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked");
        }

        private sealed class _Generator_302 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_302() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_312 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_312() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetChecked(true);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonInteractive() {
            framework.AddSuppliers(new _Generator_329());
            framework.AssertBothValid("testRadioButtonInteractive");
        }

        private sealed class _Generator_329 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_329() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCheckedInteractive() {
            framework.AddSuppliers(new _Generator_343());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_343 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_343() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_358());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_358 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_358() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                radio.SetInteractive(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearanceCheckedInteractive() {
            framework.AddSuppliers(new _Generator_375());
            framework.AssertBothValid("testRadioButtonCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_375 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_375() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "Group");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupInteractive() {
            framework.AddSuppliers(new _Generator_393());
            framework.AddSuppliers(new _Generator_402());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_393 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_393() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_402 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_402() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                return r;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_417());
            framework.AddSuppliers(new _Generator_429());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_417 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_417() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                r.SetBackgroundColor(ColorConstants.GREEN);
                r.SetInteractive(true);
                return r;
            }
        }

        private sealed class _Generator_429 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_429() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearanceCheckedInteractive() {
            framework.AddSuppliers(new _Generator_446());
            framework.AddSuppliers(new _Generator_458());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_446 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_446() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                r.SetBackgroundColor(ColorConstants.GREEN);
                r.SetInteractive(true);
                return r;
            }
        }

        private sealed class _Generator_458 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_458() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetChecked(true);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                r.SetInteractive(true);
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestButton() {
            framework.AddSuppliers(new _Generator_477(this));
            framework.AssertBothValid("testButton");
        }

        private sealed class _Generator_477 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_477(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(this._enclosing.GetFont());
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonCustomAppearance() {
            framework.AddSuppliers(new _Generator_491(this));
            framework.AssertBothValid("testButtonCustomAppearance");
        }

        private sealed class _Generator_491 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_491(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(this._enclosing.GetFont());
                b.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                b.SetBackgroundColor(ColorConstants.GREEN);
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonSingleLine() {
            framework.AddSuppliers(new _Generator_507(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_507 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_507(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetFont(this._enclosing.GetFont());
                b.SetSingleLineValue("Click me?");
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonCustomContent() {
            framework.AddSuppliers(new _Generator_521(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_521 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_521(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                Paragraph p = new Paragraph("Click me?").SetFont(this._enclosing.GetFont()).SetBorder(new SolidBorder(ColorConstants
                    .CYAN, 2));
                b.Add(p);
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonCustomContentIsAlsoForm() {
            framework.AddSuppliers(new _Generator_536());
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_536 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_536() {
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                CheckBox cb = new CheckBox("name2");
                cb.SetChecked(true);
                b.Add(cb);
                return b;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonInteractive() {
            framework.AddSuppliers(new _Generator_551(this));
            framework.AssertBothValid("testButtonInteractive");
        }

        private sealed class _Generator_551 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_551(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(this._enclosing.GetFont());
                b.SetInteractive(true);
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_568(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive");
        }

        private sealed class _Generator_568 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_568(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.SetFont(this._enclosing.GetFont());
                b.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                b.SetInteractive(true);
                b.SetBackgroundColor(ColorConstants.GREEN);
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonSingleLineInteractive() {
            framework.AddSuppliers(new _Generator_587(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_587 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_587(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetFont(this._enclosing.GetFont());
                b.SetSingleLineValue("Click me?");
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                b.SetInteractive(true);
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonCustomContentInteractive() {
            framework.AddSuppliers(new _Generator_604(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_604 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_604(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                Paragraph p = new Paragraph("Click me?").SetFont(this._enclosing.GetFont()).SetBorder(new SolidBorder(ColorConstants
                    .CYAN, 2));
                b.Add(p);
                b.SetFont(this._enclosing.GetFont());
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                b.SetInteractive(true);
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonCustomContentIsAlsoFormInteractive() {
            framework.AddSuppliers(new _Generator_622(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_622 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_622(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetFont(this._enclosing.GetFont());
                CheckBox cb = new CheckBox("name2");
                cb.SetChecked(true);
                cb.SetInteractive(true);
                b.Add(cb);
                b.SetInteractive(true);
                b.GetAccessibilityProperties().SetAlternateDescription("Click me button");
                cb.GetAccessibilityProperties().SetAlternateDescription("Check me checkbox");
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputField() {
            framework.AddSuppliers(new _Generator_642(this));
            framework.AssertBothValid("testInputField");
        }

        private sealed class _Generator_642 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_642(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithValue() {
            framework.AddSuppliers(new _Generator_655(this));
            framework.AssertBothValid("testInputFieldWithValue");
        }

        private sealed class _Generator_655 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_655(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetValue("Hello");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearance() {
            framework.AddSuppliers(new _Generator_669(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearance");
        }

        private sealed class _Generator_669 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_669(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(this._enclosing.GetFont());
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearanceAndValue() {
            framework.AddSuppliers(new _Generator_684(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_684 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_684(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetValue("Hello");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearanceAndPlaceHolder() {
            framework.AddSuppliers(new _Generator_700(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_700 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_700(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetPlaceholder(new Paragraph("Placeholder").SetFont(this._enclosing.GetFont()));
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldInteractive() {
            framework.AddSuppliers(new _Generator_716(this));
            framework.AssertBothValid("testInputFieldInteractive");
        }

        private sealed class _Generator_716 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_716(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithValueInteractive() {
            framework.AddSuppliers(new _Generator_731(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive");
        }

        private sealed class _Generator_731 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_731(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetValue("Hello");
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_747(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_747 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_747(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearanceAndValueInteractive() {
            framework.AddSuppliers(new _Generator_764(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_764 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_764(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetValue("Hello");
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearanceAndPlaceHolderInteractive() {
            framework.AddSuppliers(new _Generator_782(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_782 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_782(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                inputField.SetBackgroundColor(ColorConstants.GREEN);
                inputField.SetFont(this._enclosing.GetFont());
                inputField.SetPlaceholder(new Paragraph("Placeholder").SetFont(this._enclosing.GetFont()));
                inputField.SetInteractive(true);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextArea() {
            framework.AddSuppliers(new _Generator_800(this));
            framework.AssertBothValid("testTextArea");
        }

        private sealed class _Generator_800 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_800(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithValue() {
            framework.AddSuppliers(new _Generator_813(this));
            framework.AssertBothValid("testTextAreaWithValue");
        }

        private sealed class _Generator_813 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_813(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetValue("Hello");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearance() {
            framework.AddSuppliers(new _Generator_827(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearance");
        }

        private sealed class _Generator_827 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_827(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(this._enclosing.GetFont());
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearanceAndValue() {
            framework.AddSuppliers(new _Generator_842(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_842 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_842(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetValue("Hello");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearanceAndPlaceHolder() {
            framework.AddSuppliers(new _Generator_858(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_858 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_858(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetPlaceholder(new Paragraph("Placeholder").SetFont(this._enclosing.GetFont()));
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaInteractive() {
            framework.AddSuppliers(new _Generator_874(this));
            framework.AssertBothValid("testTextAreaInteractive");
        }

        private sealed class _Generator_874 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_874(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithValueInteractive() {
            framework.AddSuppliers(new _Generator_889(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive");
        }

        private sealed class _Generator_889 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_889(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetValue("Hello");
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_905(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_905 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_905(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearanceAndValueInteractive() {
            framework.AddSuppliers(new _Generator_922(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_922 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_922(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetValue("Hello");
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearanceAndPlaceHolderInteractive() {
            framework.AddSuppliers(new _Generator_940(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_940 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_940(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                textArea.SetBackgroundColor(ColorConstants.GREEN);
                textArea.SetFont(this._enclosing.GetFont());
                textArea.SetPlaceholder(new Paragraph("Placeholder").SetFont(this._enclosing.GetFont()));
                textArea.SetInteractive(true);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Name of the cat");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBox() {
            framework.AddSuppliers(new _Generator_958(this));
            framework.AssertBothValid("testListBox");
        }

        private sealed class _Generator_958 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_958(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(this._enclosing.GetFont());
                list.AddOption("value1");
                list.AddOption("value2");
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxCustomAppearance() {
            framework.AddSuppliers(new _Generator_973(this));
            framework.AssertBothValid("testListBoxCustomAppearance");
        }

        private sealed class _Generator_973 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_973(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(this._enclosing.GetFont());
                list.AddOption("value1");
                list.AddOption("value2");
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxCustomAppearanceSelected() {
            framework.AddSuppliers(new _Generator_991(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_991 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_991(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(this._enclosing.GetFont());
                list.AddOption("value1", true);
                list.AddOption("value2");
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxInteractive() {
            framework.AddSuppliers(new _Generator_1009(this));
            framework.AssertBothValid("testListBoxInteractive");
        }

        private sealed class _Generator_1009 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1009(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(this._enclosing.GetFont());
                list.AddOption("value1");
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.AddOption("value2");
                list.SetInteractive(true);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_1026(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1026 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1026(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.SetFont(this._enclosing.GetFont());
                list.SetInteractive(true);
                list.AddOption("value1");
                list.AddOption("value2");
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxCustomAppearanceSelectedInteractive() {
            framework.AddSuppliers(new _Generator_1046(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1046 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1046(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(this._enclosing.GetFont());
                list.SetInteractive(true);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.AddOption("value1", true);
                list.AddOption("value2");
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBox() {
            framework.AddSuppliers(new _Generator_1066(this));
            framework.AssertBothValid("testComboBox");
        }

        private sealed class _Generator_1066 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1066(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(this._enclosing.GetFont());
                list.AddOption(new SelectFieldItem("value1"));
                list.AddOption(new SelectFieldItem("value2"));
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxCustomAppearance() {
            framework.AddSuppliers(new _Generator_1081(this));
            framework.AssertBothValid("testComboBoxCustomAppearance");
        }

        private sealed class _Generator_1081 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1081(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(this._enclosing.GetFont());
                list.AddOption(new SelectFieldItem("value1"));
                list.AddOption(new SelectFieldItem("value2"));
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxCustomAppearanceSelected() {
            framework.AddSuppliers(new _Generator_1099(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_1099 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1099(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(this._enclosing.GetFont());
                list.AddOption(new SelectFieldItem("Value 1"), true);
                list.AddOption(new SelectFieldItem("Value 1"), false);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxInteractive() {
            framework.AddSuppliers(new _Generator_1117(this));
            framework.AssertBothValid("testComboBoxInteractive");
        }

        private sealed class _Generator_1117 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1117(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(this._enclosing.GetFont());
                list.AddOption(new SelectFieldItem("Value 1"));
                list.AddOption(new SelectFieldItem("Value 2"));
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.SetInteractive(true);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxCustomAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_1134(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1134 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1134(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.SetFont(this._enclosing.GetFont());
                list.SetInteractive(true);
                list.AddOption(new SelectFieldItem("Value 1"));
                list.AddOption(new SelectFieldItem("Value 2"));
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxCustomAppearanceSelectedInteractive() {
            framework.AddSuppliers(new _Generator_1154(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1154 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1154(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetBackgroundColor(ColorConstants.GREEN);
                list.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                list.SetSize(200);
                list.SetFont(this._enclosing.GetFont());
                list.SetInteractive(true);
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.AddOption(new SelectFieldItem("hello1"), true);
                list.AddOption(new SelectFieldItem("hello1"), false);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearance() {
            framework.AddSuppliers(new _Generator_1174(this));
            framework.AssertBothValid("testSignatureAppearance");
        }

        private sealed class _Generator_1174 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1174(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                appearance.SetContent("Hello");
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceWithSignedAppearanceText() {
            framework.AddSuppliers(new _Generator_1188(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1188 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1188(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceWithCustomContent() {
            framework.AddSuppliers(new _Generator_1206(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1206 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1206(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                Div div = new Div();
                div.Add(new Paragraph("Hello").SetFont(this._enclosing.GetFont()));
                appearance.SetContent(div);
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText() {
            framework.AddSuppliers(new _Generator_1223(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText");
        }

        private sealed class _Generator_1223 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1223(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                appearance.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                appearance.SetBackgroundColor(ColorConstants.GREEN);
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceInteractive() {
            framework.AddSuppliers(new _Generator_1243(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive");
        }

        private sealed class _Generator_1243 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1243(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                appearance.SetContent("Hello");
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceWithSignedAppearanceTextInteractive() {
            framework.AddSuppliers(new _Generator_1259(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1259 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1259(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                SignedAppearanceText signedAppearanceText = new SignedAppearanceText();
                signedAppearanceText.SetLocationLine("Location");
                signedAppearanceText.SetSignedBy("Leelah");
                signedAppearanceText.SetReasonLine("Cuz I can");
                appearance.SetContent(signedAppearanceText);
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceWithCustomContentInteractive() {
            framework.AddSuppliers(new _Generator_1280(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1280 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1280(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                Div div = new Div();
                div.Add(new Paragraph("Hello").SetFont(this._enclosing.GetFont()));
                appearance.SetContent(div);
                appearance.SetInteractive(true);
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceWithSignedAppearanceAndCustomAppearanceTextInteractive() {
            framework.AddSuppliers(new _Generator_1300(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceAndCustomAppearanceTextInteractive");
        }

        private sealed class _Generator_1300 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1300(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
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

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveCheckBoxNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1323());
            framework.AssertBothFail("testInteractiveCheckBoxNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1323 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1323() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveRadioButtonNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1339());
            framework.AssertBothFail("testInteractiveRadioButtonNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1339 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1339() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveButtonNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1354(this));
            framework.AssertBothFail("testInteractiveButtonNoAlternativedDescription", PdfUAExceptionMessageConstants.
                FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1354 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1354(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetInteractive(true);
                b.SetFont(this._enclosing.GetFont());
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveInputFieldNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1370(this));
            framework.AssertBothFail("testInteractiveInputFieldNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1370 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1370(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetInteractive(true);
                inputField.SetFont(this._enclosing.GetFont());
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveTextAreaNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1386(this));
            framework.AssertBothFail("testInteractiveTextAreaNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1386 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1386(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetInteractive(true);
                textArea.SetFont(this._enclosing.GetFont());
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveListBoxNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1402(this));
            framework.AssertBothFail("testInteractiveListBoxNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1402 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1402(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetInteractive(true);
                list.SetFont(this._enclosing.GetFont());
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveComboBoxNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1418(this));
            framework.AssertBothFail("testInteractiveComboBoxNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1418 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1418(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetInteractive(true);
                list.SetFont(this._enclosing.GetFont());
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-8160")]
        public virtual void TestInteractiveSignatureAppearanceNoAlternativedDescription() {
            framework.AddSuppliers(new _Generator_1434(this));
            framework.AssertBothFail("testInteractiveSignatureAppearanceNoAlternativedDescription", PdfUAExceptionMessageConstants
                .FORM_FIELD_SHALL_CONTAIN_ALT_ENTRY);
        }

        private sealed class _Generator_1434 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1434(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetInteractive(true);
                appearance.SetFont(this._enclosing.GetFont());
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxDifferentRole() {
            framework.AddSuppliers(new _Generator_1449());
            framework.AddSuppliers(new _Generator_1459());
            framework.AssertBothValid("testCheckBoxDifferentRole");
        }

        private sealed class _Generator_1449 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1449() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
        }

        private sealed class _Generator_1459 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1459() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonDifferentRole() {
            framework.AddSuppliers(new _Generator_1473());
            framework.AddSuppliers(new _Generator_1483());
            framework.AddSuppliers(new _Generator_1493());
            framework.AssertBothValid("testRadioButtonDifferentRole");
        }

        private sealed class _Generator_1473 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1473() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1483 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1483() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1493 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1493() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonDifferentRole() {
            framework.AddSuppliers(new _Generator_1506(this));
            framework.AddSuppliers(new _Generator_1517(this));
            framework.AssertBothValid("testButtonDifferentRole");
        }

        private sealed class _Generator_1506 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1506(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                b.SetValue("Click me");
                b.GetAccessibilityProperties().SetAlternateDescription("Hello");
                b.SetFont(this._enclosing.GetFont());
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1517 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1517(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                b.SetValue("Click me");
                b.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                b.SetFont(this._enclosing.GetFont());
                return b;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldDifferentRole() {
            framework.AddSuppliers(new _Generator_1532(this));
            framework.AddSuppliers(new _Generator_1543(this));
            framework.AddSuppliers(new _Generator_1554(this));
            framework.AssertBothValid("testInputFieldDifferentRole");
        }

        private sealed class _Generator_1532 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1532(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                inputField.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                inputField.GetAccessibilityProperties().SetAlternateDescription("Hello");
                inputField.SetValue("Hello");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1543 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1543(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                inputField.GetAccessibilityProperties().SetRole(StandardRoles.P);
                inputField.SetValue("Hello");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1554 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1554(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                inputField.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                inputField.SetValue("Hello");
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaDifferentRole() {
            framework.AddSuppliers(new _Generator_1569(this));
            framework.AddSuppliers(new _Generator_1579(this));
            framework.AddSuppliers(new _Generator_1588(this));
            framework.AssertBothValid("testTextAreaDifferentRole");
        }

        private sealed class _Generator_1569 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1569(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                textArea.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                textArea.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1579 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1579(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                textArea.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1588 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1588(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                textArea.GetAccessibilityProperties().SetRole(StandardRoles.P);
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestListBoxDifferentRole() {
            framework.AddSuppliers(new _Generator_1603(this));
            framework.AddSuppliers(new _Generator_1613(this));
            framework.AssertBothValid("testListBoxDifferentRole");
        }

        private sealed class _Generator_1603 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1603(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(this._enclosing.GetFont());
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                list.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1613 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1613(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ListBoxField list = new ListBoxField("name", 1, false);
                list.SetFont(this._enclosing.GetFont());
                list.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxDifferentRole() {
            framework.AddSuppliers(new _Generator_1629(this));
            framework.AddSuppliers(new _Generator_1642(this));
            framework.AssertBothValid("testComboBoxDifferentRole");
        }

        private sealed class _Generator_1629 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1629(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(this._enclosing.GetFont());
                list.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                list.AddOption(new SelectFieldItem("value1"));
                list.AddOption(new SelectFieldItem("value2"));
                list.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1642 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1642(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                ComboBoxField list = new ComboBoxField("name");
                list.SetFont(this._enclosing.GetFont());
                list.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return list;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureAppearanceDifferentRole() {
            framework.AddSuppliers(new _Generator_1656(this));
            framework.AddSuppliers(new _Generator_1668(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole");
        }

        private sealed class _Generator_1656 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1656(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                appearance.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                appearance.SetContent("Hello");
                appearance.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private sealed class _Generator_1668 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1668(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                appearance.SetFont(this._enclosing.GetFont());
                appearance.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                appearance.SetContent("Hello");
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        private PdfFont GetFont() {
            try {
                return PdfFontFactory.CreateFont(FONT);
            }
            catch (System.IO.IOException) {
                throw new Exception();
            }
        }
    }
}
