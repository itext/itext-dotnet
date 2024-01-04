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

namespace iText.Kernel.Geom {
    /// <summary>Represents a line segment in a particular coordinate system.</summary>
    /// <remarks>Represents a line segment in a particular coordinate system.  This class is immutable.</remarks>
    public class LineSegment {
        /// <summary>Start vector of the segment.</summary>
        private readonly Vector startPoint;

        /// <summary>End vector of the segment.</summary>
        private readonly Vector endPoint;

        /// <summary>Creates a new line segment.</summary>
        /// <param name="startPoint">the start point of a line segment.</param>
        /// <param name="endPoint">the end point of a line segment.</param>
        public LineSegment(Vector startPoint, Vector endPoint) {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        /// <returns>the start point</returns>
        public virtual Vector GetStartPoint() {
            return startPoint;
        }

        /// <returns>the end point</returns>
        public virtual Vector GetEndPoint() {
            return endPoint;
        }

        /// <returns>the length of this line segment</returns>
        public virtual float GetLength() {
            return endPoint.Subtract(startPoint).Length();
        }

        /// <summary>Computes the bounding rectangle for this line segment.</summary>
        /// <remarks>
        /// Computes the bounding rectangle for this line segment.  The rectangle has a rotation 0 degrees
        /// with respect to the coordinate system that the line system is in.  For example, if a line segment
        /// is 5 unit long and sits at a 37 degree angle from horizontal, the bounding rectangle will have
        /// origin of the lower left hand end point of the segment, with width = 4 and height = 3.
        /// </remarks>
        /// <returns>the bounding rectangle</returns>
        public virtual Rectangle GetBoundingRectangle() {
            float x1 = GetStartPoint().Get(Vector.I1);
            float y1 = GetStartPoint().Get(Vector.I2);
            float x2 = GetEndPoint().Get(Vector.I1);
            float y2 = GetEndPoint().Get(Vector.I2);
            return new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));
        }

        /// <summary>Transforms the segment by the specified matrix</summary>
        /// <param name="m">the matrix for the transformation</param>
        /// <returns>the transformed segment</returns>
        public virtual iText.Kernel.Geom.LineSegment TransformBy(Matrix m) {
            Vector newStart = startPoint.Cross(m);
            Vector newEnd = endPoint.Cross(m);
            return new iText.Kernel.Geom.LineSegment(newStart, newEnd);
        }

        /// <summary>Checks if a segment contains another segment in itself</summary>
        /// <param name="other">a segment to be checked</param>
        /// <returns>true if this segment contains other one, false otherwise</returns>
        public virtual bool ContainsSegment(iText.Kernel.Geom.LineSegment other) {
            return other != null && ContainsPoint(other.startPoint) && ContainsPoint(other.endPoint);
        }

        /// <summary>Checks if a segment contains a given point in itself</summary>
        /// <param name="point">a point to be checked</param>
        /// <returns>true if this segment contains given point, false otherwise</returns>
        public virtual bool ContainsPoint(Vector point) {
            if (point == null) {
                return false;
            }
            Vector diff1 = point.Subtract(startPoint);
            if (diff1.Get(0) < 0 || diff1.Get(1) < 0 || diff1.Get(2) < 0) {
                return false;
            }
            Vector diff2 = endPoint.Subtract(point);
            if (diff2.Get(0) < 0 || diff2.Get(1) < 0 || diff2.Get(2) < 0) {
                return false;
            }
            return true;
        }
    }
}
