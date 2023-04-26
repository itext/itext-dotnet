/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FilteredEventListenerTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/FilteredEventListenerTest/";

        [NUnit.Framework.Test]
        public virtual void Test() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test.pdf"));
            String[] expectedText = new String[] { "PostScript Compatibility", "Because the PostScript language does not support the transparent imaging \n"
                 + "model, PDF 1.4 consumer applications must have some means for converting the \n" + "appearance of a document that uses transparency to a purely opaque description \n"
                 + "for printing on PostScript output devices. Similar techniques can also be used to \n" + "convert such documents to a form that can be correctly viewed by PDF 1.3 and \n"
                 + "earlier consumers. ", "Otherwise, flatten the colors to some assumed device color space with pre-\n"
                 + "determined calibration. In the generated PostScript output, paint the flattened \n" + "colors in a CIE-based color space having that calibration. "
                 };
            Rectangle[] regions = new Rectangle[] { new Rectangle(90, 581, 130, 24), new Rectangle(80, 486, 370, 92), 
                new Rectangle(103, 143, 357, 53) };
            TextRegionEventFilter[] regionFilters = new TextRegionEventFilter[regions.Length];
            for (int i = 0; i < regions.Length; i++) {
                regionFilters[i] = new TextRegionEventFilter(regions[i]);
            }
            FilteredEventListener listener = new FilteredEventListener();
            LocationTextExtractionStrategy[] extractionStrategies = new LocationTextExtractionStrategy[regions.Length]
                ;
            for (int i = 0; i < regions.Length; i++) {
                extractionStrategies[i] = listener.AttachEventListener(new LocationTextExtractionStrategy(), regionFilters
                    [i]);
            }
            new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(1));
            for (int i = 0; i < regions.Length; i++) {
                String actualText = extractionStrategies[i].GetResultantText();
                NUnit.Framework.Assert.AreEqual(expectedText[i], actualText);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFiltersForOneRegionTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test.pdf"));
            Rectangle[] regions = new Rectangle[] { new Rectangle(0, 0, 500, 650), new Rectangle(0, 0, 400, 400), new 
                Rectangle(200, 200, 300, 400), new Rectangle(100, 100, 350, 300) };
            TextRegionEventFilter[] regionFilters = new TextRegionEventFilter[regions.Length];
            for (int i = 0; i < regions.Length; i++) {
                regionFilters[i] = new TextRegionEventFilter(regions[i]);
            }
            FilteredEventListener listener = new FilteredEventListener();
            LocationTextExtractionStrategy extractionStrategy = listener.AttachEventListener(new LocationTextExtractionStrategy
                (), regionFilters);
            new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(1));
            String actualText = extractionStrategy.GetResultantText();
            String expectedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new FilteredTextEventListener
                (new LocationTextExtractionStrategy(), regionFilters));
            NUnit.Framework.Assert.AreEqual(expectedText, actualText);
        }
    }
}
