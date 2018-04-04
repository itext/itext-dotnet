using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path {
    public interface IPathShape {
        void Draw(PdfCanvas canvas);

        void SetProperties(IDictionary<String, String> properties);

        void SetCoordinates(String[] coordinates);
    }
}
