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
using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;

namespace iText.Forms.Fields.Borders {
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
}
