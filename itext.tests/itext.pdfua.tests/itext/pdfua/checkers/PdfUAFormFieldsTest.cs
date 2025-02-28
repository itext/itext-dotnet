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

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBox(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_104());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testCheckBox", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testCheckBox", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_104 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_104() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxWithCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_121());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testCheckBoxWithCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testCheckBoxWithCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_121 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_121() {
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
            framework.AddSuppliers(new _Generator_142());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testCheckBox", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testCheckBox", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_142 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_142() {
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
            framework.AddSuppliers(new _Generator_162());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testCheckBoxCheckedAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_162 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_162() {
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
            framework.AddSuppliers(new _Generator_183());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testCheckBoxCheckedCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testCheckBoxCheckedCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_183 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_183() {
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
            framework.AddSuppliers(new _Generator_207());
            framework.AssertBothValid("testCheckBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_207 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_207() {
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
            framework.AddSuppliers(new _Generator_222());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_222 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_222() {
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
            framework.AddSuppliers(new _Generator_241());
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearanceChecked", pdfUAConformance);
        }

        private sealed class _Generator_241 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_241() {
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
            framework.AddSuppliers(new _Generator_261());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButton", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButton", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_261 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_261() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonChecked(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_278());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonChecked", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonChecked", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_278 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_278() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_297());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_297 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_297() {
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
            framework.AddSuppliers(new _Generator_318());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonCustomAppearanceChecked", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonCustomAppearanceChecked", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_318 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_318() {
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
            framework.AddSuppliers(new _Generator_340());
            framework.AddSuppliers(new _Generator_346());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonGroup", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonGroup", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_340 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_340() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_346 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_346() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearance(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_364());
            framework.AddSuppliers(new _Generator_374());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonGroupCustom", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonGroupCustom", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_364 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_364() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_374 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_374() {
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
            framework.AddSuppliers(new _Generator_395());
            framework.AddSuppliers(new _Generator_405());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonGroupCustomAppearanceChecked", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_395 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_395() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_405 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_405() {
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
            framework.AddSuppliers(new _Generator_428());
            framework.AssertBothValid("testRadioButtonInteractive", pdfUAConformance);
        }

        private sealed class _Generator_428 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_428() {
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
            framework.AddSuppliers(new _Generator_443());
            framework.AssertBothValid("testRadioButtonChecked", pdfUAConformance);
        }

        private sealed class _Generator_443 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_443() {
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
            framework.AddSuppliers(new _Generator_459());
            framework.AssertBothValid("testRadioButtonCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_459 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_459() {
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
            framework.AddSuppliers(new _Generator_477());
            framework.AssertBothValid("testRadioButtonCustomAppearanceCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_477 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_477() {
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
            framework.AddSuppliers(new _Generator_496());
            framework.AddSuppliers(new _Generator_505());
            framework.AssertBothValid("testRadioButtonGroupInteractive", pdfUAConformance);
        }

        private sealed class _Generator_496 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_496() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_505 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_505() {
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
            framework.AddSuppliers(new _Generator_521());
            framework.AddSuppliers(new _Generator_533());
            framework.AssertBothValid("testRadioButtonGroupInteractive", pdfUAConformance);
        }

        private sealed class _Generator_521 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_521() {
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

        private sealed class _Generator_533 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_533() {
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
            framework.AddSuppliers(new _Generator_551());
            framework.AddSuppliers(new _Generator_563());
            framework.AssertBothValid("radioBtnGroupCustomAppCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_551 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_551() {
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

        private sealed class _Generator_563 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_563() {
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
            framework.AddSuppliers(new _Generator_583(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButton", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButton", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_583 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_583(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_603(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_603 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_603(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_625(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonSingleLine", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonSingleLine", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_625 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_625(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_645(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonSingleLine", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonSingleLine", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_645 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_645(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_666());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonSingleLine", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonSingleLine", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_666 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_666() {
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
            framework.AddSuppliers(new _Generator_687(this));
            framework.AssertBothValid("testButtonInteractive", pdfUAConformance);
        }

        private sealed class _Generator_687 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_687(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_705(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_705 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_705(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_725(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_725 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_725(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_743(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_743 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_743(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_762(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_762 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_762(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_783(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testInputField", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testInputField", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_783 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_783(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_802(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testInputFieldWithValue", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testInputFieldWithValue", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_802 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_802(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_822(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testInputFieldWithCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testInputFieldWithCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_822 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_822(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_843(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_843 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_843(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_865(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_865 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_865(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_887(this));
            framework.AssertBothValid("testInputFieldInteractive", pdfUAConformance);
        }

        private sealed class _Generator_887 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_887(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_903(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_903 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_903(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_920(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_920 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_920(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_938(this));
            framework.AssertBothValid("inputFieldCustomAppearanceValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_938 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_938(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_957(this));
            framework.AssertBothValid("inputFieldCustomAppearancePlaceHolderInteractive", pdfUAConformance);
        }

        private sealed class _Generator_957 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_957(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_976(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextArea", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextArea", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_976 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_976(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_995(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextAreaWithValue", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextAreaWithValue", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_995 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_995(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1015(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextAreaWithCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextAreaWithCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1015 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1015(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1036(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1036 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1036(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1058(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1058 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1058(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1080(this));
            framework.AssertBothValid("testTextAreaInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1080 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1080(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1096(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1096 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1096(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1113(this));
            framework.AssertBothValid("textAreaWithCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1113 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1113(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1131(this));
            framework.AssertBothValid("textAreaCustomAppValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1131 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1131(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1150(this));
            framework.AssertBothValid("textAreaCustomAppearancePlaceHolderInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1150 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1150(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1169(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testListBox", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testListBox", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1169 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1169(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1190(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testListBoxCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testListBoxCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1190 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1190(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1214(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testListBoxCustomAppearanceSelected", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testListBoxCustomAppearanceSelected", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1214 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1214(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1238(this));
            framework.AssertBothValid("testListBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1238 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1238(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1256(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1256 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1256(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1277(this));
            framework.AssertBothValid("listBoxCustomAppearanceSelectedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1277 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1277(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1298(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testComboBox", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testComboBox", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1298 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1298(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1319(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testComboBoxCustomAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testComboBoxCustomAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1319 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1319(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1343(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testListBoxCustomAppearanceSelected", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testListBoxCustomAppearanceSelected", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1343 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1343(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1367(this));
            framework.AssertBothValid("testComboBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1367 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1367(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1385(this));
            framework.AssertBothValid("comboBoxCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1385 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1385(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1406(this));
            framework.AssertBothValid("comboBoxCustomAppearanceSelectedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1406 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1406(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1427(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testSignatureAppearance", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testSignatureAppearance", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1427 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1427(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1447(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("signatureAppearanceSignedAppearanceText", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("signatureAppearanceSignedAppearanceText", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1447 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1447(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1471(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("signatureAppearanceSignedAppearanceText", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("signatureAppearanceSignedAppearanceText", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1471 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1471(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1494(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("signAppSignedAppCustomAppText", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("signAppSignedAppCustomAppText", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1494 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1494(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1520(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1520 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1520(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1537(this));
            framework.AssertBothValid("signAppSignedTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1537 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1537(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1558(this));
            framework.AssertBothValid("signAppearanceSignedAppearanceTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1558 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1558(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1579(this));
            framework.AssertBothValid("signedAndCustomAppearanceTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1579 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1579(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1602());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveCheckBoxNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1602 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1602() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveRadioButtonNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1623());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveRadioButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.
                    MISSING_FORM_FIELD_DESCRIPTION, pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveRadioButtonNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1623 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1623() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveButtonNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1643(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveButtonNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1643 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1643(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1664(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveInputFieldNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveInputFieldNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1664 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1664(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1685(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveTextAreaNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1685 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1685(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1706(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveListBoxNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1706 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1706(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1727(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveComboBoxNoAlternativeDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1727 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1727(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1748(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveSignAppearanceNoAltDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("interactiveSignAppearanceNoAltDescription", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1748 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1748(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1769());
            framework.AddSuppliers(new _Generator_1779());
            framework.AssertBothValid("testCheckBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1769 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1769() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(PdfConformance.PDF_UA_1);
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }
        }

        private sealed class _Generator_1779 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1779() {
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
            framework.AddSuppliers(new _Generator_1794());
            framework.AddSuppliers(new _Generator_1804());
            framework.AddSuppliers(new _Generator_1814());
            framework.AssertBothValid("testRadioButtonDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1794 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1794() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1804 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1804() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1814 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1814() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonDifferentRole(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_1828(this));
            framework.AddSuppliers(new _Generator_1839(this));
            framework.AssertBothValid("testButtonDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1828 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1828(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1839 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1839(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1855(this));
            framework.AddSuppliers(new _Generator_1866(this));
            framework.AddSuppliers(new _Generator_1877(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testInputFieldDifferentRole", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testInputFieldDifferentRole", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1855 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1855(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1866 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1866(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1877 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1877(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1898(this));
            framework.AddSuppliers(new _Generator_1908(this));
            framework.AddSuppliers(new _Generator_1917(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextAreaDifferentRole", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextAreaDifferentRole", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1898 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1898(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1908 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1908(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1917 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1917(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1938(this));
            framework.AddSuppliers(new _Generator_1948(this));
            framework.AssertBothValid("testListBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1938 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1938(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1948 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1948(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1965(this));
            framework.AddSuppliers(new _Generator_1978(this));
            framework.AssertBothValid("testComboBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1965 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1965(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1978 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1978(PdfUAFormFieldsTest _enclosing) {
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
            framework.AddSuppliers(new _Generator_1993(this));
            framework.AddSuppliers(new _Generator_2005(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1993 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1993(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_2005 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_2005(PdfUAFormFieldsTest _enclosing) {
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
                    , 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateText();
                field.SetValue("Some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testTextBuilderWithTu", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateText();
                field.SetValue("Some value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testTextBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testTextBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateComboBox();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testChoiceBuilderWithTu", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testChoiceBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100
                    , 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateComboBox();
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("tesChoicetBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("tesChoicetBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreatePushButton();
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonBuilderWithTu", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreatePushButton();
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testButtonBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTuNotVisible(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfButtonFormField field = new PushButtonFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreatePushButton();
                IList<PdfFormAnnotation> annList = field.GetChildFormAnnotations();
                annList[0].SetVisibility(PdfFormAnnotation.HIDDEN);
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testButtonBuilderNoTuNotVisible", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testButtonBuilderNoTuNotVisible", pdfUAConformance);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonBuilderNoTu", pdfUAConformance);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testRadioButtonBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderWithTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateSignature();
                field.SetValue("some value");
                field.SetAlternativeName("Some tu entry value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("testSignatureBuilderWithTu", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testSignatureBuilderWithTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderNoTu(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle
                    (100, 100, 100, 100)).SetFont(GetFont()).SetConformance(PdfConformance.PDF_UA_1).CreateSignature();
                field.SetValue("some value");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("testSignatureBuilderNoTu", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("testSignatureBuilderNoTu", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldWithAltEntry(PdfUAConformance pdfUAConformance) {
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("FormFieldAltDescription", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("FormFieldAltDescription", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldAsStream(PdfUAConformance pdfUAConformance) {
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("FormFieldAsStream", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertVeraPdfFail("FormFieldAsStream", pdfUAConformance);
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
