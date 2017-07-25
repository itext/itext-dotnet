using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    [NUnit.Framework.Ignore("Different in result form expected in w3c suite. See html font-face test for more details"
        )]
    public class TabledataHmtxTransform003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-hmtx-transform-003";
        }

        protected internal override String GetTestInfo() {
            return "Invalid TTF flavored WOFF with transformed hmtx table that has all flags bits (including reserved bits) set.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
