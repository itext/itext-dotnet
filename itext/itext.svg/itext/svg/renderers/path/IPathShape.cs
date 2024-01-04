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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path {
    /// <summary>Interface for IPathShape, which draws the Path-data's d element instructions.</summary>
    public interface IPathShape {
        /// <summary>Draws this instruction to a canvas object.</summary>
        /// <param name="canvas">to which this instruction is drawn</param>
        void Draw(PdfCanvas canvas);

        /// <summary>
        /// This method sets the coordinates for the path painting operator and does internal
        /// preprocessing, if necessary
        /// </summary>
        /// <param name="inputCoordinates">an array containing point values for path coordinates</param>
        /// <param name="startPoint">
        /// the ending point of the previous operator, or, in broader terms,
        /// the point that the coordinates should be absolutized against, for relative operators
        /// </param>
        void SetCoordinates(String[] inputCoordinates, Point startPoint);

        /// <summary>
        /// Gets the ending point on the canvas after the path shape has been drawn
        /// via the
        /// <see cref="Draw(iText.Kernel.Pdf.Canvas.PdfCanvas)"/>
        /// method, in SVG space coordinates.
        /// </summary>
        /// <returns>
        /// The
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// representing the final point in the drawn path.
        /// If the point does not exist or does not change
        /// <see langword="null"/>
        /// may be returned.
        /// </returns>
        Point GetEndingPoint();

        /// <summary>Returns true when this shape is a relative operator.</summary>
        /// <remarks>Returns true when this shape is a relative operator. False if it is an absolute operator.</remarks>
        /// <returns>true if relative, false if absolute</returns>
        bool IsRelative();

        /// <summary>Get bounding rectangle of the current path shape.</summary>
        /// <param name="lastPoint">start point for this shape</param>
        /// <returns>calculated rectangle</returns>
        Rectangle GetPathShapeRectangle(Point lastPoint);
    }
}
