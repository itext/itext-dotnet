/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers {
    /// <summary>Interface implemented by elements that support marker drawing.</summary>
    /// <remarks>
    /// Interface implemented by elements that support marker drawing.
    /// Defines methods for working with markers.
    /// </remarks>
    public interface IMarkerCapable {
        /// <summary>Draws a marker in the specified context.</summary>
        /// <remarks>
        /// Draws a marker in the specified context.
        /// The marker is drawn on the vertices defined according to the given marker type.
        /// </remarks>
        /// <param name="context">the object that knows the place to draw this element and maintains its state</param>
        /// <param name="markerVertexType">
        /// type of marker that determine on which vertices of the given element
        /// marker should  be drawn
        /// </param>
        void DrawMarker(SvgDrawContext context, MarkerVertexType markerVertexType);

        /// <summary>
        /// Calculates marker orientation angle if
        /// <c>orient</c>
        /// attribute is set to
        /// <c>auto</c>
        /// </summary>
        /// <param name="marker">marker for which the rotation angle should be calculated</param>
        /// <param name="reverse">indicates that the resulting angle should be rotated 180 degrees</param>
        /// <returns>
        /// marker orientation angle so that its positive x-axis is pointing in the direction of the path at the
        /// point it is placed
        /// </returns>
        double GetAutoOrientAngle(MarkerSvgNodeRenderer marker, bool reverse);
    }
}
