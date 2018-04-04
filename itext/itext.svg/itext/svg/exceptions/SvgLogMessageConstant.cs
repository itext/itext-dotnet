using System;

namespace iText.Svg.Exceptions {
    /// <summary>Class that holds the logging and exception messages.</summary>
    public sealed class SvgLogMessageConstant {
        private SvgLogMessageConstant() {
        }

        public const String COULDNOTINSTANTIATE = "Could not instantiate Renderer for tag {0}";

        public const String FLOAT_PARSING_NAN = "The passed value is not a number.";

        public const String FONT_NOT_FOUND = "The font wasn't found.";

        public const String INODEROOTISNULL = "Input root value is null";

        public const String INVALID_TRANSFORM_DECLARATION = "Transformation declaration is not formed correctly.";

        public const String LOOP = "Loop detected";

        public const String NAMED_OBJECT_NAME_NULL_OR_EMPTY = "The name of the named object can't be null or empty.";

        public const String NAMED_OBJECT_NULL = "A named object can't be null.";

        public const String NOROOT = "No root found";

        public const String ROOT_SVG_NO_BBOX = "The root svg tag needs to have a bounding box defined.";

        public const String POINTS_ATTRIBUTE_INVALID_LIST = "Points attribute {0} on polyline tag does not contain a valid set of points";

        public const String TAGPARAMETERNULL = "Tag parameter must not be null";

        public const String TRANSFORM_EMPTY = "The transformation value is empty.";

        public const String TRANSFORM_INCORRECT_NUMBER_OF_VALUES = "Transformation doesn't contain the right number of values.";

        public const String TRANSFORM_INCORRECT_VALUE_TYPE = "The transformation value is not a number.";

        public const String TRANSFORM_NULL = "The transformation value is null.";

        public const String UNMAPPEDTAG = "Could not find implementation for tag {0}";

        public const String UNKNOWN_TRANSFORMATION_TYPE = "Unsupported type of transformation.";

        public const String PARAMETER_CANNOT_BE_NULL = "Parameters for this method cannot be null.";
    }
}
