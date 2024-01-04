/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontToUnicodeTest : ExtendedITextTest {
        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/FontToUnicodeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralUnicodesWithinOneGlyphTest() {
            // TODO DEVSIX-3634. In the output now we don't expect the \u2F46 unicode range.
            // TODO DEVSIX-3634. SUBSTITUTE "Assert.assertEquals("\u2F46"..." to "Assert.assertEquals("\u65E0"..." after the fix
            String outFileName = destinationFolder + "severalUnicodesWithinOneGlyphTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName));
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", PdfEncodings.IDENTITY_H);
            IList<Glyph> glyphs = JavaCollectionsUtil.SingletonList(font.GetGlyph((int)'\u65E0'));
            GlyphLine glyphLine = new GlyphLine(glyphs);
            PdfCanvas canvas2 = new PdfCanvas(pdfDocument.AddNewPage());
            canvas2.SaveState().BeginText().MoveText(36, 800).SetFontAndSize(font, 12).ShowText(glyphLine).EndText().RestoreState
                ();
            pdfDocument.Close();
            PdfDocument resultantPdfAsFile = new PdfDocument(CompareTool.CreateOutputReader(outFileName));
            String actualUnicode = PdfTextExtractor.GetTextFromPage(resultantPdfAsFile.GetFirstPage());
            NUnit.Framework.Assert.AreEqual("\u2F46", actualUnicode);
        }
    }
}
