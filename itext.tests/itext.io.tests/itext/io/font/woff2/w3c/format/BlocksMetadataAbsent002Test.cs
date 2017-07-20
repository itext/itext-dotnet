using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksMetadataAbsent002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-metadata-absent-002";
        }

        protected internal override String GetTestInfo() {
            return "The metadata length is set to zero but the offset is set to the end of the file.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
