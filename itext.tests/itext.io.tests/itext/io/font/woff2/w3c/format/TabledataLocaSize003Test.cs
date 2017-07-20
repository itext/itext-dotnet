using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataLocaSize003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-loca-size-003";
        }

        protected internal override String GetTestInfo() {
            return "A valid CFF flavoured font which naturally have no loca table.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
