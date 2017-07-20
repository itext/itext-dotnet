using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    [NUnit.Framework.Ignore("Different in result form expected in w3c suite. See html font-face test for more details"
        )]
    public class HeaderFlavor001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "header-flavor-001";
        }

        protected internal override String GetTestInfo() {
            return "The header flavor is set to 0x00010000 but the table data contains CFF data, not TTF data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
