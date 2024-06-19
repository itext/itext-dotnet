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
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;

namespace iText.Forms.Fields.Borders {
//\cond DO_NOT_DOCUMENT
    internal class BeveledBorder : AbstractFormBorder {
        private readonly Color backgroundColor;

        public BeveledBorder(Color color, float width, Color backgroundColor)
            : base(color, width) {
            this.backgroundColor = backgroundColor;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            SolidBorder solidBorder = new SolidBorder(GetColor(), width);
            solidBorder.Draw(canvas, x1, y1, x2, y2, defaultSide, borderWidthBefore, borderWidthAfter);
            float borderWidth = GetWidth();
            float borderWidthX2 = borderWidth + borderWidth;
            Color darkerBackground = backgroundColor != null ? GetDarkerColor(backgroundColor) : ColorConstants.LIGHT_GRAY;
            if (Border.Side.TOP.Equals(defaultSide)) {
                solidBorder = new SolidBorder(ColorConstants.WHITE, borderWidth);
                solidBorder.Draw(canvas, borderWidthX2, y1 - borderWidth, x2 - borderWidth, y2 - borderWidth, Border.Side.
                    TOP, borderWidth, borderWidth);
            }
            else {
                if (Border.Side.BOTTOM.Equals(defaultSide)) {
                    solidBorder = new SolidBorder(darkerBackground, borderWidth);
                    solidBorder.Draw(canvas, x1 - borderWidth, borderWidthX2, borderWidthX2, borderWidthX2, Border.Side.BOTTOM
                        , borderWidth, borderWidth);
                }
                else {
                    if (Border.Side.LEFT.Equals(defaultSide)) {
                        solidBorder = new SolidBorder(ColorConstants.WHITE, borderWidth);
                        solidBorder.Draw(canvas, borderWidthX2, borderWidthX2, borderWidthX2, y2 - borderWidth, Border.Side.LEFT, 
                            borderWidth, borderWidth);
                    }
                    else {
                        if (Border.Side.RIGHT.Equals(defaultSide)) {
                            solidBorder = new SolidBorder(darkerBackground, borderWidth);
                            solidBorder.Draw(canvas, x1 - borderWidth, y1 - borderWidth, x2 - borderWidth, borderWidthX2, Border.Side.
                                RIGHT, borderWidth, borderWidth);
                        }
                    }
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return AbstractFormBorder.FORM_BEVELED;
        }

        private Color GetDarkerColor(Color color) {
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
                    else {
                        return color;
                    }
                }
            }
        }
    }
//\endcond
}
