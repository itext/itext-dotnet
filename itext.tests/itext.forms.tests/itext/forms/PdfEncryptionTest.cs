using System;
using iText.Kernel.Pdf;

namespace iText.Forms {
    public class PdfEncryptionTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfEncryptionTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptedDocumentWithFormFields() {
            PdfReader reader = new PdfReader(sourceFolder + "encryptedDocumentWithFormFields.pdf", new ReaderProperties
                ().SetPassword("12345".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1)));
            PdfDocument pdfDocument = new PdfDocument(reader);
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            acroForm.GetField("personal.name").GetPdfObject();
            pdfDocument.Close();
        }
    }
}
