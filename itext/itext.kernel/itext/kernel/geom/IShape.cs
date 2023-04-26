/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;

namespace iText.Kernel.Geom {
    /// <summary>Represents segment from a PDF path.</summary>
    public interface IShape {
        /// <summary>Treat base points as the points which are enough to construct a shape.</summary>
        /// <remarks>
        /// Treat base points as the points which are enough to construct a shape.
        /// E.g. for a bezier curve they are control points, for a line segment - the start and the end points
        /// of the segment.
        /// </remarks>
        /// <returns>
        /// Ordered
        /// <see cref="System.Collections.IList{E}"/>
        /// consisting of shape's base points.
        /// </returns>
        IList<Point> GetBasePoints();
    }
}
