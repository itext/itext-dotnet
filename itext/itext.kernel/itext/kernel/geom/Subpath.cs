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
using System.Collections.Generic;

namespace iText.Kernel.Geom {
    /// <summary>As subpath is a part of a path comprising a sequence of connected segments.</summary>
    public class Subpath {
        private Point startPoint;

        private IList<IShape> segments = new List<IShape>();

        private bool closed;

        /// <summary>Creates a new SubPath instance.</summary>
        public Subpath() {
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="subpath">
        /// 
        /// <see cref="Subpath"/>
        /// which contents will be used to create this
        /// <see cref="Subpath"/>
        /// </param>
        public Subpath(iText.Kernel.Geom.Subpath subpath) {
            this.startPoint = subpath.startPoint;
            this.segments.AddAll(subpath.GetSegments());
            this.closed = subpath.closed;
        }

        /// <summary>Constructs a new subpath starting at the given point.</summary>
        /// <param name="startPoint">the point this subpath starts at</param>
        public Subpath(Point startPoint)
            : this((float)startPoint.GetX(), (float)startPoint.GetY()) {
        }

        /// <summary>Constructs a new subpath starting at the given point.</summary>
        /// <param name="startPointX">x-coordinate of the start point of subpath</param>
        /// <param name="startPointY">y-coordinate of the start point of subpath</param>
        public Subpath(float startPointX, float startPointY) {
            this.startPoint = new Point(startPointX, startPointY);
        }

        /// <summary>Sets the start point of the subpath.</summary>
        /// <param name="startPoint">the point this subpath starts at</param>
        public virtual void SetStartPoint(Point startPoint) {
            SetStartPoint((float)startPoint.GetX(), (float)startPoint.GetY());
        }

        /// <summary>Sets the start point of the subpath.</summary>
        /// <param name="x">x-coordinate of the start pint</param>
        /// <param name="y">y-coordinate of the start pint</param>
        public virtual void SetStartPoint(float x, float y) {
            this.startPoint = new Point(x, y);
        }

        /// <returns>The point this subpath starts at.</returns>
        public virtual Point GetStartPoint() {
            return startPoint;
        }

        /// <returns>The last point of the subpath.</returns>
        public virtual Point GetLastPoint() {
            Point lastPoint = startPoint;
            if (segments.Count > 0 && !closed) {
                IShape shape = segments[segments.Count - 1];
                lastPoint = shape.GetBasePoints()[shape.GetBasePoints().Count - 1];
            }
            return lastPoint;
        }

        /// <summary>Adds a segment to the subpath.</summary>
        /// <remarks>
        /// Adds a segment to the subpath.
        /// Note: each new segment shall start at the end of the previous segment.
        /// </remarks>
        /// <param name="segment">new segment.</param>
        public virtual void AddSegment(IShape segment) {
            if (closed) {
                return;
            }
            if (IsSinglePointOpen()) {
                startPoint = segment.GetBasePoints()[0];
            }
            segments.Add(segment);
        }

        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// comprising all the segments
        /// the subpath made on.
        /// </returns>
        public virtual IList<IShape> GetSegments() {
            return segments;
        }

        /// <summary>Checks whether subpath is empty or not.</summary>
        /// <returns>true if the subpath is empty, false otherwise.</returns>
        public virtual bool IsEmpty() {
            return startPoint == null;
        }

        /// <returns>
        /// <c>true</c> if this subpath contains only one point and it is not closed,
        /// <c>false</c> otherwise
        /// </returns>
        public virtual bool IsSinglePointOpen() {
            return segments.Count == 0 && !closed;
        }

        /// <returns>
        /// <c>true</c> if this subpath contains only one point and it is closed,
        /// <c>false</c> otherwise
        /// </returns>
        public virtual bool IsSinglePointClosed() {
            return segments.Count == 0 && closed;
        }

        /// <summary>Returns a <c>boolean</c> value indicating whether the subpath must be closed or not.</summary>
        /// <remarks>
        /// Returns a <c>boolean</c> value indicating whether the subpath must be closed or not.
        /// Ignore this value if the subpath is a rectangle because in this case it is already closed
        /// (of course if you paint the path using <c>re</c> operator)
        /// </remarks>
        /// <returns><c>boolean</c> value indicating whether the path must be closed or not.</returns>
        public virtual bool IsClosed() {
            return closed;
        }

        /// <summary>
        /// See
        /// <see cref="IsClosed()"/>
        /// </summary>
        /// <param name="closed"><c>boolean</c> value indicating whether the path is closed or not.</param>
        public virtual void SetClosed(bool closed) {
            this.closed = closed;
        }

        /// <summary>Returns a <c>boolean</c> indicating whether the subpath is degenerate or not.</summary>
        /// <remarks>
        /// Returns a <c>boolean</c> indicating whether the subpath is degenerate or not.
        /// A degenerate subpath is the subpath consisting of a single-point closed path or of
        /// two or more points at the same coordinates.
        /// </remarks>
        /// <returns><c>boolean</c> value indicating whether the path is degenerate or not.</returns>
        public virtual bool IsDegenerate() {
            if (segments.Count > 0 && closed) {
                return false;
            }
            foreach (IShape segment in segments) {
                ICollection<Point> points = new HashSet<Point>(segment.GetBasePoints());
                // The first segment of a subpath always starts at startPoint, so...
                if (points.Count != 1) {
                    return false;
                }
            }
            // the second clause is for case when we have single point
            return segments.Count > 0 || closed;
        }

        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// containing points of piecewise linear approximation
        /// for this subpath.
        /// </returns>
        public virtual IList<Point> GetPiecewiseLinearApproximation() {
            IList<Point> result = new List<Point>();
            if (segments.Count == 0) {
                return result;
            }
            if (segments[0] is BezierCurve) {
                result.AddAll(((BezierCurve)segments[0]).GetPiecewiseLinearApproximation());
            }
            else {
                result.AddAll(segments[0].GetBasePoints());
            }
            for (int i = 1; i < segments.Count; ++i) {
                IList<Point> segApprox;
                if (segments[i] is BezierCurve) {
                    segApprox = ((BezierCurve)segments[i]).GetPiecewiseLinearApproximation();
                    segApprox = segApprox.SubList(1, segApprox.Count);
                }
                else {
                    segApprox = segments[i].GetBasePoints();
                    segApprox = segApprox.SubList(1, segApprox.Count);
                }
                result.AddAll(segApprox);
            }
            return result;
        }
    }
}
