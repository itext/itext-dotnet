/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
    public class IndicTextExtractionTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/IndicTextExtractionTest/";

        [NUnit.Framework.Test]
        public virtual void Test01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test01.pdf"));
            String[] expectedText = new String[] { "\u0928\u093F\u0930\u094D\u0935\u093E\u091A\u0915", "\u0928\u0917\u0930\u0928\u093F\u0917\u092E / "
                 + "\u0928\u0917\u0930\u092A\u0930\u093F\u0937\u0926" + " / \u0928\u0917\u0930\u092A\u093E\u0932\u093F\u0915\u093E \u0915\u093E \u0928\u093E\u092E"
                , "\u0935 " + "\u0938\u0902\u0916\u094D\u092F\u093E", "\u0938\u0902\u0915\u094D\u0937\u093F\u092A\u094D\u0924 \u092A\u0941\u0928\u0930\u0940\u0915\u094D\u0937\u0923"
                , "\u092E\u0924\u0926\u093E\u0928 " + "\u0915\u0947\u0928\u094D\u0926\u094D\u0930" + "\u0915\u093E", "\u0906\u0930\u0902\u092D\u093F\u0915 "
                 + "\u0915\u094D\u0930\u092E\u0938\u0902\u0916\u094D\u092F\u093E" };
            Rectangle[] regions = new Rectangle[] { new Rectangle(30, 779, 45, 20), new Rectangle(30, 745, 210, 20), new 
                Rectangle(30, 713, 42, 20), new Rectangle(30, 679, 80, 20), new Rectangle(30, 647, 73, 20), new Rectangle
                (30, 612, 93, 20) };
            TextRegionEventFilter[] regionFilters = new TextRegionEventFilter[regions.Length];
            for (int i = 0; i < regions.Length; i++) {
                regionFilters[i] = new TextRegionEventFilter(regions[i]);
            }
            FilteredEventListener listener = new FilteredEventListener();
            LocationTextExtractionStrategy[] extractionStrategies = new LocationTextExtractionStrategy[regions.Length]
                ;
            for (int i = 0; i < regions.Length; i++) {
                extractionStrategies[i] = listener.AttachEventListener(new LocationTextExtractionStrategy().SetUseActualText
                    (true), regionFilters[i]);
            }
            new PdfCanvasProcessor(listener).ProcessPageContent(pdfDocument.GetPage(1));
            for (int i = 0; i < regions.Length; i++) {
                String actualText = extractionStrategies[i].GetResultantText();
                NUnit.Framework.Assert.AreEqual(expectedText[i], actualText);
            }
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            String expectedText = "\u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940 \u090F\u0915 \u0932\u093F\u092A\u093F \u0939\u0948 \u091C\u093F\u0938\u092E\u0947\u0902 \u0905\u0928\u0947\u0915 \u092D\u093E\u0930\u0924\u0940\u092F \u092D\u093E\u0937\u093E\u090F\u0901 \u0924\u0925\u093E \u0915\u0941\u091B \u0935\u093F\u0926\u0947\u0936\u0940 \u092D\u093E\u0937\u093E\u090F\u0902 \u0932\u093F\u0916\u0940\u0902 \u091C\u093E\u0924\u0940 \u0939\u0948\u0902\u0964 \u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940 \u092C\u093E\u092F\u0947\u0902 \u0938\u0947 \u0926\u093E\u092F\u0947\u0902 \u0932\u093F\u0916\u0940"
                 + "\n" + "\u091C\u093E\u0924\u0940 \u0939\u0948, \u0907\u0938\u0915\u0940 \u092A\u0939\u091A\u093E\u0928 \u090F\u0915 \u0915\u094D\u0937\u0948\u0924\u093F\u091C \u0930\u0947\u0916\u093E \u0938\u0947 \u0939\u0948 \u091C\u093F\u0938\u0947 \'\u0936\u093F\u0930\u093F\u0930\u0947\u0916\u093E\' \u0915\u0939\u0924\u0947 \u0939\u0948\u0902\u0964 \u0938\u0902\u0938\u094D\u0915\u0943\u0924, \u092A\u093E\u0932\u093F, \u0939\u093F\u0928\u094D\u0926\u0940, \u092E\u0930\u093E\u0920\u0940, \u0915\u094B\u0902\u0915\u0923\u0940, \u0938\u093F\u0928\u094D\u0927\u0940,"
                 + "\n" + "\u0915\u0936\u094D\u092E\u0940\u0930\u0940, \u0921\u094B\u0917\u0930\u0940, \u0928\u0947\u092A\u093E\u0932\u0940, \u0928\u0947\u092A\u093E\u0932 \u092D\u093E\u0937\u093E (\u0924\u0925\u093E \u0905\u0928\u094D\u092F \u0928\u0947\u092A\u093E\u0932\u0940 \u0909\u092A\u092D\u093E\u0937\u093E\u090F\u0901), \u0924\u093E\u092E\u093E\u0919 \u092D\u093E\u0937\u093E, \u0917\u0922\u093C\u0935\u093E\u0932\u0940, \u092C\u094B\u0921\u094B, \u0905\u0902\u0917\u093F\u0915\u093E, \u092E\u0917\u0939\u0940, \u092D\u094B\u091C\u092A\u0941\u0930\u0940,"
                 + "\n" + "\u092E\u0948\u0925\u093F\u0932\u0940, \u0938\u0902\u0925\u093E\u0932\u0940 \u0906\u0926\u093F \u092D\u093E\u0937\u093E\u090F\u0901 \u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940 \u092E\u0947\u0902 \u0932\u093F\u0916\u0940 \u091C\u093E\u0924\u0940 \u0939\u0948\u0902\u0964 \u0907\u0938\u0915\u0947 \u0905\u0924\u093F\u0930\u093F\u0915\u094D\u0924 \u0915\u0941\u091B \u0938\u094D\u0925\u093F\u0924\u093F\u092F\u094B\u0902 \u092E\u0947\u0902 \u0917\u0941\u091C\u0930\u093E\u0924\u0940, \u092A\u0902\u091C\u093E\u092C\u0940, \u092C\u093F\u0937\u094D\u0923\u0941\u092A\u0941\u0930\u093F\u092F\u093E"
                 + "\n" + "\u092E\u0923\u093F\u092A\u0941\u0930\u0940, \u0930\u094B\u092E\u093E\u0928\u0940 \u0914\u0930 \u0909\u0930\u094D\u0926\u0942 \u092D\u093E\u0937\u093E\u090F\u0902 \u092D\u0940 \u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940 \u092E\u0947\u0902 \u0932\u093F\u0916\u0940 \u091C\u093E\u0924\u0940 \u0939\u0948\u0902\u0964";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test02.pdf"));
            String extractedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new LocationTextExtractionStrategy
                ().SetUseActualText(true));
            NUnit.Framework.Assert.AreEqual(expectedText, extractedText);
        }
    }
}
