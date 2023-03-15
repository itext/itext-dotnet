/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
    }
}
