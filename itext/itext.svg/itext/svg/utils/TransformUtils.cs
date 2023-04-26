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
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    /// <summary>
    /// Utility class responsible for converting Strings containing transformation declarations
    /// into AffineTransform objects.
    /// </summary>
    /// <remarks>
    /// Utility class responsible for converting Strings containing transformation declarations
    /// into AffineTransform objects.
    /// <para />
    /// This class only supports the transformations as described in the SVG specification:
    /// - matrix
    /// - rotate
    /// - scale
    /// - skewX
    /// - skewY
    /// - translate
    /// </remarks>
    public sealed class TransformUtils {
        /// <summary>Keyword for matrix transformations.</summary>
        /// <remarks>
        /// Keyword for matrix transformations. Accepts 6 values.
        /// <para />
        /// matrix(0 1 2 3 4 5)
        /// </remarks>
        private const String MATRIX = "MATRIX";

        /// <summary>Keyword for rotation transformation.</summary>
        /// <remarks>
        /// Keyword for rotation transformation. Accepts either 1 or 3 values.
        /// In the case of 1 value, x and y are assumed to be the origin of the user space.
        /// <para />
        /// rotate(angle x y)
        /// rotate(angle)
        /// </remarks>
        private const String ROTATE = "ROTATE";

        /// <summary>Keyword for scale transformation.</summary>
        /// <remarks>
        /// Keyword for scale transformation. Accepts either 1 or 2 values.
        /// In the case of 1 value, the second value is assumed to be the same as the first one.
        /// <para />
        /// scale(x y)
        /// scale(x)
        /// </remarks>
        private const String SCALE = "SCALE";

        /// <summary>Keyword for skewX transformation.</summary>
        /// <remarks>
        /// Keyword for skewX transformation. Accepts 1 value.
        /// <para />
        /// skewX(angle)
        /// </remarks>
        private const String SKEWX = "SKEWX";

        /// <summary>Keyword for skewY transformation.</summary>
        /// <remarks>
        /// Keyword for skewY transformation. Accepts 1 value.
        /// <para />
        /// skewY(angle)
        /// </remarks>
        private const String SKEWY = "SKEWY";

        /// <summary>Keyword for translate transformation.</summary>
        /// <remarks>
        /// Keyword for translate transformation. Accepts either 1 or 2 values.
        /// In the case of 1 value, the y value is assumed to be 0.
        /// <para />
        /// translate(x y)
        /// translate(x)
        /// </remarks>
        private const String TRANSLATE = "TRANSLATE";

        private TransformUtils() {
        }

        /// <summary>Converts a string containing a transform declaration into an AffineTransform object.</summary>
        /// <remarks>
        /// Converts a string containing a transform declaration into an AffineTransform object.
        /// This class only supports the transformations as described in the SVG specification:
        /// - matrix
        /// - translate
        /// - skewx
        /// - skewy
        /// - rotate
        /// - scale
        /// </remarks>
        /// <param name="transform">value to be parsed</param>
        /// <returns>the AffineTransform object</returns>
        public static AffineTransform ParseTransform(String transform) {
            if (transform == null) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_NULL);
            }
            if (String.IsNullOrEmpty(transform)) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_EMPTY);
            }
            AffineTransform matrix = new AffineTransform();
            IList<String> listWithTransformations = SplitString(transform);
            foreach (String transformation in listWithTransformations) {
                AffineTransform newMatrix = TransformationStringToMatrix(transformation);
                if (newMatrix != null) {
                    matrix.Concatenate(newMatrix);
                }
            }
            return matrix;
        }

        /// <summary>A transformation attribute can encompass multiple transformation operation (e.g. "translate(10,20) scale(30,40)".
        ///     </summary>
        /// <remarks>
        /// A transformation attribute can encompass multiple transformation operation (e.g. "translate(10,20) scale(30,40)".
        /// This method splits the original transformation string into multiple strings so that they can be handled separately.
        /// </remarks>
        /// <param name="transform">the transformation value</param>
        /// <returns>a list containing strings describing a single transformation operation</returns>
        private static IList<String> SplitString(String transform) {
            List<String> list = new List<String>();
            StringTokenizer tokenizer = new StringTokenizer(transform, ")", false);
            while (tokenizer.HasMoreTokens()) {
                String trim = tokenizer.NextToken().Trim();
                if (trim != null && !String.IsNullOrEmpty(trim)) {
                    list.Add(trim + ")");
                }
            }
            return list;
        }

        /// <summary>This method decides which transformation operation the given transformation strings maps onto.</summary>
        /// <param name="transformation">string containing a transformation operation</param>
        /// <returns>the mapped AffineTransform object</returns>
        private static AffineTransform TransformationStringToMatrix(String transformation) {
            String name = GetNameFromString(transformation).ToUpperInvariant();
            if (String.IsNullOrEmpty(name)) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.INVALID_TRANSFORM_DECLARATION);
            }
            switch (name) {
                case MATRIX: {
                    return CreateMatrixTransformation(GetValuesFromTransformationString(transformation));
                }

                case TRANSLATE: {
                    return CreateTranslateTransformation(GetValuesFromTransformationString(transformation));
                }

                case SCALE: {
                    return CreateScaleTransformation(GetValuesFromTransformationString(transformation));
                }

                case ROTATE: {
                    return CreateRotationTransformation(GetValuesFromTransformationString(transformation));
                }

                case SKEWX: {
                    return CreateSkewXTransformation(GetValuesFromTransformationString(transformation));
                }

                case SKEWY: {
                    return CreateSkewYTransformation(GetValuesFromTransformationString(transformation));
                }

                default: {
                    throw new SvgProcessingException(SvgExceptionMessageConstant.UNKNOWN_TRANSFORMATION_TYPE);
                }
            }
        }

        /// <summary>Creates a skewY transformation.</summary>
        /// <param name="values">values of the transformation</param>
        /// <returns>AffineTransform for the skew operation</returns>
        private static AffineTransform CreateSkewYTransformation(IList<String> values) {
            if (values.Count != 1) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES);
            }
            double tan = Math.Tan(MathUtil.ToRadians((float)CssDimensionParsingUtils.ParseFloat(values[0])));
            //Differs from the notation in the PDF-spec for skews
            return new AffineTransform(1, tan, 0, 1, 0, 0);
        }

        /// <summary>Creates a skewX transformation.</summary>
        /// <param name="values">values of the transformation</param>
        /// <returns>AffineTransform for the skew operation</returns>
        private static AffineTransform CreateSkewXTransformation(IList<String> values) {
            if (values.Count != 1) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES);
            }
            double tan = Math.Tan(MathUtil.ToRadians((float)CssDimensionParsingUtils.ParseFloat(values[0])));
            //Differs from the notation in the PDF-spec for skews
            return new AffineTransform(1, 0, tan, 1, 0, 0);
        }

        /// <summary>Creates a rotate transformation.</summary>
        /// <param name="values">values of the transformation</param>
        /// <returns>AffineTransform for the rotate operation</returns>
        private static AffineTransform CreateRotationTransformation(IList<String> values) {
            if (values.Count != 1 && values.Count != 3) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES);
            }
            double angle = MathUtil.ToRadians((float)CssDimensionParsingUtils.ParseFloat(values[0]));
            if (values.Count == 3) {
                float centerX = CssDimensionParsingUtils.ParseAbsoluteLength(values[1]);
                float centerY = CssDimensionParsingUtils.ParseAbsoluteLength(values[2]);
                return AffineTransform.GetRotateInstance(angle, centerX, centerY);
            }
            return AffineTransform.GetRotateInstance(angle);
        }

        /// <summary>Creates a scale transformation.</summary>
        /// <param name="values">values of the transformation</param>
        /// <returns>AffineTransform for the scale operation</returns>
        private static AffineTransform CreateScaleTransformation(IList<String> values) {
            if (values.Count == 0 || values.Count > 2) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES);
            }
            float scaleX = CssDimensionParsingUtils.ParseRelativeValue(values[0], 1);
            float scaleY = values.Count == 2 ? CssDimensionParsingUtils.ParseRelativeValue(values[1], 1) : scaleX;
            return AffineTransform.GetScaleInstance(scaleX, scaleY);
        }

        /// <summary>Creates a translate transformation.</summary>
        /// <param name="values">values of the transformation</param>
        /// <returns>AffineTransform for the translate operation</returns>
        private static AffineTransform CreateTranslateTransformation(IList<String> values) {
            if (values.Count == 0 || values.Count > 2) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES);
            }
            float translateX = CssDimensionParsingUtils.ParseAbsoluteLength(values[0]);
            float translateY = values.Count == 2 ? CssDimensionParsingUtils.ParseAbsoluteLength(values[1]) : 0;
            return AffineTransform.GetTranslateInstance(translateX, translateY);
        }

        /// <summary>Creates a matrix transformation.</summary>
        /// <param name="values">values of the transformation</param>
        /// <returns>AffineTransform for the matrix keyword</returns>
        private static AffineTransform CreateMatrixTransformation(IList<String> values) {
            if (values.Count != 6) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES);
            }
            float a = (float)float.Parse(values[0], System.Globalization.CultureInfo.InvariantCulture);
            float b = (float)float.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture);
            float c = (float)float.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
            float d = (float)float.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
            float e = CssDimensionParsingUtils.ParseAbsoluteLength(values[4]);
            float f = CssDimensionParsingUtils.ParseAbsoluteLength(values[5]);
            return new AffineTransform(a, b, c, d, e, f);
        }

        /// <summary>This method extracts the transformation name given a transformation.</summary>
        /// <param name="transformation">the transformation</param>
        /// <returns>the name of the transformation</returns>
        private static String GetNameFromString(String transformation) {
            int indexOfParenthesis = transformation.IndexOf("(", StringComparison.Ordinal);
            if (indexOfParenthesis == -1) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.INVALID_TRANSFORM_DECLARATION);
            }
            return transformation.JSubstring(0, transformation.IndexOf("(", StringComparison.Ordinal));
        }

        /// <summary>This method extracts the values from a transformation.</summary>
        /// <param name="transformation">the transformation</param>
        /// <returns>values of the transformation</returns>
        private static IList<String> GetValuesFromTransformationString(String transformation) {
            String numbers = transformation.JSubstring(transformation.IndexOf('(') + 1, transformation.IndexOf(')'));
            return SvgCssUtils.SplitValueList(numbers);
        }
    }
}
