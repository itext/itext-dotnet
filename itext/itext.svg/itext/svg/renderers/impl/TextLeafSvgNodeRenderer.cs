/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
