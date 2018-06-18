using iText.IO.Util;

namespace iText.IO.Font.Otf {
    public class ActualTextIteratorTest {
        [NUnit.Framework.Test]
        public virtual void TestActualTestParts() {
            Glyph glyph = new Glyph(200, 200, '\u002d');
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(glyph));
            glyphLine.SetActualText(0, 1, "\u002d");
            ActualTextIterator actualTextIterator = new ActualTextIterator(glyphLine);
            GlyphLine.GlyphLinePart part = actualTextIterator.Next();
            // When actual text is the same as the result by text extraction, we should omit redundant actual text in the content stream
            NUnit.Framework.Assert.IsNull(part.actualText);
        }
    }
}
