/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal class LineHeightHelper {
        private static float DEFAULT_LINE_HEIGHT_COEFF = 1.15f;

        private LineHeightHelper() {
        }

        internal static float[] GetActualAscenderDescender(AbstractRenderer renderer) {
            float ascender;
            float descender;
            float lineHeight = iText.Layout.Renderer.LineHeightHelper.CalculateLineHeight(renderer);
            float[] fontAscenderDescender = iText.Layout.Renderer.LineHeightHelper.GetFontAscenderDescenderNormalized(
                renderer);
            float leading = lineHeight - (fontAscenderDescender[0] - fontAscenderDescender[1]);
            ascender = fontAscenderDescender[0] + leading / 2f;
            descender = fontAscenderDescender[1] - leading / 2f;
            return new float[] { ascender, descender };
        }

        internal static float[] GetFontAscenderDescenderNormalized(AbstractRenderer renderer) {
            PdfFont font = renderer.ResolveFirstPdfFont();
            float fontSize = renderer.GetPropertyAsUnitValue(Property.FONT_SIZE).GetValue();
            float[] fontAscenderDescenderFromMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            float fontAscender = fontAscenderDescenderFromMetrics[0] / FontProgram.UNITS_NORMALIZATION * fontSize;
            float fontDescender = fontAscenderDescenderFromMetrics[1] / FontProgram.UNITS_NORMALIZATION * fontSize;
            return new float[] { fontAscender, fontDescender };
        }

        internal static float CalculateLineHeight(AbstractRenderer renderer) {
            LineHeight lineHeight = renderer.GetProperty<LineHeight>(Property.LINE_HEIGHT);
            float fontSize = renderer.GetPropertyAsUnitValue(Property.FONT_SIZE).GetValue();
            float lineHeightValue;
            if (lineHeight == null || lineHeight.IsNormalValue() || lineHeight.GetValue() < 0) {
                lineHeightValue = DEFAULT_LINE_HEIGHT_COEFF * fontSize;
                float[] fontAscenderDescender = GetFontAscenderDescenderNormalized(renderer);
                float fontAscenderDescenderSum = fontAscenderDescender[0] - fontAscenderDescender[1];
                if (fontAscenderDescenderSum > lineHeightValue) {
                    lineHeightValue = fontAscenderDescenderSum;
                }
            }
            else {
                if (lineHeight.IsFixedValue()) {
                    lineHeightValue = lineHeight.GetValue();
                }
                else {
                    lineHeightValue = lineHeight.GetValue() * fontSize;
                }
            }
            return lineHeightValue;
        }
    }
}
