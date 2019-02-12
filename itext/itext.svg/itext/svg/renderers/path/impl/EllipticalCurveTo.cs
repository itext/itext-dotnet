using System;
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements elliptical curveTo (A) segment of SVG's path element.</summary>
    /// <remarks>
    /// Implements elliptical curveTo (A) segment of SVG's path element. Implemented in PDF as Bézier curves.
    /// Edge cases &amp; value correction below always refer to https://www.w3.org/TR/SVG11/implnote.html#ArcImplementationNotes
    /// For some calculations we need double precision floating point math, so we have forced all calculations to use double.
    /// However, float comparison is used instead of double comparison, because close coordinates can be considered equal.
    /// </remarks>
    public class EllipticalCurveTo : AbstractPathShape {
        private String[][] coordinates;

        private Point startPoint;

        /// <summary>Creates an absolute Elliptical curveTo.</summary>
        public EllipticalCurveTo()
            : this(false) {
        }

        /// <summary>Creates a Elliptical curveTo.</summary>
        /// <remarks>Creates a Elliptical curveTo. Set argument to true to create a relative EllipticalCurveTo.</remarks>
        /// <param name="relative">whether this is a relative EllipticalCurveTo or not</param>
        public EllipticalCurveTo(bool relative) {
            base.relative = relative;
        }

        public override void SetCoordinates(String[] coordinates, Point previous) {
            startPoint = previous;
            if (coordinates.Length == 0 || coordinates.Length % 7 != 0) {
                LogManager.GetLogger(typeof(iText.Svg.Renderers.Path.Impl.EllipticalCurveTo)).Warn(MessageFormatUtil.Format
                    (SvgLogMessageConstant.ARC_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0, JavaUtil.ArraysToString(coordinates)
                    ));
            }
            this.coordinates = new String[coordinates.Length / 7][];
            double[] initialPoint = new double[] { previous.GetX(), previous.GetY() };
            // ignore partial argument groups, which do not form a fixed set of 7 elements
            for (int i = 0; i / 7 < coordinates.Length / 7; i += 7) {
                String[] curCoordinates = new String[] { coordinates[i], coordinates[i + 1], coordinates[i + 2], coordinates
                    [i + 3], coordinates[i + 4], coordinates[i + 5], coordinates[i + 6] };
                if (IsRelative()) {
                    String[] relativeEndPoint = new String[] { coordinates[i + 5], coordinates[i + 6] };
                    String[] absoluteEndPoint = SvgCoordinateUtils.MakeRelativeOperatorCoordinatesAbsolute(relativeEndPoint, initialPoint
                        );
                    curCoordinates[i + 5] = absoluteEndPoint[0];
                    curCoordinates[i + 6] = absoluteEndPoint[1];
                    initialPoint[0] = Double.Parse(curCoordinates[i + 5], System.Globalization.CultureInfo.InvariantCulture);
                    initialPoint[1] = Double.Parse(curCoordinates[i + 6], System.Globalization.CultureInfo.InvariantCulture);
                }
                this.coordinates[i / 7] = curCoordinates;
            }
        }

        public override void Draw(PdfCanvas canvas) {
            Point previousPoint = startPoint;
            foreach (String[] coordinate in coordinates) {
                Point start = new Point(previousPoint.x * .75, previousPoint.y * .75);
                // pixels to points
                double rx = Math.Abs(CssUtils.ParseAbsoluteLength(coordinate[0]));
                double ry = Math.Abs(CssUtils.ParseAbsoluteLength(coordinate[1]));
                // φ is taken mod 360 degrees.
                double rotation = Double.Parse(coordinate[2], System.Globalization.CultureInfo.InvariantCulture) % 360.0;
                // rotation argument is given in degrees, but we need radians for easier trigonometric calculations
                rotation = MathUtil.ToRadians(rotation);
                // binary flags (Value correction: any nonzero value for either of the flags fA or fS is taken to mean the value 1.)
                bool largeArc = !CssUtils.CompareFloats((float)CssUtils.ParseFloat(coordinate[3]), 0);
                bool sweep = !CssUtils.CompareFloats((float)CssUtils.ParseFloat(coordinate[4]), 0);
                Point end = new Point(CssUtils.ParseAbsoluteLength(coordinate[5]), CssUtils.ParseAbsoluteLength(coordinate
                    [6]));
                if (CssUtils.CompareFloats(start.x, end.x) && CssUtils.CompareFloats(start.y, end.y)) {
                    /* edge case: If the endpoints (x1, y1) and (x2, y2) are identical,
                    * then this is equivalent to omitting the elliptical arc segment entirely.
                    */
                    continue;
                }
                else {
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
                previousPoint = CreatePoint(coordinate[5], coordinate[6]);
            }
        }

        public override Point GetEndingPoint() {
            return CreatePoint(coordinates[coordinates.Length - 1][5], coordinates[coordinates.Length - 1][6]);
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

        internal virtual String[][] GetCoordinates() {
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
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// .
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
                    long deltaTheta = (long)Math.Abs(Math.Round(theta2 - theta1));
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
    }
}
