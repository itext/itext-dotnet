using System;
using System.Collections.Generic;
using iText.IO.Font.Otf;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public abstract class FontSelectorStrategy {
        protected internal String text;

        protected internal int index;

        protected internal FontSelectorStrategy(String text) {
            this.text = text;
            this.index = 0;
        }

        public virtual bool EndOfText() {
            return text == null || index >= text.Length;
        }

        public abstract PdfFont GetFont();

        //TODO List or GlyphLine?
        public abstract IList<Glyph> NextGlyphs();
    }
}
