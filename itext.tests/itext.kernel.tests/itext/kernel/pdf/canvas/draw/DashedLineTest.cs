/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
