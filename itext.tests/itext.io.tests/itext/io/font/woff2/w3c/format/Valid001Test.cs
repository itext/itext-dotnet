using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class Valid001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "valid-001";
        }

        protected internal override String GetTestInfo() {
            return "Valid CFF flavored WOFF with no metadata and no private data";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
