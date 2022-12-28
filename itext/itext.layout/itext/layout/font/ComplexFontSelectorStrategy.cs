/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    /// <summary>
    /// Complex FontSelectorStrategy split text based on
    /// <see cref="iText.Commons.Utils.UnicodeScript?"/>.
    /// </summary>
    /// <remarks>
    /// Complex FontSelectorStrategy split text based on
    /// <see cref="iText.Commons.Utils.UnicodeScript?"/>.
    /// If unicode script changes, a new font will be found.
    /// If there is no suitable font, only one notdef glyph from
    /// <see cref="FontSelector.BestMatch()"/>
    /// will be added.
    /// </remarks>
    public class ComplexFontSelectorStrategy : FontSelectorStrategy {
        private PdfFont font;

        private FontSelector selector;

        public ComplexFontSelectorStrategy(String text, FontSelector selector, FontProvider provider, FontSet additionalFonts
            )
            : base(text, provider, additionalFonts) {
            this.font = null;
            this.selector = selector;
        }

        public ComplexFontSelectorStrategy(String text, FontSelector selector, FontProvider provider)
            : base(text, provider, null) {
            this.font = null;
            this.selector = selector;
        }

        public override PdfFont GetCurrentFont() {
            return font;
        }

        public override IList<Glyph> NextGlyphs() {
            font = null;
            int nextUnignorable = NextSignificantIndex();
            if (nextUnignorable < text.Length) {
                foreach (FontInfo f in selector.GetFonts()) {
                    int codePoint = IsSurrogatePair(text, nextUnignorable) ? iText.IO.Util.TextUtil.ConvertToUtf32(text, nextUnignorable
                        ) : (int)text[nextUnignorable];
                    if (f.GetFontUnicodeRange().Contains(codePoint)) {
                        PdfFont currentFont = GetPdfFont(f);
                        Glyph glyph = currentFont.GetGlyph(codePoint);
                        if (null != glyph && 0 != glyph.GetCode()) {
                            font = currentFont;
                            break;
                        }
                    }
                }
            }
            IList<Glyph> glyphs = new List<Glyph>();
            bool anyGlyphsAppended = false;
            if (font != null) {
                UnicodeScript? unicodeScript = NextSignificantUnicodeScript(nextUnignorable);
                int to = nextUnignorable;
                for (int i = nextUnignorable; i < text.Length; i++) {
                    int codePoint = IsSurrogatePair(text, i) ? iText.IO.Util.TextUtil.ConvertToUtf32(text, i) : (int)text[i];
                    UnicodeScript? currScript = UnicodeScriptUtil.Of(codePoint);
                    if (IsSignificantUnicodeScript(currScript) && currScript != unicodeScript) {
                        break;
                    }
                    if (codePoint > 0xFFFF) {
                        i++;
                    }
                    to = i;
                }
                int numOfAppendedGlyphs = font.AppendGlyphs(text, index, to, glyphs);
                anyGlyphsAppended = numOfAppendedGlyphs > 0;
                System.Diagnostics.Debug.Assert(anyGlyphsAppended);
                index += numOfAppendedGlyphs;
            }
            if (!anyGlyphsAppended) {
                font = GetPdfFont(selector.BestMatch());
                if (index != nextUnignorable) {
                    index += font.AppendGlyphs(text, index, nextUnignorable - 1, glyphs);
                }
                while (index <= nextUnignorable && index < text.Length) {
                    index += font.AppendAnyGlyph(text, index, glyphs);
                }
            }
            return glyphs;
        }

        private int NextSignificantIndex() {
            int nextValidChar = index;
            for (; nextValidChar < text.Length; nextValidChar++) {
                if (!iText.IO.Util.TextUtil.IsWhitespaceOrNonPrintable(text[nextValidChar])) {
                    break;
                }
            }
            return nextValidChar;
        }

        private UnicodeScript? NextSignificantUnicodeScript(int from) {
            for (int i = from; i < text.Length; i++) {
                int codePoint;
                if (IsSurrogatePair(text, i)) {
                    codePoint = iText.IO.Util.TextUtil.ConvertToUtf32(text, i);
                    i++;
                }
                else {
                    codePoint = (int)text[i];
                }
                UnicodeScript? unicodeScript = UnicodeScriptUtil.Of(codePoint);
                if (IsSignificantUnicodeScript(unicodeScript)) {
                    return unicodeScript;
                }
            }
            return UnicodeScript.COMMON;
        }

        private static bool IsSignificantUnicodeScript(UnicodeScript? unicodeScript) {
            // Character.UnicodeScript.UNKNOWN will be handled as significant unicode script
            return unicodeScript != UnicodeScript.COMMON && unicodeScript != UnicodeScript.INHERITED;
        }

        //This method doesn't perform additional checks if compare with TextUtil#isSurrogatePair()
        private static bool IsSurrogatePair(String text, int idx) {
            return iText.IO.Util.TextUtil.IsSurrogateHigh(text[idx]) && idx < text.Length - 1 && iText.IO.Util.TextUtil
                .IsSurrogateLow(text[idx + 1]);
        }
    }
}
