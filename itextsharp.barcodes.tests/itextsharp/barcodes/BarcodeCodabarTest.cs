using System;
using NUnit.Framework;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Barcodes
{
	public class BarcodeCodabarTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/barcodes/";

		public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
			 + "/test/itextsharp/barcodes/Codabar/";

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
