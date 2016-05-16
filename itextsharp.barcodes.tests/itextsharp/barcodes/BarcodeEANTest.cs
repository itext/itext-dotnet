using System;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Barcodes
{
	public class BarcodeEANTest : ExtendedITextTest
	{
		public const String sourceFolder = "../../resources/itextsharp/barcodes/";

		public const String destinationFolder = "test/itextsharp/barcodes/BarcodeEAN/";

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
			String filename = "barcodeEAN_01.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			Barcode1D barcode = new BarcodeEAN(document);
			barcode.SetCodeType(BarcodeEAN.EAN13);
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
		[NUnit.Framework.Test]
		public virtual void Barcode02Test()
		{
			String filename = "barcodeEAN_02.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfReader reader = new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf");
			PdfDocument document = new PdfDocument(reader, writer);
			PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
			Barcode1D barcode = new BarcodeEAN(document);
			barcode.SetCodeType(BarcodeEAN.EAN8);
			barcode.SetCode("97819351");
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
		[NUnit.Framework.Test]
		public virtual void Barcode03Test()
		{
			String filename = "barcodeEANSUP.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			BarcodeEAN codeEAN = new BarcodeEAN(document);
			codeEAN.SetCodeType(BarcodeEAN.EAN13);
			codeEAN.SetCode("9781935182610");
			BarcodeEAN codeSUPP = new BarcodeEAN(document);
			codeSUPP.SetCodeType(BarcodeEAN.SUPP5);
			codeSUPP.SetCode("55999");
			codeSUPP.SetBaseline(-2);
			BarcodeEANSUPP eanSupp = new BarcodeEANSUPP(codeEAN, codeSUPP);
			eanSupp.PlaceBarcode(canvas, null, iTextSharp.Kernel.Color.Color.BLUE);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
