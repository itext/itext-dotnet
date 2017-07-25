using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksExtraneousData007Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-extraneous-data-007";
        }

        protected internal override String GetTestInfo() {
            return "There are four null bytes after the private data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
