using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using NUnit.Framework.Rules;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Pdfa
{
	public class PdfA1AnnotationCheckTest
	{
		public const String sourceFolder = "../../resources/itextsharp/pdfa/";

		public const String cmpFolder = sourceFolder + "cmp/PdfA1AnnotationCheckTest/";

		public const String destinationFolder = "./target/test/PdfA1AnnotationCheckTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(destinationFolder).Mkdirs();
		}

		[Rule]
		public ExpectedException thrown = ExpectedException.None();

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest01()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AnnotationType1IsNotPermitted);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfAnnotation annot = new PdfFileAttachmentAnnotation(rect);
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest02()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AnAnnotationDictionaryShallNotContainTheCaKeyWithAValueOtherThan1
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
			annot.SetFlag(PdfAnnotation.PRINT);
			annot.SetOpacity(new PdfNumber(0.5));
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest03()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleAndNoviewFlagBitsShallBeSetTo0
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
			annot.SetFlag(0);
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest04()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleAndNoviewFlagBitsShallBeSetTo0
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
			annot.SetFlag(PdfAnnotation.PRINT);
			annot.SetFlag(PdfAnnotation.INVISIBLE);
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest05()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfAnnotation annot = new PdfWidgetAnnotation(rect);
			annot.SetFlag(PdfAnnotation.PRINT);
			PdfStream s = new PdfStream("Hello World".GetBytes());
			annot.SetDownAppearance(new PdfDictionary());
			annot.SetNormalAppearance(s);
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest06()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfAnnotation annot = new PdfWidgetAnnotation(rect);
			annot.SetFlag(PdfAnnotation.PRINT);
			annot.SetNormalAppearance(new PdfDictionary());
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void AnnotationCheckTest07()
		{
			String outPdf = destinationFolder + "pdfA1b_annotationCheckTest07.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA1b_annotationCheckTest07.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
			annot.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.NO_ZOOM | PdfAnnotation.NO_ROTATE
				);
			page.AddAnnotation(annot);
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="Java.IO.FileNotFoundException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void AnnotationCheckTest08()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AnnotationOfType1ShouldHaveContentsKey
				);
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.SetTagged();
			doc.GetCatalog().SetLang(new PdfString("en-US"));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfAnnotation annot = new PdfStampAnnotation(rect);
			annot.SetFlag(PdfAnnotation.PRINT);
			page.AddAnnotation(annot);
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void AnnotationCheckTest09()
		{
			String outPdf = destinationFolder + "pdfA1a_annotationCheckTest09.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA1a_annotationCheckTest09.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			doc.SetTagged();
			doc.GetCatalog().SetLang(new PdfString("en-US"));
			PdfPage page = doc.AddNewPage();
			Rectangle rect = new Rectangle(100, 100, 100, 100);
			PdfAnnotation annot = new PdfStampAnnotation(rect);
			annot.SetFlag(PdfAnnotation.PRINT);
			annot.SetContents("Hello world");
			page.AddAnnotation(annot);
			doc.Close();
			CompareResult(outPdf, cmpPdf);
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
