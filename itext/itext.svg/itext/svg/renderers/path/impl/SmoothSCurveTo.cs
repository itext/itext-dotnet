using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements shorthand/smooth curveTo (S) attribute of SVG's path element</summary>
    public class SmoothSCurveTo : AbstractPathShape {
        internal IDictionary<String, String> properties;

        /// <summary>Draws a quadratic Bézier curve from the current point to (x,y)</summary>
        public override void Draw(PdfCanvas canvas) {
            canvas.CurveTo(GetCoordinate(properties, SvgTagConstants.X2), GetCoordinate(properties, SvgTagConstants.Y2
                ), GetCoordinate(properties, SvgTagConstants.X), GetCoordinate(properties, SvgTagConstants.Y));
        }

        public override void SetProperties(IDictionary<String, String> properties) {
            this.properties = properties;
        }

        public override void SetCoordinates(String[] coordinates) {
            IDictionary<String, String> map = new Dictionary<String, String>();
            map.Put("x2", coordinates.Length > 0 && !String.IsNullOrEmpty(coordinates[0]) ? coordinates[0] : "0");
            map.Put("y2", coordinates.Length > 1 && !String.IsNullOrEmpty(coordinates[1]) ? coordinates[1] : "0");
            map.Put("x", coordinates.Length > 2 && !String.IsNullOrEmpty(coordinates[2]) ? coordinates[2] : "0");
            map.Put("y", coordinates.Length > 3 && !String.IsNullOrEmpty(coordinates[3]) ? coordinates[3] : "0");
            SetProperties(map);
        }
    }
}
