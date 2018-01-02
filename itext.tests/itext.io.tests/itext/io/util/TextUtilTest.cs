using iText.IO.Font.Otf;

namespace iText.IO.Util {
    public class TextUtilTest {
        private Glyph carriageReturn;

        private Glyph lineFeed;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            this.carriageReturn = new Glyph(0, 0, '\r');
            this.lineFeed = new Glyph(0, 0, '\n');
        }

        [NUnit.Framework.Test]
        public virtual void CarriageReturnFollowedByLineFeedTest() {
            Helper(true, 0, carriageReturn, lineFeed);
        }

        [NUnit.Framework.Test]
        public virtual void CarriageReturnFollowedByCarriageReturnAndThenLineFeedTest() {
            Helper(false, 0, carriageReturn, carriageReturn, lineFeed);
        }

        [NUnit.Framework.Test]
        public virtual void CarriageReturnPrecededByCarriageReturnAndFollowedByLineFeedTest() {
            Helper(true, 1, carriageReturn, carriageReturn, lineFeed);
        }

        [NUnit.Framework.Test]
        public virtual void CarriageReturnFollowedByNothingTest() {
            Helper(false, 0, carriageReturn);
        }

        [NUnit.Framework.Test]
        public virtual void CarriageReturnPrecededByLineFeedTest() {
            Helper(false, 0, lineFeed, carriageReturn);
        }

        [NUnit.Framework.Test]
        public virtual void CarriageReturnPrecededByTextFollowedByLineFeedTest() {
            Helper(true, 1, new Glyph(0, 0, 'a'), carriageReturn, lineFeed);
        }

        private void Helper(bool expected, int currentCRPosition, params Glyph[] glyphs) {
            GlyphLine glyphLine = new GlyphLine(iText.IO.Util.JavaUtil.ArraysAsList(glyphs));
            NUnit.Framework.Assert.IsTrue(expected == TextUtil.IsCarriageReturnFollowedByLineFeed(glyphLine, currentCRPosition
                ));
        }
    }
}
