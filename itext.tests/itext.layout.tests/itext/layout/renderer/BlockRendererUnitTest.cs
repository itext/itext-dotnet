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
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class BlockRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PAGE_WAS_FLUSHED_ACTION_WILL_NOT_BE_PERFORMED)]
        public virtual void ClippedAreaFlushedPageTest() {
            BlockRenderer blockRenderer = new DivRenderer(new Div());
            blockRenderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            blockRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 100));
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDocument.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            DrawContext context = new DrawContext(pdfDocument, pdfCanvas);
            pdfDocument.GetPage(1).Flush();
            blockRenderer.Draw(context);
            // This test checks that there is log message and there is no NPE so assertions are not required
            NUnit.Framework.Assert.IsTrue(true);
        }
    }
}
