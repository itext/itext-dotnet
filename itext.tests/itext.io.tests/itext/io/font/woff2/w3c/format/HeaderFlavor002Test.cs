using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    [NUnit.Framework.Ignore("Different in result form expected in w3c suite. See html font-face test for more details"
        )]
    public class HeaderFlavor002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "header-flavor-002";
        }

        protected internal override String GetTestInfo() {
            return "The header flavor is set to OTTO but the table data contains TTF data, not CFF data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
