using System;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Layout.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer.Typography {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultTypographyApplierTest : ExtendedITextTest {
        public static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        private static DefaultTypographyApplier sut;

        private static TrueTypeFont freeSansBold;

        private static TrueTypeFont notoSansGujaratiRegular;

        private static FontProgram helvetica;

        private static TrueTypeFont puritanRegular;

        private static TrueTypeFont notoSansRegular;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            sut = new DefaultTypographyApplier();
            freeSansBold = (TrueTypeFont)FontProgramFactory.CreateFont(FONT_FOLDER + "FreeSansBold.ttf");
            notoSansGujaratiRegular = (TrueTypeFont)FontProgramFactory.CreateFont(FONT_FOLDER + "NotoSansGujarati-Regular.ttf"
                );
            notoSansRegular = (TrueTypeFont)FontProgramFactory.CreateFont(FONT_FOLDER + "NotoSans-Regular.ttf");
            puritanRegular = (TrueTypeFont)FontProgramFactory.CreateFont(FONT_FOLDER + "Puritan-Regular.otf");
            helvetica = FontProgramFactory.CreateFont();
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_INFO, Count = 1, LogLevel = LogLevelConstants.INFO
            )]
        public virtual void TestApplyOtfShouldIssueInfoForAffectingFeatures() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansRegular.GetGlyph(84), notoSansRegular.GetGlyph
                (101), notoSansRegular.GetGlyph(115), notoSansRegular.GetGlyph(116)));
            SequenceId id = new SequenceId();
            NUnit.Framework.Assert.DoesNotThrow(() => sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN
                , null, id, null));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_INFO, Count = 1, LogLevel = LogLevelConstants.INFO
            )]
        public virtual void TestApplyOtfShouldIssueInfoOncePerDocument() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansRegular.GetGlyph(84), notoSansRegular.GetGlyph
                (101), notoSansRegular.GetGlyph(115), notoSansRegular.GetGlyph(116)));
            SequenceId id = new SequenceId();
            NUnit.Framework.Assert.DoesNotThrow(() => {
                sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN, null, id, null);
                sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN, null, id, null);
                sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN, null, id, null);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_INFO, Count = 3, LogLevel = LogLevelConstants.INFO
            )]
        public virtual void TestApplyOtfShouldIssueInfoOnceForEachDocument() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansRegular.GetGlyph(84), notoSansRegular.GetGlyph
                (101), notoSansRegular.GetGlyph(115), notoSansRegular.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN, null, new SequenceId(), null);
                sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN, null, new SequenceId(), null);
                sut.ApplyOtfScript(notoSansRegular, glyphLine, UnicodeScript.LATIN, null, new SequenceId(), null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void IsPdfCalligraphInstance() {
            NUnit.Framework.Assert.IsFalse(sut.IsPdfCalligraphInstance());
        }

        [NUnit.Framework.Test]
        public virtual void ApplyOtfShouldNotIssueWarningForCyrillicScript() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(freeSansBold.GetGlyph(84), freeSansBold.GetGlyph
                (101), freeSansBold.GetGlyph(115), freeSansBold.GetGlyph(116)));
            SequenceId id = new SequenceId();
            NUnit.Framework.Assert.DoesNotThrow(() => sut.ApplyOtfScript(freeSansBold, glyphLine, UnicodeScript.CYRILLIC
                , null, id, null));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 1, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void ApplyOtfShouldIssueWarningForGujaratiAndSupportingfontScript() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansGujaratiRegular.GetGlyph(84), notoSansGujaratiRegular
                .GetGlyph(101), notoSansGujaratiRegular.GetGlyph(115), notoSansGujaratiRegular.GetGlyph(116)));
            SequenceId id = new SequenceId();
            NUnit.Framework.Assert.DoesNotThrow(() => sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript
                .GUJARATI, null, id, null));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 1, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void ApplyOtfShouldIssueWarningOncePerDocument() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansGujaratiRegular.GetGlyph(84), notoSansGujaratiRegular
                .GetGlyph(101), notoSansGujaratiRegular.GetGlyph(115), notoSansGujaratiRegular.GetGlyph(116)));
            SequenceId id = new SequenceId();
            NUnit.Framework.Assert.DoesNotThrow(() => {
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 3, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void ApplyOtfShouldIssueWarningForEachDocument() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansGujaratiRegular.GetGlyph(84), notoSansGujaratiRegular
                .GetGlyph(101), notoSansGujaratiRegular.GetGlyph(115), notoSansGujaratiRegular.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                SequenceId id = new SequenceId();
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
                id = new SequenceId();
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
                id = new SequenceId();
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 3, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void ApplyOtfShouldIssueWarningForEachScript() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(notoSansGujaratiRegular.GetGlyph(84), notoSansGujaratiRegular
                .GetGlyph(101), notoSansGujaratiRegular.GetGlyph(115), notoSansGujaratiRegular.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                SequenceId id = new SequenceId();
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.GUJARATI, null, id, null);
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.THAI, null, id, null);
                sut.ApplyOtfScript(notoSansGujaratiRegular, glyphLine, UnicodeScript.HEBREW, null, id, null);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 1)]
        public virtual void ApplyKerningShouldIssueWarningIfFontSupport() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(helvetica.GetGlyph(84), helvetica.GetGlyph(101), 
                helvetica.GetGlyph(115), helvetica.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => sut.ApplyKerning(helvetica, glyphLine, new SequenceId(), null));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 3, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void ApplyKerningShouldIssueWarningPerDocument() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(helvetica.GetGlyph(84), helvetica.GetGlyph(101), 
                helvetica.GetGlyph(115), helvetica.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                sut.ApplyKerning(helvetica, glyphLine, new SequenceId(), null);
                sut.ApplyKerning(helvetica, glyphLine, new SequenceId(), null);
                sut.ApplyKerning(helvetica, glyphLine, new SequenceId(), null);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, Count = 1, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void ApplyKerningShouldLogOncePerDocument() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(helvetica.GetGlyph(84), helvetica.GetGlyph(101), 
                helvetica.GetGlyph(115), helvetica.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                SequenceId id = new SequenceId();
                sut.ApplyKerning(helvetica, glyphLine, id, null);
                sut.ApplyKerning(helvetica, glyphLine, id, null);
                sut.ApplyKerning(helvetica, glyphLine, id, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ApplyKerningShouldNotIssueWarningIfNoFontSupport() {
            GlyphLine glyphLine = new GlyphLine(JavaUtil.ArraysAsList(puritanRegular.GetGlyph(84), puritanRegular.GetGlyph
                (101), puritanRegular.GetGlyph(115), puritanRegular.GetGlyph(116)));
            NUnit.Framework.Assert.DoesNotThrow(() => sut.ApplyKerning(puritanRegular, glyphLine, new SequenceId(), null
                ));
        }
    }
}
