using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    [NUnit.Framework.Ignore("Different from c++ version. See html font-face test for more details")]
    public class DirectoryTableOrder002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "directory-table-order-002";
        }

        protected internal override String GetTestInfo() {
            return "An invalid WOFF2 font with loca before glyf in the table directory";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
