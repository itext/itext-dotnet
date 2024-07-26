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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("UnitTest")]
    public class TransformationApplicationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NormalDrawTest() {
            byte[] expected = "1 0 0 1 7.5 0 cm\n0 0 0 rg\nf\n".GetBytes(System.Text.Encoding.UTF8);
            ISvgNodeRenderer nodeRenderer = new _AbstractSvgNodeRenderer_50();
            // do nothing
            IDictionary<String, String> attributeMap = new Dictionary<String, String>();
            attributeMap.Put(SvgConstants.Attributes.TRANSFORM, "translate(10)");
            nodeRenderer.SetAttributesAndStyles(attributeMap);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            context.PushCanvas(canvas);
            nodeRenderer.Draw(context);
            byte[] actual = canvas.GetContentStream().GetBytes(true);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        private sealed class _AbstractSvgNodeRenderer_50 : AbstractSvgNodeRenderer {
            public _AbstractSvgNodeRenderer_50() {
            }

            public override ISvgNodeRenderer CreateDeepCopy() {
                return null;
            }

            public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
                return null;
            }

            protected internal override void DoDraw(SvgDrawContext context) {
            }
        }
    }
}
