/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.Kernel.Colors {
    /// <summary>
    /// Class containing predefined
    /// <see cref="DeviceRgb"/>
    /// colors.
    /// </summary>
    /// <remarks>
    /// Class containing predefined
    /// <see cref="DeviceRgb"/>
    /// colors.
    /// Color space specific classes should be used for the advanced handling of colors.
    /// The most common ones are
    /// <see cref="DeviceGray"/>
    /// ,
    /// <see cref="DeviceCmyk"/>
    /// and
    /// <see cref="DeviceRgb"/>.
    /// </remarks>
    public class ColorConstants {
        /// <summary>Predefined black DeviceRgb color</summary>
        public static readonly Color BLACK = DeviceRgb.BLACK;

        /// <summary>Predefined blue  DeviceRgb color</summary>
        public static readonly Color BLUE = DeviceRgb.BLUE;

        /// <summary>Predefined cyan DeviceRgb color</summary>
        public static readonly Color CYAN = new DeviceRgb(0, 255, 255);

        /// <summary>Predefined dark gray DeviceRgb color</summary>
        public static readonly Color DARK_GRAY = new DeviceRgb(64, 64, 64);

        /// <summary>Predefined gray DeviceRgb color</summary>
        public static readonly Color GRAY = new DeviceRgb(128, 128, 128);

        /// <summary>Predefined green DeviceRgb color</summary>
        public static readonly Color GREEN = DeviceRgb.GREEN;

        /// <summary>Predefined light gray DeviceRgb color</summary>
        public static readonly Color LIGHT_GRAY = new DeviceRgb(192, 192, 192);

        /// <summary>Predefined magenta DeviceRgb color</summary>
        public static readonly Color MAGENTA = new DeviceRgb(255, 0, 255);

        /// <summary>Predefined orange DeviceRgb color</summary>
        public static readonly Color ORANGE = new DeviceRgb(255, 200, 0);

        /// <summary>Predefined pink DeviceRgb color</summary>
        public static readonly Color PINK = new DeviceRgb(255, 175, 175);

        /// <summary>Predefined red DeviceRgb color</summary>
        public static readonly Color RED = DeviceRgb.RED;

        /// <summary>Predefined white DeviceRgb color</summary>
        public static readonly Color WHITE = DeviceRgb.WHITE;

        /// <summary>Predefined yellow DeviceRgb color</summary>
        public static readonly Color YELLOW = new DeviceRgb(255, 255, 0);
    }
}
