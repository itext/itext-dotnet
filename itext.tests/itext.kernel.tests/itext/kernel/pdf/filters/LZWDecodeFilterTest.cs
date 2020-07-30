using System;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Filters {
    public class LZWDecodeFilterTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DecodingTestStatic() {
            byte[] bytes = new byte[] { (byte)0x80, 0x0B, 0x60, 0x50, 0x22, 0x0C, 0x0C, (byte)0x85, 0x01 };
            String expectedResult = "-----A---B";
            String decoded = iText.IO.Util.JavaUtil.GetStringForBytes(LZWDecodeFilter.LZWDecode(bytes));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }

        [NUnit.Framework.Test]
        public virtual void DecodingTestNonStatic() {
            byte[] bytes = new byte[] { (byte)0x80, 0x0B, 0x60, 0x50, 0x22, 0x0C, 0x0C, (byte)0x85, 0x01 };
            String expectedResult = "-----A---B";
            LZWDecodeFilter filter = new LZWDecodeFilter();
            String decoded = iText.IO.Util.JavaUtil.GetStringForBytes(filter.Decode(bytes, null, new PdfDictionary(), 
                new PdfDictionary()));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }
    }
}
