using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class Valid005Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "valid-005";
        }

        protected internal override String GetTestInfo() {
            return "Valid TTF flavored WOFF with no metadata and no private data";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
