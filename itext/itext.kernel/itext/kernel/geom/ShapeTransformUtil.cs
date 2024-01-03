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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Geom {
    /// <summary>Public helper class for transforming segments and paths.</summary>
    public sealed class ShapeTransformUtil {
        /// <summary>Method for transforming a bezier curve.</summary>
        /// <remarks>
        /// Method for transforming a bezier curve.
        /// The method creates a new transformed bezier curve without changing the original curve.
        /// </remarks>
        /// <param name="bezierCurve">the source bezier curve for transformation</param>
        /// <param name="ctm">the transformation matrix</param>
        /// <returns>the new transformed bezier curve</returns>
        public static BezierCurve TransformBezierCurve(BezierCurve bezierCurve, Matrix ctm) {
            return (BezierCurve)TransformSegment(bezierCurve, ctm);
        }

        /// <summary>Method for transforming a line.</summary>
        /// <remarks>
        /// Method for transforming a line.
        /// The method creates a new transformed line without changing the original line.
        /// </remarks>
        /// <param name="line">the source line for transformation</param>
        /// <param name="ctm">the transformation matrix</param>
        /// <returns>the new transformed line</returns>
        public static Line TransformLine(Line line, Matrix ctm) {
            return (Line)TransformSegment(line, ctm);
        }

        /// <summary>Method for transforming a path.</summary>
        /// <remarks>
        /// Method for transforming a path.
        /// The method creates a new transformed path without changing the original path.
        /// </remarks>
        /// <param name="path">the source path for transformation</param>
        /// <param name="ctm">the transformation matrix</param>
        /// <returns>the new transformed path</returns>
        public static Path TransformPath(Path path, Matrix ctm) {
            Path newPath = new Path();
            foreach (Subpath subpath in path.GetSubpaths()) {
                Subpath transformedSubpath = TransformSubpath(subpath, ctm);
                newPath.AddSubpath(transformedSubpath);
            }
            return newPath;
        }

        private static Subpath TransformSubpath(Subpath subpath, Matrix ctm) {
            Subpath newSubpath = new Subpath();
            newSubpath.SetClosed(subpath.IsClosed());
            foreach (IShape segment in subpath.GetSegments()) {
                IShape transformedSegment = TransformSegment(segment, ctm);
                newSubpath.AddSegment(transformedSegment);
            }
            return newSubpath;
        }

        private static IShape TransformSegment(IShape segment, Matrix ctm) {
            IList<Point> basePoints = segment.GetBasePoints();
            Point[] newBasePoints = TransformPoints(ctm, basePoints.ToArray(new Point[basePoints.Count]));
            IShape newSegment;
            if (segment is BezierCurve) {
                newSegment = new BezierCurve(JavaUtil.ArraysAsList(newBasePoints));
            }
            else {
                newSegment = new Line(newBasePoints[0], newBasePoints[1]);
            }
            return newSegment;
        }

        private static Point[] TransformPoints(Matrix ctm, params Point[] points) {
            try {
                AffineTransform t = new AffineTransform(ctm.Get(Matrix.I11), ctm.Get(Matrix.I12), ctm.Get(Matrix.I21), ctm
                    .Get(Matrix.I22), ctm.Get(Matrix.I31), ctm.Get(Matrix.I32));
                t = t.CreateInverse();
                Point[] newPoints = new Point[points.Length];
                t.Transform(points, 0, newPoints, 0, points.Length);
                return newPoints;
            }
            catch (NoninvertibleTransformException e) {
                throw new PdfException(KernelExceptionMessageConstant.NONINVERTIBLE_MATRIX_CANNOT_BE_PROCESSED, e);
            }
        }
    }
}
