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
    internal class UnderlineBorder : AbstractFormBorder {
        public UnderlineBorder(Color color, float width)
            : base(color, width) {
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            if (Border.Side.BOTTOM.Equals(defaultSide)) {
                if (x1 < x2) {
                    x1 = x1 - width;
                    x2 = x2 + width;
                }
                else {
                    x1 = x1 + width;
                    x2 = x2 - width;
                }
                SolidBorder solidBorder = new SolidBorder(GetColor(), width);
                solidBorder.Draw(canvas, x1, y1, x2, y2, defaultSide, 0, 0);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return AbstractFormBorder.FORM_UNDERLINE;
        }
    }
}
