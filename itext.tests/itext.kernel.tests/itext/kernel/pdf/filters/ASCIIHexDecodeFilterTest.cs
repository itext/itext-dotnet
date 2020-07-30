using System;
using System.IO;
using iText.Kernel;
using iText.Test;

namespace iText.Kernel.Pdf.Filters {
    public class ASCIIHexDecodeFilterTest : ExtendedITextTest {
        public static readonly String SOURCE_FILE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/filters/ASCIIHex.bin";

        [NUnit.Framework.Test]
        public virtual void DecodingTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            byte[] bytes = File.ReadAllBytes(file.FullName);
            String expectedResult = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " + "Donec ac malesuada tellus. "
                 + "Quisque a arcu semper, tristique nibh eu, convallis lacus. " + "Donec neque justo, condimentum sed molestie ac, mollis eu nibh. "
                 + "Vivamus pellentesque condimentum fringilla. " + "Nullam euismod ac risus a semper. " + "Etiam hendrerit scelerisque sapien tristique varius.";
            String decoded = iText.IO.Util.JavaUtil.GetStringForBytes(ASCIIHexDecodeFilter.ASCIIHexDecode(bytes));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }

        [NUnit.Framework.Test]
        public virtual void DecodingIllegalaCharacterTest() {
            byte[] bytes = "4c6f72656d20697073756d2eg>".GetBytes();
            NUnit.Framework.Assert.That(() =>  {
                ASCIIHexDecodeFilter.ASCIIHexDecode(bytes);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.IllegalCharacterInAsciihexdecode))
;
        }

        [NUnit.Framework.Test]
        public virtual void DecodingSkipWhitespacesTest() {
            byte[] bytes = "4c 6f 72 65 6d 20 69 70 73 75 6d 2e>".GetBytes();
            String expectedResult = "Lorem ipsum.";
            String decoded = iText.IO.Util.JavaUtil.GetStringForBytes(ASCIIHexDecodeFilter.ASCIIHexDecode(bytes));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }
    }
}
