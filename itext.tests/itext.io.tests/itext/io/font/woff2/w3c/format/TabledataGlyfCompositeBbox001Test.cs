using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataGlyfCompositeBbox001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-glyf-composite-bbox-001";
        }

        protected internal override String GetTestInfo() {
            return "Valid TTF flavored WOFF with composite glyphs";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
