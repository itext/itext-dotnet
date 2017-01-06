using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public class FontProvider {
        private IList<PdfFont> fonts = new List<PdfFont>();

        protected internal IDictionary<FontProvider.FontSelectorKey, FontSelector> fontSelectorCache;

        public FontProvider(IList<PdfFont> pdfFonts) {
            // initial big collection of fonts, entry point for all font selector logic.
            // FontProvider depends from PdfDocument, due to PdfFont.
            this.fonts = pdfFonts;
            this.fontSelectorCache = new Dictionary<FontProvider.FontSelectorKey, FontSelector>();
        }

        public FontProvider()
            : this(new List<PdfFont>()) {
        }

        /// <summary>Note, this operation will reset internal FontSelector cache.</summary>
        /// <param name="font"/>
        public virtual void AddFont(PdfFont font) {
            fonts.Add(font);
            fontSelectorCache.Clear();
        }

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily, int style) {
            return new ComplexFontSelectorStrategy(text, GetSelector(fontFamily, style));
        }

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily) {
            return GetStrategy(text, fontFamily, FontConstants.UNDEFINED);
        }

        /// <param name="fontFamily"/>
        /// <param name="style">
        /// Shall be
        /// <see cref="iText.IO.Font.FontConstants.UNDEFINED"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.NORMAL"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.ITALIC"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.BOLD"/>
        /// , or
        /// <see cref="iText.IO.Font.FontConstants.BOLDITALIC"/>
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </returns>
        protected internal virtual FontSelector GetSelector(String fontFamily, int style) {
            FontProvider.FontSelectorKey key = new FontProvider.FontSelectorKey(fontFamily, style);
            if (fontSelectorCache.ContainsKey(key)) {
                return fontSelectorCache.Get(key);
            }
            else {
                FontSelector fontSelector = new NamedFontSelector(fonts, fontFamily, style);
                fontSelectorCache[key] = fontSelector;
                return fontSelector;
            }
        }

        private class FontSelectorKey {
            internal String fontFamily;

            internal int style;

            public FontSelectorKey(String fontFamily, int style) {
                this.fontFamily = fontFamily;
                this.style = style;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                FontProvider.FontSelectorKey that = (FontProvider.FontSelectorKey)o;
                return style == that.style && (fontFamily != null ? fontFamily.Equals(that.fontFamily) : that.fontFamily ==
                     null);
            }

            public override int GetHashCode() {
                int result = fontFamily != null ? fontFamily.GetHashCode() : 0;
                result = 31 * result + style;
                return result;
            }
        }
    }
}
