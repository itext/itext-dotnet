/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for columns.</summary>
    public class MulticolRenderer : AbstractRenderer {
        private const int MAX_RELAYOUT_COUNT = 4;

        private const float ZERO_DELTA = 0.0001F;

        private BlockRenderer elementRenderer;

        private readonly MulticolRenderer.HeightEnhancer heightCalculator = new MulticolRenderer.HeightEnhancer();

        private int columnCount;

        private float columnWidth;

        private float approximateHeight;

        /// <summary>Creates a DivRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.MulticolContainer"/>
        /// which this object should manage
        /// </param>
        public MulticolRenderer(MulticolContainer modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            this.SetProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER, true);
            Rectangle actualBBox = layoutContext.GetArea().GetBBox().Clone();
            ApplyPaddings(actualBBox, false);
            ApplyBorderBox(actualBBox, false);
            ApplyMargins(actualBBox, false);
            columnCount = (int)this.GetProperty<int?>(Property.COLUMN_COUNT);
            columnWidth = actualBBox.GetWidth() / columnCount;
            if (this.elementRenderer == null) {
                // initialize elementRenderer on first layout when first child represents renderer of element which
                // should be layouted in multicol, because on the next layouts this can have multiple children
                elementRenderer = GetElementsRenderer();
            }
            //It is necessary to set parent, because during relayout elementRenderer's parent gets cleaned up
            elementRenderer.SetParent(this);
            LayoutResult prelayoutResult = elementRenderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(columnWidth
                , INF))));
            if (prelayoutResult.GetStatus() != LayoutResult.FULL) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, prelayoutResult.GetCauseOfNothing());
            }
            approximateHeight = prelayoutResult.GetOccupiedArea().GetBBox().GetHeight() / columnCount;
            MulticolRenderer.MulticolLayoutResult layoutResult = BalanceContentAndLayoutColumns(layoutContext, actualBBox
                );
            if (layoutResult.GetSplitRenderers().IsEmpty()) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, layoutResult.GetCauseOfNothing());
            }
            else {
                if (layoutResult.GetOverflowRenderer() == null) {
                    this.childRenderers.Clear();
                    AddAllChildRenderers(layoutResult.GetSplitRenderers());
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, true);
                    return new LayoutResult(LayoutResult.FULL, this.occupiedArea, this, null);
                }
                else {
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, false);
                    return new LayoutResult(LayoutResult.PARTIAL, this.occupiedArea, CreateSplitRenderer(layoutResult.GetSplitRenderers
                        ()), CreateOverflowRenderer(layoutResult.GetOverflowRenderer()));
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.MulticolRenderer), this.GetType());
            return new iText.Layout.Renderer.MulticolRenderer((MulticolContainer)modelElement);
        }

        /// <summary>Creates a split renderer.</summary>
        /// <param name="children">children of the split renderer</param>
        /// <returns>
        /// a new
        /// <see cref="AbstractRenderer"/>
        /// instance
        /// </returns>
        protected internal virtual AbstractRenderer CreateSplitRenderer(IList<IRenderer> children) {
            AbstractRenderer splitRenderer = (AbstractRenderer)GetNextRenderer();
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.SetChildRenderers(children);
            splitRenderer.AddAllProperties(GetOwnProperties());
            ContinuousContainer.SetupContinuousContainerIfNeeded(splitRenderer);
            return splitRenderer;
        }

        /// <summary>Creates an overflow renderer.</summary>
        /// <param name="overflowedContentRenderer">an overflowed content renderer</param>
        /// <returns>
        /// a new
        /// <see cref="AbstractRenderer"/>
        /// instance
        /// </returns>
        protected internal virtual AbstractRenderer CreateOverflowRenderer(IRenderer overflowedContentRenderer) {
            AbstractRenderer overflowRenderer = (AbstractRenderer)GetNextRenderer();
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            IList<IRenderer> children = new List<IRenderer>(1);
            children.Add(overflowedContentRenderer);
            overflowRenderer.SetChildRenderers(children);
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        private float SafelyRetrieveFloatProperty(int property) {
            Object value = this.GetProperty<Object>(property);
            if (value is UnitValue) {
                return ((UnitValue)value).GetValue();
            }
            if (value is Border) {
                return ((Border)value).GetWidth();
            }
            return 0F;
        }

        private MulticolRenderer.MulticolLayoutResult BalanceContentAndLayoutColumns(LayoutContext prelayoutContext
            , Rectangle actualBbox) {
            float additionalHeightPerIteration;
            MulticolRenderer.MulticolLayoutResult result = new MulticolRenderer.MulticolLayoutResult();
            int counter = MAX_RELAYOUT_COUNT + 1;
            float maxHeight = actualBbox.GetHeight();
            bool isLastLayout = false;
            while (counter-- > 0) {
                if (approximateHeight > maxHeight) {
                    isLastLayout = true;
                    approximateHeight = maxHeight;
                }
                result = LayoutColumnsAndReturnOverflowRenderer(prelayoutContext, actualBbox);
                if (result.GetOverflowRenderer() == null || isLastLayout) {
                    return result;
                }
                additionalHeightPerIteration = heightCalculator.Apply(this, result).Value;
                if (Math.Abs(additionalHeightPerIteration) <= ZERO_DELTA) {
                    return result;
                }
                approximateHeight += additionalHeightPerIteration;
            }
            return result;
        }

        private LayoutArea CalculateContainerOccupiedArea(LayoutContext layoutContext, bool isFull) {
            LayoutArea area = layoutContext.GetArea().Clone();
            float totalHeight = approximateHeight;
            if (isFull) {
                totalHeight += SafelyRetrieveFloatProperty(Property.PADDING_BOTTOM);
                totalHeight += SafelyRetrieveFloatProperty(Property.MARGIN_BOTTOM);
                totalHeight += SafelyRetrieveFloatProperty(Property.BORDER_BOTTOM);
            }
            totalHeight += SafelyRetrieveFloatProperty(Property.PADDING_TOP);
            totalHeight += SafelyRetrieveFloatProperty(Property.MARGIN_TOP);
            totalHeight += SafelyRetrieveFloatProperty(Property.BORDER_TOP);
            float TOP_AND_BOTTOM = isFull ? 2 : 1;
            totalHeight += SafelyRetrieveFloatProperty(Property.BORDER) * TOP_AND_BOTTOM;
            area.GetBBox().SetHeight(totalHeight);
            Rectangle initialBBox = layoutContext.GetArea().GetBBox();
            area.GetBBox().SetY(initialBBox.GetY() + initialBBox.GetHeight() - area.GetBBox().GetHeight());
            return area;
        }

        private BlockRenderer GetElementsRenderer() {
            if (!(GetChildRenderers().Count == 1 && GetChildRenderers()[0] is BlockRenderer)) {
                throw new InvalidOperationException("Invalid child renderers, it should be one and be a block element");
            }
            return (BlockRenderer)GetChildRenderers()[0];
        }

        private MulticolRenderer.MulticolLayoutResult LayoutColumnsAndReturnOverflowRenderer(LayoutContext preLayoutContext
            , Rectangle actualBBox) {
            MulticolRenderer.MulticolLayoutResult result = new MulticolRenderer.MulticolLayoutResult();
            IRenderer renderer = elementRenderer;
            for (int i = 0; i < columnCount && renderer != null; i++) {
                LayoutArea tempArea = preLayoutContext.GetArea().Clone();
                tempArea.GetBBox().SetWidth(columnWidth);
                tempArea.GetBBox().SetHeight(approximateHeight);
                tempArea.GetBBox().SetX(actualBBox.GetX() + columnWidth * i);
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

        /// <summary>
        /// Represents result of one iteration of MulticolRenderer layouting
        /// It contains split renderers which were lauded on a given height and overflow renderer
        /// for which height should be increased, so it can be lauded.
        /// </summary>
        private class MulticolLayoutResult {
            private IList<IRenderer> splitRenderers = new List<IRenderer>();

            private AbstractRenderer overflowRenderer;

            private IRenderer causeOfNothing;

            public virtual IList<IRenderer> GetSplitRenderers() {
                return splitRenderers;
            }

            public virtual AbstractRenderer GetOverflowRenderer() {
                return overflowRenderer;
            }

            public virtual IRenderer GetCauseOfNothing() {
                return causeOfNothing;
            }

            public virtual void SetOverflowRenderer(AbstractRenderer overflowRenderer) {
                this.overflowRenderer = overflowRenderer;
            }

            public virtual void SetCauseOfNothing(IRenderer causeOfNothing) {
                this.causeOfNothing = causeOfNothing;
            }
        }

        /// <summary>Class which used for additional height calculation</summary>
        private class HeightEnhancer {
            private float? height = null;

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
            public virtual float? Apply(MulticolRenderer renderer, MulticolRenderer.MulticolLayoutResult result) {
                if (height != null) {
                    return height;
                }
                if (result.GetOverflowRenderer() == null) {
                    return 0.0f;
                }
                LayoutResult overflowResult = result.GetOverflowRenderer().Layout(new LayoutContext(new LayoutArea(1, new 
                    Rectangle(renderer.columnWidth, INF))));
                height = overflowResult.GetOccupiedArea().GetBBox().GetHeight() / MAX_RELAYOUT_COUNT;
                return height;
            }
        }
    }
}
