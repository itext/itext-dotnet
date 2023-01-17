/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
    [NUnit.Framework.Category("IntegrationTest")]
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
                PdfFormField simpleField = new TextFormFieldBuilder(doc, "simpleField").SetWidgetRectangle(new Rectangle(300
                    , 300, 200, 100)).CreateText();
                simpleField.RegenerateField();
                PdfFormField insetField = new TextFormFieldBuilder(doc, "insetField").SetWidgetRectangle(new Rectangle(50, 
                    600, 200, 100)).CreateText();
                insetField.GetWidgets()[0].SetBorderStyle(PdfName.I);
                insetField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField underlineField = new TextFormFieldBuilder(doc, "underlineField").SetWidgetRectangle(new Rectangle
                    (300, 600, 200, 100)).CreateText();
                underlineField.GetWidgets()[0].SetBorderStyle(PdfName.U);
                underlineField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField solidField = new TextFormFieldBuilder(doc, "solidField").SetWidgetRectangle(new Rectangle(50, 
                    450, 200, 100)).CreateText();
                solidField.GetWidgets()[0].SetBorderStyle(PdfName.S);
                solidField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField dashField = new TextFormFieldBuilder(doc, "dashField").SetWidgetRectangle(new Rectangle(300, 
                    450, 200, 100)).CreateText();
                dashField.GetWidgets()[0].SetBorderStyle(PdfName.D);
                dashField.SetBorderWidth(3f).SetBorderColor(DeviceRgb.RED).RegenerateField();
                PdfFormField beveledField = new TextFormFieldBuilder(doc, "beveledField").SetWidgetRectangle(new Rectangle
                    (50, 300, 200, 100)).CreateText();
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
                PdfFormField formField = new TextFormFieldBuilder(doc, "formField").SetWidgetRectangle(new Rectangle(100, 
                    600, 200, 100)).CreateText();
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
                PdfFormField formField = new TextFormFieldBuilder(doc, "formField").SetWidgetRectangle(new Rectangle(100, 
                    600, 200, 100)).CreateText();
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
                PdfFormField insetFormField = new TextFormFieldBuilder(doc, "insetFormField").SetWidgetRectangle(new Rectangle
                    (90, 600, 200, 100)).CreateText();
                insetFormField.GetWidgets()[0].SetBorderStyle(PdfName.I);
                insetFormField.SetBorderWidth(15f).SetBorderColor(DeviceRgb.RED).SetValue("Text after border").RegenerateField
                    ();
                PdfFormField solidFormField = new TextFormFieldBuilder(doc, "solidFormField").SetWidgetRectangle(new Rectangle
                    (300, 600, 200, 100)).CreateText();
                solidFormField.GetWidgets()[0].SetBorderStyle(PdfName.S);
                solidFormField.SetBorderWidth(15f).SetBorderColor(DeviceRgb.RED).SetValue("Text after border").RegenerateField
                    ();
                PdfFormField underlineFormField = new TextFormFieldBuilder(doc, "underlineFormField").SetWidgetRectangle(new 
                    Rectangle(90, 450, 200, 100)).CreateText();
                underlineFormField.GetWidgets()[0].SetBorderStyle(PdfName.U);
                underlineFormField.SetBorderWidth(15f).SetBorderColor(DeviceRgb.RED).SetValue("Text after border").RegenerateField
                    ();
                PdfFormField simpleFormField = new TextFormFieldBuilder(doc, "formField1").SetWidgetRectangle(new Rectangle
                    (300, 450, 200, 100)).CreateText();
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
                PdfFormField formField1 = new TextFormFieldBuilder(doc, "firstField").SetWidgetRectangle(new Rectangle(100
                    , 600, 100, 50)).CreateText().SetValue("Hello, iText!");
                formField1.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_BEVELED);
                formField1.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
                PdfFormField formField2 = new TextFormFieldBuilder(doc, "secondField").SetWidgetRectangle(new Rectangle(100
                    , 500, 100, 50)).CreateText().SetValue("Hello, iText!");
                formField2.GetWidgets()[0].SetBorderStyle(PdfAnnotation.STYLE_UNDERLINE);
                formField2.SetBorderWidth(2).SetBorderColor(ColorConstants.BLUE);
                PdfFormField formField3 = new TextFormFieldBuilder(doc, "thirdField").SetWidgetRectangle(new Rectangle(100
                    , 400, 100, 50)).CreateText().SetValue("Hello, iText!");
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
                IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
                fields.Get("firstField").SetValue("New Value 1");
                fields.Get("secondField").SetValue("New Value 2");
                fields.Get("thirdField").SetValue("New Value 3");
                form.FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }
    }
}
