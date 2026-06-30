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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for
    /// <see cref="iText.Layout.Properties.Margins.FootnoteAnchor"/>
    /// instance representing an anchor for a footnote.
    /// </summary>
    public class FootnoteAnchorRenderer : AbstractRenderer {
        private IRenderer footnoteAnchor;

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
        /// <see cref="iText.Layout.Properties.Margins.FootnoteAnchor"/>
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
                this.footnoteRenderer = (FootnoteRenderer)footnote.CreateRendererSubTree().SetParent(this);
                LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper != null) {
                    taggingHelper.AddKidsHint(this, JavaCollectionsUtil.SingletonList<IRenderer>(footnoteRenderer));
                    taggingHelper.AddKidsHint(this, JavaCollectionsUtil.SingletonList<IRenderer>(footnoteAnchor));
                    LayoutTaggingHelper.AddTreeHints(taggingHelper, footnoteAnchor);
                }
            }
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            Rectangle pageRectangle = this.GetPdfDocument().GetPage(pageNumber).GetPageSize();
            IRenderer parentRenderer = GetParent();
            while (parentRenderer != null) {
                if (parentRenderer is DocumentRenderer) {
                    DocumentRenderer documentRenderer = (DocumentRenderer)parentRenderer;
                    FootnotesUtil.SetParentForFootnoteRenderer(this.footnoteRenderer, documentRenderer);
                    float leftMargin = (float)documentRenderer.GetPropertyAsFloat(Property.MARGIN_LEFT);
                    float rightMargin = (float)documentRenderer.GetPropertyAsFloat(Property.MARGIN_RIGHT);
                    pageRectangle.MoveRight(leftMargin).DecreaseWidth(leftMargin + rightMargin);
                    break;
                }
                parentRenderer = parentRenderer.GetParent();
            }
            this.footnoteRenderer.Layout(new LayoutContext(new LayoutArea(pageNumber, pageRectangle)));
            // TODO DEVSIX-10023 Process partial result. Take it into account in line renderer
            //  and in case of table header/footer or fixed width.
            LayoutResult layoutResult = footnoteAnchor.Layout(layoutContext);
            this.occupiedArea = layoutResult.GetOccupiedArea();
            FootnotesCounterHandler.AddFootnoteAnchor(this);
            if (LayoutResult.NOTHING == layoutResult.GetStatus()) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, layoutResult.GetOverflowRenderer(), this);
            }
            return layoutResult;
        }

        public override void Draw(DrawContext drawContext) {
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            FootnoteTaggingHelper.RepairFootnoteAnchorTagIfNeeded(this, taggingHelper);
            bool isTagged = drawContext.IsTaggingEnabled();
            if (isTagged) {
                taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper == null) {
                    isTagged = false;
                }
                else {
                    TagTreePointer tagPointer = taggingHelper.UseAutoTaggingPointerAndRememberItsPosition(this);
                    taggingHelper.CreateTag(this, tagPointer);
                }
            }
            footnoteAnchor.Draw(drawContext);
            if (isTagged) {
                if (isLastRendererForModelElement) {
                    taggingHelper.FinishTaggingHint(this);
                }
                taggingHelper.RestoreAutoTaggingPointerPosition(this);
            }
            flushed = true;
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.FootnoteAnchorRenderer((FootnoteAnchor)modelElement);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual iText.Layout.Renderer.FootnoteAnchorRenderer AddSymbolRenderer(IRenderer footnoteNumberingSymbolRenderer
            ) {
            this.footnoteAnchor = footnoteNumberingSymbolRenderer.SetParent(this);
            SetFootnoteAnchor(((FootnoteAnchor)this.modelElement), footnoteNumberingSymbolRenderer.GetModelElement());
            return this;
        }
//\endcond

        private static void SetFootnoteAnchor(FootnoteAnchor footnoteAnchor, IPropertyContainer element) {
            if (element is Image) {
                footnoteAnchor.SetFootnoteAnchor((Image)element);
            }
            if (element is Text) {
                footnoteAnchor.SetFootnoteAnchor((Text)element);
            }
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
    }
}
