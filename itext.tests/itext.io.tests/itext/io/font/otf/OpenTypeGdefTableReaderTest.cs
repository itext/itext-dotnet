using System;
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
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
