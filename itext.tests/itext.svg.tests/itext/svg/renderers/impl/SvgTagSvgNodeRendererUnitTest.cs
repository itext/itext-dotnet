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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgTagSvgNodeRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CalculateNestedViewportSameAsParentTest() {
            Rectangle expected = new Rectangle(0, 0, 600, 600);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().SetCompressionLevel
                (0)));
            document.AddNewPage();
            PdfFormXObject pdfForm = new PdfFormXObject(expected);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            context.AddViewPort(expected);
            SvgTagSvgNodeRenderer parent = new SvgTagSvgNodeRenderer();
            parent.SetAttributesAndStyles(new Dictionary<String, String>());
            SvgTagSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            renderer.SetParent(parent);
            Rectangle actual = renderer.CalculateViewPort(context);
            NUnit.Framework.Assert.IsTrue(expected.EqualsWithEpsilon(actual));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsOtherObjectNegativeTest() {
            SvgTagSvgNodeRenderer one = new SvgTagSvgNodeRenderer();
            CircleSvgNodeRenderer two = new CircleSvgNodeRenderer();
            NUnit.Framework.Assert.IsFalse(one.Equals(two));
        }

        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            SvgTagSvgNodeRenderer renderer = new SvgTagSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }
    }
}
