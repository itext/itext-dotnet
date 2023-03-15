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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal class LineHeightHelper {
        public const int ASCENDER_INDEX = 0;

        public const int DESCENDER_INDEX = 1;

        public const int XHEIGHT_INDEX = 2;

        public const int LEADING_INDEX = 3;

        private const float DEFAULT_LINE_HEIGHT_COEFF = 1.15f;

        private LineHeightHelper() {
        }

        /// <summary>Get actual ascender, descender.</summary>
        /// <param name="renderer">the renderer to retrieve the ascender and descender from</param>
        /// <returns>an array containing in this order actual ascender</returns>
        internal static float[] GetActualAscenderDescender(AbstractRenderer renderer) {
            float[] result = GetActualFontInfo(renderer);
            return new float[] { result[0], result[1] };
        }

        /// <summary>Get actual ascender, descender, xHeight and leading.</summary>
        /// <param name="renderer">the renderer to retrieve the font info from</param>
        /// <returns>an array containing in this order actual ascender, descender, xHeight and leading</returns>
        internal static float[] GetActualFontInfo(AbstractRenderer renderer) {
            float ascender;
            float descender;
            float lineHeight = iText.Layout.Renderer.LineHeightHelper.CalculateLineHeight(renderer);
            float[] fontAscenderDescender = iText.Layout.Renderer.LineHeightHelper.GetFontAscenderDescenderNormalized(
                renderer);
            float leading = lineHeight - (fontAscenderDescender[0] - fontAscenderDescender[1]);
            ascender = fontAscenderDescender[0] + leading / 2F;
            descender = fontAscenderDescender[1] - leading / 2F;
            return new float[] { ascender, descender, fontAscenderDescender[2], leading };
        }

        internal static float[] GetFontAscenderDescenderNormalized(AbstractRenderer renderer) {
            PdfFont font = renderer.ResolveFirstPdfFont();
            float fontSize = renderer.GetPropertyAsUnitValue(Property.FONT_SIZE).GetValue();
            float[] fontAscenderDescenderFromMetrics = TextRenderer.CalculateAscenderDescender(font, RenderingMode.HTML_MODE
                );
            float fontAscender = FontProgram.ConvertTextSpaceToGlyphSpace(fontAscenderDescenderFromMetrics[0]) * fontSize;
            float fontDescender = FontProgram.ConvertTextSpaceToGlyphSpace(fontAscenderDescenderFromMetrics[1]) * fontSize;
            float xHeight = FontProgram.ConvertTextSpaceToGlyphSpace(font.GetFontProgram().GetFontMetrics().GetXHeight
                ()) * fontSize;
            return new float[] { fontAscender, fontDescender, xHeight };
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
