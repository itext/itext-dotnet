using System;
using Java.IO;
using NUnit.Framework;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Barcodes
{
	public class Barcode39Test
	{
		public const String sourceFolder = "../../resources/itextsharp/barcodes/";

		public const String destinationFolder = "test/itextsharp/barcodes/Barcode39/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(destinationFolder).Mkdirs();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode01Test()
		{
			String filename = "barcode39_01.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			Barcode1D barcode = new Barcode39(document);
			barcode.SetCode("9781935182610");
			barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
			barcode.PlaceBarcode(canvas, iTextSharp.Kernel.Color.Color.BLACK, iTextSharp.Kernel.Color.Color
				.BLACK);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode02Test()
		{
			String filename = "barcode39_02.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfReader reader = new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf");
			PdfDocument document = new PdfDocument(reader, writer);
			PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
			Barcode1D barcode = new Barcode39(document);
			barcode.SetCode("9781935182610");
			barcode.SetTextAlignment(Barcode1D.ALIGN_LEFT);
			barcode.PlaceBarcode(canvas, iTextSharp.Kernel.Color.Color.BLACK, iTextSharp.Kernel.Color.Color
				.BLACK);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}

		public virtual void Barcode03Test()
		{
			PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
			PdfDocument document = new PdfDocument(writer);
			Barcode39 barcode = new Barcode39(document);
			Barcode39.GetBarsCode39("9781935*182610");
		}
	}
}
