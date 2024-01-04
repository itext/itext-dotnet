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
using iText.IO.Font.Otf;

namespace iText.Layout.Splitting {
    /// <summary>
    /// The implementation of
    /// <see cref="ISplitCharacters"/>
    /// that allows breaking within words.
    /// </summary>
    public class BreakAllSplitCharacters : ISplitCharacters {
        public virtual bool IsSplitCharacter(GlyphLine text, int glyphPos) {
            if (text.Size() - 1 == glyphPos) {
                return true;
            }
            Glyph glyphToCheck = text.Get(glyphPos);
            if (!glyphToCheck.HasValidUnicode()) {
                return true;
            }
            int charCode = glyphToCheck.GetUnicode();
            Glyph nextGlyph = text.Get(glyphPos + 1);
            if (!nextGlyph.HasValidUnicode()) {
                return true;
            }
            bool nextGlyphIsLetterOrDigit = iText.IO.Util.TextUtil.IsLetterOrDigit(nextGlyph);
            bool nextGlyphIsMark = iText.IO.Util.TextUtil.IsMark(nextGlyph);
            bool currentGlyphIsDefaultSplitCharacter = charCode <= ' ' || charCode == '-' || charCode == '\u2010' || 
                        // block of whitespaces
                        (charCode >= 0x2002 && charCode <= 0x200b);
            return (currentGlyphIsDefaultSplitCharacter || nextGlyphIsLetterOrDigit || nextGlyphIsMark) && !iText.IO.Util.TextUtil
                .IsNonBreakingHyphen(glyphToCheck);
        }
    }
}
