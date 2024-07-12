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
            framework.AddSuppliers(new _Generator_84());
            framework.AssertBothValid("testCheckBox.pdf");
        }

        private sealed class _Generator_84 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_84() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxWithCustomAppearance() {
            framework.AddSuppliers(new _Generator_95());
            framework.AssertBothValid("testCheckBoxWithCustomAppearance.pdf");
        }

        private sealed class _Generator_95 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_95() {
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
            framework.AddSuppliers(new _Generator_110());
            framework.AssertBothValid("testCheckBox");
        }

        private sealed class _Generator_110 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_110() {
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
            framework.AddSuppliers(new _Generator_124());
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription");
        }

        private sealed class _Generator_124 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_124() {
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
            framework.AddSuppliers(new _Generator_139());
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance");
        }

        private sealed class _Generator_139 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_139() {
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
            framework.AddSuppliers(new _Generator_157());
            framework.AssertBothValid("testCheckBoxInteractive");
        }

        private sealed class _Generator_157 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_157() {
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
            framework.AddSuppliers(new _Generator_171());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance");
        }

        private sealed class _Generator_171 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_171() {
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
            framework.AddSuppliers(new _Generator_189());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearanceChecked");
        }

        private sealed class _Generator_189 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_189() {
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
            framework.AddSuppliers(new _Generator_208());
            framework.AssertBothValid("testRadioButton");
        }

        private sealed class _Generator_208 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_208() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonChecked() {
            framework.AddSuppliers(new _Generator_219());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_219 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_219() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearance() {
            framework.AddSuppliers(new _Generator_232());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_232 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_232() {
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
            framework.AddSuppliers(new _Generator_247());
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked");
        }

        private sealed class _Generator_247 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_247() {
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
            framework.AddSuppliers(new _Generator_263());
            framework.AddSuppliers(new _Generator_269());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_263 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_263() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_269 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_269() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearance() {
            framework.AddSuppliers(new _Generator_281());
            framework.AddSuppliers(new _Generator_291());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_281 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_281() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_291 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_291() {
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
            framework.AddSuppliers(new _Generator_306());
            framework.AddSuppliers(new _Generator_316());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked");
        }

        private sealed class _Generator_306 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_306() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_316 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_316() {
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
            framework.AddSuppliers(new _Generator_333());
            framework.AssertBothValid("testRadioButtonInteractive");
        }

        private sealed class _Generator_333 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_333() {
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
            framework.AddSuppliers(new _Generator_347());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_347 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_347() {
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
            framework.AddSuppliers(new _Generator_362());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_362 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_362() {
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
            framework.AddSuppliers(new _Generator_379());
            framework.AssertBothValid("testRadioButtonCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_379 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_379() {
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
            framework.AddSuppliers(new _Generator_397());
            framework.AddSuppliers(new _Generator_406());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_397 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_397() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_406 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_406() {
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
            framework.AddSuppliers(new _Generator_421());
            framework.AddSuppliers(new _Generator_433());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_421 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_421() {
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

        private sealed class _Generator_433 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_433() {
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
            framework.AddSuppliers(new _Generator_450());
            framework.AddSuppliers(new _Generator_462());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_450 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_450() {
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

        private sealed class _Generator_462 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_462() {
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
            framework.AddSuppliers(new _Generator_481(this));
            framework.AssertBothValid("testButton");
        }

        private sealed class _Generator_481 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_481(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_495(this));
            framework.AssertBothValid("testButtonCustomAppearance");
        }

        private sealed class _Generator_495 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_495(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_511(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_511 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_511(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_525(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_525 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_525(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_540());
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_540 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_540() {
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
            framework.AddSuppliers(new _Generator_555(this));
            framework.AssertBothValid("testButtonInteractive");
        }

        private sealed class _Generator_555 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_555(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_572(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive");
        }

        private sealed class _Generator_572 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_572(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_591(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_591 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_591(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_608(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_608 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_608(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_626(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_626 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_626(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_646(this));
            framework.AssertBothValid("testInputField");
        }

        private sealed class _Generator_646 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_646(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_659(this));
            framework.AssertBothValid("testInputFieldWithValue");
        }

        private sealed class _Generator_659 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_659(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_673(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearance");
        }

        private sealed class _Generator_673 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_673(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_688(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_688 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_688(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_704(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_704 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_704(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_720(this));
            framework.AssertBothValid("testInputFieldInteractive");
        }

        private sealed class _Generator_720 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_720(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_735(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive");
        }

        private sealed class _Generator_735 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_735(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_751(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_751 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_751(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_768(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_768 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_768(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_786(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_786 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_786(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_804(this));
            framework.AssertBothValid("testTextArea");
        }

        private sealed class _Generator_804 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_804(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_817(this));
            framework.AssertBothValid("testTextAreaWithValue");
        }

        private sealed class _Generator_817 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_817(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_831(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearance");
        }

        private sealed class _Generator_831 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_831(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_846(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_846 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_846(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_862(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_862 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_862(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_878(this));
            framework.AssertBothValid("testTextAreaInteractive");
        }

        private sealed class _Generator_878 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_878(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_893(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive");
        }

        private sealed class _Generator_893 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_893(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_909(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_909 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_909(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_926(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_926 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_926(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_944(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_944 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_944(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_962(this));
            framework.AssertBothValid("testListBox");
        }

        private sealed class _Generator_962 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_962(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_977(this));
            framework.AssertBothValid("testListBoxCustomAppearance");
        }

        private sealed class _Generator_977 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_977(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_995(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_995 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_995(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1013(this));
            framework.AssertBothValid("testListBoxInteractive");
        }

        private sealed class _Generator_1013 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1013(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1030(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1030 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1030(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1050(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1050 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1050(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1070(this));
            framework.AssertBothValid("testComboBox");
        }

        private sealed class _Generator_1070 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1070(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1085(this));
            framework.AssertBothValid("testComboBoxCustomAppearance");
        }

        private sealed class _Generator_1085 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1085(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1103(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_1103 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1103(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1121(this));
            framework.AssertBothValid("testComboBoxInteractive");
        }

        private sealed class _Generator_1121 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1121(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1138(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1138 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1138(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1158(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1158 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1158(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1178(this));
            framework.AssertBothValid("testSignatureAppearance");
        }

        private sealed class _Generator_1178 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1178(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1192(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1192 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1192(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1210(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1210 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1210(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1227(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText");
        }

        private sealed class _Generator_1227 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1227(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1247(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive");
        }

        private sealed class _Generator_1247 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1247(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1263(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1263 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1263(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1284(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1284 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1284(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1304(this));
            framework.AssertBothValid("testSignedAndCustomAppearanceTextInteractive");
        }

        private sealed class _Generator_1304 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1304(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1326());
            framework.AssertBothFail("testInteractiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1326 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1326() {
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
            framework.AddSuppliers(new _Generator_1341());
            framework.AssertBothFail("testInteractiveRadioButtonNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1341 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1341() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestInteractiveButtonNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1355(this));
            framework.AssertBothFail("testInteractiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        private sealed class _Generator_1355 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1355(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1370(this));
            framework.AssertBothFail("testInteractiveInputFieldNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
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
        public virtual void TestInteractiveTextAreaNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1385(this));
            framework.AssertBothFail("testInteractiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1385 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1385(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1400(this));
            framework.AssertBothFail("testInteractiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.
                MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1400 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1400(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1415(this));
            framework.AssertBothFail("testInteractiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1415 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1415(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1430(this));
            framework.AssertBothFail("testInteractiveSignatureAppearanceNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1430 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1430(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1445());
            framework.AddSuppliers(new _Generator_1455());
            framework.AssertBothValid("testCheckBoxDifferentRole");
        }

        private sealed class _Generator_1445 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1445() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
        }

        private sealed class _Generator_1455 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1455() {
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
            framework.AddSuppliers(new _Generator_1469());
            framework.AddSuppliers(new _Generator_1479());
            framework.AddSuppliers(new _Generator_1489());
            framework.AssertBothValid("testRadioButtonDifferentRole");
        }

        private sealed class _Generator_1469 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1469() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1479 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1479() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1489 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1489() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonDifferentRole() {
            framework.AddSuppliers(new _Generator_1502(this));
            framework.AddSuppliers(new _Generator_1513(this));
            framework.AssertBothValid("testButtonDifferentRole");
        }

        private sealed class _Generator_1502 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1502(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1513 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1513(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1528(this));
            framework.AddSuppliers(new _Generator_1539(this));
            framework.AddSuppliers(new _Generator_1550(this));
            framework.AssertBothValid("testInputFieldDifferentRole");
        }

        private sealed class _Generator_1528 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1528(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1539 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1539(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1550 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1550(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1565(this));
            framework.AddSuppliers(new _Generator_1575(this));
            framework.AddSuppliers(new _Generator_1584(this));
            framework.AssertBothValid("testTextAreaDifferentRole");
        }

        private sealed class _Generator_1565 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1565(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1575 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1575(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1584 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1584(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1599(this));
            framework.AddSuppliers(new _Generator_1609(this));
            framework.AssertBothValid("testListBoxDifferentRole");
        }

        private sealed class _Generator_1599 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1599(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1609 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1609(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1625(this));
            framework.AddSuppliers(new _Generator_1638(this));
            framework.AssertBothValid("testComboBoxDifferentRole");
        }

        private sealed class _Generator_1625 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1625(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1638 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1638(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1652(this));
            framework.AddSuppliers(new _Generator_1664(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole");
        }

        private sealed class _Generator_1652 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1652(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1664 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1664(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.Test]
        public virtual void TestFormFieldAsStream() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
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
                pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(parentDic));
            }
            );
            framework.AssertBothValid("FormFieldAsStream");
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
