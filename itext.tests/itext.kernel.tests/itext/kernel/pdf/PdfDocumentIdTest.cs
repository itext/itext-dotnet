using System;
using System.IO;

namespace iText.Kernel.Pdf {
    /// <author>Michael Demey</author>
    public class PdfDocumentIdTest {
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ChangeIdTest() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument pdfDocument = new PdfDocument(writer);
            String value = "Modified ID 1234";
            pdfDocument.SetModifiedDocumentId(new PdfString(value));
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            byte[] documentBytes = baos.ToArray();
            baos.Dispose();
            PdfReader reader = new PdfReader(new MemoryStream(documentBytes));
            pdfDocument = new PdfDocument(reader);
            PdfArray idArray = pdfDocument.GetTrailer().GetAsArray(PdfName.ID);
            NUnit.Framework.Assert.IsNotNull(idArray);
            String extractedValue = idArray.GetAsString(1).GetValue();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(value, extractedValue);
        }
    }
}
