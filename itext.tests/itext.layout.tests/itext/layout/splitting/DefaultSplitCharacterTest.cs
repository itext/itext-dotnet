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
using System.Collections.Generic;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.Layout.Splitting {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultSplitCharacterTest : ExtendedITextTest {
//\cond DO_NOT_DOCUMENT
        internal static IList<Glyph> glyphs = new List<Glyph>();
//\endcond

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            glyphs.Add(new Glyph(1, '-'));
            glyphs.Add(new Glyph(1, '5'));
            glyphs.Add(new Glyph(1, '2'));
            glyphs.Add(new Glyph(1, '5'));
            glyphs.Add(new Glyph(1, '-'));
            glyphs.Add(new Glyph(1, '5'));
            glyphs.Add(new Glyph(1, '5'));
            glyphs.Add(new Glyph(1, '7'));
            glyphs.Add(new Glyph(1, '-'));
            glyphs.Add(new Glyph(1, '-'));
            glyphs.Add(new Glyph(1, '-'));
            glyphs.Add(new Glyph(1, '7'));
            glyphs.Add(new Glyph(1, '5'));
            glyphs.Add(new Glyph(1, '-'));
            glyphs.Add(new Glyph(1, '-'));
        }

        [NUnit.Framework.Test]
        public virtual void BeginCharacterTest() {
            NUnit.Framework.Assert.IsFalse(IsPsplitCharacter(0));
        }

        [NUnit.Framework.Test]
        public virtual void MiddleCharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsPsplitCharacter(4));
        }

        [NUnit.Framework.Test]
        public virtual void LastCharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsPsplitCharacter(8));
        }

        [NUnit.Framework.Test]
        public virtual void FirstMiddleCharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsPsplitCharacter(9));
        }

        [NUnit.Framework.Test]
        public virtual void LastMiddleCharacterTest() {
            NUnit.Framework.Assert.IsTrue(IsPsplitCharacter(14));
        }

        private static bool IsPsplitCharacter(int glyphPos) {
            GlyphLine text = new GlyphLine(glyphs);
            return new DefaultSplitCharacters().IsSplitCharacter(text, glyphPos);
        }
    }
}
