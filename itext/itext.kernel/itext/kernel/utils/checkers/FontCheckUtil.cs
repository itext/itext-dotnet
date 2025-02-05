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
using iText.Kernel.Font;

namespace iText.Kernel.Utils.Checkers {
    /// <summary>Utility class that contains common checks used in both the  PDFA and PDFUA module for fonts.</summary>
    public sealed class FontCheckUtil {
        private FontCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks the text by the passed checker and the font.</summary>
        /// <param name="text">the text to check</param>
        /// <param name="font">the font to check</param>
        /// <param name="checker">the checker which checks the text according to the font</param>
        /// <returns>
        /// 
        /// <c>-1</c>
        /// if no character passes the check, or index of the first symbol which passes the check
        /// </returns>
        public static int CheckGlyphsOfText(String text, PdfFont font, FontCheckUtil.CharacterChecker checker) {
            for (int i = 0; i < text.Length; ++i) {
                int ch;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, i)) {
                    ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, i);
                    i++;
                }
                else {
                    ch = text[i];
                }
                if (checker.Check(ch, font)) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>Character checker which performs check of passed symbol against the font.</summary>
        public interface CharacterChecker {
            /// <summary>Checks passed symbol against the font</summary>
            /// <param name="ch">character to check</param>
            /// <param name="font">font to check</param>
            /// <returns>
            /// 
            /// <see langword="true"/>
            /// if check passes, otherwise
            /// <see langword="false"/>
            /// </returns>
            bool Check(int ch, PdfFont font);
        }
    }
}
