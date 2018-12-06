using System;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class FlatteningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/FlatteningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/FlatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FormFlatteningTestWithAPWithoutSubtype() {
            String filename = "job_application_filled";
            String src = sourceFolder + filename + ".pdf";
            String dest = destinationFolder + filename + "_flattened.pdf";
            String cmp = sourceFolder + "cmp_" + filename + "_flattened.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfAcroForm.GetAcroForm(doc, false).FlattenFields();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }
    }
}
