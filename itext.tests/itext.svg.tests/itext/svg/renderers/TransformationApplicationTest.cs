using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers {
    public class TransformationApplicationTest {
        [NUnit.Framework.Test]
        public virtual void NormalDrawTest() {
            // translates to "1 0 0 1 10 0 cm" followed by a newline
            byte[] expected = new byte[] { 49, 32, 48, 32, 48, 32, 49, 32, 49, 48, 32, 48, 32, 99, 109, 10 };
            ISvgNodeRenderer nodeRenderer = new _AbstractSvgNodeRenderer_28();
            // do nothing
            IDictionary<String, String> attributeMap = new Dictionary<String, String>();
            attributeMap.Put(SvgTagConstants.TRANSFORM, "translate(10)");
            nodeRenderer.SetAttributesAndStyles(attributeMap);
            SvgDrawContext context = new SvgDrawContext();
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            context.PushCanvas(canvas);
            nodeRenderer.Draw(context);
            byte[] actual = canvas.GetContentStream().GetBytes(true);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        private sealed class _AbstractSvgNodeRenderer_28 : AbstractSvgNodeRenderer {
            public _AbstractSvgNodeRenderer_28() {
            }

            protected internal override void DoDraw(SvgDrawContext context) {
            }
        }
    }
}
