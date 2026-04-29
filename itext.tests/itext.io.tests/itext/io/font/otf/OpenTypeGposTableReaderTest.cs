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
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OpenTypeGposTableReaderTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/";

        [NUnit.Framework.Test]
        public virtual void NullLookupsInGposTableTest() {
            String fontName = "NotoSansSinhala-Regular.ttf";
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + fontName);
            GlyphPositioningTableReader gpos = fontProgram.GetGposTable();
            NUnit.Framework.Assert.AreEqual(3, gpos.lookupList.Count);
            NUnit.Framework.Assert.IsNotNull(gpos.lookupList[0]);
            NUnit.Framework.Assert.AreEqual(0, gpos.lookupList[0].GetIndexInLookupList());
            NUnit.Framework.Assert.IsNull(gpos.lookupList[1]);
            NUnit.Framework.Assert.IsNotNull(gpos.lookupList[2]);
            NUnit.Framework.Assert.AreEqual(2, gpos.lookupList[2].GetIndexInLookupList());
        }
    }
}
