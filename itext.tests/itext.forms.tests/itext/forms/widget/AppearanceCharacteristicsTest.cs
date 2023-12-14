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
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Widget {
    public class AppearanceCharacteristicsTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/widget/AppearanceCharacteristicsTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/widget/AppearanceCharacteristicsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldBordersTest() {
            String outPdf = destinationFolder + "formFieldBorders.pdf";
            String cmpPdf = sourceFolder + "cmp_formFieldBorders.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField simpleField = PdfTextFormField.CreateText(doc, new Rectangle(300, 300, 200, 100), "simpleField"
                    );
                simpleField.RegenerateField();
                PdfFormField insetField = PdfTextFormField.CreateText(doc, new Rectangle(50, 600, 200, 100), "insetField");
                insetField.GetWidgets()[0].SetBorderStyle(PdfName.I);
                insetField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField underlineField = PdfTextFormField.CreateText(doc, new Rectangle(300, 600, 200, 100), "underlineField"
                    );
                underlineField.GetWidgets()[0].SetBorderStyle(PdfName.U);
                underlineField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField solidField = PdfTextFormField.CreateText(doc, new Rectangle(50, 450, 200, 100), "solidField");
                solidField.GetWidgets()[0].SetBorderStyle(PdfName.S);
                solidField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField dashField = PdfTextFormField.CreateText(doc, new Rectangle(300, 450, 200, 100), "dashField");
                dashField.GetWidgets()[0].SetBorderStyle(PdfName.D);
                dashField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField beveledField = PdfTextFormField.CreateText(doc, new Rectangle(50, 300, 200, 100), "beveledField"
                    );
                beveledField.GetWidgets()[0].SetBorderStyle(PdfName.B);
                beveledField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                form.AddField(simpleField);
                form.AddField(insetField);
                form.AddField(underlineField);
                form.AddField(solidField);
                form.AddField(dashField);
                form.AddField(beveledField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void BeveledBorderWithBackgroundTest() {
            String outPdf = destinationFolder + "beveledBorderWithBackground.pdf";
            String cmpPdf = sourceFolder + "cmp_beveledBorderWithBackground.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField formField = PdfTextFormField.CreateText(doc, new Rectangle(100, 600, 200, 100), "formField");
                formField.GetWidgets()[0].SetBorderStyle(PdfName.B);
                formField.SetBorderWidth(3f).SetBackgroundColor(DeviceRgb.GREEN).SetBorderColor(DeviceRgb.RED);
                formField.RegenerateField();
                form.AddField(formField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void DashedBorderWithBackgroundTest() {
            String outPdf = destinationFolder + "dashedBorderWithBackground.pdf";
            String cmpPdf = sourceFolder + "cmp_dashedBorderWithBackground.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField formField = PdfTextFormField.CreateText(doc, new Rectangle(100, 600, 200, 100), "formField");
                formField.GetWidgets()[0].SetBorderStyle(PdfName.D);
                formField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).SetBackgroundColor(DeviceRgb.GREEN);
                formField.RegenerateField();
                form.AddField(formField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void TextStartsAfterFieldBorderTest() {
            // TODO DEVSIX-4809 text in form filed with borders must start after border
            String outPdf = destinationFolder + "textStartsAfterFieldBorderTest.pdf";
            String cmpPdf = sourceFolder + "cmp_textStartsAfterFieldBorderTest.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField insetFormField = PdfTextFormField.CreateText(doc, new Rectangle(90, 600, 200, 100), "insetFormField"
                    );
                insetFormField.GetWidgets()[0].SetBorderStyle(PdfName.I);
                insetFormField.SetBorderWidth(15f).SetBorderColor(DeviceRgb.RED).SetValue("Text after border").RegenerateField
                    ();
                PdfFormField solidFormField = PdfTextFormField.CreateText(doc, new Rectangle(300, 600, 200, 100), "solidFormField"
                    );
                solidFormField.GetWidgets()[0].SetBorderStyle(PdfName.S);
                solidFormField.SetBorderWidth(15f).SetBorderColor(DeviceRgb.RED).SetValue("Text after border").RegenerateField
                    ();
                PdfFormField underlineFormField = PdfTextFormField.CreateText(doc, new Rectangle(90, 450, 200, 100), "underlineFormField"
                    );
                underlineFormField.GetWidgets()[0].SetBorderStyle(PdfName.U);
                underlineFormField.SetBorderWidth(15f).SetBorderColor(DeviceRgb.RED).SetValue("Text after border").RegenerateField
                    ();
                PdfFormField simpleFormField = PdfTextFormField.CreateText(doc, new Rectangle(300, 450, 200, 100), "formField1"
                    );
                simpleFormField.SetBorderWidth(15f);
                simpleFormField.SetValue("Text after border").RegenerateField();
                form.AddField(insetFormField);
                form.AddField(solidFormField);
                form.AddField(underlineFormField);
                form.AddField(simpleFormField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithRotatedFieldAndPageTest() {
            String outPdf = destinationFolder + "fillFormWithRotatedFieldAndPageTest.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithRotatedFieldAndPageTest.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "pdfWithRotatedField.pdf"), new PdfWriter
                (outPdf))) {
                PdfAcroForm form1 = PdfAcroForm.GetAcroForm(doc, false);
                form1.GetField("First field").SetValue("We filled this field").SetBorderColor(ColorConstants.BLACK);
            }
            String errorMessage = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BorderStyleInCreatedFormFieldsTest() {
            String outPdf = destinationFolder + "borderStyleInCreatedFormFields.pdf";
            String cmpPdf = sourceFolder + "cmp_borderStyleInCreatedFormFields.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
                PdfFormField formField1 = PdfTextFormField.CreateText(doc, new Rectangle(100, 600, 100, 50), "firstField", 
                    "Hello, iText!");
                formField1.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_BEVELED);
                formField1.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
                PdfFormField formField2 = PdfTextFormField.CreateText(doc, new Rectangle(100, 500, 100, 50), "secondField"
                    , "Hello, iText!");
                formField2.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_UNDERLINE);
                formField2.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
                PdfFormField formField3 = PdfTextFormField.CreateText(doc, new Rectangle(100, 400, 100, 50), "thirdField", 
                    "Hello, iText!");
                formField3.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_INSET);
                formField3.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
                form.AddField(formField1);
                form.AddField(formField2);
                form.AddField(formField3);
                form.FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void UpdatingBorderStyleInFormFieldsTest() {
            String inputPdf = sourceFolder + "borderStyleInCreatedFormFields.pdf";
            String outPdf = destinationFolder + "updatingBorderStyleInFormFields.pdf";
            String cmpPdf = sourceFolder + "cmp_updatingBorderStyleInFormFields.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(inputPdf), new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, false);
                IDictionary<String, PdfFormField> fields = form.GetFormFields();
                fields.Get("firstField").SetValue("New Value 1");
                fields.Get("secondField").SetValue("New Value 2");
                fields.Get("thirdField").SetValue("New Value 3");
                form.FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }
    }
}
