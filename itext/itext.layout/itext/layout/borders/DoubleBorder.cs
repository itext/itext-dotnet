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
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Creates a double border around the element it's set to.</summary>
    /// <remarks>
    /// Creates a double border around the element it's set to. The space between the two border lines has
    /// the same width as the two borders. If a background has been set on the element the color will show in
    /// between the two borders.
    /// </remarks>
    public class DoubleBorder : Border {
        /// <summary>Creates a DoubleBorder with the specified width for both the two borders as the space in between them.
        ///     </summary>
        /// <remarks>
        /// Creates a DoubleBorder with the specified width for both the two borders as the space in between them.
        /// The color is set to the default: black.
        /// </remarks>
        /// <param name="width">width of the borders and the space between them</param>
        public DoubleBorder(float width)
            : base(width) {
        }

        /// <summary>
        /// Creates a DoubleBorder with the specified width for both the two borders as the space in between them and
        /// the specified color for the two borders.
        /// </summary>
        /// <remarks>
        /// Creates a DoubleBorder with the specified width for both the two borders as the space in between them and
        /// the specified color for the two borders. The space in between the two borders is either colorless or will
        /// be filled with the background color of the element, if a color has been set.
        /// </remarks>
        /// <param name="color">The color of the borders</param>
        /// <param name="width">The width of the borders and the space between them</param>
        public DoubleBorder(Color color, float width)
            : base(color, width) {
        }

        /// <summary>
        /// Creates a DoubleBorder with the specified width for both the two borders as the space in between them and
        /// the specified color for the two borders.
        /// </summary>
        /// <remarks>
        /// Creates a DoubleBorder with the specified width for both the two borders as the space in between them and
        /// the specified color for the two borders. The space in between the two borders is either colorless or will
        /// be filled with the background color of the element, if a color has been set.
        /// </remarks>
        /// <param name="color">The color of the borders</param>
        /// <param name="width">The width of the borders and the space between them</param>
        /// <param name="opacity">The opacity</param>
        public DoubleBorder(Color color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return Border.DOUBLE;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float x3 = 0;
            float y3 = 0;
            float x4 = 0;
            float y4 = 0;
            float thirdOfWidth = width / 3;
            float thirdOfWidthBefore = borderWidthBefore / 3;
            float thirdOfWidthAfter = borderWidthAfter / 3;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    x3 = x2 + thirdOfWidthAfter;
                    y3 = y2 + thirdOfWidth;
                    x4 = x1 - thirdOfWidthBefore;
                    y4 = y1 + thirdOfWidth;
                    break;
                }

                case Border.Side.RIGHT: {
                    x3 = x2 + thirdOfWidth;
                    y3 = y2 - thirdOfWidthAfter;
                    x4 = x1 + thirdOfWidth;
                    y4 = y1 + thirdOfWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM: {
                    x3 = x2 - thirdOfWidthAfter;
                    y3 = y2 - thirdOfWidth;
                    x4 = x1 + thirdOfWidthBefore;
                    y4 = y1 - thirdOfWidth;
                    break;
                }

                case Border.Side.LEFT: {
                    x3 = x2 - thirdOfWidth;
                    y3 = y2 + thirdOfWidthAfter;
                    x4 = x1 - thirdOfWidth;
                    y4 = y1 - thirdOfWidthBefore;
                    break;
                }
            }
            canvas.SaveState().SetFillColor(transparentColor.GetColor());
            transparentColor.ApplyFillTransparency(canvas);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1).Fill();
            switch (borderSide) {
                case Border.Side.TOP: {
                    x2 += 2 * thirdOfWidthAfter;
                    y2 += 2 * thirdOfWidth;
                    x3 += 2 * thirdOfWidthAfter;
                    y3 += 2 * thirdOfWidth;
                    x4 -= 2 * thirdOfWidthBefore;
                    y4 += 2 * thirdOfWidth;
                    x1 -= 2 * thirdOfWidthBefore;
                    y1 += 2 * thirdOfWidth;
                    break;
                }

                case Border.Side.RIGHT: {
                    x2 += 2 * thirdOfWidth;
                    y2 -= 2 * thirdOfWidthAfter;
                    x3 += 2 * thirdOfWidth;
                    y3 -= 2 * thirdOfWidthAfter;
                    x4 += 2 * thirdOfWidth;
                    y4 += 2 * thirdOfWidthBefore;
                    x1 += 2 * thirdOfWidth;
                    y1 += 2 * thirdOfWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM: {
                    x2 -= 2 * thirdOfWidthAfter;
                    y2 -= 2 * thirdOfWidth;
                    x3 -= 2 * thirdOfWidthAfter;
                    y3 -= 2 * thirdOfWidth;
                    x4 += 2 * thirdOfWidthBefore;
                    y4 -= 2 * thirdOfWidth;
                    x1 += 2 * thirdOfWidthBefore;
                    y1 -= 2 * thirdOfWidth;
                    break;
                }

                case Border.Side.LEFT: {
                    x2 -= 2 * thirdOfWidth;
                    y2 += 2 * thirdOfWidthAfter;
                    x3 -= 2 * thirdOfWidth;
                    y3 += 2 * thirdOfWidthAfter;
                    x4 -= 2 * thirdOfWidth;
                    y4 -= 2 * thirdOfWidthBefore;
                    x1 -= 2 * thirdOfWidth;
                    y1 -= 2 * thirdOfWidthBefore;
                    break;
                }
            }
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1).Fill().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            float thirdOfWidth = width / 3;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    y1 -= thirdOfWidth;
                    y2 = y1;
                    break;
                }

                case Border.Side.RIGHT: {
                    x1 -= thirdOfWidth;
                    x2 -= thirdOfWidth;
                    y1 += thirdOfWidth;
                    y2 -= thirdOfWidth;
                    break;
                }

                case Border.Side.BOTTOM: {
                    break;
                }

                case Border.Side.LEFT: {
                    break;
                }
            }
            canvas.SaveState().SetLineWidth(thirdOfWidth).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
            switch (borderSide) {
                case Border.Side.TOP: {
                    //                x1 -= 2*thirdOfWidth;
                    y2 += 2 * thirdOfWidth;
                    y1 += 2 * thirdOfWidth;
                    break;
                }

                case Border.Side.RIGHT: {
                    x2 += 2 * thirdOfWidth;
                    x1 += 2 * thirdOfWidth;
                    //                y1 -= 2*thirdOfWidth;
                    break;
                }

                case Border.Side.BOTTOM: {
                    x2 -= 2 * thirdOfWidth;
                    y2 -= 2 * thirdOfWidth;
                    x1 += 2 * thirdOfWidth;
                    y1 -= 2 * thirdOfWidth;
                    break;
                }

                case Border.Side.LEFT: {
                    y2 += 2 * thirdOfWidth;
                    x1 -= 2 * thirdOfWidth;
                    y1 -= 2 * thirdOfWidth;
                    break;
                }
            }
            canvas.SaveState().SetLineWidth(thirdOfWidth).SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
        }
    }
}
