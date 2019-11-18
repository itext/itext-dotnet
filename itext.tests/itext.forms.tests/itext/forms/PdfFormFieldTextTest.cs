using System;
using System.Collections.Generic;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfFormFieldTextTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormFieldTextTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormFieldTextTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void FillFormWithAutosizeTest() {
            String outPdf = destinationFolder + "fillFormWithAutosizeTest.pdf";
            String inPdf = sourceFolder + "fillFormWithAutosizeSource.pdf";
            String cmpPdf = sourceFolder + "cmp_fillFormWithAutosizeTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
            IDictionary<String, PdfFormField> fields = form.GetFormFields();
            fields.Get("First field").SetValue("name name name ");
            fields.Get("Second field").SetValue("surname surname surname surname surname surname");
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAppearanceExtractionForNotMergedFieldsTest() {
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "sourceDAExtractionTest.pdf"), new PdfWriter
                (destinationFolder + "defaultAppearanceExtractionTest.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, false);
            form.GetField("First field").SetValue("Your name");
            form.GetField("Text1").SetValue("Your surname");
            doc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(destinationFolder + "defaultAppearanceExtractionTest.pdf"
                , sourceFolder + "cmp_defaultAppearanceExtractionTest.pdf", destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
