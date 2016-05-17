using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using NUnit.Framework.Rules;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Pdfa
{
	public class PdfA1CanvasCheckTest
	{
		public const String sourceFolder = "../../resources/itextsharp/pdfa/";

		public const String cmpFolder = sourceFolder + "cmp/PdfA1CanvasCheckTest/";

		public const String destinationFolder = "./target/test/PdfA1CanvasCheckTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(destinationFolder).Mkdirs();
		}

		[Rule]
		public ExpectedException thrown = ExpectedException.None();

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void CanvasCheckTest1()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.GraphicStateStackDepthIsGreaterThan28
				);
			PdfWriter writer = new PdfWriter(new MemoryStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B
				, outputIntent);
			pdfDocument.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
			for (int i = 0; i < 29; i++)
			{
				canvas.SaveState();
			}
			for (int i_1 = 0; i_1 < 28; i_1++)
			{
				canvas.RestoreState();
			}
			pdfDocument.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CanvasCheckTest2()
		{
			String outPdf = destinationFolder + "pdfA1b_canvasCheckTest2.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA1b_canvasCheckTest2.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B
				, outputIntent);
			pdfDocument.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
			for (int i = 0; i < 28; i++)
			{
				canvas.SaveState();
			}
			for (int i_1 = 0; i_1 < 28; i_1++)
			{
				canvas.RestoreState();
			}
			pdfDocument.Close();
			String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder
				, "diff_");
			if (result != null)
			{
				NUnit.Framework.Assert.Fail(result);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void CanvasCheckTest3()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.IfSpecifiedRenderingShallBeOneOfTheFollowingRelativecolorimetricAbsolutecolorimetricPerceptualOrSaturation
				);
			PdfWriter writer = new PdfWriter(new MemoryStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B
				, outputIntent);
			pdfDocument.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(pdfDocument.GetLastPage());
			canvas.SetRenderingIntent(new PdfName("Test"));
			pdfDocument.Close();
		}
	}
}
