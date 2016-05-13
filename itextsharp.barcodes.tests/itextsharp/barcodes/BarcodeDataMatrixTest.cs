using System;
using Java.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;

namespace iTextSharp.Barcodes
{
	public class BarcodeDataMatrixTest
	{
		public const String sourceFolder = "../../resources/itextsharp/barcodes/";

		public const String destinationFolder = "test/itextsharp/barcodes/BarcodeDataMatrix/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			File dir = new File(destinationFolder);
			dir.Mkdirs();
			foreach (File file in dir.ListFiles())
			{
				file.Delete();
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode01Test()
		{
			String filename = "barcodeDataMatrix.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			BarcodeDataMatrix barcode = new BarcodeDataMatrix();
			barcode.SetCode("AAAAAAAAAA;BBBBAAAA3;00028;BBBAA05;AAAA;AAAAAA;1234567;AQWXSZ;JEAN;;;;7894561;AQWXSZ;GEO;;;;1;1;1;1;0;0;1;0;1;0;0;0;1;0;1;0;0;0;0;0;0;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1"
				);
			barcode.PlaceBarcode(canvas, iTextSharp.Kernel.Color.Color.GREEN, 5);
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
			String filename = "barcodeDataMatrix2.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page1 = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page1);
			BarcodeDataMatrix barcode2 = new BarcodeDataMatrix("дима", "UTF-8");
			barcode2.PlaceBarcode(canvas, iTextSharp.Kernel.Color.Color.GREEN, 10);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
