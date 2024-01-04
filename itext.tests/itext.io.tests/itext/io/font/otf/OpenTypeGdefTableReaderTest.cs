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
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OpenTypeGdefTableReaderTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/OpenTypeGdefTableReaderTest/";

        [NUnit.Framework.Test]
        public virtual void TestLookupFlagWithMarkAttachmentTypeAndMarkGlyphWithoutMarkAttachmentClass() {
            String fontName = "Padauk-Regular.ttf";
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + fontName);
            OpenTypeGdefTableReader gdef = fontProgram.GetGdefTable();
            int glyphCode = 207;
            NUnit.Framework.Assert.AreEqual(OtfClass.GLYPH_MARK, gdef.GetGlyphClassTable().GetOtfClass(glyphCode));
            NUnit.Framework.Assert.IsTrue(gdef.IsSkip(glyphCode, (1 << 8) | OpenTypeGdefTableReader.FLAG_IGNORE_BASE));
        }

        [NUnit.Framework.Test]
        public virtual void TestLookupFlagWithMarkAttachmentTypeAndMarkGlyphWithSameMarkAttachmentClass() {
            String fontName = "Padauk-Regular.ttf";
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + fontName);
            OpenTypeGdefTableReader gdef = fontProgram.GetGdefTable();
            int glyphCode = 151;
            NUnit.Framework.Assert.AreEqual(OtfClass.GLYPH_MARK, gdef.GetGlyphClassTable().GetOtfClass(glyphCode));
            NUnit.Framework.Assert.IsFalse(gdef.IsSkip(glyphCode, (1 << 8) | OpenTypeGdefTableReader.FLAG_IGNORE_BASE)
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestLookupFlagWithMarkAttachmentTypeAndBaseGlyph() {
            String fontName = "Padauk-Regular.ttf";
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + fontName);
            OpenTypeGdefTableReader gdef = fontProgram.GetGdefTable();
            int glyphCode = 165;
            NUnit.Framework.Assert.AreEqual(OtfClass.GLYPH_BASE, gdef.GetGlyphClassTable().GetOtfClass(glyphCode));
            NUnit.Framework.Assert.IsFalse(gdef.IsSkip(glyphCode, (1 << 8)));
        }
    }
}
