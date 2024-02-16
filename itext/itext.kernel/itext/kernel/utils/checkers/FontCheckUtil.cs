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
