using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class BlocksExtraneousData003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-extraneous-data-003";
        }

        protected internal override String GetTestInfo() {
            return "There are four null bytes between the table data and the metadata.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
