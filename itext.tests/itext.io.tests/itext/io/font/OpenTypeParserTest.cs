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
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OpenTypeParserTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/OpenTypeParserTest/";

        private static readonly String FREESANS_FONT_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf";

        [NUnit.Framework.Test]
        public virtual void TryToReadFontSubsetWithoutGlyfTableTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetWithoutGlyfTable.ttf"));
            OpenTypeParser parser = new OpenTypeParser(fontBytes);
            parser.LoadTables(true);
            ICollection<int> usedGlyphs = new HashSet<int>();
            // these GIDs correspond to ABC
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => parser.GetSubset
                (usedGlyphs, true));
            String exp = MessageFormatUtil.Format(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN, "glyf", null);
            NUnit.Framework.Assert.AreEqual(exp, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetFlatGlyphsCompositeTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(FREESANS_FONT_PATH));
            OpenTypeParser parser = new OpenTypeParser(fontBytes);
            parser.LoadTables(true);
            ICollection<int> usedGlyphs = new HashSet<int>();
            // Å
            usedGlyphs.Add(137);
            IList<int> glyphs = parser.GetFlatGlyphs(usedGlyphs);
            NUnit.Framework.Assert.AreEqual(4, glyphs.Count);
            NUnit.Framework.Assert.AreEqual(137, glyphs[0]);
            NUnit.Framework.Assert.AreEqual(0, glyphs[1]);
            NUnit.Framework.Assert.AreEqual(586, glyphs[2]);
            NUnit.Framework.Assert.AreEqual(38, glyphs[3]);
        }

        [NUnit.Framework.Test]
        public virtual void SmallNumberOfMetricsTest() {
            OpenTypeParser parser = new OpenTypeParser(SOURCE_FOLDER + "NotoSansAndSpaceMono.ttc", 1);
            parser.LoadTables(true);
            ICollection<int> usedGlyphs = new HashSet<int>();
            usedGlyphs.Add(36);
            usedGlyphs.Add(37);
            usedGlyphs.Add(38);
            Tuple2<int, byte[]> subsetData = parser.GetSubset(usedGlyphs, true);
            OpenTypeParser resParser = new OpenTypeParser(subsetData.GetSecond(), true);
            resParser.LoadTables(true);
            // 86 == <number of h metrics> * 4 + (<number of glyphs> - <number of h metrics>) * 2
            // where <number of h metrics> = 4 and <number of glyphs> = 39
            NUnit.Framework.Assert.AreEqual(86, resParser.tables.Get("hmtx")[1]);
        }
    }
}
