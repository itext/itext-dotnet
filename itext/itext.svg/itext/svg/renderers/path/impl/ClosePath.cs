using iText.Kernel.Geom;
using iText.Svg;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements closePath(Z) attribute of SVG's path element</summary>
    public class ClosePath : LineTo {
        public override Point GetEndingPoint() {
            float x = GetSvgCoordinate(properties, SvgConstants.Attributes.X);
            float y = GetSvgCoordinate(properties, SvgConstants.Attributes.Y);
            return new Point(x, y);
        }
    }
}
