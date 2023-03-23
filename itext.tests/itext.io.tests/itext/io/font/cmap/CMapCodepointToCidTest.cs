using iText.Test;

namespace iText.IO.Font.Cmap {
    [NUnit.Framework.Category("UnitTest")]
    public class CMapCodepointToCidTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ReverseConstructorTest() {
            CMapCidToCodepoint cidToCode = new CMapCidToCodepoint();
            cidToCode.AddChar(iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { 32, 17 }), new CMapObject(CMapObject
                .NUMBER, 14));
            cidToCode.AddChar(iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { 32, 18 }), new CMapObject(CMapObject
                .NUMBER, 15));
            CMapCodepointToCid codeToCid = new CMapCodepointToCid(cidToCode);
            NUnit.Framework.Assert.AreEqual(14, codeToCid.Lookup(8209));
            NUnit.Framework.Assert.AreEqual(15, codeToCid.Lookup(8210));
        }

        [NUnit.Framework.Test]
        public virtual void AddCharAndLookupTest() {
            CMapCodepointToCid codeToCid = new CMapCodepointToCid();
            NUnit.Framework.Assert.AreEqual(0, codeToCid.Lookup(8209));
            codeToCid.AddChar(iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { 32, 17 }), new CMapObject(CMapObject
                .NUMBER, 14));
            codeToCid.AddChar(iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { 32, 19 }), new CMapObject(CMapObject
                .STRING, "some text"));
            NUnit.Framework.Assert.AreEqual(14, codeToCid.Lookup(8209));
            NUnit.Framework.Assert.AreEqual(0, codeToCid.Lookup(1));
        }
    }
}
