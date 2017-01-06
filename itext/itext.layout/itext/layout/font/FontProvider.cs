using System;
using System.Collections.Generic;
using iText.IO.Font;
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

        protected internal virtual FontSelector GetSelector(String fontFamily) {
            return GetSelector(fontFamily, FontConstants.UNDEFINED);
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
            return new NamedFontSelector(GetAllFonts(), fontFamily, style);
        }

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily) {
            return new ComplexFontSelectorStrategy(text, GetSelector(fontFamily));
        }
    }
}
