using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataHmtxTransform001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-hmtx-transform-001";
        }

        protected internal override String GetTestInfo() {
            return "Valid TTF flavored WOFF with transformed hmtx table.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
