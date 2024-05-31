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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for a grid.</summary>
    public class GridContainerRenderer : DivRenderer {
        private bool isFirstLayout = true;

        /// <summary>Creates a Grid renderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.GridContainer"/>
        /// which this object should manage
        /// </param>
        public GridContainerRenderer(GridContainer modelElement)
            : base(modelElement) {
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.GridContainerRenderer), this.GetType
                ());
            return new iText.Layout.Renderer.GridContainerRenderer((GridContainer)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            //TODO DEVSIX-8331 enable continuous container, right now its not working properly out of the box because
            // we don't need to enable it for every element in a grid, probably only to those which get
            // split by a page
            //this.setProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER, Boolean.TRUE);
            Rectangle actualBBox = layoutContext.GetArea().GetBBox().Clone();
            float? blockWidth = RetrieveWidth(actualBBox.GetWidth());
            if (blockWidth != null) {
                actualBBox.SetWidth((float)blockWidth);
            }
            ContinuousContainer.SetupContinuousContainerIfNeeded(this);
            ApplyPaddings(actualBBox, false);
            ApplyBorderBox(actualBBox, false);
            ApplyMargins(actualBBox, false);
            Grid grid = ConstructGrid(this, actualBBox);
            GridContainerRenderer.GridLayoutResult layoutResult = LayoutGrid(layoutContext, actualBBox, grid);
            if (layoutResult.GetOverflowRenderers().IsEmpty()) {
                this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, grid, true);
                return new LayoutResult(LayoutResult.FULL, this.occupiedArea, null, null);
            }
            else {
                if (layoutResult.GetSplitRenderers().IsEmpty()) {
                    IRenderer cause = this;
                    if (!layoutResult.GetCauseOfNothing().IsEmpty()) {
                        cause = layoutResult.GetCauseOfNothing()[0];
                    }
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, cause);
                }
                else {
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, grid, false);
                    return new LayoutResult(LayoutResult.PARTIAL, this.occupiedArea, CreateSplitRenderer(layoutResult.GetSplitRenderers
                        ()), CreateOverflowRenderer(layoutResult.GetOverflowRenderers()));
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void AddChild(IRenderer renderer) {
            renderer.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            renderer.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            base.AddChild(renderer);
        }

        private AbstractRenderer CreateSplitRenderer(IList<IRenderer> children) {
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

        private AbstractRenderer CreateOverflowRenderer(IList<IRenderer> children) {
            // TODO DEVSIX-8340 - We put the original amount of rows into overflow container.
            iText.Layout.Renderer.GridContainerRenderer overflowRenderer = (iText.Layout.Renderer.GridContainerRenderer
                )GetNextRenderer();
            overflowRenderer.isFirstLayout = false;
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            overflowRenderer.SetChildRenderers(children);
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        //Process cells by doing actual layout on the calculated layout area
        private GridContainerRenderer.GridLayoutResult LayoutGrid(LayoutContext layoutContext, Rectangle actualBBox
            , Grid grid) {
            GridContainerRenderer.GridLayoutResult layoutResult = new GridContainerRenderer.GridLayoutResult();
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.ROW)) {
                // Calculate cell layout context by getting actual x and y on parent layout area for it
                LayoutContext cellContext = GetCellLayoutContext(layoutContext, actualBBox, cell);
                IRenderer cellToRender = cell.GetValue();
                cellToRender.SetProperty(Property.COLLAPSING_MARGINS, false);
                // Now set the height for the individual items
                // We know cell height upfront and this way we tell the element what it can occupy
                Rectangle cellBBox = cellContext.GetArea().GetBBox();
                if (!cellToRender.HasProperty(Property.HEIGHT)) {
                    Rectangle rectangleWithoutBordersMarginsPaddings = cellBBox.Clone();
                    if (cellToRender is AbstractRenderer) {
                        AbstractRenderer abstractCellRenderer = ((AbstractRenderer)cellToRender);
                        // We subtract margins/borders/paddings because we should take into account that
                        // borders/paddings/margins should also fit into a cell.
                        if (AbstractRenderer.IsBorderBoxSizing(cellToRender)) {
                            abstractCellRenderer.ApplyMargins(rectangleWithoutBordersMarginsPaddings, false);
                        }
                        else {
                            abstractCellRenderer.ApplyMarginsBordersPaddings(rectangleWithoutBordersMarginsPaddings, false);
                        }
                    }
                    cellToRender.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(rectangleWithoutBordersMarginsPaddings
                        .GetHeight()));
                }
                // Adjust cell BBox to the remaining part of the layout bbox
                // This way we can layout elements partially
                cellBBox.SetHeight(cellBBox.GetTop() - actualBBox.GetBottom()).SetY(actualBBox.GetY());
                LayoutResult cellResult = cellToRender.Layout(cellContext);
                if (cellResult.GetStatus() == LayoutResult.NOTHING) {
                    layoutResult.GetOverflowRenderers().Add(cellToRender);
                    layoutResult.GetCauseOfNothing().Add(cellResult.GetCauseOfNothing());
                }
                else {
                    // PARTIAL + FULL result handling
                    layoutResult.GetSplitRenderers().Add(cellToRender);
                    if (cellResult.GetStatus() == LayoutResult.PARTIAL) {
                        layoutResult.GetOverflowRenderers().Add(cellResult.GetOverflowRenderer());
                    }
                }
            }
            return layoutResult;
        }

        //Init cell layout context based on a parent context and calculated cell layout area from grid sizing algorithm.
        private static LayoutContext GetCellLayoutContext(LayoutContext layoutContext, Rectangle actualBBox, GridCell
             cell) {
            LayoutArea tempArea = layoutContext.GetArea().Clone();
            Rectangle cellLayoutArea = cell.GetLayoutArea();
            tempArea.GetBBox().SetX(actualBBox.GetX() + cellLayoutArea.GetX());
            tempArea.GetBBox().SetY(actualBBox.GetY() + actualBBox.GetHeight() - cellLayoutArea.GetHeight() - cellLayoutArea
                .GetY());
            tempArea.GetBBox().SetWidth(actualBBox.GetWidth());
            if (cellLayoutArea.GetWidth() > 0) {
                tempArea.GetBBox().SetWidth(cellLayoutArea.GetWidth());
            }
            tempArea.GetBBox().SetHeight(cellLayoutArea.GetHeight());
            return new LayoutContext(tempArea, layoutContext.GetMarginsCollapseInfo(), layoutContext.GetFloatRendererAreas
                (), layoutContext.IsClippedHeight());
        }

        //calculate grid container occupied area based on its width/height properties and cell layout areas
        private LayoutArea CalculateContainerOccupiedArea(LayoutContext layoutContext, Grid grid, bool isFull) {
            LayoutArea area = layoutContext.GetArea().Clone();
            float totalHeight = UpdateOccupiedHeight(grid.GetHeight(), isFull);
            area.GetBBox().SetHeight(totalHeight);
            Rectangle initialBBox = layoutContext.GetArea().GetBBox();
            area.GetBBox().SetY(initialBBox.GetY() + initialBBox.GetHeight() - area.GetBBox().GetHeight());
            RecalculateHeightAndWidthAfterLayout(area.GetBBox(), isFull);
            return area;
        }

        private void RecalculateHeightAndWidthAfterLayout(Rectangle bBox, bool isFull) {
            float? height = RetrieveHeight();
            if (height != null) {
                height = UpdateOccupiedHeight((float)height, isFull);
                float heightDelta = bBox.GetHeight() - (float)height;
                bBox.MoveUp(heightDelta);
                bBox.SetHeight((float)height);
            }
            float? blockWidth = RetrieveWidth(bBox.GetWidth());
            if (blockWidth != null) {
                bBox.SetWidth((float)blockWidth);
            }
        }

        private float UpdateOccupiedHeight(float initialHeight, bool isFull) {
            if (isFull) {
                initialHeight += SafelyRetrieveFloatProperty(Property.PADDING_BOTTOM);
                initialHeight += SafelyRetrieveFloatProperty(Property.MARGIN_BOTTOM);
                if (!this.HasOwnProperty(Property.BORDER) || this.GetProperty<Border>(Property.BORDER) == null) {
                    initialHeight += SafelyRetrieveFloatProperty(Property.BORDER_BOTTOM);
                }
            }
            initialHeight += SafelyRetrieveFloatProperty(Property.PADDING_TOP);
            initialHeight += SafelyRetrieveFloatProperty(Property.MARGIN_TOP);
            if (!this.HasOwnProperty(Property.BORDER) || this.GetProperty<Border>(Property.BORDER) == null) {
                initialHeight += SafelyRetrieveFloatProperty(Property.BORDER_TOP);
            }
            // isFirstLayout is necessary to handle the case when grid container laid out on more
            // than 2 pages, and on the last page layout result is full, but there is no bottom border
            float TOP_AND_BOTTOM = isFull && isFirstLayout ? 2 : 1;
            //If container laid out on more than 3 pages, then it is a page where there are no bottom and top borders
            if (!isFull && !isFirstLayout) {
                TOP_AND_BOTTOM = 0;
            }
            initialHeight += SafelyRetrieveFloatProperty(Property.BORDER) * TOP_AND_BOTTOM;
            return initialHeight;
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

        //Grid layout algorithm is based on a https://drafts.csswg.org/css-grid/#layout-algorithm
        private static Grid ConstructGrid(iText.Layout.Renderer.GridContainerRenderer renderer, Rectangle actualBBox
            ) {
            IList<GridValue> templateColumns = renderer.GetProperty<IList<GridValue>>(Property.GRID_TEMPLATE_COLUMNS);
            IList<GridValue> templateRows = renderer.GetProperty<IList<GridValue>>(Property.GRID_TEMPLATE_ROWS);
            GridFlow flow = renderer.GetProperty<GridFlow?>(Property.GRID_FLOW) == null ? GridFlow.ROW : (GridFlow)(renderer
                .GetProperty<GridFlow?>(Property.GRID_FLOW));
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                child.SetParent(renderer);
            }
            // 8. Placing Grid Items
            Grid grid = Grid.Builder.ForItems(renderer.GetChildRenderers()).Columns(templateColumns == null ? 1 : templateColumns
                .Count).Rows(templateRows == null ? 1 : templateRows.Count).Flow(flow).Build();
            GridValue columnAutoWidth = renderer.GetProperty<GridValue>(Property.GRID_AUTO_COLUMNS);
            GridValue rowAutoHeight = renderer.GetProperty<GridValue>(Property.GRID_AUTO_ROWS);
            float? columnGapProp = renderer.GetProperty<float?>(Property.COLUMN_GAP);
            float? rowGapProp = renderer.GetProperty<float?>(Property.ROW_GAP);
            float columnGap = columnGapProp == null ? 0f : (float)columnGapProp;
            float rowGap = rowGapProp == null ? 0f : (float)rowGapProp;
            // 12. Grid Layout Algorithm
            GridSizer gridSizer = new GridSizer(grid, templateColumns, templateRows, columnAutoWidth, rowAutoHeight, columnGap
                , rowGap, actualBBox);
            gridSizer.SizeGrid();
            return grid;
        }

        private sealed class GridLayoutResult {
            private readonly IList<IRenderer> splitRenderers = new List<IRenderer>();

            private readonly IList<IRenderer> overflowRenderers = new List<IRenderer>();

            private readonly IList<IRenderer> causeOfNothing = new List<IRenderer>();

            public GridLayoutResult() {
            }

            //default constructor
            public IList<IRenderer> GetSplitRenderers() {
                return splitRenderers;
            }

            public IList<IRenderer> GetOverflowRenderers() {
                return overflowRenderers;
            }

            public IList<IRenderer> GetCauseOfNothing() {
                return causeOfNothing;
            }
        }
    }
}
