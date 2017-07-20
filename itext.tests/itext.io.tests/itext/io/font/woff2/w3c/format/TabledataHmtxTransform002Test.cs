using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataHmtxTransform002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-hmtx-transform-002";
        }

        protected internal override String GetTestInfo() {
            return "Invalid TTF flavored WOFF with transformed hmtx table that has 0 flags (null transform).";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
