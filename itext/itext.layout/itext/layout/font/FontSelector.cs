using System.Collections.Generic;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public abstract class FontSelector {
        // select font from sublist
        /// <summary>The best PdfFont match for given font family.</summary>
        public abstract PdfFont BestMatch();

        public abstract IEnumerable<PdfFont> GetFonts();
    }
}
