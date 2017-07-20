using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksPrivate001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-private-001";
        }

        protected internal override String GetTestInfo() {
            return "The private data does not begin on a four byte boundary because the metadata is not padded.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
