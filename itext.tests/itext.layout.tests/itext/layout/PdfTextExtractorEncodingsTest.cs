/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfTextExtractorEncodingsTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfTextExtractorEncodingsTest/";

        /// <summary>Basic Latin characters, with Unicode values less than 128</summary>
        private const String TEXT1 = "AZaz09*!";

        /// <summary>Latin-1 characters</summary>
        private const String TEXT2 = "\u0027\u0060\u00a4\u00a6";

        /// <summary>Test parsing a document which uses a standard non-embedded font.</summary>
        [NUnit.Framework.Test]
        public virtual void TestStandardFont() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            byte[] pdfBytes = CreatePdf(font);
            CheckPdf(pdfBytes);
        }

        /// <summary>
        /// Test parsing a document which uses a font encoding which creates a /Differences
        /// PdfArray in the PDF.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void TestEncodedFont() {
            PdfFont font = GetTTFont("ISO-8859-1", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            byte[] pdfBytes = CreatePdf(font);
            CheckPdf(pdfBytes);
        }

        /// <summary>
        /// Test parsing a document which uses a Unicode font encoding which creates a /ToUnicode
        /// PdfArray.
        /// </summary>
        [NUnit.Framework.Test]
        public virtual void TestUnicodeFont() {
            PdfFont font = GetTTFont(PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            byte[] pdfBytes = CreatePdf(font);
            CheckPdf(pdfBytes);
        }

        private void CheckPdf(byte[] pdfBytes) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(pdfBytes)));
            // Characters from http://unicode.org/charts/PDF/U0000.pdf
            NUnit.Framework.Assert.AreEqual(TEXT1, PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            // Characters from http://unicode.org/charts/PDF/U0080.pdf
            NUnit.Framework.Assert.AreEqual(TEXT2, PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(2)));
        }

        protected internal static PdfFont GetTTFont(String encoding, PdfFontFactory.EmbeddingStrategy embeddingStrategy
            ) {
            return PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", encoding, embeddingStrategy);
        }

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
