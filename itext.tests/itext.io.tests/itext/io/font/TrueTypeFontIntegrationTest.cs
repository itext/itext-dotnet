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
            byte[] mergeFontBytes = TrueTypeFont.Merge(toMerge, "NotoSans-Regular");
            TrueTypeFont mergeFont = FontProgramFactory.CreateTrueTypeFont(mergeFontBytes, true);
            // C glyphs wasn't used, it's why it was cut from merge font
            NUnit.Framework.Assert.IsNotNull(subset1.bBoxes[38]);
            NUnit.Framework.Assert.IsNotNull(subset2.bBoxes[38]);
            NUnit.Framework.Assert.AreEqual(38, mergeFont.bBoxes.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TryToReadFontSubsetWithoutGlyfTableTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetWithoutGlyfTable.ttf"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontProgramFactory
                .CreateTrueTypeFont(fontBytes, true));
            String exp = MessageFormatUtil.Format(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST, "glyf");
            NUnit.Framework.Assert.AreEqual(exp, e.Message);
        }
    }
}
