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
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Draw {
    [NUnit.Framework.Category("UnitTest")]
    public class DottedLineTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultDottedLineTest01() {
            DottedLine dottedLine = new DottedLine();
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dottedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(1, dottedLine.GetLineWidth(), 0.0001);
            NUnit.Framework.Assert.AreEqual(4, dottedLine.GetGap(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DottedLineWithSetWidthTest01() {
            DottedLine dottedLine = new DottedLine(20);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dottedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(4, dottedLine.GetGap(), 0.0001);
            NUnit.Framework.Assert.AreEqual(20, dottedLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DottedLineWithSetWidthAndGapTest01() {
            DottedLine dottedLine = new DottedLine(10, 15);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dottedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(10, dottedLine.GetLineWidth(), 0.0001);
            NUnit.Framework.Assert.AreEqual(15, dottedLine.GetGap(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DottedLineSettersTest01() {
            DottedLine dottedLine = new DottedLine(15);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dottedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(15, dottedLine.GetLineWidth(), 0.0001);
            NUnit.Framework.Assert.AreEqual(4, dottedLine.GetGap(), 0.0001);
            dottedLine.SetColor(ColorConstants.RED);
            NUnit.Framework.Assert.AreEqual(ColorConstants.RED, dottedLine.GetColor());
            dottedLine.SetLineWidth(10);
            NUnit.Framework.Assert.AreEqual(10, dottedLine.GetLineWidth(), 0.0001);
            dottedLine.SetGap(5);
            NUnit.Framework.Assert.AreEqual(5, dottedLine.GetGap(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DottedLineDrawTest01() {
            String expectedContent = "q\n" + "15 w\n" + "0 0 0 RG\n" + "[0 5] 2.5 d\n" + "1 J\n" + "100 107.5 m\n" + "200 107.5 l\n"
                 + "S\n" + "Q\n";
            PdfDocument tempDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(tempDoc.AddNewPage());
            DottedLine dottedLine = new DottedLine(15, 5);
            dottedLine.Draw(canvas, new Rectangle(100, 100, 100, 100));
            byte[] writtenContentBytes = canvas.GetContentStream().GetBytes();
            NUnit.Framework.Assert.AreEqual(expectedContent.GetBytes(), writtenContentBytes);
        }
    }
}
