using System;

namespace iText.Svg.Exceptions {
    public sealed class SvgLogMessageConstant {
        private SvgLogMessageConstant() {
        }

        public const String LOOP = "Loop detected";

        public const String NOROOT = "No root found";

        public const String INODEROOTISNULL = "Input root value is null";

        public const String TAGPARAMETERNULL = "Tag parameter must not be null";

        public const String UNMAPPEDTAG = "Could not find implementation for tag {0}";

        public const String COULDNOTINSTANTIATE = "Could not instantiate Renderer for tag {0}";
    }
}
