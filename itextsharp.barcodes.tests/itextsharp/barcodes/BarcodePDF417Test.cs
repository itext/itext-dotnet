using System;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Barcodes
{
	public class BarcodePDF417Test : ExtendedITextTest
	{
		public static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/barcodes/";

		public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
			 + "/test/itextsharp/barcodes/BarcodePDF417/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void Barcode01Test()
		{
			String filename = "barcode417_01.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfDocument document = new PdfDocument(writer);
			PdfPage page = document.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(page);
			String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
				 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
			BarcodePDF417 barcode = new BarcodePDF417();
			barcode.SetCode(text);
			barcode.PlaceBarcode(canvas, null);
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
			String filename = "barcode417_02.pdf";
			PdfWriter writer = new PdfWriter(destinationFolder + filename);
			PdfReader reader = new PdfReader(sourceFolder + "DocumentWithTrueTypeFont1.pdf");
			PdfDocument document = new PdfDocument(reader, writer);
			PdfCanvas canvas = new PdfCanvas(document.GetLastPage());
			String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
				 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
			BarcodePDF417 barcode = new BarcodePDF417();
			barcode.SetCode(text);
			barcode.PlaceBarcode(canvas, null);
			document.Close();
			NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder
				 + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
		}
	}
}
