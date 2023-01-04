/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
