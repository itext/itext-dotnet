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
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CFFFontSubsetIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/CFFFontSubsetIntegrationTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/sharedFontsResourceFiles/";

        private static readonly String CJK_JP_BOLD_PATH = FONTS_FOLDER + "NotoSansCJKjp-Bold.otf";

        private const int CJK_JP_BOLD_CFF_OFFSET = 259880;

        private const int CJK_JP_BOLD_CFF_LENGTH = 16023217;

        private static readonly String JP_REGULAR_PATH = FONTS_FOLDER + "NotoSansJP-Regular_charsetDataFormat0.otf";

        private const int JP_REGULAR_CFF_OFFSET = 337316;

        private const int JP_REGULAR_CFF_LENGTH = 4210891;

        private static readonly String PURITAN_PATH = FONTS_FOLDER + "Puritan2.otf";

        [NUnit.Framework.Test]
        public virtual void SubsetNotoSansCjkJpBoldNoUsedGlyphsTest() {
            String cmpCff = SOURCE_FOLDER + "subsetNotoSansCJKjpBoldNoUsedGlyphs.cff";
            ICollection<int> glyphsUsed = JavaCollectionsUtil.EmptySet<int>();
            byte[] cffSubsetBytes = SubsetNotoSansCjkJpBoldCff(CJK_JP_BOLD_PATH, CJK_JP_BOLD_CFF_OFFSET, CJK_JP_BOLD_CFF_LENGTH
                , glyphsUsed);
            int expectedSubsetLength = 279337;
            NUnit.Framework.Assert.AreEqual(expectedSubsetLength, cffSubsetBytes.Length);
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(cmpCff));
            NUnit.Framework.Assert.AreEqual(cmpBytes, cffSubsetBytes);
        }

        [NUnit.Framework.Test]
        public virtual void SubsetNotoSansCjkJpBoldTwoUsedGlyphsTest() {
            String cmpCff = SOURCE_FOLDER + "subsetNotoSansCJKjpBoldTwoUsedGlyphs.cff";
            // In this case cid == gid for given characters.
            // \u20eab "𠺫"
            int glyphCid1 = 59715;
            // \uff14 "４"
            int glyphCid2 = 59066;
            HashSet<int> glyphsUsed = new HashSet<int>(JavaUtil.ArraysAsList(glyphCid1, glyphCid2));
            byte[] cffSubsetBytes = SubsetNotoSansCjkJpBoldCff(CJK_JP_BOLD_PATH, CJK_JP_BOLD_CFF_OFFSET, CJK_JP_BOLD_CFF_LENGTH
                , glyphsUsed);
            int expectedSubsetLength = 365381;
            NUnit.Framework.Assert.AreEqual(expectedSubsetLength, cffSubsetBytes.Length);
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(cmpCff));
            NUnit.Framework.Assert.AreEqual(cmpBytes, cffSubsetBytes);
        }

        [NUnit.Framework.Test]
        public virtual void SubsetNotoSansJpRegularOneUsedGlyphTest() {
            // In this case cid != gid for given characters.
            // \u4FE1 "信"; gid: 0x0a72 / 2674
            int glyphGid1 = 2674;
            HashSet<int> glyphsUsed = new HashSet<int>(JavaCollectionsUtil.SingletonList(glyphGid1));
            byte[] cffSubsetBytes = SubsetNotoSansCjkJpBoldCff(JP_REGULAR_PATH, JP_REGULAR_CFF_OFFSET, JP_REGULAR_CFF_LENGTH
                , glyphsUsed);
            int expectedSubsetLength = 121796;
            NUnit.Framework.Assert.AreEqual(expectedSubsetLength, cffSubsetBytes.Length);
            byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetNotoSansJPRegularOneUsedGlyph.cff"
                ));
            NUnit.Framework.Assert.AreEqual(cmpBytes, cffSubsetBytes);
        }

        [NUnit.Framework.Test]
        public virtual void SubsetNonCidCFFFontRangeCheck() {
            // 'H' (not that it matters which glyph we use)
            int glyphGid1 = 41;
            HashSet<int> glyphsUsed = new HashSet<int>(JavaCollectionsUtil.SingletonList(glyphGid1));
            byte[] cffData = new TrueTypeFont(PURITAN_PATH).GetFontStreamBytes();
            byte[] cffSubsetBytes = new CFFFontSubset(cffData, glyphsUsed).Process();
            CFFFont result = new CFFFont(cffSubsetBytes);
            int expectedCharsetLength = 255;
            // skip over the format ID (1 byte) and the first SID (2 bytes)
            result.Seek(result.fonts[0].GetCharsetOffset() + 3);
            NUnit.Framework.Assert.AreEqual(expectedCharsetLength - 2, result.GetCard16());
        }

        private byte[] SubsetNotoSansCjkJpBoldCff(String otfFile, int offsetToCff, int cffLength, ICollection<int>
             glyphsUsed) {
            RandomAccessFileOrArray fontRaf = null;
            try {
                fontRaf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(otfFile));
                byte[] cff = new byte[cffLength];
                try {
                    fontRaf.Seek(offsetToCff);
                    fontRaf.ReadFully(cff);
                }
                finally {
                    fontRaf.Close();
                }
                CFFFontSubset cffFontSubset = new CFFFontSubset(cff, glyphsUsed);
                return cffFontSubset.Process();
            }
            finally {
                if (fontRaf != null) {
                    fontRaf.Close();
                }
            }
        }
    }
}
