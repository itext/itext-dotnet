using System;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Barcodes
{
	public class BarcodeInter25Test : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/barcodes/";

		public const String destinationFolder = "test/itextsharp/barcodes/BarcodeInter25/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void Barcode01Test()
		{
			String filename = "barcodeInter25.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			Barcode1D barcode = new BarcodeInter25(document);
			barcode.SetGenerateChecksum(true);
			barcode.SetCode("41-1200076041-001");
			barcode.SetTextAlignment(Barcode1D.ALIGN_CENTER);
			barcode.PlaceBarcode(canvas, iTextSharp.Kernel.Color.Color.BLUE, iTextSharp.Kernel.Color.Color
				.GREEN);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
