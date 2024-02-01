/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
        /// <para />
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
