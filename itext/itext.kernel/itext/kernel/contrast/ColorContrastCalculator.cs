/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Kernel.Colors;

namespace iText.Kernel.Contrast {
    /// <summary>Utility class for calculating color contrast ratios according to the Web Content Accessibility Guidelines (WCAG) 2.1.
    ///     </summary>
    /// <remarks>
    /// Utility class for calculating color contrast ratios according to the Web Content Accessibility Guidelines (WCAG) 2.1.
    /// <para />
    /// The contrast ratio ranges from 1:1 (no contrast) to 21:1 (maximum contrast between black and white).
    /// </remarks>
    public sealed class ColorContrastCalculator {
        private const double LUMINANCE_OFFSET = 0.05;

        private const double SRGB_LINEARIZATION_THRESHOLD = 0.04045;

        private const double SRGB_LINEARIZATION_DIVISOR = 12.92;

        private const double SRGB_LINEARIZATION_COEFFICIENT = 1.055;

        private const double SRGB_LINEARIZATION_OFFSET = 0.055;

        private const double SRGB_LINEARIZATION_EXPONENT = 2.4;

        // ITU-R BT.709 coefficients for relative luminance calculation
        private const double RED_LUMINANCE_COEFFICIENT = 0.2126;

        private const double GREEN_LUMINANCE_COEFFICIENT = 0.7152;

        private const double BLUE_LUMINANCE_COEFFICIENT = 0.0722;

        private const int RED_COMPONENT_INDEX = 0;

        private const int GREEN_COMPONENT_INDEX = 1;

        private const int BLUE_COMPONENT_INDEX = 2;

        /// <summary>Private constructor to prevent instantiation of this utility class.</summary>
        private ColorContrastCalculator() {
        }

        // Utility class
        /// <summary>Calculates the contrast ratio between two colors according to WCAG 2.1 guidelines.</summary>
        /// <remarks>
        /// Calculates the contrast ratio between two colors according to WCAG 2.1 guidelines.
        /// <para />
        /// The contrast ratio is calculated as (L1 + 0.05) / (L2 + 0.05), where L1 is the relative
        /// luminance of the lighter color and L2 is the relative luminance of the darker color.
        /// <para />
        /// The resulting value ranges from 1:1 (identical colors) to 21:1 (black and white).
        /// </remarks>
        /// <param name="color1">
        /// the first color to compare, must not be
        /// <see langword="null"/>
        /// </param>
        /// <param name="color2">
        /// the second color to compare, must not be
        /// <see langword="null"/>
        /// </param>
        /// <returns>the contrast ratio between the two colors, ranging from 1.0 to 21.0</returns>
        public static double ContrastRatio(DeviceRgb color1, DeviceRgb color2) {
            if (color1 == null || color2 == null) {
                throw new ArgumentException("Colors must not be null");
            }
            double[] components1 = ExtractRgbComponents(color1);
            double[] components2 = ExtractRgbComponents(color2);
            return ContrastRatio(components1[RED_COMPONENT_INDEX], components1[GREEN_COMPONENT_INDEX], components1[BLUE_COMPONENT_INDEX
                ], components2[RED_COMPONENT_INDEX], components2[GREEN_COMPONENT_INDEX], components2[BLUE_COMPONENT_INDEX
                ]);
        }

        /// <summary>Calculates the contrast ratio between two RGB colors according to WCAG 2.1 guidelines.</summary>
        /// <remarks>
        /// Calculates the contrast ratio between two RGB colors according to WCAG 2.1 guidelines.
        /// <para />
        /// The contrast ratio is calculated as (L1 + 0.05) / (L2 + 0.05), where L1 is the relative
        /// luminance of the lighter color and L2 is the relative luminance of the darker color.
        /// </remarks>
        /// <param name="r1">red component of the first color (0-1)</param>
        /// <param name="g1">green component of the first color (0-1)</param>
        /// <param name="b1">blue component of the first color (0-1)</param>
        /// <param name="r2">red component of the second color (0-1)</param>
        /// <param name="g2">green component of the second color (0-1)</param>
        /// <param name="b2">blue component of the second color (0-1)</param>
        /// <returns>the contrast ratio between the two colors, ranging from 1.0 to 21.0</returns>
        public static double ContrastRatio(double r1, double g1, double b1, double r2, double g2, double b2) {
            double l1 = Luminance(r1, g1, b1);
            double l2 = Luminance(r2, g2, b2);
            double lighter = Math.Max(l1, l2);
            double darker = Math.Min(l1, l2);
            return (lighter + LUMINANCE_OFFSET) / (darker + LUMINANCE_OFFSET);
        }

        /// <summary>Extracts the RGB components from a DeviceRgb color.</summary>
        /// <param name="color">the color to extract components from</param>
        /// <returns>an array containing the red, green, and blue components (0-1)</returns>
        private static double[] ExtractRgbComponents(DeviceRgb color) {
            float[] colorValues = color.GetColorValue();
            return new double[] { ClampToRgbRange(colorValues[RED_COMPONENT_INDEX]), ClampToRgbRange(colorValues[GREEN_COMPONENT_INDEX
                ]), ClampToRgbRange(colorValues[BLUE_COMPONENT_INDEX]) };
        }

        /// <summary>Clamps an integer value to the valid RGB range of 0-255.</summary>
        /// <param name="value">the value to clamp</param>
        /// <returns>the clamped value, guaranteed to be between 0 and 1 inclusive</returns>
        private static double ClampToRgbRange(float value) {
            if (value < 0) {
                return 0;
            }
            if (value > 1) {
                return 1;
            }
            return value;
        }

        /// <summary>Converts an sRGB color channel value to linear RGB.</summary>
        /// <remarks>
        /// Converts an sRGB color channel value to linear RGB.
        /// <para />
        /// This implements the inverse of the sRGB gamma correction according to the sRGB specification.
        /// Values below the threshold use a linear transformation, while values above use a power function.
        /// </remarks>
        /// <param name="channel">the color channel value in the range 0-1</param>
        /// <returns>the linearized channel value in the range 0.0-1.0</returns>
        private static double Linearize(double channel) {
            return (channel <= SRGB_LINEARIZATION_THRESHOLD) ? (channel / SRGB_LINEARIZATION_DIVISOR) : Math.Pow((channel
                 + SRGB_LINEARIZATION_OFFSET) / SRGB_LINEARIZATION_COEFFICIENT, SRGB_LINEARIZATION_EXPONENT);
        }

        /// <summary>Calculates the relative luminance of an RGB color according to WCAG 2.1.</summary>
        /// <remarks>
        /// Calculates the relative luminance of an RGB color according to WCAG 2.1.
        /// <para />
        /// The relative luminance is calculated using the ITU-R BT.709 coefficients:
        /// L = 0.2126 * R + 0.7152 * G + 0.0722 * B, where R, G, and B are the linearized color components.
        /// </remarks>
        /// <param name="r">red component (0-1)</param>
        /// <param name="g">green component (0-1)</param>
        /// <param name="b">blue component (0-1)</param>
        /// <returns>the relative luminance in the range 0.0 (black) to 1.0 (white)</returns>
        private static double Luminance(double r, double g, double b) {
            double rLin = Linearize(r);
            double gLin = Linearize(g);
            double bLin = Linearize(b);
            return RED_LUMINANCE_COEFFICIENT * rLin + GREEN_LUMINANCE_COEFFICIENT * gLin + BLUE_LUMINANCE_COEFFICIENT 
                * bLin;
        }
    }
}
