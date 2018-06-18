using System.Collections.Generic;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    public class GlyphLineTest {
        [NUnit.Framework.Test]
        public virtual void TestEquals() {
            Glyph glyph = new Glyph(200, 200, 200);
            GlyphLine.ActualText actualText = new GlyphLine.ActualText("-");
            GlyphLine one = new GlyphLine(new List<Glyph>(JavaUtil.ArraysAsList(glyph)), new List<GlyphLine.ActualText
                >(JavaUtil.ArraysAsList(actualText)), 0, 1);
            GlyphLine two = new GlyphLine(new List<Glyph>(JavaUtil.ArraysAsList(glyph)), new List<GlyphLine.ActualText
                >(JavaUtil.ArraysAsList(actualText)), 0, 1);
            one.Add(glyph);
            two.Add(glyph);
            one.end++;
            two.end++;
            NUnit.Framework.Assert.IsTrue(one.Equals(two));
        }
    }
}
