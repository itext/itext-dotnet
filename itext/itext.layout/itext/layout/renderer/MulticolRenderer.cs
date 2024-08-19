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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for columns.</summary>
    public class MulticolRenderer : AbstractRenderer {
        private const float ZERO_DELTA = 0.0001F;

        private MulticolRenderer.ColumnHeightCalculator heightCalculator;

        private BlockRenderer elementRenderer;

        private int columnCount;

        private float columnWidth;

        private float approximateHeight;

        private float? heightFromProperties;

        private float columnGap;

        private float containerWidth;

        private bool isFirstLayout = true;

        /// <summary>Creates a DivRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.MulticolContainer"/>
        /// which this object should manage
        /// </param>
        public MulticolRenderer(MulticolContainer modelElement)
            : base(modelElement) {
            SetHeightCalculator(new MulticolRenderer.LayoutInInfiniteHeightCalculator());
        }

        /// <summary>Sets the height calculator to be used by this renderer.</summary>
        /// <param name="heightCalculator">the height calculator to be used by this renderer.</param>
        public void SetHeightCalculator(MulticolRenderer.ColumnHeightCalculator heightCalculator) {
            this.heightCalculator = heightCalculator;
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            this.SetProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER, true);
            SetOverflowForAllChildren(this);
            Rectangle actualBBox = layoutContext.GetArea().GetBBox().Clone();
            float originalWidth = actualBBox.GetWidth();
            ContinuousContainer.SetupContinuousContainerIfNeeded(this);
            ApplyPaddings(actualBBox, false);
            ApplyBorderBox(actualBBox, false);
            ApplyMargins(actualBBox, false);
            ApplyWidth(actualBBox, originalWidth);
            containerWidth = actualBBox.GetWidth();
            CalculateColumnCountAndWidth(containerWidth);
            heightFromProperties = DetermineHeight(actualBBox);
            if (this.elementRenderer == null) {
                // initialize elementRenderer on first layout when first child represents renderer of element which
                // should be layouted in multicol, because on the next layouts this can have multiple children
                elementRenderer = GetElementsRenderer();
            }
            //It is necessary to set parent, because during relayout elementRenderer's parent gets cleaned up
            elementRenderer.SetParent(this);
            MulticolRenderer.MulticolLayoutResult layoutResult = LayoutInColumns(layoutContext, actualBBox);
            if (layoutResult.GetSplitRenderers().IsEmpty()) {
                foreach (IRenderer child in elementRenderer.GetChildRenderers()) {
                    child.SetParent(elementRenderer);
                }
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, layoutResult.GetCauseOfNothing());
            }
            else {
                if (layoutResult.GetOverflowRenderer() == null) {
                    ContinuousContainer continuousContainer = this.GetProperty<ContinuousContainer>(Property.TREAT_AS_CONTINUOUS_CONTAINER_RESULT
                        );
                    if (continuousContainer != null) {
                        continuousContainer.ReApplyProperties(this);
                    }
                    this.childRenderers.Clear();
                    AddAllChildRenderers(layoutResult.GetSplitRenderers());
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, true);
                    return new LayoutResult(LayoutResult.FULL, this.occupiedArea, this, null);
                }
                else {
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, false);
                    return new LayoutResult(LayoutResult.PARTIAL, this.occupiedArea, GridMulticolUtil.CreateSplitRenderer(layoutResult
                        .GetSplitRenderers(), this), CreateOverflowRenderer(layoutResult.GetOverflowRenderer()));
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.MulticolRenderer), this.GetType());
            return new iText.Layout.Renderer.MulticolRenderer((MulticolContainer)modelElement);
        }

        /// <summary>
        /// Performs the drawing operation for the border of this renderer, if
        /// defined by any of the
        /// <see cref="iText.Layout.Properties.Property.BORDER"/>
        /// values in either the layout
        /// element or this
        /// <see cref="IRenderer"/>
        /// itself.
        /// </summary>
        /// <param name="drawContext">the context (canvas, document, etc) of this drawing operation.</param>
        public override void DrawBorder(DrawContext drawContext) {
            base.DrawBorder(drawContext);
            Rectangle borderRect = ApplyMargins(occupiedArea.GetBBox().Clone(), GetMargins(), false);
            bool isAreaClipped = ClipBorderArea(drawContext, borderRect);
            Border gap = this.GetProperty<Border>(Property.COLUMN_GAP_BORDER);
            if (GetChildRenderers().IsEmpty() || gap == null || gap.GetWidth() <= ZERO_DELTA) {
                return;
            }
            DrawTaggedWhenNeeded(drawContext, (canvas) => {
                for (int i = 0; i < GetChildRenderers().Count - 1; ++i) {
                    Rectangle columnBBox = GetChildRenderers()[i].GetOccupiedArea().GetBBox();
                    Rectangle columnSpaceBBox = new Rectangle(columnBBox.GetX() + columnBBox.GetWidth(), columnBBox.GetY(), columnGap
                        , columnBBox.GetHeight());
                    float x1 = columnSpaceBBox.GetX() + columnSpaceBBox.GetWidth() / 2 + gap.GetWidth() / 2;
                    float y1 = columnSpaceBBox.GetY();
                    float y2 = columnSpaceBBox.GetY() + columnSpaceBBox.GetHeight();
                    gap.Draw(canvas, x1, y1, x1, y2, Border.Side.RIGHT, 0, 0);
                }
                if (isAreaClipped) {
                    drawContext.GetCanvas().RestoreState();
                }
            }
            );
        }

        /// <summary>Layouts multicol in the passed area.</summary>
        /// <param name="layoutContext">the layout context</param>
        /// <param name="actualBBox">the area to layout multicol on</param>
        /// <returns>
        /// the
        /// <see cref="MulticolLayoutResult"/>
        /// instance
        /// </returns>
        protected internal virtual MulticolRenderer.MulticolLayoutResult LayoutInColumns(LayoutContext layoutContext
            , Rectangle actualBBox) {
            LayoutResult inifiniteHeighOneColumnLayoutResult = elementRenderer.Layout(new LayoutContext(new LayoutArea
                (1, new Rectangle(columnWidth, INF))));
            if (inifiniteHeighOneColumnLayoutResult.GetStatus() != LayoutResult.FULL) {
                MulticolRenderer.MulticolLayoutResult result = new MulticolRenderer.MulticolLayoutResult();
                result.SetCauseOfNothing(inifiniteHeighOneColumnLayoutResult.GetCauseOfNothing());
                return result;
            }
            approximateHeight = inifiniteHeighOneColumnLayoutResult.GetOccupiedArea().GetBBox().GetHeight() / columnCount;
            return BalanceContentAndLayoutColumns(layoutContext, actualBBox);
        }

        /// <summary>Creates an overflow renderer.</summary>
        /// <param name="overflowedContentRenderer">an overflowed content renderer</param>
        /// <returns>
        /// a new
        /// <see cref="AbstractRenderer"/>
        /// instance
        /// </returns>
        protected internal virtual AbstractRenderer CreateOverflowRenderer(IRenderer overflowedContentRenderer) {
            iText.Layout.Renderer.MulticolRenderer overflowRenderer = (iText.Layout.Renderer.MulticolRenderer)GetNextRenderer
                ();
            overflowRenderer.isFirstLayout = false;
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            IList<IRenderer> children = new List<IRenderer>(1);
            children.Add(overflowedContentRenderer);
            overflowRenderer.SetChildRenderers(children);
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        private void SetOverflowForAllChildren(IRenderer renderer) {
            if (renderer == null || renderer is AreaBreakRenderer) {
                return;
            }
            renderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                SetOverflowForAllChildren(child);
            }
        }

        private void DrawTaggedWhenNeeded(DrawContext drawContext, Action<PdfCanvas> action) {
            PdfCanvas canvas = drawContext.GetCanvas();
            if (drawContext.IsTaggingEnabled()) {
                canvas.OpenTag(new CanvasArtifact());
            }
            action(canvas);
            if (drawContext.IsTaggingEnabled()) {
                canvas.CloseTag();
            }
        }

        private void ApplyWidth(Rectangle parentBbox, float originalWidth) {
            float? blockWidth = RetrieveWidth(originalWidth);
            if (blockWidth != null) {
                parentBbox.SetWidth((float)blockWidth);
            }
            else {
                float? minWidth = RetrieveMinWidth(parentBbox.GetWidth());
                if (minWidth != null && minWidth > parentBbox.GetWidth()) {
                    parentBbox.SetWidth((float)minWidth);
                }
            }
        }

        private float? DetermineHeight(Rectangle parentBBox) {
            float? height = RetrieveHeight();
            float? minHeight = RetrieveMinHeight();
            float? maxHeight = RetrieveMaxHeight();
            if (height == null || (minHeight != null && height < minHeight)) {
                if ((minHeight != null) && parentBBox.GetHeight() < minHeight) {
                    height = minHeight;
                }
            }
            if (height != null && maxHeight != null && height > maxHeight) {
                height = maxHeight;
            }
            return height;
        }

        private MulticolRenderer.MulticolLayoutResult BalanceContentAndLayoutColumns(LayoutContext prelayoutContext
            , Rectangle actualBbox) {
            float additionalHeightPerIteration;
            MulticolRenderer.MulticolLayoutResult result = new MulticolRenderer.MulticolLayoutResult();
            int counter = heightCalculator.MaxAmountOfRelayouts() + 1;
            float maxHeight = actualBbox.GetHeight();
            bool isLastLayout = false;
            while (counter-- > 0) {
                if (approximateHeight > maxHeight) {
                    isLastLayout = true;
                    approximateHeight = maxHeight;
                }
                // height calcultion
                float workingHeight = approximateHeight;
                if (heightFromProperties != null) {
                    workingHeight = Math.Min((float)heightFromProperties, (float)approximateHeight);
                }
                result = LayoutColumnsAndReturnOverflowRenderer(prelayoutContext, actualBbox, workingHeight);
                if (result.GetOverflowRenderer() == null || isLastLayout) {
                    ClearOverFlowRendererIfNeeded(result);
                    return result;
                }
                additionalHeightPerIteration = heightCalculator.GetAdditionalHeightOfEachColumn(this, result).Value;
                if (Math.Abs(additionalHeightPerIteration) <= ZERO_DELTA) {
                    ClearOverFlowRendererIfNeeded(result);
                    return result;
                }
                approximateHeight += additionalHeightPerIteration;
                ClearOverFlowRendererIfNeeded(result);
            }
            return result;
        }

        // Algorithm is based on pseudo algorithm from https://www.w3.org/TR/css-multicol-1/#propdef-column-span
        private void CalculateColumnCountAndWidth(float initialWidth) {
            int? columnCountTemp = (int?)this.GetProperty<int?>(Property.COLUMN_COUNT);
            float? columnWidthTemp = (float?)this.GetProperty<float?>(Property.COLUMN_WIDTH);
            float? columnGapTemp = (float?)this.GetProperty<float?>(Property.COLUMN_GAP);
            this.columnGap = columnGapTemp == null ? 0f : columnGapTemp.Value;
            if ((columnCountTemp == null && columnWidthTemp == null) || (columnCountTemp != null && columnCountTemp.Value
                 < 0) || (columnWidthTemp != null && columnWidthTemp.Value < 0) || (this.columnGap < 0)) {
                throw new InvalidOperationException(LayoutExceptionMessageConstant.INVALID_COLUMN_PROPERTIES);
            }
            if (columnWidthTemp == null) {
                this.columnCount = columnCountTemp.Value;
            }
            else {
                if (columnCountTemp == null) {
                    float columnWidthPlusGap = columnWidthTemp.Value + this.columnGap;
                    if (columnWidthPlusGap > ZERO_DELTA) {
                        this.columnCount = Math.Max(1, (int)Math.Floor((double)((initialWidth + this.columnGap) / columnWidthPlusGap
                            )));
                    }
                    else {
                        this.columnCount = 1;
                    }
                }
                else {
                    float columnWidthPlusGap = columnWidthTemp.Value + this.columnGap;
                    if (columnWidthPlusGap > ZERO_DELTA) {
                        this.columnCount = Math.Min((int)columnCountTemp, Math.Max(1, (int)Math.Floor((double)((initialWidth + this
                            .columnGap) / columnWidthPlusGap))));
                    }
                    else {
                        this.columnCount = 1;
                    }
                }
            }
            this.columnWidth = Math.Max(0.0f, ((initialWidth + this.columnGap) / this.columnCount - this.columnGap));
        }

        private void ClearOverFlowRendererIfNeeded(MulticolRenderer.MulticolLayoutResult result) {
            //When we have a height set on the element but the content doesn't fit in the given height
            //we don't want to render the overflow renderer as it would be rendered in the next area
            if (heightFromProperties != null && heightFromProperties < approximateHeight) {
                result.SetOverflowRenderer(null);
            }
        }

        private LayoutArea CalculateContainerOccupiedArea(LayoutContext layoutContext, bool isFull) {
            LayoutArea area = layoutContext.GetArea().Clone();
            Rectangle areaBBox = area.GetBBox();
            float totalContainerHeight = GridMulticolUtil.UpdateOccupiedHeight(approximateHeight, isFull, isFirstLayout
                , this);
            if (totalContainerHeight < areaBBox.GetHeight() || isFull) {
                areaBBox.SetHeight(totalContainerHeight);
                float? height = DetermineHeight(areaBBox);
                if (height != null) {
                    height = GridMulticolUtil.UpdateOccupiedHeight((float)height, isFull, isFirstLayout, this);
                    areaBBox.SetHeight((float)height);
                }
            }
            Rectangle initialBBox = layoutContext.GetArea().GetBBox();
            areaBBox.SetY(initialBBox.GetY() + initialBBox.GetHeight() - areaBBox.GetHeight());
            float totalContainerWidth = GridMulticolUtil.UpdateOccupiedWidth(containerWidth, this);
            areaBBox.SetWidth(totalContainerWidth);
            return area;
        }

        private BlockRenderer GetElementsRenderer() {
            if (!(GetChildRenderers().Count == 1 && GetChildRenderers()[0] is BlockRenderer)) {
                throw new InvalidOperationException("Invalid child renderers, it should be one and be a block element");
            }
            return (BlockRenderer)GetChildRenderers()[0];
        }

        private MulticolRenderer.MulticolLayoutResult LayoutColumnsAndReturnOverflowRenderer(LayoutContext preLayoutContext
            , Rectangle actualBBox, float workingHeight) {
            MulticolRenderer.MulticolLayoutResult result = new MulticolRenderer.MulticolLayoutResult();
            IRenderer renderer = elementRenderer;
            for (int i = 0; i < columnCount && renderer != null; i++) {
                LayoutArea tempArea = preLayoutContext.GetArea().Clone();
                tempArea.GetBBox().SetWidth(columnWidth);
                tempArea.GetBBox().SetHeight(workingHeight);
                tempArea.GetBBox().SetX(actualBBox.GetX() + (columnWidth + columnGap) * i);
                tempArea.GetBBox().SetY(actualBBox.GetY() + actualBBox.GetHeight() - tempArea.GetBBox().GetHeight());
                LayoutContext columnContext = new LayoutContext(tempArea, preLayoutContext.GetMarginsCollapseInfo(), preLayoutContext
                    .GetFloatRendererAreas(), preLayoutContext.IsClippedHeight());
                renderer.SetProperty(Property.COLLAPSING_MARGINS, false);
                LayoutResult tempResultColumn = renderer.Layout(columnContext);
                if (tempResultColumn.GetStatus() == LayoutResult.NOTHING) {
                    result.SetOverflowRenderer((AbstractRenderer)renderer);
                    result.SetCauseOfNothing(tempResultColumn.GetCauseOfNothing());
                    return result;
                }
                if (tempResultColumn.GetSplitRenderer() == null) {
                    result.GetSplitRenderers().Add(renderer);
                }
                else {
                    result.GetSplitRenderers().Add(tempResultColumn.GetSplitRenderer());
                }
                renderer = tempResultColumn.GetOverflowRenderer();
            }
            result.SetOverflowRenderer((AbstractRenderer)renderer);
            return result;
        }

        /// <summary>Interface which used for additional height calculation</summary>
        public interface ColumnHeightCalculator {
            /// <summary>
            /// Calculate height, by which current height of given
            /// <c>MulticolRenderer</c>
            /// should be increased so
            /// <c>MulticolLayoutResult#getOverflowRenderer</c>
            /// could be lauded
            /// </summary>
            /// <param name="renderer">multicol renderer for which height needs to be increased</param>
            /// <param name="result">
            /// result of one iteration of
            /// <c>MulticolRenderer</c>
            /// layouting
            /// </param>
            /// <returns>height by which current height of given multicol renderer should be increased</returns>
            float? GetAdditionalHeightOfEachColumn(MulticolRenderer renderer, MulticolRenderer.MulticolLayoutResult result
                );

            int MaxAmountOfRelayouts();
        }

        /// <summary>
        /// Represents result of one iteration of MulticolRenderer layouting
        /// It contains split renderers which were lauded on a given height and overflow renderer
        /// for which height should be increased, so it can be lauded.
        /// </summary>
        public class MulticolLayoutResult {
            private IList<IRenderer> splitRenderers = new List<IRenderer>();

            private AbstractRenderer overflowRenderer;

            private IRenderer causeOfNothing;

            public virtual IList<IRenderer> GetSplitRenderers() {
                return splitRenderers;
            }

            public virtual AbstractRenderer GetOverflowRenderer() {
                return overflowRenderer;
            }

            public virtual void SetOverflowRenderer(AbstractRenderer overflowRenderer) {
                this.overflowRenderer = overflowRenderer;
            }

            public virtual IRenderer GetCauseOfNothing() {
                return causeOfNothing;
            }

            public virtual void SetCauseOfNothing(IRenderer causeOfNothing) {
                this.causeOfNothing = causeOfNothing;
            }
        }

        public class LayoutInInfiniteHeightCalculator : MulticolRenderer.ColumnHeightCalculator {
            protected internal int maxRelayoutCount = 4;

            private float? height = null;

            public virtual float? GetAdditionalHeightOfEachColumn(MulticolRenderer renderer, MulticolRenderer.MulticolLayoutResult
                 result) {
                if (height != null) {
                    return height;
                }
                if (result.GetOverflowRenderer() == null) {
                    return 0.0f;
                }
                LayoutResult overflowResult = result.GetOverflowRenderer().Layout(new LayoutContext(new LayoutArea(1, new 
                    Rectangle(renderer.columnWidth, INF))));
                float overflowHeight = overflowResult.GetOccupiedArea().GetBBox().GetHeight();
                if (result.GetSplitRenderers().IsEmpty()) {
                    // In case when first child of content bigger or wider than column and in first layout NOTHING is
                    // returned. In that case content again layouted in infinity area without keeping in mind that some
                    // approximateHeight already exist.
                    overflowHeight -= renderer.approximateHeight;
                }
                height = overflowHeight / maxRelayoutCount;
                return height;
            }

            /// <returns>maximum amount of relayouts which can be done by this height enhancer</returns>
            public virtual int MaxAmountOfRelayouts() {
                return maxRelayoutCount;
            }
        }
    }
}
