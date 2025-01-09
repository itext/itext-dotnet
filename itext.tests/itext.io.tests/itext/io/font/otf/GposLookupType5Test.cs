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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GposLookupType5Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GposLookupType5Test/";

        [NUnit.Framework.Test]
        public virtual void VerifyMarkToBaseAttachment() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "KhmerOS.ttf");
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType5 lookup = (GposLookupType5)gposTableReader.GetLookupTable(0);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(fontProgram.GetGlyphByCode(445)), new Glyph(fontProgram
                .GetGlyphByCode(394)));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.SetIdx(1);
            lookup.TransformOne(gl);
            NUnit.Framework.Assert.AreEqual(2, gl.Size());
            NUnit.Framework.Assert.AreEqual(445, gl.Get(0).GetCode());
            NUnit.Framework.Assert.AreEqual(394, gl.Get(1).GetCode());
            NUnit.Framework.Assert.AreEqual(-1, gl.Get(1).GetAnchorDelta());
            NUnit.Framework.Assert.AreEqual(756, gl.Get(1).GetXPlacement());
        }

        [NUnit.Framework.Test]
        public virtual void TestSelectingCorrectAttachmentAlternative() {
            // TODO on completion of DEVSIX-3732 this test will probably have to be refactored
            //  since we will have to emulate previous substitutions and populate the substitution info
            //  to the glyph line so that mark is attached to the correct component of a ligature
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoNaskhArabic-Regular.ttf"
                );
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(fontProgram.GetGlyphByCode(513), fontProgram.GetGlyphByCode
                (75)));
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType5 lookup = (GposLookupType5)gposTableReader.GetLookupTable(3);
            glyphLine.SetIdx(1);
            lookup.TransformOne(glyphLine);
            NUnit.Framework.Assert.AreEqual(2, glyphLine.Size());
            NUnit.Framework.Assert.AreEqual(513, glyphLine.Get(0).GetCode());
            NUnit.Framework.Assert.AreEqual(75, glyphLine.Get(1).GetCode());
            NUnit.Framework.Assert.AreEqual(-1, glyphLine.Get(1).GetAnchorDelta());
            NUnit.Framework.Assert.AreEqual(-22, glyphLine.Get(1).GetXPlacement());
        }

        [NUnit.Framework.Test]
        public virtual void TestThatNoTransformationsAppliedForNonRelevantGlyphs() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoNaskhArabic-Regular.ttf"
                );
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(fontProgram.GetGlyph('1'), fontProgram.GetGlyphByCode
                (75)));
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType5 lookup = (GposLookupType5)gposTableReader.GetLookupTable(3);
            glyphLine.SetIdx(1);
            lookup.TransformOne(glyphLine);
            NUnit.Framework.Assert.AreEqual(2, glyphLine.Size());
            NUnit.Framework.Assert.AreEqual(1490, glyphLine.Get(0).GetCode());
            NUnit.Framework.Assert.AreEqual(75, glyphLine.Get(1).GetCode());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.Get(1).GetAnchorDelta());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.Get(1).GetXPlacement());
        }

        [NUnit.Framework.Test]
        public virtual void IdxBiggerThanLineEndTest() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "NotoNaskhArabic-Regular.ttf"
                );
            GlyphLine glyphLine = new GlyphLine(JavaCollectionsUtil.SingletonList(fontProgram.GetGlyph(203)));
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType5 lookup = (GposLookupType5)gposTableReader.GetLookupTable(3);
            glyphLine.SetIdx(10);
            NUnit.Framework.Assert.IsFalse(lookup.TransformOne(glyphLine));
        }
    }
}
