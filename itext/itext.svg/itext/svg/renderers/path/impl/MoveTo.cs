using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements moveTo(M) attribute of SVG's path element</summary>
    public class MoveTo : AbstractPathShape {
        internal IDictionary<String, String> properties;

        public override void Draw(PdfCanvas canvas) {
            canvas.MoveTo(GetCoordinate(properties, SvgTagConstants.X), GetCoordinate(properties, SvgTagConstants.Y));
        }

        public override void SetProperties(IDictionary<String, String> properties) {
            this.properties = properties;
        }

        public override void SetCoordinates(String[] coordinates) {
            IDictionary<String, String> map = new Dictionary<String, String>();
            map.Put("x", coordinates.Length > 0 && !String.IsNullOrEmpty(coordinates[0]) ? coordinates[0] : "0");
            map.Put("y", coordinates.Length > 1 && !String.IsNullOrEmpty(coordinates[1]) ? coordinates[1] : "0");
            SetProperties(map);
        }
    }
}
