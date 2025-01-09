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
using iText.IO.Font.Otf;

namespace iText.Layout.Splitting {
    /// <summary>
    /// The implementation of
    /// <see cref="ISplitCharacters"/>
    /// that prevents breaking within words.
    /// </summary>
    public class KeepAllSplitCharacters : ISplitCharacters {
        public virtual bool IsSplitCharacter(GlyphLine text, int glyphPos) {
            if (!text.Get(glyphPos).HasValidUnicode()) {
                return false;
            }
            int charCode = text.Get(glyphPos).GetUnicode();
            //Check if a hyphen proceeds a digit to denote negative value
            // TODO: DEVSIX-4863 why is glyphPos == 0? negative value could be preceded by a whitespace!
            if ((glyphPos == 0) && (charCode == '-') && (text.Size() - 1 > glyphPos) && (IsADigitChar(text, glyphPos +
                 1))) {
                return false;
            }
            return charCode <= ' ' || charCode == '-' || charCode == '\u2010' || 
                        // block of whitespaces
                        (charCode >= 0x2002 && charCode <= 0x200b);
        }

        private static bool IsADigitChar(GlyphLine text, int glyphPos) {
            return char.IsDigit(text.Get(glyphPos).GetChars()[0]);
        }
    }
}
