/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iTextSharp.Kernel.Color;
using iTextSharp.Kernel.Pdf.Canvas;

namespace iTextSharp.Layout.Border
{
    /// <summary>Represents a border that is displayed using a 3D effect.</summary>
    public abstract class Border3D : iTextSharp.Layout.Border.Border
    {
        private static readonly DeviceRgb GRAY = new DeviceRgb(212, 208, 200);

        /// <summary>Creates a Border3D instance with the specified width.</summary>
        /// <remarks>Creates a Border3D instance with the specified width. Also sets the color to gray.
        ///     </remarks>
        /// <param name="width">with of the border</param>
        protected internal Border3D(float width)
            : this(GRAY, width)
        {
        }

        /// <summary>Creates a Border3D instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">with of the border</param>
        protected internal Border3D(DeviceRgb color, float width)
            : base(color, width)
        {
        }

        /// <summary>Creates a Border3D instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">with of the border</param>
        protected internal Border3D(DeviceCmyk color, float width)
            : base(color, width)
        {
        }

        /// <summary>Creates a Border3D instance with the specified width and color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">with of the border</param>
        protected internal Border3D(DeviceGray color, float width)
            : base(color, width)
        {
        }

        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2
            , float borderWidthBefore, float borderWidthAfter)
        {
            float x3 = 0;
            float y3 = 0;
            float x4 = 0;
            float y4 = 0;
            float widthHalf = width / 2;
            float halfOfWidthBefore = borderWidthBefore / 2;
            float halfOfWidthAfter = borderWidthAfter / 2;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2);
            switch (borderSide)
            {
                case Border.Side.TOP:
                {
                    x3 = x2 + halfOfWidthAfter;
                    y3 = y2 + widthHalf;
                    x4 = x1 - halfOfWidthBefore;
                    y4 = y1 + widthHalf;
                    break;
                }

                case Border.Side.RIGHT:
                {
                    x3 = x2 + widthHalf;
                    y3 = y2 - halfOfWidthAfter;
                    x4 = x1 + widthHalf;
                    y4 = y1 + halfOfWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM:
                {
                    x3 = x2 - halfOfWidthAfter;
                    y3 = y2 - widthHalf;
                    x4 = x1 + halfOfWidthBefore;
                    y4 = y1 - widthHalf;
                    break;
                }

                case Border.Side.LEFT:
                {
                    x3 = x2 - widthHalf;
                    y3 = y2 + halfOfWidthAfter;
                    x4 = x1 - widthHalf;
                    y4 = y1 - halfOfWidthBefore;
                    break;
                }
            }
            SetInnerHalfColor(canvas, borderSide);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1)
                .Fill();
            switch (borderSide)
            {
                case Border.Side.TOP:
                {
                    x2 += borderWidthAfter;
                    y2 += width;
                    x1 -= borderWidthBefore;
                    y1 += width;
                    break;
                }

                case Border.Side.RIGHT:
                {
                    x2 += width;
                    y2 -= borderWidthAfter;
                    x1 += width;
                    y1 += borderWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM:
                {
                    x2 -= borderWidthAfter;
                    y2 -= width;
                    x1 += borderWidthBefore;
                    y1 -= width;
                    break;
                }

                case Border.Side.LEFT:
                {
                    x2 -= width;
                    y2 += borderWidthAfter;
                    x1 -= width;
                    y1 -= borderWidthBefore;
                    break;
                }
            }
            SetOuterHalfColor(canvas, borderSide);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1)
                .Fill();
        }

        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2
            , float y2)
        {
            canvas.SaveState().SetStrokeColor(color).SetLineWidth(width).MoveTo(x1, y1).LineTo
                (x2, y2).Stroke().RestoreState();
        }

        protected internal virtual iTextSharp.Kernel.Color.Color GetDarkerColor()
        {
            if (color is DeviceRgb)
            {
                return DeviceRgb.MakeDarker((DeviceRgb)color);
            }
            else
            {
                if (color is DeviceCmyk)
                {
                    return DeviceCmyk.MakeDarker((DeviceCmyk)color);
                }
                else
                {
                    if (color is DeviceGray)
                    {
                        return DeviceGray.MakeDarker((DeviceGray)color);
                    }
                }
            }
            return color;
        }

        protected internal abstract void SetInnerHalfColor(PdfCanvas canvas, Border.Side 
            side);

        protected internal abstract void SetOuterHalfColor(PdfCanvas canvas, Border.Side 
            side);
    }
}
