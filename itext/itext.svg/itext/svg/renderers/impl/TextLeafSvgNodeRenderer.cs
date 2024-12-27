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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
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

        [Obsolete]
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

        public virtual TextRectangle GetTextRectangle(SvgDrawContext context, Point startPoint) {
            if (GetParent() is TextSvgBranchRenderer && startPoint != null) {
                LineRenderer lineRenderer = LayoutText(context);
                if (lineRenderer == null) {
                    return null;
                }
                Rectangle textBBox = lineRenderer.GetOccupiedAreaBBox();
                float textLength = textBBox.GetWidth();
                float textHeight = textBBox.GetHeight();
                return new TextRectangle((float)startPoint.GetX(), (float)startPoint.GetY() - lineRenderer.GetMaxAscent(), 
                    textLength, textHeight, (float)startPoint.GetY());
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

        private LineRenderer LayoutText(SvgDrawContext context) {
            if (this.attributesAndStyles != null && this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.TEXT_CONTENT
                )) {
                // We need to keep all spaces after whitespace processing, so spaces are replaced with SpaceChar to avoid
                // trimming trailing spaces at layout level (they trimmed in the beginning of the paragraph by default,
                // but current text could be somewhere in the middle or end in the final result).
                text.SetText(this.attributesAndStyles.Get(SvgConstants.Attributes.TEXT_CONTENT).Replace(" ", "\u00a0"));
                ((TextSvgBranchRenderer)GetParent()).ApplyFontProperties(text, context);
                Paragraph paragraph = new Paragraph();
                paragraph.SetProperty(Property.FORCED_PLACEMENT, true);
                ParagraphRenderer paragraphRenderer = new ParagraphRenderer(paragraph);
                paragraph.SetNextRenderer(paragraphRenderer);
                paragraph.Add(text);
                PdfFormXObject xObject = new PdfFormXObject(new Rectangle(1e6f, 0));
                using (iText.Layout.Canvas canvas = new Canvas(new PdfCanvas(xObject, context.GetCurrentCanvas().GetDocument
                    ()), xObject.GetBBox().ToRectangle())) {
                    canvas.Add(paragraph);
                }
                return paragraphRenderer.GetLines()[0];
            }
            return null;
        }
    }
}
