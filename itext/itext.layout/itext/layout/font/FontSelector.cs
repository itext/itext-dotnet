using System.Collections.Generic;

namespace iText.Layout.Font {
    public abstract class FontSelector {
        // select font from sublist
        /// <summary>The best PdfFont match for given font family.</summary>
        public abstract FontProgramInfo BestMatch();

        public abstract IEnumerable<FontProgramInfo> GetFonts();
    }
}
