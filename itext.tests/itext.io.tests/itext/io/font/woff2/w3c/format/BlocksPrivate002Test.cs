using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksPrivate002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-private-002";
        }

        protected internal override String GetTestInfo() {
            return "The private data does not correspond to the end of the WOFF2 file because there are 4 null bytes after it.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
