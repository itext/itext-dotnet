using System;
using iText.Kernel.Geom;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;polygon&gt; tag.
    /// </summary>
    public class PolygonSvgNodeRenderer : PolylineSvgNodeRenderer {
        /// <summary>
        /// Calls setPoints(String) to set
        /// <see cref="PolylineSvgNodeRenderer.points"/>
        /// Then calls
        /// <see cref="ConnectPoints()"/>
        /// to create a path between the first and last point if it doesn't already exist
        /// </summary>
        protected internal override void SetPoints(String pointsAttribute) {
            base.SetPoints(pointsAttribute);
            ConnectPoints();
        }

        /// <summary>
        /// Appends the starting point to the end of
        /// <see cref="PolylineSvgNodeRenderer.points"/>
        /// if it is not already there.
        /// </summary>
        private void ConnectPoints() {
            if (points.Count < 2) {
                return;
            }
            Point start = points[0];
            Point end = points[points.Count - 1];
            if (start.x != end.x && start.y != end.y) {
                points.Add(new Point(start.x, start.y));
            }
        }
    }
}
