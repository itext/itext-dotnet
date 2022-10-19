/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
    [NUnit.Framework.Category("UnitTest")]
    public class OpenTypeFontTableReaderTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/OpenTypeFontTableReaderTest/";

        [NUnit.Framework.Test]
        public virtual void TestFetchLangSysByTag() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphSubstitutionTableReader gsub = fontProgram.GetGsubTable();
            ScriptRecord mym2 = gsub.GetScriptRecords()[0];
            NUnit.Framework.Assert.AreEqual("mym2", mym2.tag);
            // default LangSys has no tag
            NUnit.Framework.Assert.AreEqual("", gsub.GetLanguageRecord("mym2").tag);
            NUnit.Framework.Assert.AreEqual(LanguageTags.SGAW_KAREN, gsub.GetLanguageRecord("mym2", LanguageTags.SGAW_KAREN
                ).tag);
            NUnit.Framework.Assert.AreEqual(LanguageTags.MON, gsub.GetLanguageRecord("mym2", LanguageTags.MON).tag);
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord(null));
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord("mym3"));
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord("mym3", LanguageTags.SGAW_KAREN));
        }
    }
}
