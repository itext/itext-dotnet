using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Decoder {
    public class ValidationLocaFormat001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "validation-loca-format-001";
        }

        protected internal override String GetTestInfo() {
            return "Valid TTF flavored WOFF with simple composite glyphs where the loca table uses the short format, to check loca reconstruction";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
