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
using System.Text;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Path;
using iText.Svg.Renderers.Path.Impl;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;path&gt; tag.
    /// </summary>
    public class PathSvgNodeRenderer : AbstractSvgNodeRenderer, IMarkerCapable {
        private const String SPACE_CHAR = " ";

        /// <summary>
        /// The regular expression to find invalid operators in the <a href="https://www.w3.org/tr/svg/paths.html#pathdata">PathData
        /// attribute of the &lt;path&gt; element</a>
        /// </summary>
        /// <remarks>
        /// The regular expression to find invalid operators in the <a href="https://www.w3.org/tr/svg/paths.html#pathdata">PathData
        /// attribute of the &lt;path&gt; element</a>
        /// <para />
        /// Find any occurrence of a letter that is not an operator
        /// </remarks>
        private const String INVALID_OPERATOR_REGEX = "(?:(?![mzlhvcsqtae])\\p{L})";

        private static readonly Regex INVALID_REGEX_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile(INVALID_OPERATOR_REGEX
            , System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        /// <summary>
        /// The regular expression to split the <a href="https://www.w3.org/tr/svg/paths.html#pathdata">PathData attribute of
        /// the &lt;path&gt; element</a>
        /// </summary>
        /// <remarks>
        /// The regular expression to split the <a href="https://www.w3.org/tr/svg/paths.html#pathdata">PathData attribute of
        /// the &lt;path&gt; element</a>
        /// <para />
        /// Since
        /// <see cref="ContainsInvalidAttributes(System.String)"/>
        /// is called before the use of this expression
        /// in
        /// <see cref="ParsePathOperations()"/>
        /// the attribute to be split is valid.
        /// <para />
        /// SVG defines 6 types of path commands, for a total of 20 commands:
        /// <para />
        /// MoveTo: M, m
        /// LineTo: L, l, H, h, V, v
        /// Cubic Bezier Curve: C, c, S, s
        /// Quadratic Bezier Curve: Q, q, T, t
        /// Elliptical Arc Curve: A, a
        /// ClosePath: Z, z
        /// </remarks>
        private static readonly Regex SPLIT_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("(?=[mlhvcsqtaz])"
            , System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        /// <summary>
        /// The
        /// <see cref="iText.Svg.Renderers.Path.Impl.ClosePath"/>
        /// shape keeping track of the initial point set by a
        /// <see cref="iText.Svg.Renderers.Path.Impl.MoveTo"/>
        /// operation.
        /// </summary>
        /// <remarks>
        /// The
        /// <see cref="iText.Svg.Renderers.Path.Impl.ClosePath"/>
        /// shape keeping track of the initial point set by a
        /// <see cref="iText.Svg.Renderers.Path.Impl.MoveTo"/>
        /// operation.
        /// The original value is
        /// <see langword="null"/>
        /// , and must be set via a
        /// <see cref="iText.Svg.Renderers.Path.Impl.MoveTo"/>
        /// operation before it may be drawn.
        /// </remarks>
        private ClosePath zOperator = null;

        /// <summary>Draws this element to a canvas-like object maintained in the context.</summary>
        /// <param name="context">the object that knows the place to draw this element and maintains its state</param>
        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            canvas.WriteLiteral("% path\n");
            foreach (IPathShape item in GetShapes()) {
                if (item is AbstractPathShape) {
                    AbstractPathShape shape = (AbstractPathShape)item;
                    shape.SetParent(this);
                    shape.SetContext(context);
                }
                item.Draw(context.GetCurrentCanvas());
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            PathSvgNodeRenderer copy = new PathSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            Point lastPoint = null;
            Rectangle commonRectangle = null;
            foreach (IPathShape item in GetShapes()) {
                if (lastPoint == null) {
                    lastPoint = item.GetEndingPoint();
                }
                Rectangle rectangle = item.GetPathShapeRectangle(lastPoint);
                commonRectangle = Rectangle.GetCommonRectangle(commonRectangle, rectangle);
                lastPoint = item.GetEndingPoint();
            }
            return commonRectangle;
        }

        /// <summary>
        /// Gets the coordinates that shall be passed to
        /// <see cref="iText.Svg.Renderers.Path.IPathShape.SetCoordinates(System.String[], iText.Kernel.Geom.Point)"/>
        /// for the current shape.
        /// </summary>
        /// <param name="shape">The current shape.</param>
        /// <param name="previousShape">The previous shape which can affect the coordinates of the current shape.</param>
        /// <param name="pathProperties">
        /// The operator and all arguments as an array of
        /// <see cref="System.String">String</see>
        /// s
        /// </param>
        /// <returns>
        /// a
        /// <see cref="System.String"/>
        /// array of coordinates that shall be passed to
        /// <see cref="iText.Svg.Renderers.Path.IPathShape.SetCoordinates(System.String[], iText.Kernel.Geom.Point)"/>
        /// </returns>
        private String[] GetShapeCoordinates(IPathShape shape, IPathShape previousShape, String[] pathProperties) {
            if (shape is ClosePath) {
                return null;
            }
            String[] shapeCoordinates = null;
            if (shape is SmoothSCurveTo || shape is QuadraticSmoothCurveTo) {
                String[] startingControlPoint = new String[2];
                if (previousShape != null) {
                    Point previousEndPoint = previousShape.GetEndingPoint();
                    // If the previous command was a Bezier curve, use its last control point
                    if (previousShape is IControlPointCurve) {
                        Point lastControlPoint = ((IControlPointCurve)previousShape).GetLastControlPoint();
                        float reflectedX = (float)(2 * previousEndPoint.GetX() - lastControlPoint.GetX());
                        float reflectedY = (float)(2 * previousEndPoint.GetY() - lastControlPoint.GetY());
                        startingControlPoint[0] = SvgCssUtils.ConvertFloatToString(reflectedX);
                        startingControlPoint[1] = SvgCssUtils.ConvertFloatToString(reflectedY);
                    }
                    else {
                        startingControlPoint[0] = SvgCssUtils.ConvertDoubleToString(previousEndPoint.GetX());
                        startingControlPoint[1] = SvgCssUtils.ConvertDoubleToString(previousEndPoint.GetY());
                    }
                }
                else {
                    throw new SvgProcessingException(SvgExceptionMessageConstant.INVALID_SMOOTH_CURVE_USE);
                }
                shapeCoordinates = Concatenate(startingControlPoint, pathProperties);
            }
            if (shapeCoordinates == null) {
                shapeCoordinates = pathProperties;
            }
            return shapeCoordinates;
        }

        /// <summary>
        /// Processes an individual pathing operator and all of its arguments, converting into one or more
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// objects.
        /// </summary>
        /// <param name="pathProperties">
        /// The property operator and all arguments as an array of
        /// <see cref="System.String"/>
        /// s
        /// </param>
        /// <param name="previousShape">
        /// The previous shape which can affect the positioning of the current shape. If no previous
        /// shape exists
        /// <see langword="null"/>
        /// is passed.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// of each
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// that should be drawn to represent the operator.
        /// </returns>
        private IList<IPathShape> ProcessPathOperator(String[] pathProperties, IPathShape previousShape) {
            IList<IPathShape> shapes = new List<IPathShape>();
            if (pathProperties.Length == 0 || String.IsNullOrEmpty(pathProperties[0]) || SvgPathShapeFactory.GetArgumentCount
                (pathProperties[0]) < 0) {
                return shapes;
            }
            int argumentCount = SvgPathShapeFactory.GetArgumentCount(pathProperties[0]);
            if (argumentCount == 0) {
                // closePath operator
                if (previousShape == null) {
                    throw new SvgProcessingException(SvgExceptionMessageConstant.INVALID_CLOSEPATH_OPERATOR_USE);
                }
                shapes.Add(zOperator);
                return shapes;
            }
            for (int index = 1; index < pathProperties.Length; index += argumentCount) {
                if (index + argumentCount > pathProperties.Length) {
                    break;
                }
                IPathShape pathShape = SvgPathShapeFactory.CreatePathShape(pathProperties[0]);
                if (pathShape is MoveTo) {
                    shapes.AddAll(AddMoveToShapes(pathShape, pathProperties, previousShape));
                    return shapes;
                }
                String[] shapeCoordinates = GetShapeCoordinates(pathShape, previousShape, JavaUtil.ArraysCopyOfRange(pathProperties
                    , index, index + argumentCount));
                if (pathShape != null) {
                    if (shapeCoordinates != null) {
                        pathShape.SetCoordinates(shapeCoordinates, GetCurrentPoint(previousShape));
                    }
                    shapes.Add(pathShape);
                }
                previousShape = pathShape;
            }
            return shapes;
        }

        private IList<IPathShape> AddMoveToShapes(IPathShape pathShape, String[] pathProperties, IPathShape beforeMoveShape
            ) {
            IList<IPathShape> shapes = new List<IPathShape>();
            int argumentCount = 2;
            String[] shapeCoordinates = GetShapeCoordinates(pathShape, beforeMoveShape, JavaUtil.ArraysCopyOfRange(pathProperties
                , 1, 3));
            zOperator = new ClosePath(pathShape.IsRelative());
            Point currentPointBeforeMove = GetCurrentPoint(beforeMoveShape);
            zOperator.SetCoordinates(shapeCoordinates, currentPointBeforeMove);
            pathShape.SetCoordinates(shapeCoordinates, currentPointBeforeMove);
            shapes.Add(pathShape);
            IPathShape previousShape = pathShape;
            if (pathProperties.Length > 3) {
                for (int index = 3; index < pathProperties.Length; index += argumentCount) {
                    if (index + 2 > pathProperties.Length) {
                        break;
                    }
                    pathShape = pathShape.IsRelative() ? SvgPathShapeFactory.CreatePathShape("l") : SvgPathShapeFactory.CreatePathShape
                        ("L");
                    shapeCoordinates = GetShapeCoordinates(pathShape, previousShape, JavaUtil.ArraysCopyOfRange(pathProperties
                        , index, index + 2));
                    pathShape.SetCoordinates(shapeCoordinates, previousShape.GetEndingPoint());
                    shapes.Add(pathShape);
                    previousShape = pathShape;
                }
            }
            return shapes;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Processes the
        /// <see cref="iText.Svg.SvgConstants.Attributes.D"/>
        /// 
        /// <see cref="AbstractSvgNodeRenderer.attributesAndStyles"/>
        /// and converts them
        /// into one or more
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// objects to be drawn on the canvas.
        /// </summary>
        /// <remarks>
        /// Processes the
        /// <see cref="iText.Svg.SvgConstants.Attributes.D"/>
        /// 
        /// <see cref="AbstractSvgNodeRenderer.attributesAndStyles"/>
        /// and converts them
        /// into one or more
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// objects to be drawn on the canvas.
        /// <para />
        /// Each individual operator is passed to
        /// <see cref="ProcessPathOperator(System.String[], iText.Svg.Renderers.Path.IPathShape)"/>
        /// to be
        /// processed individually.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of each
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// that should be drawn to represent the path.
        /// </returns>
        internal virtual ICollection<IPathShape> GetShapes() {
            ICollection<String> parsedResults = ParsePathOperations();
            IList<IPathShape> shapes = new List<IPathShape>();
            foreach (String parsedResult in parsedResults) {
                String[] pathProperties = iText.Commons.Utils.StringUtil.Split(parsedResult, " +");
                IPathShape previousShape = shapes.Count == 0 ? null : shapes[shapes.Count - 1];
                IList<IPathShape> operatorShapes = ProcessPathOperator(pathProperties, previousShape);
                shapes.AddAll(operatorShapes);
            }
            return shapes;
        }
//\endcond

        private static String[] Concatenate(String[] first, String[] second) {
            String[] arr = new String[first.Length + second.Length];
            Array.Copy(first, 0, arr, 0, first.Length);
            Array.Copy(second, 0, arr, first.Length, second.Length);
            return arr;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool ContainsInvalidAttributes(String attributes) {
            return iText.Commons.Utils.Matcher.Match(INVALID_REGEX_PATTERN, attributes).Find();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<String> ParsePathOperations() {
            ICollection<String> result = new List<String>();
            String pathString = attributesAndStyles.Get(SvgConstants.Attributes.D);
            if (pathString == null) {
                pathString = "";
            }
            if (ContainsInvalidAttributes(pathString)) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.INVALID_PATH_D_ATTRIBUTE_OPERATORS).SetMessageParams
                    (pathString);
            }
            pathString = iText.Commons.Utils.StringUtil.ReplaceAll(pathString, "\\s+", " ").Trim();
            String[] operators = SplitPathStringIntoOperators(pathString);
            foreach (String inst in operators) {
                String instTrim = inst.Trim();
                if (!String.IsNullOrEmpty(instTrim)) {
                    char instruction = instTrim[0];
                    String temp = instruction + SPACE_CHAR + instTrim.Substring(1).Replace(",", SPACE_CHAR).Trim();
                    // Do a run-through for decimal point separation
                    temp = SeparateDecimalPoints(temp);
                    result.Add(temp);
                }
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Iterate over the input string and separate numbers from each other with space chars</summary>
        internal virtual String SeparateDecimalPoints(String input) {
            // If a space or minus sign is found reset
            // If a another point is found, add an extra space on before the point
            StringBuilder res = new StringBuilder();
            // We are now among the digits to the right of the decimal point
            bool fractionalPartAfterDecimalPoint = false;
            // We are now among the exponent magnitude part
            bool exponentSignMagnitude = false;
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                // Resetting flags
                if (c == '-' || iText.IO.Util.TextUtil.IsWhiteSpace(c)) {
                    fractionalPartAfterDecimalPoint = false;
                }
                if (iText.IO.Util.TextUtil.IsWhiteSpace(c)) {
                    exponentSignMagnitude = false;
                }
                // Add extra space before the next number starting from '.', or before the next number starting with '-'
                if (EndsWithNonWhitespace(res) && (c == '.' && fractionalPartAfterDecimalPoint || c == '-' && !exponentSignMagnitude
                    )) {
                    res.Append(" ");
                }
                if (c == '.') {
                    fractionalPartAfterDecimalPoint = true;
                }
                else {
                    if (char.ToLower(c) == 'e') {
                        exponentSignMagnitude = true;
                    }
                }
                res.Append(c);
            }
            return res.ToString();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Gets an array of strings representing operators with their arguments, e.g. {"M 100 100", "L 300 100", "L200,
        /// 300", "z"}
        /// </summary>
        internal static String[] SplitPathStringIntoOperators(String path) {
            return iText.Commons.Utils.StringUtil.Split(SPLIT_PATTERN, path);
        }
//\endcond

        private static bool EndsWithNonWhitespace(StringBuilder sb) {
            return sb.Length > 0 && !iText.IO.Util.TextUtil.IsWhiteSpace(sb[sb.Length - 1]);
        }

        public virtual void DrawMarker(SvgDrawContext context, MarkerVertexType markerVertexType) {
            Object[] allShapesOrdered = GetShapes().ToArray();
            Point point = null;
            if (MarkerVertexType.MARKER_START.Equals(markerVertexType)) {
                point = ((AbstractPathShape)allShapesOrdered[0]).GetEndingPoint();
            }
            else {
                if (MarkerVertexType.MARKER_END.Equals(markerVertexType)) {
                    point = ((AbstractPathShape)allShapesOrdered[allShapesOrdered.Length - 1]).GetEndingPoint();
                }
            }
            if (point != null) {
                String moveX = SvgCssUtils.ConvertDoubleToString(point.GetX());
                String moveY = SvgCssUtils.ConvertDoubleToString(point.GetY());
                MarkerSvgNodeRenderer.DrawMarker(context, moveX, moveY, markerVertexType, this);
            }
        }

        public virtual double GetAutoOrientAngle(MarkerSvgNodeRenderer marker, bool reverse) {
            Object[] pathShapes = GetShapes().ToArray();
            if (pathShapes.Length > 1) {
                Vector v = new Vector(0, 0, 0);
                if (SvgConstants.Attributes.MARKER_END.Equals(marker.attributesAndStyles.Get(SvgConstants.Tags.MARKER))) {
                    // Create vector from the last two shapes
                    IPathShape lastShape = (IPathShape)pathShapes[pathShapes.Length - 1];
                    IPathShape secondToLastShape = (IPathShape)pathShapes[pathShapes.Length - 2];
                    v = new Vector((float)(lastShape.GetEndingPoint().GetX() - secondToLastShape.GetEndingPoint().GetX()), (float
                        )(lastShape.GetEndingPoint().GetY() - secondToLastShape.GetEndingPoint().GetY()), 0f);
                }
                else {
                    if (SvgConstants.Attributes.MARKER_START.Equals(marker.attributesAndStyles.Get(SvgConstants.Tags.MARKER))) {
                        // Create vector from the first two shapes
                        IPathShape firstShape = (IPathShape)pathShapes[0];
                        IPathShape secondShape = (IPathShape)pathShapes[1];
                        v = new Vector((float)(secondShape.GetEndingPoint().GetX() - firstShape.GetEndingPoint().GetX()), (float)(
                            secondShape.GetEndingPoint().GetY() - firstShape.GetEndingPoint().GetY()), 0f);
                    }
                }
                // Get angle from this vector and the horizontal axis
                Vector xAxis = new Vector(1, 0, 0);
                double rotAngle = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(xAxis, v);
                return v.Get(1) >= 0 && !reverse ? rotAngle : rotAngle * -1f;
            }
            return 0;
        }

        private static Point GetCurrentPoint(IPathShape previousShape) {
            return previousShape == null ? new Point(0, 0) : previousShape.GetEndingPoint();
        }
    }
}
