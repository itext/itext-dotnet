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
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for columns.</summary>
    public class ColumnContainerRenderer : AbstractRenderer {
        /// <summary>Creates a DivRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.ColumnContainer"/>
        /// which this object should manage
        /// </param>
        public ColumnContainerRenderer(ColumnContainer modelElement)
            : base(modelElement) {
        }

        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.ColumnContainerRenderer), this.GetType
                ());
            return new iText.Layout.Renderer.ColumnContainerRenderer((ColumnContainer)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            ((ColumnContainer)this.GetModelElement()).CopyAllPropertiesToChildren();
            Rectangle initialBBox = layoutContext.GetArea().GetBBox();
            int columnCount = (int)this.GetProperty<int?>(Property.COLUMN_COUNT);
            float columnWidth = initialBBox.GetWidth() / columnCount;
            if (GetChildRenderers().IsEmpty() && !(GetChildRenderers()[0] is BlockRenderer)) {
                throw new InvalidOperationException("Invalid child renderers, it should be one, " + "not empty and be a block element"
                    );
            }
            BlockRenderer blockRenderer = (BlockRenderer)GetChildRenderers()[0];
            blockRenderer.SetParent(this);
            LayoutResult prelayoutResult = blockRenderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(columnWidth
                , INF))));
            if (prelayoutResult.GetStatus() != LayoutResult.FULL) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, blockRenderer);
            }
            // check if partial result is possible here
            blockRenderer = prelayoutResult.GetSplitRenderer() != null ? (BlockRenderer)prelayoutResult.GetSplitRenderer
                () : blockRenderer;
            float approximateHeight = prelayoutResult.GetOccupiedArea().GetBBox().GetHeight() / columnCount;
            approximateHeight = BalanceContentBetweenColumns(columnCount, blockRenderer, approximateHeight);
            LayoutArea area = layoutContext.GetArea().Clone();
            area.GetBBox().SetHeight(approximateHeight);
            area.GetBBox().SetY(initialBBox.GetY() + initialBBox.GetHeight() - area.GetBBox().GetHeight());
            IList<IRenderer> container = LayoutColumns(layoutContext, columnCount, columnWidth, approximateHeight);
            this.occupiedArea = area;
            this.SetChildRenderers(container);
            LayoutResult result = new LayoutResult(LayoutResult.FULL, area, this, null);
            // process some properties (keepTogether, margin collapsing), area breaks, adding height
            // Check what we do at the end of BlockRenderer
            return result;
        }

        private IList<IRenderer> LayoutColumns(LayoutContext preLayoutContext, int columnCount, float columnWidth, 
            float approximateHeight) {
            IList<IRenderer> container = new List<IRenderer>();
            Rectangle initialBBox = preLayoutContext.GetArea().GetBBox();
            IRenderer renderer = GetChildRenderers()[0];
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
                if (i == columnCount - 1 && renderer != null) {
                    throw new InvalidOperationException("The last layouting should be full");
                }
            }
            return container;
        }

        private static float BalanceContentBetweenColumns(int columnCount, IRenderer renderer, float approximateHeight
            ) {
            IList<IList<float>> columnsOverflowedHeights = CreateEmptyLists(columnCount);
            int currentColumn = 0;
            do {
                foreach (IList<float> overflowedHeights in columnsOverflowedHeights) {
                    if (!overflowedHeights.IsEmpty()) {
                        float min = (float)Enumerable.Min(overflowedHeights);
                        approximateHeight += min;
                        break;
                    }
                }
                columnsOverflowedHeights = CreateEmptyLists(columnCount);
                float[] columnsHeight = new float[columnCount];
                TraverseRender(renderer, approximateHeight, columnsHeight, columnsOverflowedHeights, currentColumn);
            }
            while (!columnsOverflowedHeights[columnCount - 1].IsEmpty());
            //TODO DEVSIX-7549: this is temporary solution, we should consider margins some other way
            approximateHeight += renderer.GetProperty<UnitValue>(Property.MARGIN_TOP).GetValue();
            approximateHeight += renderer.GetProperty<UnitValue>(Property.MARGIN_BOTTOM).GetValue();
            return approximateHeight;
        }

        private static IList<IList<float>> CreateEmptyLists(int columnCount) {
            IList<IList<float>> lists = new List<IList<float>>(columnCount);
            for (int i = 0; i < columnCount; i++) {
                lists.Add(new List<float>());
            }
            return lists;
        }

        private static void TraverseRender(IRenderer renderer, float approximateHeight, float[] columnsHeight, IList
            <IList<float>> columnsOverlowedHeights, int currentColumn) {
            float height = renderer.GetOccupiedArea().GetBBox().GetHeight();
            if (height > approximateHeight) {
                columnsOverlowedHeights[currentColumn].Add(columnsHeight[currentColumn] + height - approximateHeight);
                TraverseChildRenderers(renderer, approximateHeight, columnsHeight, columnsOverlowedHeights, currentColumn);
                return;
            }
            if (height + columnsHeight[currentColumn] > approximateHeight) {
                columnsOverlowedHeights[currentColumn].Add(columnsHeight[currentColumn] + height - approximateHeight);
                if (currentColumn == columnsHeight.Length - 1) {
                    return;
                }
                if (renderer.GetChildRenderers().IsEmpty()) {
                    TraverseRender(renderer, approximateHeight, columnsHeight, columnsOverlowedHeights, ++currentColumn);
                }
                else {
                    TraverseChildRenderers(renderer, approximateHeight, columnsHeight, columnsOverlowedHeights, currentColumn);
                }
            }
            else {
                columnsHeight[currentColumn] += height;
            }
        }

        private static void TraverseChildRenderers(IRenderer renderer, float approximateHeight, float[] columnsHeight
            , IList<IList<float>> columnsOverlowedHeights, int currentColumn) {
            if (renderer is ParagraphRenderer) {
                foreach (IRenderer child in ((ParagraphRenderer)renderer).GetLines()) {
                    TraverseRender(child, approximateHeight, columnsHeight, columnsOverlowedHeights, currentColumn);
                }
            }
            else {
                foreach (IRenderer child in renderer.GetChildRenderers()) {
                    TraverseRender(child, approximateHeight, columnsHeight, columnsOverlowedHeights, currentColumn);
                }
            }
        }
    }
}
