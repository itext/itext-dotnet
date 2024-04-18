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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
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
            framework.AddSuppliers(new _Generator_83());
            framework.AssertBothValid("testCheckBox.pdf");
        }

        private sealed class _Generator_83 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_83() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxWithCustomAppearance() {
            framework.AddSuppliers(new _Generator_94());
            framework.AssertBothValid("testCheckBoxWithCustomAppearance.pdf");
        }

        private sealed class _Generator_94 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_94() {
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
            framework.AddSuppliers(new _Generator_109());
            framework.AssertBothValid("testCheckBox");
        }

        private sealed class _Generator_109 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_109() {
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
            framework.AddSuppliers(new _Generator_123());
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription");
        }

        private sealed class _Generator_123 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_123() {
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
            framework.AddSuppliers(new _Generator_138());
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance");
        }

        private sealed class _Generator_138 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_138() {
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
            framework.AddSuppliers(new _Generator_156());
            framework.AssertBothValid("testCheckBoxInteractive");
        }

        private sealed class _Generator_156 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_156() {
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
            framework.AddSuppliers(new _Generator_170());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance");
        }

        private sealed class _Generator_170 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_170() {
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
            framework.AddSuppliers(new _Generator_188());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearanceChecked");
        }

        private sealed class _Generator_188 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_188() {
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
            framework.AddSuppliers(new _Generator_207());
            framework.AssertBothValid("testRadioButton");
        }

        private sealed class _Generator_207 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_207() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonChecked() {
            framework.AddSuppliers(new _Generator_218());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_218 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_218() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearance() {
            framework.AddSuppliers(new _Generator_231());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_231 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_231() {
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
            framework.AddSuppliers(new _Generator_246());
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked");
        }

        private sealed class _Generator_246 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_246() {
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
            framework.AddSuppliers(new _Generator_262());
            framework.AddSuppliers(new _Generator_268());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_262 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_262() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_268 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_268() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearance() {
            framework.AddSuppliers(new _Generator_280());
            framework.AddSuppliers(new _Generator_290());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_280 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_280() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_290 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_290() {
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
            framework.AddSuppliers(new _Generator_305());
            framework.AddSuppliers(new _Generator_315());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked");
        }

        private sealed class _Generator_305 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_305() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_315 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_315() {
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
            framework.AddSuppliers(new _Generator_332());
            framework.AssertBothValid("testRadioButtonInteractive");
        }

        private sealed class _Generator_332 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_332() {
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
            framework.AddSuppliers(new _Generator_346());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_346 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_346() {
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
            framework.AddSuppliers(new _Generator_361());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_361 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_361() {
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
            framework.AddSuppliers(new _Generator_378());
            framework.AssertBothValid("testRadioButtonCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_378 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_378() {
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
            framework.AddSuppliers(new _Generator_396());
            framework.AddSuppliers(new _Generator_405());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_396 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_396() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_405 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_405() {
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
            framework.AddSuppliers(new _Generator_420());
            framework.AddSuppliers(new _Generator_432());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_420 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_420() {
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

        private sealed class _Generator_432 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_432() {
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
            framework.AddSuppliers(new _Generator_449());
            framework.AddSuppliers(new _Generator_461());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_449 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_449() {
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

        private sealed class _Generator_461 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_461() {
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
            framework.AddSuppliers(new _Generator_480(this));
            framework.AssertBothValid("testButton");
        }

        private sealed class _Generator_480 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_480(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_494(this));
            framework.AssertBothValid("testButtonCustomAppearance");
        }

        private sealed class _Generator_494 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_494(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_510(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_510 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_510(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_524(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_524 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_524(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_539());
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_539 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_539() {
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
            framework.AddSuppliers(new _Generator_554(this));
            framework.AssertBothValid("testButtonInteractive");
        }

        private sealed class _Generator_554 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_554(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_571(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive");
        }

        private sealed class _Generator_571 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_571(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_590(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_590 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_590(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_607(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_607 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_607(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_625(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_625 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_625(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_645(this));
            framework.AssertBothValid("testInputField");
        }

        private sealed class _Generator_645 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_645(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_658(this));
            framework.AssertBothValid("testInputFieldWithValue");
        }

        private sealed class _Generator_658 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_658(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_672(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearance");
        }

        private sealed class _Generator_672 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_672(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_687(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_687 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_687(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_703(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_703 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_703(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_719(this));
            framework.AssertBothValid("testInputFieldInteractive");
        }

        private sealed class _Generator_719 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_719(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_734(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive");
        }

        private sealed class _Generator_734 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_734(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_750(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_750 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_750(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_767(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_767 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_767(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_785(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_785 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_785(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_803(this));
            framework.AssertBothValid("testTextArea");
        }

        private sealed class _Generator_803 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_803(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_816(this));
            framework.AssertBothValid("testTextAreaWithValue");
        }

        private sealed class _Generator_816 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_816(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_830(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearance");
        }

        private sealed class _Generator_830 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_830(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_845(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_845 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_845(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_861(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_861 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_861(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_877(this));
            framework.AssertBothValid("testTextAreaInteractive");
        }

        private sealed class _Generator_877 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_877(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_892(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive");
        }

        private sealed class _Generator_892 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_892(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_908(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_908 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_908(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_925(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_925 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_925(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_943(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_943 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_943(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_961(this));
            framework.AssertBothValid("testListBox");
        }

        private sealed class _Generator_961 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_961(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_976(this));
            framework.AssertBothValid("testListBoxCustomAppearance");
        }

        private sealed class _Generator_976 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_976(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_994(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_994 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_994(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1012(this));
            framework.AssertBothValid("testListBoxInteractive");
        }

        private sealed class _Generator_1012 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1012(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1029(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1029 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1029(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1049(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1049 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1049(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1069(this));
            framework.AssertBothValid("testComboBox");
        }

        private sealed class _Generator_1069 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1069(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1084(this));
            framework.AssertBothValid("testComboBoxCustomAppearance");
        }

        private sealed class _Generator_1084 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1084(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1102(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_1102 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1102(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1120(this));
            framework.AssertBothValid("testComboBoxInteractive");
        }

        private sealed class _Generator_1120 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1120(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1137(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1137 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1137(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1157(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1157 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1157(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1177(this));
            framework.AssertBothValid("testSignatureAppearance");
        }

        private sealed class _Generator_1177 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1177(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1191(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1191 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1191(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1209(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1209 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1209(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1226(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText");
        }

        private sealed class _Generator_1226 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1226(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1246(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive");
        }

        private sealed class _Generator_1246 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1246(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1262(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1262 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1262(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1283(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1283 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1283(PdfUAFormFieldsTest _enclosing) {
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
        public virtual void TestSignedAndCustomAppearanceTextInteractive() {
            framework.AddSuppliers(new _Generator_1303(this));
            framework.AssertBothValid("testSignedAndCustomAppearanceTextInteractive");
        }

        private sealed class _Generator_1303 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1303(PdfUAFormFieldsTest _enclosing) {
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
        public virtual void TestInteractiveCheckBoxNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1325());
            framework.AssertBothFail("testInteractiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1325 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1325() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestInteractiveRadioButtonNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1340());
            framework.AssertBothFail("testInteractiveRadioButtonNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1340 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1340() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestInteractiveButtonNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1354(this));
            framework.AssertBothFail("testInteractiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
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
        public virtual void TestInteractiveInputFieldNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1369(this));
            framework.AssertBothFail("testInteractiveInputFieldNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1369 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1369(PdfUAFormFieldsTest _enclosing) {
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
        public virtual void TestInteractiveTextAreaNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1384(this));
            framework.AssertBothFail("testInteractiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1384 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1384(PdfUAFormFieldsTest _enclosing) {
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
        public virtual void TestInteractiveListBoxNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1399(this));
            framework.AssertBothFail("testInteractiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.
                MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1399 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1399(PdfUAFormFieldsTest _enclosing) {
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
        public virtual void TestInteractiveComboBoxNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1414(this));
            framework.AssertBothFail("testInteractiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1414 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1414(PdfUAFormFieldsTest _enclosing) {
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
        public virtual void TestInteractiveSignatureAppearanceNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1429(this));
            framework.AssertBothFail("testInteractiveSignatureAppearanceNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1429 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1429(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1444());
            framework.AddSuppliers(new _Generator_1454());
            framework.AssertBothValid("testCheckBoxDifferentRole");
        }

        private sealed class _Generator_1444 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1444() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
        }

        private sealed class _Generator_1454 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1454() {
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
            framework.AddSuppliers(new _Generator_1468());
            framework.AddSuppliers(new _Generator_1478());
            framework.AddSuppliers(new _Generator_1488());
            framework.AssertBothValid("testRadioButtonDifferentRole");
        }

        private sealed class _Generator_1468 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1468() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1478 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1478() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1488 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1488() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonDifferentRole() {
            framework.AddSuppliers(new _Generator_1501(this));
            framework.AddSuppliers(new _Generator_1512(this));
            framework.AssertBothValid("testButtonDifferentRole");
        }

        private sealed class _Generator_1501 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1501(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1512 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1512(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1527(this));
            framework.AddSuppliers(new _Generator_1538(this));
            framework.AddSuppliers(new _Generator_1549(this));
            framework.AssertBothValid("testInputFieldDifferentRole");
        }

        private sealed class _Generator_1527 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1527(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1538 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1538(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1549 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1549(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1564(this));
            framework.AddSuppliers(new _Generator_1574(this));
            framework.AddSuppliers(new _Generator_1583(this));
            framework.AssertBothValid("testTextAreaDifferentRole");
        }

        private sealed class _Generator_1564 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1564(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1574 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1574(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1583 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1583(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1598(this));
            framework.AddSuppliers(new _Generator_1608(this));
            framework.AssertBothValid("testListBoxDifferentRole");
        }

        private sealed class _Generator_1598 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1598(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1608 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1608(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1624(this));
            framework.AddSuppliers(new _Generator_1637(this));
            framework.AssertBothValid("testComboBoxDifferentRole");
        }

        private sealed class _Generator_1624 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1624(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1637 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1637(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1651(this));
            framework.AddSuppliers(new _Generator_1663(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole");
        }

        private sealed class _Generator_1651 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1651(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1663 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1663(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.Test]
        public virtual void TestTextBuilderWithTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateText();
                field.SetValue("Some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            framework.AssertBothValid("testTextBuilderWithTu");
        }

        [NUnit.Framework.Test]
        public virtual void TestTextBuilderNoTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateText();
                field.SetValue("Some value");
                form.AddField(field);
            }
            );
            framework.AssertBothFail("testTextBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestChoiceBuilderWithTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateComboBox
                    ();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            framework.AssertBothValid("testChoiceBuilderWithTu");
        }

        [NUnit.Framework.Test]
        public virtual void TestChoiceBuilderNoTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateComboBox
                    ();
                form.AddField(field);
            }
            );
            framework.AssertBothFail("tesChoicetBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonBuilderWithTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreatePushButton
                    ();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            framework.AssertBothValid("testButtonBuilderWithTu");
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonBuilderNoTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreatePushButton
                    ();
                form.AddField(field);
            }
            );
            framework.AssertBothFail("testButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonBuilderNoTuNotVisible() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreatePushButton
                    ();
                IList<PdfFormAnnotation> annList = field.GetChildFormAnnotations();
                annList[0].SetVisibility(PdfFormAnnotation.HIDDEN);
                form.AddField(field);
            }
            );
            framework.AssertBothValid("testButtonBuilderNoTuNotVisible");
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonBuilderNoTu() {
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
            framework.AssertBothFail("testRadioButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonBuilderWithTu() {
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
            framework.AssertBothValid("testRadioButtonBuilderWithTu");
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureBuilderWithTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateSignature
                    ();
                field.SetValue("some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            framework.AssertBothValid("testSignatureBuilderWithTu");
        }

        [NUnit.Framework.Test]
        public virtual void TestSignatureBuilderNoTu() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateSignature
                    ();
                field.SetValue("some value");
                form.AddField(field);
            }
            );
            framework.AssertBothFail("testSignatureBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestFormFieldWithAltEntry() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateText();
                field.SetValue("Some value");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(new DefaultAccessibilityProperties(StandardRoles
                    .FORM).SetAlternateDescription("alternate description"));
                form.AddField(field);
            }
            );
            framework.AssertBothValid("FormFieldAltDescription");
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
