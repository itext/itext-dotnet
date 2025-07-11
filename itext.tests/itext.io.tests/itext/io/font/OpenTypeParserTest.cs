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
    public class OpenTypeParserTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/OpenTypeParserTest/";

        [NUnit.Framework.Test]
        public virtual void TryToReadFontSubsetWithoutGlyfTableTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "subsetWithoutGlyfTable.ttf"));
            OpenTypeParser parser = new OpenTypeParser(fontBytes);
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
    }
}
