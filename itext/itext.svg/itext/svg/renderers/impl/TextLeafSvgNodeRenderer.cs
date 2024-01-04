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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for drawing text to a canvas.
    /// </summary>
    public class TextLeafSvgNodeRenderer : AbstractSvgNodeRenderer, ISvgTextNodeRenderer {
        public override ISvgNodeRenderer CreateDeepCopy() {
            TextLeafSvgNodeRenderer copy = new TextLeafSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public virtual float GetTextContentLength(float parentFontSize, PdfFont font) {
            float contentLength = 0.0f;
            if (font != null && this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes
                .TEXT_CONTENT)) {
                // Use own font-size declaration if it is present, parent's otherwise
                float fontSize = SvgTextUtil.ResolveFontSize(this, parentFontSize);
                String content = this.attributesAndStyles.Get(SvgConstants.Attributes.TEXT_CONTENT);
                contentLength = font.GetWidth(content, fontSize);
            }
            return contentLength;
        }

        public virtual float[] GetRelativeTranslation() {
            return new float[] { 0.0f, 0.0f };
        }

        public virtual bool ContainsRelativeMove() {
            return false;
        }

        //Leaf text elements do not contain any kind of transformation
        public virtual bool ContainsAbsolutePositionChange() {
            return false;
        }

        //Leaf text elements do not contain any kind of transformation
        public virtual float[][] GetAbsolutePositionChanges() {
            float[] part = new float[] { 0f };
            return new float[][] { part, part };
        }

        public virtual TextRectangle GetTextRectangle(SvgDrawContext context, Point basePoint) {
            if (GetParent() is TextSvgBranchRenderer && basePoint != null) {
                float parentFontSize = ((AbstractSvgNodeRenderer)GetParent()).GetCurrentFontSize();
                PdfFont parentFont = ((TextSvgBranchRenderer)GetParent()).GetFont();
                float textLength = GetTextContentLength(parentFontSize, parentFont);
                float[] fontAscenderDescenderFromMetrics = TextRenderer.CalculateAscenderDescender(parentFont, RenderingMode
                    .HTML_MODE);
                float fontAscender = FontProgram.ConvertTextSpaceToGlyphSpace(fontAscenderDescenderFromMetrics[0]) * parentFontSize;
                float fontDescender = FontProgram.ConvertTextSpaceToGlyphSpace(fontAscenderDescenderFromMetrics[1]) * parentFontSize;
                // TextRenderer#calculateAscenderDescender returns fontDescender as a negative value so we should subtract this value
                float textHeight = fontAscender - fontDescender;
                return new TextRectangle((float)basePoint.GetX(), (float)basePoint.GetY() - fontAscender, textLength, textHeight
                    , (float)basePoint.GetY());
            }
            else {
                return null;
            }
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.TEXT_CONTENT
                )) {
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                //TODO(DEVSIX-2507): Support for glyph by glyph handling of x, y and rotate
                if (context.GetPreviousElementTextMove() == null) {
                    currentCanvas.MoveText(context.GetTextMove()[0], context.GetTextMove()[1]);
                }
                else {
                    currentCanvas.MoveText(context.GetPreviousElementTextMove()[0], context.GetPreviousElementTextMove()[1]);
                }
                currentCanvas.ShowText(this.attributesAndStyles.Get(SvgConstants.Attributes.TEXT_CONTENT));
            }
        }

        protected internal override bool CanElementFill() {
            return false;
        }
    }
}
