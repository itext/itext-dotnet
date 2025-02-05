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
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class Type3FontTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddGlyphTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 1, 600, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
        }

        [NUnit.Framework.Test]
        public virtual void AddGlyphsWithDifferentUnicodeTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 1, 600, null, null);
            font.AddGlyph(2, 2, 600, null, null);
            NUnit.Framework.Assert.AreEqual(2, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(1, font.GetGlyphByCode(1).GetUnicode());
            NUnit.Framework.Assert.AreEqual(2, font.GetGlyphByCode(2).GetUnicode());
        }

        [NUnit.Framework.Test]
        public virtual void AddGlyphsWithDifferentCodesTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, -1, 600, null, null);
            font.AddGlyph(2, -1, 700, null, null);
            NUnit.Framework.Assert.AreEqual(2, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(600, font.GetGlyphByCode(1).GetWidth());
            NUnit.Framework.Assert.AreEqual(700, font.GetGlyphByCode(2).GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphsWithSameUnicodeTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 1, 600, null, null);
            font.AddGlyph(2, 1, 600, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(2, font.GetGlyph(1).GetCode());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphWithSameCodeTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, -1, 600, null, null);
            font.AddGlyph(1, -1, 700, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(700, font.GetGlyphByCode(1).GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void NotAddGlyphWithSameCodeEmptyUnicodeFirstTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, -1, 600, null, null);
            font.AddGlyph(1, 100, 600, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(1, font.GetGlyph(100).GetCode());
            NUnit.Framework.Assert.AreEqual(100, font.GetGlyphByCode(1).GetUnicode());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphWithSameCodeEmptyUnicodeLastTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 100, 600, null, null);
            font.AddGlyph(1, -1, 600, null, null);
            NUnit.Framework.Assert.IsNull(font.GetGlyph(-1));
            NUnit.Framework.Assert.IsNull(font.GetGlyph(100));
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(-1, font.GetGlyphByCode(1).GetUnicode());
        }
    }
}
