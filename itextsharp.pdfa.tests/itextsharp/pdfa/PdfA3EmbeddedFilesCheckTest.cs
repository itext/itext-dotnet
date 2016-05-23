using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Pdfa
{
	public class PdfA3EmbeddedFilesCheckTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/pdfa/";

		public const String cmpFolder = sourceFolder + "cmp/PdfA3EmbeddedFilesCheckTest/";

		public const String destinationFolder = "test/itextsharp/pdfa/PdfA3EmbeddedFilesCheckTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FileSpecCheckTest01()
		{
			String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest01.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest01.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B
				, outputIntent);
			PdfPage page = pdfDocument.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText
				("Hello World!").EndText().RestoreState();
			MemoryStream txt = new MemoryStream();
			StreamWriter @out = new StreamWriter(txt);
			@out.Write("<foo><foo2>Hello world</foo2></foo>");
			@out.Close();
			pdfDocument.AddFileAttachment("foo file", txt.ToArray(), "foo.xml", PdfName.ApplicationXml
				, null, PdfName.Source);
			pdfDocument.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FileSpecCheckTest02()
		{
			String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest02.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest02.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B
				, outputIntent);
			PdfPage page = pdfDocument.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText
				("Hello World!").EndText().RestoreState();
			MemoryStream txt = new MemoryStream();
			StreamWriter @out = new StreamWriter(txt);
			@out.Write("<foo><foo2>Hello world</foo2></foo>");
			@out.Close();
			pdfDocument.AddFileAttachment("foo file", txt.ToArray(), "foo.xml", null, null, PdfName
				.Unspecified);
			pdfDocument.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FileSpecCheckTest03()
		{
			String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest03.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest03.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B
				, outputIntent);
			PdfPage page = pdfDocument.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText
				("Hello World!").EndText().RestoreState();
			byte[] somePdf = new byte[25];
			pdfDocument.AddFileAttachment("some pdf file", somePdf, "foo.pdf", PdfName.ApplicationPdf
				, null, PdfName.Data);
			pdfDocument.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void FileSpecCheckTest04()
		{
			String outPdf = destinationFolder + "pdfA3b_fileSpecCheckTest04.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA3b_fileSpecCheckTest04.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B
				, outputIntent);
			PdfPage page = pdfDocument.AddNewPage();
			PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi"
				, true);
			PdfCanvas canvas = new PdfCanvas(page);
			canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 36).ShowText
				("Hello World!").EndText().RestoreState();
			MemoryStream txt = new MemoryStream();
			StreamWriter @out = new StreamWriter(txt);
			@out.Write("<foo><foo2>Hello world</foo2></foo>");
			@out.Close();
			pdfDocument.AddFileAttachment("foo file", txt.ToArray(), "foo.xml", null, null, null
				);
			pdfDocument.Close();
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
