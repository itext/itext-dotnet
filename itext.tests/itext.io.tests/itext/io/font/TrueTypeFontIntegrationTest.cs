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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TrueTypeFontIntegrationTest : ExtendedITextTest {
        private static readonly String SHARED_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/sharedFontsResourceFiles/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/TrueTypeFontIntegrationTest/";

        [NUnit.Framework.Test]
        public virtual void SimpleSubsetTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SHARED_FOLDER + "NotoSans-Regular.ttf"));
            TrueTypeFont font = FontProgramFactory.CreateTrueTypeFont(fontBytes, false);
            ICollection<int> usedGlyphs = new HashSet<int>();
            // these GIDs correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            byte[] subsetFontBytes = font.GetSubset(usedGlyphs, true);
            TrueTypeFont subsetFont = FontProgramFactory.CreateTrueTypeFont(subsetFontBytes, true);
            NUnit.Framework.Assert.AreEqual(3271, font.bBoxes.Length);
            NUnit.Framework.Assert.AreEqual(39, subsetFont.bBoxes.Length);
            NUnit.Framework.Assert.IsNotNull(subsetFont.bBoxes[36]);
            NUnit.Framework.Assert.IsNull(subsetFont.bBoxes[35]);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSubsetWithoutTableSubsetTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SHARED_FOLDER + "NotoSans-Regular.ttf"));
            TrueTypeFont font = FontProgramFactory.CreateTrueTypeFont(fontBytes, false);
            ICollection<int> usedGlyphs = new HashSet<int>();
            // these GIDs correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            byte[] subsetFontBytes = font.GetSubset(usedGlyphs, false);
            TrueTypeFont subsetFont = FontProgramFactory.CreateTrueTypeFont(subsetFontBytes, false);
            NUnit.Framework.Assert.AreEqual(3271, font.bBoxes.Length);
            NUnit.Framework.Assert.AreEqual(39, subsetFont.bBoxes.Length);
            NUnit.Framework.Assert.IsNotNull(subsetFont.bBoxes[36]);
            NUnit.Framework.Assert.IsNull(subsetFont.bBoxes[35]);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSubsetMergeTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset1.ttf"));
            // Subset for XBC
            TrueTypeFont subset1 = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset2.ttf"));
            // Subset for ABC
            TrueTypeFont subset2 = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            IDictionary<TrueTypeFont, ICollection<int>> toMerge = new Dictionary<TrueTypeFont, ICollection<int>>();
            ICollection<int> usedGlyphs = new HashSet<int>();
            // GID correspond to B
            usedGlyphs.Add(37);
            toMerge.Put(subset1, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to A
            usedGlyphs.Add(36);
            toMerge.Put(subset2, usedGlyphs);
            byte[] mergeFontBytes = TrueTypeFont.Merge(toMerge, "NotoSans-Regular", false);
            TrueTypeFont mergeFont = FontProgramFactory.CreateTrueTypeFont(mergeFontBytes, true);
            // C glyphs wasn't used, it's why it was cut from merge font
            NUnit.Framework.Assert.IsNotNull(subset1.bBoxes[38]);
            NUnit.Framework.Assert.IsNotNull(subset2.bBoxes[38]);
            NUnit.Framework.Assert.AreEqual(38, mergeFont.bBoxes.Length);
        }

        [NUnit.Framework.Test]
        public virtual void NoCommonCmapPdfTrueTypeMergeTest() {
            // subsets are created using fonttools Python lib with the following command
            // fonttools subset ./NotoSans-Regular.ttf --text="ABC" --retain-gids --layout-features='*' --notdef-glyph --output-file=subset_abc.ttf
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_abc.ttf"));
            TrueTypeFont subsetAbc = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_def.ttf"));
            TrueTypeFont subsetDef = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_xyz.ttf"));
            TrueTypeFont subsetXyz = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            IDictionary<TrueTypeFont, ICollection<int>> toMerge = new Dictionary<TrueTypeFont, ICollection<int>>();
            ICollection<int> usedGlyphs = new HashSet<int>();
            // GID correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            toMerge.Put(subsetAbc, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to DEF
            usedGlyphs.Add(39);
            usedGlyphs.Add(40);
            usedGlyphs.Add(41);
            toMerge.Put(subsetDef, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to XYZ
            usedGlyphs.Add(59);
            usedGlyphs.Add(60);
            usedGlyphs.Add(61);
            toMerge.Put(subsetXyz, usedGlyphs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TrueTypeFont.Merge
                (toMerge, "NotoSans-Regular", true));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.CMAP_TABLE_MERGING_IS_NOT_SUPPORTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CommonCmapPdfTrueTypeMergeTest() {
            // subsets are created using fonttools Python lib with the following command
            // fonttools subset ./NotoSans-Regular.ttf --text="ABC" --retain-gids --layout-features='*' --notdef-glyph --output-file=subset_abc.ttf
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_abc.ttf"));
            TrueTypeFont subsetAbc = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_abc_def_xyz.ttf"));
            TrueTypeFont subsetAbcDefXyz = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_def.ttf"));
            TrueTypeFont subsetDef = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_xyz.ttf"));
            TrueTypeFont subsetXyz = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            IDictionary<TrueTypeFont, ICollection<int>> toMerge = new Dictionary<TrueTypeFont, ICollection<int>>();
            ICollection<int> usedGlyphs = new HashSet<int>();
            // GID correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            toMerge.Put(subsetAbc, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to ADX
            usedGlyphs.Add(36);
            usedGlyphs.Add(39);
            usedGlyphs.Add(59);
            toMerge.Put(subsetAbcDefXyz, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to DEF
            usedGlyphs.Add(39);
            usedGlyphs.Add(40);
            usedGlyphs.Add(41);
            toMerge.Put(subsetDef, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to XYZ
            usedGlyphs.Add(59);
            usedGlyphs.Add(60);
            usedGlyphs.Add(61);
            toMerge.Put(subsetXyz, usedGlyphs);
            byte[] mergeFontBytes = TrueTypeFont.Merge(toMerge, "NotoSans-Regular", true);
            TrueTypeFont mergeFont = FontProgramFactory.CreateTrueTypeFont(mergeFontBytes, true);
            // `cmap` table contains mapping for all used glyphs
            NUnit.Framework.Assert.AreEqual(9, mergeFont.GetActiveCmap().Count);
            // `glyf` table contains data for all used glyphs
            NUnit.Framework.Assert.AreEqual(62, mergeFont.bBoxes.Length);
            NUnit.Framework.Assert.IsNotNull(mergeFont.bBoxes[36]);
            NUnit.Framework.Assert.IsNotNull(mergeFont.bBoxes[39]);
            NUnit.Framework.Assert.IsNotNull(mergeFont.bBoxes[59]);
        }

        [NUnit.Framework.Test]
        public virtual void NoCommonCmapPdfType0MergeTest() {
            // subsets are created using fonttools Python lib with the following command
            // fonttools subset ./NotoSans-Regular.ttf --text="ABC" --retain-gids --layout-features='*' --notdef-glyph --output-file=subset_abc.ttf
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_abc.ttf"));
            TrueTypeFont subsetAbc = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_def.ttf"));
            TrueTypeFont subsetDef = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_xyz.ttf"));
            TrueTypeFont subsetXyz = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            IDictionary<TrueTypeFont, ICollection<int>> toMerge = new Dictionary<TrueTypeFont, ICollection<int>>();
            ICollection<int> usedGlyphs = new HashSet<int>();
            // GID correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            toMerge.Put(subsetAbc, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to DEF
            usedGlyphs.Add(39);
            usedGlyphs.Add(40);
            usedGlyphs.Add(41);
            toMerge.Put(subsetDef, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to XYZ
            usedGlyphs.Add(59);
            usedGlyphs.Add(60);
            usedGlyphs.Add(61);
            toMerge.Put(subsetXyz, usedGlyphs);
            byte[] mergeFontBytes = TrueTypeFont.Merge(toMerge, "NotoSans-Regular", false);
            TrueTypeFont mergeFont = FontProgramFactory.CreateTrueTypeFont(mergeFontBytes, true);
            // `cmap` table doesn't contain mapping for all used glyphs, but for PDF
            // Type0 CIDFontType2 it isn't required because of CIDToGIDMap presence
            NUnit.Framework.Assert.AreEqual(3, mergeFont.GetActiveCmap().Count);
            // `glyf` table contains data for all used glyphs
            NUnit.Framework.Assert.AreEqual(62, mergeFont.bBoxes.Length);
            NUnit.Framework.Assert.IsNotNull(mergeFont.bBoxes[36]);
            NUnit.Framework.Assert.IsNotNull(mergeFont.bBoxes[39]);
            NUnit.Framework.Assert.IsNotNull(mergeFont.bBoxes[59]);
        }

        [NUnit.Framework.Test]
        public virtual void NoCommonCmapUnknownPdfTypeMergeTest() {
            // subsets are created using fonttools Python lib with the following command
            // fonttools subset ./NotoSans-Regular.ttf --text="ABC" --retain-gids --layout-features='*' --notdef-glyph --output-file=subset_abc.ttf
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_abc.ttf"));
            TrueTypeFont subsetAbc = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_def.ttf"));
            TrueTypeFont subsetDef = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subset_xyz.ttf"));
            TrueTypeFont subsetXyz = FontProgramFactory.CreateTrueTypeFont(fontBytes, true);
            IDictionary<TrueTypeFont, ICollection<int>> toMerge = new Dictionary<TrueTypeFont, ICollection<int>>();
            ICollection<int> usedGlyphs = new HashSet<int>();
            // GID correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            toMerge.Put(subsetAbc, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to DEF
            usedGlyphs.Add(39);
            usedGlyphs.Add(40);
            usedGlyphs.Add(41);
            toMerge.Put(subsetDef, usedGlyphs);
            usedGlyphs = new HashSet<int>();
            // GID correspond to XYZ
            usedGlyphs.Add(59);
            usedGlyphs.Add(60);
            usedGlyphs.Add(61);
            toMerge.Put(subsetXyz, usedGlyphs);
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TrueTypeFont.Merge
                (toMerge, "NotoSans-Regular"));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.CMAP_TABLE_MERGING_IS_NOT_SUPPORTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TryToReadFontSubsetWithoutGlyfTableTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetWithoutGlyfTable.ttf"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontProgramFactory
                .CreateTrueTypeFont(fontBytes, true));
            String exp = MessageFormatUtil.Format(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST, "glyf");
            NUnit.Framework.Assert.AreEqual(exp, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ReadFontSubsetWithoutOs2TableTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetWithoutOsTable.ttf"));
            NUnit.Framework.Assert.DoesNotThrow(() => FontProgramFactory.CreateTrueTypeFont(fontBytes, true));
        }

        [NUnit.Framework.Test]
        public virtual void TryToReadFontSubsetWithoutOs2TableTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetWithoutOsTable.ttf"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontProgramFactory
                .CreateTrueTypeFont(fontBytes, false));
            String exp = MessageFormatUtil.Format(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST, "os/2");
            NUnit.Framework.Assert.AreEqual(exp, e.Message);
        }
    }
}
