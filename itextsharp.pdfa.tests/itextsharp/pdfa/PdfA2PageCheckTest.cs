using System;
using System.IO;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Test;

namespace iTextSharp.Pdfa
{
	public class PdfA2PageCheckTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/../../resources/itextsharp/pdfa/";

		/// <exception cref="System.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[NUnit.Framework.Test]
		public virtual void CatalogCheck01()
		{
			NUnit.Framework.Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open, FileAccess.Read);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
					("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
				PdfPage page = doc.AddNewPage();
				page.GetPdfObject().Put(PdfName.PresSteps, new PdfDictionary());
				doc.Close();
			}
			, NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PageDictionaryShallNotContainPressstepsEntry));
;
		}
	}
}
