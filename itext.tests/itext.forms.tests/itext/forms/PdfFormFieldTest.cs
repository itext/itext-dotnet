/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Forms.Fields;
using iText.IO.Font.Constants;
using iText.IO.Source;
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
    }
}
