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
using iText.Forms.Form.Renderer;
using iText.Forms.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>This class is used to draw a checkBox icon in HTML mode.</summary>
    public sealed class HtmlCheckBoxRenderingStrategy : ICheckBoxRenderingStrategy {
        /// <summary>
        /// Creates a new
        /// <see cref="HtmlCheckBoxRenderingStrategy"/>
        /// instance.
        /// </summary>
        public HtmlCheckBoxRenderingStrategy() {
        }

        // empty constructor
        /// <summary><inheritDoc/></summary>
        public void DrawCheckBoxContent(DrawContext drawContext, CheckBoxRenderer checkBoxRenderer, Rectangle rectangle
            ) {
            if (!checkBoxRenderer.IsBoxChecked()) {
                return;
            }
            PdfCanvas canvas = drawContext.GetCanvas();
            canvas.SaveState();
            canvas.SetFillColor(ColorConstants.BLACK);
            DrawingUtil.DrawPdfACheck(canvas, rectangle.GetWidth(), rectangle.GetHeight(), rectangle.GetLeft(), rectangle
                .GetBottom());
            canvas.RestoreState();
        }
    }
}
