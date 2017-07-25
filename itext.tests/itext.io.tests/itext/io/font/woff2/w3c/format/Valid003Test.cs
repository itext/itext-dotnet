using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class Valid003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "valid-003";
        }

        protected internal override String GetTestInfo() {
            return "Valid CFF flavored WOFF with private data";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
