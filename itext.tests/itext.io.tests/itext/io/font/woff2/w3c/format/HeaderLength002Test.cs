using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class HeaderLength002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "header-length-002";
        }

        protected internal override String GetTestInfo() {
            return "The length field contains a value that is four bytes longer than the actual data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
