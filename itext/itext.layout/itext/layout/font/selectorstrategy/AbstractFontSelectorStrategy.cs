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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Layout.Font;
using iText.Layout.Renderer;

namespace iText.Layout.Font.Selectorstrategy {
    /// <summary>
    /// The class defines complex implementation of
    /// <see cref="IFontSelectorStrategy"/>
    /// which based on the following algorithm:
    /// 1.
    /// </summary>
    /// <remarks>
    /// The class defines complex implementation of
    /// <see cref="IFontSelectorStrategy"/>
    /// which based on the following algorithm:
    /// 1. Find first significant symbol (not whitespace or special).
    /// 2. Find font which matches symbol according to passed
    /// <see cref="iText.Layout.Font.FontSelector"/>.
    /// 3. Try to append as many symbols as possible using the current font.
    /// 4. If symbol is not matched to the current font, go to step 1.
    /// <para />
    /// Algorithm takes care of the case when there is no matched font for symbol or when diacritic
    /// from another font is used (previous symbol will be processed by diacritic's font).
    /// </remarks>
    public abstract class AbstractFontSelectorStrategy : IFontSelectorStrategy {
        private readonly FontProvider fontProvider;

        private readonly FontSet additionalFonts;

        private readonly FontSelector fontSelector;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="AbstractFontSelectorStrategy"/>.
        /// </summary>
        /// <param name="fontProvider">the font provider</param>
        /// <param name="fontSelector">the font selector</param>
        /// <param name="additionalFonts">the set of fonts to be used additionally to the fonts added to font provider.
        ///     </param>
        public AbstractFontSelectorStrategy(FontProvider fontProvider, FontSelector fontSelector, FontSet additionalFonts
            ) {
            this.fontProvider = fontProvider;
            this.additionalFonts = additionalFonts;
            this.fontSelector = fontSelector;
        }

        /// <summary>If it is necessary to provide a check that the best font for passed symbol equals to the current font.
        ///     </summary>
        /// <remarks>
        /// If it is necessary to provide a check that the best font for passed symbol equals to the current font.
        /// Result of checking is used to split text into parts in case if inequality.
        /// </remarks>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if check is needed, otherwise
        /// <see langword="false"/>
        /// </returns>
        protected internal abstract bool IsCurrentFontCheckRequired();

        /// <summary><inheritDoc/></summary>
        public virtual IList<Tuple2<GlyphLine, PdfFont>> GetGlyphLines(String text) {
            IList<Tuple2<GlyphLine, PdfFont>> result = new List<Tuple2<GlyphLine, PdfFont>>();
            int index = 0;
            int indexDiacritic = -1;
            while (index < text.Length) {
                // Find the best font for first significant symbol
                PdfFont currentFont = null;
                int indexSignificant = NextSignificantIndex(index, text);
                if (indexSignificant < text.Length) {
                    int codePoint = ExtractCodePoint(text, indexDiacritic == -1 ? indexSignificant : indexDiacritic);
                    currentFont = MatchFont(codePoint, fontSelector, fontProvider, additionalFonts);
                }
                IList<Glyph> resolvedGlyphs = new List<Glyph>();
                // Try to append as many symbols as possible to the current font
                if (currentFont != null) {
                    UnicodeScript? firstScript = null;
                    int to = indexSignificant;
                    bool breakRequested = false;
                    for (int i = indexSignificant; i < text.Length; i++) {
                        int codePoint = ExtractCodePoint(text, i);
                        if (codePoint > 0xFFFF) {
                            i++;
                        }
                        if (IsCurrentFontCheckRequired() && (i != indexDiacritic - 1) && !iText.IO.Util.TextUtil.IsWhitespaceOrNonPrintable
                            (codePoint)) {
                            if (currentFont != MatchFont(codePoint, fontSelector, fontProvider, additionalFonts)) {
                                breakRequested = true;
                            }
                        }
                        if (i > indexDiacritic) {
                            if (iText.IO.Util.TextUtil.IsDiacritic(codePoint)) {
                                PdfFont diacriticFont = MatchFont(codePoint, fontSelector, fontProvider, additionalFonts);
                                // Diacritic font must contain previous symbol, if not, don't
                                // enable special logic for diacritic and process it as usual symbol
                                bool isPreviousMatchFont = i == 0 || diacriticFont == null || diacriticFont.ContainsGlyph(ExtractCodePoint
                                    (text, i - 1));
                                // If diacritic font equals to the current font or null, don't
                                // enable special logic for diacritic and process it as usual symbol
                                if (diacriticFont != null && diacriticFont != currentFont && isPreviousMatchFont) {
                                    // If it's the first diacritic in a row, we want to break to try to find a better font for
                                    // the previous letter during the next iteration
                                    if (indexDiacritic != i - 1) {
                                        breakRequested = true;
                                    }
                                    indexDiacritic = i;
                                    if (breakRequested) {
                                        to = i - 2;
                                    }
                                }
                            }
                            else {
                                indexDiacritic = -1;
                            }
                        }
                        UnicodeScript? currScript = UnicodeScriptUtil.Of(codePoint);
                        if (IsSignificantUnicodeScript(currScript)) {
                            if (firstScript == null) {
                                firstScript = currScript;
                            }
                            else {
                                if (firstScript != currScript) {
                                    breakRequested = true;
                                }
                            }
                        }
                        if (breakRequested) {
                            break;
                        }
                        to = i;
                    }
                    if (to < index) {
                        continue;
                    }
                    int numOfAppendedGlyphs = currentFont.AppendGlyphs(text, index, to, resolvedGlyphs);
                    index += numOfAppendedGlyphs;
                }
                // If no symbols were appended, try to append any symbols
                if (resolvedGlyphs.IsEmpty()) {
                    currentFont = GetPdfFont(fontSelector.BestMatch(), fontProvider, additionalFonts);
                    if (index != indexSignificant) {
                        index += currentFont.AppendGlyphs(text, index, indexSignificant - 1, resolvedGlyphs);
                    }
                    while (index <= indexSignificant && index < text.Length) {
                        index += currentFont.AppendAnyGlyph(text, index, resolvedGlyphs);
                    }
                }
                GlyphLine tempGlyphLine = new GlyphLine(resolvedGlyphs);
                GlyphLine finalGlyphLine = TextPreprocessingUtil.ReplaceSpecialWhitespaceGlyphs(tempGlyphLine, currentFont
                    );
                result.Add(new Tuple2<GlyphLine, PdfFont>(finalGlyphLine, currentFont));
            }
            return result;
        }

