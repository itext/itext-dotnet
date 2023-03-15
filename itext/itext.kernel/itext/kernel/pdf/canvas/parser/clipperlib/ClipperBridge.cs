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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Pdf.Canvas.Parser.ClipperLib {
    /// <summary>
    /// This class contains variety of methods allowing to convert iText
    /// abstractions into the abstractions of the Clipper library and vise versa.
    /// </summary>
    /// <remarks>
    /// This class contains variety of methods allowing to convert iText
    /// abstractions into the abstractions of the Clipper library and vise versa.
    /// <para />
    /// For example:
    /// <list type="bullet">
    /// <item><description>
    /// <see cref="PolyTree"/>
    /// to
    /// <see cref="iText.Kernel.Geom.Path"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="iText.Kernel.Geom.Point"/>
    /// to
    /// <see cref="IntPoint"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="IntPoint"/>
    /// to
    /// <see cref="iText.Kernel.Geom.Point"/>
    /// </description></item>
    /// </list>
    /// </remarks>
    public sealed class ClipperBridge {
        /// <summary>
        /// Since the clipper library uses integer coordinates, we should convert
        /// our floating point numbers into fixed point numbers by multiplying by
        /// this coefficient.
        /// </summary>
        /// <remarks>
        /// Since the clipper library uses integer coordinates, we should convert
        /// our floating point numbers into fixed point numbers by multiplying by
        /// this coefficient. Vary it to adjust the preciseness of the calculations.
        /// </remarks>
        public static double floatMultiplier = Math
                //TODO DEVSIX-5770 make this constant a single non-static configuration
                .Pow(10, 14);

        private ClipperBridge() {
        }

        //empty constructor
        /// <summary>
        /// Converts Clipper library
        /// <see cref="PolyTree"/>
        /// abstraction into iText
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// object.
        /// </summary>
        /// <param name="result">
        /// 
        /// <see cref="PolyTree"/>
        /// object to convert
        /// </param>
        /// <returns>
        /// resultant
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// object
        /// </returns>
        public static Path ConvertToPath(PolyTree result) {
            Path path = new Path();
            PolyNode node = result.GetFirst();
            while (node != null) {
                AddContour(path, node.Contour, !node.IsOpen);
                node = node.GetNext();
            }
            return path;
        }

        /// <summary>
        /// Adds iText
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// to the given
        /// <see cref="Clipper"/>
        /// object.
        /// </summary>
        /// <param name="clipper">
        /// The
        /// <see cref="Clipper"/>
        /// object.
        /// </param>
        /// <param name="path">
        /// The
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// object to be added to the
        /// <see cref="Clipper"/>.
        /// </param>
        /// <param name="polyType">
        /// See
        /// <see cref="PolyType"/>.
        /// </param>
        public static void AddPath(Clipper clipper, Path path, PolyType polyType) {
            foreach (Subpath subpath in path.GetSubpaths()) {
                if (!subpath.IsSinglePointClosed() && !subpath.IsSinglePointOpen()) {
                    IList<Point> linearApproxPoints = subpath.GetPiecewiseLinearApproximation();
                    clipper.AddPath(new List<IntPoint>(ConvertToLongPoints(linearApproxPoints)), polyType, subpath.IsClosed());
                }
            }
        }

        /// <summary>
        /// Adds all iText
        /// <see cref="iText.Kernel.Geom.Subpath"/>
        /// s of the iText
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// to the
        /// <see cref="ClipperOffset"/>
        /// object with one
        /// note: it doesn't add degenerate subpaths.
        /// </summary>
        /// <param name="offset">
        /// the
        /// <see cref="ClipperOffset"/>
        /// object to add all iText
        /// <see cref="iText.Kernel.Geom.Subpath"/>
        /// s that are not degenerated.
        /// </param>
        /// <param name="path">
        /// 
        /// <see cref="iText.Kernel.Geom.Path"/>
        /// object, containing the required
        /// <see cref="iText.Kernel.Geom.Subpath"/>
        /// s
        /// </param>
        /// <param name="joinType">
        /// 
        /// <see cref="Clipper"/>
        /// join type. The value could be
        /// <see cref="JoinType.BEVEL"/>
        /// ,
        /// <see cref="JoinType.ROUND"/>
        /// ,
        /// <see cref="JoinType.MITER"/>
        /// </param>
        /// <param name="endType">
        /// 
        /// <see cref="Clipper"/>
        /// end type. The value could be
        /// <see cref="EndType.CLOSED_POLYGON"/>
        /// ,
        /// <see cref="EndType.CLOSED_LINE"/>
        /// ,
        /// <see cref="EndType.OPEN_BUTT"/>
        /// ,
        /// <see cref="EndType.OPEN_SQUARE"/>
        /// ,
        /// <see cref="EndType.OPEN_ROUND"/>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// consisting of all degenerate iText
        /// <see cref="iText.Kernel.Geom.Subpath"/>
        /// s of the path.
        /// </returns>
        public static IList<Subpath> AddPath(ClipperOffset offset, Path path, JoinType joinType, EndType endType) {
            IList<Subpath> degenerateSubpaths = new List<Subpath>();
            foreach (Subpath subpath in path.GetSubpaths()) {
                if (subpath.IsDegenerate()) {
                    degenerateSubpaths.Add(subpath);
                    continue;
                }
                if (!subpath.IsSinglePointClosed() && !subpath.IsSinglePointOpen()) {
                    EndType et;
                    if (subpath.IsClosed()) {
                        // Offsetting is never used for path being filled
                        et = EndType.CLOSED_LINE;
                    }
                    else {
                        et = endType;
                    }
                    IList<Point> linearApproxPoints = subpath.GetPiecewiseLinearApproximation();
                    offset.AddPath(new List<IntPoint>(ConvertToLongPoints(linearApproxPoints)), joinType, et);
                }
            }
            return degenerateSubpaths;
        }

        /// <summary>
        /// Converts list of
        /// <see cref="IntPoint"/>
        /// objects into list of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects.
        /// </summary>
        /// <param name="points">
        /// the list of
        /// <see cref="IntPoint"/>
        /// objects to convert
        /// </param>
        /// <returns>
        /// the resultant list of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects.
        /// </returns>
        public static IList<Point> ConvertToFloatPoints(IList<IntPoint> points) {
            IList<Point> convertedPoints = new List<Point>(points.Count);
            foreach (IntPoint point in points) {
                convertedPoints.Add(new Point(point.X / floatMultiplier, point.Y / floatMultiplier));
            }
            return convertedPoints;
        }

        /// <summary>
        /// Converts list of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects into list of
        /// <see cref="IntPoint"/>
        /// objects.
        /// </summary>
        /// <param name="points">
        /// the list of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects to convert
        /// </param>
        /// <returns>
        /// the resultant list of
        /// <see cref="IntPoint"/>
        /// objects.
        /// </returns>
        public static IList<IntPoint> ConvertToLongPoints(IList<Point> points) {
            IList<IntPoint> convertedPoints = new List<IntPoint>(points.Count);
            foreach (Point point in points) {
                convertedPoints.Add(new IntPoint(floatMultiplier * point.GetX(), floatMultiplier * point.GetY()));
            }
            return convertedPoints;
        }

        /// <summary>
        /// Converts iText line join style constant into the corresponding constant
        /// of the Clipper library.
        /// </summary>
        /// <param name="lineJoinStyle">
        /// iText line join style constant. See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants"/>
        /// </param>
        /// <returns>Clipper line join style constant.</returns>
        public static JoinType GetJoinType(int lineJoinStyle) {
            switch (lineJoinStyle) {
                case PdfCanvasConstants.LineJoinStyle.BEVEL: {
                    return JoinType.BEVEL;
                }

                case PdfCanvasConstants.LineJoinStyle.MITER: {
                    return JoinType.MITER;
                }
            }
            return JoinType.ROUND;
        }

        /// <summary>
        /// Converts iText line cap style constant into the corresponding constant
        /// of the Clipper library.
        /// </summary>
        /// <param name="lineCapStyle">
        /// iText line cap style constant. See
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants"/>
        /// </param>
        /// <returns>Clipper line cap (end type) style constant.</returns>
        public static EndType GetEndType(int lineCapStyle) {
            switch (lineCapStyle) {
                case PdfCanvasConstants.LineCapStyle.BUTT: {
                    return EndType.OPEN_BUTT;
                }

                case PdfCanvasConstants.LineCapStyle.PROJECTING_SQUARE: {
                    return EndType.OPEN_SQUARE;
                }
            }
            return EndType.OPEN_ROUND;
        }

        /// <summary>
        /// Converts iText filling rule constant into the corresponding constant
        /// of the Clipper library.
        /// </summary>
        /// <param name="fillingRule">
        /// Either
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.FillingRule.EVEN_ODD"/>.
        /// </param>
        /// <returns>Clipper fill type constant.</returns>
        public static PolyFillType GetFillType(int fillingRule) {
            PolyFillType fillType = PolyFillType.NON_ZERO;
            if (fillingRule == PdfCanvasConstants.FillingRule.EVEN_ODD) {
                fillType = PolyFillType.EVEN_ODD;
            }
            return fillType;
        }

        /// <summary>
        /// Adds polygon path based on array of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// (internally converting
        /// them by
        /// <see cref="ConvertToLongPoints(System.Collections.Generic.IList{E})"/>
        /// ) and adds this path to
        /// <see cref="Clipper"/>
        /// instance, treating the path as
        /// a closed polygon.
        /// </summary>
        /// <remarks>
        /// Adds polygon path based on array of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// (internally converting
        /// them by
        /// <see cref="ConvertToLongPoints(System.Collections.Generic.IList{E})"/>
        /// ) and adds this path to
        /// <see cref="Clipper"/>
        /// instance, treating the path as
        /// a closed polygon.
        /// <para />
        /// The return value will be false if the path is invalid for clipping. A path is invalid for clipping when:
        /// <list type="bullet">
        /// <item><description>it has less than 3 vertices;
        /// </description></item>
        /// <item><description>the vertices are all co-linear.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="clipper">
        /// 
        /// <see cref="Clipper"/>
        /// instance to which the created polygon path will be added.
        /// </param>
        /// <param name="polyVertices">
        /// an array of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// which will be internally converted
        /// to clipper path and added to the clipper instance.
        /// </param>
        /// <param name="polyType">
        /// either
        /// <see cref="PolyType.SUBJECT"/>
        /// or
        /// <see cref="PolyType.CLIP"/>
        /// denoting whether added
        /// path is a subject of clipping or a part of the clipping polygon.
        /// </param>
        /// <returns>true if polygon path was successfully added, false otherwise.</returns>
        public static bool AddPolygonToClipper(Clipper clipper, Point[] polyVertices, PolyType polyType) {
            return clipper.AddPath(new List<IntPoint>(ConvertToLongPoints(new List<Point>(JavaUtil.ArraysAsList(polyVertices
                )))), polyType, true);
        }

        /// <summary>
        /// Adds polyline path based on array of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// (internally converting
        /// them by
        /// <see cref="ConvertToLongPoints(System.Collections.Generic.IList{E})"/>
        /// ) and adds this path to
        /// <see cref="Clipper"/>
        /// instance, treating the path as
        /// a polyline (an open path in terms of clipper library).
        /// </summary>
        /// <remarks>
        /// Adds polyline path based on array of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// (internally converting
        /// them by
        /// <see cref="ConvertToLongPoints(System.Collections.Generic.IList{E})"/>
        /// ) and adds this path to
        /// <see cref="Clipper"/>
        /// instance, treating the path as
        /// a polyline (an open path in terms of clipper library). This path is added to the subject of future clipping.
        /// Polylines cannot be part of clipping polygon.
        /// <para />
        /// The return value will be false if the path is invalid for clipping. A path is invalid for clipping when:
        /// <list type="bullet">
        /// <item><description>it has less than 2 vertices;
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="clipper">
        /// 
        /// <see cref="Clipper"/>
        /// instance to which the created polyline path will be added.
        /// </param>
        /// <param name="lineVertices">
        /// an array of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// which will be internally converted
        /// to clipper path and added to the clipper instance.
        /// </param>
        /// <returns>true if polyline path was successfully added, false otherwise.</returns>
        public static bool AddPolylineSubjectToClipper(Clipper clipper, Point[] lineVertices) {
            return clipper.AddPath(new List<IntPoint>(ConvertToLongPoints(new List<Point>(JavaUtil.ArraysAsList(lineVertices
                )))), PolyType.SUBJECT, false);
        }

        internal static void AddContour(Path path, IList<IntPoint> contour, bool close) {
            IList<Point> floatContour = ConvertToFloatPoints(contour);
            Point point = floatContour[0];
            path.MoveTo((float)point.GetX(), (float)point.GetY());
            for (int i = 1; i < floatContour.Count; i++) {
                point = floatContour[i];
                path.LineTo((float)point.GetX(), (float)point.GetY());
            }
            if (close) {
                path.CloseSubpath();
            }
        }
    }
}
