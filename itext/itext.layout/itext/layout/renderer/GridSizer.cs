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
using iText.Layout.Properties.Grid;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    // 12.1. Grid Sizing Algorithm
    /// <summary>Class representing grid sizing algorithm.</summary>
    internal class GridSizer {
        private readonly Grid grid;

        private readonly IList<GridValue> templateColumns;

        private readonly IList<GridValue> templateRows;

        private readonly GridValue columnAutoWidth;

        private readonly GridValue rowAutoHeight;

        private readonly float columnGap;

        private readonly float rowGap;

        private readonly Rectangle actualBBox;

        private float containerHeight;

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates new grid sizer instance.</summary>
        /// <param name="grid">grid to size</param>
        /// <param name="templateColumns">template values for columns</param>
        /// <param name="templateRows">template values for rows</param>
        /// <param name="columnAutoWidth">value which used to size columns out of template range</param>
        /// <param name="rowAutoHeight">value which used to size rows out of template range</param>
        /// <param name="columnGap">gap size between columns</param>
        /// <param name="rowGap">gap size between rows</param>
        /// <param name="actualBBox">actual bbox which restricts sizing algorithm</param>
        internal GridSizer(iText.Layout.Renderer.Grid grid, IList<GridValue> templateColumns, IList<GridValue> templateRows
            , GridValue columnAutoWidth, GridValue rowAutoHeight, float columnGap, float rowGap, Rectangle actualBBox
            ) {
            this.grid = grid;
            this.templateColumns = templateColumns;
            this.templateRows = templateRows;
            this.columnAutoWidth = columnAutoWidth;
            this.rowAutoHeight = rowAutoHeight;
            this.columnGap = columnGap;
            this.rowGap = rowGap;
            this.actualBBox = actualBBox;
        }
//\endcond

        /// <summary>Resolves grid track sizes.</summary>
        public virtual void SizeGrid() {
            // 1. First, the track sizing algorithm is used to resolve the sizes of the grid columns.
            ResolveGridColumns();
            // 2. Next, the track sizing algorithm resolves the sizes of the grid rows.
            ResolveGridRows();
        }

        /// <summary>Gets grid container height.</summary>
        /// <remarks>
        /// Gets grid container height.
        /// Use this method only after calling
        /// <see cref="SizeGrid()"/>.
        /// </remarks>
        /// <returns>grid container height covered by row template</returns>
        public virtual float GetContainerHeight() {
            return containerHeight;
        }

        private void ResolveGridRows() {
            IList<GridValue> rowsValues = new List<GridValue>();
            for (int i = 0; i < grid.GetNumberOfRows(); i++) {
                if (templateRows != null && i < templateRows.Count) {
                    rowsValues.Add(templateRows[i]);
                }
                else {
                    if (rowAutoHeight != null) {
                        rowsValues.Add(rowAutoHeight);
                    }
                    else {
                        rowsValues.Add(AutoValue.VALUE);
                    }
                }
            }
            GridTrackSizer gridTrackSizer = new GridTrackSizer(grid, rowsValues, rowGap, actualBBox.GetHeight(), Grid.GridOrder
                .ROW);
            GridTrackSizer.TrackSizingResult result = gridTrackSizer.SizeTracks();
            IList<float> rows = result.GetTrackSizesAndExpandPercents(rowsValues);
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.ROW)) {
                float y = 0.0f;
                for (int currentRow = 0; currentRow < cell.GetRowStart(); ++currentRow) {
                    y += (float)rows[currentRow];
                    y += rowGap;
                }
                cell.GetLayoutArea().SetY(y);
                float cellHeight = 0.0f;
                float[] rowSizes = new float[cell.GetRowEnd() - cell.GetRowStart()];
                int rowSizesIdx = 0;
                for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                    rowSizes[rowSizesIdx] = (float)rows[i];
                    if (rowSizesIdx != 0) {
                        // We take into account only top gap and not bottom one
                        rowSizes[rowSizesIdx] += rowGap;
                    }
                    ++rowSizesIdx;
                    cellHeight += (float)rows[i];
                }
                //Preserve row sizes for split
                cell.SetRowSizes(rowSizes);
                cellHeight += (cell.GetGridHeight() - 1) * rowGap;
                cell.GetLayoutArea().SetHeight(cellHeight);
            }
            containerHeight = CalculateGridOccupiedHeight(result.GetTrackSizes());
        }

        /// <summary>Calculate grid container occupied area based on original (non-expanded percentages) track sizes.</summary>
        /// <param name="originalSizes">original track sizes</param>
        /// <returns>grid container occupied area</returns>
        private float CalculateGridOccupiedHeight(IList<float> originalSizes) {
            // Calculate explicit height to ensure that even empty rows which covered by template would be considered
            float minHeight = 0.0f;
            for (int i = 0; i < (templateRows == null ? 0 : templateRows.Count); ++i) {
                minHeight += (float)originalSizes[i];
            }
            float maxHeight = Sum(originalSizes);
            // Add gaps
            minHeight += (grid.GetNumberOfRows() - 1) * rowGap;
            maxHeight += (grid.GetNumberOfRows() - 1) * rowGap;
            float occupiedHeight = 0.0f;
            ICollection<GridCell> cells = grid.GetUniqueGridCells(Grid.GridOrder.ROW);
            foreach (GridCell cell in cells) {
                occupiedHeight = Math.Max(occupiedHeight, cell.GetLayoutArea().GetTop());
            }
            return Math.Max(Math.Min(maxHeight, occupiedHeight), minHeight);
        }

        private float Sum(IList<float> trackSizes) {
            float sum = 0.0f;
            foreach (float? size in trackSizes) {
                sum += (float)size;
            }
            return sum;
        }

        private void ResolveGridColumns() {
            IList<GridValue> colsValues = new List<GridValue>();
            for (int i = 0; i < grid.GetNumberOfColumns(); i++) {
                if (templateColumns != null && i < templateColumns.Count) {
                    colsValues.Add(templateColumns[i]);
                }
                else {
                    if (columnAutoWidth != null) {
                        colsValues.Add(columnAutoWidth);
                    }
                    else {
                        colsValues.Add(AutoValue.VALUE);
                    }
                }
            }
            GridTrackSizer gridTrackSizer = new GridTrackSizer(grid, colsValues, columnGap, actualBBox.GetWidth(), Grid.GridOrder
                .COLUMN);
            IList<float> columns = gridTrackSizer.SizeTracks().GetTrackSizesAndExpandPercents(colsValues);
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.COLUMN)) {
                float x = 0.0f;
                for (int currentColumn = 0; currentColumn < cell.GetColumnStart(); ++currentColumn) {
                    x += (float)columns[currentColumn];
                    x += columnGap;
                }
                cell.GetLayoutArea().SetX(x);
                float cellWidth = 0.0f;
                for (int i = cell.GetColumnStart(); i < cell.GetColumnEnd(); ++i) {
                    cellWidth += (float)columns[i];
                }
                cellWidth += (cell.GetGridWidth() - 1) * columnGap;
                cell.GetLayoutArea().SetWidth(cellWidth);
            }
        }
    }
//\endcond
}
