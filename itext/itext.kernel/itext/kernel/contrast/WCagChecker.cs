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
namespace iText.Kernel.Contrast {
    /// <summary>Utility class for checking WCAG (Web Content Accessibility Guidelines) compliance for text contrast.
    ///     </summary>
    /// <seealso><a href="https://www.w3.org/wai/wcag21/understanding/contrast-minimum.html">WCAG 2.1 Contrast (Minimum)</a>
    ///     </seealso>
    /// <seealso><a href="https://www.w3.org/wai/wcag21/understanding/contrast-enhanced.html">WCAG 2.1 Contrast (Enhanced)</a>
    ///     </seealso>
    public sealed class WCagChecker {
        // 14pt in pixels (1pt = 1.333px)
        private const double LARGE_TEXT_MIN_FONT_SIZE_PX = 18.66;

        /// <summary>Private constructor to prevent instantiation of this utility class.</summary>
        private WCagChecker() {
        }

        // Utility class
        /// <summary>Checks for WCAG AA compliance.</summary>
        /// <param name="fontSize">The font size in pixels</param>
        /// <param name="contrastRatio">The contrast ratio between text and background</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the text meets WCAG AA compliance,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsTextWcagAACompliant(double fontSize, double contrastRatio) {
            bool isLargeText = fontSize >= LARGE_TEXT_MIN_FONT_SIZE_PX;
            if (isLargeText) {
                return contrastRatio >= 3.0;
            }
            else {
                return contrastRatio >= 4.5;
            }
        }

        /// <summary>Checks for WCAG AAA compliance.</summary>
        /// <param name="fontSize">The font size in pixels</param>
        /// <param name="contrastRatio">The contrast ratio between text and background</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the text meets WCAG AAA compliance,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsTextWcagAAACompliant(double fontSize, double contrastRatio) {
            bool isLargeText = fontSize >= LARGE_TEXT_MIN_FONT_SIZE_PX;
            if (isLargeText) {
                return contrastRatio >= 4.5;
            }
            else {
                return contrastRatio >= 7.0;
            }
        }
    }
}
