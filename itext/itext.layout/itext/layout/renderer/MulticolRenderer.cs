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
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for columns.</summary>
    public class MulticolRenderer : AbstractRenderer {
        private const int MAX_RELAYOUT_COUNT = 4;

        private const float ZERO_DELTA = 0.0001f;

        private BlockRenderer elementRenderer;

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

        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.MulticolRenderer), this.GetType());
            return new iText.Layout.Renderer.MulticolRenderer((MulticolContainer)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            ((MulticolContainer)this.GetModelElement()).CopyAllPropertiesToChildren();
            this.SetProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER, true);
            Rectangle initialBBox = layoutContext.GetArea().GetBBox();
            columnCount = (int)this.GetProperty<int?>(Property.COLUMN_COUNT);
            columnWidth = initialBBox.GetWidth() / columnCount;
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
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, elementRenderer);
            }
            approximateHeight = prelayoutResult.GetOccupiedArea().GetBBox().GetHeight() / columnCount;
            IList<IRenderer> container = BalanceContentAndLayoutColumns(layoutContext);
            this.occupiedArea = CalculateContainerOccupiedArea(layoutContext);
            this.SetChildRenderers(container);
            LayoutResult result = new LayoutResult(LayoutResult.FULL, this.occupiedArea, this, null);
            // process some properties (keepTogether, margin collapsing), area breaks, adding height
            // Check what we do at the end of BlockRenderer
            return result;
        }

        private IList<IRenderer> BalanceContentAndLayoutColumns(LayoutContext prelayoutContext) {
            float? additionalHeightPerIteration = null;
            IList<IRenderer> container = new List<IRenderer>();
            int counter = MAX_RELAYOUT_COUNT;
            while (counter-- > 0) {
                IRenderer overflowRenderer = LayoutColumnsAndReturnOverflowRenderer(prelayoutContext, container);
                if (overflowRenderer == null) {
                    return container;
                }
                if (additionalHeightPerIteration == null) {
                    LayoutResult overflowResult = overflowRenderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(columnWidth
                        , INF))));
                    additionalHeightPerIteration = overflowResult.GetOccupiedArea().GetBBox().GetHeight() / MAX_RELAYOUT_COUNT;
                }
                if (Math.Abs(additionalHeightPerIteration.Value) <= ZERO_DELTA) {
                    return container;
                }
                approximateHeight += additionalHeightPerIteration.Value;
            }
            return container;
        }

        private LayoutArea CalculateContainerOccupiedArea(LayoutContext layoutContext) {
            LayoutArea area = layoutContext.GetArea().Clone();
            area.GetBBox().SetHeight(approximateHeight);
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

        private IRenderer LayoutColumnsAndReturnOverflowRenderer(LayoutContext preLayoutContext, IList<IRenderer> 
            container) {
            container.Clear();
            Rectangle initialBBox = preLayoutContext.GetArea().GetBBox();
            IRenderer renderer = elementRenderer;
            for (int i = 0; i < columnCount && renderer != null; i++) {
                LayoutArea tempArea = preLayoutContext.GetArea().Clone();
                tempArea.GetBBox().SetWidth(columnWidth);
                tempArea.GetBBox().SetHeight(approximateHeight);
                tempArea.GetBBox().SetX(initialBBox.GetX() + columnWidth * i);
                tempArea.GetBBox().SetY(initialBBox.GetY() + initialBBox.GetHeight() - tempArea.GetBBox().GetHeight());
                LayoutContext columnContext = new LayoutContext(tempArea, preLayoutContext.GetMarginsCollapseInfo(), preLayoutContext
                    .GetFloatRendererAreas(), preLayoutContext.IsClippedHeight());
                LayoutResult tempResultColumn = renderer.Layout(columnContext);
                if (tempResultColumn.GetSplitRenderer() == null) {
                    container.Add(renderer);
                }
                else {
                    container.Add(tempResultColumn.GetSplitRenderer());
                }
                renderer = tempResultColumn.GetOverflowRenderer();
            }
            return renderer;
        }
    }
}
