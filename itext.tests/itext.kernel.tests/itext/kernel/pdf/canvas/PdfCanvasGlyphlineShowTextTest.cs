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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCanvasGlyphlineShowTextTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasGlyphlineShowTextTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasGlyphlineShowTextTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void NotoSerifWithInvalidXYPlacementAnchorDeltaTest() {
            String outPdf = destinationFolder + "notoSerifWithInvalidXYPlacementAnchorDeltaTest.pdf";
            String cmpPdf = sourceFolder + "cmp_notoSerifWithInvalidXYPlacementAnchorDeltaTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage page = pdfDoc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSerif-Regular_v1.7.ttf", PdfEncodings.IDENTITY_H
                );
            // ゙B̸̭̼ͣ̎̇
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(font.GetGlyph((int)'\u0042'), ApplyGlyphParameters('\u0363', -
                1, 327, 178, font), ApplyGlyphParameters('\u030e', -1, 10, 298, font), ApplyGlyphParameters('\u0307', 
                -1, 0, 224, font), ApplyGlyphParameters('\u032d', -3, 11, 620, font), ApplyGlyphParameters('\u033c', -
                1, -1, -220, font), font.GetGlyph((int)'\u0338'));
            GlyphLine glyphLine = new GlyphLine(glyphs);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 800).SetFontAndSize(font, 12).ShowText(glyphLine).EndText().RestoreState
                ();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        private Glyph ApplyGlyphParameters(char glyphUni, int anchorDelta, int xPlacement, int yPlacement, PdfFont
             font) {
            Glyph glyph = font.GetGlyph((int)glyphUni);
            glyph.SetAnchorDelta((short)anchorDelta);
            glyph.SetXPlacement((short)xPlacement);
            glyph.SetYPlacement((short)yPlacement);
            return glyph;
        }
    }
}
