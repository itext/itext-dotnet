/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;polygon&gt; tag.
    /// </summary>
    public class PolygonSvgNodeRenderer : PolylineSvgNodeRenderer, IMarkerCapable {
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
            if (JavaUtil.DoubleCompare(start.x, end.x) != 0 || JavaUtil.DoubleCompare(start.y, end.y) != 0) {
                points.Add(new Point(start.x, start.y));
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            PolygonSvgNodeRenderer copy = new PolygonSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
