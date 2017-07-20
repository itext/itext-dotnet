using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksMetadataPadding004Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-metadata-padding-004";
        }

        protected internal override String GetTestInfo() {
            return "The beginning of the metadata block is not padded.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
