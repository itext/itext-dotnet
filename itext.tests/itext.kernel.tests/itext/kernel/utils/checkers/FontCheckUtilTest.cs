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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Kernel.Utils.Checkers {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCheckUtilTest : ExtendedITextTest {
        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        [NUnit.Framework.Test]
        public virtual void CheckFontAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            NUnit.Framework.Assert.AreEqual(-1, FontCheckUtil.CheckGlyphsOfText("123", font, new _CharacterChecker_47(
                )));
        }

        private sealed class _CharacterChecker_47 : FontCheckUtil.CharacterChecker {
            public _CharacterChecker_47() {
            }

            public bool Check(int ch, PdfFont fontToCheck) {
                return !fontToCheck.ContainsGlyph(ch);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckFontNotAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            NUnit.Framework.Assert.AreEqual(2, FontCheckUtil.CheckGlyphsOfText("hi⫊", font, new _CharacterChecker_59()
                ));
        }

        private sealed class _CharacterChecker_59 : FontCheckUtil.CharacterChecker {
            public _CharacterChecker_59() {
            }

            public bool Check(int ch, PdfFont fontToCheck) {
                return !fontToCheck.ContainsGlyph(ch);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckUnicodeMappingNotAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(FONTS_FOLDER + "cmr10.afm", FONTS_FOLDER
                 + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            int index = FontCheckUtil.CheckGlyphsOfText("h i", font, new _CharacterChecker_72());
            NUnit.Framework.Assert.AreEqual(1, index);
        }

        private sealed class _CharacterChecker_72 : FontCheckUtil.CharacterChecker {
            public _CharacterChecker_72() {
            }

            public bool Check(int ch, PdfFont fontToCheck) {
                if (fontToCheck.ContainsGlyph(ch)) {
                    return !fontToCheck.GetGlyph(ch).HasValidUnicode();
                }
                else {
                    return true;
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckUnicodeMappingAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(FONTS_FOLDER + "cmr10.afm", FONTS_FOLDER
                 + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            int index = FontCheckUtil.CheckGlyphsOfText("hi", font, new _CharacterChecker_90());
            NUnit.Framework.Assert.AreEqual(-1, index);
        }

        private sealed class _CharacterChecker_90 : FontCheckUtil.CharacterChecker {
            public _CharacterChecker_90() {
            }

            public bool Check(int ch, PdfFont fontToCheck) {
                if (fontToCheck.ContainsGlyph(ch)) {
                    return !fontToCheck.GetGlyph(ch).HasValidUnicode();
                }
                else {
                    return true;
                }
            }
        }
    }
}
