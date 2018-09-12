using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements lineTo(H) attribute of SVG's path element</summary>
    public class HorizontalLineTo : OneDimensionalLineTo {
        public override void Draw(PdfCanvas canvas) {
            float minX = GetCoordinate(properties, MINIMUM_CHANGING_DIMENSION_VALUE);
            float maxX = GetCoordinate(properties, MAXIMUM_CHANGING_DIMENSION_VALUE);
            float endX = GetCoordinate(properties, ENDING_CHANGING_DIMENSION_VALUE);
            float y = GetCoordinate(properties, CURRENT_NONCHANGING_DIMENSION_VALUE);
            canvas.LineTo(maxX, y);
            canvas.LineTo(minX, y);
            canvas.LineTo(endX, y);
        }

        public override Point GetEndingPoint() {
            float y = GetSvgCoordinate(properties, CURRENT_NONCHANGING_DIMENSION_VALUE);
            float x = GetSvgCoordinate(properties, ENDING_CHANGING_DIMENSION_VALUE);
            return new Point(x, y);
        }
    }
}
