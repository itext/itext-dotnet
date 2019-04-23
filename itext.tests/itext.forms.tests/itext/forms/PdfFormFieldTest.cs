/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using System.IO;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    public class PdfFormFieldTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FormFieldTest01() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            PdfFormField field = fields.Get("Text1");
            NUnit.Framework.Assert.IsTrue(fields.Count == 6);
            NUnit.Framework.Assert.IsTrue(field.GetFieldName().ToUnicodeString().Equals("Text1"));
            NUnit.Framework.Assert.IsTrue(field.GetValue().ToString().Equals("TestField"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFieldTest02() {
            String filename = destinationFolder + "formFieldTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 490, 150, 22);
            PdfTextFormField field = PdfFormField.CreateText(pdfDoc, rect, "fieldName", "some value");
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFieldTest02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFieldTest03() {
            String filename = destinationFolder + "formFieldTest03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"), new PdfWriter(filename
                ));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetFirstPage();
            Rectangle rect = new Rectangle(210, 490, 150, 22);
            PdfTextFormField field = PdfFormField.CreateText(pdfDoc, rect, "TestField", "some value");
            form.AddField(field, page);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFieldTest03.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFieldTest04() {
            String filename = destinationFolder + "formFieldTest04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"), new PdfWriter(filename
                ));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfPage page = pdfDoc.GetFirstPage();
            Rectangle rect = new Rectangle(210, 490, 150, 22);
            PdfTextFormField field = PdfFormField.CreateText(pdfDoc, rect, "TestField", "some value in courier font", 
                PdfFontFactory.CreateFont(StandardFonts.COURIER), 10);
            form.AddField(field, page);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFieldTest04.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void UnicodeFormFieldTest() {
            String filename = sourceFolder + "unicodeFormFieldFile.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> formFields = form.GetFormFields();
            String fieldName = "\u5E10\u53F71";
            // 帐号1: account number 1
            NUnit.Framework.Assert.AreEqual(fieldName, formFields.Keys.ToArray(new String[1])[0]);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void UnicodeFormFieldTest2() {
            String filename = sourceFolder + "unicodeFormFieldFile.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String fieldName = "\u5E10\u53F71";
            // 帐号1: account number 1
            NUnit.Framework.Assert.IsNotNull(form.GetField(fieldName));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ChoiceFieldTest01() {
            String filename = destinationFolder + "choiceFieldTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 490, 150, 20);
            String[] options = new String[] { "First Item", "Second Item", "Third Item", "Fourth Item" };
            PdfChoiceFormField choice = PdfFormField.CreateComboBox(pdfDoc, rect, "TestField", "First Item", options);
            form.AddField(choice);
            Rectangle rect1 = new Rectangle(210, 250, 150, 90);
            PdfChoiceFormField choice1 = PdfFormField.CreateList(pdfDoc, rect1, "TestField1", "Second Item", options);
            choice1.SetMultiSelect(true);
            form.AddField(choice1);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_choiceFieldTest01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ButtonFieldTest01() {
            String filename = destinationFolder + "buttonFieldTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(36, 700, 20, 20);
            Rectangle rect1 = new Rectangle(36, 680, 20, 20);
            PdfButtonFormField group = PdfFormField.CreateRadioGroup(pdfDoc, "TestGroup", "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect, group, "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect1, group, "2");
            form.AddField(group);
            PdfButtonFormField pushButton = PdfFormField.CreatePushButton(pdfDoc, new Rectangle(36, 650, 40, 20), "push"
                , "Capcha");
            PdfButtonFormField checkBox = PdfFormField.CreateCheckBox(pdfDoc, new Rectangle(36, 560, 20, 20), "TestCheck"
                , "1");
            form.AddField(pushButton);
            form.AddField(checkBox);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_buttonFieldTest01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DefaultRadiobuttonFieldTest() {
            String file = "defaultRadiobuttonFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            PdfButtonFormField group = PdfFormField.CreateRadioGroup(pdfDoc, "TestGroup", "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect1, group, "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect2, group, "2");
            form.AddField(group);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomizedRadiobuttonFieldTest() {
            String file = "customizedRadiobuttonFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            PdfButtonFormField group2 = PdfFormField.CreateRadioGroup(pdfDoc, "TestGroup2", "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect1, group2, "1").SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormField.VISIBLE);
            PdfFormField.CreateRadioButton(pdfDoc, rect2, group2, "2").SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormField.VISIBLE);
            form.AddField(group2);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomizedRadiobuttonWithGroupRegeneratingFieldTest() {
            String file = "customizedRadiobuttonWithGroupRegeneratingFieldTest.pdf";
            String filename = destinationFolder + file;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect1 = new Rectangle(36, 700, 20, 20);
            Rectangle rect2 = new Rectangle(36, 680, 20, 20);
            PdfButtonFormField group2 = PdfFormField.CreateRadioGroup(pdfDoc, "TestGroup2", "1");
            PdfFormField.CreateRadioButton(pdfDoc, rect1, group2, "1").SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormField.VISIBLE);
            PdfFormField.CreateRadioButton(pdfDoc, rect2, group2, "2").SetBorderWidth(2).SetBorderColor(ColorConstants
                .RED).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetVisibility(PdfFormField.VISIBLE);
            group2.RegenerateField();
            form.AddField(group2);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ButtonFieldTest02() {
            String filename = destinationFolder + "buttonFieldTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "buttonFieldTest02_input.pdf"), new PdfWriter
                (filename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            ((PdfButtonFormField)form.GetField("push")).SetImage(sourceFolder + "Desert.jpg");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_buttonFieldTest02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RealFontSizeRegenerateAppearanceTest() {
            String sourceFilename = sourceFolder + "defaultAppearanceRealFontSize.pdf";
            String destFilename = destinationFolder + "realFontSizeRegenerateAppearance.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFilename), new PdfWriter(destFilename));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.GetField("fieldName").RegenerateField();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destFilename, sourceFolder + "cmp_realFontSizeRegenerateAppearance.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldWithKidsTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField root = PdfFormField.CreateEmptyField(pdfDoc);
            root.SetFieldName("root");
            PdfFormField child = PdfFormField.CreateEmptyField(pdfDoc);
            child.SetFieldName("child");
            root.AddKid(child);
            PdfTextFormField text1 = PdfFormField.CreateText(pdfDoc, new Rectangle(100, 700, 200, 20), "text1", "test"
                );
            child.AddKid(text1);
            form.AddField(root);
            NUnit.Framework.Assert.AreEqual(3, form.GetFormFields().Count);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFormWithDefaultResources() {
            String outPdf = destinationFolder + "fillFormWithDefaultResources.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithDefaultResources.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "formWithDefaultResources.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            PdfFormField field = fields.Get("Text1");
            field.SetValue("New value size must be 8");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFormTwiceWithoutResources() {
            String outPdf = destinationFolder + "fillFormWithoutResources.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithoutResources.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "formWithoutResources.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            PdfFormField field = fields.Get("Text1");
            field.SetValue("New value size must be 8").SetFontSize(8);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AutoScaleFontSizeInFormFields() {
            String outPdf = destinationFolder + "autoScaleFontSizeInFormFields.pdf";
            String cmpPdf = sourceFolder + "cmp_autoScaleFontSizeInFormFields.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfFormField field = PdfFormField.CreateText(pdfDoc, new Rectangle(36, 786, 80, 20), "name", "TestValueAndALittleMore"
                );
            form.AddField(field.SetFontSizeAutoScale());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.NO_FIELDS_IN_ACROFORM)]
        public virtual void AcroFieldDictionaryNoFields() {
            String outPdf = destinationFolder + "acroFieldDictionaryNoFields.pdf";
            String cmpPdf = sourceFolder + "cmp_acroFieldDictionaryNoFields.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "acroFieldDictionaryNoFields.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RegenerateAppearance() {
            String input = "regenerateAppearance.pdf";
            String output = "regenerateAppearance.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + input), new PdfWriter(destinationFolder
                 + output), new StampingProperties().UseAppendMode());
            PdfAcroForm acro = PdfAcroForm.GetAcroForm(document, false);
            int i = 1;
            foreach (KeyValuePair<String, PdfFormField> entry in acro.GetFormFields()) {
                if (entry.Key.Contains("field")) {
                    PdfFormField field = entry.Value;
                    field.SetValue("test" + i++, false);
                }
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + output, sourceFolder 
                + "cmp_" + output, destinationFolder, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RegenerateAppearance2() {
            String input = "regenerateAppearance2.pdf";
            String output = "regenerateAppearance2.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + input), new PdfWriter(destinationFolder
                 + output), new StampingProperties().UseAppendMode());
            PdfAcroForm acro = PdfAcroForm.GetAcroForm(document, false);
            acro.SetNeedAppearances(true);
            PdfFormField field = acro.GetField("number");
            field.SetValue("20150044DR");
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + output, sourceFolder 
                + "cmp_" + output, destinationFolder, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultilineTextFieldWithAlignmentTest() {
            String outPdf = destinationFolder + "multilineTextFieldWithAlignment.pdf";
            String cmpPdf = sourceFolder + "cmp_multilineTextFieldWithAlignment.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            Rectangle rect = new Rectangle(210, 600, 150, 100);
            PdfTextFormField field = PdfFormField.CreateMultilineText(pdfDoc, rect, "fieldName", "some value\nsecond line\nthird"
                );
            field.SetJustification(PdfTextFormField.ALIGN_RIGHT);
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FlushedPagesTest() {
            String filename = destinationFolder + "flushedPagesTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.AddNewPage().Flush();
            pdfDoc.AddNewPage().Flush();
            pdfDoc.AddNewPage();
            PdfTextFormField field = PdfFormField.CreateText(pdfDoc, new Rectangle(100, 100, 300, 20), "name", "");
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_flushedPagesTest.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFormWithDefaultResourcesUpdateFont() {
            String outPdf = destinationFolder + "fillFormWithDefaultResourcesUpdateFont.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithDefaultResourcesUpdateFont.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "formWithDefaultResources.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            PdfFormField field = fields.Get("Text1");
            // TODO DEVSIX-2016: the font in /DR of AcroForm dict is not updated, even though /DA field is updated.
            field.SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER));
            field.SetValue("New value size must be 8, but with different font.");
            new Canvas(new PdfCanvas(pdfDoc.GetFirstPage()), pdfDoc, new Rectangle(30, 500, 500, 200)).Add(new Paragraph
                ("The text font after modification it via PDF viewer (e.g. Acrobat) shall be preserved."));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormRegenerateWithInvalidDefaultAppearance01() {
            String testName = "formRegenerateWithInvalidDefaultAppearance01";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + "invalidDA.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            fields.Get("Text1").SetValue("New field value");
            fields.Get("Text2").SetValue("New field value");
            fields.Get("Text3").SetValue("New field value");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase1() {
            //Create a document with formfields and paragraphs in both fonts, and fill them before closing the document
            String testName = "fillFieldWithHebrewCase1";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document document = new Document(pdfDoc);
            PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                , true);
            hebrew.SetSubset(false);
            PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H, true);
            sileot.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String text = "שלום וברכה";
            CreateAcroForm(pdfDoc, form, hebrew, text, 0);
            CreateAcroForm(pdfDoc, form, sileot, text, 3);
            AddParagraph(document, text, hebrew);
            AddParagraph(document, text, sileot);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                 + testName + "_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase2() {
            //Create a document with formfields and paragraphs in both fonts, and fill them after closing and reopening the document
            String testName = "fillFieldWithHebrewCase2";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document document = new Document(pdfDoc);
            PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                , true);
            hebrew.SetSubset(false);
            PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H, true);
            sileot.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            CreateAcroForm(pdfDoc, form, hebrew, null, 0);
            CreateAcroForm(pdfDoc, form, sileot, null, 3);
            String text = "שלום וברכה";
            AddParagraph(document, text, hebrew);
            AddParagraph(document, text, sileot);
            pdfDoc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new PdfWriter(outPdf
                ));
            FillAcroForm(pdfDocument, text);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                 + testName + "_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase3() {
            //Create a document with formfields in both fonts, and fill them before closing the document
            String testName = "fillFieldWithHebrewCase3";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                , true);
            hebrew.SetSubset(false);
            PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H, true);
            sileot.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String text = "שלום וברכה";
            CreateAcroForm(pdfDoc, form, hebrew, text, 0);
            CreateAcroForm(pdfDoc, form, sileot, text, 3);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                 + testName + "_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFieldWithHebrewCase4() {
            //Create a document with formfields in both fonts, and fill them after closing and reopening the document
            String testName = "fillFieldWithHebrewCase4";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont hebrew = PdfFontFactory.CreateFont(sourceFolder + "OpenSansHebrew-Regular.ttf", PdfEncodings.IDENTITY_H
                , true);
            hebrew.SetSubset(false);
            PdfFont sileot = PdfFontFactory.CreateFont(sourceFolder + "SILEOT.ttf", PdfEncodings.IDENTITY_H, true);
            sileot.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            CreateAcroForm(pdfDoc, form, hebrew, null, 0);
            CreateAcroForm(pdfDoc, form, sileot, null, 3);
            pdfDoc.Close();
            String text = "שלום וברכה";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())), new PdfWriter(outPdf
                ));
            FillAcroForm(pdfDocument, text);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                 + testName + "_"));
        }

        private void CreateAcroForm(PdfDocument pdfDoc, PdfAcroForm form, PdfFont font, String text, int offSet) {
            for (int x = offSet; x < (offSet + 3); x++) {
                Rectangle rect = new Rectangle(100 + (30 * x), 100 + (100 * x), 55, 30);
                PdfFormField field = PdfFormField.CreateText(pdfDoc, rect, "f-" + x, "", font, 12.0f);
                field.SetJustification(PdfFormField.ALIGN_RIGHT);
                if (text != null) {
                    field.SetValue(text);
                }
                form.AddField(field);
            }
        }

        private void AddParagraph(Document document, String text, PdfFont font) {
            document.Add(new Paragraph("Hello world ").Add(text).SetFont(font));
        }

        private void FillAcroForm(PdfDocument pdfDocument, String text) {
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            foreach (PdfFormField field in acroForm.GetFormFields().Values) {
                field.SetValue(text);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldNewLineTest() {
            String testName = "multilineFormFieldNewLineTest";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            fields.Get("BEMERKUNGEN").SetValue("First line\n\n\nFourth line");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultilineFormFieldNewLineFontType3Test() {
            String testName = "multilineFormFieldNewLineFontType3Test";
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            String srcPdf = sourceFolder + testName + ".pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(srcPdf);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField info = (PdfTextFormField)form.GetField("info");
            info.SetValue("A\n\nE");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FillFormWithSameEmptyObjsForAppearance() {
            String outPdf = destinationFolder + "fillFormWithSameEmptyObjsForAppearance.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithSameEmptyObjsForAppearance.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfReader reader = new PdfReader(sourceFolder + "fillFormWithSameEmptyObjsForAppearance.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, false);
            acroForm.GetField("text_1").SetValue("Text 1!");
            acroForm.GetField("text_2").SetValue("Text 2!");
            acroForm.GetField("text.3").SetValue("Text 3!");
            acroForm.GetField("text.4").SetValue("Text 4!");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DashedBorderApearanceTest() {
            String outPdf = destinationFolder + "dashedBorderApearanceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_dashedBorderApearanceTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField[] fields = new PdfTextFormField[3];
            String[] names = new String[] { "fieldNoPattern", "fieldEmptyPattern", "fieldSingleEntryPattern" };
            float y = 830;
            PdfDictionary borderDict = new PdfDictionary();
            borderDict.Put(PdfName.S, PdfName.D);
            PdfArray patternArray = new PdfArray();
            for (int i = 0; i < 3; i++) {
                if (i == 2) {
                    patternArray.Add(new PdfNumber(10));
                }
                if (i > 0) {
                    borderDict.Put(PdfName.D, patternArray);
                }
                fields[i] = PdfTextFormField.CreateText(pdfDoc, new Rectangle(10, y -= 70, 200, 50), names[i], names[i]);
                acroForm.AddField(fields[i]);
                fields[i].SetBorderStyle(borderDict);
                fields[i].SetBorderWidth(3);
                fields[i].SetBorderColor(ColorConstants.CYAN);
                fields[i].SetBackgroundColor(ColorConstants.MAGENTA);
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT, Count = 2)]
        public virtual void NoMaxLenWithSetCombFlagTest() {
            String outPdf = destinationFolder + "noMaxLenWithSetCombFlagTest.pdf";
            String cmpPdf = sourceFolder + "cmp_noMaxLenWithSetCombFlagTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField textField = PdfFormField.CreateText(pdfDoc, new Rectangle(100, 500, 200, 200), "text");
            textField.SetComb(true);
            // The line below should throw an exception, because the Comb flag may be set only if the MaxLen entry is present in the text field dictionary
            textField.SetValue("12345678");
            textField.SetMaxLen(1);
            form.AddField(textField);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MaxLenWithSetCombFlagAppearanceTest() {
            String srcPdf = sourceFolder + "maxLenFields.pdf";
            String outPdf = destinationFolder + "maxLenWithSetCombFlagAppearanceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_maxLenWithSetCombFlagAppearanceTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            form.GetField("text1").SetValue("123");
            form.GetField("text2").SetJustification(1).SetValue("123");
            form.GetField("text3").SetJustification(2).SetValue("123");
            form.GetField("text4").SetValue("12345678");
            form.GetField("text5").SetValue("123456789101112131415161718");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PreserveFontPropsTest() {
            String srcPdf = sourceFolder + "preserveFontPropsTest.pdf";
            String outPdf = destinationFolder + "preserveFontPropsTest.pdf";
            String cmpPdf = sourceFolder + "cmp_preserveFontPropsTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            PdfFormField field1 = form.GetField("emptyField");
            field1.SetValue("Do fields on the left look the same?", field1.GetFont() != null ? field1.GetFont() : PdfFontFactory
                .CreateFont(), field1.GetFontSize());
            PdfFormField field2 = form.GetField("emptyField2");
            field2.SetValue("Do fields on the right look the same?", field2.GetFont() != null ? field2.GetFont() : PdfFontFactory
                .CreateFont(), field2.GetFontSize());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FontAutoSizeButtonFieldTest() {
            String outPdf = destinationFolder + "fontAutoSizeButtonFieldTest.pdf";
            String cmpPdf = sourceFolder + "cmp_fontAutoSizeButtonFieldTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            String itext = "itextpdf";
            PdfButtonFormField button = PdfFormField.CreatePushButton(pdfDoc, new Rectangle(36, 500, 200, 200), itext, 
                itext);
            button.SetFontSize(0);
            button.SetBackgroundColor(ColorConstants.GRAY);
            button.SetValue(itext);
            button.SetVisibility(PdfFormField.VISIBLE_BUT_DOES_NOT_PRINT);
            form.AddField(button);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MaxLenInheritanceTest() {
            String srcPdf = sourceFolder + "maxLenInheritanceTest.pdf";
            String outPdf = destinationFolder + "maxLenInheritanceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_maxLenInheritanceTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            form.GetField("text").SetValue("iText!");
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MaxLenDeepInheritanceTest() {
            String srcFilename = sourceFolder + "maxLenDeepInheritanceTest.pdf";
            String destFilename = destinationFolder + "maxLenDeepInheritanceTest.pdf";
            String cmpFilename = sourceFolder + "cmp_maxLenDeepInheritanceTest.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(destFilename));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(destDoc, false);
            acroForm.GetField("text.1").SetValue("WoOooOw");
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFilename, destinationFolder
                , "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MaxLenColoredTest() {
            String srcPdf = sourceFolder + "maxLenColoredTest.pdf";
            String outPdf = destinationFolder + "maxLenColoredTest.pdf";
            String cmpPdf = sourceFolder + "cmp_maxLenColoredTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            form.GetField("magenta").SetColor(ColorConstants.MAGENTA);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.COMB_FLAG_MAY_BE_SET_ONLY_IF_MAXLEN_IS_PRESENT, Count = 2)]
        public virtual void RegenerateMaxLenCombTest() {
            String srcPdf = sourceFolder + "regenerateMaxLenCombTest.pdf";
            String outPdf = destinationFolder + "regenerateMaxLenCombTest.pdf";
            String cmpPdf = sourceFolder + "cmp_regenerateMaxLenCombTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            for (int i = 0; i < 12; i++) {
                PdfTextFormField field = (PdfTextFormField)form.GetField("field " + i);
                if (i < 8) {
                    field.SetMaxLen(i < 4 ? 7 : 0);
                }
                if (i % 6 > 1) {
                    field.SetFieldFlag(PdfTextFormField.FF_COMB, i % 2 == 0 ? true : false);
                }
            }
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void WrapPrecedingContentOnFlattenTest() {
            String filename = destinationFolder + "wrapPrecedingContentOnFlattenTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetFillColor(ColorConstants.MAGENTA);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            PdfTextFormField[] fields = new PdfTextFormField[4];
            for (int i = 0; i < 4; i++) {
                fields[i] = PdfFormField.CreateText(pdfDoc, new Rectangle(90, 700 - i * 100, 150, 22), "black" + i, "black"
                    );
            }
            form.AddField(fields[0]);
            form.AddField(fields[1]);
            Document doc = new Document(pdfDoc);
            doc.Add(new AreaBreak());
            canvas = new PdfCanvas(pdfDoc.GetPage(2));
            canvas.SetFillColor(ColorConstants.CYAN);
            form.AddField(fields[2]);
            form.AddField(fields[3], pdfDoc.GetFirstPage());
            form.FlattenFields();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_wrapPrecedingContentOnFlattenTest.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
