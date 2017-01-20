using System;
using iText.IO.Font;

namespace iText.Layout.Font {
    internal sealed class FontCharacteristicUtils {
        internal static FontWeight CalculateFontWeight(short fw) {
            switch (fw) {
                case 100: {
                    return FontWeight.THIN;
                }

                case 200: {
                    return FontWeight.EXTRA_LIGHT;
                }

                case 300: {
                    return FontWeight.LIGHT;
                }

                case 400: {
                    return FontWeight.NORMAL;
                }

                case 500: {
                    return FontWeight.MEDIUM;
                }

                case 600: {
                    return FontWeight.SEMI_BOLD;
                }

                case 700: {
                    return FontWeight.BOLD;
                }

                case 800: {
                    return FontWeight.EXTRA_BOLD;
                }

                case 900: {
                    return FontWeight.BLACK;
                }

                default: {
                    return FontWeight.NORMAL;
                }
            }
        }

        internal static short CalculateFontWeightNumber(FontWeight fw) {
            switch (fw) {
                case FontWeight.THIN: {
                    return 100;
                }

                case FontWeight.EXTRA_LIGHT: {
                    return 200;
                }

                case FontWeight.LIGHT: {
                    return 300;
                }

                case FontWeight.NORMAL: {
                    return 400;
                }

                case FontWeight.MEDIUM: {
                    return 500;
                }

                case FontWeight.SEMI_BOLD: {
                    return 600;
                }

                case FontWeight.BOLD: {
                    return 700;
                }

                case FontWeight.EXTRA_BOLD: {
                    return 800;
                }

                case FontWeight.BLACK: {
                    return 900;
                }

                default: {
                    return 400;
                }
            }
        }

        internal static short NormalizeFontWeight(short fw) {
            fw = (short)((fw / 100) * 100);
            if (fw < 100) {
                return 100;
            }
            if (fw > 900) {
                return 900;
            }
            return fw;
        }

        internal static short ParseFontWeight(String fw) {
            if (fw == null || fw.Length == 0) {
                return -1;
            }
            fw = fw.Trim().ToLowerInvariant();
            switch (fw) {
                case "bold": {
                    return 700;
                }

                case "normal": {
                    return 400;
                }

                default: {
                    try {
                        return NormalizeFontWeight((short)System.Convert.ToInt32(fw));
                    }
                    catch (FormatException) {
                        return -1;
                    }
                    break;
                }
            }
        }
    }
}
