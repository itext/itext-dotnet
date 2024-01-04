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
    public class GposLookupType1Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GposLookupType1Test/";

        [NUnit.Framework.Test]
        public virtual void VerifyXAdvanceIsAppliedSubFormat1() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType1 lookup = (GposLookupType1)gposTableReader.GetLookupTable(29);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(fontProgram.GetGlyphByCode(174)), new Glyph(fontProgram
                .GetGlyphByCode(5)));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.idx = 0;
            NUnit.Framework.Assert.AreEqual(0, gl.Get(0).GetXAdvance());
            NUnit.Framework.Assert.IsTrue(lookup.TransformOne(gl));
            NUnit.Framework.Assert.AreEqual(219, gl.Get(0).GetXAdvance());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPositionIsNotAppliedForIrrelevantGlyphSubFormat1() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType1 lookup = (GposLookupType1)gposTableReader.GetLookupTable(29);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(fontProgram.GetGlyphByCode(5)), new Glyph(fontProgram
                .GetGlyphByCode(174)));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.idx = 0;
            NUnit.Framework.Assert.AreEqual(0, gl.Get(0).GetXAdvance());
            NUnit.Framework.Assert.IsFalse(lookup.TransformOne(gl));
            NUnit.Framework.Assert.AreEqual(0, gl.Get(0).GetXAdvance());
        }

        [NUnit.Framework.Test]
        public virtual void VerifyDifferentXAdvanceIsAppliedSubFormat2() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType1 lookup = (GposLookupType1)gposTableReader.GetLookupTable(16);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(fontProgram.GetGlyphByCode(401)), new Glyph(fontProgram
                .GetGlyphByCode(5)));
            GlyphLine gl = new GlyphLine(glyphs);
            NUnit.Framework.Assert.AreEqual(0, gl.Get(0).GetXAdvance());
            NUnit.Framework.Assert.IsTrue(lookup.TransformOne(gl));
            NUnit.Framework.Assert.AreEqual(109, gl.Get(0).GetXAdvance());
            // Subtable type 2 defines different GposValueRecords for different coverage glyphs
            glyphs = JavaUtil.ArraysAsList(new Glyph(fontProgram.GetGlyphByCode(508)), new Glyph(fontProgram.GetGlyphByCode
                (5)));
            gl = new GlyphLine(glyphs);
            NUnit.Framework.Assert.AreEqual(0, gl.Get(0).GetXAdvance());
            NUnit.Framework.Assert.IsTrue(lookup.TransformOne(gl));
            NUnit.Framework.Assert.AreEqual(158, gl.Get(0).GetXAdvance());
        }
    }
}
