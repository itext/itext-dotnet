using System;
using System.IO;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Pdfa
{
	public class PdfA2AcroFormCheckTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/../../resources/itextsharp/pdfa/";

		public static readonly String cmpFolder = sourceFolder + "cmp/PdfA2AcroFormCheckTest/";

		public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
			.TestDirectory + "/test/itextsharp/pdfa/PdfA2AcroFormCheckTest/";

		[NUnit.Framework.TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[NUnit.Framework.Test]
		public virtual void AcroFormCheck01()
		{
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open
				, FileAccess.Read);
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary acroForm = new PdfDictionary();
			acroForm.Put(PdfName.NeedAppearances, new PdfBoolean(true));
			doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
			try
			{
				doc.Close();
				NUnit.Framework.Assert.Fail("PdfAConformanceException expected");
			}
			catch (PdfAConformanceException)
			{
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void AcroFormCheck02()
		{
			String outPdf = destinationFolder + "pdfA2b_acroFormCheck02.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA2b_acroFormCheck02.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open
				, FileAccess.Read);
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary acroForm = new PdfDictionary();
			acroForm.Put(PdfName.NeedAppearances, new PdfBoolean(false));
			doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void AcroFormCheck03()
		{
			String outPdf = destinationFolder + "pdfA2b_acroFormCheck03.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA2b_acroFormCheck03.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open
				, FileAccess.Read);
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary acroForm = new PdfDictionary();
			doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[NUnit.Framework.Test]
		public virtual void AcroFormCheck04()
		{
			NUnit.Framework.Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open
					, FileAccess.Read);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
					("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
				doc.AddNewPage();
				PdfDictionary acroForm = new PdfDictionary();
				acroForm.Put(PdfName.XFA, new PdfArray());
				doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
				doc.Close();
			}
			, NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.TheInteractiveFormDictionaryShallNotContainTheXfaKey));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private void CompareResult(String outPdf, String cmpPdf)
		{
			String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder
				, "diff_");
			if (result != null)
			{
				NUnit.Framework.Assert.Fail(result);
			}
		}
	}
}
