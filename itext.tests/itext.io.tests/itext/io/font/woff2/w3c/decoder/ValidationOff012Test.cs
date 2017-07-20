using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Decoder {
    [NUnit.Framework.Ignore("Different from c++ version. See html font-face test for more details")]
    public class ValidationOff012Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "validation-off-012";
        }

        protected internal override String GetTestInfo() {
            return "Valid WOFF file from the fire format tests, the decoded file should run through a font validator to confirm the OFF structure validity.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
