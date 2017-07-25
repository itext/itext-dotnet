using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataTransformLength001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-transform-length-001";
        }

        protected internal override String GetTestInfo() {
            return "The transformed loca table contains 4 zero bytes and its transformLength is 4.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