        /// <summary>Finds the best font which matches passed symbol.</summary>
        /// <param name="codePoint">the symbol to match</param>
        /// <param name="fontSelector">the font selector</param>
        /// <param name="fontProvider">the font provider</param>
        /// <param name="additionalFonts">the addition fonts</param>
        /// <returns>font which matches the symbol</returns>
        protected internal virtual PdfFont MatchFont(int codePoint, FontSelector fontSelector, FontProvider fontProvider
            , FontSet additionalFonts) {
            PdfFont matchedFont = null;
            foreach (FontInfo fontInfo in fontSelector.GetFonts()) {
                if (fontInfo.GetFontUnicodeRange().Contains(codePoint)) {
                    PdfFont temptFont = GetPdfFont(fontInfo, fontProvider, additionalFonts);
                    if (temptFont.ContainsGlyph(codePoint)) {
                        matchedFont = temptFont;
                        break;
                    }
                }
            }
            return matchedFont;
        }

        private static int NextSignificantIndex(int startIndex, String text) {
            int nextValidChar = startIndex;
            for (; nextValidChar < text.Length; nextValidChar++) {
                if (!iText.IO.Util.TextUtil.IsWhitespaceOrNonPrintable(text[nextValidChar])) {
                    break;
                }
            }
            return nextValidChar;
        }

        private static bool IsSignificantUnicodeScript(UnicodeScript? unicodeScript) {
            // Character.UnicodeScript.UNKNOWN will be handled as significant unicode script
            return unicodeScript != UnicodeScript.COMMON && unicodeScript != UnicodeScript.INHERITED;
        }

        private static int ExtractCodePoint(String text, int idx) {
            return iText.IO.Util.TextUtil.IsSurrogatePair(text, idx) ? iText.IO.Util.TextUtil.ConvertToUtf32(text, idx
                ) : (int)text[idx];
        }

        /// <summary>Utility method to create PdfFont.</summary>
        /// <param name="fontInfo">instance of FontInfo</param>
        /// <returns>cached or just created PdfFont on success, otherwise null</returns>
        /// <seealso cref="iText.Layout.Font.FontProvider.GetPdfFont(iText.Layout.Font.FontInfo, iText.Layout.Font.FontSet)
        ///     "/>
        private static PdfFont GetPdfFont(FontInfo fontInfo, FontProvider fontProvider, FontSet additionalFonts) {
            return fontProvider.GetPdfFont(fontInfo, additionalFonts);
        }
    }
}
