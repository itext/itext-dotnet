using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
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

        [NUnit.Framework.Test]
        public virtual void NotoSerifWithInvalidXYPlacementAnchorDeltaTest() {
            String outPdf = destinationFolder + "notoSerifWithInvalidXYPlacementAnchorDeltaTest.pdf";
            String cmpPdf = sourceFolder + "cmp_notoSerifWithInvalidXYPlacementAnchorDeltaTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
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
