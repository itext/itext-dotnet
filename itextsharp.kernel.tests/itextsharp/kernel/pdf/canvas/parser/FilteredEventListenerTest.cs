using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Filter;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Listener;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf.Canvas.Parser
{
	public class FilteredEventListenerTest : ExtendedITextTest
	{
		private static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/kernel/parser/FilteredEventListenerTest/";

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void Test()
		{
			PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder
				 + "test.pdf", FileMode.Open, FileAccess.Read)));
			String[] expectedText = new String[] { "PostScript Compatibility", "Because the PostScript language does not support the transparent imaging \n"
				 + "model, PDF 1.4 consumer applications must have some means for converting the \n"
				 + "appearance of a document that uses transparency to a purely opaque description \n"
				 + "for printing on PostScript output devices. Similar techniques can also be used to \n"
				 + "convert such documents to a form that can be correctly viewed by PDF 1.3 and \n"
				 + "earlier consumers. ", "Otherwise, flatten the colors to some assumed device color space with pre-\n"
				 + "determined calibration. In the generated PostScript output, paint the flattened \n"
				 + "colors in a CIE-based color space having that calibration. " };
			Rectangle[] regions = new Rectangle[] { new Rectangle(90, 581, 130, 24), new Rectangle
				(80, 486, 370, 92), new Rectangle(103, 143, 357, 53) };
			TextRegionEventFilter[] regionFilters = new TextRegionEventFilter[regions.Length]
				;
			for (int i = 0; i < regions.Length; i++)
			{
				regionFilters[i] = new TextRegionEventFilter(regions[i]);
			}
			FilteredEventListener listener = new FilteredEventListener();
			LocationTextExtractionStrategy[] extractionStrategies = new LocationTextExtractionStrategy
				[regions.Length];
			for (int i_1 = 0; i_1 < regions.Length; i_1++)
			{
				extractionStrategies[i_1] = listener.AttachEventListener(new LocationTextExtractionStrategy
					(), regionFilters[i_1]);
			}
			new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(1));
			for (int i_2 = 0; i_2 < regions.Length; i_2++)
			{
				String actualText = extractionStrategies[i_2].GetResultantText();
				NUnit.Framework.Assert.AreEqual(expectedText[i_2], actualText);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		[Test]
		public virtual void MultipleFiltersForOneRegionTest()
		{
			PdfDocument pdfDocument = new PdfDocument(new PdfReader(new FileStream(sourceFolder
				 + "test.pdf", FileMode.Open, FileAccess.Read)));
			Rectangle[] regions = new Rectangle[] { new Rectangle(0, 0, 500, 650), new Rectangle
				(0, 0, 400, 400), new Rectangle(200, 200, 300, 400), new Rectangle(100, 100, 350
				, 300) };
			TextRegionEventFilter[] regionFilters = new TextRegionEventFilter[regions.Length]
				;
			for (int i = 0; i < regions.Length; i++)
			{
				regionFilters[i] = new TextRegionEventFilter(regions[i]);
			}
			FilteredEventListener listener = new FilteredEventListener();
			LocationTextExtractionStrategy extractionStrategy = listener.AttachEventListener(
				new LocationTextExtractionStrategy(), regionFilters);
			new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(1));
			String actualText = extractionStrategy.GetResultantText();
			String expectedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new 
				FilteredTextEventListener(new LocationTextExtractionStrategy(), regionFilters));
			NUnit.Framework.Assert.AreEqual(expectedText, actualText);
		}
	}
}
