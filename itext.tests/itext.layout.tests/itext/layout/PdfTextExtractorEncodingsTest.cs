using System;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class PdfTextExtractorEncodingsTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfTextExtractorEncodingsTest/";

        /// <summary>Basic Latin characters, with Unicode values less than 128</summary>
        private const String TEXT1 = "AZaz09*!";

        /// <summary>Latin-1 characters</summary>
        private const String TEXT2 = "\u0027\u0060\u00a4\u00a6";

        /// <summary>Test parsing a document which uses a standard non-embedded font.</summary>
        /// <exception cref="System.Exception">any exception will cause the test to fail</exception>
        [NUnit.Framework.Test]
        public virtual void TestStandardFont() {
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.TIMES_ROMAN);
            byte[] pdfBytes = CreatePdf(font);
            CheckPdf(pdfBytes);
        }

        /// <summary>
        /// Test parsing a document which uses a font encoding which creates a /Differences
        /// PdfArray in the PDF.
        /// </summary>
        /// <exception cref="System.Exception">any exception will cause the test to fail</exception>
        [NUnit.Framework.Test]
        public virtual void TestEncodedFont() {
            PdfFont font = GetTTFont("ISO-8859-1", true);
            byte[] pdfBytes = CreatePdf(font);
            CheckPdf(pdfBytes);
        }

        /// <summary>
        /// Test parsing a document which uses a Unicode font encoding which creates a /ToUnicode
        /// PdfArray.
        /// </summary>
        /// <exception cref="System.Exception">any exception will cause the test to fail</exception>
        [NUnit.Framework.Test]
        public virtual void TestUnicodeFont() {
            PdfFont font = GetTTFont(PdfEncodings.IDENTITY_H, true);
            byte[] pdfBytes = CreatePdf(font);
            CheckPdf(pdfBytes);
        }

        /// <exception cref="System.Exception"/>
        private void CheckPdf(byte[] pdfBytes) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(pdfBytes)));
            // Characters from http://unicode.org/charts/PDF/U0000.pdf
            NUnit.Framework.Assert.AreEqual(TEXT1, PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            // Characters from http://unicode.org/charts/PDF/U0080.pdf
            NUnit.Framework.Assert.AreEqual(TEXT2, PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(2)));
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal static PdfFont GetTTFont(String encoding, bool embedded) {
            return PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", encoding, embedded);
        }

        /// <exception cref="System.Exception"/>
        private static byte[] CreatePdf(PdfFont font) {
            MemoryStream byteStream = new MemoryStream();
            Document document = new Document(new PdfDocument(new PdfWriter(byteStream)));
            document.Add(new Paragraph(TEXT1).SetFont(font));
            document.Add(new AreaBreak());
            document.Add(new Paragraph(TEXT2).SetFont(font));
            document.Close();
            return byteStream.ToArray();
        }
    }
}
