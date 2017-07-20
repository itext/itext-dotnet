using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksMetadataPadding002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-metadata-padding-002";
        }

        protected internal override String GetTestInfo() {
            return "The metadata block is not padded and there is no private data.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
