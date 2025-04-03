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
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
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

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUATest/PdfUAFormFieldTest/";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBox(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_105());
            framework.AssertBothValid("testCheckBox", pdfUAConformance);
        }

        private sealed class _Generator_105 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_105() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxWithCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_117());
            framework.AssertBothValid("testCheckBoxWithCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_117 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_117() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
                cb.SetBackgroundColor(ColorConstants.YELLOW);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxChecked(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_133());
            framework.AssertBothValid("testCheckBox", pdfUAConformance);
        }

        private sealed class _Generator_133 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_133() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.SetChecked(true);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxCheckedAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_148());
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription", pdfUAConformance);
        }

        private sealed class _Generator_148 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_148() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetAlternateDescription("Yello");
                cb.SetChecked(true);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxCheckedCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_164());
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_164 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_164() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_183());
            framework.AssertBothValid("testCheckBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_183 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_183() {
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(PdfConformance.PDF_UA_1);
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                return checkBox;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractiveCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_198());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_198 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_198() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractiveCustomAppearanceChecked(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_217());
            framework.AssertBothValid("checkBoxInteractiveCustomAppChecked", pdfUAConformance);
        }

        private sealed class _Generator_217 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_217() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButton(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_237());
            framework.AssertBothValid("testRadioButton", pdfUAConformance);
        }

        private sealed class _Generator_237 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_237() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonChecked(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_249());
            framework.AssertBothValid("testRadioButtonChecked", pdfUAConformance);
        }

        private sealed class _Generator_249 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_249() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_263());
            framework.AssertBothValid("testRadioButtonCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_263 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_263() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                radio.SetBackgroundColor(ColorConstants.GREEN);
                radio.SetSize(20);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearanceChecked(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_279());
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked", pdfUAConformance);
        }

        private sealed class _Generator_279 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_279() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroup(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_296());
            framework.AddSuppliers(new _Generator_302());
            framework.AssertBothValid("testRadioButtonGroup", pdfUAConformance);
        }

        private sealed class _Generator_296 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_296() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_302 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_302() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_315());
            framework.AddSuppliers(new _Generator_325());
            framework.AssertBothValid("testRadioButtonGroupCustom", pdfUAConformance);
        }

        private sealed class _Generator_315 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_315() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_325 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_325() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearanceChecked(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_341());
            framework.AddSuppliers(new _Generator_351());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked", pdfUAConformance);
        }

        private sealed class _Generator_341 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_341() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_351 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_351() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_369());
            framework.AssertBothValid("testRadioButtonInteractive", pdfUAConformance);
        }

        private sealed class _Generator_369 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_369() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCheckedInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_384());
            framework.AssertBothValid("testRadioButtonChecked", pdfUAConformance);
        }

        private sealed class _Generator_384 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_384() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_400());
            framework.AssertBothValid("testRadioButtonCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_400 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_400() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearanceCheckedInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_419());
            framework.AssertBothValid("radioBtnCustomAppCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_419 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_419() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_438());
            framework.AddSuppliers(new _Generator_447());
            framework.AssertBothValid("testRadioButtonGroupInteractive", pdfUAConformance);
        }

        private sealed class _Generator_438 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_438() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_447 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_447() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name2", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello2");
                return r;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_463());
            framework.AddSuppliers(new _Generator_475());
            framework.AssertBothValid("testRadioButtonGroupInteractive", pdfUAConformance);
        }

        private sealed class _Generator_463 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_463() {
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

        private sealed class _Generator_475 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_475() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearanceCheckedInteractive(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_494());
            framework.AddSuppliers(new _Generator_506());
            framework.AssertBothValid("radioBtnCustomAppCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_494 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_494() {
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

        private sealed class _Generator_506 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_506() {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButton(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_526(this));
            framework.AssertBothValid("testButton", pdfUAConformance);
        }

        private sealed class _Generator_526 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_526(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_541(this));
            framework.AssertBothValid("testButtonCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_541 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_541(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonSingleLine(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_558(this));
            framework.AssertBothValid("testButtonSingleLine", pdfUAConformance);
        }

        private sealed class _Generator_558 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_558(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContent(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_573(this));
            framework.AssertBothValid("testButtonSingleLine", pdfUAConformance);
        }

        private sealed class _Generator_573 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_573(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContentIsAlsoForm(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_589());
            framework.AssertBothValid("testButtonCustomContentIsAlsoForm", pdfUAConformance);
        }

        private sealed class _Generator_589 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_589() {
            }

            public IBlockElement Generate() {
                Button b = new Button("name");
                CheckBox cb = new CheckBox("name2");
                cb.SetChecked(true);
                b.Add(cb);
                return b;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_605(this));
            framework.AssertBothValid("testButtonInteractive", pdfUAConformance);
        }

        private sealed class _Generator_605 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_605(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_623(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_623 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_623(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonSingleLineInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_643(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_643 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_643(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContentInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_661(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_661 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_661(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonCustomContentIsAlsoFormInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_680(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_680 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_680(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputField(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_701(this));
            framework.AssertBothValid("testInputField", pdfUAConformance);
        }

        private sealed class _Generator_701 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_701(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                InputField inputField = new InputField("name");
                inputField.SetFont(this._enclosing.GetFont());
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithValue(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_715(this));
            framework.AssertBothValid("testInputFieldWithValue", pdfUAConformance);
        }

        private sealed class _Generator_715 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_715(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_730(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_730 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_730(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndValue(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_746(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
        }

        private sealed class _Generator_746 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_746(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndPlaceHolder(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_763(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
        }

        private sealed class _Generator_763 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_763(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_780(this));
            framework.AssertBothValid("testInputFieldInteractive", pdfUAConformance);
        }

        private sealed class _Generator_780 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_780(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithValueInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_796(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_796 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_796(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_813(this));
            framework.AssertBothValid("inputFieldCustomAppInteractive", pdfUAConformance);
        }

        private sealed class _Generator_813 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_813(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndValueInteractive(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_832(this));
            framework.AssertBothValid("inputFieldCustomAppValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_832 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_832(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldWithCustomAppearanceAndPlaceHolderInteractive(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_852(this));
            framework.AssertBothValid("inpFieldCustomAppPlaceholderInteractive", pdfUAConformance);
        }

        private sealed class _Generator_852 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_852(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextArea(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_871(this));
            framework.AssertBothValid("testTextArea", pdfUAConformance);
        }

        private sealed class _Generator_871 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_871(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                TextArea textArea = new TextArea("name");
                textArea.SetFont(this._enclosing.GetFont());
                return textArea;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithValue(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_885(this));
            framework.AssertBothValid("testTextAreaWithValue", pdfUAConformance);
        }

        private sealed class _Generator_885 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_885(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_900(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_900 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_900(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndValue(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_916(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
        }

        private sealed class _Generator_916 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_916(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndPlaceHolder(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_933(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
        }

        private sealed class _Generator_933 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_933(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_950(this));
            framework.AssertBothValid("testTextAreaInteractive", pdfUAConformance);
        }

        private sealed class _Generator_950 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_950(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithValueInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_966(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_966 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_966(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_983(this));
            framework.AssertBothValid("textAreaWithCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_983 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_983(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndValueInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1002(this));
            framework.AssertBothValid("textAreaCustomAppValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1002 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1002(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaWithCustomAppearanceAndPlaceHolderInteractive(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_1022(this));
            framework.AssertBothValid("textAreaCustomAppPlaceHolderInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1022 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1022(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBox(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1041(this));
            framework.AssertBothValid("testListBox", pdfUAConformance);
        }

        private sealed class _Generator_1041 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1041(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1057(this));
            framework.AssertBothValid("testListBoxCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_1057 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1057(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearanceSelected(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1076(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected", pdfUAConformance);
        }

        private sealed class _Generator_1076 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1076(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1095(this));
            framework.AssertBothValid("testListBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1095 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1095(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1113(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1113 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1113(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxCustomAppearanceSelectedInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1134(this));
            framework.AssertBothValid("listBoxCustomAppSelectedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1134 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1134(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBox(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1155(this));
            framework.AssertBothValid("testComboBox", pdfUAConformance);
        }

        private sealed class _Generator_1155 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1155(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1171(this));
            framework.AssertBothValid("testComboBoxCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_1171 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1171(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearanceSelected(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1190(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected", pdfUAConformance);
        }

        private sealed class _Generator_1190 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1190(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1209(this));
            framework.AssertBothValid("testComboBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1209 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1209(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1227(this));
            framework.AssertBothValid("comboBoxCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1227 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1227(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxCustomAppearanceSelectedInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1248(this));
            framework.AssertBothValid("comboBoxCustomAppInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1248 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1248(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1269(this));
            framework.AssertBothValid("testSignatureAppearance", pdfUAConformance);
        }

        private sealed class _Generator_1269 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1269(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceText(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1284(this));
            framework.AssertBothValid("signatureAppearanceSignedAppearanceText", pdfUAConformance);
        }

        private sealed class _Generator_1284 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1284(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithCustomContent(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1303(this));
            framework.AssertBothValid("signatureAppearanceSignedAppearanceText", pdfUAConformance);
        }

        private sealed class _Generator_1303 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1303(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_1322(this));
            framework.AssertBothValid("signAppSignedAppCustomAppText", pdfUAConformance);
        }

        private sealed class _Generator_1322 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1322(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1343(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1343 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1343(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceTextInteractive(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_1361(this));
            framework.AssertBothValid("signAppSignedTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1361 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1361(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithCustomContentInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1383(this));
            framework.AssertBothValid("signedAppearanceTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1383 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1383(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignedAndCustomAppearanceTextInteractive(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1404(this));
            framework.AssertBothValid("signedCustomAppTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1404 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1404(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveCheckBoxNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1427());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                        .FONT_SHOULD_BE_EMBEDDED, "ZapfDingbats"), pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1427 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1427() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveRadioButtonNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1450());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveRadioButtonNoAltDescr", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveRadioButtonNoAltDescr", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1450 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1450() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveButtonNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1471(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveButtonNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1471 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1471(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveInputFieldNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1494(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveInputFieldNoAltDescr", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveInputFieldNoAltDescr", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1494 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1494(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveTextAreaNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1516(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveTextAreaNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1516 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1516(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveListBoxNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1538(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveListBoxNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1538 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1538(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveComboBoxNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1560(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveComboBoxNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1560 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1560(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveSignatureAppearanceNoAlternativeDescription(PdfUAConformance pdfUAConformance
            ) {
            framework.AddSuppliers(new _Generator_1583(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveSignAppearanceNoAltDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("interactiveSignAppearanceNoAltDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1583 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1583(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1605());
            framework.AssertBothValid("testCheckBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1605 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1605() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxArtifactDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1621());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testCheckBoxArtifactRoleua1", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    //TODO DEVSIX-8974 Tagging formfield as artifact will put the inner content into bad places in tagstructure
                    framework.AssertBothFail("testCheckBoxArtifactRoleua2", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1621 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1621() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1641());
            framework.AddSuppliers(new _Generator_1651());
            framework.AddSuppliers(new _Generator_1661());
            framework.AssertBothValid("testRadioButtonDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1641 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1641() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1651 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1651() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1661 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1661() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1675(this));
            framework.AddSuppliers(new _Generator_1686(this));
            framework.AssertBothValid("testButtonDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1675 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1675(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1686 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1686(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1702(this));
            framework.AddSuppliers(new _Generator_1713(this));
            framework.AddSuppliers(new _Generator_1724(this));
            framework.AssertBothValid("testInputFieldDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1702 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1702(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1713 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1713(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1724 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1724(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextAreaDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1740(this));
            framework.AddSuppliers(new _Generator_1750(this));
            framework.AddSuppliers(new _Generator_1759(this));
            framework.AssertBothValid("testTextAreaDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1740 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1740(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1750 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1750(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1759 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1759(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestListBoxDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1774(this));
            framework.AddSuppliers(new _Generator_1784(this));
            framework.AssertBothValid("testListBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1774 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1774(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1784 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1784(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestComboBoxDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1801(this));
            framework.AddSuppliers(new _Generator_1814(this));
            framework.AssertBothValid("testComboBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1801 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1801(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1814 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1814(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1829(this));
            framework.AddSuppliers(new _Generator_1841(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1829 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1829(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1841 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1841(PdfUAFormFieldsTest _enclosing) {
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextBuilderWithTu", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testTextBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testTextBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testTextBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).CreateComboBox();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testChoiceBuilderWithTu", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testChoiceBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).CreateComboBox();
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("tesChoicetBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("tesChoicetBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreatePushButton();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonBuilderWithTu", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testButtonBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreatePushButton();
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testButtonBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTuNotVisible(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreatePushButton();
                IList<PdfFormAnnotation> annList = field.GetChildFormAnnotations();
                annList[0].SetVisibility(PdfFormAnnotation.HIDDEN);
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonBuilderNoTuNotVisible", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testButtonBuilderNoTuNotVisible", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonBuilderNoTu(PdfUAConformance pdfUAConformance) {
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testRadioButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testRadioButtonBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonBuilderWithTu(PdfUAConformance pdfUAConformance) {
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonBuilderWithTu", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testRadioButtonBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreateSignature();
                field.SetValue("some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testSignatureBuilderWithTu", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testSignatureBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).CreateSignature();
                field.SetValue("some value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testSignatureBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("testSignatureBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldWithAltEntry(PdfUAConformance pdfUAConformance) {
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("FormFieldAltDescription", pdfUAConformance);
            }
            else {
                // TODO DEVSIX-8242 PDF/UA-2 checks
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyVeraPdfFail("FormFieldAltDescription", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldAsStream(PdfUAConformance pdfUAConformance) {
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
                pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(parentDic));
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("FormFieldAsStream", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                        , "StructTreeRoot", "Form");
                    framework.AssertBothFail("FormFieldAsStream", message, pdfUAConformance);
                }
            }
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
