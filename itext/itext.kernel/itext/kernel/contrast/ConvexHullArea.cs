/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Geom;

namespace iText.Kernel.Contrast {
//\cond DO_NOT_DOCUMENT
    /// <summary>Utility class for computing the convex hull of a set of points in 2D space.</summary>
    /// <remarks>
    /// Utility class for computing the convex hull of a set of points in 2D space.
    /// <para />
    /// The convex hull is the smallest convex polygon that contains all the given points.
    /// This implementation uses Andrew's monotone chain algorithm, which is a variant of
    /// Graham's scan with improved stability and efficiency.
    /// <para />
    /// The algorithm runs in O(n log n) time complexity, where n is the number of input points.
    /// </remarks>
    internal sealed class ConvexHullArea {
        private const int MIN_POINTS_FOR_HULL = 2;

        private const double EPSILON = 1e-6;

        /// <summary>Private constructor to prevent instantiation of this utility class.</summary>
        private ConvexHullArea() {
        }

        // Utility class
        /// <summary>Computes the convex hull of a set of points using Andrew's monotone chain algorithm.</summary>
        /// <remarks>
        /// Computes the convex hull of a set of points using Andrew's monotone chain algorithm.
        /// <para />
        /// The algorithm works by:
        /// <list type="number">
        /// * Sorting all points lexicographically (first by x-coordinate, then by y-coordinate)
        /// * Constructing the lower hull by scanning from left to right
        /// * Constructing the upper hull by scanning from right to left
        /// * Combining both hulls to form the complete convex hull
        /// </list>
        /// <para />
        /// The returned list of points represents the vertices of the convex hull in counter-clockwise order.
        /// <para />
        /// </remarks>
        /// <param name="points">
        /// the list of points for which to compute the convex hull. Must not be
        /// <see langword="null"/>.
        /// Points may be in any order and may include duplicates.
        /// </param>
        /// <returns>
        /// a list of points representing the vertices of the convex hull in counter-clockwise order.
        /// If the input contains 0 or 1 points, returns the input list unchanged.
        /// If all points are collinear, returns the two extreme points.
        /// </returns>
        public static IList<Point> ConvexHull(IList<Point> points) {
            IList<Point> copiedPoints = new List<Point>(points);
            if (copiedPoints.Count <= 1) {
                return copiedPoints;
            }
            // Sort copiedPoints lexicographically (first by x, then by y)
            JavaCollectionsUtil.Sort(copiedPoints, (p1, p2) => {
                if (p1.GetX() == p2.GetX()) {
                    return JavaUtil.DoubleCompare(p1.GetY(), p2.GetY());
                }
                return JavaUtil.DoubleCompare(p1.GetX(), p2.GetX());
            }
            );
            // Build lower hull
            IList<Point> lower = new List<Point>();
            foreach (Point p in copiedPoints) {
                BuildHull(lower, p);
            }
            // Build upper hull
            IList<Point> upper = new List<Point>();
            for (int i = copiedPoints.Count - 1; i >= 0; i--) {
                Point p = copiedPoints[i];
                BuildHull(upper, p);
            }
            // Remove last point of each half because it's repeated at the beginning of the other half
            lower.JRemoveAt(lower.Count - 1);
            upper.JRemoveAt(upper.Count - 1);
            // Combine lower and upper hulls
            lower.AddAll(upper);
            return lower;
        }

        private static void BuildHull(IList<Point> lower, Point p) {
            while (lower.Count >= MIN_POINTS_FOR_HULL) {
                double crossProduct = Cross(lower[lower.Count - 2], lower[lower.Count - 1], p);
                if (crossProduct > EPSILON) {
                    break;
                }
                lower.JRemoveAt(lower.Count - 1);
            }
            lower.Add(p);
        }

        /// <summary>Calculates the cross product of vectors OA and OB, where O, A, and B are points in 2D space.</summary>
        /// <remarks>
        /// Calculates the cross product of vectors OA and OB, where O, A, and B are points in 2D space.
        /// <para />
        /// The cross product is used to determine the orientation of three points:
        /// <para />
        /// * Positive value: counter-clockwise turn (left turn)
        /// * Negative value: clockwise turn (right turn)
        /// * Zero: collinear points (no turn)
        /// <para />
        /// The formula used is: (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x)
        /// </remarks>
        /// <param name="o">the origin point</param>
        /// <param name="a">the first point forming vector OA</param>
        /// <param name="b">the second point forming vector OB</param>
        /// <returns>
        /// the cross product value. Positive indicates a counter-clockwise turn,
        /// negative indicates a clockwise turn, and zero indicates collinear points.
        /// </returns>
        private static double Cross(Point o, Point a, Point b) {
            return (a.GetX() - o.GetX()) * (b.GetY() - o.GetY()) - (a.GetY() - o.GetY()) * (b.GetX() - o.GetX());
        }
    }
//\endcond
}
