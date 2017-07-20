using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataLocaSize001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-loca-size-001";
        }

        protected internal override String GetTestInfo() {
            return "A valid TTF flavoured font where the loca table uses the long format.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
