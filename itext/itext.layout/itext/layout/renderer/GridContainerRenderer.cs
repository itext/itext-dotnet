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
using iText.Layout.Properties.Grid;

namespace iText.Layout.Renderer {
    /// <summary>Represents a renderer for a grid.</summary>
    public class GridContainerRenderer : BlockRenderer {
        private bool isFirstLayout = true;

        private float containerHeight = 0.0f;

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
            //TODO DEVSIX-8331 enable continuous container
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
            float? blockHeight = RetrieveHeight();
            if (blockHeight != null && (float)blockHeight < actualBBox.GetHeight()) {
                actualBBox.SetY(actualBBox.GetY() + actualBBox.GetHeight() - (float)blockHeight);
                actualBBox.SetHeight((float)blockHeight);
            }
            Grid grid = ConstructGrid(this, new Rectangle(actualBBox.GetWidth(), blockHeight == null ? -1 : actualBBox
                .GetHeight()));
            GridContainerRenderer.GridLayoutResult layoutResult = LayoutGrid(layoutContext, actualBBox, grid);
            if (layoutResult.GetOverflowRenderers().IsEmpty()) {
                this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, grid, true);
                return new LayoutResult(LayoutResult.FULL, this.occupiedArea, null, null);
            }
            else {
                if (layoutResult.GetSplitRenderers().IsEmpty()) {
                    IRenderer cause = layoutResult.GetCauseOfNothing() == null ? this : layoutResult.GetCauseOfNothing();
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
            renderer.SetProperty(Property.COLLAPSING_MARGINS, DetermineCollapsingMargins(renderer));
            GridItemRenderer itemRenderer = new GridItemRenderer();
            itemRenderer.SetProperty(Property.COLLAPSING_MARGINS, false);
            itemRenderer.AddChild(renderer);
            base.AddChild(itemRenderer);
        }

        /// <summary>Calculates collapsing margins value.</summary>
        /// <remarks>
        /// Calculates collapsing margins value. It's based on browser behavior.
        /// Always returning true somehow also almost works.
        /// </remarks>
        private static bool? DetermineCollapsingMargins(IRenderer renderer) {
            IRenderer currentRenderer = renderer;
            while (!currentRenderer.GetChildRenderers().IsEmpty()) {
                if (currentRenderer.GetChildRenderers().Count > 1) {
                    return true;
                }
                else {
                    currentRenderer = currentRenderer.GetChildRenderers()[0];
                }
            }
            if (currentRenderer is TableRenderer) {
                return true;
            }
            return false;
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
            iText.Layout.Renderer.GridContainerRenderer overflowRenderer = (iText.Layout.Renderer.GridContainerRenderer
                )GetNextRenderer();
            overflowRenderer.isFirstLayout = false;
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            overflowRenderer.SetProperty(Property.GRID_TEMPLATE_ROWS, null);
            overflowRenderer.SetProperty(Property.GRID_AUTO_ROWS, null);
            overflowRenderer.SetChildRenderers(children);
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        //Process cells by doing actual layout on the calculated layout area
        private GridContainerRenderer.GridLayoutResult LayoutGrid(LayoutContext layoutContext, Rectangle actualBBox
            , Grid grid) {
            GridContainerRenderer.GridLayoutResult layoutResult = new GridContainerRenderer.GridLayoutResult();
            int notLayoutedRow = grid.GetNumberOfRows();
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.ROW)) {
                // Calculate cell layout context by getting actual x and y on parent layout area for it
                LayoutContext cellContext = GetCellLayoutContext(layoutContext, actualBBox, cell);
                Rectangle cellBBox = cellContext.GetArea().GetBBox();
                IRenderer cellToRender = cell.GetValue();
                // Now set the height for the individual items
                // We know cell height upfront and this way we tell the element what it can occupy
                float itemHeight = ((GridItemRenderer)cellToRender).CalculateHeight(cellBBox.GetHeight());
                cellToRender.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(itemHeight));
                // Adjust cell BBox to the remaining part of the layout bbox
                // This way we can lay out elements partially
                cellBBox.SetHeight(cellBBox.GetTop() - actualBBox.GetBottom()).SetY(actualBBox.GetY());
                cellToRender.SetProperty(Property.FILL_AVAILABLE_AREA, true);
                LayoutResult cellResult = cellToRender.Layout(cellContext);
                notLayoutedRow = Math.Min(notLayoutedRow, ProcessLayoutResult(layoutResult, cell, cellResult));
            }
            foreach (IRenderer overflowRenderer in layoutResult.GetOverflowRenderers()) {
                if (overflowRenderer.GetProperty<int?>(Property.GRID_ROW_START) != null) {
                    overflowRenderer.SetProperty(Property.GRID_ROW_START, (int)overflowRenderer.GetProperty<int?>(Property.GRID_ROW_START
                        ) - notLayoutedRow);
                    overflowRenderer.SetProperty(Property.GRID_ROW_END, (int)overflowRenderer.GetProperty<int?>(Property.GRID_ROW_END
                        ) - notLayoutedRow);
                }
            }
            return layoutResult;
        }

        private static int ProcessLayoutResult(GridContainerRenderer.GridLayoutResult layoutResult, GridCell cell, 
            LayoutResult cellResult) {
            IRenderer cellToRenderer = cell.GetValue();
            if (cellResult.GetStatus() == LayoutResult.NOTHING) {
                cellToRenderer.SetProperty(Property.GRID_COLUMN_START, cell.GetColumnStart() + 1);
                cellToRenderer.SetProperty(Property.GRID_COLUMN_END, cell.GetColumnEnd() + 1);
                cellToRenderer.SetProperty(Property.GRID_ROW_START, cell.GetRowStart() + 1);
                cellToRenderer.SetProperty(Property.GRID_ROW_END, cell.GetRowEnd() + 1);
                layoutResult.GetOverflowRenderers().Add(cellToRenderer);
                layoutResult.SetCauseOfNothing(cellResult.GetCauseOfNothing());
                return cell.GetRowStart();
            }
            // PARTIAL + FULL result handling
            layoutResult.GetSplitRenderers().Add(cellToRenderer);
            if (cellResult.GetStatus() == LayoutResult.PARTIAL) {
                IRenderer overflowRenderer = cellResult.GetOverflowRenderer();
                overflowRenderer.SetProperty(Property.GRID_COLUMN_START, cell.GetColumnStart() + 1);
                overflowRenderer.SetProperty(Property.GRID_COLUMN_END, cell.GetColumnEnd() + 1);
                int rowStart = cell.GetRowStart() + 1;
                int rowEnd = cell.GetRowEnd() + 1;
                layoutResult.GetOverflowRenderers().Add(overflowRenderer);
                // Now let's find out where we split exactly
                float accumulatedRowSize = 0;
                float layoutedHeight = cellResult.GetOccupiedArea().GetBBox().GetHeight();
                int notLayoutedRow = rowStart - 1;
                for (int i = 0; i < cell.GetRowSizes().Length; ++i) {
                    accumulatedRowSize += cell.GetRowSizes()[i];
                    if (accumulatedRowSize < layoutedHeight) {
                        ++rowStart;
                        ++notLayoutedRow;
                    }
                    else {
                        break;
                    }
                }
                // We don't know what to do if rowStart is equal or more than rowEnd
                // Let's not try to guess by just take the 1st available space in a column
                // by leaving nulls for grid-row-start/end
                if (rowEnd > rowStart) {
                    overflowRenderer.SetProperty(Property.GRID_ROW_START, rowStart);
                    overflowRenderer.SetProperty(Property.GRID_ROW_END, rowEnd);
                }
                return notLayoutedRow;
            }
            return int.MaxValue;
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

        // Calculate grid container occupied area based on its width/height properties and cell layout areas
        private LayoutArea CalculateContainerOccupiedArea(LayoutContext layoutContext, Grid grid, bool isFull) {
            LayoutArea area = layoutContext.GetArea().Clone();
            float totalHeight = UpdateOccupiedHeight(containerHeight, isFull);
            if (totalHeight < area.GetBBox().GetHeight() || isFull) {
                area.GetBBox().SetHeight(totalHeight);
                Rectangle initialBBox = layoutContext.GetArea().GetBBox();
                area.GetBBox().SetY(initialBBox.GetY() + initialBBox.GetHeight() - area.GetBBox().GetHeight());
                RecalculateHeightAndWidthAfterLayout(area.GetBBox(), isFull);
            }
            return area;
        }

        // Recalculate height/width after grid sizing and re-apply height/width properties
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

        // Grid layout algorithm is based on a https://drafts.csswg.org/css-grid/#layout-algorithm
        // This method creates grid, positions items on it and sizes grid tracks
        private static Grid ConstructGrid(iText.Layout.Renderer.GridContainerRenderer renderer, Rectangle actualBBox
            ) {
            float? columnGapProp = renderer.GetProperty<float?>(Property.COLUMN_GAP);
            float? rowGapProp = renderer.GetProperty<float?>(Property.ROW_GAP);
            float columnGap = columnGapProp == null ? 0f : (float)columnGapProp;
            float rowGap = rowGapProp == null ? 0f : (float)rowGapProp;
            // Resolving repeats
            GridTemplateResolver rowRepeatResolver = new GridTemplateResolver(actualBBox.GetHeight(), rowGap);
            GridTemplateResolver columnRepeatResolver = new GridTemplateResolver(actualBBox.GetWidth(), columnGap);
            IList<GridValue> templateRows = rowRepeatResolver.ResolveTemplate(renderer.GetProperty<IList<TemplateValue
                >>(Property.GRID_TEMPLATE_ROWS));
            IList<GridValue> templateColumns = columnRepeatResolver.ResolveTemplate(renderer.GetProperty<IList<TemplateValue
                >>(Property.GRID_TEMPLATE_COLUMNS));
            GridFlow flow = renderer.GetProperty<GridFlow?>(Property.GRID_FLOW) == null ? GridFlow.ROW : (GridFlow)(renderer
                .GetProperty<GridFlow?>(Property.GRID_FLOW));
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                child.SetParent(renderer);
            }
            // 8. Placing Grid Items
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(renderer.GetChildRenderers()).Columns(templateColumns
                 == null ? 1 : templateColumns.Count).Rows(templateRows == null ? 1 : templateRows.Count).Flow(flow).Build
                ();
            // Collapse any empty repeated tracks if auto-fit was used
            if (rowRepeatResolver.IsCollapseNullLines()) {
                templateRows = rowRepeatResolver.ShrinkTemplatesToFitSize(grid.CollapseNullLines(Grid.GridOrder.ROW, rowRepeatResolver
                    .GetFixedValuesCount()));
            }
            if (columnRepeatResolver.IsCollapseNullLines()) {
                templateColumns = columnRepeatResolver.ShrinkTemplatesToFitSize(grid.CollapseNullLines(Grid.GridOrder.COLUMN
                    , columnRepeatResolver.GetFixedValuesCount()));
            }
            // 12. Grid Layout Algorithm
            GridValue columnAutoWidth = renderer.GetProperty<GridValue>(Property.GRID_AUTO_COLUMNS);
            GridValue rowAutoHeight = renderer.GetProperty<GridValue>(Property.GRID_AUTO_ROWS);
            GridSizer gridSizer = new GridSizer(grid, templateColumns, templateRows, columnAutoWidth, rowAutoHeight, columnGap
                , rowGap, actualBBox);
            gridSizer.SizeGrid();
            renderer.containerHeight = gridSizer.GetContainerHeight();
            return grid;
        }

        private sealed class GridLayoutResult {
            private readonly IList<IRenderer> splitRenderers = new List<IRenderer>();

            private readonly IList<IRenderer> overflowRenderers = new List<IRenderer>();

            private IRenderer causeOfNothing;

            public GridLayoutResult() {
            }

            //default constructor
            public IList<IRenderer> GetSplitRenderers() {
                return splitRenderers;
            }

            public IList<IRenderer> GetOverflowRenderers() {
                return overflowRenderers;
            }

            public void SetCauseOfNothing(IRenderer causeOfNothing) {
                this.causeOfNothing = causeOfNothing;
            }

            public IRenderer GetCauseOfNothing() {
                return causeOfNothing;
            }
        }
    }
}
