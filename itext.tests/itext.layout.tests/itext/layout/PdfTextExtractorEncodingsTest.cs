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
