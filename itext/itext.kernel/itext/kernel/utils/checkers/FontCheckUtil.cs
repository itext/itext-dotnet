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
using iText.Kernel.Font;

namespace iText.Kernel.Utils.Checkers {
    /// <summary>Utility class that contains common checks used in both the  PDFA and PDFUA module for fonts.</summary>
    public sealed class FontCheckUtil {
        private FontCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if all characters in the string contain a valid glyph in the font.</summary>
        /// <param name="text">The string we want to compare.</param>
        /// <param name="font">The font we want to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if all glyphs in the string are available in the font.
        /// </returns>
        public static bool DoesFontContainAllUsedGlyphs(String text, PdfFont font) {
            for (int i = 0; i < text.Length; ++i) {
                int ch;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, i)) {
                    ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, i);
                    i++;
                }
                else {
                    ch = text[i];
                }
                if (!font.ContainsGlyph(ch)) {
                    return false;
                }
            }
            return true;
        }
    }
}
