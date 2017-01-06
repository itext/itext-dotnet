using System;
using System.Collections.Generic;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public class FontProvider {
        private IList<PdfFont> fonts = new List<PdfFont>();

        // initial big collection of fonts, entry point for all font selector logic.
        // FontProvider depends from PdfDocument, due to PdfFont.
        // TODO it might works with FontPrograms.
        public virtual IList<PdfFont> GetAllFonts() {
            return fonts;
        }

        public virtual void AddFont(PdfFont font) {
            fonts.Add(font);
        }

        public virtual FontSelector GetSelector(String fontFamily) {
            return new NamedFontSelector(GetAllFonts(), fontFamily);
        }

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily) {
            return new ComplexFontSelectorStrategy(text, GetSelector(fontFamily));
        }
    }
}
