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
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements elliptical curveTo (A) segment of SVG's path element.</summary>
    /// <remarks>
    /// Implements elliptical curveTo (A) segment of SVG's path element. Implemented in PDF as Bézier curves.
    /// Edge cases &amp; value correction below always refer to https://www.w3.org/TR/SVG11/implnote.html#ArcImplementationNotes
    /// For some calculations we need double precision floating point math, so we have forced all calculations to use double.
    /// However, float comparison is used instead of double comparison, because close coordinates can be considered equal.
    /// </remarks>
    public class EllipticalCurveTo : AbstractPathShape {
        internal const int ARGUMENT_SIZE = 7;

        private Point startPoint;

        private const double EPS = 0.00001;

        /// <summary>Creates an absolute Elliptical curveTo.</summary>
        public EllipticalCurveTo()
            : this(false) {
        }

        /// <summary>Creates a Elliptical curveTo.</summary>
        /// <remarks>Creates a Elliptical curveTo. Set argument to true to create a relative EllipticalCurveTo.</remarks>
        /// <param name="relative">whether this is a relative EllipticalCurveTo or not</param>
        public EllipticalCurveTo(bool relative)
            : base(relative) {
        }

        public override void SetCoordinates(String[] inputCoordinates, Point previous) {
            startPoint = previous;
            if (inputCoordinates.Length < ARGUMENT_SIZE) {
                throw new ArgumentException(MessageFormatUtil.Format(SvgExceptionMessageConstant.ARC_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0
                    , JavaUtil.ArraysToString(inputCoordinates)));
            }
            coordinates = new String[ARGUMENT_SIZE];
            Array.Copy(inputCoordinates, 0, coordinates, 0, ARGUMENT_SIZE);
            double[] initialPoint = new double[] { previous.GetX(), previous.GetY() };
            // ignore partial argument groups, which do not form a fixed set of 7 elements
            if (IsRelative()) {
                String[] relativeEndPoint = new String[] { inputCoordinates[5], inputCoordinates[6] };
                String[] absoluteEndPoint = copier.MakeCoordinatesAbsolute(relativeEndPoint, initialPoint);
                coordinates[5] = absoluteEndPoint[0];
                coordinates[6] = absoluteEndPoint[1];
            }
        }

        public override void Draw(PdfCanvas canvas) {
            Point start = new Point(startPoint.x * .75, startPoint.y * .75);
            // pixels to points
            double rx = Math.Abs(CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[0]));
            double ry = Math.Abs(CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[1]));
            // φ is taken mod 360 degrees.
            double rotation = Double.Parse(coordinates[2], System.Globalization.CultureInfo.InvariantCulture) % 360.0;
            // rotation argument is given in degrees, but we need radians for easier trigonometric calculations
            rotation = MathUtil.ToRadians(rotation);
            // binary flags (Value correction: any nonzero value for either of the flags fA or fS is taken to mean the value 1.)
            bool largeArc = !CssUtils.CompareFloats((float)CssDimensionParsingUtils.ParseFloat(coordinates[3]), 0);
            bool sweep = !CssUtils.CompareFloats((float)CssDimensionParsingUtils.ParseFloat(coordinates[4]), 0);
            Point end = new Point(CssDimensionParsingUtils.ParseAbsoluteLength(coordinates[5]), CssDimensionParsingUtils
                .ParseAbsoluteLength(coordinates[6]));
            if (CssUtils.CompareFloats(start.x, end.x) && CssUtils.CompareFloats(start.y, end.y)) {
                /* edge case: If the endpoints (x1, y1) and (x2, y2) are identical,
                * then this is equivalent to omitting the elliptical arc segment entirely.
                */
                return;
            }
            if (CssUtils.CompareFloats(rx, 0) || CssUtils.CompareFloats(ry, 0)) {
                /* edge case: If rx = 0 or ry = 0 then this arc is treated as a straight line segment (a "lineto")
                * joining the endpoints.
                */
                canvas.LineTo(end.x, end.y);
            }
            else {
                /* This is the first step of calculating a rotated elliptical path.
                We must simulate a transformation on the end-point in order to calculate appropriate EllipseArc angles;
                if we don't do this, then the EllipseArc class will calculate the correct bounding rectangle,
                but an incorrect starting angle and/or extent.
                */
                EllipticalCurveTo.EllipseArc arc;
                if (CssUtils.CompareFloats(rotation, 0)) {
                    arc = EllipticalCurveTo.EllipseArc.GetEllipse(start, end, rx, ry, sweep, largeArc);
                }
                else {
                    AffineTransform normalizer = AffineTransform.GetRotateInstance(-rotation);
                    normalizer.Translate(-start.x, -start.y);
                    Point newArcEnd = normalizer.Transform(end, null);
                    newArcEnd.Translate(start.x, start.y);
                    arc = EllipticalCurveTo.EllipseArc.GetEllipse(start, newArcEnd, rx, ry, sweep, largeArc);
                }
                Point[][] points = MakePoints(PdfCanvas.BezierArc(arc.ll.x, arc.ll.y, arc.ur.x, arc.ur.y, arc.startAng, arc
                    .extent));
                if (sweep) {
                    points = Rotate(points, rotation, points[0][0]);
                    for (int i = 0; i < points.Length; i++) {
                        DrawCurve(canvas, points[i][1], points[i][2], points[i][3]);
                    }
                }
                else {
                    points = Rotate(points, rotation, points[points.Length - 1][3]);
                    for (int i = points.Length - 1; i >= 0; i--) {
                        DrawCurve(canvas, points[i][2], points[i][1], points[i][0]);
                    }
                }
            }
        }

        /// <summary>This convenience method rotates a given set of points around a given point</summary>
        /// <param name="list">the input list</param>
        /// <param name="rotation">the rotation angle, in radians</param>
        /// <param name="rotator">the point to rotate around</param>
        /// <returns>the list of rotated points</returns>
        internal static Point[][] Rotate(Point[][] list, double rotation, Point rotator) {
            if (!CssUtils.CompareFloats(rotation, 0)) {
                Point[][] result = new Point[list.Length][];
                AffineTransform transRotTrans = AffineTransform.GetRotateInstance(rotation, rotator.x, rotator.y);
                for (int i = 0; i < list.Length; i++) {
                    Point[] input = list[i];
                    Point[] row = new Point[input.Length];
                    for (int j = 0; j < input.Length; j++) {
                        row[j] = transRotTrans.Transform(input[j], null);
                    }
                    result[i] = row;
                }
                return result;
            }
            return list;
        }

        internal virtual String[] GetCoordinates() {
            return coordinates;
        }

        private static void DrawCurve(PdfCanvas canvas, Point cp1, Point cp2, Point end) {
            canvas.CurveTo(cp1.x, cp1.y, cp2.x, cp2.y, end.x, end.y);
        }

        private Point[][] MakePoints(IList<double[]> input) {
            Point[][] result = new Point[input.Count][];
            for (int i = 0; i < input.Count; i++) {
                result[i] = new Point[input[i].Length / 2];
                for (int j = 0; j < input[i].Length; j += 2) {
                    result[i][j / 2] = new Point(input[i][j], input[i][j + 1]);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts between two types of definitions of an arc:
        /// The input is an arc defined by two points and the two semi-axes of the ellipse.
        /// </summary>
        /// <remarks>
        /// Converts between two types of definitions of an arc:
        /// The input is an arc defined by two points and the two semi-axes of the ellipse.
        /// The output is an arc defined by a bounding rectangle, the starting angle,
        /// and the angular extent of the ellipse that is to be drawn.
        /// The output is an intermediate step to calculating the Bézier curve(s) that approximate(s) the elliptical arc,
        /// which happens in
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>.
        /// </remarks>
        internal class EllipseArc {
            internal readonly Point ll;

            internal readonly Point ur;

            internal readonly double startAng;

            internal readonly double extent;

            internal EllipseArc(Point center, double a, double b, double startAng, double extent) {
                ll = new Point(center.x - a, center.y - b);
                ur = new Point(center.x + a, center.y + b);
                this.startAng = startAng;
                this.extent = extent;
            }

            internal static EllipticalCurveTo.EllipseArc GetEllipse(Point start, Point end, double a, double b, bool sweep
                , bool largeArc) {
                double r1 = (start.x - end.x) / (-2.0 * a);
                double r2 = (start.y - end.y) / (2.0 * b);
                double factor = Math.Sqrt(r1 * r1 + r2 * r2);
                if (factor > 1) {
                    /* If rx, ry and φ are such that there is no solution (basically, the ellipse is not big enough
                    * to reach from (x1, y1) to (x2, y2)) then the ellipse's semi-axes are scaled up uniformly
                    * until there is exactly one solution (until the ellipse is just big enough).
                    */
                    return GetEllipse(start, end, a * factor, b * factor, sweep, largeArc);
                }
                double between1 = Math.Atan(r1 / r2);
                double between2 = Math.Asin(factor);
                EllipticalCurveTo.EllipseArc result = CalculatePossibleMiddle(start, end, a, b, between1 + between2, sweep
                    , largeArc);
                if (result != null) {
                    return result;
                }
                result = CalculatePossibleMiddle(start, end, a, b, Math.PI + between1 - between2, sweep, largeArc);
                if (result != null) {
                    return result;
                }
                result = CalculatePossibleMiddle(start, end, a, b, Math.PI + between1 + between2, sweep, largeArc);
                if (result != null) {
                    return result;
                }
                result = CalculatePossibleMiddle(start, end, a, b, between1 - between2, sweep, largeArc);
                if (result != null) {
                    return result;
                }
                throw new SvgProcessingException(SvgExceptionMessageConstant.COULD_NOT_DETERMINE_MIDDLE_POINT_OF_ELLIPTICAL_ARC
                    );
            }

            internal static EllipticalCurveTo.EllipseArc CalculatePossibleMiddle(Point start, Point end, double a, double
                 b, double startToCenterAngle, bool sweep, bool largeArc) {
                double x0 = start.x - a * Math.Cos(startToCenterAngle);
                double y0 = start.y - b * Math.Sin(startToCenterAngle);
                Point center = new Point(x0, y0);
                double check = Math.Pow(((end.x - center.x) / a), 2) + Math.Pow(((end.y - center.y) / b), 2);
                /* If center is an actual candidate for a middle point, then the value of check will be very close to 1.0.
                * Otherwise it is always larger than 1.
                * Due to floating point math, we need to introduce an epsilon value.
                */
                if (CssUtils.CompareFloats(check, 1.0)) {
                    double theta1 = CalculateAngle(start, center, a, b);
                    double theta2 = CalculateAngle(end, center, a, b);
                    double startAngl = 0;
                    double extent = 0;
                    // round the difference, to catch edge cases with floating point math around the value 180
                    long deltaTheta = (long)Math.Abs(MathematicUtil.Round(theta2 - theta1));
                    //both points are on the ellipse, but is this the middle, looked for?
                    if (largeArc) {
                        //turn more than 180 degrees
                        if (sweep) {
                            if ((theta2 > theta1) && (deltaTheta >= 180)) {
                                startAngl = theta1;
                                extent = theta2 - theta1;
                            }
                            if ((theta1 > theta2) && (deltaTheta <= 180)) {
                                startAngl = theta1;
                                extent = 360 - theta1 + theta2;
                            }
                        }
                        else {
                            if ((theta2 > theta1) && (deltaTheta <= 180)) {
                                startAngl = theta2;
                                extent = 360 - theta2 + theta1;
                            }
                            //or the same extent but negative and start at p1
                            if ((theta1 > theta2) && (deltaTheta >= 180)) {
                                startAngl = theta2;
                                extent = theta1 - theta2;
                            }
                        }
                    }
                    else {
                        if (sweep) {
                            if ((theta2 > theta1) && (deltaTheta <= 180)) {
                                startAngl = theta1;
                                extent = theta2 - theta1;
                            }
                            if ((theta1 > theta2) && (deltaTheta >= 180)) {
                                startAngl = theta1;
                                extent = 360 - theta1 + theta2;
                            }
                        }
                        else {
                            if ((theta2 > theta1) && (deltaTheta >= 180)) {
                                startAngl = theta2;
                                extent = 360 - theta2 + theta1;
                            }
                            //or the same extent but negative and start at p1
                            if ((theta1 > theta2) && (deltaTheta <= 180)) {
                                startAngl = theta2;
                                extent = theta1 - theta2;
                            }
                        }
                    }
                    if (startAngl >= 0 && extent > 0) {
                        return new EllipticalCurveTo.EllipseArc(center, a, b, startAngl, extent);
                    }
                }
                return null;
            }

            internal static double CalculateAngle(Point pt, Point center, double a, double b) {
                double result = Math.Pow(((pt.x - center.x) / a), 2.0) + Math.Pow(((pt.y - center.y) / b), 2.0);
                double cos = (pt.x - center.x) / a;
                double sin = (pt.y - center.y) / b;
                // catch very small floating point errors and keep cos between [-1, 1], so we can calculate the arc cosine
                cos = Math.Max(Math.Min(cos, 1.0), -1.0);
                if ((cos >= 0 && sin >= 0) || (cos < 0 && sin >= 0)) {
                    result = ToDegrees(Math.Acos(cos));
                }
                if ((cos >= 0 && sin < 0) || (cos < 0 && sin < 0)) {
                    result = 360 - ToDegrees(Math.Acos(cos));
                }
                return result;
            }

            internal static double ToDegrees(double radians) {
                return radians * 180.0 / Math.PI;
            }
        }

        public override Rectangle GetPathShapeRectangle(Point lastPoint) {
            double[] points = GetEllipticalArcMinMaxPoints(lastPoint.GetX(), lastPoint.GetY(), GetCoordinate(0), GetCoordinate
                (1), GetCoordinate(2), GetCoordinate(3) != 0, GetCoordinate(4) != 0, GetCoordinate(5), GetCoordinate(6
                ));
            return new Rectangle((float)CssUtils.ConvertPxToPts(points[0]), (float)CssUtils.ConvertPxToPts(points[1]), 
                (float)CssUtils.ConvertPxToPts(points[2] - points[0]), (float)CssUtils.ConvertPxToPts(points[3] - points
                [1]));
        }

        private double GetCoordinate(int index) {
            // casting to double fot porting compatibility
            return (double)CssDimensionParsingUtils.ParseDouble(coordinates[index]);
        }

        /// <summary>
        /// Algorithm to find elliptical arc bounding box:
        /// 1.
        /// </summary>
        /// <remarks>
        /// Algorithm to find elliptical arc bounding box:
        /// 1. Compute extremes using parametric description of the whole ellipse
        /// We use parametric description of ellipse:
        /// x(theta) = cx + rx*cos(theta)*cos(phi) - ry*sin(theta)*sin(phi)
        /// y(theta) = cy + rx*cos(theta)*sin(phi) + ry*sin(theta)*cos(phi)
        /// After obtaining the derivative and equating it to zero, we get two solutions for x:
        /// theta = -atan(ry*tan(phi)/rx) and theta = M_PI -atan(ry*tan(phi)/rx)
        /// and two solutions for y:
        /// theta = atan(ry/(tan(phi)*rx)) and theta = M_PI + atan(ry/(tan(phi)*rx))
        /// Then to get theta values we need to know cx and cy - the coordinates of the center of the ellipse.
        /// 2. Compute the center of the ellipse
        /// Method
        /// <see cref="GetEllipseCenterCoordinates(double, double, double, double, double, bool, bool, double, double)
        ///     "/>
        /// 3. Determine the bounding box of the whole ellipse
        /// When we know cx and cy values we can get the bounding box of whole ellipse. That done in the method
        /// <see cref="GetEllipseCenterCoordinates(double, double, double, double, double, bool, bool, double, double)
        ///     "/>.
        /// 4. Find tightest possible bounding box
        /// Check that given points is on the arc using polar coordinates of points. Method
        /// <see cref="IsPointOnTheArc(double, double, double, bool)"/>.
        /// </remarks>
        /// <param name="x1">x coordinate of the starting point</param>
        /// <param name="y1">y coordinate of the starting point</param>
        /// <param name="rx">x radius</param>
        /// <param name="ry">y radius</param>
        /// <param name="phi">x-axis rotation</param>
        /// <param name="largeArc">large arc flag. If this is true, then one of the two larger arc sweeps will be chosen (greater than or equal to 180 degrees)
        ///     </param>
        /// <param name="sweep">sweep flag. If sweep flag is true, then the arc will be drawn in a "positive-angle" direction and if false - in a "negative-angle" direction
        ///     </param>
        /// <param name="x2">x coordinate of ending point</param>
        /// <param name="y2">y coordinate of ending point</param>
        /// <returns>array of {xMin, yMin, xMax, yMax} values</returns>
        private double[] GetEllipticalArcMinMaxPoints(double x1, double y1, double rx, double ry, double phi, bool
             largeArc, bool sweep, double x2, double y2) {
            phi = MathUtil.ToRadians(phi);
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            if (rx == 0.0 || ry == 0.0) {
                return new double[] { Math.Min(x1, x2), Math.Min(y1, y2), Math.Max(x1, x2), Math.Max(y1, y2) };
            }
            double[] centerCoordinatesAndRxRy = GetEllipseCenterCoordinates(x1, y1, rx, ry, phi, largeArc, sweep, x2, 
                y2);
            // the case when radicant is less than 0 and cannot be recalculated. See getEllipseCenterCoordinates() for more info.
            if (centerCoordinatesAndRxRy == null) {
                return new double[] { Math.Min(x1, x2), Math.Min(y1, y2), Math.Max(x1, x2), Math.Max(y1, y2) };
            }
            double cx = centerCoordinatesAndRxRy[0];
            double cy = centerCoordinatesAndRxRy[1];
            // rx and ry values returned cause they can be changed if radicant < 0
            rx = centerCoordinatesAndRxRy[2];
            ry = centerCoordinatesAndRxRy[3];
            double[][] extremeCoordinatesAndThetas = GetExtremeCoordinatesAndAngles(rx, ry, phi, cx, cy);
            double[] extremeCoordinates = extremeCoordinatesAndThetas[0];
            double[] angles = extremeCoordinatesAndThetas[1];
            double xMin = extremeCoordinates[0];
            double yMin = extremeCoordinates[1];
            double xMax = extremeCoordinates[2];
            double yMax = extremeCoordinates[3];
            double xMinAngle = angles[0];
            double yMinAngle = angles[1];
            double xMaxAngle = angles[2];
            double yMaxAngle = angles[3];
            // angles of starting and ending points calculated regarding to centre of ellipse
            double angle1 = GetAngleBetweenVectors(x1 - cx, y1 - cy);
            double angle2 = GetAngleBetweenVectors(x2 - cx, y2 - cy);
            // In case the sweep flag is false,  the angles are decreasing when the ellipse is drawn.
            // So we can just swap them to choose another arc.
            if (!sweep) {
                double temp = angle1;
                angle1 = angle2;
                angle2 = temp;
            }
            // We have difficulty with the fact that the angle of 0 radians is the same as the one of 2*M_PI radians.
            // This passage through the 2*M_PI / 0 border is not very easy to handle directly.
            // That is why we swap the points in case where angle1 > angle2 and will not look in this case for absence of the
            // extreme points on the arc, but for their presence on the complement arc that would close the ellipse.
            bool otherArc = angle1 > angle2;
            if (otherArc) {
                double temp = angle1;
                angle1 = angle2;
                angle2 = temp;
            }
            // If, for example, xMin does not lie on the arc, the new xMin will be the minimum
            // of the x coordinates of the starting and ending points. The same is valid for all other cases.
            if (!IsPointOnTheArc(xMinAngle, angle1, angle2, otherArc)) {
                xMin = Math.Min(x1, x2);
            }
            if (!IsPointOnTheArc(xMaxAngle, angle1, angle2, otherArc)) {
                xMax = Math.Max(x1, x2);
            }
            if (!IsPointOnTheArc(yMinAngle, angle1, angle2, otherArc)) {
                yMin = Math.Min(y1, y2);
            }
            if (!IsPointOnTheArc(yMaxAngle, angle1, angle2, otherArc)) {
                yMax = Math.Max(y1, y2);
            }
            return new double[] { xMin, yMin, xMax, yMax };
        }

        /// <summary>Calculate the center coordinates of the whole ellipse.</summary>
        /// <remarks>
        /// Calculate the center coordinates of the whole ellipse.
        /// Also return rx, ry values since they can be changed in this method.
        /// Algorithm for calculation centre coordinates: https://www.w3.org/TR/SVG/implnote.html#ArcConversionEndpointToCenter
        /// </remarks>
        /// <param name="x1">x coordinate of the starting point</param>
        /// <param name="y1">y coordinate of the starting point</param>
        /// <param name="rx">x radius</param>
        /// <param name="ry">y radius</param>
        /// <param name="phi">x-axis rotation</param>
        /// <param name="largeArc">large arc flag</param>
        /// <param name="sweep">sweep flag</param>
        /// <param name="x2">x coordinate of ending point</param>
        /// <param name="y2">y coordinate of ending point</param>
        /// <returns>the array of {cx, cy, rx, ry} values</returns>
        private double[] GetEllipseCenterCoordinates(double x1, double y1, double rx, double ry, double phi, bool 
            largeArc, bool sweep, double x2, double y2) {
            double x1Prime = Math.Cos(phi) * (x1 - x2) / 2 + Math.Sin(phi) * (y1 - y2) / 2;
            double y1Prime = -Math.Sin(phi) * (x1 - x2) / 2 + Math.Cos(phi) * (y1 - y2) / 2;
            double radicant = (rx * rx * ry * ry - rx * rx * y1Prime * y1Prime - ry * ry * x1Prime * x1Prime);
            radicant /= (rx * rx * y1Prime * y1Prime + ry * ry * x1Prime * x1Prime);
            double cxPrime = 0.0;
            double cyPrime = 0.0;
            if (radicant < 0.0) {
                double ratio = rx / ry;
                radicant = y1Prime * y1Prime + x1Prime * x1Prime / (ratio * ratio);
                if (radicant < 0.0) {
                    return null;
                }
                ry = Math.Sqrt(radicant);
                rx = ratio * ry;
            }
            else {
                double factor = (largeArc == sweep ? -1.0 : 1.0) * Math.Sqrt(radicant);
                cxPrime = factor * rx * y1Prime / ry;
                cyPrime = -factor * ry * x1Prime / rx;
            }
            double cx = cxPrime * Math.Cos(phi) - cyPrime * Math.Sin(phi) + (x1 + x2) / 2;
            double cy = cxPrime * Math.Sin(phi) + cyPrime * Math.Cos(phi) + (y1 + y2) / 2;
            // rx and ry values returned cause they can be changed if radicant < 0
            return new double[] { cx, cy, rx, ry };
        }

        /// <summary>Calculate extremes of the ellipse function and corresponding angles.</summary>
        /// <remarks>
        /// Calculate extremes of the ellipse function and corresponding angles.
        /// Angles are calculated relative to the center of the ellipse.
        /// </remarks>
        /// <param name="rx">x radius</param>
        /// <param name="ry">y radius</param>
        /// <param name="phi">x-axis rotation</param>
        /// <param name="cx">x coordinate of ellipse center</param>
        /// <param name="cy">y coordinate of ellipse center</param>
        /// <returns>array of extreme coordinate and array of angles corresponding to these coordinates.</returns>
        private double[][] GetExtremeCoordinatesAndAngles(double rx, double ry, double phi, double cx, double cy) {
            double xMin;
            double yMin;
            double xMax;
            double yMax;
            double xMinAngle;
            double yMinAngle;
            double xMaxAngle;
            double yMaxAngle;
            if (AnglesAreEquals(phi, 0) || AnglesAreEquals(phi, Math.PI)) {
                xMin = cx - rx;
                xMinAngle = GetAngleBetweenVectors(-rx, 0);
                xMax = cx + rx;
                xMaxAngle = GetAngleBetweenVectors(rx, 0);
                yMin = cy - ry;
                yMinAngle = GetAngleBetweenVectors(0, -ry);
                yMax = cy + ry;
                yMaxAngle = GetAngleBetweenVectors(0, ry);
            }
            else {
                if (AnglesAreEquals(phi, Math.PI / 2.0) || AnglesAreEquals(phi, 3.0 * Math.PI / 2.0)) {
                    xMin = cx - ry;
                    xMinAngle = GetAngleBetweenVectors(-ry, 0);
                    xMax = cx + ry;
                    xMaxAngle = GetAngleBetweenVectors(ry, 0);
                    yMin = cy - rx;
                    yMinAngle = GetAngleBetweenVectors(0, -rx);
                    yMax = cy + rx;
                    yMaxAngle = GetAngleBetweenVectors(0, rx);
                }
                else {
                    // get theta values
                    double txMin = -Math.Atan(ry * Math.Tan(phi) / rx);
                    double txMax = Math.PI - Math.Atan(ry * Math.Tan(phi) / rx);
                    // get x values substituting theta and center coordinates to the ellipse function
                    xMin = cx + rx * Math.Cos(txMin) * Math.Cos(phi) - ry * Math.Sin(txMin) * Math.Sin(phi);
                    xMax = cx + rx * Math.Cos(txMax) * Math.Cos(phi) - ry * Math.Sin(txMax) * Math.Sin(phi);
                    if (xMin > xMax) {
                        double temp = xMin;
                        xMin = xMax;
                        xMax = temp;
                        temp = txMin;
                        txMin = txMax;
                        txMax = temp;
                    }
                    // calculate angles corresponding to extremes
                    double tempY = cy + rx * Math.Cos(txMin) * Math.Sin(phi) + ry * Math.Sin(txMin) * Math.Cos(phi);
                    xMinAngle = GetAngleBetweenVectors(xMin - cx, tempY - cy);
                    tempY = cy + rx * Math.Cos(txMax) * Math.Sin(phi) + ry * Math.Sin(txMax) * Math.Cos(phi);
                    xMaxAngle = GetAngleBetweenVectors(xMax - cx, tempY - cy);
                    // get theta values
                    double tyMin = Math.Atan(ry / (Math.Tan(phi) * rx));
                    double tyMax = Math.Atan(ry / (Math.Tan(phi) * rx)) + Math.PI;
                    // get y values substituting theta and center coordinates to the ellipse function
                    yMin = cy + rx * Math.Cos(tyMin) * Math.Sin(phi) + ry * Math.Sin(tyMin) * Math.Cos(phi);
                    yMax = cy + rx * Math.Cos(tyMax) * Math.Sin(phi) + ry * Math.Sin(tyMax) * Math.Cos(phi);
                    if (yMin > yMax) {
                        double temp = yMin;
                        yMin = yMax;
                        yMax = temp;
                        temp = tyMin;
                        tyMin = tyMax;
                        tyMax = temp;
                    }
                    // calculate angles corresponding to extremes
                    double tmpX = cx + rx * Math.Cos(tyMin) * Math.Cos(phi) - ry * Math.Sin(tyMin) * Math.Sin(phi);
                    yMinAngle = GetAngleBetweenVectors(tmpX - cx, yMin - cy);
                    tmpX = cx + rx * Math.Cos(tyMax) * Math.Cos(phi) - ry * Math.Sin(tyMax) * Math.Sin(phi);
                    yMaxAngle = GetAngleBetweenVectors(tmpX - cx, yMax - cy);
                }
            }
            // extremes
            double[] coordinates = new double[] { xMin, yMin, xMax, yMax };
            // corresponding angles
            double[] angles = new double[] { xMinAngle, yMinAngle, xMaxAngle, yMaxAngle };
            return new double[][] { coordinates, angles };
        }

        /// <summary>Check that angle corresponding to extreme points is on the current arc.</summary>
        /// <remarks>
        /// Check that angle corresponding to extreme points is on the current arc.
        /// For this we check that this angle is between the angles of starting and ending points.
        /// </remarks>
        /// <param name="pointAngle">angle to check</param>
        /// <param name="angle1">angle of the first extreme point if ellipse(starting or ending)</param>
        /// <param name="angle2">angle of the second extreme point if ellipse(starting or ending)</param>
        /// <param name="otherArc">if we should check that point is placed on the other arc of the current ellipse</param>
        /// <returns>true if point is on the arc</returns>
        private bool IsPointOnTheArc(double pointAngle, double angle1, double angle2, bool otherArc) {
            bool isThetaBetweenAngles = angle1 <= pointAngle && angle2 >= pointAngle;
            // in case of other we should make sure that the point is not on the arc
            return otherArc != isThetaBetweenAngles;
        }

        /// <summary>
        /// Return the angle between the vector (1, 0) and the line specified by points (0, 0) and (bx, by) in range [ 0,
        /// Pi/2 ] U [ 3*Pi/2, 2*Pi).
        /// </summary>
        /// <remarks>
        /// Return the angle between the vector (1, 0) and the line specified by points (0, 0) and (bx, by) in range [ 0,
        /// Pi/2 ] U [ 3*Pi/2, 2*Pi).
        /// As the angle between vectors should cover the whole circle, i.e. [0, 2* Pi).
        /// General formula to find angle between two vectors is formula F.6.5.4 on the https://www.w3.org/TR/SVG/implnote.html#ArcConversionEndpointToCenter.
        /// </remarks>
        /// <param name="bx">x coordinate of the vector ending point</param>
        /// <param name="by">y coordinate of the vector ending point</param>
        /// <returns>calculated angle between vectors</returns>
        private double GetAngleBetweenVectors(double bx, double by) {
            return (2 * Math.PI + (by > 0.0 ? 1.0 : -1.0) * Math.Acos(bx / Math.Sqrt(bx * bx + by * by))) % (2 * Math.
                PI);
        }

        private bool AnglesAreEquals(double angle1, double angle2) {
            return Math.Abs(angle1 - angle2) < EPS;
        }
    }
}
