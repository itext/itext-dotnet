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
using System.IO;
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FormFieldFlatteningTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FormFieldFlatteningTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FormFieldFlatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void GetFieldsForFlatteningTest() {
            String outPdfName = destinationFolder + "flattenedFormField.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "formFieldFile.pdf"), new PdfWriter(outPdfName
                ));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            NUnit.Framework.Assert.AreEqual(0, form.GetFieldsForFlattening().Count);
            form.PartialFormFlattening("radioName");
            form.PartialFormFlattening("Text1");
            PdfFormField radioNameField = form.GetField("radioName");
            PdfFormField text1Field = form.GetField("Text1");
            NUnit.Framework.Assert.AreEqual(2, form.GetFieldsForFlattening().Count);
            NUnit.Framework.Assert.IsTrue(form.GetFieldsForFlattening().Contains(radioNameField));
            NUnit.Framework.Assert.IsTrue(form.GetFieldsForFlattening().Contains(text1Field));
            form.FlattenFields();
            pdfDoc.Close();
            PdfDocument outPdfDoc = new PdfDocument(new PdfReader(outPdfName));
            PdfAcroForm outPdfForm = PdfAcroForm.GetAcroForm(outPdfDoc, false);
            NUnit.Framework.Assert.AreEqual(2, outPdfForm.GetAllFormFields().Count);
            outPdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FormFlatteningTest01() {
            String srcFilename = "formFlatteningSource.pdf";
            String filename = "formFlatteningTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        [NUnit.Framework.Test]
        public virtual void FormFlatteningChoiceFieldTest01() {
            String srcFilename = "formFlatteningSourceChoiceField.pdf";
            String filename = "formFlatteningChoiceFieldTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        [NUnit.Framework.Test]
        public virtual void MultiLineFormFieldClippingTest() {
            String src = sourceFolder + "multiLineFormFieldClippingTest.pdf";
            String dest = destinationFolder + "multiLineFormFieldClippingTest_flattened.pdf";
            String cmp = sourceFolder + "cmp_multiLineFormFieldClippingTest_flattened.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            form.GetField("Text1").SetValue("Tall letters: T I J L R E F");
            form.FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedFieldAppearanceTest01() {
            String srcFilename = "src_rotatedFieldAppearanceTest01.pdf";
            String filename = "rotatedFieldAppearanceTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedFieldAppearanceTest02() {
            String srcFilename = "src_rotatedFieldAppearanceTest02.pdf";
            String filename = "rotatedFieldAppearanceTest02.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        [NUnit.Framework.Test]
        public virtual void DegeneratedRectTest01() {
            String srcFilename = "src_degeneratedRectTest01.pdf";
            String filename = "degeneratedRectTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        [NUnit.Framework.Test]
        public virtual void DegeneratedRectTest02() {
            String srcFilename = "src_degeneratedRectTest02.pdf";
            String filename = "degeneratedRectTest02.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        [NUnit.Framework.Test]
        public virtual void ScaledRectTest01() {
            String srcFilename = "src_scaledRectTest01.pdf";
            String filename = "scaledRectTest01.pdf";
            FlattenFieldsAndCompare(srcFilename, filename);
        }

        private static void FlattenFieldsAndCompare(String srcFile, String outFile) {
            PdfReader reader = new PdfReader(sourceFolder + srcFile);
            PdfWriter writer = new PdfWriter(destinationFolder + outFile);
            PdfDocument document = new PdfDocument(reader, writer);
            PdfAcroForm.GetAcroForm(document, false).FlattenFields();
            document.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + outFile, sourceFolder + "cmp_" + outFile
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FieldsJustificationTest01() {
            FillTextFieldsThenFlattenThenCompare("fieldsJustificationTest01");
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.ANNOTATION_IN_ACROFORM_DICTIONARY, Count = 2)]
        public virtual void FieldsJustificationTest02() {
            FillTextFieldsThenFlattenThenCompare("fieldsJustificationTest02");
        }

        private static void FillTextFieldsThenFlattenThenCompare(String testName) {
            String src = sourceFolder + "src_" + testName + ".pdf";
            String dest = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                if (field is PdfTextFormField) {
                    String newValue;
                    if (field.IsMultiline()) {
                        newValue = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                             + "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
                        field.SetFontSize(0);
                    }
                    else {
                        newValue = "HELLO!";
                    }
                    HorizontalAlignment? justification = field.GetJustification();
                    if (null == justification || justification == HorizontalAlignment.LEFT) {
                        // reddish
                        foreach (PdfFormAnnotation annot in field.GetChildFormAnnotations()) {
                            annot.SetBackgroundColor(new DeviceRgb(255, 200, 200));
                        }
                    }
                    else {
                        if (justification == HorizontalAlignment.CENTER) {
                            // greenish
                            foreach (PdfFormAnnotation annot in field.GetChildFormAnnotations()) {
                                annot.SetBackgroundColor(new DeviceRgb(200, 255, 200));
                            }
                        }
                        else {
                            if (justification == HorizontalAlignment.RIGHT) {
                                // blueish
                                foreach (PdfFormAnnotation annot in field.GetChildFormAnnotations()) {
                                    annot.SetBackgroundColor(new DeviceRgb(200, 200, 255));
                                }
                            }
                        }
                    }
                    field.SetValue(newValue);
                }
            }
            form.FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 5)]
        public virtual void FlattenReadOnly() {
            //Logging is expected since there are duplicate field names
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfReader reader = new PdfReader(sourceFolder + "readOnlyForm.pdf");
            PdfDocument pdfInnerDoc = new PdfDocument(reader);
            pdfInnerDoc.CopyPagesTo(1, pdfInnerDoc.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfInnerDoc.Close();
            reader = new PdfReader(sourceFolder + "readOnlyForm.pdf");
            pdfInnerDoc = new PdfDocument(reader);
            pdfInnerDoc.CopyPagesTo(1, pdfInnerDoc.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfInnerDoc.Close();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            bool isReadOnly = true;
            foreach (PdfFormField field in form.GetAllFormFields().Values) {
                isReadOnly = (isReadOnly && field.IsReadOnly());
            }
            pdfDoc.Close();
            NUnit.Framework.Assert.IsTrue(isReadOnly);
        }

        [NUnit.Framework.Test]
        public virtual void FieldsRegeneratePushButtonWithoutCaption() {
            FillTextFieldsThenFlattenThenCompare("pushbutton_without_caption");
        }
    }
}
