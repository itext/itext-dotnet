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
using System.IO;
using iText.Forms.Fields;
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
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
                    int? justification = field.GetJustification();
                    if (null == justification || 0 == (int)justification) {
                        // reddish
                        foreach (PdfFormAnnotation annot in field.GetChildFormAnnotations()) {
                            annot.SetBackgroundColor(new DeviceRgb(255, 200, 200));
                        }
                    }
                    else {
                        if (1 == (int)justification) {
                            // greenish
                            foreach (PdfFormAnnotation annot in field.GetChildFormAnnotations()) {
                                annot.SetBackgroundColor(new DeviceRgb(200, 255, 200));
                            }
                        }
                        else {
                            if (2 == (int)justification) {
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
