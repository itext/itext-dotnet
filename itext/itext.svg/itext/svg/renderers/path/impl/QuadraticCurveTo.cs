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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements quadratic Bezier curveTo(Q) attribute of SVG's path element</summary>
    public class QuadraticCurveTo : AbstractPathShape, IControlPointCurve {
//\cond DO_NOT_DOCUMENT
        internal const int ARGUMENT_SIZE = 4;
//\endcond

        public QuadraticCurveTo()
            : this(false) {
        }

        public QuadraticCurveTo(bool relative)
            : this(relative, new DefaultOperatorConverter()) {
        }

        public QuadraticCurveTo(bool relative, IOperatorConverter copier)
            : base(relative, copier) {
        }

        /// <summary>Draws a quadratic Bezier curve from the current point to (x,y) using (x1,y1) as the control point
        ///     </summary>
        public override void Draw() {
            float x1 = ParseHorizontalLength(coordinates[0]);
            float y1 = ParseVerticalLength(coordinates[1]);
            float x = ParseHorizontalLength(coordinates[2]);
            float y = ParseVerticalLength(coordinates[3]);
            context.GetCurrentCanvas().CurveTo(x1, y1, x, y);
        }

        public override void SetCoordinates(String[] inputCoordinates, Point startPoint) {
            // startPoint will be used when relative quadratic curve is implemented
            if (inputCoordinates.Length < ARGUMENT_SIZE) {
                throw new ArgumentException(MessageFormatUtil.Format(SvgExceptionMessageConstant.QUADRATIC_CURVE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0
                    , JavaUtil.ArraysToString(coordinates)));
            }
            coordinates = new String[ARGUMENT_SIZE];
            Array.Copy(inputCoordinates, 0, coordinates, 0, ARGUMENT_SIZE);
            double[] initialPoint = new double[] { startPoint.GetX(), startPoint.GetY() };
            if (IsRelative()) {
                coordinates = copier.MakeCoordinatesAbsolute(coordinates, initialPoint);
            }
        }

        public virtual Point GetLastControlPoint() {
            return CreatePoint(coordinates[0], coordinates[1]);
        }

        public override Rectangle GetPathShapeRectangle(Point lastPoint) {
            Point controlPoint = GetLastControlPoint();
            Point endingPoint = GetEndingPoint();
            double[] points = GetBezierMinMaxPoints(lastPoint.GetX(), lastPoint.GetY(), controlPoint.GetX(), controlPoint
                .GetY(), endingPoint.GetX(), endingPoint.GetY());
            return new Rectangle((float)CssUtils.ConvertPxToPts(points[0]), (float)CssUtils.ConvertPxToPts(points[1]), 
                (float)CssUtils.ConvertPxToPts(points[2] - points[0]), (float)CssUtils.ConvertPxToPts(points[3] - points
                [1]));
        }

        /// <summary>
        /// The algorithm is similar to cubic curve in the method CurveTo#getBezierMinMaxPoints,
        /// but the initial function is f(t) = (1 - t)^2*P0 + 2*(1-t)*t*P1 + t^2*P2, 0 &lt;= t &lt;= 1
        /// </summary>
        /// <param name="x0">x coordinate of the starting point</param>
        /// <param name="y0">y coordinate of the starting point</param>
        /// <param name="x1">x coordinate of the control point</param>
        /// <param name="y1">y coordinate of the control point</param>
        /// <param name="x2">x coordinate of the ending point</param>
        /// <param name="y2">y coordinate of the ending point</param>
        /// <returns>array of {xMin, yMin, xMax, yMax} values</returns>
        private static double[] GetBezierMinMaxPoints(double x0, double y0, double x1, double y1, double x2, double
             y2) {
            double xMin = Math.Min(x0, x2);
            double yMin = Math.Min(y0, y2);
            double xMax = Math.Max(x0, x2);
            double yMax = Math.Max(y0, y2);
            double[] extremeTValues = GetExtremeTValues(x0, y0, x1, y1, x2, y2);
            foreach (double t in extremeTValues) {
                double xValue = CalculateExtremeCoordinate(t, x0, x1, x2);
                double yValue = CalculateExtremeCoordinate(t, y0, y1, y2);
                xMin = Math.Min(xValue, xMin);
                yMin = Math.Min(yValue, yMin);
                xMax = Math.Max(xValue, xMax);
                yMax = Math.Max(yValue, yMax);
            }
            return new double[] { xMin, yMin, xMax, yMax };
        }

        /// <summary>Calculate values of t at which the function reaches its extreme points.</summary>
        /// <remarks>
        /// Calculate values of t at which the function reaches its extreme points. To do this, we get the derivative of the
        /// function and equate it to 0:
        /// f'(t) = 2a*t + b. t can only be in the range [0, 1] and it discarded otherwise.
        /// </remarks>
        /// <param name="x0">x coordinate of the starting point</param>
        /// <param name="y0">y coordinate of the starting point</param>
        /// <param name="x1">x coordinate of the control point</param>
        /// <param name="y1">y coordinate of the control point</param>
        /// <param name="x2">x coordinate of the ending point</param>
        /// <param name="y2">y coordinate of the ending point</param>
        /// <returns>array of theta values corresponding to extreme points</returns>
        private static double[] GetExtremeTValues(double x0, double y0, double x1, double y1, double x2, double y2
            ) {
            IList<double> tValuesList = new List<double>();
            AddTValueToList(GetTValue(x0, x1, x2), tValuesList);
            AddTValueToList(GetTValue(y0, y1, y2), tValuesList);
            double[] tValuesArray = new double[tValuesList.Count];
            for (int i = 0; i < tValuesList.Count; i++) {
                tValuesArray[i] = tValuesList[i];
            }
            return tValuesArray;
        }

        /// <summary>Check that t is in the range [0, 1] and add it to list</summary>
        /// <param name="t">value of t</param>
        /// <param name="tValuesList">list storing t values</param>
        private static void AddTValueToList(double t, IList<double> tValuesList) {
            if (0 <= t && t <= 1) {
                tValuesList.Add(t);
            }
        }

        private static double GetTValue(double p0, double p1, double p2) {
            double b = 2 * p1 - 2 * p0;
            double a = p0 - 2 * p1 + p2;
            return -b / (2 * a);
        }

        private static double CalculateExtremeCoordinate(double t, double p0, double p1, double p2) {
            double minusT = 1 - t;
            return (minusT * minusT * p0) + (2 * minusT * t * p1) + (t * t * p2);
        }
    }
}
