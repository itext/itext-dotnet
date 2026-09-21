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
using iText.Kernel.Colors;

namespace iText.Kernel.Utils {
    /// <summary>Utility methods for color calculations.</summary>
    public sealed class ColorUtils {
        // Constants defined in svg specification: https://www.w3.org/TR/SVG11/filters.html#feColorMatrixValuesAttribute.
        // They are used because pdf /Luminosity uses different coefficients: 0.3R × 0.59G × 0.11B
        private const float RED_LUMINANCE_COEFFICIENT = 0.2125F;

        private const float GREEN_LUMINANCE_COEFFICIENT = 0.7154F;

        private const float BLUE_LUMINANCE_COEFFICIENT = 0.0721F;

        private ColorUtils() {
        }

        /// <summary>
        /// Converts a
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// color to
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// using the SVG mask luminance coefficients.
        /// </summary>
        /// <remarks>
        /// Converts a
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// color to
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// using the SVG mask luminance coefficients.
        /// Colors of other types and
        /// <see langword="null"/>
        /// are returned unchanged.
        /// </remarks>
        /// <param name="color">the color to convert</param>
        /// <returns>the grayscale color for an RGB input, otherwise the original color</returns>
        public static Color ToDeviceGrayForSvgLuminanceMode(Color color) {
            if (!(color is DeviceRgb)) {
                return color;
            }
            return new DeviceGray(CalculateSvgLuminance(color.GetColorValue()));
        }

        /// <summary>Calculates luminance using the SVG mask coefficients without gamma correction.</summary>
        /// <param name="rgb">the red, green and blue components, each in the range [0, 1]</param>
        /// <returns>the luminance of the RGB color</returns>
        public static float CalculateSvgLuminance(float[] rgb) {
            return RED_LUMINANCE_COEFFICIENT * rgb[0] + GREEN_LUMINANCE_COEFFICIENT * rgb[1] + BLUE_LUMINANCE_COEFFICIENT
                 * rgb[2];
        }
    }
}
