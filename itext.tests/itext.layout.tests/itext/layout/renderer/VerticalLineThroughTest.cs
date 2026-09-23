/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class VerticalLineThroughTest : ExtendedITextTest {
        private const float EPS = 0.001f;

        public static ICollection<Object[]> Parameters() {
            ICollection<Object[]> parameters = new List<Object[]>();
            foreach (float fontSize in new float[] { 12, 24, 48 }) {
                foreach (float textRise in new float[] { -6, 0, 6 }) {
                    parameters.Add(new Object[] { fontSize, textRise });
                }
            }
            return parameters;
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void DefaultLineThroughPositionTest(float fontSize, float textRise) {
            Text text = new Text("ABC").SetFontSize(fontSize).SetTextRise(textRise).SetLineThrough();
            AssertDecorationPosition(text, fontSize, 0.5f);
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void CustomUnderlinePositionIsPreservedTest(float fontSize, float textRise) {
            Text text = new Text("ABC").SetFontSize(fontSize).SetTextRise(textRise).SetUnderline(null, .75f, 0, 0, 7 /
                 24f, PdfCanvasConstants.LineCapStyle.BUTT);
            AssertDecorationPosition(text, fontSize, 7 / 24f);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalPositionDoesNotChangeHorizontalPositionTest() {
            Underline underline = new Underline(null, 1, 0, 2, .25f, PdfCanvasConstants.LineCapStyle.BUTT);
            NUnit.Framework.Assert.AreEqual(8, underline.GetXPosition(24), EPS);
            NUnit.Framework.Assert.AreSame(underline, underline.SetXPosition(3, .5f));
            NUnit.Framework.Assert.AreEqual(15, underline.GetXPosition(24), EPS);
            NUnit.Framework.Assert.AreEqual(8, underline.GetYPosition(24), EPS);
            NUnit.Framework.Assert.AreEqual(.25f, underline.GetYPositionMul(), EPS);
        }

        private static void AssertDecorationPosition(Text text, float fontSize, float expectedMultiplier) {
            using (Document document = new Document(new PdfDocument(new PdfWriter(new MemoryStream())))) {
                LineRenderer line = new LineRenderer();
                line.SetParent(document.GetRenderer());
                line.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                line.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                line.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                line.AddChild(new Text("Base").SetFontSize(fontSize).CreateRendererSubTree());
                TextRenderer renderer = (TextRenderer)text.CreateRendererSubTree();
                line.AddChild(renderer);
                NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, line.Layout(new LayoutContext(new LayoutArea(1, new Rectangle
                    (100, 100, 400, 400)))).GetStatus());
                VerticalLineThroughTest.RecordingCanvas canvas = new VerticalLineThroughTest.RecordingCanvas(document.GetPdfDocument
                    ());
                Underline underline = text.GetProperty<Underline>(Property.UNDERLINE);
                renderer.DrawSingleUnderline(underline, new TransparentColor(ColorConstants.BLACK), canvas, fontSize, 0);
                Rectangle actual = canvas.rectangle;
                Rectangle inner = renderer.GetInnerAreaBBox();
                Rectangle occupied = renderer.GetOccupiedAreaBBox();
                NUnit.Framework.Assert.AreEqual(occupied.GetX() + occupied.GetWidth() * expectedMultiplier, actual.GetX() 
                    + actual.GetWidth() / 2, EPS);
                NUnit.Framework.Assert.AreEqual(inner.GetY(), actual.GetY(), EPS);
                NUnit.Framework.Assert.AreEqual(inner.GetHeight(), actual.GetHeight(), EPS);
                NUnit.Framework.Assert.AreEqual(.75f, actual.GetWidth(), EPS);
            }
        }

        private class RecordingCanvas : PdfCanvas {
//\cond DO_NOT_DOCUMENT
            internal iText.Kernel.Geom.Rectangle rectangle;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal RecordingCanvas(PdfDocument document)
                : base(document.AddNewPage()) {
            }
//\endcond

            public override PdfCanvas Rectangle(iText.Kernel.Geom.Rectangle rectangle) {
                this.rectangle = rectangle.Clone();
                return base.Rectangle(rectangle);
            }
        }
    }
}
