/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Layout.Renderer {
    public class AbstractRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreateXObjectTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_LEFT).AddColorStop(new GradientColorStop
                (ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE));
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfXObject pdfXObject = AbstractRenderer.CreateXObject(gradientBuilder, new Rectangle(0, 0, 20, 20), pdfDocument
                );
            NUnit.Framework.Assert.IsNotNull(pdfXObject.GetPdfObject().Get(PdfName.Resources));
        }

        [NUnit.Framework.Test]
        public virtual void CreateXObjectWithNullLinearGradientTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfXObject pdfXObject = AbstractRenderer.CreateXObject(null, new Rectangle(0, 0, 20, 20), pdfDocument);
            NUnit.Framework.Assert.IsNull(pdfXObject.GetPdfObject().Get(PdfName.Resources));
        }

        [NUnit.Framework.Test]
        public virtual void CreateXObjectWithInvalidColorTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfXObject pdfXObject = AbstractRenderer.CreateXObject(gradientBuilder, new Rectangle(0, 0, 20, 20), pdfDocument
                );
            NUnit.Framework.Assert.IsNull(pdfXObject.GetPdfObject().Get(PdfName.Resources));
        }
    }
}
