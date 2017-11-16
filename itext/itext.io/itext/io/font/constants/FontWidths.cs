using System;

namespace iText.IO.Font.Constants {
    public sealed class FontWidths {
        private FontWidths() {
        }

        private const int FWIDTH_ULTRA_CONDENSED = 1;

        private const int FWIDTH_EXTRA_CONDENSED = 2;

        private const int FWIDTH_CONDENSED = 3;

        private const int FWIDTH_SEMI_CONDENSED = 4;

        private const int FWIDTH_NORMAL = 5;

        private const int FWIDTH_SEMI_EXPANDED = 6;

        private const int FWIDTH_EXPANDED = 7;

        private const int FWIDTH_EXTRA_EXPANDED = 8;

        private const int FWIDTH_ULTRA_EXPANDED = 9;

        public const String ULTRA_CONDENSED = "ultra-condensed";

        public const String EXTRA_CONDENSED = "extra-condensed";

        public const String CONDENSED = "condensed";

        public const String SEMI_CONDENSED = "semi-condensed";

        public const String NORMAL = "normal";

        public const String SEMI_EXPANDED = "semi-expanded";

        public const String EXPANDED = "expanded";

        public const String EXTRA_EXPANDED = "extra-expanded";

        public const String ULTRA_EXPANDED = "ultra-expanded";

        /// <summary>Convert from Type 1 font width notation</summary>
        /// <param name="fontWidth">Type 1 font width.</param>
        /// <returns>
        /// one of the
        /// <see cref="FontWidths"/>
        /// constants.
        /// </returns>
        public static String FromType1FontWidth(String fontWidth) {
            fontWidth = fontWidth.ToLowerInvariant();
            String fontWidthValue = NORMAL;
            switch (fontWidth) {
                case "ultracondensed": {
                    fontWidthValue = ULTRA_CONDENSED;
                    break;
                }

                case "extracondensed": {
                    fontWidthValue = EXTRA_CONDENSED;
                    break;
                }

                case "condensed": {
                    fontWidthValue = CONDENSED;
                    break;
                }

                case "semicondensed": {
                    fontWidthValue = SEMI_CONDENSED;
                    break;
                }

                case "normal": {
                    fontWidthValue = NORMAL;
                    break;
                }

                case "semiexpanded": {
                    fontWidthValue = SEMI_CONDENSED;
                    break;
                }

                case "expanded": {
                    fontWidthValue = EXPANDED;
                    break;
                }

                case "extraexpanded": {
                    fontWidthValue = EXTRA_CONDENSED;
                    break;
                }

                case "ultraexpanded": {
                    fontWidthValue = ULTRA_CONDENSED;
                    break;
                }
            }
            return fontWidthValue;
        }

        /// <summary>Convert from Open Type font width notation.</summary>
        /// <remarks>
        /// Convert from Open Type font width notation.
        /// <br/>
        /// https://www.microsoft.com/typography/otspec/os2.htm#wdc
        /// </remarks>
        /// <param name="fontWidth">Open Type font width.</param>
        /// <returns>
        /// one of the
        /// <see cref="FontWidths"/>
        /// constants.
        /// </returns>
        public static String FromOpenTypeFontWidth(int fontWidth) {
            String fontWidthValue = NORMAL;
            switch (fontWidth) {
                case FWIDTH_ULTRA_CONDENSED: {
                    fontWidthValue = ULTRA_CONDENSED;
                    break;
                }

                case FWIDTH_EXTRA_CONDENSED: {
                    fontWidthValue = EXTRA_CONDENSED;
                    break;
                }

                case FWIDTH_CONDENSED: {
                    fontWidthValue = CONDENSED;
                    break;
                }

                case FWIDTH_SEMI_CONDENSED: {
                    fontWidthValue = SEMI_CONDENSED;
                    break;
                }

                case FWIDTH_NORMAL: {
                    fontWidthValue = NORMAL;
                    break;
                }

                case FWIDTH_SEMI_EXPANDED: {
                    fontWidthValue = SEMI_EXPANDED;
                    break;
                }

                case FWIDTH_EXPANDED: {
                    fontWidthValue = EXPANDED;
                    break;
                }

                case FWIDTH_EXTRA_EXPANDED: {
                    fontWidthValue = EXTRA_EXPANDED;
                    break;
                }

                case FWIDTH_ULTRA_EXPANDED: {
                    fontWidthValue = ULTRA_EXPANDED;
                    break;
                }
            }
            return fontWidthValue;
        }
    }
}
