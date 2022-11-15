using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GsubLookupType4Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GsubLookupType4Test/";

        [NUnit.Framework.Test]
        public virtual void TestNoIndexOutOfBound() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "DejaVuSansMono.ttf"
                );
            GlyphSubstitutionTableReader gsubTableReader = fontProgram.GetGsubTable();
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(1, 1, 1), new Glyph(1, 1, 1), new Glyph(1, 1, 1), new 
                Glyph(1, 1, 1), new Glyph(1, 1, 1), new Glyph(1, 1, 1));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.idx = gl.end;
            GsubLookupType4 lookup = (GsubLookupType4)gsubTableReader.GetLookupTable(6);
            //Assert that no exception is thrown if gl.idx = gl.end
            NUnit.Framework.Assert.IsFalse(lookup.TransformOne(gl));
        }
    }
}
