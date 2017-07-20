using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksExtraneousData002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-extraneous-data-002";
        }

        protected internal override String GetTestInfo() {
            return "There are four null bytes after the table data block and there is no metadata or private data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
