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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    /// <summary>Color space to specify colors according to CMYK color model.</summary>
    public class DeviceCmyk : Color {
        /// <summary>Predefined cyan DeviceCmyk color</summary>
        public static readonly iText.Kernel.Colors.DeviceCmyk CYAN = new iText.Kernel.Colors.DeviceCmyk(100, 0, 0, 
            0);

        /// <summary>Predefined magenta DeviceCmyk color</summary>
        public static readonly iText.Kernel.Colors.DeviceCmyk MAGENTA = new iText.Kernel.Colors.DeviceCmyk(0, 100, 
            0, 0);

        /// <summary>Predefined yellow DeviceCmyk color</summary>
        public static readonly iText.Kernel.Colors.DeviceCmyk YELLOW = new iText.Kernel.Colors.DeviceCmyk(0, 0, 100
            , 0);

        /// <summary>Predefined black DeviceCmyk color</summary>
        public static readonly iText.Kernel.Colors.DeviceCmyk BLACK = new iText.Kernel.Colors.DeviceCmyk(0, 0, 0, 
            100);

        /// <summary>Creates DeviceCmyk color with all colorants intensities initialised as zeroes.</summary>
        public DeviceCmyk()
            : this(0f, 0f, 0f, 1f) {
        }

        /// <summary>Creates DeviceCmyk color by intensities of cyan, magenta, yellow and black colorants.</summary>
        /// <remarks>
        /// Creates DeviceCmyk color by intensities of cyan, magenta, yellow and black colorants.
        /// The intensities are considered to be in [0, 100] gap, if not,
        /// the intensity will be considered as 100 (when colorant's value is bigger than 100)
        /// or 0 (when colorant's value is less than 0).
        /// </remarks>
        /// <param name="c">the intensity of cyan colorant</param>
        /// <param name="m">the intensity of magenta colorant</param>
        /// <param name="y">the intensity of yellow colorant</param>
        /// <param name="k">the intensity of black colorant</param>
        public DeviceCmyk(int c, int m, int y, int k)
            : this(c / 100f, m / 100f, y / 100f, k / 100f) {
        }

        /// <summary>Creates DeviceCmyk color by intensities of cyan, magenta, yellow and black colorants.</summary>
        /// <remarks>
        /// Creates DeviceCmyk color by intensities of cyan, magenta, yellow and black colorants.
        /// The intensities are considered to be in [0, 1] interval, if not,
        /// the intensity will be considered as 1 (when colorant's value is bigger than 1)
        /// or 0 (when colorant's value is less than 0).
        /// </remarks>
        /// <param name="c">the intensity of cyan colorant</param>
        /// <param name="m">the intensity of magenta colorant</param>
        /// <param name="y">the intensity of yellow colorant</param>
        /// <param name="k">the intensity of black colorant</param>
        public DeviceCmyk(float c, float m, float y, float k)
            : base(new PdfDeviceCs.Cmyk(), new float[] { c > 1 ? 1 : (c > 0 ? c : 0), m > 1 ? 1 : (m > 0 ? m : 0), y >
                 1 ? 1 : (y > 0 ? y : 0), k > 1 ? 1 : (k > 0 ? k : 0) }) {
            if (c > 1 || c < 0 || m > 1 || m < 0 || y > 1 || y < 0 || k > 1 || k < 0) {
                ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Colors.DeviceCmyk));
                LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.COLORANT_INTENSITIES_INVALID);
            }
        }

        /// <summary>
        /// Returns
        /// <see cref="DeviceCmyk">DeviceCmyk</see>
        /// color which is lighter than given one
        /// </summary>
        /// <param name="cmykColor">the DeviceCmyk color to be made lighter</param>
        /// <returns>lighter color</returns>
        public static iText.Kernel.Colors.DeviceCmyk MakeLighter(iText.Kernel.Colors.DeviceCmyk cmykColor) {
            DeviceRgb rgbEquivalent = ConvertCmykToRgb(cmykColor);
            DeviceRgb lighterRgb = DeviceRgb.MakeLighter((rgbEquivalent));
            return ConvertRgbToCmyk(lighterRgb);
        }

        /// <summary>
        /// Returns
        /// <see cref="DeviceCmyk">DeviceCmyk</see>
        /// color which is darker than given one
        /// </summary>
        /// <param name="cmykColor">the DeviceCmyk color to be made darker</param>
        /// <returns>darker color</returns>
        public static iText.Kernel.Colors.DeviceCmyk MakeDarker(iText.Kernel.Colors.DeviceCmyk cmykColor) {
            DeviceRgb rgbEquivalent = ConvertCmykToRgb(cmykColor);
            DeviceRgb darkerRgb = DeviceRgb.MakeDarker(rgbEquivalent);
            return ConvertRgbToCmyk(darkerRgb);
        }
    }
}
