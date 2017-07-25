using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataTransformLength002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-transform-length-002";
        }

        protected internal override String GetTestInfo() {
            return "The transformed tables does not have transformLength set.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
