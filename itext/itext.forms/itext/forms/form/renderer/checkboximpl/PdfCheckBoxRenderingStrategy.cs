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
using iText.Commons.Datastructures;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.Forms.Logs;
using iText.Forms.Util;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>This class is used to draw a checkBox icon in PDF mode this is the default strategy for drawing a checkBox.
    ///     </summary>
    public sealed class PdfCheckBoxRenderingStrategy : ICheckBoxRenderingStrategy {
        public static readonly BiMap<CheckBoxType, String> ZAPFDINGBATS_CHECKBOX_MAPPING;

        static PdfCheckBoxRenderingStrategy() {
            ZAPFDINGBATS_CHECKBOX_MAPPING = new BiMap<CheckBoxType, String>();
            ZAPFDINGBATS_CHECKBOX_MAPPING.Put(CheckBoxType.CHECK, "4");
            ZAPFDINGBATS_CHECKBOX_MAPPING.Put(CheckBoxType.CIRCLE, "l");
            ZAPFDINGBATS_CHECKBOX_MAPPING.Put(CheckBoxType.CROSS, "8");
            ZAPFDINGBATS_CHECKBOX_MAPPING.Put(CheckBoxType.DIAMOND, "u");
            ZAPFDINGBATS_CHECKBOX_MAPPING.Put(CheckBoxType.SQUARE, "n");
            ZAPFDINGBATS_CHECKBOX_MAPPING.Put(CheckBoxType.STAR, "H");
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfCheckBoxRenderingStrategy"/>
        /// instance.
        /// </summary>
        public PdfCheckBoxRenderingStrategy() {
        }

        // empty constructor
        /// <summary><inheritDoc/></summary>
        public void DrawCheckBoxContent(DrawContext drawContext, CheckBoxRenderer checkBoxRenderer, Rectangle rectangle
            ) {
            if (!checkBoxRenderer.IsBoxChecked()) {
                return;
            }
            float borderWidth = CheckBoxRenderer.DEFAULT_BORDER_WIDTH;
            Border border = checkBoxRenderer.GetProperty<Border>(Property.BORDER);
            if (border != null) {
                borderWidth = border.GetWidth();
                rectangle.ApplyMargins(borderWidth, borderWidth, borderWidth, borderWidth, true);
            }
            PdfCanvas canvas = drawContext.GetCanvas();
            canvas.SaveState();
            canvas.SetFillColor(ColorConstants.BLACK);
            // matrix transformation to draw the checkbox in the right place
            // because we come here with relative and not absolute coordinates
            canvas.ConcatMatrix(1, 0, 0, 1, rectangle.GetLeft(), rectangle.GetBottom());
            CheckBoxType checkBoxType = checkBoxRenderer.GetCheckBoxType();
            if (checkBoxType == CheckBoxType.CROSS || checkBoxType == null) {
                float customBorderWidth = border == null ? 1 : borderWidth;
                DrawingUtil.DrawCross(canvas, rectangle.GetWidth(), rectangle.GetHeight(), customBorderWidth);
            }
            else {
                String text = ZAPFDINGBATS_CHECKBOX_MAPPING.GetByKey(checkBoxType);
                PdfFont fontContainingSymbols = LoadFontContainingSymbols();
                float fontSize = CalculateFontSize(checkBoxRenderer, fontContainingSymbols, text, rectangle, borderWidth);
                DrawZapfdingbatsIcon(fontContainingSymbols, text, fontSize, rectangle, canvas);
            }
            canvas.RestoreState();
        }

        private PdfFont LoadFontContainingSymbols() {
            try {
                return PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
        }

        private float CalculateFontSize(CheckBoxRenderer checkBoxRenderer, PdfFont fontContainingSymbols, String text
            , Rectangle rectangle, float borderWidth) {
            float fontSize = -1;
            if (checkBoxRenderer.HasProperty(Property.FONT_SIZE)) {
                fontSize = checkBoxRenderer.GetPropertyAsUnitValue(Property.FONT_SIZE).GetValue();
            }
            if (fontSize <= 0) {
                fontSize = FontSizeUtil.ApproximateFontSizeToFitSingleLine(fontContainingSymbols, new Rectangle(rectangle.
                    GetWidth(), rectangle.GetHeight()), text, 0.1F, borderWidth);
            }
            if (fontSize <= 0) {
                throw new PdfException(FormsLogMessageConstants.CHECKBOX_FONT_SIZE_IS_NOT_POSITIVE);
            }
            return fontSize;
        }

        private void DrawZapfdingbatsIcon(PdfFont fontContainingSymbols, String text, float fontSize, Rectangle rectangle
            , PdfCanvas canvas) {
            canvas.BeginText().SetFontAndSize(fontContainingSymbols, fontSize).ResetFillColorRgb().SetTextMatrix((rectangle
                .GetWidth() - fontContainingSymbols.GetWidth(text, fontSize)) / 2, (rectangle.GetHeight() - fontContainingSymbols
                .GetAscent(text, fontSize)) / 2).ShowText(text).EndText();
        }
    }
}
