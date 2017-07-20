using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataBrotli001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-brotli-001";
        }

        protected internal override String GetTestInfo() {
            return "Font table data is compressed with zlib instead of Brotli.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
