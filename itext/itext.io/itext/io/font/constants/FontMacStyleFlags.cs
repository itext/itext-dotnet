namespace iText.IO.Font.Constants {
    /// <summary>Represents Open Type head.macStyle bits.</summary>
    /// <remarks>
    /// Represents Open Type head.macStyle bits.
    /// <br/>
    /// https://www.microsoft.com/typography/otspec/head.htm
    /// </remarks>
    public sealed class FontMacStyleFlags {
        private FontMacStyleFlags() {
        }

        public const int BOLD = 1;

        public const int ITALIC = 2;

        public const int UNDERLINE = 4;

        public const int OUTLINE = 8;

        public const int SHADOW = 16;

        public const int CONDENSED = 32;

        public const int EXTENDED = 64;
        // Bit 0: Bold (if set to 1);
        // Bit 1: Italic (if set to 1)
        // Bit 2: Underline (if set to 1)
        // Bit 3: Outline (if set to 1)
        // Bit 4: Shadow (if set to 1)
        // Bit 5: Condensed (if set to 1)
        // Bit 6: Extended (if set to 1)
    }
}
