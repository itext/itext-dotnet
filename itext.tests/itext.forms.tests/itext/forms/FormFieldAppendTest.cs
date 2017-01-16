using System;
using System.IO;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class FormFieldAppendTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FormFieldAppendTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FormFieldAppendTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFillingAppend_form_empty_Test() {
            String srcFilename = sourceFolder + "Form_Empty.pdf";
            String temp = destinationFolder + "temp_empty.pdf";
            String filename = destinationFolder + "formFillingAppend_form_empty.pdf";
            StampingProperties props = new StampingProperties();
            props.UseAppendMode();
            PdfDocument doc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(temp), props);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetFormFields().Values) {
                field.SetValue("Test");
            }
            doc.Close();
            Flatten(temp, filename);
            FileInfo toDelete = new FileInfo(temp);
            toDelete.Delete();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFillingAppend_form_empty.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFillingAppend_form_filled_Test() {
            String srcFilename = sourceFolder + "Form_Empty.pdf";
            String temp = destinationFolder + "temp_filled.pdf";
            String filename = destinationFolder + "formFillingAppend_form_filled.pdf";
            StampingProperties props = new StampingProperties();
            props.UseAppendMode();
            PdfDocument doc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(temp), props);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            foreach (PdfFormField field in form.GetFormFields().Values) {
                field.SetValue("Different");
            }
            doc.Close();
            Flatten(temp, filename);
            new FileInfo(temp).Delete();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_formFillingAppend_form_filled.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private void Flatten(String src, String dest) {
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(doc, true);
            form.FlattenFields();
            doc.Close();
        }
    }
}
