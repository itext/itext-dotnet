/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for
    /// <see cref="iText.Layout.Element.FootnoteAnchor"/>
    /// instance representing an anchor for a footnote.
    /// </summary>
    public class FootnoteAnchorRenderer : AbstractRenderer {
        private readonly IRenderer footnoteAnchor;

//\cond DO_NOT_DOCUMENT
        // Create and store footnote renderer once to save its layout result.
        internal FootnoteRenderer footnoteRenderer = null;
//\endcond

        /// <summary>
        /// Creates a
        /// <see cref="FootnoteAnchorRenderer"/>
        /// from its corresponding layout object.
        /// </summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.FootnoteAnchor"/>
        /// which this object should manage
        /// </param>
        public FootnoteAnchorRenderer(FootnoteAnchor modelElement)
            : base(modelElement) {
            footnoteAnchor = CreateFootnoteAnchorRenderer();
            if (footnoteAnchor != null) {
                footnoteAnchor.SetParent(this);
            }
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            if (this.footnoteRenderer == null) {
                Footnote footnote = ((FootnoteAnchor)this.modelElement).GetFootnote();
                ApplyFootnoteAnchor(footnote);
                this.footnoteRenderer = (FootnoteRenderer)footnote.CreateRendererSubTree().SetParent(this);
            }
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            Rectangle pageRectangle = this.GetPdfDocument().GetPage(pageNumber).GetPageSize();
            IRenderer parentRenderer = GetParent();
            while (parentRenderer != null) {
                if (parentRenderer is DocumentRenderer) {
                    DocumentRenderer documentRenderer = (DocumentRenderer)parentRenderer;
                    float leftMargin = (float)documentRenderer.GetPropertyAsFloat(Property.MARGIN_BOTTOM);
                    float rightMargin = (float)documentRenderer.GetPropertyAsFloat(Property.MARGIN_TOP);
                    pageRectangle.MoveRight(leftMargin).DecreaseWidth(leftMargin + rightMargin);
                    break;
                }
                parentRenderer = parentRenderer.GetParent();
            }
            this.footnoteRenderer.Layout(new LayoutContext(new LayoutArea(pageNumber, pageRectangle)));
            LayoutResult layoutResult = footnoteAnchor.Layout(layoutContext);
            this.occupiedArea = layoutResult.GetOccupiedArea();
            FootnotesCounterHandler.AddFootnoteAnchor(this);
            return layoutResult;
        }

        public override void Draw(DrawContext drawContext) {
            footnoteAnchor.Draw(drawContext);
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.FootnoteAnchorRenderer((FootnoteAnchor)modelElement);
        }

        private IRenderer CreateFootnoteAnchorRenderer() {
            IElement footnoteAnchorSymbol = ((FootnoteAnchor)this.modelElement).GetFootnoteAnchor();
            if (footnoteAnchorSymbol is Text) {
                return new TextRenderer((Text)footnoteAnchorSymbol);
            }
            else {
                if (footnoteAnchorSymbol is Image) {
                    return footnoteAnchorSymbol.GetRenderer();
                }
                else {
                    if (footnoteAnchorSymbol == null) {
                        return null;
                    }
                    else {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private void ApplyFootnoteAnchor(Footnote footnote) {
            if (!footnote.GetChildren().IsEmpty() && footnote.GetChildren()[0] is Paragraph) {
                Paragraph paragraph = (Paragraph)footnote.GetChildren()[0];
                InjectFootnoteAnchorIntoParagraph(paragraph);
            }
        }

        private void InjectFootnoteAnchorIntoParagraph(Paragraph paragraph) {
            // TODO DEVSIX-9981 Introduce anchor indent property to make it configurable.
            Div anchorIndent = new _Div_129().SetWidth(5F);
            IElement footnoteAnchorSymbol = ((FootnoteAnchor)this.modelElement).GetFootnoteAnchor();
            if (!paragraph.GetChildren().Contains(footnoteAnchorSymbol)) {
                bool isRtl = BaseDirection.RIGHT_TO_LEFT == this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION);
                if (!isRtl) {
                    paragraph.GetChildren().Add(0, anchorIndent);
                }
                paragraph.GetChildren().Add(0, footnoteAnchorSymbol);
                if (isRtl) {
                    paragraph.GetChildren().Add(0, anchorIndent);
                }
            }
        }

        private sealed class _Div_129 : Div {
            public _Div_129() {
            }

            public override AccessibilityProperties GetAccessibilityProperties() {
                if (this.tagProperties == null) {
                    this.tagProperties = new DefaultAccessibilityProperties(StandardRoles.ARTIFACT);
                }
                return this.tagProperties;
            }
        }
    }
}
