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

namespace iText.Svg.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class SvgExceptionMessageConstant {
        public const String ARC_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0 = "(rx ry rot largearc sweep x y)+ parameters are expected for elliptical arcs. Got: {0}";

        public const String COORDINATE_ARRAY_LENGTH_MUST_BY_DIVISIBLE_BY_CURRENT_COORDINATES_ARRAY_LENGTH = "Array of current coordinates must have length that is divisible by the length of the array with current "
             + "coordinates";

        public const String COULD_NOT_DETERMINE_MIDDLE_POINT_OF_ELLIPTICAL_ARC = "Could not determine the middle point of the ellipse traced by this elliptical arc";

        public const String CURVE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0 = "(x1 y1 x2 y2 x y)+ parameters are expected for curves. Got: {0}";

        public const String DRAW_NO_DRAW = "The renderer cannot be drawn.";

        public const String FAILED_TO_PARSE_INPUTSTREAM = "Failed to parse InputStream.";

        public const String FONT_NOT_FOUND = "The font wasn't found.";

        public const String I_NODE_ROOT_IS_NULL = "Input root value is null";

        public const String MEET_OR_SLICE_ARGUMENT_IS_INCORRECT = "The meetOrSlice argument is incorrect. It must be `meet`, `slice` or null.";

        public const String CURRENT_VIEWPORT_IS_NULL = "The current viewport is null. The viewBox applying could not be processed.";

        public const String VIEWBOX_IS_INCORRECT = "The viewBox is incorrect. The viewBox applying could not be processed.";

        public const String INVALID_CLOSEPATH_OPERATOR_USE = "The close path operator (Z) may not be used before a move to operation (M)";

        public const String INVALID_PATH_D_ATTRIBUTE_OPERATORS = "Invalid operators found in path data attribute: {0}";

        public const String INVALID_SMOOTH_CURVE_USE = "The smooth curve operations (S, s, T, t) may not be used as a first operator in path.";

        public const String INVALID_TRANSFORM_DECLARATION = "Transformation declaration is not formed correctly.";

        public const String LINE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0 = "(x y)+ parameters are expected for lineTo operator. Got: {0}";

        public const String MOVE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0 = "(x y)+ parameters are expected for moveTo operator. Got: {0}";

        public const String NAMED_OBJECT_NAME_NULL_OR_EMPTY = "The name of the named object can't be null or empty.";

        public const String NAMED_OBJECT_NULL = "A named object can't be null.";

        public const String NO_ROOT = "No root found";

        public const String PARAMETER_CANNOT_BE_NULL = "Parameters cannot be null.";

        public const String POINTS_ATTRIBUTE_INVALID_LIST = "Points attribute {0} on polyline tag does not contain a valid set of points";

        public const String QUADRATIC_CURVE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0 = "(x1 y1 x y)+ parameters are expected for quadratic curves. Got: {0}";

        public const String ROOT_SVG_NO_BBOX = "The root svg tag needs to have a bounding box defined.";

        public const String TAG_PARAMETER_NULL = "Tag parameter must not be null";

        public const String TRANSFORM_EMPTY = "The transformation value is empty.";

        public const String TRANSFORM_INCORRECT_NUMBER_OF_VALUES = "Transformation doesn't contain the right number of values.";

        public const String TRANSFORM_NULL = "The transformation value is null.";

        public const String UNKNOWN_TRANSFORMATION_TYPE = "Unsupported type of transformation.";

        public const String ILLEGAL_RELATIVE_VALUE_NO_VIEWPORT_IS_SET = "Relative value can't be resolved, no viewport is set.";

        private SvgExceptionMessageConstant() {
        }
    }
}
