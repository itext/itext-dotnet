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
            framework.AddSuppliers(new _Generator_95());
            framework.AssertBothValid("testCheckBox.pdf");
        }

        private sealed class _Generator_95 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_95() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxWithCustomAppearance() {
            framework.AddSuppliers(new _Generator_106());
            framework.AssertBothValid("testCheckBoxWithCustomAppearance.pdf");
        }

        private sealed class _Generator_106 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_106() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
                cb.SetBackgroundColor(ColorConstants.YELLOW);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxChecked() {
            framework.AddSuppliers(new _Generator_121());
            framework.AssertBothValid("testCheckBox");
        }

        private sealed class _Generator_121 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_121() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.SetChecked(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxCheckedAlternativeDescription() {
            framework.AddSuppliers(new _Generator_135());
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription");
        }

        private sealed class _Generator_135 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_135() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetAlternateDescription("Yello");
                cb.SetChecked(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxCheckedCustomAppearance() {
            framework.AddSuppliers(new _Generator_150());
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance");
        }

        private sealed class _Generator_150 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_150() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
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
            framework.AddSuppliers(new _Generator_168());
            framework.AssertBothValid("testCheckBoxInteractive");
        }

        private sealed class _Generator_168 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_168() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(PdfConformance.PDF_UA_1);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                return checkBox;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckBoxInteractiveCustomAppearance() {
            framework.AddSuppliers(new _Generator_182());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance");
        }

        private sealed class _Generator_182 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_182() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(PdfConformance.PDF_UA_1);
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
            framework.AddSuppliers(new _Generator_200());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearanceChecked");
        }

        private sealed class _Generator_200 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_200() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(PdfConformance.PDF_UA_1);
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
            framework.AddSuppliers(new _Generator_219());
            framework.AssertBothValid("testRadioButton");
        }

        private sealed class _Generator_219 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_219() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonChecked() {
            framework.AddSuppliers(new _Generator_230());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_230 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_230() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonCustomAppearance() {
            framework.AddSuppliers(new _Generator_243());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_243 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_243() {
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
            framework.AddSuppliers(new _Generator_258());
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked");
        }

        private sealed class _Generator_258 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_258() {
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
            framework.AddSuppliers(new _Generator_274());
            framework.AddSuppliers(new _Generator_280());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_274 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_274() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_280 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_280() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonGroupCustomAppearance() {
            framework.AddSuppliers(new _Generator_292());
            framework.AddSuppliers(new _Generator_302());
            framework.AssertBothValid("testRadioButtonGroup");
        }

        private sealed class _Generator_292 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_292() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_302 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_302() {
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
            framework.AddSuppliers(new _Generator_317());
            framework.AddSuppliers(new _Generator_327());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked");
        }

        private sealed class _Generator_317 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_317() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_327 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_327() {
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
            framework.AddSuppliers(new _Generator_344());
            framework.AssertBothValid("testRadioButtonInteractive");
        }

        private sealed class _Generator_344 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_344() {
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
            framework.AddSuppliers(new _Generator_358());
            framework.AssertBothValid("testRadioButtonChecked");
        }

        private sealed class _Generator_358 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_358() {
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
            framework.AddSuppliers(new _Generator_373());
            framework.AssertBothValid("testRadioButtonCustomAppearance");
        }

        private sealed class _Generator_373 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_373() {
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
            framework.AddSuppliers(new _Generator_390());
            framework.AssertBothValid("testRadioButtonCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_390 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_390() {
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
            framework.AddSuppliers(new _Generator_408());
            framework.AddSuppliers(new _Generator_417());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_408 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_408() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_417 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_417() {
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
            framework.AddSuppliers(new _Generator_432());
            framework.AddSuppliers(new _Generator_444());
            framework.AssertBothValid("testRadioButtonGroupInteractive");
        }

        private sealed class _Generator_432 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_432() {
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

        private sealed class _Generator_444 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_444() {
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
            framework.AddSuppliers(new _Generator_461());
            framework.AddSuppliers(new _Generator_473());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceCheckedInteractive");
        }

        private sealed class _Generator_461 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_461() {
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

        private sealed class _Generator_473 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_473() {
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
            framework.AddSuppliers(new _Generator_492(this));
            framework.AssertBothValid("testButton");
        }

        private sealed class _Generator_492 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_492(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_506(this));
            framework.AssertBothValid("testButtonCustomAppearance");
        }

        private sealed class _Generator_506 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_506(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_522(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_522 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_522(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_536(this));
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_536 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_536(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_551());
            framework.AssertBothValid("testButtonSingleLine");
        }

        private sealed class _Generator_551 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_551() {
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
            framework.AddSuppliers(new _Generator_566(this));
            framework.AssertBothValid("testButtonInteractive");
        }

        private sealed class _Generator_566 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_566(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_583(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive");
        }

        private sealed class _Generator_583 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_583(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_602(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_602 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_602(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_619(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_619 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_619(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_637(this));
            framework.AssertBothValid("testButtonSingleLineInteractive");
        }

        private sealed class _Generator_637 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_637(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_657(this));
            framework.AssertBothValid("testInputField");
        }

        private sealed class _Generator_657 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_657(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_670(this));
            framework.AssertBothValid("testInputFieldWithValue");
        }

        private sealed class _Generator_670 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_670(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_684(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearance");
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
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestInputFieldWithCustomAppearanceAndValue() {
            framework.AddSuppliers(new _Generator_699(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_699 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_699(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_715(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_715 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_715(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_731(this));
            framework.AssertBothValid("testInputFieldInteractive");
        }

        private sealed class _Generator_731 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_731(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_746(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive");
        }

        private sealed class _Generator_746 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_746(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_762(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_762 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_762(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_779(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_779 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_779(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_797(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_797 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_797(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_815(this));
            framework.AssertBothValid("testTextArea");
        }

        private sealed class _Generator_815 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_815(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_828(this));
            framework.AssertBothValid("testTextAreaWithValue");
        }

        private sealed class _Generator_828 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_828(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_842(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearance");
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
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.Test]
        public virtual void TestTextAreaWithCustomAppearanceAndValue() {
            framework.AddSuppliers(new _Generator_857(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_857 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_857(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_873(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue");
        }

        private sealed class _Generator_873 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_873(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_889(this));
            framework.AssertBothValid("testTextAreaInteractive");
        }

        private sealed class _Generator_889 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_889(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_904(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive");
        }

        private sealed class _Generator_904 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_904(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_920(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceInteractive");
        }

        private sealed class _Generator_920 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_920(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_937(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValueInteractive");
        }

        private sealed class _Generator_937 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_937(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_955(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndPlaceHolderInteractive");
        }

        private sealed class _Generator_955 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_955(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_973(this));
            framework.AssertBothValid("testListBox");
        }

        private sealed class _Generator_973 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_973(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_988(this));
            framework.AssertBothValid("testListBoxCustomAppearance");
        }

        private sealed class _Generator_988 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_988(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1006(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_1006 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1006(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1024(this));
            framework.AssertBothValid("testListBoxInteractive");
        }

        private sealed class _Generator_1024 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1024(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1041(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1041 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1041(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1061(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1061 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1061(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1081(this));
            framework.AssertBothValid("testComboBox");
        }

        private sealed class _Generator_1081 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1081(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1096(this));
            framework.AssertBothValid("testComboBoxCustomAppearance");
        }

        private sealed class _Generator_1096 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1096(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1114(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected");
        }

        private sealed class _Generator_1114 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1114(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1132(this));
            framework.AssertBothValid("testComboBoxInteractive");
        }

        private sealed class _Generator_1132 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1132(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1149(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceInteractive");
        }

        private sealed class _Generator_1149 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1149(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1169(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceSelectedInteractive");
        }

        private sealed class _Generator_1169 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1169(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1189(this));
            framework.AssertBothValid("testSignatureAppearance");
        }

        private sealed class _Generator_1189 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1189(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1203(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1203 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1203(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1221(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceText");
        }

        private sealed class _Generator_1221 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1221(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1238(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText");
        }

        private sealed class _Generator_1238 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1238(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1258(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive");
        }

        private sealed class _Generator_1258 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1258(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1274(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1274 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1274(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1295(this));
            framework.AssertBothValid("testSignatureAppearanceWithSignedAppearanceTextInteractive");
        }

        private sealed class _Generator_1295 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1295(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1315(this));
            framework.AssertBothValid("testSignedAndCustomAppearanceTextInteractive");
        }

        private sealed class _Generator_1315 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1315(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1337());
            framework.AssertBothFail("testInteractiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1337 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1337() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestInteractiveRadioButtonNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1352());
            framework.AssertBothFail("testInteractiveRadioButtonNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1352 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1352() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestInteractiveButtonNoAlternativeDescription() {
            framework.AddSuppliers(new _Generator_1366(this));
            framework.AssertBothFail("testInteractiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                );
        }

        private sealed class _Generator_1366 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1366(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1381(this));
            framework.AssertBothFail("testInteractiveInputFieldNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1381 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1381(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1396(this));
            framework.AssertBothFail("testInteractiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1396 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1396(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1411(this));
            framework.AssertBothFail("testInteractiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.
                MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1411 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1411(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1426(this));
            framework.AssertBothFail("testInteractiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1426 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1426(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1441(this));
            framework.AssertBothFail("testInteractiveSignatureAppearanceNoAlternativeDescription", PdfUAExceptionMessageConstants
                .MISSING_FORM_FIELD_DESCRIPTION);
        }

        private sealed class _Generator_1441 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1441(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1456());
            framework.AddSuppliers(new _Generator_1466());
            framework.AssertBothValid("testCheckBoxDifferentRole");
        }

        private sealed class _Generator_1456 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1456() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
        }

        private sealed class _Generator_1466 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1466() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRadioButtonDifferentRole() {
            framework.AddSuppliers(new _Generator_1480());
            framework.AddSuppliers(new _Generator_1490());
            framework.AddSuppliers(new _Generator_1500());
            framework.AssertBothValid("testRadioButtonDifferentRole");
        }

        private sealed class _Generator_1480 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1480() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1490 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1490() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1500 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1500() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonDifferentRole() {
            framework.AddSuppliers(new _Generator_1513(this));
            framework.AddSuppliers(new _Generator_1524(this));
            framework.AssertBothValid("testButtonDifferentRole");
        }

        private sealed class _Generator_1513 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1513(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1524 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1524(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1539(this));
            framework.AddSuppliers(new _Generator_1550(this));
            framework.AddSuppliers(new _Generator_1561(this));
            framework.AssertBothValid("testInputFieldDifferentRole");
        }

        private sealed class _Generator_1539 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1539(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1550 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1550(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1561 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1561(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1576(this));
            framework.AddSuppliers(new _Generator_1586(this));
            framework.AddSuppliers(new _Generator_1595(this));
            framework.AssertBothValid("testTextAreaDifferentRole");
        }

        private sealed class _Generator_1576 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1576(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1586 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1586(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1595 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1595(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1610(this));
            framework.AddSuppliers(new _Generator_1620(this));
            framework.AssertBothValid("testListBoxDifferentRole");
        }

        private sealed class _Generator_1610 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1610(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1620 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1620(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1636(this));
            framework.AddSuppliers(new _Generator_1649(this));
            framework.AssertBothValid("testComboBoxDifferentRole");
        }

        private sealed class _Generator_1636 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1636(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1649 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1649(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1663(this));
            framework.AddSuppliers(new _Generator_1675(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole");
        }

        private sealed class _Generator_1663 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1663(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1675 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1675(PdfUAFormFieldsTest _enclosing) {
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
                    , 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateText();
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
                    , 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateText();
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
                    , 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateComboBox();
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
                    , 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateComboBox();
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
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreatePushButton();
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
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreatePushButton();
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
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreatePushButton();
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
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateSignature();
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
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateSignature();
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
                    , 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateText();
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
