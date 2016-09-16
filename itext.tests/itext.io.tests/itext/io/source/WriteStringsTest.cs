using System;

namespace iText.IO.Source {
    public class WriteStringsTest {
        [NUnit.Framework.Test]
        public virtual void WriteStringTest() {
            String str = "SomeString";
            byte[] content = ByteUtils.GetIsoBytes(str);
            NUnit.Framework.Assert.AreEqual(str.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), content);
        }

        [NUnit.Framework.Test]
        public virtual void WriteNameTest() {
            String str = "SomeName";
            byte[] content = ByteUtils.GetIsoBytes((byte)'/', str);
            NUnit.Framework.Assert.AreEqual(("/" + str).GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), content);
        }

        [NUnit.Framework.Test]
        public virtual void WritePdfStringTest() {
            String str = "Some PdfString";
            byte[] content = ByteUtils.GetIsoBytes((byte)'(', str, (byte)')');
            NUnit.Framework.Assert.AreEqual(("(" + str + ")").GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), content
                );
        }
    }
}
