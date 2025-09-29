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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class TrueTypeFontTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/sharedFontsResourceFiles/";

        [NUnit.Framework.Test]
        public virtual void NotoSansJpCmapTest() {
            // 信
            char jpChar = '\u4FE1';
            FontProgram fontProgram = FontProgramFactory.CreateFont(SOURCE_FOLDER + "NotoSansJP-Regular_charsetDataFormat0.otf"
                );
            Glyph glyph = fontProgram.GetGlyph(jpChar);
            NUnit.Framework.Assert.AreEqual(new char[] { jpChar }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(20449, glyph.GetUnicode());
            NUnit.Framework.Assert.AreEqual(10195, glyph.GetCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansScCmapTest() {
            // 易
            char chChar = '\u6613';
            FontProgram fontProgram = FontProgramFactory.CreateFont(SOURCE_FOLDER + "NotoSansSC-Regular.otf");
            Glyph glyph = fontProgram.GetGlyph(chChar);
            NUnit.Framework.Assert.AreEqual(new char[] { chChar }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(26131, glyph.GetUnicode());
            NUnit.Framework.Assert.AreEqual(20292, glyph.GetCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansTcCmapTest() {
            // 易
            char chChar = '\u6613';
            FontProgram fontProgram = FontProgramFactory.CreateFont(SOURCE_FOLDER + "NotoSansTC-Regular.otf");
            Glyph glyph = fontProgram.GetGlyph(chChar);
            NUnit.Framework.Assert.AreEqual(new char[] { chChar }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(26131, glyph.GetUnicode());
            NUnit.Framework.Assert.AreEqual(20292, glyph.GetCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansScMapGlyphsCidsToGidsTest() {
            // 易
            char chChar = '\u6613';
            int charCidInFont = 20292;
            int charGidInFont = 14890;
            TrueTypeFont trueTypeFontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SOURCE_FOLDER + "NotoSansSC-Regular.otf"
                );
            HashSet<int> glyphs = new HashSet<int>(JavaCollectionsUtil.SingletonList(charCidInFont));
            ICollection<int> actualResult = trueTypeFontProgram.MapGlyphsCidsToGids(glyphs);
            NUnit.Framework.Assert.AreEqual(1, actualResult.Count);
            NUnit.Framework.Assert.IsTrue(actualResult.Contains(charGidInFont));
        }

        [NUnit.Framework.Test]
        public virtual void CmapPlatform0PlatEnc3Format4Test() {
            FontProgram fontProgram = FontProgramFactory.CreateFont(SOURCE_FOLDER + "glyphs.ttf");
            CheckCmapTableEntry(fontProgram, 'f', 2);
            CheckCmapTableEntry(fontProgram, 'i', 3);
        }

        [NUnit.Framework.Test]
        public virtual void CmapPlatform0PlatEnc3Format6Test() {
            FontProgram fontProgram = FontProgramFactory.CreateFont(SOURCE_FOLDER + "glyphs-fmt-6.ttf");
            CheckCmapTableEntry(fontProgram, 'f', 2);
            CheckCmapTableEntry(fontProgram, 'i', 3);
        }

        [NUnit.Framework.Test]
        public virtual void CheckSxHeightTtfTest() {
            FontProgram fontProgram = FontProgramFactory.CreateFont(SOURCE_FOLDER + "glyphs-fmt-6.ttf");
            FontMetrics metrics = fontProgram.GetFontMetrics();
            int xHeight = metrics.GetXHeight();
            NUnit.Framework.Assert.AreEqual(536, xHeight);
        }

        [NUnit.Framework.Test]
        public virtual void ContainsCmapTest() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SOURCE_FOLDER + "glyphs-fmt-6.ttf");
            NUnit.Framework.Assert.AreEqual(1, fontProgram.GetNumberOfCmaps());
            NUnit.Framework.Assert.IsTrue(fontProgram.IsCmapPresent(0, 3));
            NUnit.Framework.Assert.IsFalse(fontProgram.IsCmapPresent(1, 0));
        }

        [NUnit.Framework.Test]
        public virtual void UpdateUsedGlyphsSetTest() {
            TrueTypeFont trueTypeFontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SOURCE_FOLDER + "NotoSansSC-Regular.otf"
                );
            SortedSet<int> usedGlyphs = new SortedSet<int>();
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, true, null);
            NUnit.Framework.Assert.IsTrue(usedGlyphs.IsEmpty());
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, false, null);
            NUnit.Framework.Assert.AreEqual(40644, usedGlyphs.Count);
            usedGlyphs.Clear();
            IList<int[]> subsetRanges = new List<int[]>();
            subsetRanges.Add(new int[] { 0, 100 });
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, false, subsetRanges);
            NUnit.Framework.Assert.AreEqual(101, usedGlyphs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void UpdateUsedGlyphsMapTest() {
            TrueTypeFont trueTypeFontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SOURCE_FOLDER + "NotoSansSC-Regular.otf"
                );
            IDictionary<int, Glyph> usedGlyphs = new Dictionary<int, Glyph>();
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, true, null);
            NUnit.Framework.Assert.IsTrue(usedGlyphs.IsEmpty());
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, false, null);
            NUnit.Framework.Assert.AreEqual(40644, usedGlyphs.Count);
            usedGlyphs.Clear();
            IList<int[]> subsetRanges = new List<int[]>();
            subsetRanges.Add(new int[] { 0, 100 });
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, false, subsetRanges);
            NUnit.Framework.Assert.AreEqual(101, usedGlyphs.Count);
            usedGlyphs.Clear();
            usedGlyphs.Put(1, trueTypeFontProgram.GetGlyphByCode(10));
            subsetRanges = new List<int[]>();
            subsetRanges.Add(new int[] { 1, 1 });
            trueTypeFontProgram.UpdateUsedGlyphs(usedGlyphs, false, subsetRanges);
            NUnit.Framework.Assert.AreEqual(1, usedGlyphs.Count);
            NUnit.Framework.Assert.AreSame(trueTypeFontProgram.GetGlyphByCode(10), usedGlyphs.Get(1));
        }

        private void CheckCmapTableEntry(FontProgram fontProgram, char uniChar, int expectedGlyphId) {
            Glyph glyph = fontProgram.GetGlyph(uniChar);
            NUnit.Framework.Assert.AreEqual(expectedGlyphId, glyph.GetCode());
            NUnit.Framework.Assert.AreEqual(new char[] { uniChar }, glyph.GetUnicodeChars());
        }
    }
}
