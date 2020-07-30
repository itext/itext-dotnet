using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Filters {
    public class ASCII85DecodeFilterTest : ExtendedITextTest {
        public static readonly String SOURCE_FILE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/filters/ASCII85.bin";

        [NUnit.Framework.Test]
        public virtual void DecodingTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            byte[] bytes = File.ReadAllBytes(file.FullName);
            String expectedResult = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " + "Donec ac malesuada tellus. "
                 + "Quisque a arcu semper, tristique nibh eu, convallis lacus. " + "Donec neque justo, condimentum sed molestie ac, mollis eu nibh. "
                 + "Vivamus pellentesque condimentum fringilla. " + "Nullam euismod ac risus a semper. " + "Etiam hendrerit scelerisque sapien tristique varius.";
            ASCII85DecodeFilter filter = new ASCII85DecodeFilter();
            String decoded = iText.IO.Util.JavaUtil.GetStringForBytes(filter.Decode(bytes, null, null, new PdfDictionary
                ()));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }

        [NUnit.Framework.Test]
        public virtual void DecodingWithZeroBytesTest() {
            byte[] bytes = "z9Q+r_D#".GetBytes();
            String expectedResult = iText.IO.Util.JavaUtil.GetStringForBytes(new byte[] { 0, 0, 0, 0, (byte)'L', (byte
                )'o', (byte)'r', (byte)'e', (byte)'m' });
            ASCII85DecodeFilter filter = new ASCII85DecodeFilter();
            String decoded = iText.IO.Util.JavaUtil.GetStringForBytes(filter.Decode(bytes, null, null, new PdfDictionary
                ()));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }
    }
}
