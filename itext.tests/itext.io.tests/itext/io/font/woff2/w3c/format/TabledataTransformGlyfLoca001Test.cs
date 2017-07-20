using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataTransformGlyfLoca001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-transform-glyf-loca-001";
        }

        protected internal override String GetTestInfo() {
            return "The glyf table is transformed while loca table is not.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
