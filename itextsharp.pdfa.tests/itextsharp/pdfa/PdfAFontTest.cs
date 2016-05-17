using System;
using System.IO;
using Java.IO;
using NUnit.Framework;
using NUnit.Framework.Rules;
using iTextSharp.Kernel.Color;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Pdfa
{
	public class PdfAFontTest
	{
		internal const String sourceFolder = "../../resources/itextsharp/pdfa/";

		internal const String outputDir = "./target/test/PdfAFontTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(outputDir).Mkdirs();
		}

		[Rule]
		public ExpectedException thrown = ExpectedException.None();

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FontCheckPdfA1_01()
		{
			String outPdf = outputDir + "pdfA1b_fontCheckPdfA1_01.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_01.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void FontCheckPdfA1_02()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AllFontsMustBeEmbeddedThisOneIsnt1);
			PdfWriter writer = new PdfWriter(new MemoryStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi"
				);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FontCheckPdfA1_03()
		{
			String outPdf = outputDir + "pdfA1b_fontCheckPdfA1_03.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_03.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H"
				, false);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		[Test]
		public virtual void FontCheckPdfA1_04()
		{
			thrown.Expect(typeof(PdfAConformanceException));
			thrown.ExpectMessage(PdfAConformanceException.AllFontsMustBeEmbeddedThisOneIsnt1);
			PdfWriter writer = new PdfWriter(new MemoryStream());
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont("Helvetica", "WinAnsi", true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
			doc.Close();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FontCheckPdfA1_05()
		{
			String outPdf = outputDir + "pdfA1b_fontCheckPdfA1_05.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_05.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKtc-Light.otf"
				, "Identity-H");
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FontCheckPdfA2_01()
		{
			String outPdf = outputDir + "pdfA2b_fontCheckPdfA2_01.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_fontCheckPdfA2_01.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H"
				, false);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-2B").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FontCheckPdfA3_01()
		{
			String outPdf = outputDir + "pdfA3b_fontCheckPdfA3_01.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA3b_fontCheckPdfA3_01.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H"
				, false);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().SetFillColor(DeviceRgb.GREEN).BeginText().MoveText(36, 700).SetFontAndSize
				(font, 36).ShowText("Hello World! Pdf/A-3B").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CidFontCheckTest1()
		{
			String outPdf = outputDir + "pdfA2b_cidFontCheckTest1.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest1.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText
				("Hello World").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CidFontCheckTest2()
		{
			String outPdf = outputDir + "pdfA2b_cidFontCheckTest2.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest2.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "Puritan2.otf", "Identity-H"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText
				("Hello World").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void CidFontCheckTest3()
		{
			String outPdf = outputDir + "pdfA2b_cidFontCheckTest3.pdf";
			String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest3.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm");
			PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent
				("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
			PdfPage page = doc.AddNewPage();
			// Identity-H must be embedded
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKtc-Light.otf"
				, "Identity-H", true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText
				("Hello World").EndText().RestoreState();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private void CompareResult(String outPdf, String cmpPdf)
		{
			String result = new CompareTool().CompareByContent(outPdf, cmpPdf, outputDir, "diff_"
				);
			if (result != null)
			{
				NUnit.Framework.Assert.Fail(result);
			}
		}
	}
}
