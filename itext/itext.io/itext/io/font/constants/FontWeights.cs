/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.IO.Font.Constants {
    /// <summary>Font weight values and utility methods</summary>
    public sealed class FontWeights {
        private FontWeights() {
        }

        // Font weight Thin
        public const int THIN = 100;

        // Font weight Extra-light (Ultra-light)
        public const int EXTRA_LIGHT = 200;

        // Font weight Light
        public const int LIGHT = 300;

        // Font weight Normal
        public const int NORMAL = 400;

        // Font weight Medium
        public const int MEDIUM = 500;

        // Font weight Semi-bold
        public const int SEMI_BOLD = 600;

        // Font weight Bold
        public const int BOLD = 700;

        // Font weight Extra-bold (Ultra-bold)
        public const int EXTRA_BOLD = 800;

        // Font weight Black (Heavy)
        public const int BLACK = 900;

        /// <summary>Parses font weight constant to corresponding value.</summary>
        /// <param name="weight">weight constant</param>
        /// <returns>corresponding weight int value</returns>
        public static int FromType1FontWeight(String weight) {
            int fontWeight = NORMAL;
            switch (StringNormalizer.ToLowerCase(weight)) {
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
                case "ultrablack":
                case "fat":
                case "extrablack": {
                    fontWeight = BLACK;
                    break;
                }
            }
            return fontWeight;
        }

        /// <summary>
        /// Normalize font weight to either
        /// <see cref="THIN"/>
        /// or
        /// <see cref="BLACK"/>.
        /// </summary>
        /// <param name="fontWeight">font weight int value</param>
        /// <returns>
        /// either
        /// <see cref="THIN"/>
        /// or
        /// <see cref="BLACK"/>
        /// based on a given weight value
        /// </returns>
        public static int NormalizeFontWeight(int fontWeight) {
            fontWeight = (fontWeight / 100) * 100;
            if (fontWeight < iText.IO.Font.Constants.FontWeights.THIN) {
                return iText.IO.Font.Constants.FontWeights.THIN;
            }
            if (fontWeight > iText.IO.Font.Constants.FontWeights.BLACK) {
                return iText.IO.Font.Constants.FontWeights.BLACK;
            }
            return fontWeight;
        }
    }
}
