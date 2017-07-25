using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataTransformGlyfLoca002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-transform-glyf-loca-002";
        }

        protected internal override String GetTestInfo() {
            return "The glyf table is not transformed while loca table is transformed.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
