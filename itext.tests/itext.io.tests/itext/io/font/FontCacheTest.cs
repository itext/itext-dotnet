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
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCacheTest : ExtendedITextTest {
        [NUnit.Framework.SetUp]
        public virtual void Before() {
            FontCache.ClearSavedFonts();
        }

        [NUnit.Framework.Test]
        public virtual void ClearFontCacheTest() {
            String fontName = "FreeSans.ttf";
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(fontName));
            FontProgram fontProgram = new FontCacheTest.FontProgramMock();
            FontCache.SaveFont(fontProgram, fontName);
            NUnit.Framework.Assert.AreEqual(fontProgram, FontCache.GetFont(fontName));
            FontCache.ClearSavedFonts();
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(fontName));
        }

        [NUnit.Framework.Test]
        public virtual void FontStringTtcCacheKeyTest() {
            String fontName = "Font.ttc";
            FontCacheKey ttc0 = FontCacheKey.Create(fontName, 0);
            FontCacheKey ttc1 = FontCacheKey.Create(fontName, 1);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc1));
            FontProgram fontProgram = new FontCacheTest.FontProgramMock();
            FontCache.SaveFont(fontProgram, ttc1);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.AreEqual(fontProgram, FontCache.GetFont(ttc1));
        }

        [NUnit.Framework.Test]
        public virtual void FontBytesTtcCacheKeyTest() {
            byte[] fontBytes = "SupposedTtcFontData".GetBytes(System.Text.Encoding.UTF8);
            byte[] otherFontBytes = "DifferentTtcFontBytes".GetBytes(System.Text.Encoding.UTF8);
            byte[] normalFontBytes = "NormalFontBytes".GetBytes(System.Text.Encoding.UTF8);
            FontCacheKey ttc0 = FontCacheKey.Create(fontBytes, 1);
            FontCacheKey otherTtc0 = FontCacheKey.Create(otherFontBytes, 1);
            FontCacheKey normal = FontCacheKey.Create(normalFontBytes);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(otherTtc0));
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(normal));
            FontProgram otherTtc0MockFontProgram = new FontCacheTest.FontProgramMock();
            FontProgram normalMockFontProgram = new FontCacheTest.FontProgramMock();
            FontCache.SaveFont(otherTtc0MockFontProgram, otherTtc0);
            FontCache.SaveFont(normalMockFontProgram, normal);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.AreEqual(otherTtc0MockFontProgram, FontCache.GetFont(otherTtc0));
            NUnit.Framework.Assert.AreEqual(normalMockFontProgram, FontCache.GetFont(normal));
        }

        private class FontProgramMock : FontProgram {
            public override int GetPdfFontFlags() {
                return 0;
            }

            public override int GetKerning(Glyph first, Glyph second) {
                return 0;
            }
        }
    }
}
