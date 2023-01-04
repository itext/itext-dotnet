/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("UnitTest")]
    public class CanvasUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CanvasImmediateFlushConstructorTest() {
            PdfDocument pdf = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = pdf.AddNewPage();
            PdfCanvas pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdf);
            Rectangle rectangle = new Rectangle(0, 0);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, rectangle, false);
            NUnit.Framework.Assert.AreEqual(pdfCanvas.GetDocument(), canvas.GetPdfDocument());
            NUnit.Framework.Assert.IsFalse(canvas.immediateFlush);
        }
    }
}
