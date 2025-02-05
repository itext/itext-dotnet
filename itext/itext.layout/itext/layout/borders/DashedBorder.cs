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
using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Draws a border with dashes around the element it's been set to.</summary>
    public class DashedBorder : Border {
        /// <summary>The modifier to be applied on the width to have the dash size</summary>
        private const float DASH_MODIFIER = 5f;

        /// <summary>The modifier to be applied on the width to have the initial gap size</summary>
        private const float GAP_MODIFIER = 3.5f;

        /// <summary>Creates a DashedBorder with the specified width and sets the color to black.</summary>
        /// <param name="width">width of the border</param>
        public DashedBorder(float width)
            : base(width) {
        }

        /// <summary>Creates a DashedBorder with the specified width and the specified color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        public DashedBorder(Color color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a DashedBorder with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">width of the border</param>
        public DashedBorder(Color color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return Border.DASHED;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dash = width * DASH_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap + dash);
            if (adjustedGap > dash) {
                adjustedGap -= dash;
            }
            new FixedDashedBorder(GetColor(), width, GetOpacity(), dash, adjustedGap, dash + adjustedGap / 2).Draw(canvas
                , x1, y1, x2, y2, defaultSide, borderWidthBefore, borderWidthAfter);
        }

        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float horizontalRadius1
            , float verticalRadius1, float horizontalRadius2, float verticalRadius2, Border.Side defaultSide, float
             borderWidthBefore, float borderWidthAfter) {
            float initialGap = width * GAP_MODIFIER;
            float dash = width * DASH_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap + dash);
            if (adjustedGap > dash) {
                adjustedGap -= dash;
            }
            new FixedDashedBorder(GetColor(), width, GetOpacity(), dash, adjustedGap, dash + adjustedGap / 2).Draw(canvas
                , x1, y1, x2, y2, horizontalRadius1, verticalRadius1, horizontalRadius2, verticalRadius2, defaultSide, 
                borderWidthBefore, borderWidthAfter);
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            float initialGap = width * GAP_MODIFIER;
            float dash = width * DASH_MODIFIER;
            float dx = x2 - x1;
            float dy = y2 - y1;
            double borderLength = Math.Sqrt(dx * dx + dy * dy);
            float adjustedGap = base.GetDotsGap(borderLength, initialGap + dash);
            if (adjustedGap > dash) {
                adjustedGap -= dash;
            }
            new FixedDashedBorder(GetColor(), width, GetOpacity(), dash, adjustedGap, dash + adjustedGap / 2).DrawCellBorder
                (canvas, x1, y1, x2, y2, defaultSide);
        }
    }
}
