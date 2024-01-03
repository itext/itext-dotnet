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
using System.Collections.Generic;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.Layout.Splitting {
    [NUnit.Framework.Category("UnitTest")]
    public class BreakAllSplitCharactersTest : ExtendedITextTest {
        private const char charWithFalse = '\u201b';

        [NUnit.Framework.Test]
        public virtual void LastCharTest() {
            NUnit.Framework.Assert.IsFalse(IsSplitCharacter(new int[] { charWithFalse, charWithFalse, charWithFalse }, 
                1));
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, charWithFalse, charWithFalse }, 
                2));
        }

        [NUnit.Framework.Test]
        public virtual void CurrentIsNotUnicodeTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, -1, charWithFalse }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void NextIsNotUnicodeTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, charWithFalse, -1 }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void BeforeSpaceTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', 'a', ' ' }, 0));
            NUnit.Framework.Assert.IsFalse(IsSplitCharacter(new int[] { 'a', 'a', ' ' }, 1));
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', ' ', ' ' }, 1));
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', '-', ' ' }, 1));
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', '\u2010', ' ' }, 1));
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', '\u2004', ' ' }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void BeforeSymbolTest() {
            NUnit.Framework.Assert.IsFalse(IsSplitCharacter(new int[] { charWithFalse, charWithFalse }, 0));
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, 'a' }, 0));
            // non spacing mark
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, '\u0303' }, 0));
            // combining mark
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, '\u093e' }, 0));
            // enclosing mark
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { charWithFalse, '\u0488' }, 0));
        }

        private static bool IsSplitCharacter(int[] unicodes, int glyphPosition) {
            return new BreakAllSplitCharacters().IsSplitCharacter(CreateGlyphLine(unicodes), glyphPosition);
        }

        private static GlyphLine CreateGlyphLine(int[] unicodes) {
            IList<Glyph> glyphs = new List<Glyph>();
            foreach (int unicode in unicodes) {
                glyphs.Add(new Glyph(1, unicode));
            }
            return new GlyphLine(glyphs);
        }
    }
}
