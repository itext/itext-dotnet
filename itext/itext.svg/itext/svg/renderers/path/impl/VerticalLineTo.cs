using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements lineTo(V) attribute of SVG's path element</summary>
    public class VerticalLineTo : OneDimensionalLineTo {
        public override void Draw(PdfCanvas canvas) {
            float minY = GetCoordinate(properties, MINIMUM_CHANGING_DIMENSION_VALUE);
            float maxY = GetCoordinate(properties, MAXIMUM_CHANGING_DIMENSION_VALUE);
            float endY = GetCoordinate(properties, ENDING_CHANGING_DIMENSION_VALUE);
            float x = GetCoordinate(properties, CURRENT_NONCHANGING_DIMENSION_VALUE);
            canvas.LineTo(x, maxY);
            canvas.LineTo(x, minY);
            canvas.LineTo(x, endY);
        }

        public override Point GetEndingPoint() {
            float y = GetSvgCoordinate(properties, ENDING_CHANGING_DIMENSION_VALUE);
            float x = GetSvgCoordinate(properties, CURRENT_NONCHANGING_DIMENSION_VALUE);
            return new Point(x, y);
        }
    }
}
