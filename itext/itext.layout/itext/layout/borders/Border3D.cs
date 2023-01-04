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
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Represents a border that is displayed using a 3D effect.</summary>
    public abstract class Border3D : Border {
        /// <summary>
        /// Predefined gray
        /// <see cref="iText.Kernel.Colors.DeviceRgb">RGB-color</see>
        /// </summary>
        private static readonly DeviceRgb GRAY = new DeviceRgb(212, 208, 200);

        /// <summary>Creates a Border3D instance with the specified width.</summary>
        /// <remarks>Creates a Border3D instance with the specified width. Also sets the color to gray.</remarks>
        /// <param name="width">with of the border</param>
        protected internal Border3D(float width)
            : this(GRAY, width) {
        }

        /// <summary>Creates a Border3D instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        protected internal Border3D(DeviceRgb color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a Border3D instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        protected internal Border3D(DeviceCmyk color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a Border3D instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        protected internal Border3D(DeviceGray color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a Border3D instance with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">opacity of the border</param>
        protected internal Border3D(DeviceRgb color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary>Creates a Border3D instance with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">opacity of the border</param>
        protected internal Border3D(DeviceCmyk color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary>Creates a Border3D instance with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">opacity of the border</param>
        protected internal Border3D(DeviceGray color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float x3 = 0;
            float y3 = 0;
            float x4 = 0;
            float y4 = 0;
            float widthHalf = width / 2;
            float halfOfWidthBefore = borderWidthBefore / 2;
            float halfOfWidthAfter = borderWidthAfter / 2;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    x3 = x2 + halfOfWidthAfter;
                    y3 = y2 + widthHalf;
                    x4 = x1 - halfOfWidthBefore;
                    y4 = y1 + widthHalf;
                    break;
                }

                case Border.Side.RIGHT: {
                    x3 = x2 + widthHalf;
                    y3 = y2 - halfOfWidthAfter;
                    x4 = x1 + widthHalf;
                    y4 = y1 + halfOfWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM: {
                    x3 = x2 - halfOfWidthAfter;
                    y3 = y2 - widthHalf;
                    x4 = x1 + halfOfWidthBefore;
                    y4 = y1 - widthHalf;
                    break;
                }

                case Border.Side.LEFT: {
                    x3 = x2 - widthHalf;
                    y3 = y2 + halfOfWidthAfter;
                    x4 = x1 - widthHalf;
                    y4 = y1 - halfOfWidthBefore;
                    break;
                }
            }
            canvas.SaveState();
            transparentColor.ApplyFillTransparency(canvas);
            SetInnerHalfColor(canvas, borderSide);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1).Fill();
            switch (borderSide) {
                case Border.Side.TOP: {
                    x2 += borderWidthAfter;
                    y2 += width;
                    x1 -= borderWidthBefore;
                    y1 += width;
                    break;
                }

                case Border.Side.RIGHT: {
                    x2 += width;
                    y2 -= borderWidthAfter;
                    x1 += width;
                    y1 += borderWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM: {
                    x2 -= borderWidthAfter;
                    y2 -= width;
                    x1 += borderWidthBefore;
                    y1 -= width;
                    break;
                }

                case Border.Side.LEFT: {
                    x2 -= width;
                    y2 += borderWidthAfter;
                    x1 -= width;
                    y1 -= borderWidthBefore;
                    break;
                }
            }
            SetOuterHalfColor(canvas, borderSide);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1).Fill();
            canvas.RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            canvas.SaveState().SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineWidth(width).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
        }

        /// <summary>
        /// Makes the
        /// <see cref="Border.transparentColor"/>
        /// color of the border darker and returns the result
        /// </summary>
        /// <returns>The darker color</returns>
        protected internal virtual Color GetDarkerColor() {
            Color color = this.transparentColor.GetColor();
            if (color is DeviceRgb) {
                return DeviceRgb.MakeDarker((DeviceRgb)color);
            }
            else {
                if (color is DeviceCmyk) {
                    return DeviceCmyk.MakeDarker((DeviceCmyk)color);
                }
                else {
                    if (color is DeviceGray) {
                        return DeviceGray.MakeDarker((DeviceGray)color);
                    }
                }
            }
            return color;
        }

        /// <summary>
        /// Sets the fill color for the inner half of
        /// <see cref="Border3D">3D Border</see>
        /// </summary>
        /// <param name="canvas">PdfCanvas the color will be applied on</param>
        /// <param name="side">
        /// the
        /// <see cref="Side">side</see>
        /// the color will be applied on
        /// </param>
        protected internal abstract void SetInnerHalfColor(PdfCanvas canvas, Border.Side side);

        /// <summary>
        /// Sets the fill color for the outer half of
        /// <see cref="Border3D">3D Border</see>
        /// </summary>
        /// <param name="canvas">PdfCanvas the color will be applied on</param>
        /// <param name="side">
        /// the
        /// <see cref="Side">side</see>
        /// the color will be applied on
        /// </param>
        protected internal abstract void SetOuterHalfColor(PdfCanvas canvas, Border.Side side);
    }
}
