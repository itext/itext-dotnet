using System;
using iText.IO.Font.Woff2.W3c;

namespace iText.IO.Font.Woff2.W3c.Format {
    [NUnit.Framework.Ignore("Different in result form expected in w3c suite. See html font-face test for more details"
        )]
    public class BlocksMetadataPadding001Test : W3CWoff2DecodeTest {
        protected internal override String GetFontName() {
            return "blocks-metadata-padding-001";
        }

        protected internal override String GetTestInfo() {
            return "The metadata block is padded to a four-byte boundary but there is no private data.";
        }

        protected internal override bool IsFontValid() {
            return false;
        }
    }
}
