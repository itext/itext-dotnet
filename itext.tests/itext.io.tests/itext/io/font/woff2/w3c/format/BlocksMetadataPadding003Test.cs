using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksMetadataPadding003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-metadata-padding-003";
        }

        protected internal override String GetTestInfo() {
            return "The metadata block is padded to a four-byte boundary and there is private data.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
