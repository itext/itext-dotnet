namespace iText.IO.Font.Constants {
    public sealed class FontStyles {
        private FontStyles() {
        }

        /// <summary>Undefined font style.</summary>
        public const int UNDEFINED = -1;

        /// <summary>Normal font style.</summary>
        public const int NORMAL = 0;

        /// <summary>Bold font style.</summary>
        public const int BOLD = 1;

        /// <summary>Italic font style.</summary>
        public const int ITALIC = 2;

        /// <summary>Bold-Italic font style.</summary>
        public const int BOLDITALIC = BOLD | ITALIC;
    }
}
