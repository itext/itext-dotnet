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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
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
//\cond DO_NOT_DOCUMENT
        internal IRenderer footnoteAnchor;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float yPos = float.NaN;
//\endcond

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

        /// <summary><inheritDoc/></summary>
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
            if (parentRenderer is LineRenderer) {
                this.yPos = ((LineRenderer)parentRenderer).occupiedArea.GetBBox().GetTop();
            }
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
            LayoutResult layoutResult = footnoteAnchor.Layout(layoutContext);
            this.occupiedArea = layoutResult.GetOccupiedArea();
            if (LayoutResult.NOTHING == layoutResult.GetStatus()) {
                layoutResult.SetOverflowRenderer(this);
                layoutResult.SetCauseOfNothing(this);
            }
            else {
                if (float.IsNaN(this.yPos)) {
                    this.yPos = this.occupiedArea.GetBBox().GetTop();
                }
                FootnotesCounterHandler.AddFootnoteAnchor(this);
            }
            if (layoutResult.GetSplitRenderer() != null) {
                iText.Layout.Renderer.FootnoteAnchorRenderer splitRenderer = CreateSplitRenderer(layoutResult);
                layoutResult.SetSplitRenderer(splitRenderer);
            }
            return layoutResult;
        }

        /// <summary><inheritDoc/></summary>
        public override void Move(float dxRight, float dyUp) {
            footnoteAnchor.Move(dxRight, dyUp);
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            FootnoteTaggingHelper.RepairFootnoteAnchorTagIfNeeded(this, taggingHelper);
            bool isTagged = drawContext.IsTaggingEnabled();
            bool tagCreated = false;
            if (isTagged) {
                taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper == null) {
                    isTagged = false;
                }
                else {
                    TagTreePointer tagPointer = taggingHelper.UseAutoTaggingPointerAndRememberItsPosition(this);
                    tagCreated = taggingHelper.CreateTag(this, tagPointer);
                }
            }
            if (tagCreated || !isTagged) {
                // We only don't set up links if tagging is enabled, but tag was not created,
                // meaning this content is in fact an artifact. This happens because links contain annotations,
                // and annotations need to be tagged. But since this content is an artifact, we can't properly tag it.
                SetUpLinks(drawContext);
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

        /// <summary><inheritDoc/></summary>
        public override MinMaxWidth GetMinMaxWidth() {
            return GetMinMaxWidth(null);
        }

        /// <summary><inheritDoc/></summary>
        public override MinMaxWidth GetMinMaxWidth(float? parentBoxWidth) {
            childRenderers.Clear();
            childRenderers.Add(footnoteAnchor);
            MinMaxWidth res = base.GetMinMaxWidth(parentBoxWidth);
            childRenderers.Clear();
            return res;
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.FootnoteAnchorRenderer((FootnoteAnchor)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override float? GetFirstYLineRecursively() {
            childRenderers.Clear();
            childRenderers.Add(footnoteAnchor);
            float? res = base.GetFirstYLineRecursively();
            childRenderers.Clear();
            return res;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override float? GetLastYLineRecursively() {
            childRenderers.Clear();
            childRenderers.Add(footnoteAnchor);
            float? res = base.GetLastYLineRecursively();
            childRenderers.Clear();
            return res;
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

        private static void SetUpLinks(IPropertyContainer from, IPropertyContainer to, String name, String altDescription
            , PdfDocument document) {
            int amountOfNamedDestinations = 0;
            if (document.GetCatalog().GetNameTree(PdfName.Dests).GetNames() != null) {
                amountOfNamedDestinations = document.GetCatalog().GetNameTree(PdfName.Dests).GetNames().Count;
            }
            PdfLinkAnnotation footnoteAnnotation = (PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(0, 0)).SetAction
                (PdfAction.CreateGoTo(name + amountOfNamedDestinations)).SetFlags(PdfAnnotation.PRINT);
            footnoteAnnotation.SetBorder(new PdfArray(new float[] { 0, 0, 0 }));
            footnoteAnnotation.SetContents(altDescription);
            from.SetProperty(Property.LINK_ANNOTATION, footnoteAnnotation);
            ICollection<Object> footnoteDestinations = to.GetProperty<ICollection<Object>>(Property.DESTINATION);
            if (footnoteDestinations == null) {
                footnoteDestinations = new HashSet<Object>();
            }
            footnoteDestinations.Add(new Tuple2<String, PdfDictionary>(name + amountOfNamedDestinations, footnoteAnnotation
                .GetAction()));
            to.SetProperty(Property.DESTINATION, footnoteDestinations);
        }

        private void SetUpLinks(DrawContext drawContext) {
            IPropertyContainer footnoteLabel = FootnotesUtil.GetInjectedFootnoteAnchor((Footnote)footnoteRenderer.GetModelElement
                ());
            if (footnoteLabel == null) {
                // Footnote label is not supposed to be null. If it is, something is broken, and we don't add links.
                return;
            }
            // We don't want to override existing link annotations, if any.
            if (footnoteAnchor.GetProperty<PdfLinkAnnotation>(Property.LINK_ANNOTATION) == null && footnoteLabel.GetProperty
                <PdfLinkAnnotation>(Property.LINK_ANNOTATION) == null) {
                SetUpLinks(footnoteAnchor, footnoteLabel, "footnoteAnchor", "Go to footnote.", drawContext.GetDocument());
                SetUpLinks(footnoteLabel, footnoteAnchor, "footnoteContent", "Go to footnote anchor.", drawContext.GetDocument
                    ());
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

        private iText.Layout.Renderer.FootnoteAnchorRenderer CreateSplitRenderer(LayoutResult layoutResult) {
            iText.Layout.Renderer.FootnoteAnchorRenderer splitRenderer = (iText.Layout.Renderer.FootnoteAnchorRenderer
                )GetNextRenderer();
            splitRenderer.occupiedArea = occupiedArea.Clone();
            splitRenderer.parent = parent;
            splitRenderer.footnoteRenderer = footnoteRenderer;
            splitRenderer.AddAllProperties(GetOwnProperties());
            splitRenderer.footnoteAnchor = layoutResult.GetSplitRenderer().SetParent(splitRenderer);
            return splitRenderer;
        }
    }
}
