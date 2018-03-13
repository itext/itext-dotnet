using System;

namespace iText.Svg.Exceptions {
    public sealed class SvgLogMessageConstant {
        private SvgLogMessageConstant() {
        }

        public const String LOOP = "Loop detected";

        public const String NOROOT = "No root found";

        public const String INODEROOTISNULL = "Input root value is null";
    }
}
