using NUnit.Framework;
using iTextSharp.IO.Font;

namespace iTextSharp.IO
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
