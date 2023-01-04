/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements curveTo(C) attribute of SVG's path element</summary>
    public class CurveTo : AbstractPathShape, IControlPointCurve {
        internal const int ARGUMENT_SIZE = 6;

        private static double ZERO_EPSILON = 1e-12;

        public CurveTo()
            : this(false) {
        }

        public CurveTo(bool relative)
            : this(relative, new DefaultOperatorConverter()) {
        }

        public CurveTo(bool relative, IOperatorConverter copier)
            : base(relative, copier) {
        }

        public override void Draw(PdfCanvas canvas) {
            float x1 = CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[0]);
            float y1 = CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[1]);
            float x2 = CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[2]);
            float y2 = CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[3]);
            float x = CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[4]);
            float y = CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[5]);
            canvas.CurveTo(x1, y1, x2, y2, x, y);
        }

        public override void SetCoordinates(String[] inputCoordinates, Point startPoint) {
            if (inputCoordinates.Length < ARGUMENT_SIZE) {
                throw new ArgumentException(MessageFormatUtil.Format(SvgExceptionMessageConstant.CURVE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0
                    , JavaUtil.ArraysToString(inputCoordinates)));
            }
            coordinates = new String[ARGUMENT_SIZE];
            Array.Copy(inputCoordinates, 0, coordinates, 0, ARGUMENT_SIZE);
            double[] initialPoint = new double[] { startPoint.GetX(), startPoint.GetY() };
            if (IsRelative()) {
                coordinates = copier.MakeCoordinatesAbsolute(coordinates, initialPoint);
            }
        }

        public virtual Point GetLastControlPoint() {
            return CreatePoint(coordinates[2], coordinates[3]);
        }

        public override Rectangle GetPathShapeRectangle(Point lastPoint) {
            Point firstControlPoint = GetFirstControlPoint();
            Point lastControlPoint = GetLastControlPoint();
            Point endingPoint = GetEndingPoint();
            double[] points = GetBezierMinMaxPoints(lastPoint.GetX(), lastPoint.GetY(), firstControlPoint.GetX(), firstControlPoint
                .GetY(), lastControlPoint.GetX(), lastControlPoint.GetY(), endingPoint.GetX(), endingPoint.GetY());
            return new Rectangle((float)CssUtils.ConvertPxToPts(points[0]), (float)CssUtils.ConvertPxToPts(points[1]), 
                (float)CssUtils.ConvertPxToPts(points[2] - points[0]), (float)CssUtils.ConvertPxToPts(points[3] - points
                [1]));
        }

        private Point GetFirstControlPoint() {
            return CreatePoint(coordinates[0], coordinates[1]);
        }

        /// <summary>
        /// Initial function of cubic bezier is f(t) = (t-1)^3*P0 + 3*(1-t)^2*t*P1 + 3*(1-t)*t^2*P2 + t^3*P3, where 0 &lt;= t &lt;= 1
        /// After opening brackets it can be reduced to f(t) = a*t^3 + b*t^2 + c*t + d, where
        /// a = P3-3*P2+3*P1-P0
        /// b = 3*P2-6*P1+3*P0
        /// c = 3*P1-3*P0
        /// d = P0
        /// First we must find the values of t at which the function reaches its extreme points.
        /// </summary>
        /// <remarks>
        /// Initial function of cubic bezier is f(t) = (t-1)^3*P0 + 3*(1-t)^2*t*P1 + 3*(1-t)*t^2*P2 + t^3*P3, where 0 &lt;= t &lt;= 1
        /// After opening brackets it can be reduced to f(t) = a*t^3 + b*t^2 + c*t + d, where
        /// a = P3-3*P2+3*P1-P0
        /// b = 3*P2-6*P1+3*P0
        /// c = 3*P1-3*P0
        /// d = P0
        /// First we must find the values of t at which the function reaches its extreme points.
        /// This happens in the method
        /// <see cref="GetTValuesInExtremePoints(double, double, double, double, double, double, double, double)"/>.
        /// Next we get x and y values in extremes and compare it with the start and ending points coordinates to get the borders of the bounding box.
        /// </remarks>
        /// <param name="x0">x coordinate of the starting point</param>
        /// <param name="y0">y coordinate of the starting point</param>
        /// <param name="x1">x coordinate of the first control point</param>
        /// <param name="y1">y coordinate of the first control point</param>
        /// <param name="x2">x coordinate of the second control point</param>
        /// <param name="y2">y coordinate of the second control point</param>
        /// <param name="x3">x coordinate of the ending point</param>
        /// <param name="y3">y coordinate of the ending point</param>
        /// <returns>array of {xMin, yMin, xMax, yMax} values</returns>
        private static double[] GetBezierMinMaxPoints(double x0, double y0, double x1, double y1, double x2, double
             y2, double x3, double y3) {
            // take start and end points as a min/max
            double xMin = Math.Min(x0, x3);
            double yMin = Math.Min(y0, y3);
            double xMax = Math.Max(x0, x3);
            double yMax = Math.Max(y0, y3);
            // get array of t at which the function reaches its extreme points. This array contains both extremes for y and x coordinates.
            double[] extremeTValues = GetTValuesInExtremePoints(x0, y0, x1, y1, x2, y2, x3, y3);
            foreach (double t in extremeTValues) {
                double xValue = CalculateExtremeCoordinate(t, x0, x1, x2, x3);
                double yValue = CalculateExtremeCoordinate(t, y0, y1, y2, y3);
                // change min/max values in accordance with extreme points
                xMin = Math.Min(xValue, xMin);
                yMin = Math.Min(yValue, yMin);
                xMax = Math.Max(xValue, xMax);
                yMax = Math.Max(yValue, yMax);
            }
            return new double[] { xMin, yMin, xMax, yMax };
        }

        /// <summary>Calculate values of t at which the function reaches its extreme points.</summary>
        /// <remarks>
        /// Calculate values of t at which the function reaches its extreme points. To do this, we get the derivative of the function and equate it to 0:
        /// f'(t) = 3a*t^2 + 2b*t + c. This is parabola and for finding we calculate the discriminant. t can only be in the range [0, 1] and it discarded otherwise.
        /// </remarks>
        /// <param name="x0">x coordinate of the starting point</param>
        /// <param name="y0">y coordinate of the starting point</param>
        /// <param name="x1">x coordinate of the first control point</param>
        /// <param name="y1">y coordinate of the first control point</param>
        /// <param name="x2">x coordinate of the second control point</param>
        /// <param name="y2">y coordinate of the second control point</param>
        /// <param name="x3">x coordinate of the ending point</param>
        /// <param name="y3">y coordinate of the ending point</param>
        /// <returns>array of theta values corresponding to extreme points</returns>
        private static double[] GetTValuesInExtremePoints(double x0, double y0, double x1, double y1, double x2, double
             y2, double x3, double y3) {
            IList<double> tValuesList = new List<double>(CalculateTValues(x0, x1, x2, x3));
            tValuesList.AddAll(CalculateTValues(y0, y1, y2, y3));
            double[] tValuesArray = new double[tValuesList.Count];
            for (int i = 0; i < tValuesList.Count; i++) {
                tValuesArray[i] = tValuesList[i];
            }
            return tValuesArray;
        }

        /// <summary>Calculate the quadratic function 3a*t^2 + 2b*t + c = 0 to obtain the values of t</summary>
        /// <param name="p0">coordinate of the starting point</param>
        /// <param name="p1">coordinate of the first control point</param>
        /// <param name="p2">coordinate of the second control point</param>
        /// <param name="p3">coordinate of the ending point</param>
        /// <returns>list of t values. t should be in range [0, 1]</returns>
        private static IList<double> CalculateTValues(double p0, double p1, double p2, double p3) {
            IList<double> tValuesList = new List<double>();
            double a = (-p0 + 3 * p1 - 3 * p2 + p3) * 3;
            double b = (3 * p0 - 6 * p1 + 3 * p2) * 2;
            double c = 3 * p1 - 3 * p0;
            if (Math.Abs(a) < ZERO_EPSILON) {
                if (Math.Abs(b) >= ZERO_EPSILON) {
                    // if a = 0 and mod(b) > 0 this is linear function
                    AddTValueToList(-c / b, tValuesList);
                }
            }
            else {
                double discriminant = b * b - 4 * c * a;
                // we dont check discriminant < 0, because t can only be in the range [0, 1], and in this case there are
                // no extremums in such case, which means the max and min values are at the starting and ending points which are accounted for at the beginning.
                if (discriminant <= 0 && Math.Abs(discriminant) < ZERO_EPSILON) {
                    // in case of zero discriminant we have only one solution
                    AddTValueToList(-b / (2 * a), tValuesList);
                }
                else {
                    double discriminantSqrt = Math.Sqrt(discriminant);
                    AddTValueToList((-b + discriminantSqrt) / (2 * a), tValuesList);
                    AddTValueToList((-b - discriminantSqrt) / (2 * a), tValuesList);
                }
            }
            return tValuesList;
        }

        /// <summary>Check that t is in the range [0, 1] and add it to list</summary>
        /// <param name="t">value of t</param>
        /// <param name="tValuesList">list storing t values</param>
        private static void AddTValueToList(double t, IList<double> tValuesList) {
            if (0 <= t && t <= 1) {
                tValuesList.Add(t);
            }
        }

        private static double CalculateExtremeCoordinate(double t, double p0, double p1, double p2, double p3) {
            double minusT = 1 - t;
            // calculate extreme x,y in accordance with function f(t) = (t-1)^3*P0 + 3*(1-t)^2*t*P1 + 3*(1-t)*t^2*P2 + t^3*P3
            return (minusT * minusT * minusT * p0) + (3 * minusT * minusT * t * p1) + (3 * minusT * t * t * p2) + (t *
                 t * t * p3);
        }
    }
}
