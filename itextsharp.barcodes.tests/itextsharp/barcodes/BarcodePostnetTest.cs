using System;
using Java.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Barcodes
{
	public class BarcodePostnetTest
	{
		public const String sourceFolder = "../../resources/itextsharp/barcodes/";

		public const String destinationFolder = "test/itextsharp/barcodes/BarcodePostnet/";

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
			String filename = "barcodePostnet.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			BarcodePostnet codePlanet = new BarcodePostnet(document);
			codePlanet.SetCode("01234567890");
			codePlanet.SetCodeType(BarcodePostnet.TYPE_PLANET);
			codePlanet.PlaceBarcode(canvas, null, null);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
