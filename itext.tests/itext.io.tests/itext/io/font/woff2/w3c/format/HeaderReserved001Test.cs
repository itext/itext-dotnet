using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    [NUnit.Framework.Ignore("Different in result form expected in w3c suite. See html font-face test for more details"
        )]
    public class HeaderReserved001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "header-reserved-001";
        }

        protected internal override String GetTestInfo() {
            return "The reserved field contains 1.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
