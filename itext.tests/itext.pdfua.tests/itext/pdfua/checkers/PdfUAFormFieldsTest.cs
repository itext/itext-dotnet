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
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
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

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBox(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_119());
            framework.AssertBothValid("testCheckBox", pdfUAConformance);
        }

        private sealed class _Generator_119 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_119() {
            }

            public IBlockElement Generate() {
                return new CheckBox("name");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxWithCustomAppearance(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_132(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxWithCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_132 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_132(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(new PdfConformance(pdfUAConformance));
                cb.SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2));
                cb.SetBackgroundColor(ColorConstants.YELLOW);
                return cb;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxChecked(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_149(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxChecked", pdfUAConformance);
        }

        private sealed class _Generator_149 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_149(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(new PdfConformance(pdfUAConformance));
                cb.SetChecked(true);
                return cb;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxCheckedAlternativeDescription(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_165(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxCheckedAlternativeDescription", pdfUAConformance);
        }

        private sealed class _Generator_165 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_165(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(new PdfConformance(pdfUAConformance));
                cb.GetAccessibilityProperties().SetAlternateDescription("Yello");
                cb.SetChecked(true);
                return cb;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxCheckedCustomAppearance(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_182(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxCheckedCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_182 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_182(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(new PdfConformance(pdfUAConformance));
                cb.SetChecked(true);
                cb.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                cb.SetBackgroundColor(ColorConstants.GREEN);
                cb.SetCheckBoxType(CheckBoxType.STAR);
                cb.SetSize(20);
                return cb;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractive(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_202(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_202 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_202(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(new PdfConformance(pdfUAConformance));
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                return checkBox;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractiveCustomAppearance(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_218(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxInteractiveCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_218 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_218(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(new PdfConformance(pdfUAConformance));
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                checkBox.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                checkBox.SetBackgroundColor(ColorConstants.GREEN);
                checkBox.SetSize(20);
                checkBox.SetCheckBoxType(CheckBoxType.SQUARE);
                return checkBox;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxInteractiveCustomAppearanceChecked(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_238(pdfUAConformance));
            framework.AssertBothValid("checkBoxInteractiveCustomAppChecked", pdfUAConformance);
        }

        private sealed class _Generator_238 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_238(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox checkBox = (CheckBox)new CheckBox("name").SetInteractive(true);
                checkBox.SetPdfConformance(new PdfConformance(pdfUAConformance));
                checkBox.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                checkBox.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                checkBox.SetBackgroundColor(ColorConstants.GREEN);
                checkBox.SetSize(20);
                checkBox.SetChecked(true);
                checkBox.SetCheckBoxType(CheckBoxType.SQUARE);
                return checkBox;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButton(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_259());
            framework.AssertBothValid("testRadioButton", pdfUAConformance);
        }

        private sealed class _Generator_259 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_259() {
            }

            public IBlockElement Generate() {
                return new Radio("name");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonChecked(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_272());
            framework.AssertBothValid("testRadioButtonChecked", pdfUAConformance);
        }

        private sealed class _Generator_272 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_272() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name");
                radio.SetChecked(true);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonCustomAppearance(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_287());
            framework.AssertBothValid("testRadioButtonCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_287 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_287() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_304());
            framework.AssertBothValid("testRadioButtonCustomAppearanceChecked", pdfUAConformance);
        }

        private sealed class _Generator_304 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_304() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_322());
            framework.AddSuppliers(new _Generator_328());
            framework.AssertBothValid("testRadioButtonGroup", pdfUAConformance);
        }

        private sealed class _Generator_322 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_322() {
            }

            public IBlockElement Generate() {
                return new Radio("name", "group");
            }
        }

        private sealed class _Generator_328 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_328() {
            }

            public IBlockElement Generate() {
                return new Radio("name2", "group");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonGroupCustomAppearance(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_342());
            framework.AddSuppliers(new _Generator_352());
            framework.AssertBothValid("testRadioButtonGroupCustom", pdfUAConformance);
        }

        private sealed class _Generator_342 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_342() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_352 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_352() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_369());
            framework.AddSuppliers(new _Generator_379());
            framework.AssertBothValid("testRadioButtonGroupCustomAppearanceChecked", pdfUAConformance);
        }

        private sealed class _Generator_369 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_369() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetSize(20);
                r.SetBorder(new SolidBorder(ColorConstants.CYAN, 2));
                r.SetBackgroundColor(ColorConstants.GREEN);
                return r;
            }
        }

        private sealed class _Generator_379 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_379() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_398());
            framework.AssertBothValid("testRadioButtonInteractive", pdfUAConformance);
        }

        private sealed class _Generator_398 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_398() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_414());
            framework.AssertBothValid("testRadioButtonCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_414 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_414() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_431());
            framework.AssertBothValid("testRadioButtonCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_431 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_431() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_451());
            framework.AssertBothValid("radioBtnCustomAppCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_451 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_451() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_471());
            framework.AddSuppliers(new _Generator_480());
            framework.AssertBothValid("testRadioButtonGroupInteractive", pdfUAConformance);
        }

        private sealed class _Generator_471 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_471() {
            }

            public IBlockElement Generate() {
                Radio r = new Radio("name", "group");
                r.SetInteractive(true);
                r.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return r;
            }
        }

        private sealed class _Generator_480 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_480() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_497());
            framework.AddSuppliers(new _Generator_509());
            framework.AssertBothValid("radioBtnCustomAppInteractive", pdfUAConformance);
        }

        private sealed class _Generator_497 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_497() {
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

        private sealed class _Generator_509 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_509() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_529());
            framework.AddSuppliers(new _Generator_541());
            framework.AssertBothValid("radioBtnCustomAppGrCheckedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_529 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_529() {
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

        private sealed class _Generator_541 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_541() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_562(this));
            framework.AssertBothValid("testButton", pdfUAConformance);
        }

        private sealed class _Generator_562 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_562(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_578(this));
            framework.AssertBothValid("testButtonCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_578 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_578(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_596(this));
            framework.AssertBothValid("testButtonSingleLine", pdfUAConformance);
        }

        private sealed class _Generator_596 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_596(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_612(this));
            framework.AssertBothValid("testButtonCustomContent", pdfUAConformance);
        }

        private sealed class _Generator_612 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_612(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_629());
            framework.AssertBothValid("testButtonCustomContentIsAlsoForm", pdfUAConformance);
        }

        private sealed class _Generator_629 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_629() {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_646(this));
            framework.AssertBothValid("testButtonInteractive", pdfUAConformance);
        }

        private sealed class _Generator_646 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_646(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_665(this));
            framework.AssertBothValid("testButtonCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_665 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_665(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_686(this));
            framework.AssertBothValid("testButtonSingleLineInteractive", pdfUAConformance);
        }

        private sealed class _Generator_686 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_686(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_705(this));
            framework.AssertBothValid("testButtonCustomContentInteractive", pdfUAConformance);
        }

        private sealed class _Generator_705 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_705(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_725(this));
            framework.AssertBothValid("testButtonCustomContentIsAlsoFormInteractive", pdfUAConformance);
        }

        private sealed class _Generator_725 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_725(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_747(this));
            framework.AssertBothValid("testInputField", pdfUAConformance);
        }

        private sealed class _Generator_747 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_747(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_762(this));
            framework.AssertBothValid("testInputFieldWithValue", pdfUAConformance);
        }

        private sealed class _Generator_762 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_762(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_778(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_778 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_778(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_795(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndValue", pdfUAConformance);
        }

        private sealed class _Generator_795 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_795(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_813(this));
            framework.AssertBothValid("testInputFieldWithCustomAppearanceAndPlaceHolder", pdfUAConformance);
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
                inputField.SetPlaceholder(new Paragraph("Placeholder").SetFont(this._enclosing.GetFont()));
                return inputField;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInputFieldInteractive(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_831(this));
            framework.AssertBothValid("testInputFieldInteractive", pdfUAConformance);
        }

        private sealed class _Generator_831 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_831(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_848(this));
            framework.AssertBothValid("testInputFieldWithValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_848 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_848(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_866(this));
            framework.AssertBothValid("inputFieldCustomAppInteractive", pdfUAConformance);
        }

        private sealed class _Generator_866 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_866(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_886(this));
            framework.AssertBothValid("inputFieldCustomAppValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_886 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_886(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_907(this));
            framework.AssertBothValid("inpFieldCustomAppPlaceholderInteractive", pdfUAConformance);
        }

        private sealed class _Generator_907 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_907(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_927(this));
            framework.AssertBothValid("testTextArea", pdfUAConformance);
        }

        private sealed class _Generator_927 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_927(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_942(this));
            framework.AssertBothValid("testTextAreaWithValue", pdfUAConformance);
        }

        private sealed class _Generator_942 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_942(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_958(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_958 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_958(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_975(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndValue", pdfUAConformance);
        }

        private sealed class _Generator_975 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_975(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_993(this));
            framework.AssertBothValid("testTextAreaWithCustomAppearanceAndPlaceHolder", pdfUAConformance);
        }

        private sealed class _Generator_993 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_993(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1011(this));
            framework.AssertBothValid("testTextAreaInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1011 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1011(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1028(this));
            framework.AssertBothValid("testTextAreaWithValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1028 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1028(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1046(this));
            framework.AssertBothValid("textAreaWithCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1046 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1046(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1066(this));
            framework.AssertBothValid("textAreaCustomAppValueInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1066 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1066(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1087(this));
            framework.AssertBothValid("textAreaCustomAppPlaceHolderInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1087 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1087(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1107(this));
            framework.AssertBothValid("testListBox", pdfUAConformance);
        }

        private sealed class _Generator_1107 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1107(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1124(this));
            framework.AssertBothValid("testListBoxCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_1124 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1124(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1144(this));
            framework.AssertBothValid("testListBoxCustomAppearanceSelected", pdfUAConformance);
        }

        private sealed class _Generator_1144 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1144(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1164(this));
            framework.AssertBothValid("testListBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1164 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1164(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1183(this));
            framework.AssertBothValid("testListBoxCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1183 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1183(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1205(this));
            framework.AssertBothValid("listBoxCustomAppSelectedInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1205 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1205(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1227(this));
            framework.AssertBothValid("testComboBox", pdfUAConformance);
        }

        private sealed class _Generator_1227 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1227(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1244(this));
            framework.AssertBothValid("testComboBoxCustomAppearance", pdfUAConformance);
        }

        private sealed class _Generator_1244 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1244(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1264(this));
            framework.AssertBothValid("testComboBoxCustomAppearanceSelected", pdfUAConformance);
        }

        private sealed class _Generator_1264 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1264(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1284(this));
            framework.AssertBothValid("testComboBoxInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1284 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1284(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1303(this));
            framework.AssertBothValid("comboBoxCustomAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1303 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1303(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1325(this));
            framework.AssertBothValid("comboBoxCustomAppInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1325 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1325(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1347(this));
            framework.AssertBothValid("testSignatureAppearance", pdfUAConformance);
        }

        private sealed class _Generator_1347 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1347(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1363(this));
            framework.AssertBothValid("signatureAppearanceSignedAppearanceText", pdfUAConformance);
        }

        private sealed class _Generator_1363 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1363(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1383(this));
            framework.AssertBothValid("signatureAppearanceWithCustomContent", pdfUAConformance);
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
                return appearance;
            }

            private readonly PdfUAFormFieldsTest _enclosing;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureAppearanceWithSignedAppearanceAndCustomAppearanceText(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1403(this));
            framework.AssertBothValid("signAppSignedAppCustomAppText", pdfUAConformance);
        }

        private sealed class _Generator_1403 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1403(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1425(this));
            framework.AssertBothValid("testSignatureAppearanceInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1425 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1425(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1444(this));
            framework.AssertBothValid("signAppSignedTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1444 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1444(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1467(this));
            framework.AssertBothValid("signedAppearanceTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1467 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1467(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1489(this));
            framework.AssertBothValid("signedCustomAppTextInteractive", pdfUAConformance);
        }

        private sealed class _Generator_1489 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1489(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1513());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveCheckBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1513 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1513() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveRadioButtonNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1536());
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveRadioButtonNoAltDescr", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveRadioButtonNoAltDescr", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1536 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1536() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name", "group");
                radio.SetInteractive(true);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestInteractiveButtonNoAlternativeDescription(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1558(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveButtonNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1558 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1558(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1582(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveInputFieldNoAltDescr", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveInputFieldNoAltDescr", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1582 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1582(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1605(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveTextAreaNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1605 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1605(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1628(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveListBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1628 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1628(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1651(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveComboBoxNoAlternativeDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1651 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1651(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1675(this));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("interactiveSignAppearanceNoAltDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("interactiveSignAppearanceNoAltDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_1675 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1675(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1698(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1698 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1698(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(new PdfConformance(pdfUAConformance));
                cb.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                cb.GetAccessibilityProperties().SetAlternateDescription("Hello");
                return cb;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestCheckBoxArtifactRole(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1715(pdfUAConformance));
            framework.AssertBothValid("testCheckBoxArtifactRole", pdfUAConformance);
        }

        private sealed class _Generator_1715 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1715(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetPdfConformance(new PdfConformance(pdfUAConformance));
                cb.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return cb;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonDifferentRole(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1731());
            framework.AddSuppliers(new _Generator_1741());
            framework.AddSuppliers(new _Generator_1751());
            framework.AssertBothValid("testRadioButtonDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1731 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1731() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name1", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio " + "that " + "was " + "not " + "checked"
                    );
                return radio;
            }
        }

        private sealed class _Generator_1741 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1741() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name2", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1751 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1751() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name3", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonArtifactRole(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1766());
            framework.AddSuppliers(new _Generator_1776());
            framework.AddSuppliers(new _Generator_1786());
            framework.AssertBothValid("testRadioButtonArtifactRole", pdfUAConformance);
        }

        private sealed class _Generator_1766 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1766() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name1", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1776 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1776() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name2", "group");
                radio.SetChecked(true);
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                radio.GetAccessibilityProperties().SetAlternateDescription("Radio that was not checked");
                return radio;
            }
        }

        private sealed class _Generator_1786 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1786() {
            }

            public IBlockElement Generate() {
                Radio radio = new Radio("name3", "group");
                radio.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                return radio;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonDifferentRole(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1801(this));
            framework.AddSuppliers(new _Generator_1812(this));
            framework.AssertBothValid("testButtonDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1801 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1801(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1812 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1812(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1829(this));
            framework.AddSuppliers(new _Generator_1840(this));
            framework.AddSuppliers(new _Generator_1851(this));
            framework.AssertBothValid("testInputFieldDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1829 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1829(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1840 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1840(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1851 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1851(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1868(this));
            framework.AddSuppliers(new _Generator_1878(this));
            framework.AddSuppliers(new _Generator_1887(this));
            framework.AssertBothValid("testTextAreaDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1868 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1868(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1878 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1878(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1887 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1887(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1903(this));
            framework.AddSuppliers(new _Generator_1913(this));
            framework.AssertBothValid("testListBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1903 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1903(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1913 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1913(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1931(this));
            framework.AddSuppliers(new _Generator_1944(this));
            framework.AssertBothValid("testComboBoxDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1931 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1931(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1944 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1944(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_1960(this));
            framework.AddSuppliers(new _Generator_1972(this));
            framework.AssertBothValid("testSignatureAppearanceDifferentRole", pdfUAConformance);
        }

        private sealed class _Generator_1960 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1960(PdfUAFormFieldsTest _enclosing) {
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

        private sealed class _Generator_1972 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_1972(PdfUAFormFieldsTest _enclosing) {
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
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testTextBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestTextBuilderNoTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testTextBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderWithTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testChoiceBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestChoiceBuilderNoTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("tesChoicetBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderWithTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testButtonBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testButtonBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestButtonBuilderNoTuNotVisible(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testButtonBuilderNoTuNotVisible", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonBuilderNoTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                    framework.AssertBothFail("testRadioButtonBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestRadioButtonBuilderWithTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                    framework.AssertBothFail("testRadioButtonBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderWithTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testSignatureBuilderWithTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestSignatureBuilderNoTu(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("testSignatureBuilderNoTu", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldWithAltEntry(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("FormFieldAltDescription", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldWithContentsEntry(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "hello").SetWidgetRectangle(new Rectangle(100, 100
                    , 100, 100)).SetFont(GetFont()).CreateText();
                field.SetValue("Some value");
                field.GetFirstFormAnnotation().SetAlternativeDescription("Some alt");
                form.AddField(field);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("formFieldContentsDescription", PdfUAExceptionMessageConstants.MISSING_FORM_FIELD_DESCRIPTION
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("formFieldContentsDescription", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestFormFieldAsStream(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                    pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(parentDic));
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new PdfStructElem(parentDic));
                }
            }
            );
            framework.AssertBothValid("FormFieldAsStream", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SeveralWidgetKidsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("severalWidgetKids", PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_WITHOUT_ROLE_SHALL_CONTAIN_ONE_WIDGET
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("severalWidgetKids", PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_SHALL_CONTAIN_AT_MOST_ONE_WIDGET
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SeveralWidgetKidsWithRoleTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("severalWidgetKidsWithRole", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("severalWidgetKidsWithRole", PdfUAExceptionMessageConstants.FORM_STRUCT_ELEM_SHALL_CONTAIN_AT_MOST_ONE_WIDGET
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetNeitherFormNorArtifactTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("widgetNeitherFormNorArtifact", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                    , pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    // TODO DEVSIX-9580. VeraPDF claims the document to be valid, although it's not.
                    //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
                    framework.AssertOnlyITextFail("widgetNeitherFormNorArtifact", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetNeitherFormNorArtifactInAcroformTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                , pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetIsArtifactInAcroformTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("widgetIsArtifactInAcroform", PdfUAExceptionMessageConstants.WIDGET_SHALL_BE_FORM_OR_ARTIFACT
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("widgetIsArtifactInAcroform", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetLabelNoContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            framework.AssertBothValid("widgetLabelNoContentsTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AdditionalActionAndContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                    pdfDoc.GetStructTreeRoot().AddKid(elem);
                }
                else {
                    ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(elem);
                }
            }
            );
            framework.AssertBothValid("additionalActionAndContents", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AdditionalActionNoContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("additionalActionNoContents", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("additionalActionNoContents", PdfUAExceptionMessageConstants.WIDGET_WITH_AA_SHALL_PROVIDE_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AdditionalActionNoContentsAcroformTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("additionalActionNoContentsAcroform", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("additionalActionNoContentsAcroform", PdfUAExceptionMessageConstants.WIDGET_WITH_AA_SHALL_PROVIDE_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NoContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("noContents", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("noContents", PdfUAExceptionMessageConstants.WIDGET_SHALL_PROVIDE_LABEL_OR_CONTENTS
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVPositiveTest1(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            framework.AssertBothValid("textFieldRVAndVPositiveTest1", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVPositiveTest2(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            framework.AssertBothValid("textFieldRVAndVPositiveTest2", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVPositiveTest3(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            framework.AssertBothValid("textFieldRVAndVPositiveTest3", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVNegativeTest1(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("textFieldRVAndVNegativeTest1", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("textFieldRVAndVNegativeTest1", PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVNegativeTest2(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("textFieldRVAndVNegativeTest2", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("textFieldRVAndVNegativeTest2", PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TextFieldRVAndVNegativeTest3(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("textFieldRVAndVNegativeTest3", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("textFieldRVAndVNegativeTest3", PdfUAExceptionMessageConstants.TEXT_FIELD_V_AND_RV_SHALL_BE_TEXTUALLY_EQUIVALENT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SignatureAppearanceWithImage(PdfUAConformance pdfUAConformance) {
            // TODO DEVSIX-9023 Support "Signature fields" UA-2 rules
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_2889());
            framework.AssertBothValid("signatureAppearanceWithImage", pdfUAConformance);
        }

        private sealed class _Generator_2889 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_2889() {
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                Div div = new Div();
                Image img;
                try {
                    img = new Image(ImageDataFactory.Create(PdfUAFormFieldsTest.DOG));
                }
                catch (UriFormatException e) {
                    throw new Exception(e.Message);
                }
                div.Add(img);
                appearance.SetContent(div);
                appearance.SetInteractive(true);
                appearance.SetAlternativeDescription("Alternative Description");
                return appearance;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SignatureAppearanceWithLineSeparator(PdfUAConformance pdfUAConformance) {
            // TODO DEVSIX-9023 Support "Signature fields" UA-2 rules
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_2915());
            framework.AssertBothValid("signatureAppearanceLineSep", pdfUAConformance);
        }

        private sealed class _Generator_2915 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_2915() {
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                Div div = new Div();
                LineSeparator line = new LineSeparator(new SolidLine(3));
                div.Add(line);
                appearance.SetContent(div);
                appearance.SetInteractive(true);
                appearance.SetAlternativeDescription("Alternative Description");
                return appearance;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SignatureAppearanceBackgroundImage(PdfUAConformance pdfUAConformance) {
            // TODO DEVSIX-9023 Support "Signature fields" UA-2 rules
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_2936(this));
            framework.AssertBothValid("signatureAppearanceBackgroundImage", pdfUAConformance);
        }

        private sealed class _Generator_2936 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_2936(PdfUAFormFieldsTest _enclosing) {
                this._enclosing = _enclosing;
            }

            public IBlockElement Generate() {
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("name");
                try {
                    appearance.SetFont(this._enclosing.GetFont());
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(PdfUAFormFieldsTest.DOG));
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
                    throw new Exception(e.Message);
                }
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
