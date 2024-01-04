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
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class TextUtilTest : ExtendedITextTest {
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

        [NUnit.Framework.Test]
        public virtual void IsLetterPositiveTest() {
            Glyph glyph = new Glyph(0, 0, 'a');
            NUnit.Framework.Assert.IsTrue(iText.IO.Util.TextUtil.IsLetterOrDigit(glyph));
        }

        [NUnit.Framework.Test]
        public virtual void IsDigitPositiveTest() {
            Glyph glyph = new Glyph(0, 0, '8');
            NUnit.Framework.Assert.IsTrue(iText.IO.Util.TextUtil.IsLetterOrDigit(glyph));
        }

        [NUnit.Framework.Test]
        public virtual void IsLetterOrDigitNegativeTest() {
            Glyph glyph = new Glyph(0, 0, '-');
            NUnit.Framework.Assert.IsFalse(iText.IO.Util.TextUtil.IsLetterOrDigit(glyph));
        }

        [NUnit.Framework.Test]
        public virtual void IsMarkPositiveTest() {
            // TAI THAM SIGN KHUEN TONE-3
            Glyph glyph = new Glyph(0, 0, 0x1A77);
            NUnit.Framework.Assert.IsTrue(iText.IO.Util.TextUtil.IsMark(glyph));
        }

        [NUnit.Framework.Test]
        public virtual void IsMarkNegativeTest() {
            Glyph glyph = new Glyph(0, 0, '-');
            NUnit.Framework.Assert.IsFalse(iText.IO.Util.TextUtil.IsMark(glyph));
        }

        private void Helper(bool expected, int currentCRPosition, params Glyph[] glyphs) {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(glyphs));
            NUnit.Framework.Assert.IsTrue(expected == iText.IO.Util.TextUtil.IsCarriageReturnFollowedByLineFeed(glyphLine
                , currentCRPosition));
        }
    }
}
