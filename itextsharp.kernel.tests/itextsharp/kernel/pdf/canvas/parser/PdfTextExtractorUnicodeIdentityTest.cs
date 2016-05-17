using System;
using NUnit.Framework;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Filter;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Listener;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf.Canvas.Parser
{
	public class PdfTextExtractorUnicodeIdentityTest : ExtendedITextTest
	{
		private const String sourceFolder = "../../resources/itextsharp/kernel/parser/PdfTextExtractorUnicodeIdentityTest/";

		/// <exception cref="System.IO.IOException"/>
		[NUnit.Framework.Test]
		public virtual void Test()
		{
			PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "user10.pdf"
				));
			Rectangle rectangle = new Rectangle(71, 708, 154, 9);
			IEventFilter filter = new TextRegionEventFilter(rectangle);
			String txt = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new FilteredTextEventListener
				(new LocationTextExtractionStrategy(), filter));
			NUnit.Framework.Assert.AreEqual("Pname Dname Email Address", txt);
		}
	}
}
