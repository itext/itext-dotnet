using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    public class TabledataDecompressedLength003Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "tabledata-decompressed-length-003";
        }

        protected internal override String GetTestInfo() {
            return "The transformed length of the glyf table in the directory is increased by 1, making the decompressed length of the table data less than the sum of transformed table lengths.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
