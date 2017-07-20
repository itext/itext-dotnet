using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksExtraneousData004Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-extraneous-data-004";
        }

        protected internal override String GetTestInfo() {
            return "There are four null bytes between the table data and the private data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
