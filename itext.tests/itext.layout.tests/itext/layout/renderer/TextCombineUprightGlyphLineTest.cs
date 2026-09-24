/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class TextCombineUprightGlyphLineTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EachRunIsOnePlaceholderAndNewlinesArePreservedTest() {
            String[] sources = new String[] { "", "12 34-56", "12\r\n34\n", "\n\r\n\r12\n" };
            String[] expected = new String[] { "", "\uFFFC", "\uFFFC\r\n\uFFFC\n", "\uFFFC\n\uFFFC\r\n\uFFFC\r\uFFFC\n"
                 };
            for (int i = 0; i < sources.Length; ++i) {
                GlyphLine source = Glyphs(sources[i]);
                TextCombineUprightGlyphLine placeholders = new TextCombineUprightGlyphLine(source, 800, -200);
                NUnit.Framework.Assert.AreEqual(expected[i], placeholders.ToString());
                NUnit.Framework.Assert.AreEqual(sources[i], placeholders.Restore(placeholders).ToString());
                NUnit.Framework.Assert.AreEqual(sources[i], source.ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RestoreKeepsSourceOffsetsShapingAndActualTextTest() {
            GlyphLine source = Glyphs("[[12\r\n34\n]]");
            source.SetStart(2);
            source.SetEnd(9);
            source.Get(2).SetXAdvance((short)75);
            source.SetActualText(2, 4, "twelve");
            TextCombineUprightGlyphLine placeholders = new TextCombineUprightGlyphLine(source, 900, -250);
            GlyphLine placed = new GlyphLine(placeholders);
            placed.SetEnd(1);
            GlyphLine restored = placeholders.Restore(placed);
            NUnit.Framework.Assert.AreEqual(2, restored.GetStart());
            NUnit.Framework.Assert.AreEqual(4, restored.GetEnd());
            NUnit.Framework.Assert.AreEqual("twelve", restored.ToString());
            NUnit.Framework.Assert.AreSame(source.Get(2), restored.Get(2));
            NUnit.Framework.Assert.AreEqual(75, restored.Get(2).GetXAdvance());
            NUnit.Framework.Assert.AreEqual(6, placeholders.GetSourcePosition(3));
            GlyphLine overflow = new GlyphLine(placeholders);
            overflow.SetStart(3);
            NUnit.Framework.Assert.AreEqual("34\n", placeholders.Restore(overflow).ToString());
            NUnit.Framework.Assert.AreEqual("twelve\r\n34\n", source.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyRunAndUnplacedLineRestoreToEmptySourceRangesTest() {
            GlyphLine source = Glyphs("\r\n12");
            TextCombineUprightGlyphLine placeholders = new TextCombineUprightGlyphLine(source, 800, -200);
            GlyphLine placed = new GlyphLine(placeholders);
            placed.SetEnd(1);
            NUnit.Framework.Assert.AreEqual("", placeholders.Restore(placed).ToString());
            placed.SetStart(-1);
            placed.SetEnd(-1);
            NUnit.Framework.Assert.AreEqual("", placeholders.Restore(placed).ToString());
            NUnit.Framework.Assert.AreEqual("\r\n12", source.ToString());
        }

        private static GlyphLine Glyphs(String text) {
            GlyphLine result = new GlyphLine();
            for (int i = 0; i < text.Length; ++i) {
                result.Add(new Glyph(text[i], 500, text[i]));
            }
            result.SetEnd(result.Size());
            return result;
        }
    }
}
