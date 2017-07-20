using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataExtraneousData001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-extraneous-data-001";
        }

        protected internal override String GetTestInfo() {
            return "There is extraneous data before the last table.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
