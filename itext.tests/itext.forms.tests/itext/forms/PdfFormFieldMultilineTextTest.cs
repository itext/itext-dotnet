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
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFormFieldMultilineTextTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldMultilineTextTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldMultilineTextTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
        }

        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldTest() {
            String filename = destinationFolder + "multilineFormFieldTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfTextFormField name = new TextFormFieldBuilder(pdfDoc, "fieldName").SetWidgetRectangle(new Rectangle(150
                , 600, 277, 44)).CreateMultilineText();
            name.SetValue("").SetFont(null).SetFontSize(0);
            name.SetScroll(false);
            name.GetFirstFormAnnotation().SetBorderColor(ColorConstants.GRAY);
            String itextLicence = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                 + "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            name.SetValue(itextLicence);
            form.AddField(name);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_multilineFormFieldTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineTextFieldWithAlignmentTest() {
            String outPdf = destinationFolder + "multilineTextFieldWithAlignment.pdf";
            String cmpPdf = sourceFolder + "cmp_multilineTextFieldWithAlignment.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 600, 150, 100);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "fieldName").SetWidgetRectangle(rect).CreateMultilineText
                ();
            field.SetValue("some value\nsecond line\nthird");
            field.SetJustification(TextAlignment.RIGHT);
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldNewLineTest() {
            String testName = "multilineFormFieldNewLineTest";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetAllFormFields();
            fields.Get("BEMERKUNGEN").SetValue("First line\n\n\nFourth line");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldNewLineFontType3Test() {
            String testName = "multilineFormFieldNewLineFontType3Test";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfTextFormField info = (PdfTextFormField)form.GetField("info");
            info.SetValue("A\n\nE");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotFittingByHeightTest() {
            String filename = "notFittingByHeightTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            for (int i = 15; i <= 50; i += 15) {
                PdfFormField[] fields = new PdfFormField[] { new TextFormFieldBuilder(pdfDoc, "multi " + i).SetWidgetRectangle
                    (new Rectangle(100, 800 - i * 4, 150, i)).CreateMultilineText().SetValue("MULTI"), new TextFormFieldBuilder
                    (pdfDoc, "single " + i).SetWidgetRectangle(new Rectangle(300, 800 - i * 4, 150, i)).CreateText().SetValue
                    ("SINGLE") };
                foreach (PdfFormField field in fields) {
                    field.SetFontSize(40);
                    field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
                    form.AddField(field);
                }
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BorderWidthIndentMultilineTest() {
            String filename = destinationFolder + "borderWidthIndentMultilineTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "multi").SetWidgetRectangle(new Rectangle(100, 500
                , 400, 300)).CreateMultilineText();
            field.SetValue("Does this text overlap the border? Well it shouldn't!");
            field.SetFontSize(30);
            field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.RED);
            field.GetFirstFormAnnotation().SetBorderWidth(50);
            form.AddField(field);
            PdfTextFormField field2 = new TextFormFieldBuilder(pdfDoc, "multiAuto").SetWidgetRectangle(new Rectangle(100
                , 400, 400, 50)).CreateMultilineText();
            field2.SetValue("Does this autosize text overlap the border? Well it shouldn't! Does it fit accurately though?"
                );
            field2.SetFontSize(0);
            field2.GetFirstFormAnnotation().SetBorderColor(ColorConstants.RED);
            field2.GetFirstFormAnnotation().SetBorderWidth(20);
            form.AddField(field2);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_borderWidthIndentMultilineTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FormFieldFilledWithStringTest() {
            String value = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "formFieldWithStringTest.pdf"));
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKtc-Light.otf", PdfEncodings.IDENTITY_H
                );
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField form = new TextFormFieldBuilder(pdfDoc, "field").SetWidgetRectangle(new Rectangle(59, 715, 127
                , 69)).CreateMultilineText().SetValue("");
            form.SetFont(font).SetFontSize(10f);
            form.GetFirstFormAnnotation().SetBorderWidth(2).SetBorderColor(ColorConstants.BLACK);
            form.SetValue(value);
            acroForm.AddField(form);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "formFieldWithStringTest.pdf"
                , sourceFolder + "cmp_formFieldWithStringTest.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MultilineTextFieldLeadingSpacesAreNotTrimmedTest() {
            String filename = destinationFolder + "multilineTextFieldLeadingSpacesAreNotTrimmed.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetFirstPage();
            Rectangle rect = new Rectangle(210, 490, 300, 200);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(rect).CreateMultilineText
                ();
            field.SetValue("        value\n      with\n    leading\n    space");
            form.AddField(field, page);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_multilineTextFieldLeadingSpacesAreNotTrimmed.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultilineTextFieldRedundantSpacesAreTrimmedTest() {
            String filename = destinationFolder + "multilineTextFieldRedundantSpacesAreTrimmedTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetFirstPage();
            Rectangle rect = new Rectangle(210, 490, 90, 200);
            PdfTextFormField field = new TextFormFieldBuilder(pdfDoc, "TestField").SetWidgetRectangle(rect).CreateMultilineText
                ();
            field.SetValue("before spaces           after spaces");
            form.AddField(field, page);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_multilineTextFieldRedundantSpacesAreTrimmedTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
