/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Layout.Borders {
    /// <summary>Draws a solid border around the element it's set to.</summary>
    public class SolidBorder : Border {
        /// <summary>Creates a SolidBorder with the specified width and sets the color to black.</summary>
        /// <param name="width">width of the border</param>
        public SolidBorder(float width)
            : base(width) {
        }

        /// <summary>Creates a SolidBorder with the specified width and the specified color.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        public SolidBorder(Color color, float width)
            : base(color, width) {
        }

        /// <summary>Creates a SolidBorder with the specified width, color and opacity.</summary>
        /// <param name="color">color of the border</param>
        /// <param name="width">width of the border</param>
        /// <param name="opacity">the opacity which border should have</param>
        public SolidBorder(Color color, float width, float opacity)
            : base(color, width, opacity) {
        }

        /// <summary><inheritDoc/></summary>
        public override int GetBorderType() {
            return SOLID;
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter) {
            float x3 = 0;
            float y3 = 0;
            float x4 = 0;
            float y4 = 0;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    x3 = x2 + borderWidthAfter;
                    y3 = y2 + width;
                    x4 = x1 - borderWidthBefore;
                    y4 = y1 + width;
                    break;
                }

                case Border.Side.RIGHT: {
                    x3 = x2 + width;
                    y3 = y2 - borderWidthAfter;
                    x4 = x1 + width;
                    y4 = y1 + borderWidthBefore;
                    break;
                }

                case Border.Side.BOTTOM: {
                    x3 = x2 - borderWidthAfter;
                    y3 = y2 - width;
                    x4 = x1 + borderWidthBefore;
                    y4 = y1 - width;
                    break;
                }

                case Border.Side.LEFT: {
                    x3 = x2 - width;
                    y3 = y2 + borderWidthAfter;
                    x4 = x1 - width;
                    y4 = y1 - borderWidthBefore;
                    break;
                }
            }
            canvas.SaveState().SetFillColor(transparentColor.GetColor());
            transparentColor.ApplyFillTransparency(canvas);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1).Fill().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float horizontalRadius1
            , float verticalRadius1, float horizontalRadius2, float verticalRadius2, Border.Side defaultSide, float
             borderWidthBefore, float borderWidthAfter) {
            float x3 = 0;
            float y3 = 0;
            float x4 = 0;
            float y4 = 0;
            float innerRadiusBefore;
            float innerRadiusFirst;
            float innerRadiusSecond;
            float innerRadiusAfter;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    x3 = x2 + borderWidthAfter;
                    y3 = y2 + width;
                    x4 = x1 - borderWidthBefore;
                    y4 = y1 + width;
                    innerRadiusBefore = Math.Max(0, horizontalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, verticalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, verticalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, horizontalRadius2 - borderWidthAfter);
                    if (innerRadiusBefore > innerRadiusFirst) {
                        x1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x4, y1 - innerRadiusFirst
                            ), new Point(x1 + innerRadiusBefore, y1 - innerRadiusFirst)).GetX();
                        y1 -= innerRadiusFirst;
                    }
                    else {
                        if (0 != innerRadiusBefore && 0 != innerRadiusFirst) {
                            y1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1 + innerRadiusBefore, y1
                                ), new Point(x1 + innerRadiusBefore, y1 - innerRadiusFirst)).GetY();
                            x1 += innerRadiusBefore;
                        }
                    }
                    if (innerRadiusAfter > innerRadiusSecond) {
                        x2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2, y2 - innerRadiusSecond
                            ), new Point(x2 - innerRadiusAfter, y2 - innerRadiusSecond)).GetX();
                        y2 -= innerRadiusSecond;
                    }
                    else {
                        if (0 != innerRadiusAfter && 0 != innerRadiusSecond) {
                            y2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2 - innerRadiusAfter, y2
                                ), new Point(x2 - innerRadiusAfter, y2 - innerRadiusSecond)).GetY();
                            x2 -= innerRadiusAfter;
                        }
                    }
                    break;
                }

                case Border.Side.RIGHT: {
                    x3 = x2 + width;
                    y3 = y2 - borderWidthAfter;
                    x4 = x1 + width;
                    y4 = y1 + borderWidthBefore;
                    innerRadiusBefore = Math.Max(0, verticalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, horizontalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, horizontalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, verticalRadius2 - borderWidthAfter);
                    if (innerRadiusFirst > innerRadiusBefore) {
                        x1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1, y1 - innerRadiusBefore
                            ), new Point(x1 - innerRadiusFirst, y1 - innerRadiusBefore)).GetX();
                        y1 -= innerRadiusBefore;
                    }
                    else {
                        if (0 != innerRadiusBefore && 0 != innerRadiusFirst) {
                            y1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1 - innerRadiusFirst, y1
                                ), new Point(x1 - innerRadiusFirst, y1 - innerRadiusBefore)).GetY();
                            x1 -= innerRadiusFirst;
                        }
                    }
                    if (innerRadiusAfter > innerRadiusSecond) {
                        y2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2 - innerRadiusSecond, y2
                            ), new Point(x2 - innerRadiusSecond, y2 + innerRadiusAfter)).GetY();
                        x2 -= innerRadiusSecond;
                    }
                    else {
                        if (0 != innerRadiusAfter && 0 != innerRadiusSecond) {
                            x2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2, y2 + innerRadiusAfter
                                ), new Point(x2 - innerRadiusSecond, y2 + innerRadiusAfter)).GetX();
                            y2 += innerRadiusAfter;
                        }
                    }
                    break;
                }

                case Border.Side.BOTTOM: {
                    x3 = x2 - borderWidthAfter;
                    y3 = y2 - width;
                    x4 = x1 + borderWidthBefore;
                    y4 = y1 - width;
                    innerRadiusBefore = Math.Max(0, horizontalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, verticalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, verticalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, horizontalRadius2 - borderWidthAfter);
                    if (innerRadiusFirst > innerRadiusBefore) {
                        y1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1 - innerRadiusBefore, y1
                            ), new Point(x1 - innerRadiusBefore, y1 + innerRadiusFirst)).GetY();
                        x1 -= innerRadiusBefore;
                    }
                    else {
                        if (0 != innerRadiusBefore && 0 != innerRadiusFirst) {
                            x1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1, y1 + innerRadiusFirst
                                ), new Point(x1 - innerRadiusBefore, y1 + innerRadiusFirst)).GetX();
                            y1 += innerRadiusFirst;
                        }
                    }
                    if (innerRadiusAfter > innerRadiusSecond) {
                        x2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2, y2 + innerRadiusSecond
                            ), new Point(x2 + innerRadiusAfter, y2 + innerRadiusSecond)).GetX();
                        y2 += innerRadiusSecond;
                    }
                    else {
                        if (0 != innerRadiusAfter && 0 != innerRadiusSecond) {
                            y2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2 + innerRadiusAfter, y2
                                ), new Point(x2 + innerRadiusAfter, y2 + innerRadiusSecond)).GetY();
                            x2 += innerRadiusAfter;
                        }
                    }
                    break;
                }

                case Border.Side.LEFT: {
                    x3 = x2 - width;
                    y3 = y2 + borderWidthAfter;
                    x4 = x1 - width;
                    y4 = y1 - borderWidthBefore;
                    innerRadiusBefore = Math.Max(0, verticalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, horizontalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, horizontalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, verticalRadius2 - borderWidthAfter);
                    if (innerRadiusFirst > innerRadiusBefore) {
                        x1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1, y1 + innerRadiusBefore
                            ), new Point(x1 + innerRadiusFirst, y1 + innerRadiusBefore)).GetX();
                        y1 += innerRadiusBefore;
                    }
                    else {
                        if (0 != innerRadiusBefore && 0 != innerRadiusFirst) {
                            y1 = (float)GetIntersectionPoint(new Point(x1, y1), new Point(x4, y4), new Point(x1 + innerRadiusFirst, y1
                                ), new Point(x1 + innerRadiusFirst, y1 + innerRadiusBefore)).GetY();
                            x1 += innerRadiusFirst;
                        }
                    }
                    if (innerRadiusAfter > innerRadiusSecond) {
                        y2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2 + innerRadiusSecond, y2
                            ), new Point(x2 + innerRadiusSecond, y2 - innerRadiusAfter)).GetY();
                        x2 += innerRadiusSecond;
                    }
                    else {
                        if (0 != innerRadiusAfter && 0 != innerRadiusSecond) {
                            x2 = (float)GetIntersectionPoint(new Point(x2, y2), new Point(x3, y3), new Point(x2, y2 - innerRadiusAfter
                                ), new Point(x2 + innerRadiusSecond, y2 - innerRadiusAfter)).GetX();
                            y2 -= innerRadiusAfter;
                        }
                    }
                    break;
                }
            }
            canvas.SaveState().SetFillColor(transparentColor.GetColor());
            transparentColor.ApplyFillTransparency(canvas);
            canvas.MoveTo(x1, y1).LineTo(x2, y2).LineTo(x3, y3).LineTo(x4, y4).LineTo(x1, y1).Fill().RestoreState();
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide) {
            canvas.SaveState().SetStrokeColor(transparentColor.GetColor());
            transparentColor.ApplyStrokeTransparency(canvas);
            canvas.SetLineWidth(width).MoveTo(x1, y1).LineTo(x2, y2).Stroke().RestoreState();
        }
    }
}
