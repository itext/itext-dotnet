using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class DirectoryTableOrder001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "directory-table-order-001";
        }

        protected internal override String GetTestInfo() {
            return "A valid WOFF2 font with tables ordered correctly in the table directory";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
