using System;
using System.Collections.Generic;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public class NamedFontSelector : FontSelector {
        internal IList<PdfFont> fonts;

        public NamedFontSelector(IList<PdfFont> allFonts, String fontFamily) {
            this.fonts = allFonts;
        }

        public override PdfFont BestMatch() {
            return fonts[0];
        }

        public override IEnumerable<PdfFont> GetFonts() {
            return fonts;
        }
    }
}
