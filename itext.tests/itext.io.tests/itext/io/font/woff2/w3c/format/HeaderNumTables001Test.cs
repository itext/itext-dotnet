using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class HeaderNumTables001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "header-numTables-001";
        }

        protected internal override String GetTestInfo() {
            return "The header contains 0 in the numTables field. A table directory and table data are present.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
