using System;
using System.Collections.Generic;
using iText.IO.Font.Otf;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    /// <summary>
    /// <see cref="FontSelectorStrategy"/>
    /// responsible for splitting text into sub texts with font.
    /// <see cref="NextGlyphs()"/>
    /// will create next sub text and set current font.
    /// </summary>
    public abstract class FontSelectorStrategy {
        protected internal String text;

        protected internal int index;

        protected internal FontProvider provider;

        protected internal FontSelectorStrategy(String text, FontProvider provider) {
            this.text = text;
            this.index = 0;
            this.provider = provider;
        }

        public virtual bool EndOfText() {
            return text == null || index >= text.Length;
        }

        public abstract PdfFont GetCurrentFont();

        public abstract IList<Glyph> NextGlyphs();
    }
}
