/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    /// <summary>Color space to specify colors according to RGB color model.</summary>
    public class DeviceRgb : Color {
        /// <summary>Predefined black DeviceRgb color</summary>
        public static readonly Color BLACK = new iText.Kernel.Colors.DeviceRgb(0, 0, 0);

        /// <summary>Predefined white DeviceRgb color</summary>
        public static readonly Color WHITE = new iText.Kernel.Colors.DeviceRgb(255, 255, 255);

        /// <summary>Predefined red DeviceRgb color</summary>
        public static readonly Color RED = new iText.Kernel.Colors.DeviceRgb(255, 0, 0);

        /// <summary>Predefined green DeviceRgb color</summary>
        public static readonly Color GREEN = new iText.Kernel.Colors.DeviceRgb(0, 255, 0);

        /// <summary>Predefined blue  DeviceRgb color</summary>
        public static readonly Color BLUE = new iText.Kernel.Colors.DeviceRgb(0, 0, 255);

        /// <summary>Creates DeviceRgb color by intensities of red, green and blue colorants.</summary>
        /// <remarks>
        /// Creates DeviceRgb color by intensities of red, green and blue colorants.
        /// The intensities are considered to be in [0, 255] gap, if not,
        /// the intensity will be considered as 255 (when colorant's value is bigger than 255)
        /// or 0 (when colorant's value is less than 0).
        /// </remarks>
        /// <param name="r">the intensity of red colorant</param>
        /// <param name="g">the intensity of green colorant</param>
        /// <param name="b">the intensity of blue colorant</param>
        public DeviceRgb(int r, int g, int b)
            : this(r / 255f, g / 255f, b / 255f) {
        }

        /// <summary>Creates DeviceRgb color by intensities of red, green and blue colorants.</summary>
        /// <remarks>
        /// Creates DeviceRgb color by intensities of red, green and blue colorants.
        /// The intensities are considered to be in [0, 1] interval, if not,
        /// the intensity will be considered as 1 (when colorant's value is bigger than 1)
        /// or 0 (when colorant's value is less than 0).
        /// </remarks>
        /// <param name="r">the intensity of red colorant</param>
        /// <param name="g">the intensity of green colorant</param>
        /// <param name="b">the intensity of blue colorant</param>
        public DeviceRgb(float r, float g, float b)
            : base(new PdfDeviceCs.Rgb(), new float[] { r > 1 ? 1 : (r > 0 ? r : 0), g > 1 ? 1 : (g > 0 ? g : 0), b > 
                1 ? 1 : (b > 0 ? b : 0) }) {
            if (r > 1 || r < 0 || g > 1 || g < 0 || b > 1 || b < 0) {
                ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Colors.DeviceRgb));
                LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.COLORANT_INTENSITIES_INVALID);
            }
        }

        /// <summary>Creates DeviceRgb color with all colorants intensities initialised as zeroes.</summary>
        public DeviceRgb()
            : this(0f, 0f, 0f) {
        }

        /// <summary>
        /// Create DeviceRGB color from R, G, B values of System.Drawing.Color
        /// <br/>
        /// Note, that alpha chanel is ignored, but opacity still can be achieved
        /// in some places by using 'setOpacity' method or 'TransparentColor' class.
        /// </summary>
        /// <param name="color">the color which RGB values are used</param>
        public DeviceRgb(System.Drawing.Color color)
            : this(color.R, color.G, color.B) {
            if (color.A != 255) {
                ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Colors.DeviceRgb));
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COLOR_ALPHA_CHANNEL_IS_IGNORED, color.A));
            }
        }

        /// <summary>
        /// Returns
        /// <see cref="DeviceRgb">DeviceRgb</see>
        /// color which is lighter than given one
        /// </summary>
        /// <param name="rgbColor">the DeviceRgb color to be made lighter</param>
        /// <returns>lighter color</returns>
        public static iText.Kernel.Colors.DeviceRgb MakeLighter(iText.Kernel.Colors.DeviceRgb rgbColor) {
            float r = rgbColor.GetColorValue()[0];
            float g = rgbColor.GetColorValue()[1];
            float b = rgbColor.GetColorValue()[2];
            float v = Math.Max(r, Math.Max(g, b));
            if (v == 0f) {
                return new iText.Kernel.Colors.DeviceRgb(0x54, 0x54, 0x54);
            }
            float multiplier = Math.Min(1f, v + 0.33f) / v;
            r = multiplier * r;
            g = multiplier * g;
            b = multiplier * b;
            return new iText.Kernel.Colors.DeviceRgb(r, g, b);
        }

        /// <summary>
        /// Returns
        /// <see cref="DeviceRgb">DeviceRgb</see>
        /// color which is darker than given one
        /// </summary>
        /// <param name="rgbColor">the DeviceRgb color to be made darker</param>
        /// <returns>darker color</returns>
        public static iText.Kernel.Colors.DeviceRgb MakeDarker(iText.Kernel.Colors.DeviceRgb rgbColor) {
            float r = rgbColor.GetColorValue()[0];
            float g = rgbColor.GetColorValue()[1];
            float b = rgbColor.GetColorValue()[2];
            float v = Math.Max(r, Math.Max(g, b));
            float multiplier = Math.Max(0f, (v - 0.33f) / v);
            r = multiplier * r;
            g = multiplier * g;
            b = multiplier * b;
            return new iText.Kernel.Colors.DeviceRgb(r, g, b);
        }
    }
}
