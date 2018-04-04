using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements closePath(Z) attribute of SVG's path element</summary>
    public class ClosePath : AbstractPathShape {
        public override void Draw(PdfCanvas canvas) {
            canvas.ClosePathStroke();
        }

        public override void SetProperties(IDictionary<String, String> properties) {
        }

        public override void SetCoordinates(String[] coordinates) {
        }
    }
}
