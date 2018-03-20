using System;

namespace iText.Svg.Exceptions {
    public sealed class SvgLogMessageConstant {
        private SvgLogMessageConstant() {
        }

        public const String COULDNOTINSTANTIATE = "Could not instantiate Renderer for tag {0}";

        public const String FLOAT_PARSING_NAN = "The passed value is not a number.";

        public const String INODEROOTISNULL = "Input root value is null";

        public const String INVALID_TRANSFORM_DECLARATION = "Transformation declaration is not formed correctly.";

        public const String LOOP = "Loop detected";

        public const String NOROOT = "No root found";

        public const String TAGPARAMETERNULL = "Tag parameter must not be null";

        public const String TRANSFORM_EMPTY = "The transformation value is empty.";

        public const String TRANSFORM_INCORRECT_NUMBER_OF_VALUES = "Transformation doesn't contain the right number of values.";

        public const String TRANSFORM_INCORRECT_VALUE_TYPE = "The transformation value is not a number.";

        public const String TRANSFORM_NULL = "The transformation value is null.";

        public const String UNMAPPEDTAG = "Could not find implementation for tag {0}";

        public const String UNKNOWN_TRANSFORMATION_TYPE = "Unsupported type of transformation.";
    }
}
