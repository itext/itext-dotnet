using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksOrdering003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-ordering-003";
        }

        protected internal override String GetTestInfo() {
            return "The metadata block is stored after the private data block.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
