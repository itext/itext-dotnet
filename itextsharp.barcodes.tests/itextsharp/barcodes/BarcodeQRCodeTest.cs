using System;
using System.Collections.Generic;
using NUnit.Framework;
using com.itextpdf.barcodes.qrcode;
using com.itextpdf.kernel.color;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.pdf.canvas;
using com.itextpdf.kernel.utils;
using java.io;

namespace com.itextpdf.barcodes
{
	public class BarcodeQRCodeTest
	{
		public const String sourceFolder = "./src/test/resources/com/itextpdf/barcodes/";

		public const String destinationFolder = "./target/test/com/itextpdf/barcodes/BarcodeQRCode/";

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
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode01Test()
		{
			String filename = "barcodeQRCode01.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object
				>();
			hints[EncodeHintType.ERROR_CORRECTION] = ErrorCorrectionLevel.L;
			BarcodeQRCode barcode = new BarcodeQRCode("some specific text 239214 hello world"
				);
			barcode.PlaceBarcode(canvas, Color.GRAY, 12);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode02Test()
		{
			String filename = "barcodeQRCode02.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page1 = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page1);
			IDictionary<EncodeHintType, Object> hints = new Dictionary<EncodeHintType, Object
				>();
			hints[EncodeHintType.CHARACTER_SET] = "UTF-8";
			BarcodeQRCode barcode1 = new BarcodeQRCode("дима", hints);
			barcode1.PlaceBarcode(canvas, Color.GRAY, 12);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
