using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class HeaderSignature001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "header-signature-001";
        }

        protected internal override String GetTestInfo() {
            return "The signature field contains XXXX instead of wOFF.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
