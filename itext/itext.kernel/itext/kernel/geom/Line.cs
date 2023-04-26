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
    /// <summary>Represents a line.</summary>
    public class Line : IShape {
        private readonly Point p1;

        private readonly Point p2;

        /// <summary>Constructs a new zero-length line starting at zero.</summary>
        public Line()
            : this(0, 0, 0, 0) {
        }

        /// <summary>Constructs a new line based on the given coordinates.</summary>
        /// <param name="x1">x-coordinate of start point of this Line</param>
        /// <param name="y1">y-coordinate of start point of this Line</param>
        /// <param name="x2">x-coordinate of end point of this Line</param>
        /// <param name="y2">y-coordinate of end point of this Line</param>
        public Line(float x1, float y1, float x2, float y2) {
            p1 = new Point(x1, y1);
            p2 = new Point(x2, y2);
        }

        /// <summary>Constructs a new line based on the given coordinates.</summary>
        /// <param name="p1">start point of this Line</param>
        /// <param name="p2">end point of this Line</param>
        public Line(Point p1, Point p2)
            : this((float)p1.GetX(), (float)p1.GetY(), (float)p2.GetX(), (float)p2.GetY()) {
        }

        public virtual IList<Point> GetBasePoints() {
            IList<Point> basePoints = new List<Point>(2);
            basePoints.Add(p1);
            basePoints.Add(p2);
            return basePoints;
        }
    }
}
