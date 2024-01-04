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
    public class GposLookupType7Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GposLookupType7Test/";

        [NUnit.Framework.Test]
        public virtual void VerifyXAdvanceIsAppliedForContextualPositioning() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType7 lookup = (GposLookupType7)gposTableReader.GetLookupTable(28);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(25), fontProgram.GetGlyphByCode(174
                ), fontProgram.GetGlyphByCode(5), fontProgram.GetGlyphByCode(411));
            GlyphLine gl = new GlyphLine(glyphs);
            NUnit.Framework.Assert.AreEqual(0, gl.Get(1).GetXAdvance());
            NUnit.Framework.Assert.IsTrue(lookup.TransformLine(gl));
            NUnit.Framework.Assert.AreEqual(219, gl.Get(1).GetXAdvance());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyXAdvanceIsNotAppliedForUnsatisfiedContextualPositioning() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType7 lookup = (GposLookupType7)gposTableReader.GetLookupTable(28);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(1), fontProgram.GetGlyphByCode(174)
                , fontProgram.GetGlyphByCode(5), fontProgram.GetGlyphByCode(411));
            GlyphLine gl = new GlyphLine(glyphs);
            NUnit.Framework.Assert.IsFalse(lookup.TransformLine(gl));
            for (int i = 0; i < gl.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(0, gl.Get(i).GetXAdvance());
                NUnit.Framework.Assert.AreEqual(0, gl.Get(i).GetYAdvance());
            }
        }
    }
}
