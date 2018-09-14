/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.Text;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
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
    public class PathSvgNodeRenderer : AbstractSvgNodeRenderer {
        private const String SEPARATOR = "";

        private const String SPACE_CHAR = " ";

        /// <summary>
        /// The regular expression to find invalid operators in the <a href="https://www.w3.org/TR/SVG/paths.html#PathData">PathData attribute of the &ltpath&gt element</a>
        /// <p>
        /// Any two consecutive letters are an invalid operator.
        /// </summary>
        private readonly String INVALID_OPERATOR_REGEX = "(\\p{L}{2,})";

        /// <summary>
        /// The regular expression to split the <a href="https://www.w3.org/TR/SVG/paths.html#PathData">PathData attribute of the &ltpath&gt element</a>
        /// <p>
        /// Since
        /// <see cref="ContainsInvalidAttributes(System.String)"/>
        /// is called before the use of this expression in
        /// <see cref="ParsePropertiesAndStyles()"/>
        /// the attribute to be split is valid.
        /// The regex splits at each letter.
        /// </summary>
        private readonly String SPLIT_REGEX = "(?=[\\p{L}])";

        /// <summary>
        /// The
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// representing the current point in the path to be used for relative pathing operations.
        /// The original value is
        /// <see langword="null"/>
        /// , and must be set via a
        /// <see cref="iText.Svg.Renderers.Path.Impl.MoveTo"/>
        /// operation before it may be referenced.
        /// </summary>
        private Point currentPoint = null;

        /// <summary>
        /// The
        /// <see cref="iText.Svg.Renderers.Path.Impl.ClosePath"/>
        /// shape keeping track of the initial point set by a
        /// <see cref="iText.Svg.Renderers.Path.Impl.MoveTo"/>
        /// operation.
        /// The original value is
        /// <see langword="null"/>
        /// , and must be set via a
        /// <see cref="iText.Svg.Renderers.Path.Impl.MoveTo"/>
        /// operation before it may drawn.
        /// </summary>
        private ClosePath zOperator = null;

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            canvas.WriteLiteral("% path\n");
            currentPoint = new Point(0, 0);
            foreach (IPathShape item in GetShapes()) {
                item.Draw(canvas);
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            PathSvgNodeRenderer copy = new PathSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        /// <summary>
        /// Gets the coordinates that shall be passed to
        /// <see cref="iText.Svg.Renderers.Path.IPathShape.SetCoordinates(System.String[])"/>
        /// for the current shape.
        /// </summary>
        /// <param name="shape">The current shape.</param>
        /// <param name="previousShape">The previous shape which can affect the coordinates of the current shape.</param>
        /// <param name="pathProperties">
        /// The operator and all arguments as a
        /// <see>String[]</see>
        /// </param>
        /// <returns>
        /// a
        /// <see>String[]</see>
        /// of coordinates that shall be passed to
        /// <see cref="iText.Svg.Renderers.Path.IPathShape.SetCoordinates(System.String[])"/>
        /// </returns>
        private String[] GetShapeCoordinates(IPathShape shape, IPathShape previousShape, String[] pathProperties) {
            if (shape is ClosePath) {
                return null;
            }
            String[] shapeCoordinates = null;
            String[] operatorArgs = JavaUtil.ArraysCopyOfRange(pathProperties, 1, pathProperties.Length);
            if (shape is SmoothSCurveTo) {
                String[] startingControlPoint = new String[2];
                if (previousShape != null) {
                    IDictionary<String, String> coordinates = previousShape.GetCoordinates();
                    //if the previous command was a C or S use its last control point
                    if (((previousShape is CurveTo))) {
                        float reflectedX = (float)(2 * CssUtils.ParseFloat(coordinates.Get(SvgConstants.Attributes.X)) - CssUtils.
                            ParseFloat(coordinates.Get(SvgConstants.Attributes.X2)));
                        float reflectedy = (float)(2 * CssUtils.ParseFloat(coordinates.Get(SvgConstants.Attributes.Y)) - CssUtils.
                            ParseFloat(coordinates.Get(SvgConstants.Attributes.Y2)));
                        startingControlPoint[0] = SvgCssUtils.ConvertFloatToString(reflectedX);
                        startingControlPoint[1] = SvgCssUtils.ConvertFloatToString(reflectedy);
                    }
                    else {
                        startingControlPoint[0] = coordinates.Get(SvgConstants.Attributes.X);
                        startingControlPoint[1] = coordinates.Get(SvgConstants.Attributes.Y);
                    }
                }
                else {
                    // TODO RND-951
                    startingControlPoint[0] = pathProperties[1];
                    startingControlPoint[1] = pathProperties[2];
                }
                shapeCoordinates = Concatenate(startingControlPoint, operatorArgs);
            }
            else {
                if (shape is VerticalLineTo) {
                    String currentX = currentPoint.x.ToString();
                    String currentY = currentPoint.y.ToString();
                    String[] yValues = Concatenate(new String[] { currentY }, shape.IsRelative() ? MakeRelativeOperatorsAbsolute
                        (operatorArgs, currentPoint.y) : operatorArgs);
                    shapeCoordinates = Concatenate(new String[] { currentX }, yValues);
                }
                else {
                    if (shape is HorizontalLineTo) {
                        String currentX = currentPoint.x.ToString();
                        String currentY = currentPoint.y.ToString();
                        String[] xValues = Concatenate(new String[] { currentX }, shape.IsRelative() ? MakeRelativeOperatorsAbsolute
                            (operatorArgs, currentPoint.x) : operatorArgs);
                        shapeCoordinates = Concatenate(new String[] { currentY }, xValues);
                    }
                }
            }
            if (shapeCoordinates == null) {
                shapeCoordinates = operatorArgs;
            }
            return shapeCoordinates;
        }

        private String[] MakeRelativeOperatorsAbsolute(String[] relativeOperators, double currentCoordinate) {
            String[] absoluteOperators = new String[relativeOperators.Length];
            for (int i = 0; i < relativeOperators.Length; i++) {
                double relativeDouble = Double.Parse(relativeOperators[i], System.Globalization.CultureInfo.InvariantCulture
                    );
                relativeDouble += currentCoordinate;
                absoluteOperators[i] = relativeDouble.ToString();
            }
            return absoluteOperators;
        }

        /// <summary>
        /// Processes an individual pathing operator and all of its arguments, converting into one or more
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// objects.
        /// </summary>
        /// <param name="pathProperties">
        /// The property operator and all arguments as a
        /// <see>String[]</see>
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
            if (pathProperties.Length == 0 || pathProperties[0].Equals(SEPARATOR)) {
                return shapes;
            }
            //Implements (absolute) command value only
            //TODO implement relative values e. C(absolute), c(relative)
            IPathShape pathShape = SvgPathShapeFactory.CreatePathShape(pathProperties[0]);
            String[] shapeCoordinates = GetShapeCoordinates(pathShape, previousShape, pathProperties);
            if (pathShape is ClosePath) {
                pathShape = zOperator;
                if (pathShape == null) {
                    throw new SvgProcessingException(SvgLogMessageConstant.INVALID_CLOSEPATH_OPERATOR_USE);
                }
            }
            else {
                if (pathShape is MoveTo) {
                    zOperator = new ClosePath();
                    zOperator.SetCoordinates(shapeCoordinates);
                }
            }
            if (pathShape != null) {
                if (shapeCoordinates != null) {
                    pathShape.SetCoordinates(shapeCoordinates);
                }
                currentPoint = pathShape.GetEndingPoint();
                // unsupported operators are ignored.
                shapes.Add(pathShape);
            }
            return shapes;
        }

        /// <summary>
        /// Processes the
        /// <see cref="SvgConstants.Attributes.D"/>
        /// 
        /// <see cref="AbstractSvgNodeRenderer.attributesAndStyles"/>
        /// and converts them
        /// into one or more
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// objects to be drawn on the canvas.
        /// <p>
        /// Each individual operator is passed to
        /// <see cref="ProcessPathOperator(System.String[], iText.Svg.Renderers.Path.IPathShape)"/>
        /// to be processed individually.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.ICollection{E}"/>
        /// of each
        /// <see cref="iText.Svg.Renderers.Path.IPathShape"/>
        /// that should be drawn to represent the path.
        /// </returns>
        private ICollection<IPathShape> GetShapes() {
            ICollection<String> parsedResults = ParsePropertiesAndStyles();
            IList<IPathShape> shapes = new List<IPathShape>();
            foreach (String parsedResult in parsedResults) {
                String[] pathProperties = iText.IO.Util.StringUtil.Split(parsedResult, SPACE_CHAR);
                IPathShape previousShape = shapes.Count == 0 ? null : shapes[shapes.Count - 1];
                IList<IPathShape> operatorShapes = ProcessPathOperator(pathProperties, previousShape);
                shapes.AddAll(operatorShapes);
            }
            return shapes;
        }

        private static String[] Concatenate(String[] first, String[] second) {
            String[] arr = new String[first.Length + second.Length];
            Array.Copy(first, 0, arr, 0, first.Length);
            Array.Copy(second, 0, arr, first.Length, second.Length);
            return arr;
        }

        private bool ContainsInvalidAttributes(String attributes) {
            return iText.IO.Util.StringUtil.Split(attributes, INVALID_OPERATOR_REGEX).Length > 1;
        }

        private ICollection<String> ParsePropertiesAndStyles() {
            StringBuilder result = new StringBuilder();
            String attributes = attributesAndStyles.Get(SvgConstants.Attributes.D);
            if (ContainsInvalidAttributes(attributes)) {
                throw new SvgProcessingException(SvgLogMessageConstant.INVALID_PATH_D_ATTRIBUTE_OPERATORS).SetMessageParams
                    (attributes);
            }
            String[] coordinates = iText.IO.Util.StringUtil.Split(attributes, SPLIT_REGEX);
            //gets an array attributesAr of {M 100 100, L 300 100, L200, 300, z}
            foreach (String inst in coordinates) {
                if (!inst.Equals(SEPARATOR)) {
                    String instTrim = inst.Trim();
                    String instruction = instTrim[0] + SPACE_CHAR;
                    String temp = instruction + instTrim.Replace(instTrim[0] + SEPARATOR, SEPARATOR).Replace(",", SPACE_CHAR).
                        Trim();
                    result.Append(SPACE_CHAR);
                    result.Append(temp);
                }
            }
            String[] resultArray = iText.IO.Util.StringUtil.Split(result.ToString(), SPLIT_REGEX);
            IList<String> resultList = new List<String>(JavaUtil.ArraysAsList(resultArray));
            return resultList;
        }
    }
}
