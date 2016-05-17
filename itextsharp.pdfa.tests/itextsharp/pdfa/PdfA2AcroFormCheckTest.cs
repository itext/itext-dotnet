using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using NUnit.Framework.Rules;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Pdfa
{
	public class PdfA2AcroFormCheckTest
	{
		public const String sourceFolder = "../../resources/itextsharp/pdfa/";

		public const String cmpFolder = sourceFolder + "cmp/PdfA2AcroFormCheckTest/";

		public const String destinationFolder = "./target/test/PdfA2AcroFormCheckTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(destinationFolder).Mkdirs();
		}

		[Rule]
		public ExpectedException thrown = ExpectedException.None();

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public virtual void AcroFormCheck01()
		{
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary acroForm = new PdfDictionary();
			acroForm.Put(PdfName.NeedAppearances, new PdfBoolean(true));
			doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void AcroFormCheck02()
		{
			String outPdf = destinationFolder + "pdfA2b_acroFormCheck02.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA2b_acroFormCheck02.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
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
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void AcroFormCheck03()
		{
			String outPdf = destinationFolder + "pdfA2b_acroFormCheck03.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA2b_acroFormCheck03.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary acroForm = new PdfDictionary();
			doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AcroFormCheck04()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.TheInteractiveFormDictionaryShallNotContainTheXfaKey
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.AddNewPage();
			PdfDictionary acroForm = new PdfDictionary();
			acroForm.Put(PdfName.XFA, new PdfArray());
			doc.GetCatalog().Put(PdfName.AcroForm, acroForm);
			doc.Close();
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
