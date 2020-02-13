using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Font.Otf {
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
            gl.idx = 1;
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
            glyphLine.idx = 1;
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
            glyphLine.idx = 1;
            lookup.TransformOne(glyphLine);
            NUnit.Framework.Assert.AreEqual(2, glyphLine.Size());
            NUnit.Framework.Assert.AreEqual(1490, glyphLine.Get(0).GetCode());
            NUnit.Framework.Assert.AreEqual(75, glyphLine.Get(1).GetCode());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.Get(1).GetAnchorDelta());
            NUnit.Framework.Assert.AreEqual(0, glyphLine.Get(1).GetXPlacement());
        }
    }
}
