using System;
using NUnit.Framework;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.pdf.canvas;
using com.itextpdf.kernel.utils;
using java.io;

namespace com.itextpdf.barcodes
{
	public class BarcodeCodabarTest
	{
		public const String sourceFolder = "./src/test/resources/com/itextpdf/barcodes/";

		public const String destinationFolder = "./target/test/com/itextpdf/barcodes/Codabar/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			new File(destinationFolder).Mkdirs();
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode01Test()
		{
			String filename = "codabar.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			BarcodeCodabar codabar = new BarcodeCodabar(document);
			codabar.SetCode("A123A");
			codabar.SetStartStopText(true);
			codabar.PlaceBarcode(canvas, null, null);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
