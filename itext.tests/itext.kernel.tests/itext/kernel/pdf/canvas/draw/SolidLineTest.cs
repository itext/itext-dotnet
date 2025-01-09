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
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Draw {
    [NUnit.Framework.Category("UnitTest")]
    public class SolidLineTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultSolidLineTest01() {
            SolidLine solidLine = new SolidLine();
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, solidLine.GetColor());
            NUnit.Framework.Assert.AreEqual(1, solidLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void SolidLineWithSetWidthTest01() {
            SolidLine solidLine = new SolidLine(20);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, solidLine.GetColor());
            NUnit.Framework.Assert.AreEqual(20, solidLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void SolidLineSettersTest01() {
            SolidLine solidLine = new SolidLine(15);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, solidLine.GetColor());
            NUnit.Framework.Assert.AreEqual(15, solidLine.GetLineWidth(), 0.0001);
            solidLine.SetColor(ColorConstants.RED);
            NUnit.Framework.Assert.AreEqual(ColorConstants.RED, solidLine.GetColor());
            solidLine.SetLineWidth(10);
            NUnit.Framework.Assert.AreEqual(10, solidLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void SolidLineDrawTest01() {
            String expectedContent = "q\n" + "0 0 0 RG\n" + "15 w\n" + "100 107.5 m\n" + "200 107.5 l\n" + "S\n" + "Q\n";
            PdfDocument tempDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(tempDoc.AddNewPage());
            SolidLine solidLine = new SolidLine(15);
            solidLine.Draw(canvas, new Rectangle(100, 100, 100, 100));
            byte[] writtenContentBytes = canvas.GetContentStream().GetBytes();
            NUnit.Framework.Assert.AreEqual(expectedContent.GetBytes(), writtenContentBytes);
        }
    }
}
