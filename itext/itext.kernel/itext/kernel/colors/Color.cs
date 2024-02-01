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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Colors {
    /// <summary>Represents a color</summary>
    public class Color {
        /// <summary>The color space of the color</summary>
        protected internal PdfColorSpace colorSpace;

        /// <summary>The color value of the color</summary>
        protected internal float[] colorValue;

        /// <summary>Creates a Color of certain color space and color value.</summary>
        /// <remarks>
        /// Creates a Color of certain color space and color value.
        /// If color value is set in null, all value components will be initialised with zeroes.
        /// </remarks>
        /// <param name="colorSpace">the color space to which the created Color object relates</param>
        /// <param name="colorValue">the color value of the created Color object</param>
        protected internal Color(PdfColorSpace colorSpace, float[] colorValue) {
            this.colorSpace = colorSpace;
            if (colorValue == null) {
                this.colorValue = new float[colorSpace.GetNumberOfComponents()];
            }
            else {
                this.colorValue = colorValue;
            }
        }

        /// <summary>Makes a Color of certain color space.</summary>
        /// <remarks>
        /// Makes a Color of certain color space.
        /// All color value components will be initialised with zeroes.
        /// </remarks>
        /// <param name="colorSpace">the color space to which the returned Color object relates</param>
        /// <returns>the created Color object.</returns>
        public static iText.Kernel.Colors.Color MakeColor(PdfColorSpace colorSpace) {
            return MakeColor(colorSpace, null);
        }

        /// <summary>Makes a Color of certain color space and color value.</summary>
        /// <remarks>
        /// Makes a Color of certain color space and color value.
        /// If color value is set in null, all value components will be initialised with zeroes.
        /// </remarks>
        /// <param name="colorSpace">the color space to which the returned Color object relates</param>
        /// <param name="colorValue">the color value of the returned Color object</param>
        /// <returns>the created Color object.</returns>
        public static iText.Kernel.Colors.Color MakeColor(PdfColorSpace colorSpace, float[] colorValue) {
            iText.Kernel.Colors.Color c = null;
            bool unknownColorSpace = false;
            if (colorSpace is PdfDeviceCs) {
                if (colorSpace is PdfDeviceCs.Gray) {
                    c = colorValue != null ? new DeviceGray(colorValue[0]) : new DeviceGray();
                }
                else {
                    if (colorSpace is PdfDeviceCs.Rgb) {
                        c = colorValue != null ? new DeviceRgb(colorValue[0], colorValue[1], colorValue[2]) : new DeviceRgb();
                    }
                    else {
                        if (colorSpace is PdfDeviceCs.Cmyk) {
                            c = colorValue != null ? new DeviceCmyk(colorValue[0], colorValue[1], colorValue[2], colorValue[3]) : new 
                                DeviceCmyk();
                        }
                        else {
                            unknownColorSpace = true;
                        }
                    }
                }
            }
            else {
                if (colorSpace is PdfCieBasedCs) {
                    if (colorSpace is PdfCieBasedCs.CalGray) {
                        PdfCieBasedCs.CalGray calGray = (PdfCieBasedCs.CalGray)colorSpace;
                        c = colorValue != null ? new CalGray(calGray, colorValue[0]) : new CalGray(calGray);
                    }
                    else {
                        if (colorSpace is PdfCieBasedCs.CalRgb) {
                            PdfCieBasedCs.CalRgb calRgb = (PdfCieBasedCs.CalRgb)colorSpace;
                            c = colorValue != null ? new CalRgb(calRgb, colorValue) : new CalRgb(calRgb);
                        }
                        else {
                            if (colorSpace is PdfCieBasedCs.IccBased) {
                                PdfCieBasedCs.IccBased iccBased = (PdfCieBasedCs.IccBased)colorSpace;
                                c = colorValue != null ? new IccBased(iccBased, colorValue) : new IccBased(iccBased);
                            }
                            else {
                                if (colorSpace is PdfCieBasedCs.Lab) {
                                    PdfCieBasedCs.Lab lab = (PdfCieBasedCs.Lab)colorSpace;
                                    c = colorValue != null ? new Lab(lab, colorValue) : new Lab(lab);
                                }
                                else {
                                    unknownColorSpace = true;
                                }
                            }
                        }
                    }
                }
                else {
                    if (colorSpace is PdfSpecialCs) {
                        if (colorSpace is PdfSpecialCs.Separation) {
                            PdfSpecialCs.Separation separation = (PdfSpecialCs.Separation)colorSpace;
                            c = colorValue != null ? new Separation(separation, colorValue[0]) : new Separation(separation);
                        }
                        else {
                            if (colorSpace is PdfSpecialCs.DeviceN) {
                                //NChannel goes here also
                                PdfSpecialCs.DeviceN deviceN = (PdfSpecialCs.DeviceN)colorSpace;
                                c = colorValue != null ? new DeviceN(deviceN, colorValue) : new DeviceN(deviceN);
                            }
                            else {
                                if (colorSpace is PdfSpecialCs.Indexed) {
                                    c = colorValue != null ? new Indexed(colorSpace, (int)colorValue[0]) : new Indexed(colorSpace);
                                }
                                else {
                                    unknownColorSpace = true;
                                }
                            }
                        }
                    }
                    else {
                        if (colorSpace is PdfSpecialCs.Pattern) {
                            c = new iText.Kernel.Colors.Color(colorSpace, colorValue);
                        }
                        else {
                            unknownColorSpace = true;
                        }
                    }
                }
            }
            if (unknownColorSpace) {
                throw new PdfException("Unknown color space.");
            }
            return c;
        }

        /// <summary>
        /// Converts
        /// <see cref="DeviceCmyk">DeviceCmyk</see>
        /// color to
        /// <see cref="DeviceRgb">DeviceRgb</see>
        /// color
        /// </summary>
        /// <param name="cmykColor">the DeviceCmyk color which will be converted to DeviceRgb color</param>
        /// <returns>converted color</returns>
        public static DeviceRgb ConvertCmykToRgb(DeviceCmyk cmykColor) {
            float cyanComp = 1 - cmykColor.GetColorValue()[0];
            float magentaComp = 1 - cmykColor.GetColorValue()[1];
            float yellowComp = 1 - cmykColor.GetColorValue()[2];
            float blackComp = 1 - cmykColor.GetColorValue()[3];
            float r = cyanComp * blackComp;
            float g = magentaComp * blackComp;
            float b = yellowComp * blackComp;
            return new DeviceRgb(r, g, b);
        }

        /// <summary>
        /// Converts
        /// <see cref="DeviceRgb">DeviceRgb</see>
        /// color to
        /// <see cref="DeviceCmyk">DeviceCmyk</see>
        /// color
        /// </summary>
        /// <param name="rgbColor">the DeviceRgb color which will be converted to DeviceCmyk color</param>
        /// <returns>converted color</returns>
        public static DeviceCmyk ConvertRgbToCmyk(DeviceRgb rgbColor) {
            float redComp = rgbColor.GetColorValue()[0];
            float greenComp = rgbColor.GetColorValue()[1];
            float blueComp = rgbColor.GetColorValue()[2];
            float k = 1 - Math.Max(Math.Max(redComp, greenComp), blueComp);
            float c = (1 - redComp - k) / (1 - k);
            float m = (1 - greenComp - k) / (1 - k);
            float y = (1 - blueComp - k) / (1 - k);
            return new DeviceCmyk(c, m, y, k);
        }

        /// <summary>Creates a color object based on the passed through values.</summary>
        /// <remarks>
        /// Creates a color object based on the passed through values.
        /// <para />
        /// </remarks>
        /// <param name="colorValue">
        /// the float array with the values
        /// <para />
        /// The number of array elements determines the colour space in which the colour shall be defined:
        /// 0 - No colour; transparent
        /// 1 - DeviceGray
        /// 3 - DeviceRGB
        /// 4 - DeviceCMYK
        /// </param>
        /// <returns>Color the color or null if it's invalid</returns>
        public static iText.Kernel.Colors.Color CreateColorWithColorSpace(float[] colorValue) {
            if (colorValue == null || colorValue.Length == 0) {
                return null;
            }
            if (colorValue.Length == 1) {
                return MakeColor(new PdfDeviceCs.Gray(), colorValue);
            }
            if (colorValue.Length == 3) {
                return MakeColor(new PdfDeviceCs.Rgb(), colorValue);
            }
            if (colorValue.Length == 4) {
                return MakeColor(new PdfDeviceCs.Cmyk(), colorValue);
            }
            return null;
        }

        /// <summary>Returns the number of color value components</summary>
        /// <returns>the number of color value components</returns>
        public virtual int GetNumberOfComponents() {
            return colorValue.Length;
        }

        /// <summary>
        /// Returns the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace">color space</see>
        /// to which the color is related.
        /// </summary>
        /// <returns>the color space of the color</returns>
        public virtual PdfColorSpace GetColorSpace() {
            return colorSpace;
        }

        /// <summary>Returns the color value of the color</summary>
        /// <returns>the color value</returns>
        public virtual float[] GetColorValue() {
            return colorValue;
        }

        /// <summary>Sets the color value of the color</summary>
        /// <param name="value">new color value</param>
        public virtual void SetColorValue(float[] value) {
            if (colorValue.Length != value.Length) {
                throw new PdfException(KernelExceptionMessageConstant.INCORRECT_NUMBER_OF_COMPONENTS, this);
            }
            colorValue = value;
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            int result = colorSpace == null ? 0 : colorSpace.GetPdfObject().GetHashCode();
            result = 31 * result + (colorValue != null ? JavaUtil.ArraysHashCode(colorValue) : 0);
            return result;
        }

        /// <summary>Indicates whether the color is equal to the given color.</summary>
        /// <remarks>
        /// Indicates whether the color is equal to the given color.
        /// The
        /// <see cref="colorSpace">color space</see>
        /// and
        /// <see cref="colorValue">color value</see>
        /// are considered during the
        /// comparison.
        /// </remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Kernel.Colors.Color color = (iText.Kernel.Colors.Color)o;
            return (colorSpace != null ? colorSpace.GetPdfObject().Equals(color.colorSpace.GetPdfObject()) : color.colorSpace
                 == null) && JavaUtil.ArraysEquals(colorValue, color.colorValue);
        }
    }
}
