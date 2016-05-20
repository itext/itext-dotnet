using System;
using NUnit.Framework;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfNameTest : ITextTest
	{
		[Test]
		public virtual void SpecialCharactersTest()
		{
			String str1 = " %()<>";
			String str2 = "[]{}/#";
			PdfName name1 = new PdfName(str1);
			NUnit.Framework.Assert.AreEqual(str1, CreateStringByEscaped(name1.GetInternalContent
				()));
			PdfName name2 = new PdfName(str2);
			NUnit.Framework.Assert.AreEqual(str2, CreateStringByEscaped(name2.GetInternalContent
				()));
		}
	}
}
