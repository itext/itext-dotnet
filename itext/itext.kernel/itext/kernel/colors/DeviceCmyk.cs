/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
