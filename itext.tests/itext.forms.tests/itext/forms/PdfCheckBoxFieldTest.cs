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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCheckBoxFieldTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfCheckBoxFieldTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfCheckBoxFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest01() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest01.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 6, 750, 7, 7);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest02() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest02.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 0, 730, 7, 7);
            // fallback to default fontsize — 12 is expected.
            AddCheckBox(pdfDoc, -1, 710, 7, 7);
            AddCheckBox(pdfDoc, 0, 640, 20, 20);
            AddCheckBox(pdfDoc, 0, 600, 40, 20);
            AddCheckBox(pdfDoc, 0, 550, 20, 40);
            AddCheckBox(pdfDoc, 0, 520, 5, 5);
            AddCheckBox(pdfDoc, 0, 510, 5, 3);
            AddCheckBox(pdfDoc, 0, 500, 3, 5);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest03() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest03.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 2, 730, 7, 7);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest04() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest04.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 0, 730, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_1").SetWidgetRectangle(new Rectangle
                (50, 730, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.CIRCLE).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 700, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_2").SetWidgetRectangle(new Rectangle
                (50, 700, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.CROSS).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 670, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_3").SetWidgetRectangle(new Rectangle
                (50, 670, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.DIAMOND).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 640, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_4").SetWidgetRectangle(new Rectangle
                (50, 640, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.SQUARE).SetValue("YES"));
            AddCheckBox(pdfDoc, 0, 610, 10, new CheckBoxFormFieldBuilder(pdfDoc, "cb_5").SetWidgetRectangle(new Rectangle
                (50, 610, 10, 10)).CreateCheckBox().SetCheckType(CheckBoxType.STAR).SetValue("YES"));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxFontSizeTest05() {
            String outPdf = destinationFolder + "checkBoxFontSizeTest05.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxFontSizeTest05.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            pdfDoc.AddNewPage();
            AddCheckBox(pdfDoc, 0, 730, 40, 40);
            AddCheckBox(pdfDoc, 0, 600, 100, 100);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxToggleTest01() {
            String srcPdf = sourceFolder + "checkBoxToggledOn.pdf";
            String outPdf = destinationFolder + "checkBoxToggleTest01.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxToggleTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField checkBox = form.GetField("cb_fs_6_7_7");
            checkBox.SetValue("Off");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxToggleTest02() {
            String srcPdf = sourceFolder + "checkBoxToggledOn.pdf";
            String outPdf = destinationFolder + "checkBoxToggleTest02.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxToggleTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField checkBox = form.GetField("cb_fs_6_7_7");
            checkBox.SetValue("Off", false);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void KeepCheckTypeTest() {
            String srcPdf = destinationFolder + "keepCheckTypeTestInput.pdf";
            String outPdf = destinationFolder + "keepCheckTypeTest.pdf";
            String cmpPdf = sourceFolder + "cmp_keepCheckTypeTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(srcPdf))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
                PdfButtonFormField checkField = new CheckBoxFormFieldBuilder(pdfDoc, "checkField").SetWidgetRectangle(new 
                    Rectangle(100, 600, 100, 100)).SetCheckType(CheckBoxType.CHECK).CreateCheckBox();
                checkField.SetValue("Off");
                checkField.SetFontSizeAutoScale();
                form.AddField(checkField);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc_1, true);
                form.GetField("checkField").SetValue("Yes");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AppearanceRegenerationTest() {
            String outPdf = destinationFolder + "appearanceRegenerationTest.pdf";
            String cmpPdf = sourceFolder + "cmp_appearanceRegenerationTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
                PdfButtonFormField checkBox1 = new CheckBoxFormFieldBuilder(pdfDoc, "checkbox1").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                checkBox1.SetValue("My_Value");
                String offStream = "1 0 0 1 0.86 0.5 cm 0 0 m\n" + "0 0.204 -0.166 0.371 -0.371 0.371 c\n" + "-0.575 0.371 -0.741 0.204 -0.741 0 c\n"
                     + "-0.741 -0.204 -0.575 -0.371 -0.371 -0.371 c\n" + "-0.166 -0.371 0 -0.204 0 0 c\n" + "f\n";
                checkBox1.GetFirstFormAnnotation().SetAppearance(PdfName.N, "Off", new PdfStream(offStream.GetBytes()));
                String onStream = "1 0 0 1 0.835 0.835 cm 0 0 -0.669 -0.67 re\n" + "f\n";
                checkBox1.GetFirstFormAnnotation().SetAppearance(PdfName.N, "My_Value", new PdfStream(onStream.GetBytes())
                    );
                checkBox1.RegenerateField();
                form.AddField(checkBox1);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetValueForMutuallyExclusiveCheckBoxTest() {
            String outPdf = destinationFolder + "setValueForMutuallyExclusiveCheckBox.pdf";
            String cmpPdf = sourceFolder + "cmp_setValueForMutuallyExclusiveCheckBox.pdf";
            String srcPdf = sourceFolder + "mutuallyExclusiveCheckBox.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfFormField radioGroupField = acroForm.GetField("group");
                radioGroupField.SetValue("1");
                radioGroupField.SetValue("2");
                radioGroupField.RegenerateField();
                PdfFormField checkBoxField = acroForm.GetField("check");
                checkBoxField.SetValue("1");
                checkBoxField.SetValue("2");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ChangeOnStateAppearanceNameForCheckBoxWidgetTest() {
            String outPdf = destinationFolder + "changeOnStateAppearanceNameForCheckBoxWidget.pdf";
            String cmpPdf = sourceFolder + "cmp_changeOnStateAppearanceNameForCheckBoxWidget.pdf";
            String srcPdf = sourceFolder + "mutuallyExclusiveCheckBox.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfFormField checkBoxField = acroForm.GetField("check");
                checkBoxField.SetValue("3");
                checkBoxField.GetFirstFormAnnotation().SetCheckBoxAppearanceOnStateName("3");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ChangeOnStateAppearanceNameSeveralTimesTest() {
            String outPdf = destinationFolder + "changeOnStateAppearanceNameSeveralTimes.pdf";
            String cmpPdf = sourceFolder + "cmp_changeOnStateAppearanceNameSeveralTimes.pdf";
            String srcPdf = sourceFolder + "mutuallyExclusiveCheckBox.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfFormField checkBoxField = acroForm.GetField("check");
                checkBoxField.SetValue("3");
                checkBoxField.GetFirstFormAnnotation().SetCheckBoxAppearanceOnStateName("3");
                checkBoxField.GetFirstFormAnnotation().SetCheckBoxAppearanceOnStateName("1");
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxWidgetAppearanceTest() {
            String outPdf = destinationFolder + "checkBoxWidgetAppearance.pdf";
            String cmpPdf = sourceFolder + "cmp_checkBoxWidgetAppearance.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(doc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                PdfFormAnnotation widget = checkBox.GetFirstFormAnnotation();
                // Default case
                widget.SetCheckBoxAppearanceOnStateName("initial");
                NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysAsList(widget.GetAppearanceStates()).Contains("initial"));
                NUnit.Framework.Assert.AreEqual("Off", widget.GetPdfObject().GetAsName(PdfName.AS).GetValue());
                // Setting value changes on state name and appearance state for widget
                checkBox.SetValue("value");
                NUnit.Framework.Assert.IsTrue(JavaUtil.ArraysAsList(widget.GetAppearanceStates()).Contains("value"));
                NUnit.Framework.Assert.AreEqual("value", widget.GetPdfObject().GetAsName(PdfName.AS).GetValue());
                // Setting value generates normal appearance and changes appearance state for widget
                widget.GetWidget().SetNormalAppearance(new PdfDictionary());
                checkBox.SetValue("new_value");
                IList<String> appearanceStates = JavaUtil.ArraysAsList(widget.GetAppearanceStates());
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("new_value"));
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("Off"));
                NUnit.Framework.Assert.AreEqual("new_value", widget.GetPdfObject().GetAsName(PdfName.AS).GetValue());
                acroForm.AddField(checkBox);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetInvalidCheckBoxOnAppearanceTest() {
            String outPdf = destinationFolder + "setInvalidCheckBoxOnAppearance.pdf";
            String cmpPdf = sourceFolder + "cmp_setInvalidCheckBoxOnAppearance.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(doc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                PdfFormAnnotation widget = checkBox.GetFirstFormAnnotation();
                checkBox.SetValue("value");
                IList<String> appearanceStates = JavaUtil.ArraysAsList(widget.GetAppearanceStates());
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("value"));
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("Off"));
                NUnit.Framework.Assert.AreEqual("value", widget.GetPdfObject().GetAsName(PdfName.AS).GetValue());
                // Setting invalid appearance name for on state does nothing
                widget.SetCheckBoxAppearanceOnStateName("Off");
                appearanceStates = JavaUtil.ArraysAsList(widget.GetAppearanceStates());
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("value"));
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("Off"));
                NUnit.Framework.Assert.AreEqual("value", widget.GetPdfObject().GetAsName(PdfName.AS).GetValue());
                widget.SetCheckBoxAppearanceOnStateName("");
                appearanceStates = JavaUtil.ArraysAsList(widget.GetAppearanceStates());
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("value"));
                NUnit.Framework.Assert.IsTrue(appearanceStates.Contains("Off"));
                NUnit.Framework.Assert.AreEqual("value", widget.GetPdfObject().GetAsName(PdfName.AS).GetValue());
                acroForm.AddField(checkBox);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CreateMutuallyExclusiveCheckBoxesTest() {
            String outPdf = destinationFolder + "createMutuallyExclusiveCheckBoxes.pdf";
            String cmpPdf = sourceFolder + "cmp_createMutuallyExclusiveCheckBoxes.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(doc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                checkBox.AddKid(new PdfWidgetAnnotation(new Rectangle(60, 650, 40, 20)));
                checkBox.AddKid(new PdfWidgetAnnotation(new Rectangle(110, 650, 40, 20)));
                checkBox.SetValue("3");
                checkBox.GetFirstFormAnnotation().SetCheckBoxAppearanceOnStateName("1");
                checkBox.GetChildFormAnnotations()[1].SetCheckBoxAppearanceOnStateName("2");
                acroForm.AddField(checkBox);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CreateNotMutuallyExclusiveCheckBoxTest() {
            String outPdf = destinationFolder + "createNotMutuallyExclusiveCheckBox.pdf";
            String cmpPdf = sourceFolder + "cmp_createNotMutuallyExclusiveCheckBox.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(doc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(doc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                checkBox.SetValue("1");
                checkBox.AddKid(new PdfWidgetAnnotation(new Rectangle(60, 650, 40, 20)));
                NUnit.Framework.Assert.IsNull(checkBox.GetWidgets()[1].GetNormalAppearanceObject());
                checkBox.SetValue("2");
                acroForm.AddField(checkBox);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private void AddCheckBox(PdfDocument pdfDoc, float fontSize, float yPos, float checkBoxW, float checkBoxH) {
            Rectangle rect = new Rectangle(50, yPos, checkBoxW, checkBoxH);
            AddCheckBox(pdfDoc, fontSize, yPos, checkBoxW, new CheckBoxFormFieldBuilder(pdfDoc, MessageFormatUtil.Format
                ("cb_fs_{0}_{1}_{2}", fontSize, checkBoxW, checkBoxH)).SetWidgetRectangle(rect).CreateCheckBox().SetCheckType
                (CheckBoxType.CHECK).SetValue("YES"));
        }

        private void AddCheckBox(PdfDocument pdfDoc, float fontSize, float yPos, float checkBoxW, PdfFormField checkBox
            ) {
            PdfPage page = pdfDoc.GetFirstPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            if (fontSize >= 0) {
                checkBox.SetFontSize(fontSize);
            }
            checkBox.GetFirstFormAnnotation().SetBorderWidth(1);
            checkBox.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            form.AddField(checkBox, page);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(50 + checkBoxW + 10, yPos).SetFontAndSize(PdfFontFactory.CreateFont
                (), 12).ShowText("okay?").EndText().RestoreState();
        }
    }
}
