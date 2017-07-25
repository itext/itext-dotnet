using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class Valid002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "valid-002";
        }

        protected internal override String GetTestInfo() {
            return "Valid CFF flavored WOFF with metadata";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
