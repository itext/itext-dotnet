using System;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// This test checks correct handling of pdf documents with (slightly) corrupt XREF table
    /// xref
    /// 0 30
    /// 0000000000 65535 f
    /// 0000000000 65535 f
    /// </summary>
    public class CorruptXRefTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/CorruptXRefTest/";

        public static readonly String outputFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/kernel/pdf/CorruptXRefTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(outputFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ReadPdfWithCorruptXRef() {
            String cmpFile = sourceFolder + "cmp_docOut.pdf";
            String outputFile = outputFolder + "docOut.pdf";
            String inputFile = sourceFolder + "docIn.pdf";
            PdfWriter writer = new PdfWriter(outputFile);
            PdfReader reader = new PdfReader(inputFile);
            PdfDocument inputPdfDocument = new PdfDocument(reader);
            PdfDocument outputPdfDocument = new PdfDocument(writer);
            int lastPage = inputPdfDocument.GetNumberOfPages();
            inputPdfDocument.CopyPagesTo(lastPage, lastPage, outputPdfDocument);
            inputPdfDocument.Close();
            outputPdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFolder + "docOut.pdf", cmpFile, outputFolder
                , "diff_"));
        }
    }
}
