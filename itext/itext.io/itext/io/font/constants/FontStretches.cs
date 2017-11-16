using System;

namespace iText.IO.Font.Constants {
    public sealed class FontStretches {
        private FontStretches() {
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

        public const String ULTRA_CONDENSED = "UltraCondensed";

        public const String EXTRA_CONDENSED = "ExtraCondensed";

        public const String CONDENSED = "Condensed";

        public const String SEMI_CONDENSED = "SemiCondensed";

        public const String NORMAL = "Normal";

        public const String SEMI_EXPANDED = "SemiExpanded";

        public const String EXPANDED = "Expanded";

        public const String EXTRA_EXPANDED = "ExtraExpanded";

        public const String ULTRA_EXPANDED = "UltraExpanded";

        /// <summary>Convert from Open Type font width class notation.</summary>
        /// <remarks>
        /// Convert from Open Type font width class notation.
        /// <br/>
        /// https://www.microsoft.com/typography/otspec/os2.htm#wdc
        /// </remarks>
        /// <param name="fontWidth">Open Type font width.</param>
        /// <returns>
        /// one of the
        /// <see cref="FontStretches"/>
        /// constants.
        /// </returns>
        public static String FromOpenTypeWidthClass(int fontWidth) {
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
