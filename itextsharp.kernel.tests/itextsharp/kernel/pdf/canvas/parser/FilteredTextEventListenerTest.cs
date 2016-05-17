using System;
using NUnit.Framework;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Filter;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Listener;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf.Canvas.Parser
{
	public class FilteredTextEventListenerTest : ExtendedITextTest
	{
		private const String sourceFolder = "../../resources/itextsharp/kernel/parser/FilteredTextEventListenerTest/";

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestRegion()
		{
			PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "in.pdf"));
			float pageHeight = doc.GetPage(1).GetPageSize().GetHeight();
			Rectangle upperLeft = new Rectangle(0, (int)pageHeight - 30, 250, (int)pageHeight
				);
			NUnit.Framework.Assert.IsTrue(TextIsInRectangle(doc, "Upper Left", upperLeft));
			NUnit.Framework.Assert.IsFalse(TextIsInRectangle(doc, "Upper Right", upperLeft));
		}

		/// <exception cref="System.Exception"/>
		private bool TextIsInRectangle(PdfDocument doc, String text, Rectangle rect)
		{
			FilteredTextEventListener filterListener = new FilteredTextEventListener(new LocationTextExtractionStrategy
				(), new TextRegionEventFilter(rect));
			String extractedText = PdfTextExtractor.GetTextFromPage(doc.GetPage(1), filterListener
				);
			return extractedText.Equals(text);
		}
	}
}
