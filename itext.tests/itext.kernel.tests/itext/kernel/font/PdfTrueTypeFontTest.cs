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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfTrueTypeFontTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/PdfTrueTypeFontTest/";

        [NUnit.Framework.Test]
        public virtual void TestReadingPdfTrueTypeFontWithType1StandardFontProgram() {
            // We deliberately use an existing PDF in this test and not simplify the test to create the
            // PDF object structure on the fly to be able to easily inspect the PDF with other processors
            String filePath = SOURCE_FOLDER + "trueTypeFontWithStandardFontProgram.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath));
            PdfDictionary fontDict = pdfDocument.GetPage(1).GetResources().GetResource(PdfName.Font).GetAsDictionary(new 
                PdfName("F1"));
            PdfFont pdfFont = PdfFontFactory.CreateFont(fontDict);
            NUnit.Framework.Assert.AreEqual(542, pdfFont.GetFontProgram().GetAvgWidth());
            NUnit.Framework.Assert.AreEqual(556, pdfFont.GetGlyph('a').GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void IsBuiltInTest() {
            PdfFont font = PdfFontFactory.CreateFont(CreateTrueTypeFontDictionaryWithStandardHelveticaFont());
            NUnit.Framework.Assert.IsTrue(font is PdfTrueTypeFont);
            NUnit.Framework.Assert.IsTrue(((PdfTrueTypeFont)font).IsBuiltInFont());
        }

        [NUnit.Framework.Test]
        public virtual void IsNotBuiltInTest() {
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSans-Regular.ttf", PdfEncodings.WINANSI);
            NUnit.Framework.Assert.IsTrue(font is PdfTrueTypeFont);
            NUnit.Framework.Assert.IsFalse(((PdfTrueTypeFont)font).IsBuiltInFont());
        }

        private static PdfDictionary CreateTrueTypeFontDictionaryWithStandardHelveticaFont() {
            PdfDictionary fontDictionary = new PdfDictionary();
            fontDictionary.Put(PdfName.Type, PdfName.Font);
            fontDictionary.Put(PdfName.Subtype, PdfName.TrueType);
            fontDictionary.Put(PdfName.Encoding, PdfName.WinAnsiEncoding);
            fontDictionary.Put(PdfName.BaseFont, new PdfName(StandardFonts.HELVETICA));
            return fontDictionary;
        }
    }
}
