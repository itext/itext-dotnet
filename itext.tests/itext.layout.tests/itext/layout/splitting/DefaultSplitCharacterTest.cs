using System.Collections.Generic;
using iText.IO.Font.Otf;

namespace iText.Layout.Splitting {
    public class DefaultSplitCharacterTest {
        internal static IList<Glyph> glyphs = new List<Glyph>();

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
