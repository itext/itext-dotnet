using System;
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfFormFieldsHierarchyTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldsHierarchyTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldsHierarchyTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FillingFormWithKidsTest() {
            String srcPdf = sourceFolder + "formWithKids.pdf";
            String cmpPdf = sourceFolder + "cmp_fillingFormWithKidsTest.pdf";
            String outPdf = destinationFolder + "fillingFormWithKidsTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            IDictionary<String, PdfFormField> formFields = acroForm.GetFormFields();
            foreach (String key in formFields.Keys) {
                PdfFormField field = acroForm.GetField(key);
                field.SetValue(key);
            }
            pdfDocument.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
