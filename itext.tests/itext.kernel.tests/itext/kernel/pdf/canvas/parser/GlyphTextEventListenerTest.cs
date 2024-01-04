/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    public class GlyphTextEventListenerTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/GlyphTextEventListenerTest/";

        [NUnit.Framework.Test]
        public virtual void Test01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test.pdf"));
            float x1;
            float y1;
            float x2;
            float y2;
            x1 = 203;
            x2 = 21;
            y1 = 749;
            y2 = 49;
            String extractedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new GlyphTextEventListener
                (new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(new Rectangle
                (x1, y1, x2, y2)))));
            NUnit.Framework.Assert.AreEqual("1234\nt5678", extractedText);
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "Sample.pdf"));
            String extractedText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new GlyphTextEventListener
                (new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(new Rectangle
                (111, 855, 25, 12)))));
            NUnit.Framework.Assert.AreEqual("Your ", extractedText);
        }

        [NUnit.Framework.Test]
        public virtual void TestWithMultiFilteredRenderListener() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test.pdf"));
            float x1;
            float y1;
            float x2;
            float y2;
            FilteredEventListener listener = new FilteredEventListener();
            x1 = 122;
            x2 = 22;
            y1 = 678.9f;
            y2 = 12;
            ITextExtractionStrategy region1Listener = listener.AttachEventListener(new LocationTextExtractionStrategy(
                ), new TextRegionEventFilter(new Rectangle(x1, y1, x2, y2)));
            x1 = 156;
            x2 = 13;
            y1 = 678.9f;
            y2 = 12;
            ITextExtractionStrategy region2Listener = listener.AttachEventListener(new LocationTextExtractionStrategy(
                ), new TextRegionEventFilter(new Rectangle(x1, y1, x2, y2)));
            PdfCanvasProcessor parser = new PdfCanvasProcessor(new GlyphEventListener(listener));
            parser.ProcessPageContent(pdfDocument.GetPage(1));
            NUnit.Framework.Assert.AreEqual("Your", region1Listener.GetResultantText());
            NUnit.Framework.Assert.AreEqual("dju", region2Listener.GetResultantText());
        }
    }
}
