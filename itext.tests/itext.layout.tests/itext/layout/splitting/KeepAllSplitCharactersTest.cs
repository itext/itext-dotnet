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
    public class KeepAllSplitCharactersTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DashAtStartTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { '-', 'a' }, 0));
        }

        [NUnit.Framework.Test]
        public virtual void MinusSignAtStartTest() {
            NUnit.Framework.Assert.IsFalse(IsSplitCharacter(new int[] { '-', '5' }, 0));
        }

        [NUnit.Framework.Test]
        public virtual void DashBeforeLetterInTheMiddleTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', ' ', '-', 'a' }, 2));
        }

        [NUnit.Framework.Test]
        public virtual void MinusSignInTheMiddleTest() {
            // TODO: DEVSIX-4863 minus sign for digests should not be split
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', ' ', '-', '5' }, 2));
        }

        [NUnit.Framework.Test]
        public virtual void DashBeforeDigitInTheMiddleTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', 'a', '-', '5' }, 2));
        }

        [NUnit.Framework.Test]
        public virtual void DashAtTheEndTest() {
            int[] unicodes = new int[] { 'a', '-' };
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(unicodes, unicodes.Length - 1));
        }

        [NUnit.Framework.Test]
        public virtual void DashCharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', '-', 'a' }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void NoUnicodeTest() {
            NUnit.Framework.Assert.IsFalse(IsSplitCharacter(new int[] { 'a', -1, 'a' }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void Unicode2010CharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', '\u2010', 'a' }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void Unicode2003CharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsSplitCharacter(new int[] { 'a', '\u2003', 'a' }, 1));
        }

        [NUnit.Framework.Test]
        public virtual void Unicode2e81CharacterTest() {
            NUnit.Framework.Assert.IsFalse(IsSplitCharacter(new int[] { 'a', '\u2e81', 'a' }, 1));
        }

        private static bool IsSplitCharacter(int[] unicodes, int glyphPosition) {
            return new KeepAllSplitCharacters().IsSplitCharacter(CreateGlyphLine(unicodes), glyphPosition);
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
