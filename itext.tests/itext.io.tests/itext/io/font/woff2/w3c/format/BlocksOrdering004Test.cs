using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksOrdering004Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-ordering-004";
        }

        protected internal override String GetTestInfo() {
            return "The private data block is stored before the metadata block.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
