using System;

namespace iText.IO.Font.Constants {
    public sealed class FontWeights {
        private FontWeights() {
        }

        public const int THIN = 100;

        public const int EXTRA_LIGHT = 200;

        public const int LIGHT = 300;

        public const int NORMAL = 400;

        public const int MEDIUM = 500;

        public const int SEMI_BOLD = 600;

        public const int BOLD = 700;

        public const int EXTRA_BOLD = 800;

        public const int BLACK = 900;

        // Font weight Thin
        // Font weight Extra-light (Ultra-light)
        // Font weight Light
        // Font weight Normal
        // Font weight Medium
        // Font weight Semi-bold
        // Font weight Bold
        // Font weight Extra-bold (Ultra-bold)
        // Font weight Black (Heavy)
        public static int FromType1FontWeight(String weight) {
            int fontWeight = NORMAL;
            switch (weight.ToLowerInvariant()) {
                case "ultralight": {
                    fontWeight = THIN;
                    break;
                }

                case "thin":
                case "extralight": {
                    fontWeight = EXTRA_LIGHT;
                    break;
                }

                case "light": {
                    fontWeight = LIGHT;
                    break;
                }

                case "book":
                case "regular":
                case "normal": {
                    fontWeight = NORMAL;
                    break;
                }

                case "medium": {
                    fontWeight = MEDIUM;
                    break;
                }

                case "demibold":
                case "semibold": {
                    fontWeight = SEMI_BOLD;
                    break;
                }

                case "bold": {
                    fontWeight = BOLD;
                    break;
                }

                case "extrabold":
                case "ultrabold": {
                    fontWeight = EXTRA_BOLD;
                    break;
                }

                case "heavy":
                case "black":
                case "ultra":
                case "ultrablack": {
                    fontWeight = BLACK;
                    break;
                }

                case "fat":
                case "extrablack": {
                    fontWeight = 1000;
                    break;
                }
            }
            return fontWeight;
        }
    }
}
