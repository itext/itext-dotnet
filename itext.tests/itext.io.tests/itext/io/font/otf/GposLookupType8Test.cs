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
    public class GposLookupType8Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GposLookupType8Test/";

        [NUnit.Framework.Test]
        public virtual void VerifyXAdvanceIsAppliedForContextualPositioning() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "Padauk-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(233), fontProgram.GetGlyphByCode(163
                ), fontProgram.GetGlyphByCode(158), fontProgram.GetGlyphByCode(227));
            GlyphLine gl = new GlyphLine(glyphs);
            GposLookupType8 lookup = (GposLookupType8)gposTableReader.GetLookupTable(92);
            NUnit.Framework.Assert.AreEqual(0, gl.Get(2).GetXAdvance());
            NUnit.Framework.Assert.IsTrue(lookup.TransformLine(gl));
            NUnit.Framework.Assert.AreEqual(28, gl.Get(2).GetXAdvance());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyXAdvanceIsAppliedForPosTableLookup8Format2() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(29), fontProgram.GetGlyphByCode(26)
                , fontProgram.GetGlyphByCode(431), fontProgram.GetGlyphByCode(415), fontProgram.GetGlyphByCode(199), fontProgram
                .GetGlyphByCode(26), fontProgram.GetGlyphByCode(407), fontProgram.GetGlyphByCode(210), fontProgram.GetGlyphByCode
                (417));
            GlyphLine gl = new GlyphLine(glyphs);
            GposLookupType8 lookup = (GposLookupType8)gposTableReader.GetLookupTable(0);
            NUnit.Framework.Assert.AreEqual(0, gl.Get(1).GetXAdvance());
            NUnit.Framework.Assert.IsTrue(lookup.TransformLine(gl));
            NUnit.Framework.Assert.AreEqual(134, gl.Get(1).GetXAdvance());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyXAdvanceIsNotAppliedForUnsatisfiedContextualPositioning() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "Padauk-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(233), fontProgram.GetGlyphByCode(163
                ), fontProgram.GetGlyphByCode(158), fontProgram.GetGlyphByCode(233));
            GlyphLine gl = new GlyphLine(glyphs);
            GposLookupType8 lookup = (GposLookupType8)gposTableReader.GetLookupTable(92);
            NUnit.Framework.Assert.IsFalse(lookup.TransformLine(gl));
            for (int i = 0; i < gl.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(0, gl.Get(i).GetXAdvance());
                NUnit.Framework.Assert.AreEqual(0, gl.Get(i).GetYAdvance());
            }
        }
    }
}
