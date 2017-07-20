using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Decoder {
    public class ValidationChecksum002Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "validation-checksum-002";
        }

        protected internal override String GetTestInfo() {
            return "Valid CFF flavored WOFF file, the output file is put through an OFF validator to check the validity of head table checkSumAdjustment.";
        }

        protected internal override bool IsFontValid() {
            return true;
        }
    }
}
