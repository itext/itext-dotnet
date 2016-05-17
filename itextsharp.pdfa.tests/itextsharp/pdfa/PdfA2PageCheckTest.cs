using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Rules;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Pdfa
{
	public class PdfA2PageCheckTest
	{
		public const String sourceFolder = "../../resources/itextsharp/pdfa/";

		[Rule]
		public ExpectedException thrown = ExpectedException.None();

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void CatalogCheck01()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.PageDictionaryShallNotContainPressstepsEntry
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			page.GetPdfObject().Put(PdfName.PresSteps, new PdfDictionary());
			doc.Close();
		}
	}
}
