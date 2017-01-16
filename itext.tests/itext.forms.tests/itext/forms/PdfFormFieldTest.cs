using System;
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

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
    }
}
