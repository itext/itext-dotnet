using NUnit.Framework;
using com.itextpdf.io.font;

namespace com.itextpdf.io
{
	public class AdobeGlyphListTest
	{
		[Test]
		public virtual void TestGlyphListCount()
		{
			NUnit.Framework.Assert.AreEqual(4200, AdobeGlyphList.GetNameToUnicodeLength());
			NUnit.Framework.Assert.AreEqual(3680, AdobeGlyphList.GetUnicodeToNameLength());
		}
	}
}
