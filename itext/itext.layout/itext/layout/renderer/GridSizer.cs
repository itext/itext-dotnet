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
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>This class is responsible for sizing grid elements and calculating their layout area for future layout process.
    ///     </summary>
    internal class GridSizer {
        private readonly Grid grid;

        //TODO DEVSIX-8326 since templates will have not only absoulute values, we're probably need to create
        // separate fields, something like rowsHeights, columnsWidths which will store absolute/calculated values.
        // replace all absolute value logic using this new fields
        private readonly IList<GridValue> templateRows;

        private readonly IList<GridValue> templateColumns;

        private readonly GridValue rowAutoHeight;

        private readonly GridValue columnAutoWidth;

        //TODO DEVSIX-8326 here should be a list/map of different resolvers
        private readonly GridSizer.SizeResolver sizeResolver;

        private readonly float columnGap;

        private readonly float rowGap;

        internal GridSizer(Grid grid, IList<GridValue> templateRows, IList<GridValue> templateColumns, GridValue rowAutoHeight
            , GridValue columnAutoWidth, float? columnGap, float? rowGap) {
            this.grid = grid;
            this.templateRows = templateRows;
            this.templateColumns = templateColumns;
            this.rowAutoHeight = rowAutoHeight;
            this.columnAutoWidth = columnAutoWidth;
            this.sizeResolver = new GridSizer.MinContentResolver(grid);
            this.columnGap = columnGap == null ? 0.0f : (float)columnGap;
            this.rowGap = rowGap == null ? 0.0f : (float)rowGap;
        }

        //Grid Sizing Algorithm
        /// <summary>This method simulates positioning of cells on the grid, by calculating their occupied area.</summary>
        /// <remarks>
        /// This method simulates positioning of cells on the grid, by calculating their occupied area.
        /// If there was enough place to fit the renderer value of a cell on the grid, then such cell will be marked as
        /// non-fit and will be added to nothing result during actual layout.
        /// </remarks>
        internal virtual void SizeCells() {
            //TODO DEVSIX-8326 if rowsAutoSize/columnsAutoSize == auto/fr/min-content/max-content we need to track the
            // corresponding values of the elements and update auto height/width after calculating each cell layout area
            // and if its changed than re-layout
            //Grid Sizing Algorithm
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.COLUMN)) {
                cell.GetLayoutArea().SetX(CalculateCellX(cell));
                cell.GetLayoutArea().SetWidth(CalculateCellWidth(cell));
            }
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.ROW)) {
                cell.GetLayoutArea().SetY(CalculateCellY(cell));
                cell.GetLayoutArea().SetHeight(CalculateCellHeight(cell));
            }
        }

        /// <summary>Calculate cell left upper corner y position.</summary>
        /// <param name="cell">cell to calculate starting position</param>
        /// <returns>left upper corner y value</returns>
        private float CalculateCellY(GridCell cell) {
            float y = 0.0f;
            int currentRow = 0;
            if (templateRows != null) {
                for (; currentRow < Math.Min(templateRows.Count, cell.GetRowStart()); ++currentRow) {
                    y += (float)templateRows[currentRow].GetAbsoluteValue();
                    y += rowGap;
                }
                if (currentRow == cell.GetRowStart()) {
                    return y;
                }
            }
            if (rowAutoHeight != null) {
                for (; currentRow < cell.GetRowStart(); ++currentRow) {
                    y += (float)rowAutoHeight.GetAbsoluteValue();
                    y += rowGap;
                }
                return y;
            }
            GridCell topNeighbor = grid.GetClosestTopNeighbor(cell);
            if (topNeighbor != null) {
                return topNeighbor.GetLayoutArea().GetTop() + rowGap;
            }
            return 0.0f;
        }

        /// <summary>Calculate cell left upper corner x position.</summary>
        /// <param name="cell">cell to calculate starting position</param>
        /// <returns>left upper corner x value</returns>
        private float CalculateCellX(GridCell cell) {
            float x = 0.0f;
            int currentColumn = 0;
            if (templateColumns != null) {
                for (; currentColumn < Math.Min(templateColumns.Count, cell.GetColumnStart()); ++currentColumn) {
                    x += (float)templateColumns[currentColumn].GetAbsoluteValue();
                    x += columnGap;
                }
                if (currentColumn == cell.GetColumnStart()) {
                    return x;
                }
            }
            if (columnAutoWidth != null) {
                for (; currentColumn < cell.GetColumnStart(); ++currentColumn) {
                    x += (float)columnAutoWidth.GetAbsoluteValue();
                    x += columnGap;
                }
                return x;
            }
            GridCell leftNeighbor = grid.GetClosestLeftNeighbor(cell);
            if (leftNeighbor != null) {
                if (leftNeighbor.GetColumnEnd() > cell.GetColumnStart()) {
                    x = leftNeighbor.GetLayoutArea().GetX() + leftNeighbor.GetLayoutArea().GetWidth() * ((float)(cell.GetColumnStart
                        () - leftNeighbor.GetColumnStart())) / leftNeighbor.GetGridWidth();
                }
                else {
                    x = leftNeighbor.GetLayoutArea().GetRight();
                }
                x += columnGap;
            }
            return x;
        }

        //TODO DEVSIX-8327 Calculate fr units of rowsAutoSize here
        //TODO DEVSIX-8327 currently the default behaviour when there is no templateRows or rowAutoHeight is
        // css grid-auto-columns: min-content analogue, the default one should be 1fr
        // (for rows they are somewhat similar, if there where no fr values in templateRows, then the
        // size of all subsequent rows is the size of the highest row after templateRows
        //TODO DEVSIX-8327 add a comment for this method and how it works (not done in the scope of previous ticket
        // because logic will be changed after fr value implementation)
        private float CalculateCellHeight(GridCell cell) {
            float cellHeight = 0.0f;
            //process cells with grid height > 1;
            if (cell.GetGridHeight() > 1) {
                int counter = 0;
                for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                    if (templateRows != null && i < templateRows.Count) {
                        ++counter;
                        cellHeight += (float)templateRows[i].GetAbsoluteValue();
                    }
                    else {
                        if (rowAutoHeight != null) {
                            ++counter;
                            cellHeight += (float)rowAutoHeight.GetAbsoluteValue();
                        }
                    }
                }
                if (counter > 1) {
                    cellHeight += rowGap * (counter - 1);
                }
                if (counter == cell.GetGridHeight()) {
                    return cellHeight;
                }
            }
            if (templateRows == null || cell.GetRowStart() >= templateRows.Count) {
                //TODO DEVSIX-8324 if row auto height value is fr or min-content do not return here
                if (rowAutoHeight != null) {
                    return (float)rowAutoHeight.GetAbsoluteValue();
                }
                cellHeight = sizeResolver.ResolveHeight(cell, cellHeight);
            }
            else {
                cellHeight = (float)templateRows[cell.GetRowStart()].GetAbsoluteValue();
            }
            return cellHeight;
        }

        //TODO DEVSIX-8327 currently the default behaviour when there is no templateColumns or columnAutoWidth is
        // css grid-auto-columns: min-content analogue, the default one should be 1fr
        private float CalculateCellWidth(GridCell cell) {
            float cellWidth = 0.0f;
            //process absolute values for wide cells
            if (cell.GetGridWidth() > 1) {
                int counter = 0;
                for (int i = cell.GetColumnStart(); i < cell.GetColumnEnd(); ++i) {
                    if (templateColumns != null && i < templateColumns.Count) {
                        ++counter;
                        cellWidth += (float)templateColumns[i].GetAbsoluteValue();
                    }
                    else {
                        if (columnAutoWidth != null) {
                            ++counter;
                            cellWidth += (float)columnAutoWidth.GetAbsoluteValue();
                        }
                    }
                }
                if (counter > 1) {
                    cellWidth += columnGap * (counter - 1);
                }
                if (counter == cell.GetGridWidth()) {
                    return cellWidth;
                }
            }
            if (templateColumns == null || cell.GetColumnEnd() > templateColumns.Count) {
                //TODO DEVSIX-8324 if row auto width value is fr or min-content do not return here
                if (columnAutoWidth != null) {
                    return (float)columnAutoWidth.GetAbsoluteValue();
                }
                cellWidth = sizeResolver.ResolveWidth(cell, cellWidth);
            }
            else {
                //process absolute template values
                cellWidth = (float)templateColumns[cell.GetColumnStart()].GetAbsoluteValue();
            }
            return cellWidth;
        }

        /// <summary>
        /// The
        /// <c>SizeResolver</c>
        /// is used to calculate cell width and height on layout area.
        /// </summary>
        protected internal abstract class SizeResolver {
            protected internal Grid grid;

            /// <summary>
            /// Create a new
            /// <c>SizeResolver</c>
            /// instance for the given
            /// <c>Grid</c>
            /// instance.
            /// </summary>
            /// <param name="grid">grid which cells sizes will be resolved</param>
            public SizeResolver(Grid grid) {
                this.grid = grid;
            }

            public abstract float ResolveHeight(GridCell cell, float explicitCellHeight);

            public abstract float ResolveWidth(GridCell cell, float explicitCellWidth);

            //TODO DEVSIX-8326 If we're getting LayoutResult = NOTHING (PARTIAL ? if it is even possible) / null occupied area
            // from this method, we need to return current default sizing for a grid.
            // For min-content - that's should be practically impossible but as a safe guard we can return height of any adjacent
            // item in a row
            // For fr (flex) unit - currently calculated flex value
            // For other relative unit this should be investigated
            // Basically this method will only be called for relative values of rowsAutoHeight and we need to carefully think
            // what to return if we failed to layout a cell item in a given space
            /// <summary>Calculate minimal cell height required for cell value to be laid out.</summary>
            /// <param name="cell">cell container in which cell value has to be laid out</param>
            /// <returns>required height for cell to be laid out</returns>
            protected internal virtual float CalculateImplicitCellHeight(GridCell cell) {
                cell.GetValue().SetProperty(Property.FILL_AVAILABLE_AREA, false);
                LayoutResult inifiniteHeighLayoutResult = cell.GetValue().Layout(new LayoutContext(new LayoutArea(1, new Rectangle
                    (cell.GetLayoutArea().GetWidth(), AbstractRenderer.INF))));
                if (inifiniteHeighLayoutResult.GetStatus() == LayoutResult.NOTHING || inifiniteHeighLayoutResult.GetStatus
                    () == LayoutResult.PARTIAL) {
                    cell.SetValueFitOnCellArea(false);
                    return -1;
                }
                return inifiniteHeighLayoutResult.GetOccupiedArea().GetBBox().GetHeight();
            }

            /// <summary>Calculate minimal cell width required for cell value to be laid out.</summary>
            /// <param name="cell">cell container in which cell value has to be laid out</param>
            /// <returns>required width for cell to be laid out</returns>
            protected internal virtual float CalculateMinRequiredCellWidth(GridCell cell) {
                cell.GetValue().SetProperty(Property.FILL_AVAILABLE_AREA, false);
                if (cell.GetValue() is AbstractRenderer) {
                    AbstractRenderer abstractRenderer = (AbstractRenderer)cell.GetValue();
                    return abstractRenderer.GetMinMaxWidth().GetMinWidth();
                }
                return -1;
            }
        }

        /// <summary>
        /// The
        /// <c>MinContentResolver</c>
        /// is used to calculate cell width and height on layout area by calculating their
        /// min required size.
        /// </summary>
        protected internal class MinContentResolver : GridSizer.SizeResolver {
            /// <summary><inheritDoc/></summary>
            public MinContentResolver(Grid grid)
                : base(grid) {
            }

            /// <summary><inheritDoc/></summary>
            public override float ResolveHeight(GridCell cell, float cellHeight) {
                float maxRowTop = grid.GetMaxRowTop(cell.GetRowStart(), cell.GetColumnStart());
                cellHeight = Math.Max(cellHeight, CalculateImplicitCellHeight(cell));
                if (maxRowTop >= cellHeight + cell.GetLayoutArea().GetY()) {
                    cellHeight = maxRowTop - cell.GetLayoutArea().GetY();
                }
                else {
                    grid.AlignRow(cell.GetRowEnd() - 1, cellHeight + cell.GetLayoutArea().GetY());
                }
                return cellHeight;
            }

            /// <summary><inheritDoc/></summary>
            public override float ResolveWidth(GridCell cell, float cellWidth) {
                float maxColumnRight = grid.GetMaxColumnRight(cell.GetRowStart(), cell.GetColumnStart());
                cellWidth = Math.Max(cellWidth, CalculateMinRequiredCellWidth(cell));
                if (maxColumnRight >= cellWidth + cell.GetLayoutArea().GetX()) {
                    cellWidth = maxColumnRight - cell.GetLayoutArea().GetX();
                }
                else {
                    grid.AlignColumn(cell.GetColumnEnd() - 1, cellWidth + cell.GetLayoutArea().GetX());
                }
                return cellWidth;
            }
        }
    }
}
