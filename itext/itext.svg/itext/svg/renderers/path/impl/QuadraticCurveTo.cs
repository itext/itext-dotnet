using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements curveTo(L) attribute of SVG's path element</summary>
    public class QuadraticCurveTo : AbstractPathShape {
        internal IDictionary<String, String> properties;

        /// <summary>Draws a quadratic Bézier curve from the current point to (x,y) using (x1,y1) as the control point
        ///     </summary>
        public override void Draw(PdfCanvas canvas) {
            canvas.CurveTo(GetCoordinate(properties, SvgTagConstants.X1), GetCoordinate(properties, SvgTagConstants.Y1
                ), GetCoordinate(properties, SvgTagConstants.X), GetCoordinate(properties, SvgTagConstants.Y));
        }

        public override void SetProperties(IDictionary<String, String> properties) {
            this.properties = properties;
        }

        public override void SetCoordinates(String[] coordinates) {
            IDictionary<String, String> map = new Dictionary<String, String>();
            map.Put("x1", coordinates.Length > 0 && !String.IsNullOrEmpty(coordinates[0]) ? coordinates[0] : "0");
            map.Put("y1", coordinates.Length > 1 && !String.IsNullOrEmpty(coordinates[1]) ? coordinates[1] : "0");
            map.Put("x", coordinates.Length > 2 && !String.IsNullOrEmpty(coordinates[2]) ? coordinates[2] : "0");
            map.Put("y", coordinates.Length > 3 && !String.IsNullOrEmpty(coordinates[3]) ? coordinates[3] : "0");
            SetProperties(map);
        }
    }
}
