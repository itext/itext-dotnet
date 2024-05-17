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
using System.Linq;
using iText.Commons.Utils;
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
            //TODO DEVSIX-8329 improve nothing processing, consider checking for cause of nothing here?
            //TODO DEVSIX-8329 improve forced placement logic
            if (layoutResult.GetSplitRenderers().IsEmpty()) {
                if (true.Equals(this.GetProperty<bool?>(Property.FORCED_PLACEMENT))) {
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, grid, true);
                    return new LayoutResult(LayoutResult.FULL, this.occupiedArea, this, null);
                }
                IRenderer cause = this;
                if (!layoutResult.GetCauseOfNothing().IsEmpty()) {
                    cause = layoutResult.GetCauseOfNothing()[0];
                }
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, cause);
            }
            else {
                if (layoutResult.GetOverflowRenderers().IsEmpty()) {
                    ContinuousContainer continuousContainer = this.GetProperty<ContinuousContainer>(Property.TREAT_AS_CONTINUOUS_CONTAINER_RESULT
                        );
                    if (continuousContainer != null) {
                        continuousContainer.ReApplyProperties(this);
                    }
                    this.childRenderers.Clear();
                    this.AddAllChildRenderers(layoutResult.GetSplitRenderers());
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, grid, true);
                    return new LayoutResult(LayoutResult.FULL, this.occupiedArea, this, null);
                }
                else {
                    this.occupiedArea = CalculateContainerOccupiedArea(layoutContext, grid, false);
                    return new LayoutResult(LayoutResult.PARTIAL, this.occupiedArea, CreateSplitRenderer(layoutResult.GetSplitRenderers
                        ()), CreateOverflowRenderer(layoutResult.GetOverflowRenderers()));
                }
            }
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
            overflowRenderer.SetChildRenderers(children);
            ContinuousContainer.ClearPropertiesFromOverFlowRenderer(overflowRenderer);
            return overflowRenderer;
        }

        //Process cells by doing actual layout on the calculated layout area
        private GridContainerRenderer.GridLayoutResult LayoutGrid(LayoutContext layoutContext, Rectangle actualBBox
            , Grid grid) {
            GridContainerRenderer.GridLayoutResult layoutResult = new GridContainerRenderer.GridLayoutResult();
            EnsureTemplateValuesFit(grid, actualBBox);
            //TODO DEVSIX-8329 improve forced placement logic, right now if we have a cell which could not be fitted on its
            // area or returns nothing as layout result RootRenderer sets FORCED_PLACEMENT on this class instance.
            // And basically every cell inherits this value and force placed, but we only need to force place cells
            // which were not fitted originally.
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.ROW_ORDER)) {
                //If cell couldn't fit during cell layout area calculation than we need to put such cell straight to
                //nothing result list
                if (!cell.IsValueFitOnCellArea()) {
                    layoutResult.GetOverflowRenderers().Add(cell.GetValue());
                    layoutResult.GetCauseOfNothing().Add(cell.GetValue());
                    continue;
                }
                //Calculate cell layout context by getting actual x and y on parent layout area for it
                LayoutContext cellContext = GetCellLayoutContext(layoutContext, actualBBox, cell);
                //We need to check for forced placement here, because otherwise we would infinitely return partial result.
                if (!true.Equals(this.GetProperty<bool?>(Property.FORCED_PLACEMENT)) && !actualBBox.Contains(cellContext.GetArea
                    ().GetBBox())) {
                    layoutResult.GetOverflowRenderers().Add(cell.GetValue());
                    continue;
                }
                IRenderer cellToRender = cell.GetValue();
                cellToRender.SetProperty(Property.FILL_AVAILABLE_AREA, true);
                cellToRender.SetProperty(Property.COLLAPSING_MARGINS, false);
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

        private void EnsureTemplateValuesFit(Grid grid, Rectangle actualBBox) {
            if (grid.GetMinWidth() > actualBBox.GetWidth()) {
                actualBBox.SetWidth(grid.GetMinWidth());
            }
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

        //TODO DEVSIX-8330: Probably height also has to be calculated before layout and set into actual bbox
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

        //TODO DEVSIX-8330: Consider extracting this method and same from MulticolRenderer to a separate class
        // or derive GridRenderer and MulticolRenderer from one class which will manage this and isFirstLayout field
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
        //It's not a 1 to 1 implementation and Grid Sizing Algorithm differs a little bit
        //TODO DEVSIX-8324 Left actualBBox parameter since it will be needed for fr and % calculations
        // if it is inline-grid, than it won't be needed.
        private static Grid ConstructGrid(iText.Layout.Renderer.GridContainerRenderer renderer, Rectangle actualBBox
            ) {
            //TODO DEVSIX-8324 create a new class GridTemplateValue, which will store fr, pt, %, minmax, etc. values
            //TODO DEVSIX-8324 Use this new class instead of Float and use it inside Grid Sizing Algorithm
            //TODO DEVSIX-8324 Right now we're assuming that all template values are points, and there is no % and fr in this list
            IList<float> templateColumns = ProcessTemplateValues(renderer.GetProperty<IList<UnitValue>>(Property.GRID_TEMPLATE_COLUMNS
                ));
            IList<float> templateRows = ProcessTemplateValues(renderer.GetProperty<IList<UnitValue>>(Property.GRID_TEMPLATE_ROWS
                ));
            float? columnAutoWidth = renderer.GetProperty<UnitValue>(Property.GRID_AUTO_COLUMNS) == null ? null : (float?
                )((UnitValue)renderer.GetProperty<UnitValue>(Property.GRID_AUTO_COLUMNS)).GetValue();
            float? rowAutoHeight = renderer.GetProperty<UnitValue>(Property.GRID_AUTO_ROWS) == null ? null : (float?)(
                (UnitValue)renderer.GetProperty<UnitValue>(Property.GRID_AUTO_ROWS)).GetValue();
            //Grid Item Placement Algorithm
            int initialRowsCount = templateRows == null ? 1 : templateRows.Count;
            int initialColumnsCount = templateColumns == null ? 1 : templateColumns.Count;
            //Sort cells to first position anything that’s not auto-positioned, then process the items locked to a given row.
            //It would be better to use tree set here, but not using it due to .NET porting issues
            IList<GridCell> sortedCells = new List<GridCell>();
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                sortedCells.Add(new GridCell(child));
            }
            JavaCollectionsUtil.Sort(sortedCells, new GridContainerRenderer.CellComparator());
            //Find a cell with a max column end to ensure it will fit on the grid
            foreach (GridCell cell in sortedCells) {
                if (cell != null) {
                    initialColumnsCount = Math.Max(initialColumnsCount, cell.GetColumnEnd());
                }
            }
            Grid grid = new Grid(initialRowsCount, initialColumnsCount, false);
            foreach (GridCell cell in sortedCells) {
                cell.GetValue().SetParent(renderer);
                grid.AddCell(cell);
            }
            //TODO DEVSIX-8325 eliminate null rows/columns
            // for rows it's easy: grid.getCellsRows().removeIf(row -> row.stream().allMatch(cell -> cell == null));
            // shrinkNullAxis(grid);
            GridSizer gridSizer = new GridSizer(grid, templateRows, templateColumns, rowAutoHeight, columnAutoWidth);
            gridSizer.SizeCells();
            //calculating explicit height to ensure that even empty rows which covered by template would be considered
            //TODO DEVSIX-8324 improve those methods in future for working correctly with minmax/repeat/etc.
            SetGridContainerMinimalHeight(grid, templateRows);
            SetGridContainerMinimalWidth(grid, templateColumns);
            return grid;
        }

        //TODO DEVSIX-8324 This is temporary method, we should remove it and instead of having Property.GRID_TEMPLATE_...
        // as a UnitValue and returning list of Float, we need a new class which will be passed to Grid Sizing Algorithm.
        private static IList<float> ProcessTemplateValues(IList<UnitValue> template) {
            if (template == null) {
                return null;
            }
            return template.Select((value) => value.GetValue()).ToList();
        }

        //This method calculates container minimal height, because if number of cells is not enough to fill all specified
        //rows by template than we need to set the height of the container higher than it's actual occupied height.
        private static void SetGridContainerMinimalHeight(Grid grid, IList<float> templateRows) {
            float explicitContainerHeight = 0.0f;
            if (templateRows != null) {
                foreach (float? template in templateRows) {
                    explicitContainerHeight += (float)template;
                }
            }
            grid.SetMinHeight(explicitContainerHeight);
        }

        private static void SetGridContainerMinimalWidth(Grid grid, IList<float> templateColumns) {
            float explicitContainerWidth = 0.0f;
            if (templateColumns != null) {
                foreach (float? template in templateColumns) {
                    explicitContainerWidth += (float)template;
                }
            }
            grid.SetMinWidth(explicitContainerWidth);
        }

        /// <summary>
        /// This comparator sorts cells so ones with both fixed row and column positions would go first,
        /// then cells with fixed row and then cells without such properties.
        /// </summary>
        private sealed class CellComparator : IComparer<GridCell> {
            public int Compare(GridCell lhs, GridCell rhs) {
                int lhsModifiers = 0;
                if (lhs.GetColumnStart() != -1 && lhs.GetRowStart() != -1) {
                    lhsModifiers = 2;
                }
                else {
                    if (lhs.GetRowStart() != -1) {
                        lhsModifiers = 1;
                    }
                }
                int rhsModifiers = 0;
                if (rhs.GetColumnStart() != -1 && rhs.GetRowStart() != -1) {
                    rhsModifiers = 2;
                }
                else {
                    if (rhs.GetRowStart() != -1) {
                        rhsModifiers = 1;
                    }
                }
                //passing parameters in reversed order so ones with properties would come first
                return JavaUtil.IntegerCompare(rhsModifiers, lhsModifiers);
            }
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
