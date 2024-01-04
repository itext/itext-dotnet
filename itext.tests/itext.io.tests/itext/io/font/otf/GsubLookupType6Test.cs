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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GsubLookupType6Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GsubLookupType6Test/";

        [NUnit.Framework.Test]
        public virtual void TestSubstitutionApplied() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "Padauk-Regular.ttf"
                );
            GlyphSubstitutionTableReader gsubTableReader = fontProgram.GetGsubTable();
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(233), fontProgram.GetGlyphByCode(167
                ), fontProgram.GetGlyphByCode(207), fontProgram.GetGlyphByCode(149), fontProgram.GetGlyphByCode(207), 
                fontProgram.GetGlyphByCode(186), fontProgram.GetGlyphByCode(229), fontProgram.GetGlyphByCode(248));
            GlyphLine gl = new GlyphLine(glyphs);
            GsubLookupType6 lookup = (GsubLookupType6)gsubTableReader.GetLookupTable(57);
            NUnit.Framework.Assert.AreEqual(233, gl.Get(0).GetCode());
            NUnit.Framework.Assert.IsTrue(lookup.TransformLine(gl));
            NUnit.Framework.Assert.AreEqual(234, gl.Get(0).GetCode());
        }

        [NUnit.Framework.Test]
        public virtual void TestSubstitutionNotApplied() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "Padauk-Regular.ttf"
                );
            GlyphSubstitutionTableReader gsubTableReader = fontProgram.GetGsubTable();
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(233), fontProgram.GetGlyphByCode(167
                ), fontProgram.GetGlyphByCode(207), fontProgram.GetGlyphByCode(149), fontProgram.GetGlyphByCode(207), 
                fontProgram.GetGlyphByCode(186), fontProgram.GetGlyphByCode(229), fontProgram.GetGlyphByCode(248));
            GlyphLine gl = new GlyphLine(glyphs);
            GsubLookupType6 lookup = (GsubLookupType6)gsubTableReader.GetLookupTable(54);
            NUnit.Framework.Assert.AreEqual(233, gl.Get(0).GetCode());
            NUnit.Framework.Assert.IsFalse(lookup.TransformLine(gl));
            NUnit.Framework.Assert.AreEqual(233, gl.Get(0).GetCode());
        }
    }
}
