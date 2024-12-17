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
using iText.Layout.Element;
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
        private readonly Text text = new Text("");

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

        [Obsolete]
        public virtual float[] GetRelativeTranslation() {
            return new float[] { 0.0f, 0.0f };
        }

        [Obsolete]
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
                float parentFontSize = ((AbstractSvgNodeRenderer)GetParent()).GetCurrentFontSize(context);
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
                text.SetText(this.attributesAndStyles.Get(SvgConstants.Attributes.TEXT_CONTENT));
                ((TextSvgBranchRenderer)GetParent()).ApplyFontProperties(text, context);
                ((TextSvgBranchRenderer)GetParent()).ApplyTextRenderingMode(text);
                ApplyTransform(context);
                ApplyGraphicsState(context);
                ((TextSvgBranchRenderer)GetParent()).AddTextChild(text, context);
            }
        }

        protected internal override bool CanElementFill() {
            return false;
        }

        private void ApplyTransform(SvgDrawContext context) {
            AffineTransform transform = context.GetRootTransform();
            text.SetHorizontalScaling((float)transform.GetScaleX());
            text.SetProperty(Property.VERTICAL_SCALING, transform.GetScaleY());
            text.SetProperty(Property.SKEW, new float[] { (float)transform.GetShearX(), (float)transform.GetShearY() }
                );
        }

        private void ApplyGraphicsState(SvgDrawContext context) {
            SvgTextProperties textProperties = context.GetSvgTextProperties();
            // TODO DEVSIX-8774 support stroke-opacity for text at layout level
            // TODO DEVSIX-8776 support dash-pattern in layout
            text.SetFontColor(textProperties.GetFillColor());
            text.SetStrokeWidth(textProperties.GetLineWidth());
            text.SetStrokeColor(textProperties.GetStrokeColor());
            text.SetOpacity(textProperties.GetFillOpacity());
        }
    }
}
