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
    public class DashedLineTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultDashedLineTest01() {
            DashedLine dashedLine = new DashedLine();
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dashedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(1, dashedLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DashedLineWithSetWidthTest01() {
            DashedLine dashedLine = new DashedLine(20);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dashedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(20, dashedLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DashedLineSettersTest01() {
            DashedLine dashedLine = new DashedLine(15);
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, dashedLine.GetColor());
            NUnit.Framework.Assert.AreEqual(15, dashedLine.GetLineWidth(), 0.0001);
            dashedLine.SetColor(ColorConstants.RED);
            NUnit.Framework.Assert.AreEqual(ColorConstants.RED, dashedLine.GetColor());
            dashedLine.SetLineWidth(10);
            NUnit.Framework.Assert.AreEqual(10, dashedLine.GetLineWidth(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void DashedLineDrawTest01() {
            String expectedContent = "q\n" + "15 w\n" + "0 0 0 RG\n" + "[2] 2 d\n" + "100 107.5 m\n" + "200 107.5 l\n"
                 + "S\n" + "Q\n";
            PdfDocument tempDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(tempDoc.AddNewPage());
            DashedLine dashedLine = new DashedLine(15);
            dashedLine.Draw(canvas, new Rectangle(100, 100, 100, 100));
            byte[] writtenContentBytes = canvas.GetContentStream().GetBytes();
            NUnit.Framework.Assert.AreEqual(expectedContent.GetBytes(), writtenContentBytes);
        }
    }
}
